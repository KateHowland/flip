/*
 * Flip - a visual programming language for scripting video games
 * Copyright (C) 2009, 2010 University of Sussex
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
using System.Reflection;
using System.Threading;
using NWN2Toolset;
using NWN2Toolset.NWN2.IO;
using NWN2Toolset.NWN2.Data;
using NWN2Toolset.NWN2.Data.Blueprints;
using NWN2Toolset.NWN2.Data.Campaign;
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
		
		protected static ushort nss = OEIShared.Utils.BWResourceTypes.GetResourceType("NSS");
		public static ushort NSS {
			get { return nss; }
		}
		
		protected static ushort ncs = OEIShared.Utils.BWResourceTypes.GetResourceType("NCS");	
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
				throw new NotSupportedException("Creating temporary modules is not supported - use " +
				                                "CreateAndOpenTemporaryModule instead.");
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
		/// Creates and opens a Neverwinter Nights 2 game module of location type Temporary.
		/// </summary>
		/// <returns>The path the module was created at.</returns>
		public string CreateAndOpenTemporaryModule()
		{
			lock (padlock) {				
				// This is all the toolset does when the user creates a new module:	
				NWN2ToolsetMainForm.App.DoNewModule(true);
				return GetModulePath();
			}
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
					break;
					
				case ModuleLocationType.File:
					if (extension.ToLower() != ".mod") {
						throw new ArgumentException("Path must be a .mod file.","path");
					}					
					break;
					
				default:
					throw new ArgumentException("Saving " + module.LocationType + 
					                            " modules is not supported.","location");
			}
			
			foreach (NWN2GameArea area in module.Areas.Values) {
				if (area.Loaded) area.OEISerialize();
			}					
			foreach (NWN2GameScript script in module.Scripts.Values) {
				if (script.Loaded) script.OEISerialize();
			}			
			
			if (NWN2CampaignManager.Instance.ActiveCampaign != null) {
				NWN2CampaignManager.Instance.ActiveCampaign.SaveBlueprints();
				NWN2ToolsetMainForm.App.ContentManager.SaveCampaignList(NWN2CampaignManager.Instance.ActiveCampaign.Repository,true);
			}
			NWN2ToolsetMainForm.App.RunVerification(NWN2Toolset.Plugins.NWN2ModuleVerificationType.Fast);
			
			module.OEISerialize(path);
			module.Name = Path.GetFileNameWithoutExtension(path);
			NWN2ToolsetMainForm.App.ContentManager.InitializeModule(module.Repository);
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
		
		
		/// <summary>
		/// Adds an object to the given area.
		/// </summary>
		/// <param name="area">The area in the current module to add the object to.</param>
		/// <param name="type">The type of object to add.</param>
		/// <param name="resref">The resref of the blueprint to create the object from.</param>
		/// <param name="tag">The tag of the object.</param>
		/// <returns>Returns the newly created object.</returns>
		public INWN2Instance AddObject(NWN2GameArea area, NWN2ObjectType type, string resref, string tag)
		{	
			if (area == null) {
				throw new ArgumentNullException("area","No area was provided (was null).");
			}			
				
			NWN2GameModule module = GetModule();
				
			if (module == null) {
				throw new InvalidOperationException("No module is currently open.");
			}						
			if (!module.Areas.Contains(area)) {
				throw new ArgumentException("The current module does not contain area '" + area.Name + "'.","area");
			}
				
			Area a = new Area(area);
				
			Microsoft.DirectX.Vector3 position = a.GetRandomPosition(true);
				
			INWN2Instance instance = a.AddGameObject(type,resref,tag,position);
			return instance;
		}
		
		
		/// <summary>
		/// Gets a blueprint with the given resref and type,
		/// if one exists.
		/// </summary>
		/// <param name="resRef">The resref value of the blueprint.</param>
		/// <param name="type">The object type of the blueprint.</param>
		/// <returns>A blueprint, or null if no blueprint
		/// matching the given criteria was found.</returns>
		public INWN2Blueprint GetBlueprint(string resRef, NWN2ObjectType type)
		{	
			if (resRef == null) {
				throw new ArgumentNullException("resRef","No resref was provided (was null).");
			}			
			if (resRef == String.Empty) {
				throw new ArgumentException("No resref was provided (was empty).","resRef");
			}
				
			NWN2GameModule module = GetModule();				
			if (module == null) {
				throw new InvalidOperationException("No module is currently open.");
			}	
				
			INWN2Blueprint blueprint = NWN2GlobalBlueprintManager.FindBlueprint(type,new OEIResRef(resRef),true,true,true);
			return blueprint;
		}
		
		
		/// <summary>
		/// Gets all blueprints of a given type.
		/// </summary>
		/// <param name="type">The object type of the
		/// blueprints to return.</param>
		/// <returns>A collection of blueprints.</returns>
		public NWN2BlueprintCollection GetBlueprints(NWN2ObjectType type)
		{
			return NWN2GlobalBlueprintManager.GetBlueprintsOfType(type,true,true,true);
		}		
		
		
		/// <summary>
		/// Gets a unique object in an area from its properties.
		/// </summary>
		/// <param name="area">The area which has the object.</param>
		/// <param name="type">The type of the object.</param>
		/// <param name="id">The unique ID of the object.</param>
		/// <returns>The object within this area with the given properties,
		/// or null if one could not be found.</returns>
		/// <remarks>This method will throw an InvalidOperationException if
		/// the area is not open.</remarks>
		public INWN2Instance GetObject(NWN2GameArea area, NWN2ObjectType type, Guid id)
		{
			if (id == null) {
				throw new ArgumentNullException("guid","No guid was provided (was null).");
			}	
			if (area == null) {
				throw new ArgumentNullException("area","No area was provided (was null).");
			}			
				
			NWN2GameModule module = GetModule();
				
			if (module == null) {
				throw new InvalidOperationException("No module is currently open.");
			}					
			if (!module.Areas.Contains(area)) {
				throw new ArgumentException("The current module does not contain area '" + area.Name + "'.","area");
			}
				
			AreaBase a = CreateAreaBase(area);
				
			return a.GetObject(type,id);
		}		
		
		
		/// <summary>
		/// Gets all objects of a given type in a given area.
		/// </summary>
		/// <param name="area">The area which has the objects.</param>
		/// <param name="type">The type of objects to collect.</param>
		/// <returns>A collection of objects.</returns>
		/// <remarks>This method will throw an InvalidOperationException if
		/// the area is not open.</remarks>
		public NWN2InstanceCollection GetObjects(NWN2GameArea area, NWN2ObjectType type)
		{
			if (area == null) {
				throw new ArgumentNullException("area","No area was provided (was null).");
			}			
				
			NWN2GameModule module = GetModule();
				
			if (module == null) {
				throw new InvalidOperationException("No module is currently open.");
			}					
			if (!module.Areas.Contains(area)) {
				throw new ArgumentException("The current module does not contain area '" + area.Name + "'.","area");
			}
			
			AreaBase a = CreateAreaBase(area);				
			return a.GetObjects(type);
		}		
		
		
		/// <summary>
		/// Gets all objects of a given type with a given tag in a given area.
		/// </summary>
		/// <param name="area">The area which has the objects.</param>
		/// <param name="type">The type of objects to collect.</param>
		/// <param name="tag">The tag that objects must have to be collected.
		/// Pass null to ignore this requirement.</param>
		/// <returns>A collection of objects.</returns>
		/// <remarks>This method will throw an InvalidOperationException if
		/// the area is not open.</remarks>
		public NWN2InstanceCollection GetObjectsByTag(NWN2GameArea area, NWN2ObjectType type, string tag)
		{
			if (area == null) {
				throw new ArgumentNullException("area","No area was provided (was null).");
			}			
				
			NWN2GameModule module = GetModule();
				
			if (module == null) {
				throw new InvalidOperationException("No module is currently open.");
			}					
			if (!module.Areas.Contains(area)) {
				throw new ArgumentException("The current module does not contain area '" + area.Name + "'.","area");
			}
				
			AreaBase a = CreateAreaBase(area);
							
			return a.GetObjects(type,tag);	
		}		
		
		
		/// <summary>
		/// Gets an area in the current module.
		/// </summary>
		/// <param name="name">The name of the area.</param>
		/// <returns>The named area, or null 
		/// if the area could not be found.</returns>
		public NWN2GameArea GetArea(string name)
		{
			if (name == null) {
				throw new ArgumentNullException("name","No area name was provided (was null).");
			}			
			if (name == String.Empty) {
				throw new ArgumentException("No area name was provided (was empty).","name");
			}
				
			NWN2GameModule module = GetModule();
				
			if (module == null) {
				throw new InvalidOperationException("No module is currently open.");
			}						
			if (!module.Areas.ContainsCaseInsensitive(name)) {
				return null;
			}
				
			return module.Areas[name];
		}		
		
		
		/// <summary>
		/// Gets the scripts in the current module.
		/// </summary>
		/// <returns>A list of scripts.</returns>
		public NWN2GameScriptDictionary GetScripts()
		{
			NWN2GameModule module = GetModule();
			if (module == null) {
				throw new InvalidOperationException("No module is currently open.");
			}		
			return module.Scripts;
		}		
		
		
		/// <summary>
		/// Gets the script with a given name in the current module.
		/// </summary>
		/// <returns>The script with the given name,
		/// or null if no such script exists.</returns>
		public NWN2GameScript GetScript(string name)
		{
			if (name == null) {
				throw new ArgumentNullException("name","No script name was provided (was null).");
			}	
			if (name == String.Empty) {
				throw new ArgumentException("name","No script name was provided (was empty).");
			}		
				
			NWN2GameModule module = GetModule();
				
			if (module == null) {
				throw new InvalidOperationException("No module is currently open.");
			}				
			if (!module.Scripts.ContainsCaseInsensitive(name)) {
				return null;
			}
			else {
				return module.Scripts[name];
			}
		}			
		
		
		/// <summary>
		/// Gets the compiled scripts in the current module.
		/// </summary>
		/// <returns>A list of compiled scripts.</returns>
		public OEIGenericCollectionWithEvents<IResourceEntry> GetCompiledScripts()
		{
			NWN2GameModule module = GetModule();
			if (module == null) {
				throw new InvalidOperationException("No module is currently open.");
			}				
			if (module.Repository == null) {
				throw new ApplicationException("The module's repository was missing.");
			}	
			
			ushort NCS = OEIShared.Utils.BWResourceTypes.GetResourceType("NCS");					
			return module.Repository.FindResourcesByType(NCS);
		}		
		
		
		/// <summary>
		/// Gets the compiled script with a given name in the current module.
		/// </summary>
		/// <returns>The compiled script with the given name,
		/// or null if no such script exists.</returns>
		public IResourceEntry GetCompiledScript(string name)
		{
			if (name == null) {
				throw new ArgumentNullException("name","No script name was provided (was null).");
			}	
			if (name == String.Empty) {
				throw new ArgumentException("name","No script name was provided (was empty).");
			}		
			
			NWN2GameModule module = GetModule();
			if (module == null) {
				throw new InvalidOperationException("No module is currently open.");
			}				
			if (module.Repository == null) {
				throw new ApplicationException("The module's repository was missing.");
			}
			
			ushort resourceType = OEIShared.Utils.BWResourceTypes.GetResourceType("NCS");	
			OEIResRef cResRef = new OEIResRef(name);
			return module.Repository.FindResource(cResRef,resourceType);
		}			
				

		/// <summary>
		/// Adds an uncompiled script to the current module.
		/// </summary>
		/// <param name="name">The name to save the script under.</param>
		/// <param name="code">The contents of the script.</param>
		/// <returns>The newly created script.</returns>
		public NWN2GameScript AddScript(string name, string code)
		{
			if (name == null) {
				throw new ArgumentNullException("name");
			}
			if (code == null) {
				throw new ArgumentNullException("code");
			}
			
			NWN2GameModule module = GetModule();
			if (module == null) {
				throw new InvalidOperationException("No module is currently open.");
			}
							
			NWN2GameScript script = new NWN2GameScript(name,
			                                           module.Repository.DirectoryName,
			                                           module.Repository);				
			script.Module = module;		
			module.Scripts.Add(script);		
			
			/*
			 * Adding a script in the toolset ALWAYS opens it, so the issue that occurs where you
			 * 'blank' a newly created script because you Demand() it from a non-existent serialised
			 * version never occurs - the script is always Demand()ed and Loaded immediately. However,
			 * our AddScript() CANNOT just start opening script viewers all over the place - we want
			 * to keep these hidden! Instead, we'll just serialise the script when we create it, and
			 * subsequent methods can Demand() it.
			 */ 
			
			script.Demand();
			script.Data = code;
			script.OEISerialize();
			script.Release();
			
			return script;
		}		
		
		
		/// <summary>
		/// Deletes a script from the current module.
		/// </summary>
		/// <param name="name">The name of the script.</param>
		/// <remarks>Both compiled and uncompiled copies
		/// of the script are deleted.</remarks>
		public void DeleteScript(string name)
		{
			if (name == null) {
				throw new ArgumentNullException("name");
			}
			if (name == String.Empty) {
				throw new ArgumentException("name");
			}
							
			NWN2GameModule module = GetModule();
			if (module == null) {
				throw new InvalidOperationException("No module is currently open.");
			}
			if (!HasUncompiled(name) && !HasCompiled(name)) {
				throw new ArgumentException("Module '" + module.Name + "' has no script named '" + name + "'.","name");
			}
													
			try {					
				// Deleting an uncompiled script also deletes the compiled version, but deleting a compiled
				// script DOESN'T also delete the uncompiled version, and a subsequent attempt to delete
				// the uncompiled version explicitly will fail. Always delete uncompiled, then compiled.
				
				OEIResRef resRef = new OEIResRef(name);
									
				IResourceEntry uncompiled = module.Repository.FindResource(resRef,BWResourceTypes.GetResourceType("NSS"));
				if (uncompiled != null) {
					NWN2GameScript u = new NWN2GameScript(uncompiled);
					if (u != null) {
						module.RemoveResource(u);
					}
				}
				
				IResourceEntry compiled = module.Repository.FindResource(resRef,BWResourceTypes.GetResourceType("NCS"));
				if (compiled != null) {
					module.Repository.Resources.Remove(compiled);
				}
			}
			catch (ArgumentOutOfRangeException) {
				throw new ArgumentException("Scripts collection for this module did not feature a script named '" + name + "'.");
			}
		}		
				

		/// <summary>
		/// Compiles a script in the current module.
		/// </summary>
		/// <param name="script">The script to compile.</param>
		public void CompileScript(NWN2GameScript script)
		{
			if (script == null) {
				throw new ArgumentNullException("script");
			}
								
			NWN2GameModule module = GetModule();
			if (module == null) {
				throw new InvalidOperationException("No module is currently open.");
			}
			if (!HasUncompiled(script.Name)) {
				throw new ArgumentException("Module '" + module.Name + "' has no uncompiled script named '" + script.Name + "'.","name");
			}
				
			NWN2Toolset.NWN2ToolsetMainForm.Compiler.GenerateDebugInfo = true;
					
			string resourcePath = Path.Combine(GetModuleTempPath(),script.VersionControlName);
			bool scriptHasNeverBeenSerialised = !File.Exists(resourcePath) || File.ReadAllText(resourcePath).Length == 0;
									
			// Don't serialise if the script was never Demand()ed, as this will
			// overwrite the 'real' script with the blank copy in memory...
			// but otherwise, you need to serialise as the copy on disk is the
			// one which will actually be compiled:
			if (script.Loaded || scriptHasNeverBeenSerialised) {			
				script.OEISerialize();
			}
				
			string compiled = Path.Combine(GetModuleTempPath(),Path.GetFileNameWithoutExtension(script.VersionControlName) + ".ncs");
					
			/* The compiler will automatically overwrite the compiled script file,
			 * but by deleting it beforehand we can wait for the new version to appear
			 * before returning, so we know the compile operation has finished: */															
			if (File.Exists(compiled)) {
				File.Delete(compiled);
			}
											
			string debug = NWN2Toolset.NWN2ToolsetMainForm.Compiler.CompileFile(script.Name,GetModuleTempPath());	
				
			if (debug.Length > 0) {
				throw new InvalidDataException("'" + script.Name + "' could not be compiled: " + debug);
			}
					
			/*
			 * CompileScript() should wait until the output file appears on the filesystem before returning
			 * (otherwise there are intermittent bugs where other methods don't find the file they expect)
			 * but neither of the commented-out methods that follow work when called from within this service.
			 * That is, the file is NEVER found from here, even though it certainly exists, and even though
			 * it is immediately found when these methods are called from outside this service method (e.g.
			 * when called from the test suite).
			 * 
			 * This problem is 'fixed' if you call MessageBox.Show() before checking for the file (or was 
			 * it before calling CompileFile? I forget) - the file is then found as normal. From this I'm
			 * assuming it's some kind of a threading issue to do with MessageBox.Show() blocking the UI 
			 * thread...? But I don't really know, and I've spent too much time trying to fix it, so I'm
			 * just going to put the responsibility onto the clients to wait until a script has been compiled
			 * before attempting to use it.
			 */						
			
			//while (!session.HasCompiled(name));
			//while (!File.Exists(compiled) || new FileInfo(compiled).Length == 0);
		}					
		
		
		/// <summary>
		/// Attaches a script to a named script slot on a given instance.
		/// </summary>
		/// <param name="script">The script to attach.</param>
		/// <param name="instance">The instance to attach the script to.</param>
		/// <param name="slot">The script slot to attach the script to.</param>
		public void AttachScriptToObject(NWN2GameScript script, INWN2Instance instance, string slot)
		{
				if (script == null) {
					throw new ArgumentNullException("script");
				}
				if (slot == null) {
					throw new ArgumentNullException("slot");
				}
				if (slot == String.Empty) {
					throw new ArgumentException("slot");
				}
				if (!Nwn2ScriptSlot.GetScriptSlotNames(instance.ObjectType).Contains(slot)) {
					throw new ArgumentException("Objects of type " + instance.ObjectType + " do not have a script slot " +
					                            "named " + slot + " (call Sussex.Flip.Games.NeverwinterNightsTwo" +
					                            ".Utils.Nwn2ScriptSlot.GetScriptSlotNames() to find valid " +
					                            "script slot names.)","slot");
				}
				
				NWN2GameModule module = GetModule();
				if (module == null) {
					throw new InvalidOperationException("No module is currently open.");
				}
				if (!module.Scripts.Contains(script)) {
					throw new ArgumentException("Script does not belong to the current module.","script");
				}
							
				PropertyInfo pi = instance.GetType().GetProperty(slot);
				if (pi == null) {
					throw new ArgumentException(instance.ObjectType.ToString() + " objects do not " +
					                            "have a " + slot + " property.");
				}
				
				bool loaded = script.Loaded;
				if (!loaded) script.Demand();
				pi.SetValue(instance,script.Resource,null);
//				if (!loaded) script.Release();
		}	
				
		
		/// <summary>
		/// Attaches a script to a named script slot on a given area.
		/// </summary>
		/// <param name="script">The script to attach.</param>
		/// <param name="area">The area to attach the script to.</param>
		/// <param name="slot">The script slot to attach the script to.</param>
		public void AttachScriptToArea(NWN2GameScript script, NWN2GameArea area, string slot)
		{
			throw new NotImplementedException();
		}		
				
		
		/// <summary>
		/// Attaches a script to a named script slot on a given module.
		/// </summary>
		/// <param name="script">The script to attach.</param>
		/// <param name="module">The module to attach the script to.</param>
		/// <param name="slot">The script slot to attach the script to.</param>
		public void AttachScriptToModule(NWN2GameScript script, NWN2GameModule module, string slot)
		{
			throw new NotImplementedException();
		}		
		
		
		/// <summary>
		/// Clears the value of a named script slot on a given instance.
		/// </summary>
		/// <param name="instance">The object which owns the script slot to be cleared.</param>
		/// <param name="slot">The script slot to clear
		/// any scripts from.</param>
		public void ClearScriptSlotOnObject(INWN2Instance instance, string slot)
		{
			throw new NotImplementedException();
		}		
		
		
		/// <summary>
		/// Clears the value of a named script slot on a given area.
		/// </summary>
		/// <param name="area">The area which owns the script slot to be cleared.</param>
		/// <param name="slot">The script slot to clear
		/// any scripts from.</param>
		public void ClearScriptSlotOnArea(NWN2GameArea area, string slot)
		{
			throw new NotImplementedException();
		}		
		
		
		/// <summary>
		/// Clears the value of a named script slot on a given module.
		/// </summary>
		/// <param name="module">The module which owns the script slot to be cleared.</param>
		/// <param name="slot">The script slot to clear
		/// any scripts from.</param>
		public void ClearScriptSlotOnModule(NWN2GameModule module, string slot)
		{
			throw new NotImplementedException();
		}		
		
		
		/// <summary>
		/// Opens an area in an area viewer.
		/// </summary>
		/// <param name="area">The area to open.</param>
		public void OpenArea(NWN2GameArea area)
		{
			throw new NotImplementedException();
		}		
		
		
		/// <summary>
		/// Closes the area viewer for an area, if one is
		/// currently open.
		/// </summary>
		/// <param name="area">The area to close.</param>
		public void CloseArea(NWN2GameArea area)
		{
			throw new NotImplementedException();
		}		
		
		
		/// <summary>
		/// Checks whether an area viewer is currently open
		/// for a particular area.
		/// </summary>
		/// <param name="area">The area which may have a viewer open.</param>
		/// <returns>True if an area viewer for the given
		/// area is currently open in the toolset; false
		/// otherwise.</returns>
		public bool AreaIsOpen(NWN2GameArea area)
		{
			throw new NotImplementedException();
		}		
		
		
		/// <summary>
		/// Gets a list of scripts
		/// which are currently open in script viewers.
		/// </summary>
		/// <returns>A list of open scripts.</returns>
		public IList<NWN2GameScript> GetOpenScripts()
		{
			throw new NotImplementedException();
		}		
		
		
		/// <summary>
		/// Opens a script in a script viewer.
		/// </summary>
		/// <param name="script">The script to open.</param>
		public void OpenScript(NWN2GameScript script)
		{
			throw new NotImplementedException();
		}		
		
		
		/// <summary>
		/// Closes the script viewer for a script, if one is
		/// currently open.
		/// </summary>
		/// <param name="script">The script to close.</param>
		public void CloseScript(NWN2GameScript script)
		{
			throw new NotImplementedException();
		}		
		
		
		/// <summary>
		/// Checks whether a script viewer is currently open
		/// for a particular script.
		/// </summary>
		/// <param name="script">The script which may have a viewer open.</param>
		/// <returns>True if a script viewer for the given
		/// script is currently open in the toolset; false
		/// otherwise.</returns>
		public bool ScriptIsOpen(NWN2GameScript script)
		{
			throw new NotImplementedException();
		}		
		
		
		/// <summary>
		/// Loads a script resource from disk, ensuring that
		/// the script object is fully loaded (but overwriting
		/// any unsaved changes that were made).
		/// </summary>
		/// <param name="script">The script to demand.</param>
		public void DemandScript(NWN2GameScript script)
		{
			throw new NotImplementedException();
		}		
		
		
		/// <summary>
		/// Releases a script resource.
		/// </summary>
		/// <param name="script">The script to release.</param>
		public void ReleaseScript(NWN2GameScript script)
		{
			throw new NotImplementedException();
		}		
		
		
		/// <summary>
		/// Loads an area from disk, ensuring that
		/// the area object is fully loaded (but overwriting
		/// any unsaved changes that were made).
		/// </summary>
		/// <param name="area">The area to demand.</param>
		public void DemandArea(NWN2GameArea area)
		{
			throw new NotImplementedException();
		}		
		
		
		/// <summary>
		/// Releases an area resource.
		/// </summary>
		/// <param name="area">The area to release.</param>
		public void ReleaseArea(NWN2GameArea area)
		{
			throw new NotImplementedException();
		}		
		
		#endregion
	}
}
