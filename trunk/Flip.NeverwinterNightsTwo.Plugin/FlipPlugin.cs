/*
 * Flip - a visual programming language for scripting video games
 * Copyright (C) 2009 University of Sussex
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
		/// User preferences relating to the operation of this plugin.
		/// </summary>
		protected object preferences;
				
		/// <summary>
		/// The host for services provided by this plugin.
		/// </summary>
		protected ServiceHostBase host;
		
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
			pluginMenuItem = null;
			preferences = new object();
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
		}
		
		
		/// <summary>
		/// Performs setup operations. 
		/// Called by the toolset when it has finished loading.
		/// </summary>
		/// <param name="cHost">A plugin host component which
		/// manages the plugins currently loaded into the toolset.</param>
		public void Load(INWN2PluginHost cHost)
		{			
			BlockAccessToScripts();
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
			} 
			catch (Exception e) {
				System.Windows.MessageBox.Show("Error starting services.\n\n" + e);
			}
		}
		
		
		/// <summary>
		/// Stop hosting services.
		/// </summary>
		protected void StopServices()
		{
			if (host != null && host.State != CommunicationState.Closed && host.State != CommunicationState.Closing) {
				host.Close();
			}
		}
		
		
		/// <summary>
		/// Hide script slot fields on property grids, and prevent scripts from being created or opened.
		/// </summary>
		protected void BlockAccessToScripts()
		{
			FieldInfo[] fields = typeof(NWN2ToolsetMainForm).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
			
			// Locate all the necessary UI elements via reflection:
			foreach (FieldInfo field in fields) {
				
				// Hide script slots on the main property grid whenever it changes:
				if (field.FieldType == typeof(NWN2PropertyGrid)) {
					NWN2PropertyGrid grid = (NWN2PropertyGrid)field.GetValue(NWN2ToolsetMainForm.App);	
					foreach (FieldInfo fi in grid.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic)) {
						if (fi.FieldType == typeof(PropertyGrid)) {
							PropertyGrid innerGrid = (PropertyGrid)fi.GetValue(grid);
							innerGrid.SelectedObjectChanged += delegate	{ HideScriptSlots(innerGrid); };
						}
					}
				}
				
				// Hide script slots on floating property grids when they first appear:
				else if (field.FieldType == typeof(DockingManager)) {
					DockingManager dm = (DockingManager)field.GetValue(NWN2ToolsetMainForm.App);	
					dm.ContentShown += new DockingManager.ContentHandler(HideScriptSlots);
				}
				
				// Disable the scripts window:
				else if (field.FieldType == typeof(NWN2ModuleScriptList)) {
					NWN2ModuleScriptList sl = (NWN2ModuleScriptList)field.GetValue(NWN2ToolsetMainForm.App);
					sl.Enabled = false;
				}
				
				// Disable the ability to create or open scripts from the file menu:
				else if (field.FieldType == typeof(TD.SandBar.MenuButtonItem)) {
					TD.SandBar.MenuButtonItem mbi = (TD.SandBar.MenuButtonItem)field.GetValue(NWN2ToolsetMainForm.App);
					if (mbi.Text == "&Script" || mbi.Text == "Open Conversation/Script") mbi.Enabled = false;
				}
			}
		}
						

		/// <summary>
		/// Check whether the newly opened content is a property grid, and if so,
		/// hide its script slot fields.
		/// </summary>
		/// <param name="c">The content which was just opened.</param>
		/// <param name="cea">Arguments relating to this event.</param>
		protected void HideScriptSlots(Content c, EventArgs cea)
		{
			if (c.Control is NWN2PropertyGrid) {
				NWN2PropertyGrid grid = (NWN2PropertyGrid)c.Control;
				foreach (FieldInfo fi in grid.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic)) {
					if (fi.FieldType == typeof(PropertyGrid)) {
						PropertyGrid innerGrid = (PropertyGrid)fi.GetValue(grid);
						HideScriptSlots(innerGrid);
						break;
					}
				}
			}
		}
		
		
		/// <summary>
		/// Hide script slot fields on a property grid.
		/// </summary>
		/// <param name="innerGrid"></param>
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
