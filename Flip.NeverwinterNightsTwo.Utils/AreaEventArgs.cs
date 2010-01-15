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
 * This file added by Keiron Nicholson on 15/01/2010 at 12:37.
 */

using System;
using NWN2Toolset.NWN2.Data;

namespace Sussex.Flip.Games.NeverwinterNightsTwo.Utils
{
	/// <summary>
	/// Arguments accompanying an area event.
	/// </summary>
	public class AreaEventArgs : EventArgs
	{
		/// <summary>
		/// The area relating to this event.
		/// </summary>
		private NWN2GameArea area;		
		public NWN2GameArea Area {
			get { return area; }
		}
		
		
		/// <summary>
		/// Constructs a new <see cref="Sussex.Flip.Games.NeverwinterNightsTwo.Utils.AreaEventArgs"/> instance.
		/// </summary>
		/// <param name="area">The area relating to this event.</param>
		public AreaEventArgs(NWN2GameArea area)
		{
			this.area = area;
		}
		
		
		public override string ToString()
		{
			return area.Name;
		}
	}
}
