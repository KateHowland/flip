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
 * This file added by Keiron Nicholson on 14/08/2009 at 16:08.
 */

using System;
using NWN2Toolset.NWN2.Data.Blueprints;

namespace Sussex.Flip.Games.NeverwinterNightsTwo.Utils
{
	/// <summary> 
	/// Provides functionality relating to Neverwinter
	/// Nights 2 game objects.
	/// </summary>
	public static class Nwn2GameObjectFunctions
	{
		/// <summary>
		/// Checks whether a particular type of game object must 
		/// be placed within an exterior area's player-accessible region.
		/// </summary>
		/// <param name="blueprint">A blueprint representing a particular
		/// type of game object</param>
		/// <returns>True if an instance of the given blueprint must be
		/// placed inside the player-accessible region; false otherwise.</returns>
		public static bool MustPlaceWithinPlayerAccessibleRegion(INWN2Blueprint blueprint) 
		{
			// TODO: Implement this method properly following testing.
			return true;
		}
	}
}
