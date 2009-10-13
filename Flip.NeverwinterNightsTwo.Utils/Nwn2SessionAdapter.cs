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
using System.ServiceModel;
using NWN2Toolset.NWN2.Data;
using NWN2Toolset.NWN2.Data.Instances;
using NWN2Toolset.NWN2.Data.Templates;
using NWN2Toolset.NWN2.IO;

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
		
		#endregion
	}
}
