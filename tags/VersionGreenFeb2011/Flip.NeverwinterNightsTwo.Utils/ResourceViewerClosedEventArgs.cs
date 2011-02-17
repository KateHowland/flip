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
 * This file added by Keiron Nicholson on 15/01/2010 at 13:23.
 */

using System;
using NWN2Toolset.NWN2.Data;

namespace Sussex.Flip.Games.NeverwinterNightsTwo.Utils
{
	/// <summary>
	/// Arguments accompanying the event of a resource viewer being closed.
	/// </summary>
	/// <remarks>By the time a resource viewer has been closed, the resource
	/// it was displaying has already been disposed, so only the name
	/// of the resource is available in the arguments.</remarks>
	public class ResourceViewerClosedEventArgs : EventArgs
	{
		/// <summary>
		/// The name of the resource that was closed.
		/// </summary>
		private string resourceName;		
		public string ResourceName {
			get { return resourceName; }
		}
		
		
		/// <summary>
		/// Constructs a new <see cref="Sussex.Flip.Games.NeverwinterNightsTwo.Utils.ResourceViewerClosedEventArgs"/> instance.
		/// </summary>
		/// <param name="resourceName">The name of the resource that was closed.</param>
		public ResourceViewerClosedEventArgs(string resourceName)
		{
			this.resourceName = resourceName;
		}
		
		
		public override string ToString()
		{
			return resourceName;
		}
	}
}
