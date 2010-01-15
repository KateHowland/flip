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
 * This file added by Keiron Nicholson on 15/01/2010 at 11:34.
 */

using System;
using NWN2Toolset.NWN2.Data;
using NWN2Toolset.NWN2.Data.Instances;

namespace Sussex.Flip.Games.NeverwinterNightsTwo.Utils
{
	/// <summary>
	/// Arguments accompanying an INWN2Object event.
	/// </summary>
	public class InstanceEventArgs : EventArgs
	{
		/// <summary>
		/// The instance relating to this event.
		/// </summary>
		private INWN2Instance instance;		
		public INWN2Instance Instance {
			get { return instance; }
		}
		
		
		/// <summary>
		/// The area that owns (or owned) the instance.
		/// </summary>
		private NWN2GameArea area;
		public NWN2GameArea Area {
			get { return area; }
		}
		
		
		/// <summary>
		/// Constructs a new <see cref="Sussex.Flip.Games.NeverwinterNightsTwo.Utils.InstanceEventArgs"/> instance.
		/// </summary>
		/// <param name="instance">The instance relating to this event.</param>
		/// <param name="area">The area that owns (or owned) the instance.</param>
		public InstanceEventArgs(INWN2Instance instance, NWN2GameArea area)
		{
			this.instance = instance;
			this.area = area;
		}
		
		
		public override string ToString()
		{
			return instance.Name;
		}
	}
}