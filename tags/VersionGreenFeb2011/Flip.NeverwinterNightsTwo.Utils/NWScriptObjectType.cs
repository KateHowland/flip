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
 * This file added by Keiron Nicholson on 01/04/2010 at 14:13.
 */

using System;

namespace Sussex.Flip.Games.NeverwinterNightsTwo.Utils
{
	/// <summary>
	/// Constants used by NWScript to represent object types.
	/// </summary>
	public enum NWScriptObjectType
	{
		OBJECT_TYPE_ALL = 32767,
		OBJECT_TYPE_AREA_OF_EFFECT = 16,
		OBJECT_TYPE_CREATURE = 1,
		OBJECT_TYPE_DOOR = 8,
		OBJECT_TYPE_ENCOUNTER = 256,
		OBJECT_TYPE_INVALID = 32767,
		OBJECT_TYPE_ITEM = 2,
		OBJECT_TYPE_LIGHT = 512,
		OBJECT_TYPE_PLACEABLE = 64,
		OBJECT_TYPE_PLACED_EFFECT = 1024,
		OBJECT_TYPE_STORE = 128,
		OBJECT_TYPE_TRIGGER = 4,
		OBJECT_TYPE_WAYPOINT = 32
	}
}
