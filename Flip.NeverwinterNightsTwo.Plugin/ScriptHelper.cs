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
using Sussex.Flip.Games.NeverwinterNightsTwo.Utils;
using Sussex.Flip.UI;
using NWN2Toolset;
using NWN2Toolset.NWN2.Data;
using NWN2Toolset.NWN2.Data.ConversationData;
using NWN2Toolset.NWN2.Data.Instances;
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
		
		
		public ScriptHelper(Nwn2TriggerFactory triggerFactory)
		{
			if (triggerFactory == null) throw new ArgumentNullException("triggerFactory");
			
			this.triggerFactory = triggerFactory;
			this.addressFactory = new Nwn2AddressFactory();
			this.session = new Nwn2Session();
		}
		
		
		public static bool WasCreatedByFlip(NWN2GameScript script)
		{
			if (script == null) throw new ArgumentNullException("script");
			
			return script.Name.StartsWith("flipscript");
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
			
			string code, address;
			ScriptWriter.ExtractFlipCodeFromNWScript(script.Data, out code, out address);				
				
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
		public FlipScript GetFlipScript(NWN2GameScript script, bool onlyReturnIfScriptIsAttached)
		{
			if (script == null) throw new ArgumentNullException("script");
			
			string code, address;
			ScriptWriter.ExtractFlipCodeFromNWScript(script.Data, out code, out address);
			
			if (!onlyReturnIfScriptIsAttached) return new FlipScript(code,script.Name);
			
			NWN2GameModule mod = session.GetModule();					
			if (mod == null) throw new InvalidOperationException("No module is open.");
				
			Nwn2Address nwn2Address = Nwn2Address.TryCreate(address);
			Nwn2ConversationAddress nwn2ConversationAddress = Nwn2ConversationAddress.TryCreate(address);
			
			if (nwn2Address != null) {
					
				try {				
					if (nwn2Address.TargetType == Nwn2Type.Module) {	
						
						IResourceEntry resource = typeof(NWN2ModuleInformation).GetProperty(nwn2Address.TargetSlot).GetValue(mod.ModuleInfo,null) as IResourceEntry;					
						if (resource != null && resource.ResRef.Value == script.Name) {
							return new FlipScript(code,script.Name);
						}
					}
					
					else if (nwn2Address.TargetType == Nwn2Type.Area) {					
						NWN2GameArea area = session.GetArea(nwn2Address.AreaTag);
						
						if (area != null) {						
							IResourceEntry resource = typeof(NWN2GameArea).GetProperty(nwn2Address.TargetSlot).GetValue(area,null) as IResourceEntry;						
							if (resource != null && resource.ResRef.Value == script.Name) {
								return new FlipScript(code,script.Name);
							}
						}
					}
					
					else {
						
						foreach (NWN2GameArea area in mod.Areas.Values) { // Check through all OPEN areas.
						
							if (!session.AreaIsOpen(area)) continue;
							
							NWN2InstanceCollection instances = session.GetObjectsByAddressInArea(nwn2Address,area.Tag);
							
							if (instances != null) {
							
								// Check the script is still attached to the target object in the slot it claims to be,
								// in at least one instance, and if so add it to the set of scripts to be opened:
														
								foreach (INWN2Instance instance in instances) {							
									IResourceEntry resource = instance.GetType().GetProperty(nwn2Address.TargetSlot).GetValue(instance,null) as IResourceEntry;						
									if (resource != null && resource.ResRef.Value == script.Name) {
										return new FlipScript(code,script.Name);
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
					return new FlipScript(code,script.Name);
				}
			}
			
			else {
				throw new ArgumentException("Address must represent a valid Nwn2Address or Nwn2ConversationAddress.","address");
			}
			
			return null;
		}
		
									
		public List<ScriptTriggerTuple> GetAllScripts()
		{			
			NWN2GameModule mod = NWN2ToolsetMainForm.App.Module;
			
			if (mod == null) throw new InvalidOperationException("No module is open.");
			
			List<ScriptTriggerTuple> tuples = new List<ScriptTriggerTuple>();
			
			foreach (NWN2GameScript script in mod.Scripts.Values) {
				
				if (!WasCreatedByFlip(script)) continue;
				
				script.Demand();
				
				FlipScript flipScript = GetFlipScript(script,true);
								
				if (flipScript != null) {
					TriggerControl trigger = GetTrigger(script);
					tuples.Add(new ScriptTriggerTuple(flipScript,trigger));
				}
			}
			
			return tuples;
		}
	}
}
