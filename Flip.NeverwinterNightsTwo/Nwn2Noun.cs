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
 * This file added by Keiron Nicholson on 15/02/2010 at 14:38.
 */

using System;
using System.Windows.Controls;
using NWN2Toolset.NWN2.Data.Templates;
using Sussex.Flip.UI;
using Sussex.Flip.Games.NeverwinterNightsTwo.Utils;

namespace Sussex.Flip.Games.NeverwinterNightsTwo
{
	/// <summary>
	/// Description of Nwn2Noun.
	/// </summary>
	public class Nwn2Noun : Noun
	{
		protected Nwn2Type type;
		
		public Nwn2Type Type {
			get { return type; }
		}
		
		
		public Nwn2Noun(Image image, Nwn2Type type) : base(image)
		{			
			this.type = type;
		}
		
		
		public Nwn2Noun(Image image, NWN2ObjectType type) : this(image,Nwn2ScriptSlot.GetNwn2Type(type))
		{			
		}
	}
}
