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
 * This file added by Keiron Nicholson on 10/08/2009 at 15:25.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using NWN2Toolset;
using NWN2Toolset.Data;
using NWN2Toolset.NWN2.Data;
using NWN2Toolset.NWN2.Data.Blueprints;
using NWN2Toolset.NWN2.Data.ConversationData;
using NWN2Toolset.NWN2.Data.Instances;
using NWN2Toolset.NWN2.Data.Templates;
using NWN2Toolset.NWN2.Data.TypedCollections;
using NWN2Toolset.NWN2.Views;
using NWN2Toolset.Plugins;
using Sussex.Flip.Core;
using Sussex.Flip.Games.NeverwinterNightsTwo.Behaviours;
using Sussex.Flip.Games.NeverwinterNightsTwo.Integration;
using Sussex.Flip.Games.NeverwinterNightsTwo.Utils;
using Sussex.Flip.UI;
using Sussex.Flip.Utils;

namespace Sussex.Flip.Games.NeverwinterNightsTwo
{
	/// <summary>
	/// A plugin which provides information about the current
	/// toolset session to the Flip application.
	/// </summary>
	public class FlipPlugin : INWN2Plugin
	{		
		#region Constants
		
		/// <summary>
		/// The name of the plugin.
		/// </summary>
		public static string NAME = "Flip";
		
		#endregion
		
		#region Fields
				
		/// <summary>
		/// The menu button which activates the plugin.
		/// </summary>
		protected TD.SandBar.MenuButtonItem pluginMenuItem;
		
		/// <summary>
		/// User preferences relating to the operation of this plugin.
		/// </summary>
		protected object preferences;
		
		/// <summary>
		/// Manages the service provided by Nwn2SessionAdapter.
		/// </summary>
		protected ServiceController service;
		
		/// <summary>
		/// The main Flip application window.
		/// </summary>
		protected FlipWindow window; 
		
		/// <summary>
		/// A factory which provides instances of TriggerControl.
		/// </summary>
		protected Nwn2TriggerFactory triggers;
		
		/// <summary>
		/// 
		/// </summary>
		protected ScriptHelper scriptHelper;
		
		/// <summary>
		/// 
		/// </summary>
		protected Nwn2Session session;
		
		protected Nwn2ObjectBlockFactory blocks;
		
		protected Nwn2MoveableProvider provider;
		
		protected NWScriptAttacher attacher;
		
		/// <summary>
		/// The folder that holds Flip data for this user.
		/// </summary>
		protected string systemFolder;
		
		/// <summary>
		/// The folder that holds Flip (and Adventure Author) logs for this user.
		/// </summary>
		protected string logsFolder;
		
		/// <summary>
		/// The folder that holds copies of Flip scripts created by this user.
		/// </summary>
		protected string scriptsFolder;
		
		#endregion
		
		#region Properties
		
		/// <summary>
		/// The menu button which activates the plugin.
		/// </summary>
		public TD.SandBar.MenuButtonItem PluginMenuItem {
			get {
				return pluginMenuItem;
			}
		}
		
		
		/// <summary>
		/// User preferences relating to the operation of this plugin.
		/// </summary>
		public object Preferences {
			get {
				return preferences;
			}
			set {
				preferences = value;
			}
		}
		
		
		/// <summary>
		/// The name of the plugin.
		/// </summary>
		public string Name {
			get {
				return FlipPlugin.NAME;
			}
		}
		
		
		/// <summary>
		/// The display name of the plugin.
		/// </summary>
		public string DisplayName {
			get {
				return FlipPlugin.NAME;
			}
		}
		
		
		/// <summary>
		/// The menu name of the plugin.
		/// </summary>
		public string MenuName {
			get {
				return FlipPlugin.NAME;
			}
		}
		
		#endregion
		
		#region Constructors
		
		/// <summary>
		/// Constructs a new <see cref="FlipPlugin"/> instance.
		/// </summary>
		public FlipPlugin()
		{
			preferences = new object();
			service = new ServiceController();
			window = null;
			
			string myDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			systemFolder = Path.Combine(myDocuments,"Adventure Author");
			logsFolder = Path.Combine(systemFolder,"User logs");
			scriptsFolder = Path.Combine(systemFolder,"Scripts");
			
			try {
				Tools.EnsureDirectoryExists(systemFolder);
				Tools.EnsureDirectoryExists(logsFolder);
				Tools.EnsureDirectoryExists(scriptsFolder);
			}
			catch (Exception x) {
				MessageBox.Show("Could not create necessary Flip folders within this user's My Documents folder. " +
				                "Logs and copies of scripts will not be saved, and the software may not work as intended.\n\n" + x);
			}
		}
			
		#endregion
		
		#region Methods
		
		protected static List<string> tracking = new List<string>{ "Tag", "First Name", "Last Name", "Localized Name", "Display Name" };
		public void UpdateBlockWithNewTag(NWN2PropertyValueChangedEventArgs e)
		{
			try {
				if (tracking.Contains(e.PropertyName) && e.NewValue != e.OldValue) {
									
					foreach (object o in e.ChangedObjects) {
						
						if (o is INWN2Instance) {
							INWN2Instance instance = (INWN2Instance)o;
							InstanceBehaviour behaviour = blocks.CreateInstanceBehaviour(instance);
							
							string bag = String.Format(Nwn2MoveableProvider.InstanceBagNamingFormat,instance.ObjectType);
						
							if (window.BlockBox.HasBag(bag)) {
								UIElementCollection existingBlocks = window.BlockBox.GetMoveables(bag);
								
								// If it's the tag that's changed, use the old tag to search, otherwise use the current one:
								string tag;
								if (e.PropertyName == "Tag") tag = e.OldValue as string;
								else tag = ((INWN2Object)instance).Tag;
											
								bool updated = false;
								
								foreach (UIElement u in existingBlocks) {
									ObjectBlock existing = u as ObjectBlock;								
									if (existing == null) continue;
									InstanceBehaviour existingBehaviour = existing.Behaviour as InstanceBehaviour;
									if (existingBehaviour == null) continue;
									
									// If you find an instance of the same type, resref and tag, replace its behaviour to update it:
									if (existingBehaviour.ResRef == behaviour.ResRef && existingBehaviour.Nwn2Type == behaviour.Nwn2Type && existingBehaviour.Tag == tag) {
										existing.Behaviour = behaviour;
										updated = true;
										break;
									}
								}
								
								if (!updated) {
									ObjectBlock block = blocks.CreateInstanceBlock(behaviour);
									window.BlockBox.AddMoveable(bag,block,false);
								}
							}
							
							
							if (e.PropertyName == "Tag") UpdateScriptsFollowingTagChange(instance,(string)e.OldValue,(string)e.NewValue,true);
							
						}
						
						else if (o is NWN2GameArea) {
							NWN2GameArea area = (NWN2GameArea)o;
							
							// Refuse changes to tags or areas unless they are to match the resource name:
							if (e.PropertyName == "Tag" && ((string)e.NewValue) != area.Name) {
								area.Tag = area.Name;
								return;
							}							
							else if (e.PropertyName == "Display Name" && (Nwn2Strings.GetStringFromOEIString((OEIShared.Utils.OEIExoLocString)e.NewValue) != area.Name)) {
								area.DisplayName = Nwn2Strings.GetOEIStringFromString(area.Name);
								return;
							}
							
							AreaBehaviour behaviour = blocks.CreateAreaBehaviour(area);
													
							if (window.BlockBox.HasBag(Nwn2MoveableProvider.AreasBagName)) {
								UIElementCollection existingBlocks = window.BlockBox.GetMoveables(Nwn2MoveableProvider.AreasBagName);
								
								string tag;
								if (e.PropertyName == "Tag") tag = e.OldValue as string;
								else tag = area.Tag;
								
								bool updated = false;
								
								foreach (UIElement u in existingBlocks) {
									ObjectBlock existing = u as ObjectBlock;								
									if (existing == null) continue;
									AreaBehaviour existingBehaviour = existing.Behaviour as AreaBehaviour;
									if (existingBehaviour == null) continue;
																	
									// If you find an area with the same tag, replace its behaviour to update it:
									if (existingBehaviour.Tag == tag) {
										existing.Behaviour = behaviour;
										updated = true;
										break;
									}
								}
								
								if (!updated) {
									ObjectBlock block = blocks.CreateAreaBlock(behaviour);
									window.BlockBox.AddMoveable(Nwn2MoveableProvider.AreasBagName,block,false);
								}
							}
						}
					}
				}
			}
			catch (Exception x) {
				MessageBox.Show("Something went wrong when updating a block.\n\n" + x);
			}
		}
		

		protected void UpdateScriptsFollowingTagChange(object obj, string oldName, string newName, bool createNewCopy)
		{			
			if (obj == null) throw new ArgumentNullException("obj");
			if (oldName == null) throw new ArgumentNullException("oldName");
			if (newName == null) throw new ArgumentNullException("newName");
			
			foreach (System.Reflection.PropertyInfo property in obj.GetType().GetProperties()) {
								
				if (property.Name.StartsWith("On")) {
					
					try {
						NWN2GameArea area = obj as NWN2GameArea;
						if (area != null && !session.AreaIsOpen(area)) area.Demand();
						
						OEIShared.IO.IResourceEntry res = property.GetValue(obj,null) as OEIShared.IO.IResourceEntry;
						
						if (res != null) {
										
							NWN2GameScript script = new NWN2GameScript(res);
								
							if (ScriptHelper.WasCreatedByFlip(script)) {
								
								if (!script.Loaded) script.Demand();
											
								string flipCode, address, naturalLanguage;
								AbstractScriptWriter.ParseNWScript(script.Data, out flipCode, out address, out naturalLanguage);
																
								string newAddress = address.Replace(oldName,newName);																				
								string newData = script.Data.Replace(address,newAddress);										
									
								if (createNewCopy) { // for instances, create a new copy of the existing script and point at that	
									NWN2GameScript newScript = session.AddScript(attacher.GetUnusedName(),newData);
									property.SetValue(obj,newScript.Resource,null);									
								}
								
								else { // for areas, change the existing script	
									string n = script.Name;
									session.DeleteScript(n);									
									NWN2GameScript newScript = session.AddScript(n,newData);									
									property.SetValue(obj,newScript.Resource,null);	
								}
							}
						}
					}
					catch (Exception x) {
						MessageBox.Show("Something went wrong when trying to update the scripts on " + newName + ".\n"+x);
					}
				}
			}
		}
		
		
		public void CreateInstanceBlocksFromBlueprints(NWN2BlueprintCollection blueprints)
		{
			if (blueprints == null || blueprints.Count == 0) return;
			
			LaunchFlip();
			
			try {
				foreach (INWN2Blueprint blueprint in blueprints) {
					ObjectBlock block = blocks.CreateInstanceBlockFromBlueprint(blueprint);
					string bag = String.Format(Nwn2MoveableProvider.InstanceBagNamingFormat,blueprint.ObjectType);
					if (window.BlockBox.HasBag(bag)) {
						window.BlockBox.AddMoveable(bag,block,true);
						//ActivityLog.Write(new Activity("CreatedBlockFromBlueprint","Block",block.GetLogText()));
						Log.WriteMessage("created blueprint block (" + block.GetLogText() + ")");
					}
				}
			}
			catch (Exception x) {
				MessageBox.Show("Something went wrong when creating a block from a blueprint.\n\n" + x);
			}
		}
		
		
		public void OpenScript(NWN2GameScript script)
		{			
			if (script == null) throw new ArgumentNullException("script");
							
			LaunchFlip();
			
			if (window.AskWhetherToSaveCurrentScript() == MessageBoxResult.Cancel) return;
			
			script.Demand();
			
			try {
				FlipScript flipScript = scriptHelper.GetFlipScript(script,Attachment.Ignore);
				
				if (flipScript == null) throw new InvalidOperationException("Script could not be understood as a Flip script.");
				
				TriggerControl trigger;

				try {
					trigger = scriptHelper.GetTrigger(script);
				}
				catch (ArgumentException) {
					trigger = null;
				}
				
				ScriptTriggerTuple tuple = new ScriptTriggerTuple(flipScript,trigger);
				
				window.OpenFlipScript(tuple);
				
				//ActivityLog.Write(new Activity("OpenedScript","ScriptName",script.Name,"Event",triggerTextForLog));
						
				if (trigger != null) {
					Log.WriteAction(LogAction.opened,"script",script.Name + " (attached to '" + trigger.GetLogText() + "')");
				}
				else {							
					Log.WriteAction(LogAction.opened,"script",script.Name);
				}
			}
			catch (Exception x) {
				throw new ApplicationException("Failed to open script '" + script.Name + "'.",x);
			}
		}
		
		
		public void UseConversationLineAsTrigger(NWN2ConversationConnector line, NWN2GameConversation conversation)
		{			
			if (line == null) throw new ArgumentNullException("line");
			if (conversation == null) throw new ArgumentNullException("conversation");
			if (!conversation.AllConnectors.Contains(line)) throw new ArgumentException("Line is not a part of the given conversation.","line");
							
			LaunchFlip();
			
			bool openingExistingScript = ScriptHelper.HasFlipScriptAttachedAsAction(line);
			
			if (openingExistingScript && window.AskWhetherToSaveCurrentScript() == MessageBoxResult.Cancel) return;
			
			TriggerControl trigger = triggers.GetTrigger(line,conversation);
			
			if (openingExistingScript) {
				
				NWN2GameScript script = new NWN2GameScript(line.Actions[0].Script);
				script.Demand();
				FlipScript flipScript = scriptHelper.GetFlipScript(script,Attachment.Ignore);
				
				window.OpenFlipScript(new ScriptTriggerTuple(flipScript,trigger));
				
				//ActivityLog.Write(new Activity("OpenedScript","ScriptName",script.Name,"Event",trigger.GetLogText()));	
				Log.WriteAction(LogAction.opened,"script",script.Name + " (attached to line '" + line.Line.Text.GetSafeString(OEIShared.Utils.BWLanguages.CurrentLanguage) + "')");
			}
			
			else {
				
				window.SetTrigger(trigger);
				window.IsDirty = true;
				
				//ActivityLog.Write(new Activity("NewScript","CreatedVia","UsingConversationLineAsEvent","Event",trigger.GetLogText()));
				string lineText; 
				try {
					lineText = line.Line.Text.GetSafeString(OEIShared.Utils.BWLanguages.CurrentLanguage).Value;
				}
				catch (Exception) {
					lineText = String.Empty;
				}
				Log.WriteAction(LogAction.added,"script","to a line of conversation ('" + lineText + "')");
			}
		}
		
		
		protected Nwn2AddressFactory addressFactory = new Nwn2AddressFactory();
		
		public void AddConditionToConversationLine(NWN2ConversationConnector line, NWN2GameConversation conversation)
		{			
			if (line == null) throw new ArgumentNullException("line");
			if (conversation == null) throw new ArgumentNullException("conversation");
			if (!conversation.AllConnectors.Contains(line)) throw new ArgumentException("Line is not a part of the given conversation.","line");
									
			LaunchFlip();
			
			bool openingExistingScript = ScriptHelper.HasFlipScriptAttachedAsCondition(line);
			
			if (openingExistingScript && window.AskWhetherToSaveCurrentScript() == MessageBoxResult.Cancel) return;
					
			window.EnterConditionMode(Nwn2Strings.GetStringFromOEIString(line.Line.Text));
			
			window.ConditionalFrame.Address = addressFactory.GetConversationAddress(conversation.Name,line.Line.LineGuid,ScriptType.Conditional).Value;
						
			if (openingExistingScript) {
				
				NWN2GameScript script = new NWN2GameScript(line.Conditions[0].Script);
				script.Demand();
				FlipScript flipScript = scriptHelper.GetFlipScript(script,Attachment.Ignore);
				
				window.OpenFlipScript(new ScriptTriggerTuple(flipScript,null));
				
				//ActivityLog.Write(new Activity("OpenedScript","ScriptName",script.Name,"Event",String.Empty));
				Log.WriteAction(LogAction.opened,"script",script.Name + "(attached as condition to line '" + line.Line.Text.GetSafeString(OEIShared.Utils.BWLanguages.CurrentLanguage) + "')");
			}
			
			else {
				
				window.IsDirty = true;
				
				//ActivityLog.Write(new Activity("NewScript","CreatedVia","AddingConditionToConversationLine","Event",String.Empty));
				string lineText; 
				try {
					lineText = line.Line.Text.GetSafeString(OEIShared.Utils.BWLanguages.CurrentLanguage).Value;
				}
				catch (Exception) {
					lineText = String.Empty;
				}
				Log.WriteAction(LogAction.added,"script","as condition to a line of conversation ('" + lineText + "')");
			}
		}
		
		
		public string GetNaturalLanguage(string scriptData)
		{
			if (String.IsNullOrEmpty(scriptData)) return null;
			
			string flipCode, address, naturalLanguage;
			
			try {
				ScriptWriter.ParseNWScript(scriptData, out flipCode, out address, out naturalLanguage);	
				
				return ScriptWriter.RemoveTrigger(naturalLanguage);
			}
			catch (Exception) {
				return null;
			}
		}
		
		
		/// <summary>
		/// When a conversation is closed, close the current script if it was
		/// connected to a conversation.
		/// </summary>
		public void ConversationClosing()
		{
			if (window != null) {
				if (window.Mode == ScriptType.Conditional) {
					window.CloseScript();
					window.LeaveConditionMode();
				}
				else if (window.TriggerBar.TriggerControl != null && window.TriggerBar.TriggerControl.GetType() == typeof(DialogueWasSpoken)) {
					window.CloseScript();
				}
			}
		}
		
		
		/// <summary>
		/// Performs setup operations.
		/// Called by the toolset when it is started.
		/// </summary>
		/// <param name="cHost">A plugin host component which
		/// manages the plugins currently loaded into the toolset.</param>
		public void Startup(INWN2PluginHost cHost)
		{
			// Start providing useful NWN2 functions as a service to the test suite (obsolete):
			try {
				service.Start();
			}
			catch (Exception) { }
			
			// Ensure flip_functions.nss is in the Override directory of NWN2 - otherwise scripts won't compile:
			//ProvideSpecialFunctionsScriptFile();
			
			// Modify the user interface:
			ToolsetUIModifier UI = new ToolsetUIModifier(new ToolsetUIModifier.AddScriptToConversation(UseConversationLineAsTrigger),
			                                             new ToolsetUIModifier.AddScriptToConversation(AddConditionToConversationLine),
			                                             new ToolsetUIModifier.CreateBlockFromBlueprintDelegate(CreateInstanceBlocksFromBlueprints),
			                                             new ToolsetUIModifier.UpdateBlockWhenTagChangesDelegate(UpdateBlockWithNewTag));
			
			try {
				UI.ModifyUI();
			}
			catch (Exception x) {
				MessageBox.Show("Something went wrong when modifying the user interface.\n\n" + x);
			}
			
			try {
				TD.SandBar.ButtonItem flipButton = UI.AddFlipButton();
				if (flipButton != null) {
					flipButton.Activate += delegate 
					{ 
						LaunchFlip(); 
						//ActivityLog.Write(new Activity("LaunchedFlip","LaunchedFrom","Toolbar"));
						Log.WriteAction(LogAction.launched,"flip","from toolbar");
					};
				}
			}
			catch (Exception x) {
				MessageBox.Show("Something went wrong when adding Flip button to toolbar.\n\n" + x);
			}
			
			// Set up plugin menu items:
			pluginMenuItem = cHost.GetMenuForPlugin(this);
			pluginMenuItem.Activate += PluginActivated;
			
			TD.SandBar.MenuButtonItem scriptAccessMenuItem = new TD.SandBar.MenuButtonItem("Enable script access");
			scriptAccessMenuItem.Checked = UI.AllowScriptAccess;
			scriptAccessMenuItem.Activate += delegate 
			{  
				try {
					if (UI.AllowScriptAccess) {
						foreach (INWN2Viewer viewer in NWN2ToolsetMainForm.App.GetAllViewers()) {
							if (viewer is NWN2ScriptViewer) {
								System.Windows.MessageBox.Show("All scripts must be closed before script access can be disabled.",
								                               "Close all scripts",
								                			   System.Windows.MessageBoxButton.OK,
								                			   System.Windows.MessageBoxImage.Exclamation);
								return;						                               
							}
						}
					}
					
					UI.AllowScriptAccess = !UI.AllowScriptAccess;				
					scriptAccessMenuItem.Checked = UI.AllowScriptAccess;
				}
				catch (Exception x) {
					MessageBox.Show("Something went wrong when changing script access settings.\n\n" + x);
				}
			};			
			
			pluginMenuItem.Items.Add(scriptAccessMenuItem);		
			
			TD.SandBar.MenuButtonItem launchFlip = new TD.SandBar.MenuButtonItem("Flip");
			launchFlip.Activate += delegate 
			{ 
				try {
					LaunchFlip();
					//ActivityLog.Write(new Activity("LaunchedFlip","LaunchedFrom","Menu"));
					Log.WriteAction(LogAction.launched,"flip","from plugin menu");
				}
				catch (Exception x) {
					MessageBox.Show("Something went wrong when trying to launch Flip.\n\n" + x);
				}
			};
			
			pluginMenuItem.Items.Add(launchFlip);	
			
			//StartLogging();
		}
		
		
		/// <summary>
		/// Performs setup operations. 
		/// Called by the toolset when it has finished loading.
		/// </summary>
		/// <param name="cHost">A plugin host component which
		/// manages the plugins currently loaded into the toolset.</param>
		public void Load(INWN2PluginHost cHost)
		{	
			try {
				InitialiseFlip();
			}
			catch (Exception x) {
				MessageBox.Show("Something went wrong when initialising Flip.\n\n" + x);
			}
		}
		
		
		/// <summary>
		/// Construct a new instance of FlipWindow, and forbid it from closing fully.
		/// </summary>
		protected void InitialiseFlip()
		{
			session = new Nwn2Session();
			FlipTranslator translator = new NWScriptTranslator();
			attacher = new NWScriptAttacher(translator,session,scriptsFolder);
								
			Nwn2Fitters fitters = new Nwn2Fitters();				
			triggers = new Nwn2TriggerFactory(fitters);
			scriptHelper = new ScriptHelper(triggers);
			
			Nwn2StatementFactory statements = new Nwn2StatementFactory(fitters);	
			Nwn2ImageProvider images = new Nwn2ImageProvider(new NarrativeThreadsHelper());
			blocks = new Nwn2ObjectBlockFactory(images);
				
			ToolsetEventReporter reporter = new ToolsetEventReporter();			
			
			provider = new Nwn2MoveableProvider(blocks,statements,triggers,reporter);
				
			window = new FlipWindow(provider,images,
			                        new FlipWindow.OpenDeleteScriptDelegate(OpenDeleteScriptDialog),
			                        new FlipWindow.SaveScriptDelegate(SaveScript),
			                        new Nwn2DeserialisationHelper());
			
			window.Closing += delegate(object sender, CancelEventArgs e) 
			{  
				// Hide the window instead of closing it:
				e.Cancel = true;
				
				// Unless the user changes their mind about closing the script,
				// in which case don't even do that:
				if (window.AskWhetherToSaveCurrentScript() != MessageBoxResult.Cancel) {
					//window.CloseScript();					
					window.Visibility = Visibility.Hidden;
					//window.LeaveConditionMode();
					Log.WriteAction(LogAction.exited,"flip");
				}
			};
			
			reporter.ModuleChanged += delegate 
			{ 
				Action action = new Action
				(
					delegate()
					{		
						if (window == null) return;
						try {
							window.CloseScript();
							window.Close();
						}
						catch (Exception) {}
					}
				);					
					
				if (window != null) window.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,action);
			};
			
			MenuItem addWildcardBlock = new MenuItem();
			addWildcardBlock.Header = "Add Wildcard block";
			addWildcardBlock.Click += delegate 
			{  
				try {
					CreateWildcardDialog dialog = new CreateWildcardDialog();
					dialog.ShowDialog();
					
					if (!String.IsNullOrEmpty(dialog.WildcardTag)) {
						ObjectBlock block = blocks.CreateWildcardBlock(dialog.WildcardTag);
										
						window.BlockBox.AddMoveable(Nwn2MoveableProvider.ValuesBagName,block,true);
						//ActivityLog.Write(new Activity("CreatedWildcardBlock","Block",block.GetLogText()));
						Log.WriteMessage("created wildcard block (" + block.GetLogText() + ")");
					}
				}
				catch (Exception x) {
					MessageBox.Show("Something went wrong when creating a Wildcard block.\n\n" + x);
				}
			};			
			window.EditMenu.Items.Add(addWildcardBlock);
						
			MenuItem openTriggerlessScripts = new MenuItem();
			openTriggerlessScripts.Header = "Open unattached script";
			openTriggerlessScripts.Click += delegate 
			{  
				try {
					OpenAnyScriptViaDialog();
				}
				catch (Exception x) {
					MessageBox.Show("Something went wrong when opening a script via its filename.\n\n" + x);
				}
			};			
			window.DevelopmentMenu.Items.Add(openTriggerlessScripts);
						
			MenuItem analyseAllScripts = new MenuItem();
			analyseAllScripts.Header = "Analyse all scripts";
			analyseAllScripts.Click += delegate 
			{  
				try {
					AnalyseAllScripts();
				}
				catch (Exception x) {
					MessageBox.Show("Something went wrong when analysing scripts.\n\n" + x);
				}
			};			
			window.DevelopmentMenu.Items.Add(analyseAllScripts);
						
			MenuItem analyseScript = new MenuItem();
			analyseScript.Header = "Analyse script";
			analyseScript.Click += delegate 
			{  
				try {
					AnalyseScript();
				}
				catch (Exception x) {
					MessageBox.Show("Something went wrong when analysing script.\n\n" + x);
				}
			};			
			window.DevelopmentMenu.Items.Add(analyseScript);
						
//			MenuItem showLogWindow = new MenuItem();
//			showLogWindow.Header = "Show log window";
//			showLogWindow.Click += delegate 
//			{  
//				try {
//					new ActivityLogWindow().Show();
//				}
//				catch (Exception x) {
//					MessageBox.Show("Something went wrong when launching the log window.\n\n" + x);
//				}
//			};			
//			window.DevelopmentMenu.Items.Add(showLogWindow);
				
			// Start recording debug messages and user actions:
			try {
				LogWriter.StartRecording("flip");
			}
			catch (Exception x) {
				MessageBox.Show("Something went wrong when setting up a Flip user log.\n" + x);
			}
			
			reporter.AreaNameChanged += delegate(object oObject, NameChangedEventArgs eArgs) 
			{  
				UpdateScriptsFollowingTagChange(oObject,eArgs.OldName,eArgs.NewName,false);
			};
		}
		
		
		protected void OpenAnyScriptViaDialog()
		{			
			if (window == null) return;
			
			if (window.AskWhetherToSaveCurrentScript() == MessageBoxResult.Cancel) return;
			
			try {
				OpenDeleteScriptDialog(Attachment.Ignore);
			}
			catch (Exception x) {
				MessageBox.Show(String.Format("Something went wrong when opening or deleting a script.{0}{0}{1}",Environment.NewLine,x));
			}
		}
		
		
		public void OpenDeleteScriptDialog()
		{
			OpenDeleteScriptDialog(Attachment.AttachedToScriptSlot);
		}
		
		
		public void OpenDeleteScriptDialog(Attachment attachment)
		{
			List<ScriptTriggerTuple> tuples = scriptHelper.GetAllScripts(attachment);
			
			ScriptSelector dialog = new ScriptSelector(tuples,session,window);
			
			dialog.ShowDialog();
		}
		
		
		public bool SaveScript(FlipWindow window)
		{
			if (window == null) throw new ArgumentNullException("window");
			if (attacher == null) throw new InvalidOperationException("No attacher to save scripts with.");
			
			if (!window.IsComplete) {
				//ActivityLog.Write(new Activity("TriedToSaveIncompleteScript"));
				Log.WriteMessage("tried to save incomplete script");
				MessageBox.Show("Your script isn't finished! Fill in all the blanks before saving.");
				return false;
			}
			
			IScriptFrame scriptFrame = window.GetCurrentScriptFrame();
				
			AbstractScriptWriter scriptWriter;			
			if (window.Mode == ScriptType.Conditional) scriptWriter = new ConditionalScriptWriter(window.ConditionalFrame);
			else scriptWriter = new ScriptWriter(window.TriggerBar);
			
			string code = scriptWriter.GetCombinedCode();
			
			FlipScript script = new FlipScript(code,window.Mode,String.Empty);
			
			string address = scriptFrame.GetAddress();
			
			try {
				string savedAs = attacher.Attach(script,address);
				
				window.TriggerBar.CurrentScriptIsBasedOn = savedAs;
				window.ConditionalFrame.CurrentScriptIsBasedOn = savedAs;
				
				window.IsDirty = false;
			
				//ActivityLog.Write(new Activity("SavedScript","SavedAs",savedAs));
				Log.WriteAction(LogAction.saved,"script","as "+ savedAs);
				
				//MessageBox.Show("Script was saved successfully.");
				try {
					string nl = window.NaturalLanguage;
					if (String.IsNullOrEmpty(nl)) window.NaturalLanguage = "Saved.";
					else window.NaturalLanguage = nl + Environment.NewLine + "Saved.";
				}
				catch (Exception) {}
				
				return true;
			}
			
			catch (MatchingInstanceNotFoundException x) {
				//ActivityLog.Write(new Activity("TriedToSaveScriptButTargetCouldNotBeFound","TargetType",x.Address.TargetType.ToString(),"TargetTagOrResRef",x.Address.InstanceTag));
				Log.WriteMessage("tried to save script but couldn't find target (was looking for " + x.Address.TargetType + " with tag/resref " + x.Address.InstanceTag + ")");
				MessageBox.Show(String.Format("There's no {0} like this (with tag '{1}') in any area that's open.\nMake sure that the area containing " +
				                              "the {0} is open when you try to save, or it won't work.",x.Address.TargetType,x.Address.InstanceTag));
				return false;
			}
			
			catch (Exception x) {
				
				string errorMessage = x.ToString();
				
				if (errorMessage.Contains("flip_functions")) {
					try {
						string path = Path.Combine(NWN2Toolset.NWN2.IO.NWN2ResourceManager.Instance.OverrideDirectory.DirectoryName,"flip_functions.nss");
						if (!File.Exists(path)) {
							MessageBox.Show("File " + path + " is missing. Scripts will not compile successfully. Please re-install Flip.");
							return false;
						}
					}
					catch (Exception) {}
				}
				
				MessageBox.Show(String.Format("Something went wrong when saving the script.{0}{0}{1}",Environment.NewLine,errorMessage));
				return false;
			}
		}
		
		
		/// <summary>
		/// Launch the Flip application.
		/// </summary>
		public void LaunchFlip()
		{
			if (window == null) InitialiseFlip();
			
			if (!window.ShowActivated) window.Show();
			
			else window.Visibility = Visibility.Visible;
			
			window.Activate();
		}
		
		
		/// <summary>
		/// Performs teardown operations.
		/// Called by the toolset when it begins to shutdown.
		/// </summary>
		/// <param name="cHost">A plugin host component which
		/// manages the plugins currently loaded into the toolset.</param>
		public void Unload(INWN2PluginHost cHost)
		{
		}
		
		
		/// <summary>
		/// Performs teardown operations.
		/// Called by the toolset when it is about to finish shutting down.
		/// </summary>
		/// <param name="cHost">A plugin host component which
		/// manages the plugins currently loaded into the toolset.</param>
		public void Shutdown(INWN2PluginHost cHost)
		{			
			// Stop logging Flip actions:
			try {
				LogWriter.StopRecording();
				//ActivityLog.StopLog();
			}
			catch (Exception x) {
				MessageBox.Show("Something went wrong when closing Flip activity log.\n\n" + x);
			}
			
			try {
				service.Stop();
			}
			catch (Exception x) {
				MessageBox.Show("Something went wrong when stopping the service.\n\n" + x);
			}
		}
		
		
		/// <summary> 
		/// Runs the plugin.
		/// </summary>
		protected void PluginActivated(object sender, EventArgs e)
		{	
		}
		
		
		public override string ToString()
		{
			return "Flip plugin";
		}
		
		#endregion
		
		public void AnalyseScript()
		{
			MessageBox.Show(window.GetStatistics().ToString());
		}
		
		
		public void AnalyseAllScripts()
		{
			string text = "Analysis of attached scripts.";
			
			List<ScriptTriggerTuple> scripts = scriptHelper.GetAllScriptsFromModule(Attachment.Attached);
				
			ModuleStats ms = new ModuleStats();
			ms.AttachedScripts += scripts.Count;
			ms.Name = session.GetModule().Name;
			
			foreach (ScriptTriggerTuple s in scripts) {
				
				window.OpenFlipScript(s);
				
				ScriptStats stats = window.GetStatistics();
				ms.Add(stats);
				text += Environment.NewLine + "'" + s.Script.Name + "': " + stats;
				
				window.CloseScript();
			}
			
			text += Environment.NewLine + Environment.NewLine + ms;
			
			MessageBox.Show(text);
		}
	}
}
