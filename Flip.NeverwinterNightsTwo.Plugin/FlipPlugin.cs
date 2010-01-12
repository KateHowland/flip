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
using System.Reflection;
using System.ServiceModel;
using NWN2Toolset;
using NWN2Toolset.NWN2.Data;
using NWN2Toolset.NWN2.Views;
using NWN2Toolset.Plugins;
using Crownwood.DotNetMagic.Docking;
using VisualHint.SmartPropertyGrid;
using Sussex.Flip.Games.NeverwinterNightsTwo.Utils;

namespace Sussex.Flip.Games.NeverwinterNightsTwo.Plugin
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
		/// The menu button which allows the user to enable or disable
		/// access to scripts.
		/// </summary>
		protected TD.SandBar.MenuButtonItem scriptAccessMenuItem;
		
		/// <summary>
		/// User preferences relating to the operation of this plugin.
		/// </summary>
		protected object preferences;
				
		/// <summary>
		/// The host for services provided by this plugin.
		/// </summary>
		protected ServiceHostBase host;
		
		/// <summary>
		/// Controls access to toolset panels.
		/// </summary>
		protected DockingManager dockingManager = null;
		
		/// <summary>
		/// The private inner grid of the main property grid on the toolset interface.
		/// </summary>
		/// <remarks>Narrative Threads disposes of the main property grid, so always
		/// check that this control is not
		/// null or disposed before using it.</remarks>
		protected PropertyGrid mainPropertyInnerGrid = null;
		
		/// <summary>
		/// The user interface panels which allow users to access scripts.
		/// </summary>
		protected List<NWN2ModuleScriptList> scriptPanels = new List<NWN2ModuleScriptList>(2);
		
		/// <summary>
		/// The menu items which allow users to access scripts.
		/// </summary>
		protected List<TD.SandBar.MenuButtonItem> scriptMenuItems = new List<TD.SandBar.MenuButtonItem>(2);
		
		/// <summary>
		/// Reflected field information for the private
		/// property grid on class NWN2PropertyGrid.
		/// </summary>
		protected FieldInfo innerPropertyGridFieldInfo = null;
		
		/// <summary>
		/// The event handler for the main property grid.
		/// </summary>
		protected PropertyGrid.SelectedObjectChangedEventHandler mainGridHandler;
		
		/// <summary>
		/// The event handler for newly-created floating property grids.
		/// </summary>
		protected DockingManager.ContentHandler floatingGridHandler;
		
		/// <summary>
		/// Tracks whether the toolset is currently set to allow access to 
		/// scripts. Call EnableScriptAccess() and DisableScriptAccess() to change.
		/// </summary>
		protected bool canAccessScripts = false;
		
		/// <summary>
		/// True if the plugin has successfully connected to the main Flip
		/// application via a service; false otherwise.
		/// </summary>
		/// <remarks>Only one instance of the toolset can connect to Flip -
		/// additional instances will be unable to talk to the service, and
		/// as a result this value will be false.
		/// </remarks>
		protected bool connected = false;
		
		#endregion
		
		#region Properties
		
		/// <summary>
		/// True if the plugin has successfully connected to the main Flip
		/// application via a service; false otherwise.
		/// </summary>
		/// <remarks>Only one instance of the toolset can connect to Flip -
		/// additional instances will be unable to talk to the service, and
		/// as a result this value will be false.
		/// </remarks>
		public bool Connected {
			get { return connected; }
		}
		
		
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
			
			GatherFieldReferences();
			
			floatingGridHandler = new DockingManager.ContentHandler(HideScriptSlotsOnFloatingGrid);
			mainGridHandler = new PropertyGrid.SelectedObjectChangedEventHandler(HideScriptSlotsOnMainGrid);			
		}
				
		#endregion
		
		#region Methods
		
		/// <summary>
		/// Performs setup operations.
		/// Called by the toolset when it is started.
		/// </summary>
		/// <param name="cHost">A plugin host component which
		/// manages the plugins currently loaded into the toolset.</param>
		public void Startup(INWN2PluginHost cHost)
		{
			StartServices();	
			pluginMenuItem = cHost.GetMenuForPlugin(this);
			pluginMenuItem.Activate += PluginActivated;
			
			scriptAccessMenuItem = new TD.SandBar.MenuButtonItem("Enable script access");
			scriptAccessMenuItem.Activate += delegate 
			{  
				if (canAccessScripts) DisableScriptAccess();
				else EnableScriptAccess();
			};
			
			pluginMenuItem.Items.Add(scriptAccessMenuItem);
		}
		
		
		/// <summary>
		/// Performs setup operations. 
		/// Called by the toolset when it has finished loading.
		/// </summary>
		/// <param name="cHost">A plugin host component which
		/// manages the plugins currently loaded into the toolset.</param>
		public void Load(INWN2PluginHost cHost)
		{			
			DisableScriptAccess();
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
			StopServices();
		}
		
		
		/// <summary> 
		/// Runs the plugin.
		/// </summary>
		protected void PluginActivated(object sender, EventArgs e)
		{	
		}
		
		
		/// <summary>
		/// Gather references to toolset fields which are used by other plugin methods.
		/// </summary>
		protected void GatherFieldReferences()
		{						
			foreach (FieldInfo field in typeof(NWN2PropertyGrid).GetFields(BindingFlags.Instance | BindingFlags.NonPublic)) {
				if (field.FieldType == typeof(PropertyGrid)) {
					innerPropertyGridFieldInfo = field;
				}
			}
				
			foreach (FieldInfo field in typeof(NWN2ToolsetMainForm).GetFields(BindingFlags.Instance | BindingFlags.NonPublic)) {
				if (field.FieldType == typeof(NWN2PropertyGrid)) {
					NWN2PropertyGrid mainPropertyGrid = (NWN2PropertyGrid)field.GetValue(NWN2ToolsetMainForm.App);
					/*
					 * If Narrative Threads is running the main property grid may already have been disposed,
					 * so check that it (and its inner grid) are not null or disposed whenever using them.
					 */
					if (mainPropertyGrid != null) {
						mainPropertyInnerGrid = (PropertyGrid)innerPropertyGridFieldInfo.GetValue(mainPropertyGrid);
					}
				}
				else if (field.FieldType == typeof(DockingManager)) {
					dockingManager = (DockingManager)field.GetValue(NWN2ToolsetMainForm.App);
				}
				else if (field.FieldType == typeof(NWN2ModuleScriptList)) {
					scriptPanels.Add((NWN2ModuleScriptList)field.GetValue(NWN2ToolsetMainForm.App));
				}
				else if (field.FieldType == typeof(TD.SandBar.MenuButtonItem)) {
					TD.SandBar.MenuButtonItem mbi = (TD.SandBar.MenuButtonItem)field.GetValue(NWN2ToolsetMainForm.App);
					if (mbi.Text == "&Script" || mbi.Text == "Open Conversation/Script") {
						scriptMenuItems.Add(mbi);
					}
				}
					
				// No longer required to find mainPropertyInnerGrid as a result of Narrative Threads integration:
				if (dockingManager != null && 
				    //mainPropertyInnerGrid != null &&
				    innerPropertyGridFieldInfo != null && 
				    scriptPanels.Count == 2 &&
				    scriptMenuItems.Count == 2) return;
			}
			
			throw new ApplicationException("Failed to find a crucial field via reflection.");
		}
		
		
		/// <summary>
		/// Start hosting services.
		/// </summary>
		protected void StartServices()
		{
			try {
				host = new ServiceHost(typeof(Nwn2SessionAdapter),new Uri[]{ new Uri("net.pipe://localhost") });
				
				NetNamedPipeBinding binding = new NetNamedPipeBinding();
				binding.MaxReceivedMessageSize = Int32.MaxValue;
				
				host.AddServiceEndpoint(typeof(INwn2Service).ToString(),
				                        binding,
				                        "NamedPipeEndpoint");				
				
				host.Open();
				connected = true;
			} 
			catch (System.ServiceModel.AddressAlreadyInUseException) {
				connected = false;
				System.Windows.MessageBox.Show("The Neverwinter Nights 2 " +
				                               "toolset is already running.",
				                               "Already running",
				                               System.Windows.MessageBoxButton.OK,
				                               System.Windows.MessageBoxImage.Error);
				/*
				 * This seems to work fine, even if you launch many copies of the toolset...
				 * the first one connects to the service, subsequent copies warn you and then
				 * shut down.
				 * 
				 * When there's actually a Flip application to connect to, the procedure will be:
				 * the user launches the toolset; the toolset launches Flip, and Flip immediately
				 * connects to the service. Launching additional copies of the toolset will fail
				 * at the setting up the service stage, at which point the new versions of both
				 * Flip and the toolset should be shut down immediately. (Launching additional
				 * copies of Flip can simply be forbidden to the user.)
				 */
				System.Diagnostics.Process.GetCurrentProcess().Kill();
			}
			catch (Exception e) {
				connected = false;
				System.Windows.MessageBox.Show("There was a problem when trying to set up the connection between " +
				                               "Neverwinter Nights 2 and Flip. This may mean that the software " +
				                               "does not function correctly." +
				                               Environment.NewLine + Environment.NewLine +
				                               "Exception detail:" + Environment.NewLine +
				                               e,
				                               "Failed to setup service",
				                               System.Windows.MessageBoxButton.OK,
				                               System.Windows.MessageBoxImage.Error);
			}
		}
		
		
		/// <summary>
		/// Stop hosting services.
		/// </summary>
		protected void StopServices()
		{
			if (connected && host != null && host.State != CommunicationState.Closed && host.State != CommunicationState.Closing) {
				host.Close();
			}
		}
		
		
		/// <summary>
		/// Allow script-related functionality to be accessed in the toolset.
		/// </summary>
		public void EnableScriptAccess()
		{			
			canAccessScripts = true;
			ControlAccessToScripts(canAccessScripts);
			scriptAccessMenuItem.Checked = canAccessScripts;
		}
		
		
		/// <summary>
		/// Disables the ability to access script-related functionality in the toolset.
		/// </summary>
		/// <remarks>Aborts with a warning if any script viewers are open when the method
		/// is run.</remarks>
		public void DisableScriptAccess()
		{
			foreach (INWN2Viewer viewer in NWN2ToolsetMainForm.App.GetAllViewers()) {
				if (viewer is NWN2ScriptViewer) {
					System.Windows.MessageBox.Show("All scripts must be closed before script access can be disabled.",
					                               "Close all scripts",
					                			   System.Windows.MessageBoxButton.OK,
					                			   System.Windows.MessageBoxImage.Exclamation);
					return;						                               
				}
			}
			
			canAccessScripts = false;
			ControlAccessToScripts(canAccessScripts);
			scriptAccessMenuItem.Checked = canAccessScripts;
		}
		
		
		/// <summary>
		/// Enable or disable access to script-related functionality in the toolset. This includes
		/// the script list panels, the ability to create or open scripts, and the script slot
		/// properties on game objects.
		/// </summary>
		/// <param name="allow">True to enable script-related functionality, false to disable.</param>
		protected void ControlAccessToScripts(bool allow)
		{	
			// Narrative Threads may have disposed the main property grid, so check that it exists:
			if (mainPropertyInnerGrid != null && !mainPropertyInnerGrid.IsDisposed) {
				// If blocking scripts, hide script slots on the main property grid whenever it changes:
				mainPropertyInnerGrid.SelectedObjectChanged -= mainGridHandler;
				if (!allow) mainPropertyInnerGrid.SelectedObjectChanged += mainGridHandler;		
			}
			
			// If blocking scripts, hide script slots on floating property grids when they first appear:
			dockingManager.ContentShown -= floatingGridHandler;
			if (!allow) dockingManager.ContentShown += floatingGridHandler;
			
			// Enable/disable user controls relating to script panels:
			foreach (NWN2ModuleScriptList panel in scriptPanels) {
				panel.Enabled = allow;
			}
			foreach (TD.SandBar.MenuButtonItem menuItem in scriptMenuItems) {
				menuItem.Enabled = allow;
			}
					
			// Refresh each open property grid, then hide script slots if appropriate:
			foreach (Content content in dockingManager.Contents) {
				if (content.Control is NWN2PropertyGrid) {
					PropertyGrid innerGrid = (PropertyGrid)innerPropertyGridFieldInfo.GetValue(content.Control);
					innerGrid.SelectedObjects = innerGrid.SelectedObjects;
					if (!allow) HideScriptSlots(innerGrid);
				}
			}
		}

		
		/// <summary>
		/// Hide script slot fields on a property grid.
		/// </summary>
		protected void HideScriptSlotsOnMainGrid(object sender, SelectedObjectChangedEventArgs e)
		{
			HideScriptSlots((PropertyGrid)sender);
		}
						

		/// <summary>
		/// Check whether the newly opened content is a property grid, and if so,
		/// hide its script slot fields.
		/// </summary>
		/// <param name="c">The content which was just opened.</param>
		/// <param name="cea">Arguments relating to this event.</param>
		protected void HideScriptSlotsOnFloatingGrid(Content c, EventArgs cea)
		{
			if (c.Control is NWN2PropertyGrid) {
				NWN2PropertyGrid grid = (NWN2PropertyGrid)c.Control;
				PropertyGrid innerGrid = (PropertyGrid)innerPropertyGridFieldInfo.GetValue(grid);
				HideScriptSlots(innerGrid);
			}
		}
		
		
		/// <summary>
		/// Hide script slot fields on a property grid.
		/// </summary>
		/// <param name="innerGrid">The property grid.</param>
		protected void HideScriptSlots(PropertyGrid innerGrid)
		{
			PropertyEnumerator enumerator = innerGrid.FirstProperty;
			do {
				if (enumerator.Property != null && enumerator.Property.Name.StartsWith("On")) {
					innerGrid.DeleteProperty(enumerator);
				}
				else {
					enumerator.MoveNext();
				}
			}
			while (enumerator != enumerator.RightBound);
		}
		
		
		public override string ToString()
		{
			return "Flip plugin";
		}
		
		#endregion
	}
}
