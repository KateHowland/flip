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
 * This file added by Keiron Nicholson on 17/03/2010 at 11:05.
 */
 
 using System;
 using System.Collections.Generic;
 using System.Reflection;
 using System.Windows.Media;
 using Sussex.Flip.Games.NeverwinterNightsTwo.Utils;
 using Sussex.Flip.UI;

namespace Sussex.Flip.Games.NeverwinterNightsTwo
{
	/// <summary>
	/// Description of Nwn2EventBlockFactory.
	/// </summary>
	public class Nwn2EventBlockFactory
	{		
		#region Constructors
		
		public Nwn2EventBlockFactory()
		{
		}
			
		#endregion
		
		#region Methods
				
		public List<EventBlock> GetEvents()
		{
			int count = 51;
			
			List<EventBlock> eventBlocks = new List<EventBlock>(count);
			
			List<ScriptSlotTuple> slots = new List<ScriptSlotTuple>(count);
			
			foreach (Nwn2Type type in Enum.GetValues(typeof(Nwn2Type))) {
				
				foreach (ScriptSlotTuple slot in Nwn2ScriptSlot.GetScriptSlotTuples(type)) {
					if (!slots.Contains(slot)) slots.Add(slot);
				}				
			}
			
			foreach (ScriptSlotTuple slot in slots) {
				EventBehaviour behaviour = new EventBehaviour(slot.SlotName,slot.DisplayName);
				EventBlock block = new EventBlock(behaviour);
				eventBlocks.Add(block);
			}
			
			return eventBlocks;
		}
		
		#endregion
	}
}
