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
		
		
		public Nwn2TriggerFactory(Nwn2Fitters fitters)
		{
			if (fitters == null) throw new ArgumentNullException("fitters");
			this.fitters = fitters;
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
			return new NullTrigger();
		}
		
		
		public TriggerControl GetTrigger(NWN2ConversationConnector line, NWN2GameConversation conversation)
		{
			return new DialogueWasSpoken(line,conversation);
		}
	}
}
