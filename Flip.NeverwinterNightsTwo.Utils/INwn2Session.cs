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
 * This file added by Keiron Nicholson on 23/09/2009 at 12:57.
 */

using System;
using System.Drawing;
using System.ServiceModel;
using NWN2Toolset.NWN2.Data;
using NWN2Toolset.NWN2.IO;

namespace Sussex.Flip.Games.NeverwinterNightsTwo.Utils
{
	/// <summary>
	/// A facade for a Neverwinter Nights 2 toolset session.
	/// </summary>
	public interface INwn2Session
	{		
		/// <summary>
		/// Creates a Neverwinter Nights 2 game module.
		/// </summary>
		/// <param name="name">The name to give the module.</param>
		/// <param name="location">The type of module to create.</param>
		void CreateModule(string name, ModuleLocationType location);
		
		
		/// <summary>
		/// Opens a directory-based Neverwinter Nights 2 game module.
		/// </summary>
		/// <param name="name">The name of the module to open.</param>
		void OpenDirectoryModule(string name);
		
		
		/// <summary>
		/// Opens a file-based Neverwinter Nights 2 game module.
		/// </summary>
		/// <param name="name">The full path of the module to open, including file extension.</param>
		void OpenFileModule(string path);
		
		
		/// <summary>
		/// Saves changes to a Neverwinter Nights 2 game module to disk.
		/// </summary>
		/// <param name="module">The module to save.</param>
		/// <remarks>Saves to the default modules directory.</remarks>
		void SaveModule(NWN2Toolset.NWN2.Data.NWN2GameModule module);
		
		
		/// <summary>
		/// Gets the module that is currently open in the toolset.
		/// </summary>
		/// <returns>The current module, or null if no module is open.</returns>
		NWN2Toolset.NWN2.Data.NWN2GameModule GetCurrentModule();
		
		
		/// <summary>
		/// Adds an area to the current module.
		/// </summary>
		/// <param name="name">The name to give the area.</param>
		/// <param name="exterior">True to create an exterior area
		/// with terrain; false to create an interior area with tiles.</param>
		/// <param name="size">The size of area to create.</param>
		/// <returns>A facade for an empty Neverwinter Nights 2 area.</returns>
		AreaBase AddArea(string name, bool exterior, Size size);
		
			
		/// <summary>
		/// Adds an area to a given module.
		/// </summary>
		/// <param name="module">The module to add the area to.</param>
		/// <param name="name">The name to give the area.</param>
		/// <param name="exterior">True to create an exterior area
		/// with terrain; false to create an interior area with tiles.</param>
		/// <param name="size">The size of area to create.</param>
		/// <returns>A facade for an empty Neverwinter Nights 2 area.</returns>
		AreaBase AddArea(NWN2GameModule module, string name, bool exterior, Size size);
	}
}