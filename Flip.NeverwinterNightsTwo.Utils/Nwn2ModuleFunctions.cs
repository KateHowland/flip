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
			
			//if (location == ModuleLocationType.File) name += ".mod";
			//module.FileName = Path.Combine(NWN2ToolsetMainForm.ModulesDirectory,name);
			//module.FileName = name;
			
			SaveModule(module);
		}
		
		
		/// <summary>
		/// Opens a Neverwinter Nights 2 game module in the Electron toolset.
		/// </summary>
		/// <param name="name">The name of the module to open.</param>
		/// <param name="location">The serialisation form of the module to open.</param>
		/// <returns>The opened Neverwinter Nights 2 module.</returns>
		public static NWN2GameModule OpenModule(string name, ModuleLocationType location)
		{
			ThreadedOpenHelper helper = new ThreadedOpenHelper(NWN2ToolsetMainForm.App,
			                                                   name,
			                                                   location);
			helper.Go();
			
			NWN2GameModule module = NWN2ToolsetMainForm.App.Module;
			
			NWN2ToolsetMainForm.App.SetupHandlersForGameResourceContainer(module);
			
			System.Windows.Forms.MessageBox.Show("Opened '" + name + "' at " + location);
			
			return module;
		}
		
		
		/// <summary>
		/// Saves changes to a Neverwinter Nights 2 game module to disk.
		/// </summary>
		/// <param name="module">The module to save.</param>
		public static void SaveModule(NWN2GameModule module)
		{			
			//string path = Path.Combine(NWN2ToolsetMainForm.ModulesDirectory,module.Name);				
			//string path = Path.Combine(NWN2ToolsetMainForm.ModulesDirectory,module.Name + ".mod");	
			Report(module,"before");
			string path = module.Name;
			if (module.LocationType == ModuleLocationType.File) {
				path = Path.Combine(NWN2ToolsetMainForm.ModulesDirectory,path += ".mod");
			}
			System.Windows.Forms.MessageBox.Show("serializing to " + path);
			module.OEISerialize(path);
			Report(module,"after");
		}
		
		
		private static void Report(NWN2GameModule mod, string prepend)
		{			
			System.Windows.Forms.MessageBox.Show(prepend + "\n\nmodule.Name= "
			                                     + mod.Name + "\n\nmodule.FileName= "
			                                     + mod.FileName + "\nmodule.Repository.Name= " +
			                                     mod.Repository.Name + "\nmodule.Repository.DirectoryName= " + 
			                                     mod.Repository.DirectoryName);
		}
	}
}
