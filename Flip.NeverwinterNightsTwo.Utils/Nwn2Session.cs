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
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using NWN2Toolset;
using NWN2Toolset.NWN2.IO;
using NWN2Toolset.NWN2.Data;
using NWN2Toolset.NWN2.Data.Blueprints;
using NWN2Toolset.NWN2.Data.Instances;
using NWN2Toolset.NWN2.Data.Templates;
using NWN2Toolset.NWN2.Data.TypedCollections;
using OEIShared.IO;
using OEIShared.Utils;
using OEIShared.UI;

namespace Sussex.Flip.Games.NeverwinterNightsTwo.Utils
{
	/// <summary>
	/// A facade for a Neverwinter Nights 2 toolset session.
	/// </summary>
	/// <remarks>Any operation on a module must be followed
	/// by an explicit SaveModule() call if the changes are to persist.
	/// (The only exception is adding an area to a directory-based module -
	/// this is a quirk of the way Neverwinter Nights 2 is implemented.)</remarks>
	public class Nwn2Session : INwn2Session
	{
		#region Fields
		
		protected object padlock;
		
		protected static ushort nss;		
		public static ushort NSS {
			get { return nss; }
		}
		
		protected static ushort ncs;		
		public static ushort NCS {
			get { return ncs; }
		}
		
		#endregion
		
		#region Constructors
		
		/// <summary>
		/// Constructs a new <see cref="Nwn2Session"/> instance.
		/// </summary>
		public Nwn2Session()
		{
			padlock = new object();
			nss = OEIShared.Utils.BWResourceTypes.GetResourceType("NSS");	
			ncs = OEIShared.Utils.BWResourceTypes.GetResourceType("NCS");	
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
			if (location == ModuleLocationType.Temporary) {	
				throw new NotSupportedException("Creating temporary modules is not supported.");
			}			
			if (path == null) {
				throw new ArgumentNullException("path");
			}			
			if (path == String.Empty) {
				throw new ArgumentException("Path cannot be an empty string.","path");
			}			
			if (location == ModuleLocationType.Directory && Directory.Exists(path) ||
			    location == ModuleLocationType.File && File.Exists(path)) {
				throw new IOException("The path provided was already occupied (" + path + ").");
			}
						
			string name = Path.GetFileNameWithoutExtension(path);
			
			NWN2GameModule module;
			
			lock (padlock) {							
				NWN2ToolsetMainForm.App.DoNewModule(true);
				module = GetModule();
				
				module.Name = name;
				module.LocationType = location;
				module.ModuleInfo.Tag = name;
				module.ModuleInfo.Description = new OEIExoLocString();
			}
				
			SaveModule(module,path);
			
			CloseModule();
		}
				
		
		/// <summary>
		/// Opens a Neverwinter Nights 2 game module.
		/// </summary>
		/// <param name="name">The path of the module, including file extension
		/// if appropriate.</param>
		/// <param name="location">The serialisation form of the module.</param>
		/// <remarks>Also calls Demand() on all areas, simulating the effect
		/// of having each area open in the toolset. (The same is NOT true for
		/// any other type of resource, e.g. scripts, conversations.)</remarks>
		public void OpenModule(string path, ModuleLocationType location)
		{
			if (path == null) throw new ArgumentNullException("path");
			if (path == String.Empty) throw new ArgumentException("Path cannot be an empty string.","path");
			
			if (location == ModuleLocationType.Directory && !(Directory.Exists(path))) {
				throw new IOException("Directory at " + path + " does not exist.");
			}
			else if (location == ModuleLocationType.File && !(File.Exists(path))) {
				throw new IOException("File at " + path + " does not exist.");
			}
			
			ThreadedOpenHelper opener;						
			
			NWN2ToolsetMainForm.App.AutosaveTemporarilyDisabled = false;
					
			lock (padlock) {
				try {				
					if (NWN2ToolsetMainForm.App.CloseModule(true)) {					
						string parameter;
						if (location == ModuleLocationType.Directory) {
							parameter = Path.GetFileName(path);
						}
						else {
							parameter = path;
						}
						
				        opener = new ThreadedOpenHelper(NWN2ToolsetMainForm.App,parameter,location);
				        
				        ThreadedProgressDialog progress = new ThreadedProgressDialog();
				        progress.Text = "Opening " + location + " module";	
				        progress.Message = "Opening module '" + Path.GetFileName(path) + "'...";
				        progress.WorkerThread = new ThreadedProgressDialog.WorkerThreadDelegate(opener.Go);
				        progress.ShowDialog();
				        
				        NWN2ToolsetMainForm.App.SetupHandlersForGameResourceContainer(GetModule());
			        }
				}
			    catch (Exception) {
			        NWN2ToolsetMainForm.App.DoNewModule(false);
			    }
			    finally {
			    	NWN2ToolsetMainForm.App.AutosaveTemporarilyDisabled = false;
			    }
			}
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
						throw new ArgumentException("Path must be a folder, not a file.","path");
					}
					if (parent != NWN2ToolsetMainForm.ModulesDirectory) {
						throw new ArgumentException("Path must be a folder located within the " +
						                            "modules directory specified at NWN2Toolset." + 
						                            "NWN2ToolsetMainForm.ModulesDirectory.","path");
					}
					
					lock (padlock) {
						foreach (NWN2GameArea area in module.Areas.Values) {
							area.OEISerialize();
						}					
						foreach (NWN2GameScript script in module.Scripts.Values) {
							script.OEISerialize();
						}			
						string name = Path.GetFileName(path);			
						module.OEISerialize(name);
					}
					
					break;
					
				case ModuleLocationType.File:
					if (extension.ToLower() != ".mod") {
						throw new ArgumentException("Path must be a .mod file.","path");
					}
					
					lock (padlock) {
						foreach (NWN2GameArea area in module.Areas.Values) {
							area.OEISerialize();
						}									
						foreach (NWN2GameScript script in module.Scripts.Values) {
							script.OEISerialize();
						}							
						module.OEISerialize(path);
					}
					
					break;
					
				default:
					throw new ArgumentException("Saving " + module.LocationType + 
					                            " modules is not supported.","location");
			}
		}
		
		
		/// <summary>
		/// Closes the current module.
		/// </summary>
		public void CloseModule()
		{
			lock (padlock) {
				NWN2ToolsetMainForm.App.DoNewModule(false);
			}
		}
		
		
		/// <summary>
		/// Gets the module that is currently open in the toolset.
		/// </summary>
		/// <returns>The current module, or null if no module is open.</returns>
		public NWN2Toolset.NWN2.Data.NWN2GameModule GetModule()
		{
			return NWN2ToolsetMainForm.App.Module;
		}	
		
		
		/// <summary>
		/// Gets the absolute path of the module that is currently open in the toolset.
		/// </summary>
		/// <returns>The absolute path of the current module, or null if no module is open.</returns>
		public string GetModulePath()
		{
			NWN2GameModule module = GetModule();
			if (module == null) 
				return null;
			else {
				return GetModulePath(module);
			}
		}
		
		
		/// <summary>
		/// Gets the path to the working ('temp') copy of the module that is currently open in the toolset.
		/// </summary>
		/// <returns>The temp path of the current module, or null if no module is open.</returns>
		public string GetModuleTempPath()
		{
			NWN2GameModule module = GetModule();
			if (module == null) {
				return null;
			}
			else if (module.Repository == null) {
				throw new ApplicationException("Repository of current module was unexpectedly null.");
			}
			else {
				return module.Repository.DirectoryName;
			}
		}
		
		
		/// <summary>
		/// Gets the absolute path of a given module.
		/// </summary>
		/// <param name="module">The module to return the path of.</param>
		/// <returns>The absolute path of the given module.</returns>
		public string GetModulePath(NWN2GameModule module)
		{
			if (module == null) throw new ArgumentNullException("module");
			
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
		/// <remarks>As is the case when working directly with the toolset,
		/// file modules must be saved after adding an area or the area will
		/// not persist - directory modules do not have this requirement.</remarks>
		public AreaBase AddArea(string name, bool exterior, Size size)
		{					    
			NWN2GameModule module = GetModule();
				
			if (module == null) {
				throw new InvalidOperationException("No module is currently open.");
			}			
			if (module.Areas.ContainsCaseInsensitive(name)) {
				throw new IOException("An area with the given name ('" + name + "') already exists.");
			}
				
			NWN2GameArea oObject = new NWN2GameArea();
			
			oObject.Size = size;			    
			oObject.HasTerrain = exterior;			    
			oObject.Interior = !exterior;			    
			oObject.Name = name;			    
			oObject.Tag = name;			    
			oObject.DisplayName[BWLanguages.CurrentLanguage] = name;
				
			lock (padlock) {
				NWN2GameArea oResource = module.AddResource(oObject) as NWN2GameArea;
				if (oObject == oResource) {
					oObject.InitializeArea(name, module.TempDirectory, module.Repository);
				}
				oObject.SetAreaTypePropertiesToDefault();
				if (!exterior) {
				    foreach (OEIShared.NetDisplay.DayNightStage stage in oObject.DayNightStages) {
					    stage.SetToDefaultInteriorValues();
				    }
				}
				oObject.OEISerialize();
			}
			
			/*
			 * Adding an area in the toolset ALWAYS opens it, so the issue that occurs where you
			 * 'blank' a newly created area because you Demand() it from a non-existent serialised
			 * version never occurs - the area is always Demand()ed and Loaded immediately. We can
			 * therefore safely replicate this by automatically opening the area in an area viewer.
			 */ 
			
			NWN2Toolset.NWN2ToolsetMainForm.App.ShowResource(oObject);
				
			return CreateAreaBase(oObject);
		}
		
		
		/// <summary>
		/// Constructs a new <see cref="AreaBase"/> instance.
		/// </summary>
		/// <param name="nwn2area">The Neverwinter Nights 2 area
		/// the <see cref="AreaBase"/> facade will wrap.</param>
		/// <returns>A new <see cref="AreaBase"/> instance.</returns>
		/// <remarks>This is a Factory Method.</remarks>
		public AreaBase CreateAreaBase(NWN2GameArea nwn2area)
		{
			return new Area(nwn2area);
		}
		
		
		/// <summary>
		/// Checks whether the current module has a resource
		/// of the given name and resource type.
		/// </summary>
		/// <param name="name">The name of the resource.</param>
		/// <param name="resourceType">The number indicating the type 
		/// of the resource.</param>
		/// <returns>True if the current module has a resource
		/// of the given name and type, and false otherwise.</returns>
		public bool HasResource(string name, ushort resourceType)
		{
			if (name == null) {
				throw new ArgumentNullException("name","No name was provided (was null).");
			}	
			if (name == String.Empty) {
				throw new ArgumentException("name","No name was provided (was empty).");
			}		
			
			NWN2GameModule module = GetModule();
			
			if (module == null) {
				throw new InvalidOperationException("No module is currently open.");
			}					
			if (module.Repository == null) {
				throw new ApplicationException("The module's repository was missing.");
			}
			
			OEIResRef cResRef = new OEIResRef(name);	
			
			IResourceEntry entry = module.Repository.FindResource(cResRef,resourceType);
			
			return entry != null;
		}
		
		
		/// <summary>
		/// Checks whether the current module has a compiled script
		/// of the given name.
		/// </summary>
		/// <param name="name">The name of the script.</param>
		/// <returns>True if the current module has a .NCS compiled
		/// script file of the given name, and false otherwise.</returns>
		public bool HasCompiled(string name)
		{
			return HasResource(name,NCS);
		}
		

		/// <summary>
		/// Checks whether the current module has an uncompiled script
		/// of the given name.
		/// </summary>
		/// <param name="name">The name of the script.</param>
		/// <returns>True if the current module has a .NSS uncompiled
		/// script file of the given name, and false otherwise.</returns>
		public bool HasUncompiled(string name)
		{
			return HasResource(name,NSS);
		}
		
		#endregion
	}
}
