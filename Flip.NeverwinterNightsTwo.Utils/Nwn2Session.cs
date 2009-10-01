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
		/// <param name="path">The path to create the module at. If 'location'
		/// is set to ModuleLocationType.Directory, this must be the path for
		/// a folder to be created within NWN2Toolset.NWN2ToolsetMainForm.ModulesDirectory.</param>
		/// <param name="location">The serialisation form of the module.</param>
		public void CreateModule(string path, ModuleLocationType location)
		{			
			if (location == ModuleLocationType.Temporary) 
				throw new NotSupportedException("Creating temporary modules is not supported.");
			
			if (path == null) throw new ArgumentNullException("path");
			if (path == String.Empty) throw new ArgumentException("path");
			
			if (location == ModuleLocationType.Directory && Directory.Exists(path) ||
			    location == ModuleLocationType.File && File.Exists(path)) {
				throw new IOException("The path provided was already occupied (" + path + ").");
			}
						
			string name = Path.GetFileNameWithoutExtension(path);
			
			NWN2GameModule module = new NWN2GameModule();
			module.Name = name;
			module.LocationType = location;
			module.ModuleInfo.Tag = name;
			module.ModuleInfo.Description = new OEIShared.Utils.OEIExoLocString();
			
			SaveModule(module,path);
		}		
		
		
		/// <summary>
		/// Opens a Neverwinter Nights 2 game module.
		/// </summary>
		/// <param name="name">The path of the module, including file extension
		/// if appropriate.</param>
		/// <param name="location">The serialisation form of the module.</param>
		public void OpenModule(string path, ModuleLocationType location)
		{
			if (path == null) throw new ArgumentNullException("path");
			if (path == String.Empty) throw new ArgumentException("path");
			
			ThreadedOpenHelper opener;
			
			switch (location) {
				case ModuleLocationType.Directory:
					string name = Path.GetFileName(path);
					opener = new ThreadedOpenHelper(NWN2ToolsetMainForm.App,name,location);
					break;
					
				case ModuleLocationType.Temporary:
				case ModuleLocationType.File:
					opener = new ThreadedOpenHelper(NWN2ToolsetMainForm.App,path,location);
					break;
					
				default:
					throw new NotSupportedException("Opening " + location + " modules is not supported.");
			}
			
			opener.Go();
									
			NWN2ToolsetMainForm.App.SetupHandlersForGameResourceContainer(GetCurrentModule());
		}
				
		
		/// <summary>
		/// Saves a Neverwinter Nights 2 game module to its
		/// current location.
		/// </summary>
		/// <param name="module">The module to save.</param>.
		public void SaveModule(NWN2GameModule module)
		{
			SaveModule(module,GetModulePath(module));
		}
		
		
		/// <summary>
		/// Saves a Neverwinter Nights 2 game module to a given path.
		/// </summary>
		/// <param name="module">The module to save.</param>
		/// <param name="path">The path to save the module to.</param>
		public void SaveModule(NWN2GameModule module, string path)
		{	
			string extension = Path.GetExtension(path);
			string parent = Path.GetDirectoryName(path);
			
			switch (module.LocationType) {
					
				case ModuleLocationType.Directory:
					if (extension != String.Empty) {
						throw new ArgumentException("path","Path must be a folder, not a file.");
					}
					if (parent != NWN2ToolsetMainForm.ModulesDirectory) {
						throw new ArgumentException("path","Path must be a folder located within the " +
						                            "modules directory specified at NWN2Toolset." + 
						                            "NWN2ToolsetMainForm.ModulesDirectory.");
					}
					module.OEISerialize(Path.GetFileName(path));
					break;
					
				case ModuleLocationType.File:
					if (extension.ToLower() != ".mod") {
						throw new ArgumentException("path","Path must be a .mod file.");
					}
					module.OEISerialize(path);
					break;
					
				default:
					throw new ArgumentException("location","Saving " + module.LocationType + 
					                            " modules is not supported.");
			}
		}
		
		
		/// <summary>
		/// Closes the current module.
		/// </summary>
		public void CloseModule()
		{
			int count = -1;
			string path;
						
			string message = String.Empty;
			
			while (Directory.Exists(path = Path.Combine(NWN2ToolsetMainForm.ModulesDirectory,"temp"+ ++count))) { message += path + Environment.NewLine;}
			
			string name = Path.GetFileName(path);
			
			System.Diagnostics.Debug.WriteLine(name);
			
			NWN2GameModule module = new NWN2GameModule();
			module.Name = name;
			module.LocationType = ModuleLocationType.Temporary;
			module.ModuleInfo.Tag = name;
			module.ModuleInfo.Description = new OEIShared.Utils.OEIExoLocString();
			
			OpenModule(path,ModuleLocationType.Temporary);
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
		/// Gets the absolute path of the module that is currently open in the toolset.
		/// </summary>
		/// <returns>The absolute path of the current module, or null if no module is open.</returns>
		public string GetCurrentModulePath()
		{
			return GetModulePath(GetCurrentModule());
		}
		
		
		/// <summary>
		/// Gets the absolute path of a given module.
		/// </summary>
		/// <param name="module">The module to return the path of.</param>
		/// <returns>The absolute path of the given module.</returns>
		public string GetModulePath(NWN2GameModule module)
		{
			switch (module.LocationType) {					
				case ModuleLocationType.File:
					return module.FileName;
					
				case ModuleLocationType.Directory:
				case ModuleLocationType.Temporary:
					return module.Repository.Name; // module.FileName only has the module name - no path!
					
				default:
					throw new NotSupportedException("Unknown ModuleLocationType: " + module.LocationType);
			}
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
