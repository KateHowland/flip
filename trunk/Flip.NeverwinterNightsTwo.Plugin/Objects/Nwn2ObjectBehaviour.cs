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
 * This file added by Keiron Nicholson on 01/04/2010 at 16:57.
 */

using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Sussex.Flip.Games.NeverwinterNightsTwo.Utils;
using Sussex.Flip.UI;

namespace Sussex.Flip.Games.NeverwinterNightsTwo.Behaviours
{
	/// <summary>
	/// Description of Nwn2ObjectBehaviour.
	/// </summary>
	public abstract class Nwn2ObjectBehaviour : ObjectBehaviour
	{
		protected Nwn2ObjectBehaviour() : this(string.Empty,string.Empty)
		{			
		}
		
		
		public Nwn2ObjectBehaviour(string identifier, string displayName) : base(identifier,displayName)
		{			
		}
		
		
		public abstract Nwn2Type Nwn2Type { get; }
		
		
		public override bool Equals(ObjectBehaviour other)
		{
			if (!base.Equals(other)) return false;
			
			Nwn2ObjectBehaviour compare = other as Nwn2ObjectBehaviour;
			
			return compare != null && compare.Nwn2Type == this.Nwn2Type;
		}
	}
}
