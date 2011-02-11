/*
 * Flip - a visual programming language for scripting video games
 * Copyright (C) 2009, 2010 University of Sussex
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 *
 * To contact the authors of this program, email flip@sussex.ac.uk.
 *
 * You can also write to Keiron Nicholson at the School of Informatics, 
 * University of Sussex, Sussex House, Brighton, BN1 9RH, United Kingdom.
 * 
 * This file added by Keiron Nicholson on 04/06/2010 at 13:32.
 */

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using Sussex.Flip.Core;
using Sussex.Flip.Games.NeverwinterNightsTwo;
using Sussex.Flip.Games.NeverwinterNightsTwo.Integration;
using Sussex.Flip.Games.NeverwinterNightsTwo.Utils;
using Sussex.Flip.UI;
using NWN2Toolset;
using NWN2Toolset.NWN2;
using NWN2Toolset.NWN2.Data;
using NWN2Toolset.NWN2.Data.Blueprints;
using NWN2Toolset.NWN2.Data.ConversationData;
using NWN2Toolset.NWN2.Data.Instances;
using NWN2Toolset.NWN2.Data.Templates;
using NWN2Toolset.NWN2.Data.TypedCollections;
using OEIShared.IO;

namespace Sussex.Flip.Games.NeverwinterNightsTwo
{
	/// <summary>
	/// Description of ScriptHelper.
	/// </summary>
	public class ScriptHelper
	{
		protected Nwn2TriggerFactory triggerFactory;
		protected Nwn2AddressFactory addressFactory;
		protected Nwn2Session session;
		protected NarrativeThreadsHelper nt;
		
		
		public ScriptHelper(Nwn2TriggerFactory triggerFactory)
		{
			if (triggerFactory == null) throw new ArgumentNullException("triggerFactory");
			
			this.triggerFactory = triggerFactory;
			this.addressFactory = new Nwn2AddressFactory();
			this.session = new Nwn2Session();
			this.nt = new NarrativeThreadsHelper();
		}
		
		
		public static bool WasCreatedByFlip(NWN2GameScript script)
		{
			if (script == null) throw new ArgumentNullException("script");
			
			return script.Name.StartsWith("flip");
		}
		
		
		public static bool HasFlipScriptAttachedAsAction(NWN2ConversationConnector line)
		{
			if (line == null) throw new ArgumentNullException("line");
			
			try {
				if (line.Actions.Count > 0) {
					NWN2GameScript script = new NWN2GameScript(line.Actions[0].Script);
					return WasCreatedByFlip(script);
				}
			}
			catch (Exception) {}
			
			return false;
		}
		
		
		public static bool HasFlipScriptAttachedAsCondition(NWN2ConversationConnector line)
		{
			if (line == null) throw new ArgumentNullException("line");
			
			try {
				if (line.Conditions.Count > 0) {
					NWN2GameScript script = new NWN2GameScript(line.Conditions[0].Script);
					return WasCreatedByFlip(script);
				}
			}
			catch (Exception) {}
			
			return false;
		}
		
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="script"></param>
		/// <returns></returns>
		/// <remarks>This method expects that the NWN2GameScript is already Loaded
		/// (that is, the responsibility for calling Demand() falls to the client.)</remarks>
		public TriggerControl GetTrigger(NWN2GameScript script)
		{
			if (script == null) throw new ArgumentNullException("script");
			
			string code, address, naturalLanguage;
			ScriptWriter.ParseNWScript(script.Data, out code, out address, out naturalLanguage);				
				
			Nwn2Address nwn2Address = Nwn2Address.TryCreate(address);
			Nwn2ConversationAddress nwn2ConversationAddress = Nwn2ConversationAddress.TryCreate(address);
			
			if (nwn2Address != null) return triggerFactory.GetTriggerFromAddress(nwn2Address);
			
			else if (nwn2ConversationAddress != null) return triggerFactory.GetTriggerFromAddress(nwn2ConversationAddress);
			
			else {
				throw new ArgumentException("Address must represent a valid Nwn2Address or Nwn2ConversationAddress.","address");
			}
		}
		
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="script"></param>
		/// <returns></returns>
		/// <remarks>This method expects that the NWN2GameScript is already Loaded
		/// (that is, the responsibility for calling Demand() falls to the client.)</remarks>
		public FlipScript GetFlipScript(NWN2GameScript script)
		{
			if (script == null) throw new ArgumentNullException("script");
			
			string code, address, naturalLanguage;
			ScriptWriter.ParseNWScript(script.Data, out code, out address, out naturalLanguage);		
			
			ScriptType scriptType = GetScriptType(script);
			
			return new FlipScript(code,scriptType,script.Name);
		}
		
		
		public List<FlipScript> GetFlipScripts(NWN2GameScriptDictionary dict)
		{
			if (dict == null) throw new ArgumentNullException("dict");
			
			List<FlipScript> flipScripts = new List<FlipScript>(dict.Count);
			
			foreach (NWN2GameScript script in dict.Values) {
				try {
					FlipScript fs = GetFlipScript(script);
					flipScripts.Add(fs);
				}
				catch (Exception) {}
			}
			
			return flipScripts;
		}
		
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="dict"></param>
		/// <param name="attachment"></param>
		/// <returns></returns>
		/// <remarks>Note that this automatically opens and closes areas/conversations etc. - it is only
		/// for use with analysis methods, not for users.</remarks>
		public List<FlipScript> GetAllScriptsFromModule(Attachment attachment)
		{			
			NWN2GameModule mod = session.GetModule();
			if (mod == null) throw new InvalidOperationException("No module is open.");
			
			NWN2GameScriptDictionary dict = mod.Scripts;
			
			if (attachment == Attachment.Ignore) return GetFlipScripts(dict);
			
			Dictionary<Nwn2Address,FlipScript> moduleScripts = new Dictionary<Nwn2Address,FlipScript>();
			Dictionary<Nwn2Address,FlipScript> areaScripts = new Dictionary<Nwn2Address,FlipScript>();
			Dictionary<Nwn2ConversationAddress,FlipScript> conversationScripts = new Dictionary<Nwn2ConversationAddress,FlipScript>();
						
			foreach (NWN2GameScript nwn2Script in dict.Values) {
				
				try {					
					nwn2Script.Demand();
					
					string code, address, naturalLanguage;
					ScriptWriter.ParseNWScript(nwn2Script.Data, out code, out address, out naturalLanguage);		
					
					ScriptType scriptType = GetScriptType(nwn2Script);
					
					FlipScript script = new FlipScript(code,scriptType,nwn2Script.Name);
					
					Nwn2ConversationAddress ca = Nwn2ConversationAddress.TryCreate(address);
					if (ca != null) {
						conversationScripts.Add(ca,script);
					}
					
					else {
						Nwn2Address a = Nwn2Address.TryCreate(address);
						
						if (a != null) {
							
							if (a.TargetType == Nwn2Type.Module) moduleScripts.Add(a,script);
							
							else areaScripts.Add(a,script);
						}
					}
					
					nwn2Script.Release();
				}
				catch (Exception) {}
				
			}
			
			List<FlipScript> scripts = new List<FlipScript>(dict.Count); // this is what we'll return
							
			if (attachment == Attachment.AttachedToConversation || attachment == Attachment.Attached) {
				
				// Index by conversation name, so we can check all the scripts for a conversation in one go.
				
				Dictionary<string,List<Nwn2ConversationAddress>> convNameIndex = new Dictionary<string,List<Nwn2ConversationAddress>>();
				
				foreach (Nwn2ConversationAddress address in conversationScripts.Keys) {		
					
					if (!convNameIndex.ContainsKey(address.Conversation)) convNameIndex.Add(address.Conversation,new List<Nwn2ConversationAddress>());	
					
					convNameIndex[address.Conversation].Add(address);						
				}
				
				foreach (string convName in convNameIndex.Keys) {
					
					NWN2GameConversation conv = mod.Conversations[convName];
					
					if (conv == null) continue;
					
					conv.Demand();	
					
					foreach (Nwn2ConversationAddress address in convNameIndex[convName]) {
						
						FlipScript script = conversationScripts[address];
			
						NWN2ConversationLine line = conv.GetLineFromGUID(address.LineID);
						
						if (line == null) continue;
						
						if (address.AttachedAs == ScriptType.Standard && line.Actions.Count > 0) {
												
							IResourceEntry resource = line.Actions[0].Script;
							if (resource != null && resource.ResRef.Value == script.Name) {
								scripts.Add(script);
							}	
							
						}
						
						else if (address.AttachedAs == ScriptType.Conditional && line.OwningConnector.Conditions.Count > 0) {
														
							IResourceEntry resource = line.OwningConnector.Conditions[0].Script;
							if (resource != null && resource.ResRef.Value == script.Name) {
								scripts.Add(script);
							}	
						
						}
					}
					
					conv.Release();
				}
				
			}
			
			if (attachment == Attachment.AttachedToScriptSlot || attachment == Attachment.Attached) {
				
				// First, just check for scripts attached to the module - easy:
				
				foreach (Nwn2Address address in moduleScripts.Keys) {
											
					FlipScript script = moduleScripts[address];
					
					IResourceEntry resource = typeof(NWN2ModuleInformation).GetProperty(address.TargetSlot).GetValue(mod.ModuleInfo,null) as IResourceEntry;					
					
					if (resource != null && resource.ResRef.Value == script.Name) {
						scripts.Add(script);
					}
					
				}		
				
				
				// Next, check whether any script is attached to a Narrative Threads blueprint, and 
				// if you find one, add it to the collection and remove it from consideration:
					
				List<Nwn2Address> processed = new List<Nwn2Address>();
				
				foreach (Nwn2Address address in areaScripts.Keys) {
					
					FlipScript script = areaScripts[address];	
					
					// First check that if a blueprint which uses this tag as resref exists, it was created by Narrative Threads:												
					if (nt.CreatedByNarrativeThreads(address.TargetType,address.InstanceTag)) {
						
						NWN2ObjectType objectType = Nwn2ScriptSlot.GetObjectType(address.TargetType).Value;							
						INWN2Blueprint blueprint = session.GetBlueprint(address.InstanceTag.ToLower(),objectType);	
						
						if (blueprint != null) {			
							
							IResourceEntry resource = blueprint.GetType().GetProperty(address.TargetSlot).GetValue(blueprint,null) as IResourceEntry;
							
							if (resource != null && resource.ResRef.Value == script.Name) {
								scripts.Add(script);
								processed.Add(address);
							}								
						}
					}				
				}					
				
				foreach (Nwn2Address p in processed) areaScripts.Remove(p); // has been added, so don't check for it more than once
					
				
				// Then, open each area in turn, and see whether a script is attached to it. If it is, add
				// it to the collection and remove it from consideration:
				
				foreach (NWN2GameArea area in mod.Areas.Values) {
					
					if (areaScripts.Count == 0) break;
					
					session.OpenArea(area);
					
					processed = new List<Nwn2Address>();
					
					foreach (Nwn2Address address in areaScripts.Keys) {	
						
						FlipScript script = areaScripts[address];
						
						if (address.TargetType == Nwn2Type.Area) {
										
							IResourceEntry resource = typeof(NWN2GameArea).GetProperty(address.TargetSlot).GetValue(area,null) as IResourceEntry;	
							
							if (resource != null && resource.ResRef.Value == script.Name) {
								scripts.Add(script);
								processed.Add(address); // don't check whether to add it more than once
							}
							
						}
						
						else {
							
							NWN2InstanceCollection instances = session.GetObjectsByAddressInArea(address,area.Tag);
							
							if (instances != null) {
															
								// Check the script is still attached to the target object in the slot it claims to be,
								// in at least one instance, and if so add it to the set of scripts to be opened:
														
								foreach (INWN2Instance instance in instances) {		
									
									IResourceEntry resource = instance.GetType().GetProperty(address.TargetSlot).GetValue(instance,null) as IResourceEntry;						
									
									if (resource != null && resource.ResRef.Value == script.Name) {	
										scripts.Add(script);
										processed.Add(address);
										break;
									}
								}
							}								
						}							
					}
					
					foreach (Nwn2Address p in processed) areaScripts.Remove(p); // has been added, so don't check for it more than once
					
					session.CloseArea(area);
					
				}			
			}				
				
			return scripts;
		}
										
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="script"></param>
		/// <param name="attachment">Only return a script if it is attached
		/// in the manner indicated by this parameter.</param>
		/// <returns></returns>
		/// <remarks>This method expects that the NWN2GameScript is already Loaded
		/// (that is, the responsibility for calling Demand() falls to the client.)</remarks>
		public FlipScript GetFlipScript(NWN2GameScript script, Attachment attachment)
		{
			if (script == null) throw new ArgumentNullException("script");
			
			string code, address, naturalLanguage;
			ScriptWriter.ParseNWScript(script.Data, out code, out address, out naturalLanguage);		
			
			ScriptType scriptType = GetScriptType(script);
			
			if (attachment == Attachment.Ignore) return new FlipScript(code,scriptType,script.Name);
			
			NWN2GameModule mod = session.GetModule();					
			if (mod == null) throw new InvalidOperationException("No module is open.");
						
			Nwn2Address nwn2Address;
			Nwn2ConversationAddress nwn2ConversationAddress;
			
			switch (attachment) {
				case Attachment.Attached:
					nwn2Address = Nwn2Address.TryCreate(address);
					nwn2ConversationAddress = Nwn2ConversationAddress.TryCreate(address);					
					break;
					
				case Attachment.AttachedToConversation:
					nwn2Address = null;
					nwn2ConversationAddress = Nwn2ConversationAddress.TryCreate(address);		
					break;
					
				case Attachment.AttachedToScriptSlot:
					nwn2Address = Nwn2Address.TryCreate(address);
					nwn2ConversationAddress = null;	
					break;
					
				default:
					nwn2Address = null;
					nwn2ConversationAddress = null;
					break;
			}
						
			if (nwn2Address != null) {
					
				try {				
					if (nwn2Address.TargetType == Nwn2Type.Module) {	
						
						IResourceEntry resource = typeof(NWN2ModuleInformation).GetProperty(nwn2Address.TargetSlot).GetValue(mod.ModuleInfo,null) as IResourceEntry;					
						if (resource != null && resource.ResRef.Value == script.Name) {
							return new FlipScript(code,scriptType,script.Name);
						}
					}
					
					else if (nwn2Address.TargetType == Nwn2Type.Area) {					
						NWN2GameArea area = session.GetArea(nwn2Address.AreaTag);
						
						if (area != null) {						
							IResourceEntry resource = typeof(NWN2GameArea).GetProperty(nwn2Address.TargetSlot).GetValue(area,null) as IResourceEntry;						
							if (resource != null && resource.ResRef.Value == script.Name) {
								return new FlipScript(code,scriptType,script.Name);
							}
						}
					}
					
					else {
						
						/*
						 * The script might be attached to a Narrative Threads blueprint:
						 */ 				
						
						// First check that if a blueprint which uses this tag as resref exists, it was created by Narrative Threads:
												
						if (nt.CreatedByNarrativeThreads(nwn2Address.TargetType,nwn2Address.InstanceTag)) {
							
							NWN2ObjectType objectType = Nwn2ScriptSlot.GetObjectType(nwn2Address.TargetType).Value;							
							INWN2Blueprint blueprint = session.GetBlueprint(nwn2Address.InstanceTag.ToLower(),objectType);	
							
							if (blueprint != null) {			
								IResourceEntry resource = blueprint.GetType().GetProperty(nwn2Address.TargetSlot).GetValue(blueprint,null) as IResourceEntry;
								if (resource != null && resource.ResRef.Value == script.Name) {
									return new FlipScript(code,scriptType,script.Name);
								}								
							}
						}
						
						// Check through all OPEN areas.
						foreach (NWN2GameArea area in mod.Areas.Values) { 
						
							if (!session.AreaIsOpen(area)) continue;
							
							NWN2InstanceCollection instances = session.GetObjectsByAddressInArea(nwn2Address,area.Tag);
							
							if (instances != null) {
							
								// Check the script is still attached to the target object in the slot it claims to be,
								// in at least one instance, and if so add it to the set of scripts to be opened:
														
								foreach (INWN2Instance instance in instances) {							
									IResourceEntry resource = instance.GetType().GetProperty(nwn2Address.TargetSlot).GetValue(instance,null) as IResourceEntry;						
									if (resource != null && resource.ResRef.Value == script.Name) {
										return new FlipScript(code,scriptType,script.Name);
									}
								}
							}
						}
					}
				}
					
				catch (Exception x) {
					throw new ApplicationException(String.Format("Failed to handle script + '{0}' properly.",script.Name),x);
				}
			}
			
			else if (nwn2ConversationAddress != null) { // Check through all conversations, regardless of whether they're open.
				
				NWN2GameConversation conversation = session.GetConversation(nwn2ConversationAddress.Conversation);
				
				if (conversation == null) return null;
				
				if (NWN2Toolset.NWN2ToolsetMainForm.App.GetViewerForResource(conversation) == null) {
					conversation.Demand();
				}
				
				NWN2ConversationLine line = conversation.GetLineFromGUID(nwn2ConversationAddress.LineID);
				
				if (line == null) return null;
				
				if (line.Actions.Count == 0) return null;
				
				IResourceEntry resource = line.Actions[0].Script;
				if (resource != null && resource.ResRef.Value == script.Name) {
					return new FlipScript(code,scriptType,script.Name);
				}
			}
			
			return null;
		}	
		
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="script"></param>
		/// <returns></returns>
		/// <remarks>This method expects that the NWN2GameScript is already Loaded
		/// (that is, the responsibility for calling Demand() falls to the client.)</remarks>
		public ScriptType GetScriptType(NWN2GameScript script)
		{
			if (script == null) throw new ArgumentNullException("script");
						
			if (script.Data.Contains("int StartingConditional()")) return ScriptType.Conditional;
			else if (script.Data.Contains("void main()")) return ScriptType.Standard;
			else throw new ArgumentException("Format of script was not recognised.","script");
		}
		
					
		public List<ScriptTriggerTuple> GetAllScripts(Attachment attachment)
		{			
			NWN2GameModule mod = NWN2ToolsetMainForm.App.Module;
			
			if (mod == null) throw new InvalidOperationException("No module is open.");
			
			List<ScriptTriggerTuple> tuples = new List<ScriptTriggerTuple>();
			
			foreach (NWN2GameScript script in mod.Scripts.Values) {
				
				if (!WasCreatedByFlip(script)) continue;
				
				try {
				
					script.Demand();
					
					FlipScript flipScript = GetFlipScript(script,attachment);
									
					if (flipScript != null) {
						
						TriggerControl trigger;
						
						if (attachment == Attachment.Ignore) trigger = new BlankTrigger(flipScript.Name);
						
						else trigger = GetTrigger(script);
						
						tuples.Add(new ScriptTriggerTuple(flipScript,trigger));
					}
				}
				catch (Exception x) {
					MessageBox.Show("Something went wrong when trying to add " + script.Name + " to the set of openable scripts.\n\n" + x);
				}
			}
			
			return tuples;
		}
	}
}
