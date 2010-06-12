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
 * This file added by Keiron Nicholson on 12/06/2010 at 15:29.
 */

using System;
using System.Collections.Generic;
using NWN2Toolset.NWN2.Data.Templates;
using Sussex.Flip.UI;

namespace Sussex.Flip.Games.NeverwinterNightsTwo
{
	/// <summary>
	/// Description of InstancePlayerFitter.
	/// </summary>
	public class InstancePlayerFitter : Nwn2Fitter
	{
		protected List<NWN2ObjectType> types;
		protected string description;
		
		
		public InstancePlayerFitter()
		{
			this.types = new List<NWN2ObjectType>(0);
			this.description = "something";
		}
		
		
		public InstancePlayerFitter(NWN2ObjectType type, string description)
		{	
			if (description == null) throw new ArgumentException("description");
			
			this.types = new List<NWN2ObjectType>{type};
			this.description = description;
		}
		
		
		public InstancePlayerFitter(List<NWN2ObjectType> types, string description)
		{
			if (types == null) this.types = new List<NWN2ObjectType>(0);
			if (description == null) throw new ArgumentException("description");
			
			this.types = types;
			this.description = description;
		}
		
		
		public override bool Fits(Moveable moveable)
		{
			if (IsPlayer(moveable)) return true;
			
			if (types.Count > 0) {
				return IsInstance(moveable,types);
			}
			else {
				return IsInstance(moveable);
			}
		}
		
		
		public override string GetMoveableDescription()
		{
			return description;
		}
	}
}
