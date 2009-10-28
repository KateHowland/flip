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
using System.Collections.Generic;
using System.Drawing;
using System.ServiceModel;

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
		/// <param name="path">The path to create the module at. If 'location'
		/// is set to ModuleLocationType.Directory, this must be the path for
		/// a folder to be created within NWN2Toolset.NWN2ToolsetMainForm.ModulesDirectory.</param>
		/// <param name="location">The serialisation form of the module.</param>
		void CreateModule(string path, NWN2Toolset.NWN2.IO.ModuleLocationType location);
							
				
		/// <summary>
		/// Opens a Neverwinter Nights 2 game module.
		/// </summary>
		/// <param name="name">The path of the module to open.</param>
		/// <param name="location">The serialisation form of the module.</param>
		void OpenModule(string path, NWN2Toolset.NWN2.IO.ModuleLocationType location);
		
		
		/// <summary>
		/// Saves a Neverwinter Nights 2 game module to its
		/// current location.
		/// </summary>
		/// <param name="module">The module to save.</param>.
		void SaveModule(NWN2Toolset.NWN2.Data.NWN2GameModule module);
		
		
		/// <summary>
		/// Saves a Neverwinter Nights 2 game module to a given path.
		/// </summary>
		/// <param name="module">The module to save.</param>
		/// <param name="path">The path to save the module to.</param>
		void SaveModule(NWN2Toolset.NWN2.Data.NWN2GameModule module, string path);
		
				
		/// <summary>
		/// Closes the current module.
		/// </summary>
		void CloseModule();
		
		
		/// <summary>
		/// Gets the module that is currently open in the toolset.
		/// </summary>
		/// <returns>The current module, or null if no module is open.</returns>
		NWN2Toolset.NWN2.Data.NWN2GameModule GetCurrentModule();
		
		
		/// <summary>
		/// Gets the absolute path of the module that is currently open in the toolset.
		/// </summary>
		/// <returns>The absolute path of the current module, or null if no module is open.</returns>
		string GetCurrentModulePath();
		
		
		/// <summary>
		/// Gets the path to the working ('temp') copy of the module that is currently open in the toolset.
		/// </summary>
		/// <returns>The temp path of the current module, or null if no module is open.</returns>
		string GetCurrentModuleTempPath();
		
		
		/// <summary>
		/// Gets the absolute path of a given module.
		/// </summary>
		/// <param name="module">The module to return the path of.</param>
		/// <returns>The absolute path of the given module.</returns>
		string GetModulePath(NWN2Toolset.NWN2.Data.NWN2GameModule module);
		
		
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
		/// Constructs a new <see cref="AreaBase"/> instance.
		/// </summary>
		/// <param name="nwn2area">The Neverwinter Nights 2 area
		/// the <see cref="AreaBase"/> facade will wrap.</param>
		/// <returns>A new <see cref="AreaBase"/> instance.</returns>
		/// <remarks>This is a Factory Method.</remarks>
		AreaBase CreateAreaBase(NWN2Toolset.NWN2.Data.NWN2GameArea nwn2area);
		
		
		/// <summary>
		/// Checks whether the current module has a compiled script
		/// of the given name.
		/// </summary>
		/// <param name="name">The name of the script.</param>
		/// <returns>True if the current module has a .NCS compiled
		/// script file of the given name, and false otherwise.</returns>
		bool HasCompiled(string name);
		

		/// <summary>
		/// Checks whether the current module has an uncompiled script
		/// of the given name.
		/// </summary>
		/// <param name="name">The name of the script.</param>
		/// <returns>True if the current module has a .NSS uncompiled
		/// script file of the given name, and false otherwise.</returns>
		bool HasUncompiled(string name);
	}
}
