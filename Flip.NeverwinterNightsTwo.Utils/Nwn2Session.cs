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
 * This file added by Keiron Nicholson on 23/09/2009 at 12:58.
 */

using System;
using System.Drawing;
using System.IO;
using NWN2Toolset;
using NWN2Toolset.NWN2.IO;
using NWN2Toolset.NWN2.Data;

namespace Sussex.Flip.Games.NeverwinterNightsTwo.Utils
{
	/// <summary>
	/// A facade for a Neverwinter Nights 2 toolset session.
	/// </summary>
	public class Nwn2Session : INwn2Session
	{
		#region Constructors
		
		/// <summary>
		/// Constructs a new <see cref="Nwn2Session"/> instance.
		/// </summary>
		public Nwn2Session()
		{
		}
		
		#endregion
		
		#region Methods
		
		/// <summary>
		/// Creates a Neverwinter Nights 2 game module.
		/// </summary>
		/// <param name="name">The name to give the module.</param>
		/// <param name="location">The type of module to create - either Directory
		/// or File. Creating Temporary modules is not supported.</param>
		public void CreateModule(string name, ModuleLocationType location)
		{
			if (location == ModuleLocationType.Temporary) 
				throw new NotSupportedException("Creating temporary modules is not supported.");
			
			NWN2GameModule module = new NWN2GameModule();
			module.Name = name;
			module.LocationType = location;
			module.ModuleInfo.Tag = name;
			module.ModuleInfo.Description = new OEIShared.Utils.OEIExoLocString();
			
			SaveModule(module);
		}
		
		
		/// <summary>
		/// Creates a directory-based Neverwinter Nights 2 game module.
		/// </summary>
		/// <param name="name">The name to give the module.</param>
		public void CreateDirectoryModule(string name)
		{
			CreateModule(name,ModuleLocationType.Directory);
		}
				
		
		/// <summary>
		/// Creates a file-based Neverwinter Nights 2 game module.
		/// </summary>
		/// <param name="name">The name to give the module.</param>
		public void CreateFileModule(string name)
		{
			CreateModule(name,ModuleLocationType.File);
		}
						
		
		/// <summary>
		/// Opens a directory-based Neverwinter Nights 2 game module.
		/// </summary>
		/// <param name="name">The name of the module to open.</param>
		public void OpenDirectoryModule(string name)
		{
			ModuleLocationType location = ModuleLocationType.Directory;
			
			ThreadedOpenHelper helper = new ThreadedOpenHelper(NWN2ToolsetMainForm.App,
			                                                   name,
			                                                   location);
			helper.Go();			
									
			NWN2ToolsetMainForm.App.SetupHandlersForGameResourceContainer(GetCurrentModule());
		}
		
		
		/// <summary>
		/// Opens a file-based Neverwinter Nights 2 game module.
		/// </summary>
		/// <param name="name">The full path of the module to open, including file extension.</param>
		/// <remarks>Use the static method <see cref="GetExpectedPathForFileBasedModule"/> to
		/// get the expected path from the module name.</remarks>
		public void OpenFileModule(string path)
		{
			ModuleLocationType location = ModuleLocationType.File;
			
			ThreadedOpenHelper helper = new ThreadedOpenHelper(NWN2ToolsetMainForm.App,
			                                                   path,
			                                                   location);
			helper.Go();
									
			NWN2ToolsetMainForm.App.SetupHandlersForGameResourceContainer(GetCurrentModule());
		}
		
		
		/// <summary>
		/// Saves a Neverwinter Nights 2 game module to the
		/// modules directory.
		/// </summary>
		/// <param name="module">The module to save.</param>
		public void SaveModule(NWN2GameModule module)
		{			
			switch (module.LocationType) {
				case ModuleLocationType.Directory:
					module.OEISerialize(module.Name);
					break;
				case ModuleLocationType.File:
					string path = GetExpectedPathForFileBasedModule(module.Name);
					module.OEISerialize(path);
					break;
				default:
					throw new ArgumentException("location","Saving " + module.LocationType + 
					                            " modules is not supported.");
			}
		}
		
		
		/// <summary>
		/// Gets the module that is currently open in the toolset.
		/// </summary>
		/// <returns>The current module, or null if no module is open.</returns>
		public NWN2Toolset.NWN2.Data.NWN2GameModule GetCurrentModule()
		{
			return NWN2ToolsetMainForm.App.Module;
		}	
		
		
		/// <summary>
		/// Gets the expected path for a file-based module of the given name,
		/// assuming that the module is stored in the modules directory.
		/// </summary>
		/// <param name="name">The name of the module.</param>
		/// <returns>The expected path for a file-based module of the given name.</returns>
		public static string GetExpectedPathForFileBasedModule(string name)
		{
			return Path.Combine(NWN2ToolsetMainForm.ModulesDirectory,name + ".mod");
		}		
		
		
		/// <summary>
		/// Adds an area to the current module.
		/// </summary>
		/// <param name="name">The name to give the area.</param>
		/// <param name="exterior">True to create an exterior area
		/// with terrain; false to create an interior area with tiles.</param>
		/// <param name="size">The size of area to create.</param>
		/// <returns>A facade for an empty Neverwinter Nights 2 area.</returns>
		public AreaBase AddArea(string name, bool exterior, Size size)
		{
			if (GetCurrentModule() == null) throw new InvalidOperationException("No module open.");
			
			return AddArea(GetCurrentModule(),name,exterior,size);
		}
		
			
		/// <summary>
		/// Adds an area to a given module.
		/// </summary>
		/// <param name="module">The module to add the area to.</param>
		/// <param name="name">The name to give the area.</param>
		/// <param name="exterior">True to create an exterior area
		/// with terrain; false to create an interior area with tiles.</param>
		/// <param name="size">The size of area to create.</param>
		/// <returns>A facade for an empty Neverwinter Nights 2 area.</returns>
		public AreaBase AddArea(NWN2GameModule module, string name, bool exterior, Size size)
		{
			if (module == null) throw new ArgumentNullException("module");
			
			// TODO: Check that module.Repository.Name works for both types of module:
			NWN2GameArea nwn2area = new NWN2GameArea(name,
				                                     module.Repository.Name,
				                                     module.Repository);
			nwn2area.Tag = name;
			nwn2area.HasTerrain = exterior;
			nwn2area.Size = size;
			
			module.AddResource(nwn2area);
			
			nwn2area.OEISerialize();
			
			return CreateAreaBase(nwn2area);
		}
		
		
		/// <summary>
		/// Constructs a new <see cref="AreaBase"/> instance.
		/// </summary>
		/// <param name="nwn2area">The Neverwinter Nights 2 area
		/// the <see cref="AreaBase"/> facade will wrap.</param>
		/// <returns>A new <see cref="AreaBase"/> instance.</returns>
		/// <remarks>This is a Factory Method.</remarks>
		protected AreaBase CreateAreaBase(NWN2GameArea nwn2area)
		{
			return new Area(nwn2area);
		}
		
		#endregion
	}
}
