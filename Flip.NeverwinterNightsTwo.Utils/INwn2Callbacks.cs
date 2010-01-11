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
 * This file added by Keiron Nicholson on 10/12/2009 at 09:44.
 */

using System;
using System.ServiceModel;

namespace Sussex.Flip.Games.NeverwinterNightsTwo.Utils
{
	/// <summary>
	/// Callbacks for notifying a client when one of a selection of toolset events occurs,
	/// including objects/resources/blueprints being added or removed, resource
	/// viewers being opened or closed, a script slot on an object or resource
	/// changing, and the module changing.
	/// </summary>
	public interface INwn2Callbacks
	{
		/// <summary>
		/// Notifies the client when the current module changes.
		/// </summary>
		/// <param name="newModule">The name of the new module.</param>
		/// <param name="oldModule">The name of the old module.</param>
		[OperationContract(IsOneWay = true)]
		void NotifyModuleChanged(string newModule, string oldModule);
		
		
		/// <summary>
		/// Notifies the client when a resource 
		/// is added to the module.
		/// </summary>
		/// <param name="type">The type of resource ('script', 'area' or
		/// 'conversation').</param>
		/// <param name="name">The name of the resource.</param>
		[OperationContract(IsOneWay = true)]
		void NotifyResourceAdded(string type, string name);
		
		
		/// <summary>
		/// Notifies the client when a resource 
		/// is removed from the module.
		/// </summary>
		/// <param name="type">The type of resource ('script', 'area' or
		/// 'conversation').</param>
		/// <param name="name">The name of the resource.</param>
		[OperationContract(IsOneWay = true)]
		void NotifyResourceRemoved(string type, string name);
		
		
		/// <summary>
		/// Notifies the client when an object is added to an area.
		/// </summary>
		/// <param name="type">The type of object.</param>
		/// <param name="tag">The tag of the object.</param>
		/// <param name="id">The unique ID of the object.</param>
		/// <param name="area">The name of the area the object was added to.</param>
		[OperationContract(IsOneWay = true)]
		void NotifyObjectAdded(NWN2Toolset.NWN2.Data.Templates.NWN2ObjectType type, string tag, Guid id, string area);
		
		
		/// <summary>
		/// Notifies the client when an object is removed from an area.
		/// </summary>
		/// <param name="type">The type of object.</param>
		/// <param name="tag">The tag of the object.</param>
		/// <param name="id">The unique ID of the object.</param>
		/// <param name="area">The name of the area the object was removed from.</param>
		[OperationContract(IsOneWay = true)]
		void NotifyObjectRemoved(NWN2Toolset.NWN2.Data.Templates.NWN2ObjectType type, string tag, Guid id, string area);
				
		
		/// <summary>
		/// Notifies the client when a blueprint is added to the module.
		/// </summary>
		/// <param name="type">The type of blueprint.</param>
		/// <param name="resourceName">The unique resource name of the blueprint.
		/// <remarks>There appears to be a bug in the toolset or plugin which causes this event
		/// to fire again at a later point.</remarks>
		[OperationContract(IsOneWay = true)]
		void NotifyBlueprintAdded(NWN2Toolset.NWN2.Data.Templates.NWN2ObjectType type, string resourceName);
				

		/// <summary>
		/// Notifies the client when a blueprint is removed from the module.
		/// </summary>
		/// <param name="type">The type of blueprint.</param>
		/// <param name="resourceName">The unique resource name of the blueprint.
		/// <remarks>There appears to be a bug in the toolset or plugin which causes this event
		/// to fire again at a later point.</remarks>
		[OperationContract(IsOneWay = true)]
		void NotifyBlueprintRemoved(NWN2Toolset.NWN2.Data.Templates.NWN2ObjectType type, string resourceName);
		
		
		/// <summary>
		/// Notifies the client when a resource viewer is opened in the toolset.
		/// </summary>
		/// <param name="type">The type of resource ('script', 'area' or
		/// 'conversation') being viewed.</param>
		/// <param name="name">The name of the resource being viewed.</param>
		[OperationContract(IsOneWay = true)]
		void NotifyResourceViewerOpened(string type, string name);
		
		
		/// <summary>
		/// Notifies the client when a resource viewer is closed in the toolset.
		/// </summary>
		/// <param name="type">The type of resource ('script', 'area' or
		/// 'conversation') which was being viewed.</param>
		/// <param name="name">The name of the resource which was being viewed.</param>
		[OperationContract(IsOneWay = true)]
		void NotifyResourceViewerClosed(string name);
		
		
		/// <summary>
		/// Notifies the client when a different script is attached to a script slot
		/// on an area.
		/// </summary>
		/// <param name="area">The name of the area.</param>
		/// <param name="slot">The name of the script slot property.</param>
		/// <param name="oldValue">The name of the previously attached script.</param>
		/// <param name="newValue">The name of the newly attached script.</param>
		[OperationContract(IsOneWay = true)]
		void NotifyScriptSlotChangedOnArea(string area, string slot, string oldValue, string newValue);
		
		
		/// <summary>
		/// Notifies the client when a different script is attached to a script slot
		/// on a module.
		/// </summary>
		/// <param name="module">The name of the module.</param>
		/// <param name="slot">The name of the script slot property.</param>
		/// <param name="oldValue">The name of the previously attached script.</param>
		/// <param name="newValue">The name of the newly attached script.</param>
		[OperationContract(IsOneWay = true)]
		void NotifyScriptSlotChangedOnModule(string module, string slot, string oldValue, string newValue);
		
		
		/// <summary>
		/// Notifies the client when a different script is attached to a script slot
		/// on a game object.
		/// </summary>
		/// <param name="type">The type of the object.</param>
		/// <param name="id">The unique ID of the object.</param>
		/// <param name="area">The name of the area in which this object resides.</param>
		/// <param name="slot">The name of the script slot property.</param>
		/// <param name="oldValue">The name of the previously attached script.</param>
		/// <param name="newValue">The name of the newly attached script.</param>
		[OperationContract(IsOneWay = true)]
		void NotifyScriptSlotChangedOnObject(NWN2Toolset.NWN2.Data.Templates.NWN2ObjectType type,
		                                     Guid id,		                                     
		                                     string area,
		                                     string slot,
		                                     string oldValue,
		                                     string newValue);
	}
}
