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
 * This file added by Keiron Nicholson on 10/12/2009 at 09:45.
 */

using System;
using NWN2Toolset.NWN2.Data.Templates;

namespace Sussex.Flip.Games.NeverwinterNightsTwo.Utils
{
	/// <summary>
	/// TODO
	/// </summary>
	public class Callbacks : ICallbacks
	{
		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="message"></param>
		public void NotifyModuleChanged(string message)
		{
			Console.WriteLine("Module changed: {0}",message);
		}
		
		
		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="message"></param>
		public void NotifyResourceAdded(string type, string name)
		{
			Console.WriteLine("added resource... Type: " + type + ", Name: " + name);
		}
		
		
		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="message"></param>
		public void NotifyResourceRemoved(string type, string name)
		{
			Console.WriteLine("removed resource... Type: " + type + ", Name: " + name);
		}
		
		
		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="message"></param>
		public void NotifyObjectAdded(NWN2ObjectType type, string tag, Guid id, string area)
		{
			Console.WriteLine("added object... Type: " + type + ", Tag: " + tag + ", ID: " + id + ", Area: " + area);
		}
		
		
		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="message"></param>
		public void NotifyObjectRemoved(NWN2ObjectType type, string tag, Guid id, string area)
		{
			Console.WriteLine("removed object... Type: " + type + ", Tag: " + tag + ", ID: " + id + ", Area: " + area);
		}
		
		
		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="message"></param>
		public void NotifyBlueprintAdded(NWN2ObjectType type, string resref)
		{
			Console.WriteLine("added " + type + " blueprint with resref " + resref);
		}
		
		
		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="message"></param>
		public void NotifyBlueprintRemoved(NWN2ObjectType type, string resref)
		{
			Console.WriteLine("removed " + type + " blueprint with resref " + resref);
		}
		
		
		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="message"></param>
		public void NotifyResourceViewerOpened(string type, string name)
		{
			Console.WriteLine("opened " + type + " '" + name + "'");
		}
		
		
		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="message"></param>
		public void NotifyResourceViewerClosed(string name)
		{
			Console.WriteLine("closed resource '" + name + "'");
		}
		
		
		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="message"></param>
		public void NotifyScriptSlotChangedOnArea(string area, string slot, string oldValue, string newValue)
		{
			Console.WriteLine(slot + " script on area " + area + " changed to '" + newValue + "' (was '" + oldValue + "')");
		}
		
		
		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="message"></param>
		public void NotifyScriptSlotChangedOnModule(string module, string slot, string oldValue, string newValue)
		{
			Console.WriteLine(slot + " script on module " + module + " changed to '" + newValue + "' (was '" + oldValue + "')");
		}
		
		
		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="message"></param>
		public void NotifyScriptSlotChangedOnObject(NWN2ObjectType type, Guid id, string area, string slot, string oldValue, string newValue)
		{
			Console.WriteLine(slot + " script on " + type + " with ID " + id + " changed to '" + newValue + "' (was '" + oldValue + "')");
		}
	}
}
