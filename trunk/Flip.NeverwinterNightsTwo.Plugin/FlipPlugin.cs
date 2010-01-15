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
		/// User preferences relating to the operation of this plugin.
		/// </summary>
		protected object preferences;
				
		/// <summary>
		/// The host for services provided by this plugin.
		/// </summary>
		protected ServiceHostBase host;
		
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
			
			ToolsetUIModifier UI = new ToolsetUIModifier();
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
		
		
		public override string ToString()
		{
			return "Flip plugin";
		}
		
		#endregion
	}
}
