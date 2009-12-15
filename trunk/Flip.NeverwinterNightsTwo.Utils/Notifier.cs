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
 * This file added by Keiron Nicholson on 10/12/2009 at 11:44.
 */

using System;
using System.Collections.Generic;
using System.ServiceModel;
using NWN2Toolset.NWN2.Data.Templates;

namespace Sussex.Flip.Games.NeverwinterNightsTwo.Utils
{
	/// <summary>
	/// TODO
	/// </summary>
	public class Notifier : ICallbacks
	{
		ICallbacks context;
		public Notifier()
		{
			context = OperationContext.Current.GetCallbackChannel<ICallbacks>();
		}
		
		
		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="message"></param>
		public void NotifyModuleChanged(string message)
		{
			context.NotifyModuleChanged(message);
		}
		
		
		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="message"></param>
		public void NotifyResourceAdded(string type, string name)
		{
			context.NotifyResourceAdded(type,name);
		}
		
		
		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="message"></param>
		public void NotifyResourceRemoved(string type, string name)
		{
			context.NotifyResourceRemoved(type,name);
		}
		
		
		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="message"></param>
		public void NotifyObjectAdded(NWN2Toolset.NWN2.Data.Templates.NWN2ObjectType type, string tag, Guid id, string area)
		{
			context.NotifyObjectAdded(type,tag,id,area);
		}
		
		
		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="message"></param>
		public void NotifyObjectRemoved(NWN2Toolset.NWN2.Data.Templates.NWN2ObjectType type, string tag, Guid id, string area)
		{
			context.NotifyObjectRemoved(type,tag,id,area);
		}
		
		
		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="message"></param>
		public void NotifyBlueprintAdded(NWN2ObjectType type, string resref)
		{
			context.NotifyBlueprintAdded(type,resref);
		}
		
		
		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="message"></param>
		public void NotifyBlueprintRemoved(NWN2ObjectType type, string resref)
		{
			context.NotifyBlueprintRemoved(type,resref);
		}
		
		
		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="message"></param>
		public void NotifyResourceViewerOpened(string type, string name)
		{
			context.NotifyResourceViewerOpened(type,name);
		}
		
		
		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="message"></param>
		public void NotifyResourceViewerClosed(string name)
		{
			context.NotifyResourceViewerClosed(name);
		}
		
		
		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="message"></param>
		public void NotifyScriptSlotChangedOnArea(string area, string slot, string oldValue, string newValue)
		{
			context.NotifyScriptSlotChangedOnArea(area,slot,oldValue,newValue);
		}
		
		
		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="message"></param>
		public void NotifyScriptSlotChangedOnModule(string module, string slot, string oldValue, string newValue)
		{
			context.NotifyScriptSlotChangedOnArea(module,slot,oldValue,newValue);
		}
		
		
		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="message"></param>
		public void NotifyScriptSlotChangedOnObject(NWN2ObjectType type, Guid id, string area, string slot, string oldValue, string newValue)
		{
			context.NotifyScriptSlotChangedOnObject(type,id,area,slot,oldValue,newValue);
		}
	}
}
