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
using Sussex.Flip.Utils;

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
		/// Gets the path to the working ('temp') copy of the module that is currently open in the toolset.
		/// </summary>
		/// <returns>The temp path of the current module, or null if no module is open.</returns>
		public string GetCurrentModuleTempPath()
		{
			try {
				return session.GetCurrentModuleTempPath();
			}
			catch (ApplicationException e) {
				throw new FaultException<ApplicationException>(e,e.Message);
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
		/// Gets an area in the current module.
		/// </summary>
		/// <param name="name">The name of the area.</param>
		/// <returns>The named area, or null if one could not be found.</returns>
		[FaultContract(typeof(System.ArgumentException))]
		[FaultContract(typeof(System.ArgumentNullException))]
		[FaultContract(typeof(System.InvalidOperationException))]
		public Bean GetArea(string name)
		{
			try {
				if (name == null) {
					throw new ArgumentNullException("name","No area name was provided (was null).");
				}			
				if (name == String.Empty) {
					throw new ArgumentException("No area name was provided (was empty).","name");
				}
				
				NWN2GameModule module = session.GetCurrentModule();
				
				if (module == null) {
					throw new InvalidOperationException("No module is currently open.");
				}						
				if (!module.Areas.ContainsCaseInsensitive(name)) {
					return null;
				}
				
				NWN2GameArea nwn2area = module.Areas[name];
				return new Bean(nwn2area);
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
		/// Gets a list of beans representing 
		/// the areas in the current module.
		/// </summary>
		/// <returns>A list of beans representing all of the
		/// areas owned by the current module.</returns>
		[FaultContract(typeof(System.InvalidOperationException))]
		public IList<Bean> GetAreas()
		{
			try {
				NWN2GameModule module = session.GetCurrentModule();
				if (module == null) {
					throw new InvalidOperationException("No module is currently open.");
				}			
				
				IList<Bean> beans = new List<Bean>();
				
				foreach (NWN2GameArea area in module.Areas.Values) {
					beans.Add(new Bean(area));
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
		/// Gets a bean representing the current module.
		/// </summary>
		/// <returns>A bean representing the current module.</returns>
		[FaultContract(typeof(System.InvalidOperationException))]
		public Bean GetModule()
		{
			try {
				NWN2GameModule module = session.GetCurrentModule();
				if (module == null) {
					throw new InvalidOperationException("No module is currently open.");
				}			
				Bean bean = new Bean(module);
				bean.Capture(module.ModuleInfo,false);
				return bean;
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
		/// uncompiled scripts owned by the current module.
		/// </summary>
		/// <returns>A list of beans representing all of the
		/// uncompiled scripts owned by the current module.</returns>
		public IList<Bean> GetUncompiledScripts()
		{
			try {
				NWN2GameModule module = session.GetCurrentModule();
				if (module == null) {
					throw new InvalidOperationException("No module is currently open.");
				}			
				
				IList<Bean> beans = new List<Bean>();
				
				foreach (NWN2GameScript script in module.Scripts.Values) {
					Bean bean = new Bean(script);
					beans.Add(bean);
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
		/// Gets a bean representing an uncompiled
		/// script in the current module.
		/// </summary>
		/// <returns>A bean representing an uncompiled
		/// script in the current module, or null if no
		/// such script exists.</returns>
		[FaultContract(typeof(System.ArgumentException))]
		[FaultContract(typeof(System.ArgumentNullException))]
		[FaultContract(typeof(System.InvalidOperationException))]
		public Bean GetUncompiledScript(string name)
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
					Bean bean = new Bean(script);
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
		/// Gets a list of beans representing all of the
		/// compiled scripts owned by the current module.
		/// </summary>
		/// <returns>A list of beans representing all of the
		/// compiled scripts owned by the current module.</returns>
		[FaultContract(typeof(System.ApplicationException))]
		[FaultContract(typeof(System.InvalidOperationException))]
		public IList<Bean> GetCompiledScripts()
		{
			try {
				NWN2GameModule module = session.GetCurrentModule();
				if (module == null) {
					throw new InvalidOperationException("No module is currently open.");
				}				
				if (module.Repository == null) {
					throw new ApplicationException("The module's repository was missing.");
				}	
								
				IList<Bean> beans = new List<Bean>();
				
				ushort NCS = OEIShared.Utils.BWResourceTypes.GetResourceType("NCS");					
				OEIGenericCollectionWithEvents<IResourceEntry> resources = module.Repository.FindResourcesByType(NCS);
				
				foreach (IResourceEntry r in resources) {
					NWN2GameScript script = new NWN2GameScript(r);
					script.Demand();
					beans.Add(new Bean(script));
					script.Release();
				}
				
				return beans;
			}
			catch (ApplicationException e) {
				throw new FaultException<ApplicationException>(e,e.Message);
			}
			catch (InvalidOperationException e) {
				throw new FaultException<InvalidOperationException>(e,e.Message);
			}
			catch (Exception e) {
				throw new FaultException("(" + e.GetType() + ") " + e.Message);
			}
		}
		
							
		/// <summary>
		/// Gets a bean representing an compiled
		/// script in the current module.
		/// </summary>
		/// <returns>A bean representing an compiled
		/// script in the current module, or null if no
		/// such script exists.</returns>
		[FaultContract(typeof(System.ApplicationException))]
		[FaultContract(typeof(System.ArgumentNullException))]
		[FaultContract(typeof(System.ArgumentException))]
		[FaultContract(typeof(System.InvalidOperationException))]
		public Bean GetCompiledScript(string name)
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
				if (module.Repository == null) {
					throw new ApplicationException("The module's repository was missing.");
				}
				
				ushort resourceType = OEIShared.Utils.BWResourceTypes.GetResourceType("NCS");	
				OEIResRef cResRef = new OEIResRef(name);
				IResourceEntry r = module.Repository.FindResource(cResRef,resourceType);
							
				if (r == null) return null;
				else {
					NWN2GameScript script = new NWN2GameScript(r);
					script.Demand();
					Bean bean = new Bean(script);
					script.Release();
					return bean;
				}
			}
			catch (ArgumentNullException e) {
				throw new FaultException<ArgumentNullException>(e,e.Message);
			}
			catch (ArgumentException e) {
				throw new FaultException<ArgumentException>(e,e.Message);
			}
			catch (ApplicationException e) {
				throw new FaultException<ApplicationException>(e,e.Message);
			}
			catch (InvalidOperationException e) {
				throw new FaultException<InvalidOperationException>(e,e.Message);
			}
			catch (Exception e) {
				throw new FaultException("(" + e.GetType() + ") " + e.Message);
			}
		}
		
		
		/// <summary>
		/// Checks whether the current module has a compiled script
		/// of the given name.
		/// </summary>
		/// <param name="name">The name of the script.</param>
		/// <returns>True if the current module has a .NCS compiled
		/// script file of the given name, and false otherwise.</returns>
		[FaultContract(typeof(System.ArgumentNullException))]
		[FaultContract(typeof(System.ArgumentException))]
		[FaultContract(typeof(System.InvalidOperationException))]
		public bool HasCompiled(string name)
		{
			try {
				return session.HasCompiled(name);
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
		/// Checks whether the current module has an uncompiled script
		/// of the given name.
		/// </summary>
		/// <param name="name">The name of the script.</param>
		/// <returns>True if the current module has a .NSS uncompiled
		/// script file of the given name, and false otherwise.</returns>
		[FaultContract(typeof(System.ArgumentNullException))]
		[FaultContract(typeof(System.ArgumentException))]
		[FaultContract(typeof(System.InvalidOperationException))]
		public bool HasUncompiled(string name)
		{
			try {
				return session.HasUncompiled(name);
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
		/// Adds an uncompiled script to the current module.
		/// </summary>
		/// <param name="name">The name to save the script under.</param>
		/// <param name="code">The contents of the script.</param>
		[FaultContract(typeof(System.ArgumentException))]
		[FaultContract(typeof(System.ArgumentNullException))]
		[FaultContract(typeof(System.InvalidOperationException))]
		public void AddUncompiledScript(string name, string code)
		{
			try {
				if (name == null) {
					throw new ArgumentNullException("name");
				}
				if (code == null) {
					throw new ArgumentNullException("code");
				}
				
				NWN2GameModule module = session.GetCurrentModule();
				if (module == null) {
					throw new InvalidOperationException("No module is currently open.");
				}
								
				NWN2GameScript script = new NWN2GameScript(name,
				                                           module.Repository.DirectoryName,
				                                           module.Repository);				
				script.Module = module;
				script.Data = code;
				
				module.Scripts.Add(script);	/* or module.AddResource(script) */
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
		[FaultContract(typeof(System.ArgumentNullException))]
		[FaultContract(typeof(System.InvalidOperationException))]
		[FaultContract(typeof(System.IO.IOException))]
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
				
				string filename = Path.GetFileName(path);
				string temp = session.GetCurrentModuleTempPath();
				string newPath = Path.Combine(temp,filename);
				
				File.Copy(path,newPath);
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
		/// Deletes a script from the current module.
		/// </summary>
		/// <param name="name">The name of the script.</param>
		/// <remarks>Both compiled and uncompiled copies
		/// of the script are deleted.</remarks>
		[FaultContract(typeof(ArgumentException))]
		[FaultContract(typeof(ArgumentNullException))]
		[FaultContract(typeof(InvalidOperationException))]
		[FaultContract(typeof(System.IO.IOException))]
		public void DeleteScript(string name)
		{
			try {
				if (name == null) {
					throw new ArgumentNullException("name");
				}
				if (name == String.Empty) {
					throw new ArgumentException("name");
				}
								
				NWN2GameModule module = session.GetCurrentModule();
				if (module == null) {
					throw new InvalidOperationException("No module is currently open.");
				}
				if (!session.HasUncompiled(name) && !session.HasCompiled(name)) {
					throw new ArgumentException("Module '" + GetCurrentModuleName() + "' has no script named '" + name + "'.","name");
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
			catch (ArgumentNullException e) {
				throw new FaultException<ArgumentNullException>(e,e.Message);
			}
			catch (ArgumentException e) {
				throw new FaultException<ArgumentException>(e,e.Message);
			}
			catch (InvalidDataException e) {
				throw new FaultException<InvalidDataException>(e,e.Message);
			}
			catch (InvalidOperationException e) {
				throw new FaultException<InvalidOperationException>(e,e.Message);
			}
			catch (Exception e) {
				throw new FaultException("(" + e.GetType() + ") " + e.Message);
			}
		}
		
		
		/// <summary>
		/// Compiles a script in the current module.
		/// </summary>
		/// <param name="name">The name of the script to compile.</param>
		[FaultContract(typeof(System.ArgumentException))]
		[FaultContract(typeof(System.ArgumentNullException))]
		[FaultContract(typeof(System.InvalidOperationException))]
		[FaultContract(typeof(System.IO.InvalidDataException))]
		public void CompileScript(string name)
		{
			try {
				if (name == null) {
					throw new ArgumentNullException("name");
				}
				if (name == String.Empty) {
					throw new ArgumentException("name");
				}
								
				NWN2GameModule module = session.GetCurrentModule();
				if (module == null) {
					throw new InvalidOperationException("No module is currently open.");
				}
				if (!session.HasUncompiled(name)) {
					throw new ArgumentException("Module '" + GetCurrentModuleName() + "' has no uncompiled script named '" + name + "'.","name");
				}
				
				NWN2GameScript script;
				try {
					script = module.Scripts[name];
				}
				catch (ArgumentOutOfRangeException) {
					throw new ArgumentException("Scripts collection for this module did not feature a script named '" + name + "'.");
				}
								
				NWN2Toolset.NWN2ToolsetMainForm.Compiler.GenerateDebugInfo = true;
				string debug = NWN2Toolset.NWN2ToolsetMainForm.Compiler.CompileFile(script.Name,GetCurrentModuleTempPath());
				if (debug.Length > 0) {
					throw new InvalidDataException("'" + name + "' could not be compiled: " + debug);
				}
			}
			catch (ArgumentNullException e) {
				throw new FaultException<ArgumentNullException>(e,e.Message);
			}
			catch (ArgumentException e) {
				throw new FaultException<ArgumentException>(e,e.Message);
			}
			catch (InvalidDataException e) {
				throw new FaultException<InvalidDataException>(e,e.Message);
			}
			catch (InvalidOperationException e) {
				throw new FaultException<InvalidOperationException>(e,e.Message);
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
		[FaultContract(typeof(System.IO.InvalidDataException))]
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
					                            "script slot names.)","scriptSlot");
				}
				
				NWN2GameModule module = session.GetCurrentModule();
				if (module == null) {
					throw new InvalidOperationException("No module is currently open.");
				}
				if (!module.Areas.ContainsCaseInsensitive(areaName)) {
					throw new ArgumentException("Module '" + GetCurrentModuleName() + "' has no area named '" + areaName + "'.","areaName");
				}
				if (!HasCompiled(scriptName)) {
					if (HasUncompiled(scriptName)) {
						throw new InvalidDataException("Script '" + scriptName + "' must be compiled before it can be attached.");
					}
					else {
						throw new ArgumentException("Module '" + GetCurrentModuleName() + "' has no script named '" + scriptName + "'.","scriptName");
					}
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
								PropertyInfo pi = instance.GetType().GetProperty(scriptSlot);
															
								NWN2GameScript script = module.Scripts[scriptName];		
								bool loaded = script.Loaded;
								if (!loaded) script.Demand();
								pi.SetValue(instance,script.Resource,null);
								if (!loaded) script.Release();
								return;
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
			catch (InvalidDataException e) {
				throw new FaultException<InvalidDataException>(e,e.Message);
			}
			catch (InvalidOperationException e) {
				throw new FaultException<InvalidOperationException>(e,e.Message);
			}
			catch (Exception e) {
				throw new FaultException("(" + e.GetType() + ") " + e.Message);
			}
		}
				
				
		/// <summary>
		/// Finds the compiled script with the given name which 
		/// is already present in the module's script 
		/// collection, and attaches it to the nominated
		/// script slot on the area of the given name.
		/// </summary>
		/// <param name="scriptName">The name of the compiled script.</param>
		/// <param name="areaName">The area to attach the script to.</param>
		/// <param name="scriptSlot">The script slot to attach
		/// the script to.</param>
		[FaultContract(typeof(System.ArgumentNullException))]
		[FaultContract(typeof(System.ArgumentException))]
		[FaultContract(typeof(System.InvalidOperationException))]
		[FaultContract(typeof(System.IO.InvalidDataException))]
		[FaultContract(typeof(System.IO.IOException))]
		public void AttachScriptToArea(string scriptName, string areaName, string scriptSlot)
		{
			try {
				if (scriptName == null) {
					throw new ArgumentNullException("scriptName");
				}
				if (scriptName == String.Empty) {
					throw new ArgumentException("scriptName cannot be empty.","scriptName");
				}
				if (areaName == null) {
					throw new ArgumentNullException("areaName");
				}
				if (areaName == String.Empty) {
					throw new ArgumentException("areaName cannot be empty.","areaName");
				}
				if (scriptSlot == null) {
					throw new ArgumentNullException("scriptSlot");
				}
				if (scriptSlot == String.Empty) {
					throw new ArgumentException("scriptSlot cannot be empty.","scriptSlot");
				}
				if (!Nwn2ScriptSlot.GetScriptSlotNames(Nwn2EventRaiser.Area).Contains(scriptSlot)) {
					throw new ArgumentException("Areas do not have a script slot " +
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
				if (!HasCompiled(scriptName)) {
					if (HasUncompiled(scriptName)) {
						throw new InvalidDataException("Script '" + scriptName + "' must be compiled before it can be attached.");
					}
					else {
						throw new ArgumentException("Module '" + GetCurrentModuleName() + "' has no script named '" + scriptName + "'.","scriptName");
					}
				}	
				
				NWN2GameArea area = module.Areas[areaName];			
				
				PropertyInfo p = area.GetType().GetProperty(scriptSlot);
				if (p == null) {
					throw new ArgumentException("No property named " + scriptSlot +
					                            " could be found on area " + areaName + ".");
				}
				
				NWN2GameScript script = module.Scripts[scriptName];
				
				bool loaded = script.Loaded;
				if (!loaded) script.Demand();
				p.SetValue(area,script.Resource,null);
				if (!loaded) script.Release();
			}
			catch (ArgumentNullException e) {
				throw new FaultException<ArgumentNullException>(e,e.Message);
			}
			catch (ArgumentException e) {
				throw new FaultException<ArgumentException>(e,e.Message);
			}
			catch (InvalidDataException e) {
				throw new FaultException<InvalidDataException>(e,e.Message);
			}
			catch (InvalidOperationException e) {
				throw new FaultException<InvalidOperationException>(e,e.Message);
			}
			catch (Exception e) {
				throw new FaultException("(" + e.GetType() + ") " + e.Message);
			}
		}
		
				
		/// <summary>
		/// Finds the compiled script with the given name which 
		/// is already present in the module's script 
		/// collection, and attaches it to the nominated
		/// script slot on the module.
		/// </summary>
		/// <param name="scriptName">The name of the compiled script.</param>
		/// <param name="scriptSlot">The script slot to attach
		/// the script to.</param>
		[FaultContract(typeof(System.ArgumentNullException))]
		[FaultContract(typeof(System.ArgumentException))]
		[FaultContract(typeof(System.InvalidOperationException))]
		[FaultContract(typeof(System.IO.InvalidDataException))]
		[FaultContract(typeof(System.IO.IOException))]
		public void AttachScriptToModule(string scriptName, string scriptSlot)
		{
			try {
				if (scriptName == null) {
					throw new ArgumentNullException("scriptName");
				}
				if (scriptName == String.Empty) {
					throw new ArgumentException("scriptName cannot be empty.","scriptName");
				}
				if (scriptSlot == null) {
					throw new ArgumentNullException("scriptSlot");
				}
				if (scriptSlot == String.Empty) {
					throw new ArgumentException("scriptSlot cannot be empty.","scriptSlot");
				}
				if (!Nwn2ScriptSlot.GetScriptSlotNames(Nwn2EventRaiser.Module).Contains(scriptSlot)) {
					throw new ArgumentException("Modules do not have a script slot " +
					                            "named " + scriptSlot + " (call Sussex.Flip.Games.NeverwinterNightsTwo" +
					                            ".Utils.Nwn2ScriptSlot.GetScriptSlotNames() to find valid " +
					                            "script slot names.","scriptSlot");
				}
				
				NWN2GameModule module = session.GetCurrentModule();
				if (module == null) {
					throw new InvalidOperationException("No module is currently open.");
				}
				if (!HasCompiled(scriptName)) {
					if (HasUncompiled(scriptName)) {
						throw new InvalidDataException("Script '" + scriptName + "' must be compiled before it can be attached.");
					}
					else {
						throw new ArgumentException("Module '" + GetCurrentModuleName() + "' has no script named '" + scriptName + "'.","scriptName");
					}
				}	
				
				PropertyInfo p = module.ModuleInfo.GetType().GetProperty(scriptSlot);
				if (p == null) {
					throw new ArgumentException("No property named " + scriptSlot +
					                            " could be found on the module.");
				}
				
				NWN2GameScript script = module.Scripts[scriptName];
				
				bool loaded = script.Loaded;
				if (!loaded) script.Demand();
				p.SetValue(module.ModuleInfo,script.Resource,null);
				if (!loaded) script.Release();
			}
			catch (ArgumentNullException e) {
				throw new FaultException<ArgumentNullException>(e,e.Message);
			}
			catch (ArgumentException e) {
				throw new FaultException<ArgumentException>(e,e.Message);
			}
			catch (InvalidDataException e) {
				throw new FaultException<InvalidDataException>(e,e.Message);
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
