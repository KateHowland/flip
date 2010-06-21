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
 * This file added by Keiron Nicholson on 01/04/2010 at 14:36.
 */

using System;
using NWN2Toolset.NWN2.Data.Templates;

namespace Sussex.Flip.Games.NeverwinterNightsTwo.Utils
{
	/// <summary>
	/// Description of NWScriptHelper.
	/// </summary>
	public static class NWScriptHelper
	{
		public static NWScriptObjectType? GetNWScriptConstant(Nwn2Type type)
		{
			switch (type) {
				case Nwn2Type.Creature:
					return NWScriptObjectType.OBJECT_TYPE_CREATURE;
				case Nwn2Type.Door:
					return NWScriptObjectType.OBJECT_TYPE_DOOR;
				case Nwn2Type.Encounter:
					return NWScriptObjectType.OBJECT_TYPE_ENCOUNTER;
				case Nwn2Type.Placeable:
					return NWScriptObjectType.OBJECT_TYPE_PLACEABLE;
				case Nwn2Type.Store:
					return NWScriptObjectType.OBJECT_TYPE_STORE;
				case Nwn2Type.Trigger:
					return NWScriptObjectType.OBJECT_TYPE_TRIGGER;
				case Nwn2Type.Item:
					return NWScriptObjectType.OBJECT_TYPE_ITEM;
				case Nwn2Type.Light:
					return NWScriptObjectType.OBJECT_TYPE_LIGHT;
				case Nwn2Type.PlacedEffect:
					return NWScriptObjectType.OBJECT_TYPE_PLACED_EFFECT;
				case Nwn2Type.Waypoint:
					return NWScriptObjectType.OBJECT_TYPE_WAYPOINT;
				default:
					return null;
			}
		}
		
		
		public static NWScriptObjectType? GetNWScriptConstant(NWN2ObjectType type)
		{
			Nwn2Type nwn2Type = Nwn2ScriptSlot.GetNwn2Type(type);
			return GetNWScriptConstant(nwn2Type);
		}
	}
}
