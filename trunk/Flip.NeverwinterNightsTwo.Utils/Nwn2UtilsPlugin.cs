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
using NWN2Toolset.NWN2.Data;
using NWN2Toolset.NWN2.IO;
using NWN2Toolset.Plugins;

namespace Sussex.Flip.Games.NeverwinterNightsTwo.Utils
{
	/// <summary>
	/// A plugin which provides some useful generic
	/// functions relating to the Neverwinter Nights
	/// 2 toolset.
	/// </summary>
	public class Nwn2UtilsPlugin : INWN2Plugin
	{		
		#region Constants
		
		/// <summary>
		/// The name of the plugin.
		/// </summary>
		public static string NAME = "NWN2 Utils";
		
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
				return Nwn2UtilsPlugin.NAME;
			}
		}
		
		
		/// <summary>
		/// The display name of the plugin.
		/// </summary>
		public string DisplayName {
			get {
				return Nwn2UtilsPlugin.NAME;
			}
		}
		
		
		/// <summary>
		/// The menu name of the plugin.
		/// </summary>
		public string MenuName {
			get {
				return Nwn2UtilsPlugin.NAME;
			}
		}
		
		#endregion
		
		#region Constructors
		
		/// <summary>
		/// Constructs a new <see cref="NWN2UtilsPlugin"/> instance.
		/// </summary>
		public Nwn2UtilsPlugin()
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
		}
		
	
		/// <summary>
		/// Runs the plugin.
		/// </summary>
		protected void PluginActivated(object sender, EventArgs e)
		{			
			ModuleLocationType location;
			//ModuleLocationType location = ModuleLocationType.Directory;
			
			string name = "module" + DateTime.Now.Millisecond;
			
			System.Windows.Forms.DialogResult result = 
			System.Windows.Forms.MessageBox.Show("Directory?","Yes for Directory, No for File.",
			                                     System.Windows.Forms.MessageBoxButtons.YesNo);
			location = result == System.Windows.Forms.DialogResult.Yes ? ModuleLocationType.Directory : ModuleLocationType.File;
			
			Nwn2TestingFunctions.CreateTestModule(name,location);
			Nwn2ModuleFunctions.OpenModule(name,location);
		}
		
		#endregion
	}
}
