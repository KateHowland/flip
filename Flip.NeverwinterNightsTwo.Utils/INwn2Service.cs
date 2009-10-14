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
 * This file added by Keiron Nicholson on 25/09/2009 at 15:03.
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.ServiceModel;
using NWN2Toolset.NWN2.Data;

namespace Sussex.Flip.Games.NeverwinterNightsTwo.Utils
{
	/// <summary>
	/// A service providing access to a Neverwinter Nights 2 toolset session.
	/// </summary>
	[ServiceContract]
	public interface INwn2Service
	{
		/// <summary>
		/// Creates a Neverwinter Nights 2 game module.
		/// </summary>
		/// <param name="path">The path to create the module at. If 'location'
		/// is set to ModuleLocationType.Directory, this must be the path for
		/// a folder to be created within NWN2Toolset.NWN2ToolsetMainForm.ModulesDirectory.</param>
		/// <param name="location">The serialisation form of the module.</param>
		[OperationContract]
		[FaultContract(typeof(System.ArgumentNullException))]
		[FaultContract(typeof(System.ArgumentException))]
		[FaultContract(typeof(System.IO.IOException))]
		[FaultContract(typeof(System.NotSupportedException))]
		void CreateModule(string path, NWN2Toolset.NWN2.IO.ModuleLocationType location);
		
		
		/// <summary>
		/// Opens a Neverwinter Nights 2 game module.
		/// </summary>
		/// <param name="name">The path of the module to open.</param>
		/// <param name="location">The serialisation form of the module.</param>
		[OperationContract]
		[FaultContract(typeof(System.ArgumentNullException))]
		[FaultContract(typeof(System.ArgumentException))]
		[FaultContract(typeof(System.IO.IOException))]
		void OpenModule(string path, NWN2Toolset.NWN2.IO.ModuleLocationType location);
		
		
		/// <summary>
		/// Saves changes to the current game module.
		/// </summary>
		/// <remarks>Saves to the default modules directory.</remarks>
		[OperationContract]
		[FaultContract(typeof(System.ArgumentException))]
		void SaveModule();
		
		
		/// <summary>
		/// Closes the current module.
		/// </summary>
		[OperationContract]
		void CloseModule();
		
		
		/// <summary>
		/// Gets the name of the module that is currently open in the toolset.
		/// </summary>
		/// <returns>The name of the current module, or null if no module is open.</returns>
		[OperationContract]
		string GetCurrentModuleName();
		
		
		/// <summary>
		/// Gets the absolute path of the module that is currently open in the toolset.
		/// </summary>
		/// <returns>The absolute path of the current module, or null if no module is open.</returns>
		[OperationContract]
		string GetCurrentModulePath();
		
		
		/// <summary>
		/// Gets the location type of the module that is currently open in the toolset.
		/// </summary>
		/// <returns>The location type of the current module, or null if no module is open.</returns>
		[OperationContract]
		NWN2Toolset.NWN2.IO.ModuleLocationType? GetCurrentModuleLocation();
		
		
		/// <summary>
		/// Creates a Neverwinter Nights 2 area
		/// in the current module.
		/// <param name="name">The name to give the area.</param>
		/// <param name="exterior">True to create an exterior area
		/// with terrain; false to create an interior area with tiles.</param>
		/// <param name="size">The size of area to create.</param>
		[OperationContract]
		[FaultContract(typeof(System.InvalidOperationException))]
		[FaultContract(typeof(System.IO.IOException))]
		void AddArea(string name, bool exterior, Size size);
		
		
		/// <summary>
		/// Gets a collection of names of every area in the 
		/// current module.
		/// </summary>
		/// <returns>A collection of area names, or null
		/// if there is no module open.</returns>
		[OperationContract]
		System.Collections.Generic.ICollection<string> GetAreas();		
		
		
		/// <summary>
		/// Adds an object to the given area.
		/// </summary>
		/// <param name="areaName">The name of the area in the current module to add the object to.</param>
		/// <param name="type">The type of object to add.</param>
		/// <param name="resref">The resref of the blueprint to create the object from.</param>
		/// <param name="tag">The tag of the object.</param>
		[OperationContract]
		[FaultContract(typeof(System.ArgumentException))]
		[FaultContract(typeof(System.ArgumentNullException))]
		[FaultContract(typeof(System.InvalidOperationException))]
		void AddObject(string areaName, NWN2Toolset.NWN2.Data.Templates.NWN2ObjectType type, string resref, string tag);
		
		
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
		[OperationContract]
		[FaultContract(typeof(System.ArgumentException))]
		[FaultContract(typeof(System.ArgumentNullException))]
		[FaultContract(typeof(System.InvalidOperationException))]
		int GetObjectCount(string areaName, NWN2Toolset.NWN2.Data.Templates.NWN2ObjectType type, string tag);	
		
		
		/// <summary>
		/// Gets a list of beans containing information about objects
		/// matching a given description in a given area.
		/// </summary>
		/// <param name="areaName">The area which has the objects.</param>
		/// <param name="type">The type of objects to collect.</param>
		/// <param name="tag">The tag that objects must have to be collected.
		/// Pass null to ignore this requirement.</param>
		/// <returns>A list of beans containing information about
		/// objects matching the description.</returns>
		[OperationContract]
		[FaultContract(typeof(System.ArgumentException))]
		[FaultContract(typeof(System.ArgumentNullException))]
		[FaultContract(typeof(System.InvalidOperationException))]
		IList<Flip.Games.NeverwinterNightsTwo.Utils.Bean> GetObjects(string areaName, 
					                                                 NWN2Toolset.NWN2.Data.Templates.NWN2ObjectType type,
					                                                 string tag);
		
		
		/// <summary>
		/// Gets a unique object in an area from its properties.
		/// </summary>
		/// <param name="areaName">The area which has the object.</param>
		/// <param name="type">The type of the object.</param>
		/// <param name="guid">The unique Guid of the object.</param>
		/// <returns>The object within this area with the given properties,
		/// or null if one could not be found.</returns>
		[OperationContract]
		[FaultContract(typeof(System.ArgumentException))]
		[FaultContract(typeof(System.ArgumentNullException))]
		[FaultContract(typeof(System.InvalidOperationException))]
		Flip.Games.NeverwinterNightsTwo.Utils.Bean GetObject(string areaName, 
		                                                     NWN2Toolset.NWN2.Data.Templates.NWN2ObjectType type, 
		                                                     Guid guid);
	}
}
