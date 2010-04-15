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
			Nwn2Type nwn2Type = GetNwn2Type(type);
			return GetScriptSlotNames(nwn2Type);
		}
				
		
		/// <summary>
		/// Gets the Nwn2Type value which corresponds to the 
		/// given NWN2ObjectType value.
		/// </summary>
		/// <param name="type">The type to return a corresponding type for.</param>
		/// <returns>A Nwn2Type value.</returns>
		public static Nwn2Type GetNwn2Type(NWN2ObjectType type)
		{
			return (Nwn2Type)Enum.Parse(typeof(Nwn2Type),Enum.GetName(typeof(NWN2ObjectType),type));
		}
				
		
		/// <summary>
		/// Gets the NWN2ObjectType value which corresponds to the 
		/// given Nwn2Type value, or null if there is
		/// no corresponding type.
		/// </summary>
		/// <param name="type">The type to return a corresponding type for.</param>
		/// <returns>A nullable NWN2ObjectType.</returns>
		public static NWN2ObjectType? GetObjectType(Nwn2Type type)
		{
			try {
				return (NWN2ObjectType?)Enum.Parse(typeof(NWN2ObjectType),Enum.GetName(typeof(Nwn2Type),type));
			}
			catch (Exception) {
				return null;
			}
		}
		
		
		/// <summary>
		/// Gets a list of script slot names that are defined 
		/// on a particular type of game object.
		/// </summary>
		/// <param name="type">The type of game object.</param>
		/// <returns>A list of names of script slots defined on an object
		/// of this type.</returns>
		public static IList<string> GetScriptSlotNames(Nwn2Type type)
		{
			switch (type) {
				case Nwn2Type.Area:
					return new List<string>{ "OnClientEnterScript",
											 "OnEnterScript",
											 "OnExitScript",
											 "OnHeartbeat",
											 "OnUserDefined" };
					
				case Nwn2Type.Creature:
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
					
				case Nwn2Type.Door:
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
					
				case Nwn2Type.Encounter:
					return new List<string>{ "OnEntered",
											 "OnExhausted",
											 "OnExit",
											 "OnHeartbeat",
											 "OnUserDefined" };
					
				case Nwn2Type.Module:
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
					
				case Nwn2Type.Placeable:
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
					
				case Nwn2Type.Store:
					return new List<string>{ "OnCloseStore",
											 "OnOpenStore" };
					
				case Nwn2Type.Trigger:
					return new List<string>{ "OnClick",
											 "OnDisarm",
											 "OnEnter",
											 "OnExit",
											 "OnHeartbeat",
											 "OnTrapTriggered",
											 "OnUserDefined" };
					
				default:
					return new List<string>(0);
			}
		}
		
		
		private static ScriptSlotTuple GetTuple(string slotName, string displayName)
		{
			return new ScriptSlotTuple(slotName,displayName);
		}
		
		
		private static ScriptSlotTuple GetTuple(string slotName)
		{
			return GetTuple(slotName,slotName);
		}
		
		
		public static IList<ScriptSlotTuple> GetScriptSlotTuples(Nwn2Type type)
		{
			switch (type) {
				case Nwn2Type.Area:
					return new List<ScriptSlotTuple>{ 
						GetTuple("OnClientEnterScript"),
						GetTuple("OnEnterScript"),
						GetTuple("OnExitScript"),
						GetTuple("OnHeartbeat"),
						GetTuple("OnUserDefined")
					};
					
				case Nwn2Type.Creature:
					return new List<ScriptSlotTuple>{ 
						GetTuple("OnBlocked"),
						GetTuple("OnConversation"),
						GetTuple("OnDamaged"),
						GetTuple("OnDeath"),
						GetTuple("OnEndCombatRound"),
						GetTuple("OnHeartbeat"),
						GetTuple("OnInventoryDisturbed"),
						GetTuple("OnPerception"),
						GetTuple("OnPhysicalAttacked"),
						GetTuple("OnRested"),
						GetTuple("OnSpawnIn"),
						GetTuple("OnSpellCastAt"),
						GetTuple("OnUserDefined")
					};
			
				case Nwn2Type.Door:
					return new List<ScriptSlotTuple>{ 
						GetTuple("OnClick"),
						GetTuple("OnClosed"),
						GetTuple("OnConversation"),
						GetTuple("OnDamaged"),
						GetTuple("OnDeath"),
						GetTuple("OnDisarm"),
						GetTuple("OnFailToOpen"),
						GetTuple("OnHeartbeat"),
						GetTuple("OnLock"),
						GetTuple("OnMeleeAttacked"),
						GetTuple("OnOpen"),
						GetTuple("OnSpellCastAt"),
						GetTuple("OnTrapTriggered"),
						GetTuple("OnUnlock"),
						GetTuple("OnUsed")
					};
					
				case Nwn2Type.Encounter:
					return new List<ScriptSlotTuple>{ 
						GetTuple("OnEntered"),
						GetTuple("OnExhausted"),
						GetTuple("OnExit"),
						GetTuple("OnHeartbeat"),
						GetTuple("OnUserDefined")
					};
					
				case Nwn2Type.Module:
					return new List<ScriptSlotTuple>{ 
						GetTuple("OnAcquireItem"),
						GetTuple("OnActivateItem"),
						GetTuple("OnClientEnter"),
						GetTuple("OnClientLeave"),
						GetTuple("OnChat"),
						GetTuple("OnCutsceneAbort"),
						GetTuple("OnHeartbeat"),
						GetTuple("OnModuleLoad"),
						GetTuple("OnModuleStart"),
						GetTuple("OnPCLoaded"),
						GetTuple("OnPlayerDeath"),
						GetTuple("OnPlayerDying"),
						GetTuple("OnPlayerEquipItem"),
						GetTuple("OnPlayerLevelUp"),
						GetTuple("OnPlayerRespawn"),
						GetTuple("OnPlayerRest"),
						GetTuple("OnPlayerUnequipItem"),
						GetTuple("OnUnacquireItem"),
						GetTuple("OnUserDefined")
					};
					
				case Nwn2Type.Placeable:
					return new List<ScriptSlotTuple>{ 
						GetTuple("OnClosed"),
						GetTuple("OnConversation"),
						GetTuple("OnDamaged"),
						GetTuple("OnDeath"),
						GetTuple("OnDisarm"),
						GetTuple("OnHeartbeat"),
						GetTuple("OnInvDisturbed"),
						GetTuple("OnLeftClick"),
						GetTuple("OnLock"),
						GetTuple("OnMeleeAttacked"),
						GetTuple("OnOpen"),
						GetTuple("OnSpellCastAt"),
						GetTuple("OnTrapTriggered"),
						GetTuple("OnUnlock"),
						GetTuple("OnUsed"),
						GetTuple("OnUserDefined")
					};
					
				case Nwn2Type.Store:
					return new List<ScriptSlotTuple>{ 
						GetTuple("OnCloseStore"),
						GetTuple("OnOpenStore")
					};
					
				case Nwn2Type.Trigger:
					return new List<ScriptSlotTuple>{ 
						GetTuple("OnClick"),
						GetTuple("OnDisarm"),
						GetTuple("OnEnter"),
						GetTuple("OnExit"),
						GetTuple("OnHeartbeat"),
						GetTuple("OnTrapTriggered"),
						GetTuple("OnUserDefined")
					};
					
				default:
					return new List<ScriptSlotTuple>(0);
			}
		}
	}
}
