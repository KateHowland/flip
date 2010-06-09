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
 * This file added by Keiron Nicholson on 24/05/2010 at 12:45.
 */

using System;
using System.Collections.Generic;
using Sussex.Flip.Games.NeverwinterNightsTwo.Utils;
using Sussex.Flip.UI;
using NWN2Toolset.NWN2.Data;
using NWN2Toolset.NWN2.Data.ConversationData;

namespace Sussex.Flip.Games.NeverwinterNightsTwo
{
	/// <summary>
	/// Description of Nwn2TriggerFactory.
	/// </summary>
	public class Nwn2TriggerFactory
	{
		protected Nwn2Fitters fitters;
		protected Nwn2Session session;
		
		
		public Nwn2TriggerFactory(Nwn2Fitters fitters)
		{
			if (fitters == null) throw new ArgumentNullException("fitters");
			this.fitters = fitters;
			this.session = new Nwn2Session();
		}
		
		
		public List<TriggerControl> GetTriggers()
		{
			return new List<TriggerControl> { 
				new NullTrigger(),
				new CreatureDies(fitters.OnlyCreatures),
				new DoorOrPlaceableIsLocked(fitters.OnlyDoorsOrPlaceables),
				new DoorOrPlaceableIsUnlocked(fitters.OnlyDoorsOrPlaceables),
				new PlaceableUsed(fitters.OnlyPlaceables),
				new TriggerEntered(fitters.OnlyTriggers),
				new TriggerExited(fitters.OnlyTriggers),
				new AreaEntered(fitters.OnlyAreas),
				new ItemAcquired(),
				new ItemUnacquired(),
				new ItemActivated(),
				new ModuleHeartbeat(),
				new ModuleStarted(),
				new PlayerRespawned()
			};
		}
		
		
		public TriggerControl GetDefaultTrigger()
		{
			// HACK:
			return new NullTrigger();
		}
		
		
		public TriggerControl GetTrigger(NWN2ConversationConnector line, NWN2GameConversation conversation)
		{
			return new DialogueWasSpoken(line,conversation);
		}
		
		
		public TriggerControl GetTriggerFromAddress(Nwn2Address address)
		{
			if (address == null) throw new ArgumentNullException("address");
		
			TriggerControl empty = GetEmptyTrigger(address.TargetType,address.TargetSlot);
			
			if (empty == null) return null;
			
//			ObjectBlock block = GetBlock(address);
			
			return empty;
		}
		
		
		public TriggerControl GetTriggerFromAddress(Nwn2ConversationAddress address)
		{
			if (address == null) throw new ArgumentNullException("address");
			
			NWN2GameModule mod = NWN2Toolset.NWN2ToolsetMainForm.App.Module;
			if (mod == null) throw new InvalidOperationException("No module is open.");
			
			NWN2GameConversation conversation = session.GetConversation(address.Conversation);
			
			if (conversation == null) throw new ArgumentException("Could not find conversation '" + address.Conversation + "' in this module.","conversation");
			
			if (NWN2Toolset.NWN2ToolsetMainForm.App.GetViewerForResource(conversation) == null) {
				conversation.Demand();
			}
			
			NWN2ConversationLine line = conversation.GetLineFromGUID(address.LineID);
			
			if (line == null) throw new ArgumentException("Could not find a line with the given ID in conversation '" + address.Conversation + "'.","conversation");
		
			return new DialogueWasSpoken(line.OwningConnector,conversation);
		}
		
		
//		protected ObjectBlock GetBlock(Nwn2Address address)
//		{
//			if (address == null) throw new ArgumentNullException("address");
//			
//			// HACK:
//			Nwn2ObjectBlockFactory blocks = new Nwn2ObjectBlockFactory(new Nwn2ImageProvider(new Sussex.Flip.Games.NeverwinterNightsTwo.Integration.NarrativeThreadsHelper()));
//			
//			if (address.TargetType == Nwn2Type.Module) {
//				return blocks.CreateModuleBlock();
//			}
//			
//			else if (address.TargetType == Nwn2Type.Area) {
//				blocks.CreateAreaBlock(new Sussex.Flip.Games.NeverwinterNightsTwo.Behaviours.AreaBehaviour(address.AreaTag,addr
//			}
//		}
		
		
		//protected void PopulateTrigger(TriggerControl trigger, 
		
		
		// TODO:
		// TriggerControls should stop taking in fitters as it's stupid.
		// Once that's done, fix below (the use of AnyFitters).
		protected TriggerControl GetEmptyTrigger(Nwn2Type targetType, string scriptSlot)
		{
			if (scriptSlot == null) throw new ArgumentNullException("scriptSlot");
						
			switch (targetType) {
					
				case Nwn2Type.Area:
					if (scriptSlot == "OnClientEnterScript") return new AreaEntered(new AnyFitter());					
					break;
					
				case Nwn2Type.Creature:
					if (scriptSlot == "OnDeath") return new CreatureDies(new AnyFitter());	
					break;
					
				case Nwn2Type.Door:
					if (scriptSlot == "OnLock") return new DoorOrPlaceableIsLocked(new AnyFitter());	
					if (scriptSlot == "OnUnlock") return new DoorOrPlaceableIsUnlocked(new AnyFitter());	
					break;
					
				case Nwn2Type.Module:
					if (scriptSlot == "OnAcquireItem") return new ItemAcquired();	
					if (scriptSlot == "OnActivateItem") return new ItemActivated();	
					if (scriptSlot == "OnUnacquireItem") return new ItemUnacquired();	
					if (scriptSlot == "OnHeartbeat") return new ModuleHeartbeat();	
					if (scriptSlot == "OnModuleStart") return new ModuleStarted();	
					if (scriptSlot == "OnPlayerRespawn") return new PlayerRespawned();	
					break;
					
				case Nwn2Type.Trigger:
					if (scriptSlot == "OnEnter") return new TriggerEntered(new AnyFitter());	
					if (scriptSlot == "OnExit") return new TriggerExited(new AnyFitter());	
					break;
					
				case Nwn2Type.Placeable:
					if (scriptSlot == "OnLock") return new DoorOrPlaceableIsLocked(new AnyFitter());	
					if (scriptSlot == "OnUnlock") return new DoorOrPlaceableIsUnlocked(new AnyFitter());	
					break;
					
				default:
					break;
			}
			
			return null;
		}
	}
}
