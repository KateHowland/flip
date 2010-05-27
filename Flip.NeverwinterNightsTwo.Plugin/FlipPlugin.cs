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
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Media;
using NWN2Toolset;
using NWN2Toolset.NWN2.Data;
using NWN2Toolset.NWN2.Data.ConversationData;
using NWN2Toolset.NWN2.IO;
using NWN2Toolset.NWN2.Views;
using NWN2Toolset.Plugins;
using Sussex.Flip.Core;
using Sussex.Flip.UI;
using Sussex.Flip.Games.NeverwinterNightsTwo;
using Sussex.Flip.Games.NeverwinterNightsTwo.Utils;

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
		}
			
		#endregion
		
		#region Methods
		
		public void UseConversationLineAsTrigger(NWN2ConversationConnector line, NWN2GameConversation conversation)
		{
			if (window == null) {
				MessageBox.Show("Flip is not currently open.");
				return;
			}
			if (line == null) throw new ArgumentNullException("line");
			if (conversation == null) throw new ArgumentNullException("conversation");
			if (!conversation.AllConnectors.Contains(line)) throw new ArgumentException("Line is not a part of the given conversation.","line");
			
			TriggerControl trigger = triggers.GetTrigger(line,conversation);
			
			LaunchFlip();
			
			window.SetTrigger(trigger);
		}
		
		
		/// <summary>
		/// Performs setup operations.
		/// Called by the toolset when it is started.
		/// </summary>
		/// <param name="cHost">A plugin host component which
		/// manages the plugins currently loaded into the toolset.</param>
		public void Startup(INWN2PluginHost cHost)
		{
			service.Start();
			
			ProvideSpecialFunctionsScriptFile();
			
			pluginMenuItem = cHost.GetMenuForPlugin(this);
			pluginMenuItem.Activate += PluginActivated;
			
			ToolsetUIModifier UI = new ToolsetUIModifier(new ToolsetUIModifier.ProvideTriggerDelegate(UseConversationLineAsTrigger));
			UI.ModifyUI();
			
			TD.SandBar.MenuButtonItem scriptAccessMenuItem = new TD.SandBar.MenuButtonItem("Enable script access");
			scriptAccessMenuItem.Checked = UI.AllowScriptAccess;
			scriptAccessMenuItem.Activate += delegate 
			{  
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
			};
			
			pluginMenuItem.Items.Add(scriptAccessMenuItem);
			
			TD.SandBar.MenuButtonItem launchFlip = new TD.SandBar.MenuButtonItem("Flip");
			launchFlip.Activate += delegate { LaunchFlip(); };
			
			pluginMenuItem.Items.Add(launchFlip);
		}
		
		
		protected void ProvideSpecialFunctionsScriptFile()
		{
//			System.Reflection.Assembly a = System.Reflection.Assembly.GetExecutingAssembly();
//        
//			a.GetType().
//			
//			
//            // get a list of resource names from the manifest
//            FileStream[] files = a.GetFiles(true);
//            
//            foreach (FileStream file in files) MessageBox.Show(file.Name);
//
//			DirectoryInfo directory = new DirectoryInfo(NWN2ResourceManager.Instance.OverrideDirectory.DirectoryName);
//			if (directory.GetFiles("flip_functions.nss",SearchOption.TopDirectoryOnly).Length == 0) {
//				File.Copy("flip_functions.nss",Path.Combine(directory.FullName,"flip_functions.nss"));
//			}
		}
		
		
		/// <summary>
		/// Performs setup operations. 
		/// Called by the toolset when it has finished loading.
		/// </summary>
		/// <param name="cHost">A plugin host component which
		/// manages the plugins currently loaded into the toolset.</param>
		public void Load(INWN2PluginHost cHost)
		{	
			LaunchFlip();
		}
		
		
		/// <summary>
		/// Construct a new instance of FlipWindow, and forbid it from closing fully.
		/// </summary>
		protected void InitialiseFlip()
		{
			FlipTranslator translator = new NWScriptTranslator();
			Nwn2Session session = new Nwn2Session();
			FlipAttacher attacher = new NWScriptAttacher(translator,session);
								
			Nwn2Fitters fitters = new Nwn2Fitters();	
			
			triggers = new Nwn2TriggerFactory(fitters);
			
			Nwn2StatementFactory statements = new Nwn2StatementFactory(fitters);
			Nwn2ObjectBlockFactory blocks = new Nwn2ObjectBlockFactory();
				
			ToolsetEventReporter reporter = new ToolsetEventReporter();
			
			Nwn2MoveableProvider provider = new Nwn2MoveableProvider(blocks,statements,triggers,reporter);
				
			window = new FlipWindow(attacher,provider,new Nwn2BehaviourFactory());		
			
			window.Closing += delegate(object sender, CancelEventArgs e) 
			{  
				e.Cancel = true;
				window.Visibility = Visibility.Hidden;
			};
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
			
//			if (window == null) {
//				InitialiseFlip();
//				window.Show();
//			}			
//			else {
//				window.Visibility = Visibility.Visible;
//			}
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
			service.Stop();
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
	}
}
