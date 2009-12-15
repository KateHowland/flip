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
 * This file added by Keiron Nicholson on 10/12/2009 at 09:44.
 */

using System;
using System.ServiceModel;

namespace Sussex.Flip.Games.NeverwinterNightsTwo.Utils
{
	/// <summary>
	/// TODO
	/// </summary>
	public interface ICallbacks
	{
		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="message"></param>
		[OperationContract(IsOneWay = true)]
		void NotifyModuleChanged(string message);
		
		
		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="message"></param>
		[OperationContract(IsOneWay = true)]
		void NotifyResourceAdded(string type, string name);
		
		
		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="message"></param>
		[OperationContract(IsOneWay = true)]
		void NotifyResourceRemoved(string type, string name);
		
		
		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="message"></param>
		[OperationContract(IsOneWay = true)]
		void NotifyObjectAdded(NWN2Toolset.NWN2.Data.Templates.NWN2ObjectType type, string tag, Guid id, string area);
		
		
		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="message"></param>
		[OperationContract(IsOneWay = true)]
		void NotifyObjectRemoved(NWN2Toolset.NWN2.Data.Templates.NWN2ObjectType type, string tag, Guid id, string area);
		
		
		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="message"></param>
		[OperationContract(IsOneWay = true)]
		void NotifyBlueprintAdded(NWN2Toolset.NWN2.Data.Templates.NWN2ObjectType type, string resref);
		
		
		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="message"></param>
		[OperationContract(IsOneWay = true)]
		void NotifyBlueprintRemoved(NWN2Toolset.NWN2.Data.Templates.NWN2ObjectType type, string resref);
		
		
		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="message"></param>
		[OperationContract(IsOneWay = true)]
		void NotifyResourceViewerOpened(string type, string name);
		
		
		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="message"></param>
		[OperationContract(IsOneWay = true)]
		void NotifyResourceViewerClosed(string name);
		
		
		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="message"></param>
		[OperationContract(IsOneWay = true)]
		void NotifyScriptSlotChangedOnArea(string area, string slot, string oldValue, string newValue);
		
		
		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="message"></param>
		[OperationContract(IsOneWay = true)]
		void NotifyScriptSlotChangedOnModule(string module, string slot, string oldValue, string newValue);
		
		
		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="message"></param>
		[OperationContract(IsOneWay = true)]
		void NotifyScriptSlotChangedOnObject(NWN2Toolset.NWN2.Data.Templates.NWN2ObjectType type,
		                                     Guid id,		                                     
		                                     string area,
		                                     string slot,
		                                     string oldValue,
		                                     string newValue);
	}
}
