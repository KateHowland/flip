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
 * This file added by Keiron Nicholson on 10/03/2010 at 11:37.
 */

using System;
using NWN2Toolset.NWN2.Data;
using NWN2Toolset.NWN2.Data.ConversationData;
using NWN2Toolset.NWN2.Data.Instances;
using NWN2Toolset.NWN2.Data.TypedCollections;
using Sussex.Flip.Core;
using Sussex.Flip.Games.NeverwinterNightsTwo;
using Sussex.Flip.Games.NeverwinterNightsTwo.Utils;

namespace Sussex.Flip.Games.NeverwinterNightsTwo
{
	/// <summary>
	/// Translates Flip source code into NWScript (for NWN2),
	/// compiles the generated script, and attaches the result
	/// to a user-created Neverwinter Nights 2 module. 
	/// </summary>
	public class NWScriptAttacher : FlipAttacher
	{
		#region Fields
		
		protected GameInformation game;
		protected INwn2Session session;
		protected ModuleTextFile moduleTextFile;
		
		#endregion
		
		#region Properties
		
		public override GameInformation Game {
			get { return game; }
		}
		
		#endregion
		
		#region Constructors
		
		/// <summary>
		/// Constructs a new <see cref="NWScriptAttacher"/> instance.
		/// </summary>
		/// <param name="translator">The translator which will
		/// be used to translate scripts before attaching them.</param>
		/// <param name="session">The helper class used for creating,
		/// compiling and attaching scripts.</param>
		public NWScriptAttacher(FlipTranslator translator, INwn2Session session) : base(translator)
		{
			if (session == null) throw new ArgumentNullException("session");
			
			this.game = new GameInformation("Neverwinter Nights 2");
			this.session = session;
			this.moduleTextFile = new ModuleTextFile();
		}
		
		#endregion
		
		#region Methods
		
		/// <summary>
		/// Translates Flip source into NWScript, compiles it, 
		/// and attaches the results to a Neverwinter Nights 2 module.
		/// </summary>
		/// <param name="source">The Flip source to be compiled.</param>
		/// <param name="address">An address representing the location
		/// to attach this script to.</param>
		public override void Attach(FlipScript source, string address)
		{
			if (source == null) throw new ArgumentNullException("source");
			if (address == null) throw new ArgumentNullException("address");
			if (address == String.Empty) throw new ArgumentException("Must provide a valid address for attaching script.","address");
			
			if (!Nwn2ToolsetFunctions.ToolsetIsOpen()) throw new InvalidOperationException("Toolset must be open to attach scripts.");
			
			NWN2GameModule module = session.GetModule();
			if (module == null) {
				throw new InvalidOperationException("No module is currently open in the toolset.");
			}
						
			try {			
				string name = GetUnusedScriptName(source.Name);
					
				NWN2GameScript script = session.AddScript(name,source.Code);
				
				session.CompileScript(script);
				
				if (address.StartsWith("Conversation")) {
					
					Nwn2ConversationAddress convAddress = new Nwn2ConversationAddress(address);
										
					NWN2GameConversation conversation = session.GetConversation(convAddress.Conversation);
					
					if (conversation == null) {
						throw new ArgumentException("Conversation '" + conversation + "' was not found in current module.","address");
					}
					
					NWN2ConversationLine line = session.GetConversationLine(conversation,convAddress.LineID);
					
					if (line == null) {
						throw new ArgumentException("Line with ID " + convAddress.LineID + " was not found in current module.","address");
					}
					
					session.AttachScriptToConversation(script,line);
				}
				
				else {
					Nwn2Address nwn2Address = new Nwn2Address(address);
					
					if (!Scripts.IsEventRaiser(nwn2Address.TargetType)) {
						throw new ArgumentException("Cannot attach scripts to a " + nwn2Address.TargetType + ".");
					}
									
					if (nwn2Address.TargetType == Nwn2Type.Module) {
						session.AttachScriptToModule(script,nwn2Address.TargetSlot);
					}	
					
					else {					
						NWN2GameArea area = session.GetArea(nwn2Address.AreaTag);
						if (area == null) {
							throw new ArgumentException("Area '" + nwn2Address.AreaTag + "' was not found in current module.","address");
						}
						
						if (nwn2Address.TargetType == Nwn2Type.Area) {					
							session.AttachScriptToArea(script,area,nwn2Address.TargetSlot);
						}
						
						else {
							NWN2Toolset.NWN2.Data.Templates.NWN2ObjectType nwn2ObjectType = Nwn2ScriptSlot.GetObjectType(nwn2Address.TargetType).Value;
							
							NWN2InstanceCollection instances = session.GetObjectsByTag(area,nwn2ObjectType,nwn2Address.InstanceTag);
							
							if (instances.Count == 0) {
								string error = String.Format("No objects of the given type ({0}) and tag ('{1}') were found in area '{2}'.",
								                             nwn2Address.TargetType,
								                             nwn2Address.InstanceTag,
								                             nwn2Address.AreaTag);
								throw new ArgumentException(error,"address");
							}
							
							if (nwn2Address.UseIndex) {	
								int count = instances.Count;
								
								if (nwn2Address.Index >= count) {
									string error = String.Format("Found only {0} objects of the given type ({1}) and tag ('{2}') in area '{3}' - could not assign to index [{4}].",
									                             count,
									                             nwn2Address.TargetType,
									                             nwn2Address.InstanceTag,
									                             nwn2Address.AreaTag,
									                             nwn2Address.Index);
									throw new ArgumentException(error,"address");
								}
								
								else {
									INWN2Instance instance = instances[nwn2Address.Index];
									session.AttachScriptToObject(script,instance,nwn2Address.TargetSlot);
								}
							}
							
							else {													
								foreach (INWN2Instance instance in instances) {
									session.AttachScriptToObject(script,instance,nwn2Address.TargetSlot);
								}
							}
						}
					}
				}
			}
			catch (Exception e) {
				throw new ApplicationException("Failed to translate and attach script.\n\n" + e.ToString(),e);
			}
		}
		
		
		/// <summary>
		/// Get a script name which has not already been taken in the current module.
		/// </summary>
		/// <param name="preferred">The preferred name for the script,
		/// which will be amended based on availability.</param>
		/// <returns>An available script name.</returns>
		public string GetUnusedScriptName(string preferred)
		{
			if (preferred == null) throw new ArgumentNullException("preferred");
			
			string name = preferred;			
			int count = 2;
			
			while (session.HasUncompiled(name)) {
				name = preferred + " (" + count++ + ")";
			}
			
			return name;
		}
		
		#endregion
	}
}
