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
 * This file added by Keiron Nicholson on 15/02/2010 at 15:37.
 */

using System;
using System.Windows.Controls;
using NWN2Toolset.NWN2.Data;
using OEIShared.Utils;
using Sussex.Flip.UI;
using Sussex.Flip.Games.NeverwinterNightsTwo.Utils;

namespace Sussex.Flip.Games.NeverwinterNightsTwo
{
	/// <summary>
	/// Description of AreaBlock.
	/// </summary>
	public class AreaBlock : Nwn2Noun
	{
		protected NWN2GameArea area;
		
		public NWN2GameArea Area {
			get { return area; }
		}
		
		
		public string AreaName {
			get {
				if (area == null) throw new InvalidOperationException("AreaBlock has no area.");
				else return area.Name;
			}
		}
		
		
		public AreaBlock(Image image, NWN2GameArea area) : base(image,Nwn2Type.Area)
		{
			this.area = area;
		}
	}
}
