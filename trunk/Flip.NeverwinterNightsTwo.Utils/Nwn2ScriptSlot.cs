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
 * This file added by Keiron Nicholson on 14/10/2009 at 17:26.
 */

using System;
using System.Collections.Generic;
using NWN2Toolset.NWN2.Data.Templates;

namespace Sussex.Flip.Games.NeverwinterNightsTwo.Utils
{
	/// <summary>
	/// Provides functionality relating to Neverwinter Nights 2 script slots.
	/// </summary>
	public static class Nwn2ScriptSlot
	{
		/// <summary>
		/// Gets a list of script slot names that are defined on a game
		/// object of the given type.
		/// </summary>
		/// <param name="type">The type of game object.</param>
		/// <returns>A list of names of script slots defined on an object
		/// of this type, which may be empty.</returns>
		public static IList<string> GetScriptSlotNames(NWN2ObjectType type)
		{
			Nwn2EventRaiser eventRaiser;
			switch (type) {
				case NWN2ObjectType.Creature:
					eventRaiser = Nwn2EventRaiser.Creature;
					break;
				case NWN2ObjectType.Door:
					eventRaiser = Nwn2EventRaiser.Door;
					break;
				case NWN2ObjectType.Encounter:
					eventRaiser = Nwn2EventRaiser.Encounter;
					break;
				case NWN2ObjectType.Placeable:
					eventRaiser = Nwn2EventRaiser.Placeable;
					break;
				case NWN2ObjectType.Store:
					eventRaiser = Nwn2EventRaiser.Store;
					break;
				case NWN2ObjectType.Trigger:
					eventRaiser = Nwn2EventRaiser.Trigger;
					break;
				default:
					return new List<string>(0);
			}
			return GetScriptSlotNames(eventRaiser);
		}
		
		
		/// <summary>
		/// Gets a list of script slot names that are defined on an
		/// event-raising game object of the given type.
		/// </summary>
		/// <param name="type">The type of event-raising game object.</param>
		/// <returns>A list of names of script slots defined on an object
		/// of this type.</returns>
		public static IList<string> GetScriptSlotNames(Nwn2EventRaiser type)
		{
			switch (type) {
				case Nwn2EventRaiser.Area:
					return new List<string>{ "OnClientEnterScript",
											 "OnEnterScript",
											 "OnExitScript",
											 "OnHeartbeat",
											 "OnUserDefined" };
					
				case Nwn2EventRaiser.Creature:
					return new List<string>{ "OnBlocked",
											 "OnConversation",
											 "OnDamaged",
											 "OnDeath",
											 "OnEndCombatRound",
											 "OnHeartbeat",
											 "OnInventoryDisturbed",
											 "OnPerception",
											 "OnPhysicalAttacked",
											 "OnRested",
											 "OnSpawnIn",
											 "OnSpellCastAt",
											 "OnUserDefined" };
					
				case Nwn2EventRaiser.Door:
					return new List<string>{ "OnClick",
											 "OnClosed",
											 "OnConversation",
											 "OnDamaged",
											 "OnDeath",
											 "OnDisarm",
											 "OnFailToOpen",
											 "OnHeartbeat",
											 "OnLock",
											 "OnMeleeAttacked",
											 "OnOpen",
											 "OnSpellCastAt",
											 "OnTrapTriggered",
											 "OnUnlock",
											 "OnUsed",
											 "OnUserDefined" };
					
				case Nwn2EventRaiser.Encounter:
					return new List<string>{ "OnEntered",
											 "OnExhausted",
											 "OnExit",
											 "OnHeartbeat",
											 "OnUserDefined" };
					
				case Nwn2EventRaiser.Module:
					return new List<string>{ "OnAcquireItem",
											 "OnActivateItem",
											 "OnClientEnter",
											 "OnClientLeave",
											 "OnChat",	
											 "OnCutsceneAbort",
											 "OnHeartbeat",
											 "OnModuleLoad",
											 "OnModuleStart",
											 "OnPCLoaded",
											 "OnPlayerDeath",
											 "OnPlayerDying",
											 "OnPlayerEquipItem",
											 "OnPlayerLevelUp",
											 "OnPlayerRespawn",
											 "OnPlayerRest",
											 "OnPlayerUnequipItem",	
											 "OnUnacquireItem",	 
											 "OnUserDefined" };
					
				case Nwn2EventRaiser.Placeable:
					return new List<string>{ "OnClosed",
											 "OnConversation",
											 "OnDamaged",
											 "OnDeath",
											 "OnDisarm",
											 "OnHeartbeat",
											 "OnInvDisturbed",
											 "OnLeftClick",
											 "OnLock",
											 "OnMeleeAttacked",
											 "OnOpen",
											 "OnSpellCastAt",
											 "OnTrapTriggered",
											 "OnUnlock",
											 "OnUsed",
											 "OnUserDefined" };
					
				case Nwn2EventRaiser.Store:
					return new List<string>{ "OnCloseStore",
											 "OnOpenStore" };
					
				case Nwn2EventRaiser.Trigger:
					return new List<string>{ "OnClick",
											 "OnDisarm",
											 "OnEnter",
											 "OnExit",
											 "OnHeartbeat",
											 "OnTrapTriggered",
											 "OnUserDefined" };
					
				default:
					throw new ArgumentException("Nwn2EventRaiser value was not recognised.","type");
			}
		}
	}
}
