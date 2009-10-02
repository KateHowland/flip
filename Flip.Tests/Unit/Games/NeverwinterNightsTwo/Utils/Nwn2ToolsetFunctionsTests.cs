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
 * This file added by Keiron Nicholson on 25/08/2009 at 10:16.
 */


using System;
using System.Threading;
using System.Diagnostics;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Sussex.Flip.Games.NeverwinterNightsTwo.Utils;

namespace Sussex.Flip.Games.NeverwinterNightsTwo.Utils.Tests
{	
	/// <summary>
	/// Tests the <see cref="Nwn2ToolsetFunctions"/> class.
	/// </summary>
	[TestFixture]
	public class Nwn2ToolsetFunctionsTests
	{	
		#region Setup
		
		
		
		#endregion
		
		#region Tests
				
		[Test]
		public void LaunchesToolset()
		{
			Process[] processes = Process.GetProcessesByName(Nwn2ToolsetFunctions.ToolsetLauncherDefaultName);
			if (processes.Length > 0) {
				Assert.Fail("Toolset was already running when test began.");
			}
						
			Nwn2ToolsetFunctions.RunNeverwinterNightsTwoToolset();
						
			for (int i = 0; i < 20; i++) { // make 20 checks over 5 seconds before failing.
				processes = Process.GetProcessesByName(Nwn2ToolsetFunctions.ToolsetLauncherDefaultName);
				
				if (processes.Length == 1) {
					processes[0].Kill(); // kill the toolset once the process has been found
					return;
				}
			
				Assert.Less(processes.Length,2,"Only 1 toolset process should be running, " +
				            "but " + processes.Length + " were found.");
				
				Thread.Sleep(250);
			}
			
			Assert.Fail("Toolset did not run (within 5 seconds of method call).");
		}
			
				
		[Test]
		public void KillsToolset()
		{			
			Process[] processes = Process.GetProcessesByName(Nwn2ToolsetFunctions.ToolsetLauncherDefaultName);
			if (processes.Length > 0) {
				Assert.Fail("Toolset was already running when test began.");
			}
						
			Nwn2ToolsetFunctions.RunNeverwinterNightsTwoToolset();
			
			Process toolset = null;
			
			for (int i = 0; i < 20; i++) { // check for 5 seconds before failing.
				processes = Process.GetProcessesByName(Nwn2ToolsetFunctions.ToolsetLauncherDefaultName);
				
				if (processes.Length == 1) {
					toolset = processes[0];
					break;
				}
			
				Assert.Less(processes.Length,2,"Only 1 toolset process should be running, " +
				            "but " + processes.Length + " were found.");
				
				Thread.Sleep(250);
			}
			
			Assert.IsNotNull(toolset,"Toolset did not run, so the toolset process could not be ended.");
			
			Nwn2ToolsetFunctions.KillNeverwinterNightsTwoToolset();
						
			for (int i = 0; i < 20; i++) { // check for 5 seconds before failing.				
				if (toolset.HasExited) 
					return;
				Thread.Sleep(250);
			}
			
			Assert.Fail("Toolset process was not ended.");
		}	
		
		#endregion
	}
}
