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
 * This file added by Keiron Nicholson on 10/08/2009 at 18:08.
 */

using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Microsoft.Win32;

namespace Sussex.Flip.Games.NeverwinterNightsTwo.Utils
{
	/// <summary>
	/// Provides functionality relating to the Neverwinter Nights 2 toolset.
	/// </summary>
	public static class Nwn2ToolsetFunctions
	{
		#region Constants
		
		public const string ToolsetLauncherDefaultName = "NWN2ToolsetLauncher"; // assumes .exe filename has not changed!
		
		#endregion
		
		/// <summary>
		/// Runs the Neverwinter Nights 2 toolset.
		/// </summary>
		/// <exception cref="NotSupportedException">Neverwinter Nights 2
		/// install directory could not be found.</exception>
		/// <exception cref="Win32Exception">Neverwinter Nights 2
		/// toolset launcher application could not be found.</exception>
		public static void RunNeverwinterNightsTwoToolset()
		{
			ProcessStartInfo psi = new ProcessStartInfo();
			psi.WorkingDirectory = GetNeverwinterNightsTwoInstallPath();
			psi.FileName = ToolsetLauncherDefaultName + ".exe";
			Process.Start(psi);			
		}	
		
		
		/// <summary>
		/// Runs the Neverwinter Nights 2 toolset and returns true
		/// once it has loaded, or false after a timeout period has passed.
		/// </summary>
		/// <param name="timeout">The number of milliseconds to wait for
		/// the toolset to run before returning false.</param>
		/// <returns>True once the toolset has loaded, if this is within
		/// the timeout period; false
		/// if the timeout period expired.</returns>
		/// <exception cref="NotSupportedException">Neverwinter Nights 2
		/// install directory could not be found.</exception>
		/// <exception cref="Win32Exception">Neverwinter Nights 2
		/// toolset launcher application could not be found.</exception>
		public static bool RunNeverwinterNightsTwoToolset(int timeout)
		{
			int wait = 1000;
		
			if (timeout < wait) 
				throw new ArgumentException("Timeout period must exceed 1000ms.","timeout");
			
			RunNeverwinterNightsTwoToolset();
									
			for (int i = 0; i < timeout/wait; i++) {
				if (Process.GetProcessesByName(Nwn2ToolsetFunctions.ToolsetLauncherDefaultName).Length > 0) {
					return true;
				}
				
				Thread.Sleep(wait);
			}
			
			return false;
		}
		
		
		/// <summary>
		/// Immediately shuts down all copies of the Neverwinter Nights 2 toolset.
		/// </summary>
		public static void KillNeverwinterNightsTwoToolset()
		{
			Process[] processes = Process.GetProcessesByName(ToolsetLauncherDefaultName);
			foreach (Process toolset in processes) {
				if (!toolset.HasExited)	{
					try {
						toolset.Kill();
					}
					catch (InvalidOperationException) { 
						// process already exited
					}
				}
			}
		}
		
		
		/// <summary>
		/// Retrieves the path of the Neverwinter Nights 2 installation
		/// from registry.
		/// </summary>
		/// <exception cref="NotSupportedException">Neverwinter Nights 2
		/// wasn't found in the local machine's registry.</exception>
		/// <returns>The Neverwinter Nights 2 install path.</returns>
		public static string GetNeverwinterNightsTwoInstallPath()
		{
			RegistryKey key = Registry.LocalMachine;
			
			key = key.OpenSubKey(@"SOFTWARE\Obsidian\NWN 2\Neverwinter");
			
			if (key == null) 
				throw new NotSupportedException("Neverwinter Nights 2 could not be found " +
				                                "in the local machine's registry.");
			
			string location = (string)key.GetValue("Location");
			
			if (location == null)
				throw new NotSupportedException("The Neverwinter Nights 2 location field " +
				                                "in the local machine's registry was empty.");
			
			return location;
		}
		
		
		public static bool ToolsetIsOpen()
		{
			// TODO:
			// check this works when working with the actual toolset and not just Flip.UI.Generic:
			return NWN2Toolset.NWN2ToolsetMainForm.App != null;
		}
	}
}
