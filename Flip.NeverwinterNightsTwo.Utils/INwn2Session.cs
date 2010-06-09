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
 * This file added by Keiron Nicholson on 23/09/2009 at 12:57.
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.ServiceModel;
using NWN2Toolset;
using NWN2Toolset.NWN2.IO;
using NWN2Toolset.NWN2.Data;
using NWN2Toolset.NWN2.Data.Blueprints;
using NWN2Toolset.NWN2.Data.ConversationData;
using NWN2Toolset.NWN2.Data.Instances;
using NWN2Toolset.NWN2.Data.Templates;
using NWN2Toolset.NWN2.Data.TypedCollections;
using OEIShared.IO;
using OEIShared.Utils;

namespace Sussex.Flip.Games.NeverwinterNightsTwo.Utils
{
	/// <summary>
	/// A facade for a Neverwinter Nights 2 toolset session.
	/// </summary>
	public interface INwn2Session
	{		
		/// <summary>
		/// Creates a Neverwinter Nights 2 game module.
		/// </summary>
		/// <param name="path">The path to create the module at. If 'location'
		/// is set to ModuleLocationType.Directory, this must be the path for
		/// a folder to be created within NWN2Toolset.NWN2ToolsetMainForm.ModulesDirectory.</param>
		/// <param name="location">The serialisation form of the module.</param>
		void CreateModule(string path, ModuleLocationType location);
									
		
		/// <summary>
		/// Creates and opens a Neverwinter Nights 2 game module of location type Temporary.
		/// </summary>
		/// <returns>The path the module was created at.</returns>
		string CreateAndOpenTemporaryModule();
			
				
		/// <summary>
		/// Opens a Neverwinter Nights 2 game module.
		/// </summary>
		/// <param name="name">The path of the module to open.</param>
		/// <param name="location">The serialisation form of the module.</param>
		void OpenModule(string path, ModuleLocationType location);
		
		
		/// <summary>
		/// Saves a Neverwinter Nights 2 game module to its
		/// current location.
		/// </summary>
		/// <param name="module">The module to save.</param>.
		void SaveModule(NWN2GameModule module);
		
		
		/// <summary>
		/// Saves a Neverwinter Nights 2 game module to a given path.
		/// </summary>
		/// <param name="module">The module to save.</param>
		/// <param name="path">The path to save the module to.</param>
		void SaveModule(NWN2GameModule module, string path);
		
				
		/// <summary>
		/// Closes the current module.
		/// </summary>
		void CloseModule();
		
		
		/// <summary>
		/// Gets the module that is currently open in the toolset.
		/// </summary>
		/// <returns>The current module, or null if no module is open.</returns>
		NWN2GameModule GetModule();
		
		
		/// <summary>
		/// Gets the absolute path of the module that is currently open in the toolset.
		/// </summary>
		/// <returns>The absolute path of the current module, or null if no module is open.</returns>
		string GetModulePath();
		
		
		/// <summary>
		/// Gets the path to the working ('temp') copy of the module that is currently open in the toolset.
		/// </summary>
		/// <returns>The temp path of the current module, or null if no module is open.</returns>
		string GetModuleTempPath();
		
		
		/// <summary>
		/// Gets the absolute path of a given module.
		/// </summary>
		/// <param name="module">The module to return the path of.</param>
		/// <returns>The absolute path of the given module.</returns>
		string GetModulePath(NWN2GameModule module);
		
		
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
		AreaBase AddArea(string name, bool exterior, Size size);
				
		
		/// <summary>
		/// Constructs a new <see cref="AreaBase"/> instance.
		/// </summary>
		/// <param name="nwn2area">The Neverwinter Nights 2 area
		/// the <see cref="AreaBase"/> facade will wrap.</param>
		/// <returns>A new <see cref="AreaBase"/> instance.</returns>
		/// <remarks>This is a Factory Method.</remarks>
		AreaBase CreateAreaBase(NWN2GameArea nwn2area);
		
		
		/// <summary>
		/// Checks whether the current module has a compiled script
		/// of the given name.
		/// </summary>
		/// <param name="name">The name of the script.</param>
		/// <returns>True if the current module has a .NCS compiled
		/// script file of the given name, and false otherwise.</returns>
		bool HasCompiled(string name);
		

		/// <summary>
		/// Checks whether the current module has an uncompiled script
		/// of the given name.
		/// </summary>
		/// <param name="name">The name of the script.</param>
		/// <returns>True if the current module has a .NSS uncompiled
		/// script file of the given name, and false otherwise.</returns>
		bool HasUncompiled(string name);
		
				
		/// <summary>
		/// Adds an object to the given area.
		/// </summary>
		/// <param name="area">The area in the current module to add the object to.</param>
		/// <param name="type">The type of object to add.</param>
		/// <param name="resref">The resref of the blueprint to create the object from.</param>
		/// <param name="tag">The tag of the object.</param>
		/// <returns>Returns the newly created object.</returns>
		INWN2Instance AddObject(NWN2GameArea area, NWN2ObjectType type, string resref, string tag);
		
		
		/// <summary>
		/// Gets a blueprint with the given resref and type,
		/// if one exists.
		/// </summary>
		/// <param name="resRef">The resref value of the blueprint.</param>
		/// <param name="type">The object type of the blueprint.</param>
		/// <returns>A blueprint, or null if no blueprint
		/// matching the given criteria was found.</returns>
		INWN2Blueprint GetBlueprint(string resRef, NWN2ObjectType type);
		
		
		/// <summary>
		/// Gets all blueprints of a given type.
		/// </summary>
		/// <param name="type">The object type of the
		/// blueprints to return.</param>
		/// <returns>A collection of blueprints.</returns>
		NWN2BlueprintCollection GetBlueprints(NWN2ObjectType type);
		
		
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
		INWN2Instance GetObject(NWN2GameArea area, NWN2ObjectType type, Guid id);
		
		
		/// <summary>
		/// Gets all objects of a given type in a given area.
		/// </summary>
		/// <param name="area">The area which has the objects.</param>
		/// <param name="type">The type of objects to collect.</param>
		/// <returns>A collection of objects.</returns>
		/// <remarks>This method will throw an InvalidOperationException if
		/// the area is not open.</remarks>
		NWN2InstanceCollection GetObjects(NWN2GameArea area, NWN2ObjectType type);		
		
		
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
		NWN2InstanceCollection GetObjectsByTag(NWN2GameArea area, NWN2ObjectType type, string tag);
		
		
		/// <summary>
		/// Gets all objects matching a particular address.
		/// </summary>
		/// <param name="address">The address specifying an instance or instances.</param>
		/// <returns>A collection of objects.</returns>
		/// <remarks>TODO update this header</remarks>
		NWN2InstanceCollection GetObjectsByAddressInArea(Nwn2Address address, string areaTag);
			
		
		/// <summary>
		/// Gets an area in the current module.
		/// </summary>
		/// <param name="name">The name of the area.</param>
		/// <returns>The named area, or null 
		/// if the area could not be found.</returns>
		NWN2GameArea GetArea(string name);
		
		
		/// <summary>
		/// Gets a conversation in the current module.
		/// </summary>
		/// <param name="name">The name of the conversation.</param>
		/// <returns>The named conversation, or null 
		/// if the conversation could not be found.</returns>
		NWN2GameConversation GetConversation(string name);
		
		
		/// <summary>
		/// Gets a line of dialogue in the given conversation.
		/// </summary>
		/// <param name="conversation">The conversation which has the line.</param>
		/// <param name="lineID">The unique ID of the desired line of dialogue.</param>
		/// <returns>The desired line of dialogue, or null 
		/// if the line could not be found.</returns>
		NWN2ConversationLine GetConversationLine(NWN2GameConversation conversation, Guid lineID);
		
		
		/// <summary>
		/// Gets the scripts in the current module.
		/// </summary>
		/// <returns>A list of scripts.</returns>
		NWN2GameScriptDictionary GetScripts();	
		
		
		/// <summary>
		/// Gets the script with a given name in the current module.
		/// </summary>
		/// <returns>The script with the given name,
		/// or null if no such script exists.</returns>
		NWN2GameScript GetScript(string name);	
		
		
		/// <summary>
		/// Gets the compiled scripts in the current module.
		/// </summary>
		/// <returns>A list of compiled scripts.</returns>
		OEIGenericCollectionWithEvents<IResourceEntry> GetCompiledScripts();	
		
		
		/// <summary>
		/// Gets the compiled script with a given name in the current module.
		/// </summary>
		/// <returns>The compiled script with the given name,
		/// or null if no such script exists.</returns>
		IResourceEntry GetCompiledScript(string name);	
				

		/// <summary>
		/// Adds an uncompiled script to the current module.
		/// </summary>
		/// <param name="name">The name to save the script under.</param>
		/// <param name="code">The contents of the script.</param>
		/// <returns>The newly created script.</returns>
		NWN2GameScript AddScript(string name, string code);	
		
		
		/// <summary>
		/// Deletes a script from the current module.
		/// </summary>
		/// <param name="name">The name of the script.</param>
		/// <remarks>Both compiled and uncompiled copies
		/// of the script are deleted.</remarks>
		void DeleteScript(string name);
				

		/// <summary>
		/// Compiles a script in the current module.
		/// </summary>
		/// <param name="script">The script to compile.</param>
		void CompileScript(NWN2GameScript script);			
		
		
		/// <summary>
		/// Attaches a script to a named script slot on a given instance.
		/// </summary>
		/// <param name="script">The script to attach.</param>
		/// <param name="instance">The instance to attach the script to.</param>
		/// <param name="slot">The script slot to attach the script to.</param>
		void AttachScriptToObject(NWN2GameScript script, INWN2Instance instance, string slot);
				
		
		/// <summary>
		/// Attaches a script to a named script slot on a given area.
		/// </summary>
		/// <param name="script">The script to attach.</param>
		/// <param name="area">The area to attach the script to.</param>
		/// <param name="slot">The script slot to attach the script to.</param>
		void AttachScriptToArea(NWN2GameScript script, NWN2GameArea area, string slot);
				
		
		/// <summary>
		/// Attaches a script to a named script slot on a given module.
		/// </summary>
		/// <param name="script">The script to attach.</param>
		/// <param name="slot">The script slot to attach the script to.</param>
		void AttachScriptToModule(NWN2GameScript script, string slot);
			
		
		/// <summary>
		/// Attaches a script to a particular line of dialogue.
		/// </summary>
		/// <param name="script">The script to attach.</param>
		/// <param name="line">The line of dialogue to attach the script to.</param>
		void AttachScriptToConversation(NWN2GameScript script, NWN2ConversationLine line);
			
			
		/// <summary>
		/// Clears the value of a named script slot on a given instance.
		/// </summary>
		/// <param name="instance">The object which owns the script slot to be cleared.</param>
		/// <param name="slot">The script slot to clear
		/// any scripts from.</param>
		void ClearScriptSlotOnObject(INWN2Instance instance, string slot);
		
		
		/// <summary>
		/// Clears the value of a named script slot on a given area.
		/// </summary>
		/// <param name="area">The area which owns the script slot to be cleared.</param>
		/// <param name="slot">The script slot to clear
		/// any scripts from.</param>
		void ClearScriptSlotOnArea(NWN2GameArea area, string slot);
		
		
		/// <summary>
		/// Clears the value of a named script slot on the current module.
		/// </summary>
		/// <param name="slot">The script slot to clear
		/// any scripts from.</param>
		void ClearScriptSlotOnModule(string slot);
		
		
		/// <summary>
		/// Opens an area in an area viewer.
		/// </summary>
		/// <param name="area">The area to open.</param>
		void OpenArea(NWN2GameArea area);
		
		
		/// <summary>
		/// Closes the area viewer for an area, if one is
		/// currently open.
		/// </summary>
		/// <param name="area">The area to close.</param>
		void CloseArea(NWN2GameArea area);
		
		
		/// <summary>
		/// Checks whether an area viewer is currently open
		/// for a particular area.
		/// </summary>
		/// <param name="area">The area which may have a viewer open.</param>
		/// <returns>True if an area viewer for the given
		/// area is currently open in the toolset; false
		/// otherwise.</returns>
		bool AreaIsOpen(NWN2GameArea area);
		
		
		/// <summary>
		/// Gets a list of scripts
		/// which are currently open in script viewers.
		/// </summary>
		/// <returns>A list of open scripts.</returns>
		IList<NWN2GameScript> GetOpenScripts();
		
		
		/// <summary>
		/// Opens a script in a script viewer.
		/// </summary>
		/// <param name="script">The script to open.</param>
		void OpenScript(NWN2GameScript script);
		
		
		/// <summary>
		/// Closes the script viewer for a script, if one is
		/// currently open.
		/// </summary>
		/// <param name="script">The script to close.</param>
		void CloseScript(NWN2GameScript script);
		
		
		/// <summary>
		/// Checks whether a script viewer is currently open
		/// for a particular script.
		/// </summary>
		/// <param name="script">The script which may have a viewer open.</param>
		/// <returns>True if a script viewer for the given
		/// script is currently open in the toolset; false
		/// otherwise.</returns>
		bool ScriptIsOpen(NWN2GameScript script);
	}
}
