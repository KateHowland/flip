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
		string GetModuleName();
		
		
		/// <summary>
		/// Gets the absolute path of the module that is currently open in the toolset.
		/// </summary>
		/// <returns>The absolute path of the current module, or null if no module is open.</returns>
		[OperationContract]
		string GetModulePath();
		
		
		/// <summary>
		/// Gets the path to the working ('temp') copy of the module that is currently open in the toolset.
		/// </summary>
		/// <returns>The temp path of the current module, or null if no module is open.</returns>
		[OperationContract]		
		[FaultContract(typeof(System.ApplicationException))]
		string GetModuleTempPath();
		
		
		/// <summary>
		/// Gets the name of the currently viewed area, if one exists.
		/// </summary>
		/// <returns>The name of the currently viewed area, or null if no area is being viewed (or
		/// if no module is open).</returns>		
		[OperationContract]
		string GetCurrentArea();
		
		
		/// <summary>
		/// Gets the location type of the module that is currently open in the toolset.
		/// </summary>
		/// <returns>The location type of the current module, or null if no module is open.</returns>
		[OperationContract]
		NWN2Toolset.NWN2.IO.ModuleLocationType? GetModuleLocation();
		
		
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
		[OperationContract]
		[FaultContract(typeof(System.InvalidOperationException))]
		[FaultContract(typeof(System.IO.IOException))]
		void AddArea(string name, bool exterior, Size size);
		
		
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
		IList<Flip.Utils.Bean> GetObjects(string areaName, 
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
		Flip.Utils.Bean GetObject(string areaName, 
		                          NWN2Toolset.NWN2.Data.Templates.NWN2ObjectType type, 
		                          Guid guid);		
		
		
		/// <summary>
		/// Gets a bean representing an area in the current module.
		/// </summary>
		/// <param name="name">The name of the area.</param>
		/// <returns>A bean representing the named area, or null 
		/// if the area could not be found.</returns>
		[OperationContract]
		[FaultContract(typeof(System.ArgumentException))]
		[FaultContract(typeof(System.ArgumentNullException))]
		[FaultContract(typeof(System.InvalidOperationException))]
		Flip.Utils.Bean GetArea(string name);
		
		
		/// <summary>
		/// Gets a list of beans representing 
		/// the areas in the current module.
		/// </summary>
		/// <returns>A list of beans representing all of the
		/// areas owned by the current module.</returns>
		[OperationContract]
		[FaultContract(typeof(System.InvalidOperationException))]
		IList<Flip.Utils.Bean> GetAreas();	
		
		
		/// <summary>
		/// Gets a bean representing the current module.
		/// </summary>
		/// <returns>A bean representing the current module.</returns>
		[OperationContract]
		[FaultContract(typeof(System.InvalidOperationException))]
		Flip.Utils.Bean GetModule();
		
		
		/// <summary>
		/// Gets a list of beans representing all of the
		/// uncompiled scripts owned by the current module.
		/// </summary>
		/// <returns>A list of beans representing all of the
		/// uncompiled scripts owned by the current module.</returns>
		[OperationContract]
		[FaultContract(typeof(System.InvalidOperationException))]
		IList<Flip.Utils.Bean> GetUncompiledScripts();	
		
		
		/// <summary>
		/// Gets a bean representing an uncompiled
		/// script in the current module.
		/// </summary>
		/// <returns>A bean representing an uncompiled
		/// script in the current module, or null if no
		/// such script exists.</returns>
		[OperationContract]
		[FaultContract(typeof(System.ArgumentException))]
		[FaultContract(typeof(System.ArgumentNullException))]
		[FaultContract(typeof(System.InvalidOperationException))]
		Flip.Utils.Bean GetUncompiledScript(string name);	
		
		
		/// <summary>
		/// Gets a list of beans representing all of the
		/// compiled scripts owned by the current module.
		/// </summary>
		/// <returns>A list of beans representing all of the
		/// compiled scripts owned by the current module.</returns>
		[OperationContract]
		[FaultContract(typeof(System.ApplicationException))]
		[FaultContract(typeof(System.InvalidOperationException))]
		IList<Flip.Utils.Bean> GetCompiledScripts();	
		
		
		/// <summary>
		/// Gets a bean representing an compiled
		/// script in the current module.
		/// </summary>
		/// <returns>A bean representing an compiled
		/// script in the current module, or null if no
		/// such script exists.</returns>
		[OperationContract]
		[FaultContract(typeof(System.ApplicationException))]
		[FaultContract(typeof(System.ArgumentException))]
		[FaultContract(typeof(System.ArgumentNullException))]
		[FaultContract(typeof(System.InvalidOperationException))]
		Flip.Utils.Bean GetCompiledScript(string name);	
				

		/// <summary>
		/// Checks whether the current module has a compiled script
		/// of the given name.
		/// </summary>
		/// <param name="name">The name of the script.</param>
		/// <returns>True if the current module has a .NCS compiled
		/// script file of the given name, and false otherwise.</returns>
		[OperationContract]
		[FaultContract(typeof(System.ArgumentNullException))]
		[FaultContract(typeof(System.ArgumentException))]
		[FaultContract(typeof(System.InvalidOperationException))]
		bool HasCompiled(string name);		
		

		/// <summary>
		/// Checks whether the current module has an uncompiled script
		/// of the given name.
		/// </summary>
		/// <param name="name">The name of the script.</param>
		/// <returns>True if the current module has a .NSS uncompiled
		/// script file of the given name, and false otherwise.</returns>
		[OperationContract]
		[FaultContract(typeof(System.ArgumentNullException))]
		[FaultContract(typeof(System.ArgumentException))]
		[FaultContract(typeof(System.InvalidOperationException))]
		bool HasUncompiled(string name);	
				

		/// <summary>
		/// Adds an uncompiled script to the current module.
		/// </summary>
		/// <param name="name">The name to save the script under.</param>
		/// <param name="code">The contents of the script.</param>
		[OperationContract]
		[FaultContract(typeof(System.ArgumentException))]
		[FaultContract(typeof(System.ArgumentNullException))]
		[FaultContract(typeof(System.InvalidOperationException))]
		void AddScript(string name, string code);	
		
		
		/// <summary>
		/// Deletes a script from the current module.
		/// </summary>
		/// <param name="name">The name of the script.</param>
		/// <remarks>Both compiled and uncompiled copies
		/// of the script are deleted.</remarks>
		[OperationContract]
		[FaultContract(typeof(ArgumentException))]
		[FaultContract(typeof(ArgumentNullException))]
		[FaultContract(typeof(InvalidOperationException))]
		[FaultContract(typeof(System.IO.IOException))]
		void DeleteScript(string name);
				

		/// <summary>
		/// Compiles a script in the current module.
		/// </summary>
		/// <param name="name">The name of the script to compile.</param>
		[OperationContract]
		[FaultContract(typeof(System.ApplicationException))]
		[FaultContract(typeof(System.ArgumentException))]
		[FaultContract(typeof(System.ArgumentNullException))]
		[FaultContract(typeof(System.InvalidOperationException))]
		[FaultContract(typeof(System.IO.InvalidDataException))]
		void CompileScript(string name);	
		
				
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
		[OperationContract]
		[FaultContract(typeof(System.ArgumentNullException))]
		[FaultContract(typeof(System.ArgumentException))]
		[FaultContract(typeof(System.InvalidOperationException))]
		[FaultContract(typeof(System.IO.InvalidDataException))]
		void AttachScriptToObject(string scriptName, 
				                  string areaName, 
				                  Nwn2EventRaiser type, 
				                  Guid objectID, 
				                  string scriptSlot);
		
				
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
		[OperationContract]
		[FaultContract(typeof(System.ArgumentNullException))]
		[FaultContract(typeof(System.ArgumentException))]
		[FaultContract(typeof(System.InvalidOperationException))]
		[FaultContract(typeof(System.IO.InvalidDataException))]
		void AttachScriptToArea(string scriptName, string areaName, string scriptSlot);
		
				
		/// <summary>
		/// Finds the compiled script with the given name which 
		/// is already present in the module's script 
		/// collection, and attaches it to the nominated
		/// script slot on the module.
		/// </summary>
		/// <param name="scriptName">The name of the compiled script.</param>
		/// <param name="scriptSlot">The script slot to attach
		/// the script to.</param>
		[OperationContract]
		[FaultContract(typeof(System.ArgumentNullException))]
		[FaultContract(typeof(System.ArgumentException))]
		[FaultContract(typeof(System.InvalidOperationException))]
		[FaultContract(typeof(System.IO.InvalidDataException))]
		void AttachScriptToModule(string scriptName, string scriptSlot);
		
		
		/// <summary>
		/// Detaches the script from the given script slot on the given
		/// object, if a script was attached.
		/// </summary>
		/// <param name="areaName">The area containing the object.</param>
		/// <param name="objectID">The unique ObjectID of the 
		/// object with the given script slot.</param>
		/// <param name="type">The type of the receiving object.</param>
		/// <param name="scriptSlot">The script slot to remove
		/// any scripts from.</param>
		[OperationContract]
		[FaultContract(typeof(System.ArgumentNullException))]
		[FaultContract(typeof(System.ArgumentException))]
		[FaultContract(typeof(System.InvalidOperationException))]
		void ClearScriptSlotOnObject(string areaName, Guid objectID, Nwn2EventRaiser type, string scriptSlot);
		
		
		/// <summary>
		/// Detaches the script from the given script slot on the given
		/// area, if a script was attached.
		/// </summary>
		/// <param name="areaName">The area with the given script slot.</param>
		/// <param name="scriptSlot">The script slot to remove
		/// any scripts from.</param>
		[OperationContract]
		[FaultContract(typeof(System.ArgumentNullException))]
		[FaultContract(typeof(System.ArgumentException))]
		[FaultContract(typeof(System.InvalidOperationException))]
		void ClearScriptSlotOnArea(string areaName, string scriptSlot);
		
		
		/// <summary>
		/// Detaches the script from the given script slot on the current
		/// module, if a script was attached.
		/// </summary>
		/// <param name="scriptSlot">The script slot to remove
		/// any scripts from.</param>
		[OperationContract]
		[FaultContract(typeof(System.ArgumentNullException))]
		[FaultContract(typeof(System.ArgumentException))]
		[FaultContract(typeof(System.InvalidOperationException))]
		void ClearScriptSlotOnModule(string scriptSlot);
		
		
		/// <summary>
		/// Gets a list containing the names of all areas
		/// which are open in area viewers in the current module.
		/// </summary>
		/// <returns>A list of names of open areas.</returns>
		[OperationContract]
		[FaultContract(typeof(System.InvalidOperationException))]
		IList<string> GetOpenAreas();
		
		
		/// <summary>
		/// Opens an area in an area viewer.
		/// </summary>
		/// <param name="name">The name of the area to open.</param>
		[OperationContract]
		[FaultContract(typeof(System.ArgumentNullException))]
		[FaultContract(typeof(System.ArgumentException))]
		[FaultContract(typeof(System.InvalidOperationException))]
		void OpenArea(string name);
		
		
		/// <summary>
		/// Closes the area viewer for an area, if one is
		/// currently open.
		/// </summary>
		/// <param name="name">The name of the area to close.</param>
		[OperationContract]
		[FaultContract(typeof(System.ArgumentNullException))]
		[FaultContract(typeof(System.ArgumentException))]
		[FaultContract(typeof(System.InvalidOperationException))]
		void CloseArea(string name);
		
		
		/// <summary>
		/// Checks whether an area viewer is currently open
		/// for a particular area.
		/// </summary>
		/// <param name="name">The name of the area.</param>
		/// <returns>True if an area viewer for the named
		/// area is currently open in the toolset; false
		/// otherwise.</returns>
		[OperationContract]
		[FaultContract(typeof(System.ArgumentNullException))]
		[FaultContract(typeof(System.ArgumentException))]
		[FaultContract(typeof(System.InvalidOperationException))]
		bool AreaIsOpen(string name);
		
		
		/// <summary>
		/// Gets a list containing the names of all scripts
		/// which are open in script viewers in the current module.
		/// </summary>
		/// <returns>A list of names of open scripts.</returns>
		[OperationContract]
		[FaultContract(typeof(System.InvalidOperationException))]
		IList<string> GetOpenScripts();
		
		
		/// <summary>
		/// Opens a script in a script viewer.
		/// </summary>
		/// <param name="name">The name of the script to open.</param>
		[OperationContract]
		[FaultContract(typeof(System.ArgumentNullException))]
		[FaultContract(typeof(System.ArgumentException))]
		[FaultContract(typeof(System.InvalidOperationException))]
		void OpenScript(string name);
		
		
		/// <summary>
		/// Closes the script viewer for a script, if one is
		/// currently open.
		/// </summary>
		/// <param name="name">The name of the script to close.</param>
		[OperationContract]
		[FaultContract(typeof(System.ArgumentNullException))]
		[FaultContract(typeof(System.ArgumentException))]
		[FaultContract(typeof(System.InvalidOperationException))]
		void CloseScript(string name);
		
		
		/// <summary>
		/// Checks whether a script viewer is currently open
		/// for a particular script.
		/// </summary>
		/// <param name="name">The name of the script.</param>
		/// <returns>True if a script viewer for the named
		/// script is currently open in the toolset; false
		/// otherwise.</returns>
		[OperationContract]
		[FaultContract(typeof(System.ArgumentNullException))]
		[FaultContract(typeof(System.ArgumentException))]
		[FaultContract(typeof(System.InvalidOperationException))]
		bool ScriptIsOpen(string name);
		
		
		/// <summary>
		/// Loads a script resource from disk, ensuring that
		/// the script object is fully loaded (but overwriting
		/// any unsaved changes that were made).
		/// </summary>
		/// <param name="name">The name of the script.</param>
		[OperationContract]
		[FaultContract(typeof(System.ArgumentNullException))]
		[FaultContract(typeof(System.ArgumentException))]
		[FaultContract(typeof(System.InvalidOperationException))]
		void DemandScript(string name);
		
		
		/// <summary>
		/// Releases a script resource.
		/// </summary>
		/// <param name="name">The name of the script.</param>
		[OperationContract]
		[FaultContract(typeof(System.ArgumentNullException))]
		[FaultContract(typeof(System.ArgumentException))]
		[FaultContract(typeof(System.InvalidOperationException))]
		void ReleaseScript(string name);
		
		
		/// <summary>
		/// Loads an area from disk, ensuring that
		/// the area object is fully loaded (but overwriting
		/// any unsaved changes that were made).
		/// </summary>
		/// <param name="name">The name of the area.</param>
		[OperationContract]
		[FaultContract(typeof(System.ArgumentNullException))]
		[FaultContract(typeof(System.ArgumentException))]
		[FaultContract(typeof(System.InvalidOperationException))]
		void DemandArea(string name);
		
		
		/// <summary>
		/// Releases an area resource.
		/// </summary>
		/// <param name="name">The name of the area.</param>
		[OperationContract]
		[FaultContract(typeof(System.ArgumentNullException))]
		[FaultContract(typeof(System.ArgumentException))]
		[FaultContract(typeof(System.InvalidOperationException))]
		void ReleaseArea(string name);
	}
}
