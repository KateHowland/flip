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
 * This file added by Keiron Nicholson on 25/09/2009 at 15:36.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.ServiceModel;
using NWN2Toolset;
using NWN2Toolset.NWN2.Data;
using NWN2Toolset.NWN2.Data.Blueprints;
using NWN2Toolset.NWN2.Data.Instances;
using NWN2Toolset.NWN2.Data.Templates;
using NWN2Toolset.NWN2.Data.TypedCollections;
using NWN2Toolset.NWN2.Views;
using NWN2Toolset.NWN2.IO;
using OEIShared.IO;
using OEIShared.IO.GFF;
using OEIShared.Utils;
using Sussex.Flip.Utils;
using Crownwood.DotNetMagic.Controls;
using Crownwood.DotNetMagic.Common;
using Crownwood.DotNetMagic.Docking;
using VisualHint.SmartPropertyGrid;

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
		
		/// <summary>
		/// A collection of lists of fields to serialise for a particular
		/// type of object, indexed by the name of the type of object. 
		/// </summary>
		protected Dictionary<string,List<string>> allSerialisingFields;
		
		/// <summary>
		/// The client(s) subscribing to this service, who the service
		/// will call callback methods on.
		/// </summary>
		protected ClientNotifier notify;
		
		#endregion
		
		#region Constructors
				
		/// <summary>
		/// Constructs a new <see cref="Nwn2SessionAdapter"/> instance.
		/// </summary>
		/// <remarks>This default constructor wraps a Nwn2Session object.</remarks>
		public Nwn2SessionAdapter()
		{
			session = new Nwn2Session();
			notify = new ClientNotifier();
			ReportToolsetEvents();
			InitialiseListsOfFieldsToSerialise();			
		}
		
		
		/// <summary>
		/// Constructs a new <see cref="Nwn2SessionAdapter"/> instance.
		/// </summary>
		/// <param name="session">The session this adapter wraps.</param>
		public Nwn2SessionAdapter(INwn2Session session)
		{
			if (session == null) throw new ArgumentNullException("session");			
			this.session = session;
			notify = new ClientNotifier();
			ReportToolsetEvents();
			InitialiseListsOfFieldsToSerialise();
		}
		
		#endregion
		
		#region Tracking toolset events
		
		/// <summary>
		/// Notifies the client when one of a selection of toolset events occurs,
		/// including objects/resources/blueprints being added or removed, resource
		/// viewers being opened or closed, a script slot on an object or resource
		/// changing, and the module changing.
		/// </summary>
		protected void ReportToolsetEvents()
		{
			/*
			 * Track when a module is opened, when resources are added to or removed from
			 * the module, when blueprints are added to or removed from the module, and 
			 * when objects are added to or removed from an area.
			 */
			NWN2ToolsetMainForm.ModuleChanged += delegate(object oSender, ModuleChangedEventArgs e) 
			{  
				NWN2GameModule mod = NWN2ToolsetMainForm.App.Module;
				
				/*
				 * There seems to be a bug where these events fire at a later stage... they do fire
				 * when they should, but when the paused test is allowed to complete, they fire again.
				 */
				foreach (NWN2BlueprintCollection bc in mod.BlueprintCollections) {
					NWN2BlueprintSetInfoTuple t = (NWN2BlueprintSetInfoTuple)bc.Tag;
							
					bc.Inserted += delegate(OEICollectionWithEvents cList, int index, object value) 
					{
						notify.NotifyBlueprintAdded(t.ObjectType,((INWN2Blueprint)value).ResourceName.Value);
					};
					
					bc.Removed += delegate(OEICollectionWithEvents cList, int index, object value)
					{
						notify.NotifyBlueprintRemoved(t.ObjectType,((INWN2Blueprint)value).ResourceName.Value);
					};
				}
				
				string oldModule = e.OldModule == null ? String.Empty : e.OldModule.Name;
				notify.NotifyModuleChanged(GetModuleName(),e.OldModule.Name);
				
				if (mod != null) {				
					List<OEIDictionaryWithEvents> dictionaries;
					dictionaries = new List<OEIDictionaryWithEvents> { mod.Areas, mod.Conversations, mod.Scripts };
						
					OEIDictionaryWithEvents.ChangeHandler dAdded = new OEIDictionaryWithEvents.ChangeHandler(ResourceAdded);
					OEIDictionaryWithEvents.ChangeHandler dRemoved = new OEIDictionaryWithEvents.ChangeHandler(ResourceRemoved);
					foreach (OEIDictionaryWithEvents dictionary in dictionaries) {
						dictionary.Inserted += dAdded;
						dictionary.Removed += dRemoved;
					}
				}
			};
			
			FieldInfo[] fields = typeof(NWN2ToolsetMainForm).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
			foreach (FieldInfo field in fields) {	
				/*
				 * Track when resource viewers are opening or closing.
				 */
				if (field.FieldType == typeof(TabbedGroups)) {
					TabbedGroups tg = (TabbedGroups)field.GetValue(NWN2ToolsetMainForm.App);
					
					CollectionChange opened = new CollectionChange(ViewerOpened);
					CollectionChange closed = new CollectionChange(ViewerClosed);
					
					tg.ActiveLeaf.TabPages.Inserted += opened;
					tg.ActiveLeaf.TabPages.Removed += closed;
					
					// ActiveLeaf is disposed whenever all viewers are closed, so attach handlers again:
					tg.ActiveLeafChanged += delegate
					{  
						tg.ActiveLeaf.TabPages.Inserted += opened;
						tg.ActiveLeaf.TabPages.Removed += closed;
					};
				}
			}
		}

		
		/// <summary>
		/// Notifies the client that a resource viewer has been opened.
		/// </summary>
		protected void ViewerOpened(int index, object value)
		{
			TabPage page = (TabPage)value;
			INWN2Viewer viewer = page.Control as INWN2Viewer;
			if (viewer == null) return;
			
			string type;
			
			if (viewer.ViewedResource is NWN2GameScript) {
				type = "Script";
			}
			else if (viewer.ViewedResource is NWN2GameConversation) {
				type = "Conversation";
			}
			else if (viewer.ViewedResource is NWN2GameArea) {
				type = "Area";
			}
			else {
				type = viewer.ViewedResource.GetType().ToString();
			}
			
			notify.NotifyResourceViewerOpened(type,page.Title);
		}

		
		/// <summary>
		/// Notifies the client that a resource viewer has been closed.
		/// </summary>
		/// <remarks>Due to the viewer already being disposed, the client
		/// is only informed of the name of the resource, not the type.</remarks>
		protected void ViewerClosed(int index, object value)
		{					
			TabPage page = (TabPage)value;
			/* Even when handling Removing rather than Removed, the viewer object
			 * has already been disposed, so we can't tell what type of resource
			 * was closed. The responsibility for checking falls to the client. */
			notify.NotifyResourceViewerClosed(page.Title);
		}
		

		/// <summary>
		/// Notifies the client that a game object has been added to an area.
		/// </summary>
		protected void ObjectAdded(OEICollectionWithEvents cList, int index, object value)
		{
			INWN2Object obj = (INWN2Object)value;
			INWN2Instance ins = (INWN2Instance)value;
			NWN2GameArea area = (NWN2GameArea)cList.Tag;
			
			notify.NotifyObjectAdded(ins.ObjectType,obj.Tag,ins.ObjectID,area.Name);
		}
		

		/// <summary>
		/// Notifies the client that a game object has been removed from an area.
		/// </summary>
		protected void ObjectRemoved(OEICollectionWithEvents cList, int index, object value)
		{
			INWN2Object obj = (INWN2Object)value;
			INWN2Instance ins = (INWN2Instance)value;
			NWN2GameArea area = (NWN2GameArea)cList.Tag;
			
			notify.NotifyObjectRemoved(ins.ObjectType,obj.Tag,ins.ObjectID,area.Name);
		}

		
		/// <summary>
		/// Notifies the client that a script, area, or conversation has been added to the module.
		/// </summary>
		protected void ResourceAdded(OEIDictionaryWithEvents cDictionary, object key, object value)
		{
			NWN2GameArea area = null;			
			string type, name;
			
			if (value is NWN2GameScript) {
				type = "Script";
				name = ((NWN2GameScript)value).Name;
			}
			else if (value is NWN2GameConversation) {
				type = "Conversation";
				name = ((NWN2GameConversation)value).Name;
			}
			else if (value is NWN2GameArea) {
				area = (NWN2GameArea)value;
				type = "Area";
				name = area.Name;
			}
			else {
				type = value.GetType().ToString();
				name = String.Empty;
			}
			
			notify.NotifyResourceAdded(type,name);
			
			if (area != null) {
				OEICollectionWithEvents.ChangeHandler cAdded = new OEICollectionWithEvents.ChangeHandler(ObjectAdded);
				OEICollectionWithEvents.ChangeHandler cRemoved = new OEICollectionWithEvents.ChangeHandler(ObjectRemoved);
				
				foreach (NWN2InstanceCollection instances in area.AllInstances) {
					instances.Inserted += cAdded;
					instances.Removed += cRemoved;
				}
			}
		}

		
		/// <summary>
		/// Notifies the client that a script, area, or conversation has been removed from the module.
		/// </summary>
		protected void ResourceRemoved(OEIDictionaryWithEvents cDictionary, object key, object value)
		{			
			string type, name;
			
			if (value is NWN2GameScript) {
				type = "Script";
				name = ((NWN2GameScript)value).Name;
			}
			else if (value is NWN2GameConversation) {
				type = "Conversation";
				name = ((NWN2GameConversation)value).Name;
			}
			else if (value is NWN2GameArea) {
				type = "Area";
				name = ((NWN2GameArea)value).Name;
			}
			else {
				type = value.GetType().ToString();
				name = String.Empty;
			}
			
			notify.NotifyResourceRemoved(type,name);
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
		/// Creates and opens a Neverwinter Nights 2 game module of location type Temporary.
		/// </summary>
		/// <returns>The path the module was created at.</returns>
		public string CreateAndOpenTemporaryModule()
		{
			try {
				return session.CreateAndOpenTemporaryModule();
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
				session.SaveModule(session.GetModule());
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
		public string GetModuleName()
		{
			try {
				NWN2GameModule module = session.GetModule();
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
		public string GetModulePath()
		{
			try {
				return session.GetModulePath();
			}
			catch (Exception e) {
				throw new FaultException("(" + e.GetType() + ") " + e.Message);
			}
		}
		
		
		/// <summary>
		/// Gets the path to the working ('temp') copy of the module that is currently open in the toolset.
		/// </summary>
		/// <returns>The temp path of the current module, or null if no module is open.</returns>
		public string GetModuleTempPath()
		{
			try {
				return session.GetModuleTempPath();
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
		public ModuleLocationType? GetModuleLocation()
		{
			try {
				NWN2GameModule module = session.GetModule();
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
		/// Gets the name of the currently viewed area, if one exists.
		/// </summary>
		/// <returns>The name of the currently viewed area, or null if no area is being viewed (or
		/// if no module is open).</returns>		
		public string GetCurrentArea()
		{
			try {
				NWN2GameModule module = session.GetModule();
				if (module == null) return null;
				else if (NWN2Toolset.NWN2ToolsetMainForm.App.AreaContents.Area == null) return null;
				else return NWN2Toolset.NWN2ToolsetMainForm.App.AreaContents.Area.Name;
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
		/// <remarks>As is the case when working directly with the toolset,
		/// file modules must be saved after adding an area or the area will
		/// not persist - directory modules do not have this requirement.</remarks>
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
		/// <returns>Returns the unique ID the object is assigned upon creation.</returns>
		public Guid AddObject(string areaName, NWN2ObjectType type, string resref, string tag)
		{			
			try {
				if (areaName == null) {
					throw new ArgumentNullException("areaName","No area name was provided (was null).");
				}			
				if (areaName == String.Empty) {
					throw new ArgumentException("No area name was provided (was empty).","areaName");
				}
				
				NWN2GameModule module = session.GetModule();
				
				if (module == null) {
					throw new InvalidOperationException("No module is currently open.");
				}						
				if (!module.Areas.ContainsCaseInsensitive(areaName)) {
					throw new ArgumentException("The current module does not contain an area named '" + areaName + "'.","areaName");
				}
				
				NWN2GameArea nwn2area = module.Areas[areaName];
				INWN2Instance instance = session.AddObject(nwn2area,type,resref,tag);
				return instance.ObjectID;
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
		/// Gets a bean containing information about
		/// the blueprint with the given resref and type,
		/// if one exists.
		/// </summary>
		/// <param name="resRef">The resref value of the
		/// blueprint to return.</param>
		/// <param name="type">The object type of the
		/// blueprint to return.</param>
		/// <param name="full">True to serialise every field on this
		/// blueprint; false to only serialise a predetermined selection 
		/// of fields.</param>
		/// <returns>A bean containing information
		/// about the blueprint, or null if no such blueprint exists.</returns>
		public Bean GetBlueprint(string resRef, NWN2ObjectType type, bool full)
		{
			try {	
				INWN2Blueprint blueprint = session.GetBlueprint(resRef,type);
				
				if (blueprint == null) {
					return null;
				}
				else {
					Bean bean;
					if (!full) bean = new Bean(blueprint,GetFieldsToSerialise(type.ToString()));
					else bean = new Bean(blueprint);
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
		/// Gets a list of the resrefs of all blueprints of a given type.
		/// </summary>
		/// <param name="type">The object type of the
		/// blueprints to return.</param>
		/// <returns>A list of resrefs.</returns>
		public IList<string> GetBlueprintResRefs(NWN2ObjectType type)
		{
			try {
				NWN2BlueprintCollection blueprints = session.GetBlueprints(type);
				IList<string> list = new List<string>(blueprints.Count);
				foreach (INWN2Blueprint blueprint in blueprints) {
					list.Add(blueprint.ResourceName.Value);
				}
				return list;
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
		/// <param name="id">The unique ID of the object.</param>
		/// <param name="full">True to serialise every field on this
		/// object; false to only serialise a predetermined selection 
		/// of fields.</param>
		/// <returns>The object within this area with the given properties,
		/// or null if one could not be found.</returns>
		/// <remarks>This method will throw an InvalidOperationException if
		/// the area is not open.</remarks>
		public Bean GetObject(string areaName, NWN2ObjectType type, Guid id, bool full)
		{
			try {
				if (areaName == null) {
					throw new ArgumentNullException("areaName","No area name was provided (was null).");
				}			
				if (areaName == String.Empty) {
					throw new ArgumentException("No area name was provided (was empty).","areaName");
				}
				
				NWN2GameModule module = session.GetModule();
				
				if (module == null) {
					throw new InvalidOperationException("No module is currently open.");
				}						
				if (!module.Areas.ContainsCaseInsensitive(areaName)) {
					throw new ArgumentException("The current module does not contain an area named '" + areaName + "'.","areaName");
				}
				
				NWN2GameArea nwn2area = module.Areas[areaName];
				
				INWN2Instance unique = session.GetObject(nwn2area,type,id);
				
				if (unique == null) return null;
				else {
					Bean bean;
					if (!full) bean = new Bean(unique,GetFieldsToSerialise(type.ToString()));
					else bean = new Bean(unique);
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
		/// Gets a list of the unique IDs of each object of the given type
		/// in the given area.
		/// </summary>
		/// <param name="areaName">The area which has the objects.</param>
		/// <param name="type">The type of objects to collect.</param>
		/// <returns>A list of Guid values.</returns>
		/// <remarks>This method will throw an InvalidOperationException if
		/// the area is not open.</remarks>
		public IList<Guid> GetObjectIDs(string areaName, NWN2ObjectType type)
		{
			try {
				if (areaName == null) {
					throw new ArgumentNullException("areaName","No area name was provided (was null).");
				}			
				if (areaName == String.Empty) {
					throw new ArgumentException("No area name was provided (was empty).","areaName");
				}
				
				NWN2GameModule module = session.GetModule();
				
				if (module == null) {
					throw new InvalidOperationException("No module is currently open.");
				}						
				if (!module.Areas.ContainsCaseInsensitive(areaName)) {
					throw new ArgumentException("The current module does not contain an area named '" + areaName + "'.","areaName");
				}
				
				NWN2GameArea nwn2area = module.Areas[areaName];
				NWN2InstanceCollection instances = session.GetObjects(nwn2area,type);
								
				List<Guid> ids = new List<Guid>(instances.Count);
				
				foreach (INWN2Instance instance in instances) {
					ids.Add(instance.ObjectID);
				}
				
				return ids;
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
		/// Gets a list of the unique IDs of each object of the given type
		/// and tag in the given area.
		/// </summary>
		/// <param name="areaName">The area which has the objects.</param>
		/// <param name="type">The type of objects to collect.</param>
		/// <param name="tag">The tag that objects must have to be collected.
		/// Pass null to ignore this requirement.</param>
		/// <returns>A list of Guid values.</returns>
		/// <remarks>This method will throw an InvalidOperationException if
		/// the area is not open.</remarks>
		public IList<Guid> GetObjectIDsByTag(string areaName, NWN2ObjectType type, string tag)
		{
			try {
				if (areaName == null) {
					throw new ArgumentNullException("areaName","No area name was provided (was null).");
				}			
				if (areaName == String.Empty) {
					throw new ArgumentException("No area name was provided (was empty).","areaName");
				}
				
				NWN2GameModule module = session.GetModule();
				
				if (module == null) {
					throw new InvalidOperationException("No module is currently open.");
				}						
				if (!module.Areas.ContainsCaseInsensitive(areaName)) {
					throw new ArgumentException("The current module does not contain an area named '" + areaName + "'.","areaName");
				}
				
				NWN2GameArea nwn2area = module.Areas[areaName];
				AreaBase area = session.CreateAreaBase(nwn2area);
							
				NWN2InstanceCollection instances = area.GetObjects(type,tag);				
				List<Guid> ids = new List<Guid>(instances.Count);
				
				foreach (INWN2Instance instance in instances) {
					ids.Add(instance.ObjectID);
				}
				
				return ids;
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
		/// <param name="full">True to serialise every field on this
		/// area; false to only serialise a predetermined selection 
		/// of fields.</param>
		/// <returns>The named area, or null if one could not be found.</returns>
		public Bean GetArea(string name, bool full)
		{
			try {
				NWN2GameArea area = session.GetArea(name);
				if (area == null) return null;
				
				bool loaded = area.Loaded;
				if (!loaded) area.Demand();	
				
				Bean bean;
				if (!full) bean = new Bean(area,GetFieldsToSerialise("Area"));
				else bean = new Bean(area);
				
				// Store the value of 'Loaded' that will apply by the time the bean is returned,
				// rather than now (when the area MUST be loaded to populate the bean):
				bean["Loaded"] = loaded.ToString();
				
				if (!loaded) area.Release();
				
				return bean;
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
		/// Gets the names of the
		/// areas in the current module.
		/// </summary>
		/// <returns>A list of area names.</returns>
		public IList<string> GetAreaNames()
		{
			try {
				NWN2GameModule module = session.GetModule();
				if (module == null) {
					throw new InvalidOperationException("No module is currently open.");
				}			
				
				IList<string> areas = new List<string>(module.Areas.Count);
				
				foreach (string name in module.Areas.Keys) {
					areas.Add(name);
				}
				
				return areas;
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
		public Bean GetModule()
		{
			try {
				NWN2GameModule module = session.GetModule();
				if (module == null) {
					throw new InvalidOperationException("No module is currently open.");
				}			
				Bean bean = new Bean(module,GetFieldsToSerialise("Module"));
				bean.Capture(module.ModuleInfo,false,GetFieldsToSerialise("ModuleInformation")); // add ModuleInformation fields
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
		/// Gets a list of names of all of the
		/// scripts owned by the current module
		/// in uncompiled form.
		/// </summary>
		/// <returns>A list of script names.</returns>
		public IList<string> GetScriptNames()
		{
			try {
				IList<string> scripts = new List<string>();				
				foreach (NWN2GameScript script in session.GetScripts().Values) {
					scripts.Add(script.Name);
				}				
				return scripts;
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
		public Bean GetScript(string name)
		{
			try {
				NWN2GameScript script = session.GetScript(name);
				if (script == null) return null;
					
				bool loaded = script.Loaded;
				if (!loaded) script.Demand();	
				Bean bean = new Bean(script,GetFieldsToSerialise("Script"));
					
				// Store the value of 'Loaded' that will apply by the time the bean is returned,
				// rather than now (when the script MUST be loaded to populate the bean):
				bean["Loaded"] = loaded.ToString();
					
				if (!loaded) script.Release();
					
				return bean;
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
		/// Gets a list of names of all of the
		/// scripts owned by the current module
		/// in compiled form.
		/// </summary>
		/// <returns>A list of script names.</returns>
		public IList<string> GetCompiledScriptNames()
		{
			try {	
				IList<string> scripts = new List<string>();				
				OEIGenericCollectionWithEvents<IResourceEntry> resources = session.GetCompiledScripts();
				
				foreach (IResourceEntry r in resources) {
					scripts.Add(r.ResRef.Value);
				}
				
				return scripts;
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
		/// Gets a bean representing the compiled
		/// version of a script owned by the current module.
		/// </summary>
		/// <returns>A bean representing an compiled
		/// script in the current module, or null if no
		/// such script exists.</returns>
		public Bean GetCompiledScript(string name)
		{
			try {				
				IResourceEntry r = session.GetCompiledScript(name);
							
				if (r == null) {
					return null;
				}
				else {
					NWN2GameScript script = new NWN2GameScript(r);
					
					bool loaded = script.Loaded;
					if (!loaded) script.Demand();	
					Bean bean = new Bean(script,GetFieldsToSerialise("Script"));
					
					// Store the value of 'Loaded' that will apply by the time the bean is returned,
					// rather than now (when the script MUST be loaded to populate the bean):
					bean["Loaded"] = loaded.ToString();
					
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
		public void AddScript(string name, string code)
		{
			try {
				session.AddScript(name,code);
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
		/// Deletes a script from the current module.
		/// </summary>
		/// <param name="name">The name of the script.</param>
		/// <remarks>Both compiled and uncompiled copies
		/// of the script are deleted.</remarks>
		public void DeleteScript(string name)
		{
			try {
				session.DeleteScript(name);
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
		/// <remarks>The script will be automatically saved before compiling.</remarks>
		public void CompileScript(string name)
		{
			try {
				if (name == null) {
					throw new ArgumentNullException("name");
				}
				if (name == String.Empty) {
					throw new ArgumentException("name");
				}
								
				NWN2GameModule module = session.GetModule();
				if (module == null) {
					throw new InvalidOperationException("No module is currently open.");
				}
				if (!session.HasUncompiled(name)) {
					throw new ArgumentException("Module '" + GetModuleName() + "' has no uncompiled script named '" + name + "'.","name");
				}
				
				NWN2GameScript script;
				try {
					script = module.Scripts[name];
				}
				catch (ArgumentOutOfRangeException) {
					throw new ArgumentException("Scripts collection for this module did not feature a script named '" + name + "'.");
				}
				session.CompileScript(script);
			}
			catch (ApplicationException e) {
				throw new FaultException<ApplicationException>(e,e.Message);
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
		/// <param name="slot">The script slot to attach
		/// the script to.</param>
		/// <remarks>To attach scripts to areas and modules,
		/// use AttachScriptToArea() and AttachScriptToModule().</remarks>
		public void AttachScriptToObject(string scriptName, string areaName, Nwn2EventRaiser type, Guid objectID, string slot)
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
				if (slot == null) {
					throw new ArgumentNullException("slot");
				}
				if (slot == String.Empty) {
					throw new ArgumentException("slot");
				}
				if (!Nwn2ScriptSlot.GetScriptSlotNames(type).Contains(slot)) {
					throw new ArgumentException("Objects of type " + type + " do not have a script slot " +
					                            "named " + slot + " (call Sussex.Flip.Games.NeverwinterNightsTwo" +
					                            ".Utils.Nwn2ScriptSlot.GetScriptSlotNames() to find valid " +
					                            "script slot names.)","slot");
				}
								
				switch (type) {
					case Nwn2EventRaiser.Area:
						throw new InvalidOperationException("Correct usage: To add scripts to areas, use AttachScriptToArea().");
						
					case Nwn2EventRaiser.Module:
						throw new InvalidOperationException("Correct usage: To add scripts to areas, use AttachScriptToModule().");
						
					default:
						NWN2GameModule module = session.GetModule();
						if (module == null) {
							throw new InvalidOperationException("No module is currently open.");
						}
						
						NWN2GameArea nwn2area = session.GetArea(areaName);
						if (nwn2area == null) {
							throw new ArgumentException("Module '" + GetModuleName() + "' has no area named '" + areaName + "'.","areaName");
						}
						
						if (!session.HasCompiled(scriptName)) {
							if (session.HasUncompiled(scriptName)) {
								throw new InvalidDataException("Script '" + scriptName + "' must be compiled before it can be attached.");
							}
							else {
								throw new ArgumentException("Module '" + module.Name + "' has no script named '" + scriptName + "'.");
							}
						}	
						
						NWN2GameScript script = session.GetScript(scriptName);
						
						NWN2ObjectType? nwn2Type = Nwn2ScriptSlot.GetObjectType(type);
						if (!nwn2Type.HasValue) {
							throw new ArgumentException("Couldn't understand Nwn2EventRaiserType " + type +
							                            " - it is not a module, an area, or one of the NWN2ObjectType " +
							                            "values!","type");
						}				
						
						INWN2Instance instance = session.GetObject(nwn2area,nwn2Type.Value,objectID);
						if (instance == null) {
							throw new ArgumentException("No object matching the given criteria could be found.","objectID");
						}
						
						session.AttachScriptToObject(script,instance,slot);
						break;
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
		/// <param name="slot">The script slot to attach
		/// the script to.</param>
		public void AttachScriptToArea(string scriptName, string areaName, string slot)
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
				if (slot == null) {
					throw new ArgumentNullException("slot");
				}
				if (slot == String.Empty) {
					throw new ArgumentException("slot cannot be empty.","slot");
				}
				if (!Nwn2ScriptSlot.GetScriptSlotNames(Nwn2EventRaiser.Area).Contains(slot)) {
					throw new ArgumentException("Areas do not have a script slot " +
					                            "named " + slot + " (call Sussex.Flip.Games.NeverwinterNightsTwo" +
					                            ".Utils.Nwn2ScriptSlot.GetScriptSlotNames() to find valid " +
					                            "script slot names.","scriptSlot");
				}
				
				NWN2GameModule module = session.GetModule();
				if (module == null) {
					throw new InvalidOperationException("No module is currently open.");
				}
						
				NWN2GameArea nwn2area = session.GetArea(areaName);
				if (nwn2area == null) {
					throw new ArgumentException("Module '" + GetModuleName() + "' has no area named '" + areaName + "'.","areaName");
				}
						
				if (!session.HasCompiled(scriptName)) {
					if (session.HasUncompiled(scriptName)) {
						throw new InvalidDataException("Script '" + scriptName + "' must be compiled before it can be attached.");
					}
					else {
						throw new ArgumentException("Module '" + module.Name + "' has no script named '" + scriptName + "'.");
					}
				}	
					
				NWN2GameScript script = session.GetScript(scriptName);
						
				session.AttachScriptToArea(script,nwn2area,slot);
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
		/// <param name="slot">The script slot to attach
		/// the script to.</param>
		public void AttachScriptToModule(string scriptName, string slot)
		{
			try {
				if (scriptName == null) {
					throw new ArgumentNullException("scriptName");
				}
				if (scriptName == String.Empty) {
					throw new ArgumentException("scriptName cannot be empty.","scriptName");
				}
				if (slot == null) {
					throw new ArgumentNullException("slot");
				}
				if (slot == String.Empty) {
					throw new ArgumentException("slot cannot be empty.","slot");
				}
				if (!Nwn2ScriptSlot.GetScriptSlotNames(Nwn2EventRaiser.Module).Contains(slot)) {
					throw new ArgumentException("Modules do not have a script slot " +
					                            "named " + slot + " (call Sussex.Flip.Games.NeverwinterNightsTwo" +
					                            ".Utils.Nwn2ScriptSlot.GetScriptSlotNames() to find valid " +
					                            "script slot names.","scriptSlot");
				}
				
				NWN2GameModule module = session.GetModule();
				if (module == null) {
					throw new InvalidOperationException("No module is currently open.");
				}
						
				if (!session.HasCompiled(scriptName)) {
					if (session.HasUncompiled(scriptName)) {
						throw new InvalidDataException("Script '" + scriptName + "' must be compiled before it can be attached.");
					}
					else {
						throw new ArgumentException("Module '" + module.Name + "' has no script named '" + scriptName + "'.");
					}
				}	
					
				NWN2GameScript script = session.GetScript(scriptName);
						
				session.AttachScriptToModule(script,slot);
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
		/// Detaches the script from the given script slot on the given
		/// object, if a script was attached.
		/// </summary>
		/// <param name="areaName">The area containing the object.</param>
		/// <param name="type">The type of the receiving object.</param>
		/// <param name="objectID">The unique ObjectID of the 
		/// object with the given script slot.</param>
		/// <param name="slot">The script slot to remove
		/// any scripts from.</param>
		public void ClearScriptSlotOnObject(string areaName, Guid objectID, Nwn2EventRaiser type, string slot)
		{
			try {
				if (areaName == null) {
					throw new ArgumentNullException("areaName");
				}
				if (areaName == String.Empty) {
					throw new ArgumentException("areaName");
				}
				if (slot == null) {
					throw new ArgumentNullException("slot");
				}
				if (slot == String.Empty) {
					throw new ArgumentException("slot");
				}
				if (!Nwn2ScriptSlot.GetScriptSlotNames(type).Contains(slot)) {
					throw new ArgumentException("Objects of type " + type + " do not have a script slot " +
					                            "named " + slot + " (call Sussex.Flip.Games.NeverwinterNightsTwo" +
					                            ".Utils.Nwn2ScriptSlot.GetScriptSlotNames() to find valid " +
					                            "script slot names.)","slot");
				}
								
				switch (type) {
					case Nwn2EventRaiser.Area:
						throw new InvalidOperationException("Correct usage: To clear script slots on areas, use ClearScriptSlotOnArea().");
						
					case Nwn2EventRaiser.Module:
						throw new InvalidOperationException("Correct usage: To clear script slots on modules, use ClearScriptSlotOnModule().");
						
					default:
						NWN2GameModule module = session.GetModule();
						if (module == null) {
							throw new InvalidOperationException("No module is currently open.");
						}
						
						NWN2GameArea nwn2area = session.GetArea(areaName);
						if (nwn2area == null) {
							throw new ArgumentException("Module '" + GetModuleName() + "' has no area named '" + areaName + "'.","areaName");
						}
						
						NWN2ObjectType? nwn2Type = Nwn2ScriptSlot.GetObjectType(type);
						if (!nwn2Type.HasValue) {
							throw new ArgumentException("Couldn't understand Nwn2EventRaiserType " + type +
							                            " - it is not a module, an area, or one of the NWN2ObjectType " +
							                            "values!","type");
						}				
						
						INWN2Instance instance = session.GetObject(nwn2area,nwn2Type.Value,objectID);
						if (instance == null) {
							throw new ArgumentException("No object matching the given criteria could be found.","objectID");
						}
						
						session.ClearScriptSlotOnObject(instance,slot);
						break;
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
		/// Detaches the script from the given script slot on the given
		/// area, if a script was attached.
		/// </summary>
		/// <param name="areaName">The area with the given script slot.</param>
		/// <param name="slot">The script slot to remove
		/// any scripts from.</param>
		public void ClearScriptSlotOnArea(string areaName, string slot)
		{
			try {
				if (areaName == null) {
					throw new ArgumentNullException("areaName");
				}
				if (areaName == String.Empty) {
					throw new ArgumentException("areaName cannot be empty.","areaName");
				}
				if (slot == null) {
					throw new ArgumentNullException("slot");
				}
				if (slot == String.Empty) {
					throw new ArgumentException("slot cannot be empty.","slot");
				}
				if (!Nwn2ScriptSlot.GetScriptSlotNames(Nwn2EventRaiser.Area).Contains(slot)) {
					throw new ArgumentException("Areas do not have a script slot " +
					                            "named " + slot + " (call Sussex.Flip.Games.NeverwinterNightsTwo" +
					                            ".Utils.Nwn2ScriptSlot.GetScriptSlotNames() to find valid " +
					                            "script slot names.","scriptSlot");
				}
				
				NWN2GameModule module = session.GetModule();
				if (module == null) {
					throw new InvalidOperationException("No module is currently open.");
				}
						
				NWN2GameArea nwn2area = session.GetArea(areaName);
				if (nwn2area == null) {
					throw new ArgumentException("Module '" + GetModuleName() + "' has no area named '" + areaName + "'.","areaName");
				}
						
				session.ClearScriptSlotOnArea(nwn2area,slot);
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
		/// Detaches the script from the given script slot on the current
		/// module, if a script was attached.
		/// </summary>
		/// <param name="slot">The script slot to remove
		/// any scripts from.</param>
		public void ClearScriptSlotOnModule(string slot)
		{
			try {
				if (slot == null) {
					throw new ArgumentNullException("slot");
				}
				if (slot == String.Empty) {
					throw new ArgumentException("slot cannot be empty.","slot");
				}
				if (!Nwn2ScriptSlot.GetScriptSlotNames(Nwn2EventRaiser.Module).Contains(slot)) {
					throw new ArgumentException("Modules do not have a script slot " +
					                            "named " + slot + " (call Sussex.Flip.Games.NeverwinterNightsTwo" +
					                            ".Utils.Nwn2ScriptSlot.GetScriptSlotNames() to find valid " +
					                            "script slot names.","scriptSlot");
				}
				
				NWN2GameModule module = session.GetModule();
				if (module == null) {
					throw new InvalidOperationException("No module is currently open.");
				}
						
				session.ClearScriptSlotOnModule(slot);
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
		/// Gets a list containing the names of all areas
		/// which are open in area viewers in the current module.
		/// </summary>
		/// <returns>A list of names of open areas.</returns>
		public IList<string> GetOpenAreaNames()
		{
			try {				
				NWN2GameModule module = session.GetModule();
				if (module == null) {
					throw new InvalidOperationException("No module is currently open.");
				}
				
				List<NWN2Toolset.NWN2.Views.NWN2AreaViewer> viewers = NWN2Toolset.NWN2ToolsetMainForm.App.GetAllAreaViewers();
				List<string> areas = new List<string>(viewers.Count);
				
				foreach (NWN2Toolset.NWN2.Views.NWN2AreaViewer av in viewers) {
					areas.Add(av.Area.Name);
				}
				
				return areas;
			}
			catch (InvalidOperationException e) {
				throw new FaultException<InvalidOperationException>(e,e.Message);
			}
			catch (Exception e) {
				throw new FaultException("(" + e.GetType() + ") " + e.Message);
			}
		}
		
		
		/// <summary>
		/// Opens an area in an area viewer.
		/// </summary>
		/// <param name="name">The name of the area to open.</param>
		public void OpenArea(string name)
		{
			try {
				if (name == null) {
					throw new ArgumentNullException("name");
				}
				if (name == String.Empty) {
					throw new ArgumentException("name cannot be empty.","name");
				}
				
				NWN2GameModule module = session.GetModule();
				if (module == null) {
					throw new InvalidOperationException("No module is currently open.");
				}
				if (!module.Areas.ContainsCaseInsensitive(name)) {
					throw new ArgumentException("Module '" + GetModuleName() + "' has no area named '" + name + "'.","areaName");
				}
				
				if (NWN2Toolset.NWN2ToolsetMainForm.App.GetAllAreaViewers().Count == 3) {
					throw new InvalidOperationException("3 is the maximum number of areas which can be open at once.");
				}
				
				NWN2GameArea area = module.Areas[name];	
				if (area == null) throw new ArgumentException("The NWN2GameArea object for this area ('" +
				                                              name + "') could not be found.");	
								
				NWN2Toolset.NWN2ToolsetMainForm.App.ShowResource(area);
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
		/// Closes the area viewer for an area, if one is
		/// currently open.
		/// </summary>
		/// <param name="name">The name of the area to close.</param>
		public void CloseArea(string name)
		{
			try {
				if (name == null) {
					throw new ArgumentNullException("name");
				}
				if (name == String.Empty) {
					throw new ArgumentException("name cannot be empty.","name");
				}
				
				NWN2GameModule module = session.GetModule();
				if (module == null) {
					throw new InvalidOperationException("No module is currently open.");
				}
				if (!module.Areas.ContainsCaseInsensitive(name)) {
					throw new ArgumentException("Module '" + GetModuleName() + "' has no area named '" + name + "'.","areaName");
				}
				
				NWN2GameArea area = module.Areas[name];	
				if (area == null) throw new ArgumentException("The NWN2GameArea object for this area ('" +
				                                              name + "') could not be found.");	
				
				NWN2Toolset.NWN2.Views.INWN2Viewer viewer = NWN2Toolset.NWN2ToolsetMainForm.App.GetViewerForResource(area);
				if (viewer == null) return;
				NWN2Toolset.NWN2ToolsetMainForm.App.CloseViewer(viewer,true);
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
		/// Checks whether an area viewer is currently open
		/// for a particular area.
		/// </summary>
		/// <param name="name">The name of the area.</param>
		/// <returns>True if an area viewer for the named
		/// area is currently open in the toolset; false
		/// otherwise.</returns>
		public bool AreaIsOpen(string name)
		{
			try {
				if (name == null) {
					throw new ArgumentNullException("name");
				}
				if (name == String.Empty) {
					throw new ArgumentException("name cannot be empty.","name");
				}
				
				NWN2GameModule module = session.GetModule();
				if (module == null) {
					throw new InvalidOperationException("No module is currently open.");
				}
				if (!module.Areas.ContainsCaseInsensitive(name)) {
					throw new ArgumentException("Module '" + GetModuleName() + "' has no area named '" + name + "'.","areaName");
				}
				
				NWN2GameArea area = module.Areas[name];	
				if (area == null) throw new ArgumentException("The NWN2GameArea object for this area ('" +
				                                              name + "') could not be found.");	
				
				NWN2Toolset.NWN2.Views.INWN2Viewer viewer = NWN2Toolset.NWN2ToolsetMainForm.App.GetViewerForResource(area);
				return viewer != null;
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
		/// Gets a list containing the names of all scripts
		/// which are open in script viewers in the current module.
		/// </summary>
		/// <returns>A list of names of open scripts.</returns>
		public IList<string> GetOpenScriptNames()
		{
			try {				
				NWN2GameModule module = session.GetModule();
				if (module == null) {
					throw new InvalidOperationException("No module is currently open.");
				}
				
				List<NWN2Toolset.NWN2.Views.NWN2ScriptViewer> viewers = new List<NWN2Toolset.NWN2.Views.NWN2ScriptViewer>();
				foreach (NWN2Toolset.NWN2.Views.INWN2Viewer v in NWN2Toolset.NWN2ToolsetMainForm.App.GetAllViewers()) {
					NWN2Toolset.NWN2.Views.NWN2ScriptViewer s = v as NWN2Toolset.NWN2.Views.NWN2ScriptViewer;
					if (s != null) viewers.Add(s);
				}
				
				List<string> scripts = new List<string>(viewers.Count);
				
				foreach (NWN2Toolset.NWN2.Views.NWN2ScriptViewer sv in viewers) {
					scripts.Add(sv.Script.Name);
				}
				
				return scripts;
			}
			catch (InvalidOperationException e) {
				throw new FaultException<InvalidOperationException>(e,e.Message);
			}
			catch (Exception e) {
				throw new FaultException("(" + e.GetType() + ") " + e.Message);
			}
		}
		
		
		/// <summary>
		/// Opens a script in a script viewer.
		/// </summary>
		/// <param name="name">The name of the script to open.</param>
		public void OpenScript(string name)
		{
			try {
				if (name == null) {
					throw new ArgumentNullException("name");
				}
				if (name == String.Empty) {
					throw new ArgumentException("name cannot be empty.","name");
				}
				
				NWN2GameModule module = session.GetModule();
				if (module == null) {
					throw new InvalidOperationException("No module is currently open.");
				}
				if (!module.Scripts.ContainsCaseInsensitive(name)) {
					throw new ArgumentException("Module '" + GetModuleName() + "' has no script named '" + name + "'.","name");
				}
				
				NWN2GameScript script = module.Scripts[name];
				if (script == null) throw new ArgumentException("The NWN2GameScript object for this script ('" + 
				                                                name + "') could not be found.");				
				
				NWN2Toolset.NWN2ToolsetMainForm.App.ShowResource(script);
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
		/// Closes the script viewer for a script, if one is
		/// currently open.
		/// </summary>
		/// <param name="name">The name of the script to close.</param>
		[FaultContract(typeof(System.ArgumentNullException))]
		[FaultContract(typeof(System.ArgumentException))]
		[FaultContract(typeof(System.InvalidOperationException))]
		public void CloseScript(string name)
		{
			try {
				if (name == null) {
					throw new ArgumentNullException("name");
				}
				if (name == String.Empty) {
					throw new ArgumentException("name cannot be empty.","name");
				}
				
				NWN2GameModule module = session.GetModule();
				if (module == null) {
					throw new InvalidOperationException("No module is currently open.");
				}
				if (!module.Scripts.ContainsCaseInsensitive(name)) {
					throw new ArgumentException("Module '" + GetModuleName() + "' has no script named '" + name + "'.","name");
				}
				
				NWN2GameScript script = module.Scripts[name];
				if (script == null) throw new ArgumentException("The NWN2GameScript object for this script ('" + 
				                                                name + "') could not be found.");
				
				NWN2Toolset.NWN2.Views.INWN2Viewer viewer = NWN2Toolset.NWN2ToolsetMainForm.App.GetViewerForResource(script);
				if (viewer == null) return;				
				NWN2Toolset.NWN2ToolsetMainForm.App.CloseViewer(viewer,true);
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
		/// Checks whether a script viewer is currently open
		/// for a particular script.
		/// </summary>
		/// <param name="name">The name of the script.</param>
		/// <returns>True if a script viewer for the named
		/// script is currently open in the toolset; false
		/// otherwise.</returns>
		public bool ScriptIsOpen(string name)
		{
			try {
				if (name == null) {
					throw new ArgumentNullException("name");
				}
				if (name == String.Empty) {
					throw new ArgumentException("name cannot be empty.","name");
				}
				
				NWN2GameModule module = session.GetModule();
				if (module == null) {
					throw new InvalidOperationException("No module is currently open.");
				}
				if (!module.Scripts.ContainsCaseInsensitive(name)) {
					throw new ArgumentException("Module '" + GetModuleName() + "' has no script named '" + name + "'.","name");
				}
				
				NWN2GameScript script = module.Scripts[name];	
				if (script == null) throw new ArgumentException("The NWN2GameScript object for this script ('" +
				                                              	name + "') could not be found.");	
				
				NWN2Toolset.NWN2.Views.INWN2Viewer viewer = NWN2Toolset.NWN2ToolsetMainForm.App.GetViewerForResource(script);
				return viewer != null;
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
		/// Loads a script resource from disk, ensuring that
		/// the script object is fully loaded (but overwriting
		/// any unsaved changes that were made).
		/// </summary>
		/// <param name="name">The name of the script.</param>
		public void DemandScript(string name)
		{
			try {
				if (name == null) {
					throw new ArgumentNullException("name");
				}
				if (name == String.Empty) {
					throw new ArgumentException("name cannot be empty.","name");
				}
				
				NWN2GameModule module = session.GetModule();
				if (module == null) {
					throw new InvalidOperationException("No module is currently open.");
				}
				if (!module.Scripts.ContainsCaseInsensitive(name)) {
					throw new ArgumentException("Module '" + GetModuleName() + "' has no script named '" + name + "'.","name");
				}
				
				NWN2GameScript script = module.Scripts[name];	
				if (script == null) throw new ArgumentException("The NWN2GameScript object for this script ('" +
				                                              	name + "') could not be found.");	
				
				script.Demand();
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
		/// Releases a script resource.
		/// </summary>
		/// <param name="name">The name of the script.</param>
		public void ReleaseScript(string name)
		{
			try {
				if (name == null) {
					throw new ArgumentNullException("name");
				}
				if (name == String.Empty) {
					throw new ArgumentException("name cannot be empty.","name");
				}
				
				NWN2GameModule module = session.GetModule();
				if (module == null) {
					throw new InvalidOperationException("No module is currently open.");
				}
				if (!module.Scripts.ContainsCaseInsensitive(name)) {
					throw new ArgumentException("Module '" + GetModuleName() + "' has no script named '" + name + "'.","name");
				}
				
				NWN2GameScript script = module.Scripts[name];	
				if (script == null) throw new ArgumentException("The NWN2GameScript object for this script ('" +
				                                              	name + "') could not be found.");	
				
				script.Release();
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
		/// Loads an area from disk, ensuring that
		/// the area object is fully loaded (but overwriting
		/// any unsaved changes that were made).
		/// </summary>
		/// <param name="name">The name of the area.</param>
		public void DemandArea(string name)
		{
			try {
				if (name == null) {
					throw new ArgumentNullException("name");
				}
				if (name == String.Empty) {
					throw new ArgumentException("name cannot be empty.","name");
				}
				
				NWN2GameModule module = session.GetModule();
				if (module == null) {
					throw new InvalidOperationException("No module is currently open.");
				}
				if (!module.Areas.ContainsCaseInsensitive(name)) {
					throw new ArgumentException("Module '" + GetModuleName() + "' has no area named '" + name + "'.","name");
				}
				
				NWN2GameArea area = module.Areas[name];	
				if (area == null) throw new ArgumentException("The NWN2GameArea object for this area ('" +
				                                               name + "') could not be found.");	
				
				area.Demand();
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
		/// Releases an area resource.
		/// </summary>
		/// <param name="name">The name of the area.</param>
		public void ReleaseArea(string name)
		{
			try {
				if (name == null) {
					throw new ArgumentNullException("name");
				}
				if (name == String.Empty) {
					throw new ArgumentException("name cannot be empty.","name");
				}
				
				NWN2GameModule module = session.GetModule();
				if (module == null) {
					throw new InvalidOperationException("No module is currently open.");
				}
				if (!module.Areas.ContainsCaseInsensitive(name)) {
					throw new ArgumentException("Module '" + GetModuleName() + "' has no area named '" + name + "'.","name");
				}
				
				NWN2GameArea area = module.Areas[name];	
				if (area == null) throw new ArgumentException("The NWN2GameArea object for this area ('" +
				                                               name + "') could not be found.");	
				
				area.Release();
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
	
		#region Other methods
				
		/// <summary>
		/// Initialises the lists of fields to serialise.
		/// </summary>
		protected virtual void InitialiseListsOfFieldsToSerialise()
		{
			allSerialisingFields = new Dictionary<string,List<string>>(17);
			
			// Serialise these fields for every object (if they appear):
			List<string> universal = new List<string>{"BlueprintLocation",
													  "LocalizedDescription",
													  "LocalizedName",
													  "Name",
													  "ObjectID",
													  "ObjectType",
													  "ResourceName",
													  "Tag",
													  "TemplateResRef"};	
							
			foreach (NWN2ObjectType type in Enum.GetValues(typeof(NWN2ObjectType))) {
				string key = type.ToString();
				allSerialisingFields.Add(key,new List<string>(universal));
				allSerialisingFields[key].AddRange(Nwn2ScriptSlot.GetScriptSlotNames(type));
			}			
						
			// Serialise these fields for specific types of object:
			allSerialisingFields["Creature"].AddRange(new List<string>{"Conversation",
																	   "CustomPortrait",
																	   "FactionID",
																	   "FirstName",
																	   "Gender",
																	   "GoodEvil",																	   
																	   "LastName",
																	   "LawfulChaotic",
																	   "Tag"});	
			allSerialisingFields["Item"].AddRange(new List<string>{"LocalizedDescriptionIdentified"});	
			
			// Serialise these fields for areas and modules:
			allSerialisingFields.Add("Area",new List<string>{"DisplayName",
			                         						 "HasTerrain",
			                         						 "Interior",
			                         						 "Loaded",
			                         						 "Module",
			                         						 "Name",
			                         						 "Natural",
			                         						 "Size",
			                         						 "Tag"});	
			allSerialisingFields["Area"].AddRange(Nwn2ScriptSlot.GetScriptSlotNames(Nwn2EventRaiser.Area));
			
			allSerialisingFields.Add("Module",new List<string>{"FileName",
			                         						   "LocationType",
			                         						   "Name"});	
			
			allSerialisingFields.Add("ModuleInformation",new List<string>{"CampaignID",
						                         						  "Description",
						                         						  "EntryArea",
						                         						  "MinimumGameVersion",
						                         						  "ModuleID",
						                         						  "NX1Required",
						                         						  "NX2Required",
						                         						  "Tag"});			
			allSerialisingFields["ModuleInformation"].AddRange(Nwn2ScriptSlot.GetScriptSlotNames(Nwn2EventRaiser.Module));
		}
		
		
		/// <summary>
		/// Returns a list of names of fields which will be serialised
		/// on objects of the given type.
		/// </summary>
		/// <param name="type">A string representing a particular type of object.</param>
		/// <returns>A list of field names, or null if no list is stored
		/// for the given type.</returns>
		public IList<string> GetFieldsToSerialise(string type)
		{
			if (allSerialisingFields.ContainsKey(type)) return allSerialisingFields[type];
			else return null;
		}
		
		#endregion
	}
}
