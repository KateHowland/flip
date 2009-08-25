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
 * This file added by Keiron Nicholson on 10/08/2009 at 18:08.
 */

using System;
using System.Diagnostics;
using System.IO;
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
			string location = GetNeverwinterNightsTwoInstallPath();
			
			if (location == null || !System.IO.Directory.Exists(location))
				throw new NotSupportedException("Failed to locate the Neverwinter Nights 2 install.");
			
			location = Path.Combine(location,ToolsetLauncherDefaultName + ".exe");
			
			Process.Start(location);
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
	}
}
