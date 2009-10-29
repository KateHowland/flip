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
using NWN2Toolset.NWN2.Data;
using NWN2Toolset.NWN2.IO;
using NWN2Toolset.Plugins;
using TD.SandBar;

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
		protected MenuButtonItem pluginMenuItem;
		
		/// <summary>
		/// User preferences relating to the operation of this plugin.
		/// </summary>
		protected object preferences;
				
		/// <summary>
		/// Provides methods for this NWN2 session.
		/// </summary>
		protected INwn2Session session;
		
		#endregion
		
		#region Properties
		
		/// <summary>
		/// The menu button which activates the plugin.
		/// </summary>
		public MenuButtonItem PluginMenuItem {
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
		/// Provides methods for this NWN2 session.
		/// </summary>
		public INwn2Session Session {
			get { 
				return session; 
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
			session = new Nwn2Session();
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
			
			// Reflect over the INwn2Session object to get all parameterless
			// public methods and allow the user to call them via the menu:
			foreach (MethodInfo method in session.GetType().GetMethods()) {
				MenuButtonItem item = new MenuButtonItem(method.Name);
				if (method.GetParameters().Length > 0) {
					item.Enabled = false;
				}
				else {
					item.Activate += delegate 
					{  
						object returned = method.Invoke(session,null);
					};
				}
				
				pluginMenuItem.Items.Add(item);
			}
			
			EnableDebuggingMethods();
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
		/// Makes a set of methods available through the plugin menu
		/// which are for debugging purposes.
		/// </summary>
		private void EnableDebuggingMethods()
		{			
			MenuButtonItem debuggingMethods = new MenuButtonItem("Debugging methods");
			pluginMenuItem.Items.Add(debuggingMethods);
			
			MenuButtonItem checkAreaTerrainProperties = new MenuButtonItem("Check area terrain properties");
			debuggingMethods.Items.Add(checkAreaTerrainProperties);
			checkAreaTerrainProperties.Activate += delegate 
			{
				string message;
				NWN2GameArea area = NWN2Toolset.NWN2ToolsetMainForm.App.AreaContents.Area;
				if (area == null) {
					message = "No module open.";
				}
				else {
					message = 	
						"-- " + area.Natural + " --" + Environment.NewLine +
						"HasTerrain: " + area.HasTerrain + Environment.NewLine +
						"Interior: " + area.Interior + Environment.NewLine +
						"Natural: " + area.Natural + Environment.NewLine +
						"Skybox: " + area.Skybox + Environment.NewLine +
						"TerrainFlags: " + area.TerrainFlags + Environment.NewLine +
						"TerrainResource: " + area.TerrainResource + Environment.NewLine +
						"TerrainWalkmeshResource: " + area.TerrainWalkmeshResource + Environment.NewLine +
						"Tiles: " + area.Tiles + Environment.NewLine +
						"Underground: " + area.Underground;
				}
				System.Windows.Forms.MessageBox.Show(message);
			};
			
			MenuButtonItem createInteriorAndExteriorArea = new MenuButtonItem("Create interior and exterior area");
			debuggingMethods.Items.Add(createInteriorAndExteriorArea);
			createInteriorAndExteriorArea.Activate += delegate 
			{
				System.Drawing.Size size = new System.Drawing.Size(8,16);
				session.AddArea("interior",false,size);
				session.AddArea("exterior",true,size);
			};
			
			MenuButtonItem reportOnAllScripts = new MenuButtonItem("reportOnAllScripts");
			debuggingMethods.Items.Add(reportOnAllScripts);
			reportOnAllScripts.Activate += delegate 
			{
				NWN2GameModule module = NWN2Toolset.NWN2ToolsetMainForm.App.Module;
				if (module != null) {
					System.Windows.Forms.MessageBox.Show("uncompiled scripts:");
					foreach (NWN2GameScript script in module.Scripts.Values) {
						bool loaded = script.Loaded;
						if (!loaded) script.Demand();
						System.Windows.Forms.MessageBox.Show(new Bean(script).ToString());
						if (!loaded) script.Release();
					}
					
					System.Windows.Forms.MessageBox.Show("compiled scripts:");
										
					ushort NCS = 2010;
					OEIShared.Utils.OEIGenericCollectionWithEvents<OEIShared.IO.IResourceEntry> resources = module.Repository.FindResourcesByType(NCS);
					
					foreach (OEIShared.IO.IResourceEntry r in resources) {
						NWN2GameScript script = new NWN2GameScript(r);
						script.Demand();
						System.Windows.Forms.MessageBox.Show(new Bean(script).ToString());
						script.Release();
					}
				}
				else {
					System.Windows.Forms.MessageBox.Show("Module was null.");
				}
			};
		}
		
		#endregion
	}
}
