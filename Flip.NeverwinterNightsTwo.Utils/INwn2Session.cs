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
using System.ServiceModel;

namespace Sussex.Flip.Games.NeverwinterNightsTwo.Utils
{
	/// <summary>
	/// Description of INwn2Session.
	/// </summary>
	[ServiceContract]
	public interface INwn2Session
	{
		/// <summary>
		/// Creates a Neverwinter Nights 2 game module.
		/// </summary>
		/// <param name="name">The name to give the module.</param>
		/// <param name="location">The serialisation form of the module.</param>
		[OperationContract]
		void CreateModule(string name, NWN2Toolset.NWN2.IO.ModuleLocationType location);
		
		
		/// <summary>
		/// Opens a Neverwinter Nights 2 game module in the Electron toolset.
		/// </summary>
		/// <param name="name">The name of the module to open.</param>
		/// <param name="location">The serialisation form of the module to open.</param>
		/// <remarks>Pass ONLY the name of the module - exclude paths and file extensions.
		/// 
		/// This method opens modules located in the Neverwinter Nights 2
		/// modules directory only. To open a file-based module from elsewhere, 
		/// use the <see cref="OpenFileBasedModule"/> method. (Directory-based modules
		/// cannot be opened from any other location.)
		/// </remarks>
		[OperationContract]
		void OpenModule(string name, NWN2Toolset.NWN2.IO.ModuleLocationType location);
		
		
		/// <summary>
		/// Opens a directory-based Neverwinter Nights 2 game module.
		/// </summary>
		/// <param name="name">The name of the module to open.</param>
		[OperationContract]
		void OpenDirectoryBasedModule(string name);
		
		
		/// <summary>
		/// Opens a file-based Neverwinter Nights 2 game module.
		/// </summary>
		/// <param name="name">The full path of the module to open, including file extension.</param>
		[OperationContract]
		void OpenFileBasedModule(string path);
		
		
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
		/// Gets the name of the module that is currently open in the toolset.
		/// </summary>
		/// <returns>The name of the current module, or null if no module is open.</returns>
		[OperationContract]
		string GetCurrentModuleName();
	}
}
