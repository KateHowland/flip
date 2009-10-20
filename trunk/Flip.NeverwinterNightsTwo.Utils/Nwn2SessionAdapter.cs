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
 * This file added by Keiron Nicholson on 25/09/2009 at 15:36.
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.ServiceModel;
using NWN2Toolset.NWN2.Data;
using NWN2Toolset.NWN2.Data.Instances;
using NWN2Toolset.NWN2.Data.Templates;
using NWN2Toolset.NWN2.Data.TypedCollections;
using NWN2Toolset.NWN2.IO;
using OEIShared.IO;
using OEIShared.IO.GFF;
using OEIShared.Utils;

namespace Sussex.Flip.Games.NeverwinterNightsTwo.Utils
{
	/// <summary>
	/// A service providing access to a Neverwinter Nights 2 toolset session.
	/// </summary>
	[ServiceBehavior(IncludeExceptionDetailInFaults=true)]
	public class Nwn2SessionAdapter : INwn2Service
	{
		#region Fields
		
		/// <summary>
		/// The session this adapter wraps.
		/// </summary>
		protected INwn2Session session;
		
		#endregion
		
		#region Constructors
				
		/// <summary>
		/// Constructs a new <see cref="Nwn2SessionAdapter"/> instance.
		/// </summary>
		/// <remarks>This default constructor wraps a Nwn2Session object.</remarks>
		public Nwn2SessionAdapter()
		{
			session = new Nwn2Session();
		}
		
		
		/// <summary>
		/// Constructs a new <see cref="Nwn2SessionAdapter"/> instance.
		/// </summary>
		/// <param name="session">The session this adapter wraps.</param>
		public Nwn2SessionAdapter(INwn2Session session)
		{
			if (session == null) throw new ArgumentNullException("session");			
			this.session = session;
		}
		
		#endregion
		
		#region INwn2Service methods
		
		/// <summary>
		/// Creates a Neverwinter Nights 2 game module.
		/// </summary>
		/// <param name="path">The path to create the module at. If 'location'
		/// is set to ModuleLocationType.Directory, this must be the path for
		/// a folder to be created within NWN2Toolset.NWN2ToolsetMainForm.ModulesDirectory.</param>
		/// <param name="location">The serialisation form of the module.</param>
		public void CreateModule(string path, NWN2Toolset.NWN2.IO.ModuleLocationType location)
		{			
			try {
				session.CreateModule(path,location);
			}
			catch (ArgumentNullException e) {
				throw new FaultException<ArgumentNullException>(e,e.Message);
			}
			catch (ArgumentException e) {
				throw new FaultException<ArgumentException>(e,e.Message);
			}
			catch (IOException e) {
				throw new FaultException<IOException>(e,e.Message);
			}
			catch (NotSupportedException e) {
				throw new FaultException<NotSupportedException>(e,e.Message);
			}
			catch (Exception e) {
				throw new FaultException("(" + e.GetType() + ") " + e.Message);
			}
		}
		
		
		/// <summary>
		/// Opens a Neverwinter Nights 2 game module.
		/// </summary>
		/// <param name="name">The path of the module to open.</param>
		/// <param name="location">The serialisation form of the module.</param>
		public void OpenModule(string path, ModuleLocationType location)
		{
			try {
				session.OpenModule(path,location);
			}
			catch (ArgumentNullException e) {
				throw new FaultException<ArgumentNullException>(e,e.Message);
			}
			catch (ArgumentException e) {
				throw new FaultException<ArgumentException>(e,e.Message);
			}
			catch (IOException e) {
				throw new FaultException<IOException>(e,e.Message);
			}
			catch (Exception e) {
				throw new FaultException("(" + e.GetType() + ") " + e.Message);
			}
		}
		
		
		/// <summary>
		/// Saves changes to the current game module.
		/// </summary>
		/// <remarks>Saves to the default modules directory.</remarks>
		public void SaveModule()
		{
			try {
				session.SaveModule(session.GetCurrentModule());
			}
			catch (ArgumentException e) {
				throw new FaultException<ArgumentException>(e,e.Message);
			}
			catch (Exception e) {
				throw new FaultException("(" + e.GetType() + ") " + e.Message);
			}
		}
		
		
		/// <summary>
		/// Closes the current module.
		/// </summary>
		public void CloseModule()
		{
			try {
				session.CloseModule();
			}
			catch (Exception e) {
				throw new FaultException("(" + e.GetType() + ") " + e.Message);
			}
		}
		
		
		/// <summary>
		/// Gets the name of the module that is currently open in the toolset.
		/// </summary>
		/// <returns>The name of the current module, or null if no module is open.</returns>
		public string GetCurrentModuleName()
		{
			try {
				NWN2GameModule module = session.GetCurrentModule();
				if (module == null) {
					return null;
				}
				else {
					return module.Name;
				}
			}
			catch (Exception e) {
				throw new FaultException("(" + e.GetType() + ") " + e.Message);
			}
		}		
		
		
		/// <summary>
		/// Gets the absolute path of the module that is currently open in the toolset.
		/// </summary>
		/// <returns>The absolute path of the current module, or null if no module is currently open.</returns>
		public string GetCurrentModulePath()
		{
			try {
				return session.GetCurrentModulePath();
			}
			catch (Exception e) {
				throw new FaultException("(" + e.GetType() + ") " + e.Message);
			}
		}
		
		
		/// <summary>
		/// Gets the location type of the module that is currently open in the toolset.
		/// </summary>
		/// <returns>The location type of the current module.</returns>
		public ModuleLocationType? GetCurrentModuleLocation()
		{
			try {
				NWN2GameModule module = session.GetCurrentModule();
				if (module == null) {
					return null;
				}
				else {
					return (ModuleLocationType?)module.LocationType;
				}
			}
			catch (Exception e) {
				throw new FaultException("(" + e.GetType() + ") " + e.Message);
			}
		}
		
		
		/// <summary>
		/// Creates a Neverwinter Nights 2 area
		/// in the current module.
		/// <param name="name">The name to give the area.</param>
		/// <param name="exterior">True to create an exterior area
		/// with terrain; false to create an interior area with tiles.</param>
		/// <param name="size">The size of area to create.</param>
		public void AddArea(string name, bool exterior, Size size)
		{
			try {
				session.AddArea(name,exterior,size);
			}
			catch (InvalidOperationException e) {
				throw new FaultException<InvalidOperationException>(e,e.Message);
			}
			catch (IOException e) {
				throw new FaultException<System.IO.IOException>(e,e.Message);
			}
			catch (Exception e) {
				throw new FaultException("(" + e.GetType() + ") " + e.Message);
			}
		}
		
		
		/// <summary>
		/// Gets a list of names of every area in the 
		/// current module.
		/// </summary>
		/// <returns>A list of area names, or null
		/// if there is no module open.</returns>
		public ICollection<string> GetAreas()
		{
			try {
				NWN2GameModule module = session.GetCurrentModule();
							
				if (module == null) {
					InvalidOperationException e = new InvalidOperationException("No module is currently open.");
					FaultReasonText text = new FaultReasonText(e.Message,CultureInfo.CurrentCulture);
					throw new FaultException<InvalidOperationException>(e,new FaultReason(text));
				}
				
				List<string> areas = new List<string>(module.Areas.Count);
				foreach (string key in module.Areas.Keys) {
					areas.Add(key);
				}
				return areas;
			}			
			catch (Exception e) {
				throw new FaultException("(" + e.GetType() + ") " + e.Message);
			}
		}
				
				
		/// <summary>
		/// Adds an object to the given area.
		/// </summary>
		/// <param name="areaName">The name of the area in the current module to add the object to.</param>
		/// <param name="type">The type of object to add.</param>
		/// <param name="resref">The resref of the blueprint to create the object from.</param>
		/// <param name="tag">The tag of the object.</param>
		public void AddObject(string areaName, NWN2ObjectType type, string resref, string tag)
		{			
			try {
				if (areaName == null) {
					throw new ArgumentNullException("areaName","No area name was provided (was null).");
				}			
				if (areaName == String.Empty) {
					throw new ArgumentException("No area name was provided (was empty).","areaName");
				}
				
				NWN2GameModule module = session.GetCurrentModule();
				
				if (module == null) {
					throw new InvalidOperationException("No module is currently open.");
				}						
				if (!module.Areas.ContainsCaseInsensitive(areaName)) {
					throw new ArgumentException("The current module does not contain an area named '" + areaName + "'.","areaName");
				}
				
				NWN2GameArea nwn2area = module.Areas[areaName];
				Area area = new Area(nwn2area);
				
				Microsoft.DirectX.Vector3 position = area.GetRandomPosition(true);
				
				area.AddGameObject(type,resref,tag,position);
			}
			catch (ArgumentNullException e) {
				throw new FaultException<ArgumentNullException>(e,e.Message);
			}
			catch (ArgumentException e) {
				throw new FaultException<ArgumentException>(e,e.Message);
			}
			catch (InvalidOperationException e) {
				throw new FaultException<InvalidOperationException>(e,e.Message);
			}
			catch (Exception e) {
				throw new FaultException("(" + e.GetType() + ") " + e.Message);
			}
		}
		
		
		/// <summary>
		/// Gets the number of objects matching a given description
		/// in a given area.
		/// </summary>
		/// <param name="areaName">The area which has the objects.</param>
		/// <param name="type">The type of objects to count.</param>
		/// <param name="tag">Only objects with this tag will be counted.
		/// Pass null to ignore this criterion.</param>
		/// <returns>The number of objects matching the given description
		/// in the given area.</returns>
		public int GetObjectCount(string areaName, NWN2ObjectType type, string tag)
		{	
			try {
				if (areaName == null) {
					throw new ArgumentNullException("areaName","No area name was provided (was null).");
				}			
				if (areaName == String.Empty) {
					throw new ArgumentException("No area name was provided (was empty).","areaName");
				}
				
				NWN2GameModule module = session.GetCurrentModule();
				
				if (module == null) {
					throw new InvalidOperationException("No module is currently open.");
				}						
				if (!module.Areas.ContainsCaseInsensitive(areaName)) {
					throw new ArgumentException("The current module does not contain an area named '" + areaName + "'.","areaName");
				}
				
				NWN2GameArea nwn2area = module.Areas[areaName];
				AreaBase area = session.CreateAreaBase(nwn2area);
							
				return area.GetObjects(type,tag).Count;
			}
			catch (ArgumentNullException e) {
				throw new FaultException<ArgumentNullException>(e,e.Message);
			}
			catch (ArgumentException e) {
				throw new FaultException<ArgumentException>(e,e.Message);
			}
			catch (InvalidOperationException e) {
				throw new FaultException<InvalidOperationException>(e,e.Message);
			}
			catch (Exception e) {
				throw new FaultException("(" + e.GetType() + ") " + e.Message);
			}
		}	
		
		
		/// <summary>
		/// Gets a collection of beans containing information about objects
		/// matching a given description in a given area.
		/// </summary>
		/// <param name="areaName">The area which has the objects.</param>
		/// <param name="type">The type of objects to collect.</param>
		/// <param name="tag">The tag that objects must have to be collected.
		/// Pass null to ignore this requirement.</param>
		/// <returns>A collection of beans containing information about
		/// objects matching the description.</returns>
		public IList<Bean> GetObjects(string areaName, NWN2ObjectType type, string tag)
		{
			try {
				if (areaName == null) {
					throw new ArgumentNullException("areaName","No area name was provided (was null).");
				}			
				if (areaName == String.Empty) {
					throw new ArgumentException("No area name was provided (was empty).","areaName");
				}
				
				NWN2GameModule module = session.GetCurrentModule();
				
				if (module == null) {
					throw new InvalidOperationException("No module is currently open.");
				}						
				if (!module.Areas.ContainsCaseInsensitive(areaName)) {
					throw new ArgumentException("The current module does not contain an area named '" + areaName + "'.","areaName");
				}
				
				NWN2GameArea nwn2area = module.Areas[areaName];
				AreaBase area = session.CreateAreaBase(nwn2area);
							
				List<INWN2Instance> instances = area.GetObjects(type,tag);				
				List<Bean> beans = new List<Bean>(instances.Count);
				
				foreach (INWN2Instance instance in instances) {
					beans.Add(new Bean(instance));
				}
				
				return beans;
			}
			catch (ArgumentNullException e) {
				throw new FaultException<ArgumentNullException>(e,e.Message);
			}
			catch (ArgumentException e) {
				throw new FaultException<ArgumentException>(e,e.Message);
			}
			catch (InvalidOperationException e) {
				throw new FaultException<InvalidOperationException>(e,e.Message);
			}
			catch (Exception e) {
				throw new FaultException("(" + e.GetType() + ") " + e.Message);
			}
		}
		
		
		/// <summary>
		/// Gets a unique object in an area from its properties.
		/// </summary>
		/// <param name="areaName">The area which has the object.</param>
		/// <param name="type">The type of the object.</param>
		/// <param name="guid">The unique Guid of the object.</param>
		/// <returns>The object within this area with the given properties,
		/// or null if one could not be found.</returns>
		public Bean GetObject(string areaName, NWN2ObjectType type, Guid guid)
		{
			try {
				if (guid == null) {
					throw new ArgumentNullException("guid","No guid was provided (was null).");
				}	
				if (areaName == null) {
					throw new ArgumentNullException("areaName","No area name was provided (was null).");
				}			
				if (areaName == String.Empty) {
					throw new ArgumentException("No area name was provided (was empty).","areaName");
				}
				
				NWN2GameModule module = session.GetCurrentModule();
				
				if (module == null) {
					throw new InvalidOperationException("No module is currently open.");
				}						
				if (!module.Areas.ContainsCaseInsensitive(areaName)) {
					throw new ArgumentException("The current module does not contain an area named '" + areaName + "'.","areaName");
				}
				
				NWN2GameArea nwn2area = module.Areas[areaName];
				AreaBase area = session.CreateAreaBase(nwn2area);
				
				INWN2Instance unique = area.GetObject(type,guid);
				
				if (unique == null) return null;
				else return new Bean(unique);
			}
			catch (ArgumentNullException e) {
				throw new FaultException<ArgumentNullException>(e,e.Message);
			}
			catch (ArgumentException e) {
				throw new FaultException<ArgumentException>(e,e.Message);
			}
			catch (InvalidOperationException e) {
				throw new FaultException<InvalidOperationException>(e,e.Message);
			}
			catch (Exception e) {
				throw new FaultException("(" + e.GetType() + ") " + e.Message);
			}
		}
						
		
		/// <summary>
		/// Gets a list of beans representing all of the
		/// scripts owned by the current module.
		/// </summary>
		/// <returns>A list of beans representing all of the
		/// scripts owned by the current module.</returns>
		public IList<Bean> GetScripts()
		{
			try {
				NWN2GameModule module = session.GetCurrentModule();
				if (module == null) {
					throw new InvalidOperationException("No module is currently open.");
				}			
				
				IList<Bean> beans = new List<Bean>();
				
				foreach (NWN2GameScript script in module.Scripts.Values) {
					bool loaded = script.Loaded;
					if (!loaded) script.Demand();
					beans.Add(new Bean(script));
					if (!loaded) script.Release();
				}
				
				return beans;
			}
			catch (InvalidOperationException e) {
				throw new FaultException<InvalidOperationException>(e,e.Message);
			}
			catch (Exception e) {
				throw new FaultException("(" + e.GetType() + ") " + e.Message);
			}
		}
		
		
		/// <summary>
		/// Gets a bean representing a
		/// script in the current module.
		/// </summary>
		/// <returns>A bean representing a
		/// script in the current module, or null if no
		/// such script exists.</returns>
		[FaultContract(typeof(System.ArgumentException))]
		[FaultContract(typeof(System.ArgumentNullException))]
		[FaultContract(typeof(System.InvalidOperationException))]
		public Bean GetScript(string name)
		{
			try {
				if (name == null) {
					throw new ArgumentNullException("name","No script name was provided (was null).");
				}	
				if (name == String.Empty) {
					throw new ArgumentException("name","No script name was provided (was empty).");
				}		
				
				NWN2GameModule module = session.GetCurrentModule();
				
				if (module == null) {
					throw new InvalidOperationException("No module is currently open.");
				}	
				
				if (!module.Scripts.ContainsCaseInsensitive(name)) {
					return null;
				}
				else {
					NWN2GameScript script = module.Scripts[name];
					bool loaded = script.Loaded;
					if (!loaded) script.Demand();
					Bean bean = new Bean(script);
					if (!loaded) script.Release();
					return bean;
				}
			}
			catch (ArgumentNullException e) {
				throw new FaultException<ArgumentNullException>(e,e.Message);
			}
			catch (ArgumentException e) {
				throw new FaultException<ArgumentException>(e,e.Message);
			}
			catch (InvalidOperationException e) {
				throw new FaultException<InvalidOperationException>(e,e.Message);
			}
			catch (Exception e) {
				throw new FaultException("(" + e.GetType() + ") " + e.Message);
			}
		}
		

		/// <summary>
		/// Adds a compiled script to the current module.
		/// </summary>
		/// <param name="path">The path to the compiled
		/// script, which will be an .NCS file.</param>
		public void AddCompiledScript(string path)
		{
			try {
				if (path == null) {
					throw new ArgumentNullException("path");
				}
				
				NWN2GameModule module = session.GetCurrentModule();
				if (module == null) {
					throw new InvalidOperationException("No module is currently open.");
				}
				
				FileInfo f = new FileInfo(path);
				if (!f.Exists) {
					throw new IOException("There is no file at " + path + ".");
				}
				if (f.Extension.ToLower() != ".ncs") {
					throw new IOException("The file at " + path + " has the wrong extension - " +
					                      "file has " + f.Extension + ", while compiled NWN2 " +
					                      "scripts have the extension .NCS.");
				}
				
				
				string folder = Path.GetDirectoryName(path);
				DirectoryResourceRepository repos = new DirectoryResourceRepository(folder);
				string filename = Path.GetFileName(path);
				
				IResourceEntry resource = null;
				
				/*
				 * There is another DirectoryResourceRepository method for finding resources:
				 * repos.FindResources(OEIResRef,ushort). This would be much quicker! And if we
				 * have lots of scripts in a module, that might be an important difference.
				 * However, I can't get it to work - an OEIResRef with an identical value is
				 * not seen as being equal. This comes down to its GetHashCode() method, but
				 * I'm not sure exactly why.
				 */
				foreach (IResourceEntry r in repos.FindResourcesByType(2010)) {
					if (r.FullName == filename) {
						resource = r;
					}
				}		
				
				if (resource == null) {
					throw new InvalidOperationException("Something went wrong - when trying" +
					                                    "to add a script to the module, the script " +
					                                    "resource was null.");
				}
							
				NWN2GameScript script = new NWN2GameScript(resource);
				module.Scripts.Add(script);
			}
			catch (ArgumentNullException e) {
				throw new FaultException<ArgumentNullException>(e,e.Message);
			}
			catch (InvalidOperationException e) {
				throw new FaultException<InvalidOperationException>(e,e.Message);
			}
			catch (IOException e) {
				throw new FaultException<IOException>(e,e.Message);
			}
			catch (Exception e) {
				throw new FaultException("(" + e.GetType() + ") " + e.Message);
			}
		}		
		
		
		/// <summary>
		/// Finds the script with the given name which 
		/// is already present in the module's script 
		/// collection, and attaches it to a particular
		/// script slot on a particular object in a 
		/// particular area.
		/// </summary>
		/// <param name="scriptName">The name of the script to use.</param>
		/// <param name="areaName">The area which has the object.</param>
		/// <param name="type">The type of the receiving object.</param>
		/// <param name="objectID">The unique ObjectID of the 
		/// receiving object.</param>
		/// <param name="scriptSlot">The script slot to attach
		/// the script to.</param>
		/// <remarks>To attach scripts to areas and modules,
		/// use AttachScriptToArea() and AttachScriptToModule().</remarks>
		[FaultContract(typeof(System.ArgumentNullException))]
		[FaultContract(typeof(System.ArgumentException))]
		[FaultContract(typeof(System.InvalidOperationException))]
		[FaultContract(typeof(System.IO.IOException))]
		public void AttachScriptToObject(string scriptName, string areaName, Nwn2EventRaiser type, Guid objectID, string scriptSlot)
		{
			try {
				if (scriptName == null) {
					throw new ArgumentNullException("scriptName");
				}
				if (scriptName == String.Empty) {
					throw new ArgumentException("scriptName");
				}
				if (areaName == null) {
					throw new ArgumentNullException("areaName");
				}
				if (areaName == String.Empty) {
					throw new ArgumentException("areaName");
				}
				if (scriptSlot == null) {
					throw new ArgumentNullException("scriptSlot");
				}
				if (scriptSlot == String.Empty) {
					throw new ArgumentException("scriptSlot");
				}
				if (!Nwn2ScriptSlot.GetScriptSlotNames(type).Contains(scriptSlot)) {
					throw new ArgumentException("Objects of type " + type + " do not have a script slot " +
					                            "named " + scriptSlot + " (call Sussex.Flip.Games.NeverwinterNightsTwo" +
					                            ".Utils.Nwn2ScriptSlot.GetScriptSlotNames() to find valid " +
					                            "script slot names.","scriptSlot");
				}
				
				NWN2GameModule module = session.GetCurrentModule();
				if (module == null) {
					throw new InvalidOperationException("No module is currently open.");
				}
				if (!module.Areas.ContainsCaseInsensitive(areaName)) {
					throw new ArgumentException("Module '" + GetCurrentModuleName() + "' has no area named '" + areaName + "'.","areaName");
				}
				if (!module.Scripts.ContainsCaseInsensitive(scriptName)) {
					throw new ArgumentException("Module '" + GetCurrentModuleName() + "' has no script named '" + scriptName + "'.","scriptName");
				}
								
				switch (type) {
					case Nwn2EventRaiser.Area:
						throw new InvalidOperationException("Correct usage: To add scripts to areas, use AttachScriptToArea().");
						
					case Nwn2EventRaiser.Module:
						throw new InvalidOperationException("Correct usage: To add scripts to areas, use AttachScriptToModule().");
						
					default:				
						NWN2ObjectType? nwn2Type = Nwn2ScriptSlot.GetObjectType(type);
						if (!nwn2Type.HasValue) {
							throw new ArgumentException("Couldn't understand Nwn2EventRaiserType " + type +
							                            " - it is not a module, an area, or one of the NWN2ObjectType " +
							                            "values!","type");
						}
						
						NWN2GameArea nwn2area = module.Areas[areaName];
						NWN2InstanceCollection instances = nwn2area.GetInstancesForObjectType(nwn2Type.Value);
						
						foreach (INWN2Instance instance in instances) {
							if (instance.ObjectID == objectID) {								
								foreach (PropertyInfo pi in instance.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)) {
									if (pi.Name == scriptSlot) {										
										NWN2GameScript script = module.Scripts[scriptName];		
										bool loaded = script.Loaded;
										if (!loaded) script.Demand();
										pi.SetValue(instance,script.Resource,null);
										if (!loaded) script.Release();
										return;
									}
								}								
								throw new ArgumentException("Couldn't find a script slot named " + scriptSlot + ".","scriptSlot");
							}
						}
						throw new ArgumentException("No " + type + " with ObjectID " + objectID + " could be found in area " + areaName + ".",
						                            "objectID");
				}
			}
			catch (ArgumentNullException e) {
				throw new FaultException<ArgumentNullException>(e,e.Message);
			}
			catch (ArgumentException e) {
				throw new FaultException<ArgumentException>(e,e.Message);
			}
			catch (InvalidOperationException e) {
				throw new FaultException<InvalidOperationException>(e,e.Message);
			}
			catch (Exception e) {
				throw new FaultException("(" + e.GetType() + ") " + e.Message);
			}
		}
		
		#endregion
	}
}
