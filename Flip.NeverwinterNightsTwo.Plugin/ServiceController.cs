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
 * This file added by Keiron Nicholson on 15/01/2010 at 15:03.
 */

using System;
using System.Diagnostics;
using System.ServiceModel;
using Sussex.Flip.Games.NeverwinterNightsTwo;
using Sussex.Flip.Games.NeverwinterNightsTwo.Utils;

namespace Sussex.Flip.Games.NeverwinterNightsTwo.Plugin
{
	/// <summary>
	/// Manages the service provided by Nwn2SessionAdapter.
	/// </summary>
	public class ServiceController
	{
		#region Fields
		
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
		/// True if a client has connected to this service; false otherwise.
		/// </summary>
		/// </remarks>
		public bool Connected {
			get { return connected; }
		}
		
		#endregion
		
		#region Constructors
		
		public ServiceController()
		{
		}
		
		#endregion
		
		#region Methods
		
		/// <summary>
		/// Start hosting services.
		/// </summary>
		public void Start()
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
			catch (AddressAlreadyInUseException) {
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
				Process.GetCurrentProcess().Kill();
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
		public void Stop()
		{
			if (connected && host != null && host.State != CommunicationState.Closed && host.State != CommunicationState.Closing) {
				host.Close();
			}
		}
		
		#endregion
	}
}
