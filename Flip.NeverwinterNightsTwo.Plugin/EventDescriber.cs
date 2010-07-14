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
 * This file added by Keiron Nicholson on 13/04/2010 at 14:29.
 */

using System;

namespace Sussex.Flip.Games.NeverwinterNightsTwo
{
	/// <summary>
	/// Description of EventDescriber!!!
	/// </summary>
	public class EventDescriber
	{				
		protected string userDefined = "When the user-defined special script for {0} fires";
		protected string onHeartbeat = "Every few seconds";
		
		
		public virtual string GetAreaEventDescription(string area, string eventName)
		{
			string natural;
			
			if (eventName == null) eventName = String.Empty;
			if (String.IsNullOrEmpty(area)) area = String.Empty;
			
			switch (eventName) {
					
				case "OnClientEnterScript":
					natural = "When the player (client) enters {0}";
					break;
					
				case "OnEnterScript":
					natural = "When the player enters {0}";
					break;
					
				case "OnExitScript":
					natural = "When the player exits {0}";
					break;
					
				case "OnHeartbeat":
					natural = "Every few seconds in {0}";
					break;
					
				case "OnUserDefined":
					natural = userDefined;
					break;
					
				default:
					return null;			
			}
			
			return String.Format(natural,area);
		}
				
		
		public virtual string GetModuleEventDescription(string eventName)
		{
			string natural;
			
			if (eventName == null) eventName = String.Empty;
			
			switch (eventName) {
					
				case "OnAcquireItem":
					natural = "When someone gets an item";
					break;
					
				case "OnActivateItem":
					natural = "When someone uses an item";
					break;
					
				case "OnClientEnter":
					natural = "When the player (client) enters the module";
					break;
					
				case "OnClientLeave":
					natural = "When the player (client) leaves the module";
					break;
					
				case "OnChat":
					natural = "When someone chats in a multiplayer game";
					break;
					
				case "OnCutsceneAbort":
					natural = "When a cutscene is aborted";
					break;
					
				case "OnHeartbeat":
					natural = onHeartbeat;
					break;
					
				case "OnModuleLoad":
					natural = "Once the module has been loaded";
					break;
					
				case "OnModuleStart":
					natural = "When the module begins";
					break;
					
				case "OnPCLoaded":
					natural = "Once the player character has been loaded";
					break;
					
				case "OnPlayerDeath":
					natural = "When the player dies";
					break;
					
				case "OnPlayerDying":
					natural = "When the player is dying";
					break;
					
				case "OnPlayerEquipItem":
					natural = "When the player equips an item";
					break;
					
				case "OnPlayerLevelUp":
					natural = "When the player levels up";
					break;
					
				case "OnPlayerRespawn":
					natural = "When the player respawns";
					break;
					
				case "OnPlayerRest":
					natural = "When the player rests";
					break;
					
				case "OnPlayerUnequipItem":
					natural = "When the player unequips an item";
					break;
					
				case "OnUnacquireItem":
					natural = "When someone loses an item";
					break;
					
				case "OnUserDefined":
					natural = "When the user-defined special script for the module fires";
					break;
					
				default:
					return null;			
			}
			
			return natural;
		}
				
		
		public virtual string GetCreatureEventDescription(string creature, string eventName)
		{
			string natural;
			
			if (eventName == null) eventName = String.Empty;
			if (String.IsNullOrEmpty(creature)) creature = String.Empty;
			
			switch (eventName) {
					
				case "OnBlocked":
					natural = "When {0} can't get to where it's trying to go";
					break;
					
				case "OnConversation":
					natural = "When {0} has a conversation";
					break;
					
				case "OnDamaged":
					natural = "Every time {0} is injured";
					break;
					
				case "OnDeath":
					natural = "When {0} dies";
					break;
					
				case "OnEndCombatRound":
					natural = "Every few seconds while {0} is in a fight";
					break;
					
				case "OnHeartbeat":
					natural = onHeartbeat;
					break;
					
				case "OnInventoryDisturbed":
					natural = "When the inventory of {0} changes";
					break;
					
				case "OnPerception":
					natural = "When {0} sees another creature";
					break;
					
				case "OnPhysicalAttacked":
					natural = "When {0} is physically attacked";
					break;
					
				case "OnRested":
					natural = "When {0} rests";
					break;
					
				case "OnSpawnIn":
					natural = "When {0} spawns in";
					break;
					
				case "OnSpellCastAt":
					natural = "When {0} has a spell cast at it";
					break;
					
				case "OnUserDefined":
					natural = userDefined;
					break;
					
				default:
					return null;			
			}
			
			return String.Format(natural,creature);
		}
				
		
		public virtual string GetOpenableEventDescription(string openable, string eventName)
		{
			string natural;
			
			if (eventName == null) eventName = String.Empty;
			if (String.IsNullOrEmpty(openable)) openable = String.Empty;
			
			switch (eventName) {
				
				case "OnClick": // doors only
					natural = "When the player clicks on {0}";
					break;
					
				case "OnClosed":
					natural = "When {0} closes";
					break;
					
				case "OnConversation":
					natural = "When {0} is spoken to";
					break;
					
				case "OnDamaged":
					natural = "When {0} is damaged";
					break;
					
				case "OnDeath":
					natural = "When {0} is destroyed";
					break;
					
				case "OnDisarm":
					natural = "When a trap on {0} is disarmed";
					break;
					
				case "OnFailToOpen": // doors only
					natural = "When someone tries and fails to open {0}";
					break;
					
				case "OnHeartbeat":
					natural = onHeartbeat;
					break;
					
				case "OnInvDisturbed": // placeables only
					natural = "When the contents of {0} change";
					break;
					
				case "OnLeftClick": // placeables only
					natural = "When the player clicks on {0}";
					break;
					
				case "OnLock":
					natural = "When {0} becomes locked";
					break;
					
				case "OnMeleeAttacked":
					natural = "When {0} is physically attacked";
					break;
					
				case "OnOpen":
					natural = "When {0} opens";
					break;
					
				case "OnSpellCastAt":
					natural = "When {0} has a spell cast at it";
					break;
					
				case "OnTrapTriggered":
					natural = "When {0} has a trap triggered";
					break;
					
				case "OnUnlock":
					natural = "When {0} becomes unlocked";
					break;
					
				case "OnUsed":
					natural = "When {0} is used";
					break;
					
				case "OnUserDefined":
					natural = userDefined;
					break;
					
				default:
					return null;			
			}
			
			return String.Format(natural,openable);
		}
				
		
		public virtual string GetEncounterEventDescription(string encounter, string eventName)
		{
			string natural;
			
			if (eventName == null) eventName = String.Empty;
			if (String.IsNullOrEmpty(encounter)) encounter = String.Empty;
			
			switch (eventName) {
					
				case "OnEntered":
					natural = "When something enters {0}";
					break;
					
				case "OnExhausted":
					natural = "When {0} has finished";
					break;
					
				case "OnExit":
					natural = "When something exits {0}";
					break;
					
				case "OnHeartbeat":
					natural = onHeartbeat;
					break;
					
				case "OnUserDefined":
					natural = userDefined;
					break;
					
				default:
					return null;			
			}
			
			return String.Format(natural,encounter);
		}
				
		
		public virtual string GetStoreEventDescription(string store, string eventName)
		{
			string natural;
			
			if (eventName == null) eventName = String.Empty;
			if (String.IsNullOrEmpty(store)) store = String.Empty;
			
			switch (eventName) {
					
				case "OnCloseStore":
					natural = "When {0} closes";
					break;
					
				case "OnOpenStore":
					natural = "When {0} opens";
					break;
					
				default:
					return null;			
			}
			
			return String.Format(natural,store);
		}
				
		
		public virtual string GetTriggerEventDescription(string trigger, string eventName)
		{
			string natural;
			
			if (eventName == null) eventName = String.Empty;
			if (String.IsNullOrEmpty(trigger)) trigger = String.Empty;
			
			switch (eventName) {
					
				case "OnClick":
					natural = "When the player clicks on {0}";
					break;
					
				case "OnDisarm":
					natural = "When a trap on {0} is disarmed";
					break;
					
				case "OnEnter":
					natural = "When something enters {0}";
					break;
					
				case "OnExit":
					natural = "When something exits {0}";
					break;
					
				case "OnHeartbeat":
					natural = onHeartbeat;
					break;
					
				case "OnTrapTriggered":
					natural = "When {0} has a trap triggered";
					break;
					
				case "OnUserDefined":
					natural = userDefined;
					break;
					
				default:
					return null;			
			}
			
			return String.Format(natural,trigger);
		}
	}
}
