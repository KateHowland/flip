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
 * This file added by Keiron Nicholson on 14/04/2010 at 09:49.
 */

using System;
using System.Collections.Generic;

namespace Sussex.Flip.Games.NeverwinterNightsTwo.Utils
{
	/// <summary>
	/// Description of Scripts.
	/// </summary>
	public static class Scripts
	{
		private static List<Nwn2Type> eventRaisers;
			
			
		static Scripts()
		{
			eventRaisers = new List<Nwn2Type>
			{
				Nwn2Type.Module,
				Nwn2Type.Area,
				Nwn2Type.Creature,
				Nwn2Type.Door,
				Nwn2Type.Encounter,
				Nwn2Type.Placeable,
				Nwn2Type.Store,
				Nwn2Type.Trigger,
			};
		}
		
		
		public static List<Nwn2Type> GetEventRaisers()
		{
			return eventRaisers;
		}
		
		
		public static bool IsEventRaiser(Nwn2Type type)
		{
			return eventRaisers.Contains(type);
		}
	}
}
