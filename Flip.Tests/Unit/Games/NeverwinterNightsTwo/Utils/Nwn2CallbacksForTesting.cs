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
 * This file added by Keiron Nicholson on 10/12/2009 at 09:45.
 */

using System;
using NWN2Toolset.NWN2.Data.Templates;
using Sussex.Flip.Games.NeverwinterNightsTwo.Utils;

namespace Sussex.Flip.Games.NeverwinterNightsTwo.Utils.Tests
{
	/// <summary>
	/// Responds to being notified about one of a selection of toolset events occurring,
	/// including objects/resources/blueprints being added or removed, resource
	/// viewers being opened or closed, a script slot on an object or resource
	/// changing, and the module changing.
	/// </summary>
	public class Nwn2CallbacksForTesting : INwn2Callbacks
	{	
		/// <summary>
		/// Notifies the client when the current module changes.
		/// </summary>
		/// <param name="newModule">The name of the new module.</param>
		/// <param name="oldModule">The name of the old module.</param>
		public void NotifyModuleChanged(string newModule, string oldModule)
		{
			Console.WriteLine("module changed... Now: " + newModule + ", Before: " + oldModule);
		}
		
		
		/// <summary>
		/// Notifies the client when a resource 
		/// is added to the module.
		/// </summary>
		/// <param name="type">The type of resource ('script', 'area' or
		/// 'conversation').</param>
		/// <param name="name">The name of the resource.</param>
		public void NotifyResourceAdded(string type, string name)
		{
			Console.WriteLine("added resource... Type: " + type + ", Name: " + name);
		}
		
		
		/// <summary>
		/// Notifies the client when a resource 
		/// is removed from the module.
		/// </summary>
		/// <param name="type">The type of resource ('script', 'area' or
		/// 'conversation').</param>
		/// <param name="name">The name of the resource.</param>
		public void NotifyResourceRemoved(string type, string name)
		{
			Console.WriteLine("removed resource... Type: " + type + ", Name: " + name);
		}
		
		
		/// <summary>
		/// Notifies the client when an object is added to an area.
		/// </summary>
		/// <param name="type">The type of object.</param>
		/// <param name="tag">The tag of the object.</param>
		/// <param name="id">The unique ID of the object.</param>
		/// <param name="area">The name of the area the object was added to.</param>
		public void NotifyObjectAdded(NWN2ObjectType type, string tag, Guid id, string area)
		{
			Console.WriteLine("added object... Type: " + type + ", Tag: " + tag + ", ID: " + id + ", Area: " + area);
		}
		
		
		/// <summary>
		/// Notifies the client when an object is removed from an area.
		/// </summary>
		/// <param name="type">The type of object.</param>
		/// <param name="tag">The tag of the object.</param>
		/// <param name="id">The unique ID of the object.</param>
		/// <param name="area">The name of the area the object was removed from.</param>
		public void NotifyObjectRemoved(NWN2ObjectType type, string tag, Guid id, string area)
		{
			Console.WriteLine("removed object... Type: " + type + ", Tag: " + tag + ", ID: " + id + ", Area: " + area);
		}
		
		
		/// <summary>
		/// Notifies the client when a blueprint is added to the module.
		/// </summary>
		/// <param name="type">The type of blueprint.</param>
		/// <param name="resourceName">The unique resource name of the blueprint.
		/// <remarks>There appears to be a bug in the toolset or plugin which causes this event
		/// to fire again at a later point.</remarks>
		public void NotifyBlueprintAdded(NWN2ObjectType type, string resourceName)
		{
			Console.WriteLine("added " + type + " blueprint with resource name " + resourceName);
		}
				

		/// <summary>
		/// Notifies the client when a blueprint is removed from the module.
		/// </summary>
		/// <param name="type">The type of blueprint.</param>
		/// <param name="resourceName">The unique resource name of the blueprint.
		/// <remarks>There appears to be a bug in the toolset or plugin which causes this event
		/// to fire again at a later point.</remarks>
		public void NotifyBlueprintRemoved(NWN2ObjectType type, string resourceName)
		{
			Console.WriteLine("removed " + type + " blueprint with resource name " + resourceName);
		}
		
		
		/// <summary>
		/// Notifies the client when a resource viewer is opened in the toolset.
		/// </summary>
		/// <param name="type">The type of resource ('script', 'area' or
		/// 'conversation') being viewed.</param>
		/// <param name="name">The name of the resource being viewed.</param>
		public void NotifyResourceViewerOpened(string type, string name)
		{
			Console.WriteLine("opened " + type + " '" + name + "'");
		}
		
		
		/// <summary>
		/// Notifies the client when a resource viewer is closed in the toolset.
		/// </summary>
		/// <param name="type">The type of resource ('script', 'area' or
		/// 'conversation') which was being viewed.</param>
		/// <param name="name">The name of the resource which was being viewed.</param>
		public void NotifyResourceViewerClosed(string name)
		{
			Console.WriteLine("closed resource '" + name + "'");
		}
		
		
		/// <summary>
		/// Notifies the client when a different script is attached to a script slot
		/// on an area.
		/// </summary>
		/// <param name="area">The name of the area.</param>
		/// <param name="slot">The name of the script slot property.</param>
		/// <param name="oldValue">The name of the previously attached script.</param>
		/// <param name="newValue">The name of the newly attached script.</param>
		public void NotifyScriptSlotChangedOnArea(string area, string slot, string oldValue, string newValue)
		{
			Console.WriteLine(slot + " script on area " + area + " changed to '" + newValue + "' (was '" + oldValue + "')");
		}
		
		
		/// <summary>
		/// Notifies the client when a different script is attached to a script slot
		/// on a module.
		/// </summary>
		/// <param name="module">The name of the module.</param>
		/// <param name="slot">The name of the script slot property.</param>
		/// <param name="oldValue">The name of the previously attached script.</param>
		/// <param name="newValue">The name of the newly attached script.</param>
		public void NotifyScriptSlotChangedOnModule(string module, string slot, string oldValue, string newValue)
		{
			Console.WriteLine(slot + " script on module " + module + " changed to '" + newValue + "' (was '" + oldValue + "')");
		}
		
		
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
		public void NotifyScriptSlotChangedOnObject(NWN2ObjectType type, Guid id, string area, string slot, string oldValue, string newValue)
		{
			Console.WriteLine(slot + " script on " + type + " with ID " + id + " changed to '" + newValue + "' (was '" + oldValue + "')");
		}
	}
}
