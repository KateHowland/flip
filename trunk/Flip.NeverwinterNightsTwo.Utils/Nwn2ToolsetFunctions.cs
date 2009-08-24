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

namespace Sussex.Flip.Games.NeverwinterNightsTwo.Utils
{
	/// <summary>
	/// Provides functionality relating to the Neverwinter Nights 2 Electron toolset.
	/// </summary>
	public static class Nwn2ToolsetFunctions
	{
		/// <summary>
		/// Runs the Neverwinter Nights 2 toolset.
		/// </summary>
		public static void RunNeverwinterNightsTwoToolset()
		{
			string location = GetNeverwinterNightsTwoLocation();
			
			if (location == null)
				throw new NotSupportedException("Neverwinter Nights 2 is not listed in registry.");
			
			System.Diagnostics.Process.Start(location);
		}
		
		
		/// <summary>
		/// Retrieves the path of the Neverwinter Nights 2 installation
		/// from registry.
		/// </summary>
		/// <returns>The Neverwinter Nights 2 install path.</returns>
		public static string GetNeverwinterNightsTwoLocation()
		{
			throw new NotImplementedException();
		}
	}
}
