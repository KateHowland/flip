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
using System.IO;
using NWN2Toolset.NWN2.Data;
using NWN2Toolset.NWN2.Data.Blueprints;
using NWN2Toolset.NWN2.Data.ConversationData;
using NWN2Toolset.NWN2.Data.Instances;
using NWN2Toolset.NWN2.Data.Templates;
using NWN2Toolset.NWN2.Data.TypedCollections;
using Sussex.Flip.Core;
using Sussex.Flip.Games.NeverwinterNightsTwo;
using Sussex.Flip.Games.NeverwinterNightsTwo.Integration;
using Sussex.Flip.Games.NeverwinterNightsTwo.Utils;
using Sussex.Flip.Utils;

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
		protected PathChecker pathChecker;
		protected string backups;
		protected bool createFoldersForUsers;
		private NarrativeThreadsHelper nt;
		
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
		/// <param name="backups">The folder to save a second backup
		/// copy of created script files to. A null or invalid backup
		/// path will be ignored.</param>
		public NWScriptAttacher(FlipTranslator translator, INwn2Session session, string backups) : base(translator)
		{
			if (session == null) throw new ArgumentNullException("session");
			
			this.game = new GameInformation("Neverwinter Nights 2");
			this.session = session;
			this.backups = backups;
			
			pathChecker = new PathChecker();
			createFoldersForUsers = false; // not necessary as now backing up to user profile
			nt = new NarrativeThreadsHelper();
		}
		
		
		/// <summary>
		/// Constructs a new <see cref="NWScriptAttacher"/> instance.
		/// </summary>
		/// <param name="translator">The translator which will
		/// be used to translate scripts before attaching them.</param>
		/// <param name="session">The helper class used for creating,
		/// compiling and attaching scripts.</param>
		public NWScriptAttacher(FlipTranslator translator, INwn2Session session) : this(translator,session,null)
		{
		}
		
		#endregion
		
		#region Methods
		
		/// <summary>
		/// Get a script name which has not already been taken in the current module.
		/// </summary>
		/// <returns>An available script name.</returns>
		public string GetUnusedName()
		{
			NWN2GameModule module = session.GetModule();
			if (module == null) throw new ArgumentNullException("module");
			
			// MUST be 32 characters long or under! The module won't warn you of this, rather
			// it will simply truncate the filename without reporting that it has done so.
			
			// Naming format:
			// flip kn70 s47
			string username = Environment.UserName.ToLower(); // upper-case script names cause errors
			if (username.Length > 16) username = username.Substring(0,16);
			
			string ideal = String.Format("flip {0}",username);			
					
			int count = 2;
			
			string name = ideal;
			
			while (session.HasUncompiled(name)) {
				name = ideal + " s" + count++;
			}
			
			return name;
		}
		
		
		/// <summary>
		/// Translates Flip source into NWScript, compiles it, 
		/// and attaches the results to a Neverwinter Nights 2 module.
		/// </summary>
		/// <param name="source">The Flip source to be compiled.</param>
		/// <param name="address">An address representing the location
		/// to attach this script to.</param>
		/// <returns>The name the script was saved under.</returns>
		public override string Attach(FlipScript source, string address)
		{
			if (source == null) throw new ArgumentNullException("source");
			if (address == null) throw new ArgumentNullException("address");
			if (address == String.Empty) throw new ArgumentException("Must provide a valid address for attaching script.","address");
			
			if (!Nwn2ToolsetFunctions.ToolsetIsOpen()) throw new InvalidOperationException("Toolset must be open to attach scripts.");
			
			NWN2GameModule module = session.GetModule();
			if (module == null) {
				throw new InvalidOperationException("No module is currently open in the toolset.");
			}
						
			string name = GetUnusedName();
			
			try {				
				if (name.Length > 32) throw new ApplicationException("Cannot attach script under the generated name ('" + name + "') " +
				                                                     "because it is of length " + name.Length + ", and the maximum " +
				                                                     "valid length of a resource name for NWN2 is 32.");
				
				NWN2GameScript script = session.AddScript(name,source.Code);
				
				session.CompileScript(script);
				
				if (address.StartsWith("Conversation")) {
					
					Nwn2ConversationAddress convAddress = new Nwn2ConversationAddress(address);
										
					NWN2GameConversation conversation = session.GetConversation(convAddress.Conversation);
					
					if (conversation == null) {
						throw new ArgumentException("Conversation '" + convAddress.Conversation + "' was not found in current module.","address");
					}
					
//					foreach (NWN2Toolset.NWN2.Views.INWN2Viewer v in NWN2Toolset.NWN2ToolsetMainForm.App.GetAllViewers()) {
//						NWN2Toolset.NWN2.Views.NWN2ConversationViewer cv = v as NWN2Toolset.NWN2.Views.NWN2ConversationViewer;
//						if (cv != null) {
//							System.Windows.MessageBox.Show("From viewer...\n" + cv.Conversation.Name + 
//							                               "\nConnectors: " + cv.Conversation.AllConnectors.Count + "\n" +
//							                               "Entries: " + cv.Conversation.Entries.Count + "\n" +
//							                               "Replies: " + cv.Conversation.Replies.Count + "\n" +
//							                               "Loaded: " + conversation.Loaded);
//						}
//					}
//					
//					
//							System.Windows.MessageBox.Show("From module, before insisting on load...\n" + conversation.Name + 
//							                               "\nConnectors: " + conversation.AllConnectors.Count + "\n" +
//							                               "Entries: " + conversation.Entries.Count + "\n" +
//							                               "Replies: " + conversation.Replies.Count + "\n" +
//							                               "Loaded: " + conversation.Loaded);
//					
//					
//					
//					if (!conversation.Loaded) conversation.Demand();
//					
//					
//							System.Windows.MessageBox.Show("From module, after insisting on load...\n" + conversation.Name + 
//							                               "\nConnectors: " + conversation.AllConnectors.Count + "\n" +
//							                               "Entries: " + conversation.Entries.Count + "\n" +
//							                               "Replies: " + conversation.Replies.Count + "\n" +
//							                               "Loaded: " + conversation.Loaded);
					
					
					
					
					
					
					NWN2ConversationLine line = session.GetConversationLine(conversation,convAddress.LineID);
					
					if (line == null) {
						throw new ArgumentException("Line with ID " + convAddress.LineID + " was not found in current module.","address");
					}
					
					if (convAddress.AttachedAs == ScriptType.Conditional) session.AttachScriptToConversationAsCondition(script,line,conversation);
						
					else session.AttachScriptToConversation(script,line,conversation);
				}
				
				else {
					Nwn2Address nwn2Address = new Nwn2Address(address);
					
					if (!Scripts.IsEventRaiser(nwn2Address.TargetType)) {
						throw new ArgumentException("Cannot attach scripts to a " + nwn2Address.TargetType + ".");
					}
									
					if (nwn2Address.TargetType == Nwn2Type.Module) {
						session.AttachScriptToModule(script,nwn2Address.TargetSlot);
					}	
					
					else if (nwn2Address.TargetType == Nwn2Type.Area) {
						NWN2GameArea area = session.GetArea(nwn2Address.AreaTag);
						if (area == null) {
							throw new ArgumentException("Area '" + nwn2Address.AreaTag + "' was not found in current module.","address");
						}
						
						session.AttachScriptToArea(script,area,nwn2Address.TargetSlot);
					}
						
					else {							
						/*
						 * We want to attach to ALL instances matching the address in ALL OPEN areas, ignoring AreaTag and UseIndex.
						 */ 
						
						NWN2ObjectType nwn2ObjectType = Nwn2ScriptSlot.GetObjectType(nwn2Address.TargetType).Value;
										
						bool attached = false;
						
						foreach (NWN2GameArea a in module.Areas.Values) {
							
							if (!session.AreaIsOpen(a)) continue;								
							
							NWN2InstanceCollection instances = session.GetObjectsByTag(a,nwn2ObjectType,nwn2Address.InstanceTag);
																									
							foreach (INWN2Instance instance in instances) {
								session.AttachScriptToObject(script,instance,nwn2Address.TargetSlot);
								attached = true;
							}
						}	
						
						/*
						 * We also want to attach to any blueprints which use this tag as their resref.
						 */ 				
						
						// First check that if a blueprint which uses this tag as resref exists, it was created by Narrative Threads:
						if (nt.CreatedByNarrativeThreads(Nwn2ScriptSlot.GetNwn2Type(nwn2ObjectType),nwn2Address.InstanceTag)) {
							
							INWN2Blueprint blueprint = session.GetBlueprint(nwn2Address.InstanceTag.ToLower(), nwn2ObjectType);
							
							if (blueprint != null) {								
								session.AttachScriptToBlueprint(script,blueprint,nwn2Address.TargetSlot);								
								attached = true;
							}
						}
						
						if (!attached) {
							string error;
							
							if (nt.IsLoaded) error = String.Format("There isn't a {0} with tag '{1}' in any of the areas that are open, or " +
											                       "in the blueprints collection.",
										                           nwn2Address.TargetType,
										                           nwn2Address.InstanceTag);
							else error = String.Format("There isn't a {0} with tag '{1}' in any of the areas that are open.",
							                           nwn2Address.TargetType,
							                           nwn2Address.InstanceTag);
							
							throw new MatchingInstanceNotFoundException(error,nwn2Address);
						}
					}
				}
			}
			catch (MatchingInstanceNotFoundException x) {
				throw x;
			}
			catch (Exception x) {
				throw new ApplicationException("Something went wrong while saving and attaching script.",x);
			}
			
			if (backups != null) {			
				
				if (!Directory.Exists(backups)) {
					try {
						Directory.CreateDirectory(backups);
					}
					catch (Exception) { 
						return name;
					}
				}
				
				string saveTo;
				
				if (createFoldersForUsers) {					
					try {
						saveTo = Path.Combine(backups,Environment.UserName);
						if (!Directory.Exists(saveTo)) Directory.CreateDirectory(saveTo);
						
						saveTo = Path.Combine(saveTo,module.Name);
						if (!Directory.Exists(saveTo)) Directory.CreateDirectory(saveTo);
					}
					catch (Exception) {
						saveTo = backups;
					}
				}
				else {
					saveTo = backups;
				}
				
				try {
					WriteBackup(name,saveTo,source.Code);
				}
				catch (Exception) {	}
			}
			
			return name;
		}
				
		
		protected string WriteBackup(string name, string folder, string contents)
		{
			if (name == null) throw new ArgumentNullException("name");
			if (name == String.Empty) throw new ArgumentException("Name cannot be blank","name");
			if (folder == null) throw new ArgumentNullException("folder");
			if (!Directory.Exists(folder)) throw new ArgumentException("Path '" + folder + "' does not exist.","folder");
			if (contents == null) throw new ArgumentNullException("contents");
			
			string path = Path.Combine(folder,name + ".nss");
			path = pathChecker.GetUnusedFilePath(path);
							
			try {
				using (StreamWriter writer = new StreamWriter(path)) {
					writer.Write(contents);
					writer.Flush();
				}
			}
			catch (Exception x) {
				throw new ApplicationException("Failed to save a backup copy of this script to " + path,x);
			}
			
			return path;
		}
		
		#endregion
	}
}
