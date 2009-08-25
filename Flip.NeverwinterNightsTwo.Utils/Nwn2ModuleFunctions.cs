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
 * This file added by Keiron Nicholson on 14/08/2009 at 14:51.
 */

using System;
using System.IO;
using NWN2Toolset;
using NWN2Toolset.NWN2.Data;
using NWN2Toolset.NWN2.IO;

namespace Sussex.Flip.Games.NeverwinterNightsTwo.Utils
{
	/// <summary> 
	/// Provides functionality relating to 
	/// Neverwinter Nights 2 game modules.
	/// </summary>
	public static class Nwn2ModuleFunctions
	{
		/// <summary>
		/// Creates a Neverwinter Nights 2 game module.
		/// </summary>
		/// <param name="name">The name to give the module.</param>
		/// <param name="location">The serialisation form of the module.</param>
		public static void CreateModule(string name, ModuleLocationType location)
		{
			NWN2GameModule module = new NWN2GameModule();
			module.Name = name;
			module.LocationType = location;
			module.ModuleInfo.Tag = name;
			module.ModuleInfo.Description = new OEIShared.Utils.OEIExoLocString();
			module.ModuleInfo.Description.SetString("I am " + name.ToUpper() + "!",
			                                        OEIShared.Utils.BWLanguages.CurrentLanguage,
			                                        OEIShared.Utils.BWLanguages.Gender.Male);
			
			SaveModule(module);
		}
		
		
		/// <summary>
		/// Opens a Neverwinter Nights 2 game module in the Electron toolset.
		/// </summary>
		/// <param name="name">The name of the module to open.</param>
		/// <param name="location">The serialisation form of the module to open.</param>
		/// <returns>The opened Neverwinter Nights 2 module.</returns>
		/// <remarks>Pass ONLY the name of the module - exclude paths and file extensions.
		/// 
		/// This method opens modules located in the Neverwinter Nights 2
		/// modules directory only. To open a file-based module from elsewhere, 
		/// use the <see cref="OpenFileBasedModule"/> method. (Directory-based modules
		/// cannot be opened from any other location.)
		/// </remarks>
		public static NWN2GameModule OpenModule(string name, ModuleLocationType location)
		{
			if (location == ModuleLocationType.File) {
				string path = GetDefaultModulePathFromName(name);
				return OpenFileBasedModule(path);
			}
			else if (location == ModuleLocationType.Directory) {
				return OpenDirectoryBasedModule(name);
			}
			else {
				throw new ArgumentException("location","Opening " + location + " modules is not supported.");
			}
		}
		
		
		/// <summary>
		/// Opens a directory-based Neverwinter Nights 2 game module.
		/// </summary>
		/// <param name="name">The name of the module to open.</param>
		/// <returns>The opened Neverwinter Nights 2 module.</returns>
		public static NWN2GameModule OpenDirectoryBasedModule(string name)
		{
			ModuleLocationType location = ModuleLocationType.Directory;
			
			ThreadedOpenHelper helper = new ThreadedOpenHelper(NWN2ToolsetMainForm.App,
			                                                   name,
			                                                   location);
			helper.Go();
			
			NWN2GameModule module = NWN2ToolsetMainForm.App.Module;
			
			NWN2ToolsetMainForm.App.SetupHandlersForGameResourceContainer(module);
			
			return module;
		}
		
		
		/// <summary>
		/// Opens a file-based Neverwinter Nights 2 game module.
		/// </summary>
		/// <param name="name">The full path of the module to open, including file extension.</param>
		/// <returns>The opened Neverwinter Nights 2 module.</returns>
		public static NWN2GameModule OpenFileBasedModule(string path)
		{
			ModuleLocationType location = ModuleLocationType.File;
			
			ThreadedOpenHelper helper = new ThreadedOpenHelper(NWN2ToolsetMainForm.App,
			                                                   path,
			                                                   location);
			helper.Go();
			
			NWN2GameModule module = NWN2ToolsetMainForm.App.Module;
			
			NWN2ToolsetMainForm.App.SetupHandlersForGameResourceContainer(module);
			
			return module;
		}
		
		
		/// <summary>
		/// Saves changes to a Neverwinter Nights 2 game module to disk.
		/// </summary>
		/// <param name="module">The module to save.</param>
		/// <remarks>Saves to the default modules directory.</remarks>
		public static void SaveModule(NWN2GameModule module)
		{			
			switch (module.LocationType) {
				case ModuleLocationType.Directory:
					module.OEISerialize(module.Name);
					break;
				case ModuleLocationType.File:
					string path = GetDefaultModulePathFromName(module.Name);
					module.OEISerialize(path);
					break;
				default:
					throw new ArgumentException("location","Saving " + module.LocationType + " modules is not supported.");
			}
		}
		
		
		private static string GetDefaultModulePathFromName(string name)
		{
			return Path.Combine(NWN2ToolsetMainForm.ModulesDirectory,name + ".mod");
		}
	}
}
