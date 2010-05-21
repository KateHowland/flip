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
 * This file added by Keiron Nicholson on 21/05/2010 at 08:40.
 */

using System;
using System.Collections.Generic;
using Sussex.Flip.Utils;
using Sussex.Flip.UI;

namespace Sussex.Flip.Games.NeverwinterNightsTwo
{
	public class Delete : Nwn2StatementBehaviour
	{	
		// Remarks:
		// Destroys items which are in inventory as well as on the ground (for both PC and creatures).
		// This is how you would remove an item from inventory, there doesn't seem to be another way.
		// Destroys Placeables and PlacedEffects AS LONG AS they have Plot and Static set to FALSE.
		// Works with items, creatures, placeables, lights, placed effects, triggers and doors.
		// Doesn't work with trees, sounds and encounters.
		// Haven't tried waypoints or stores (possibly others).
		
		// Destroy oObject (irrevocably).
		// This will not work on modules and areas.
		// RWT-OEI 08/15/07 - If nDisplayFeedback is false, and the object being
		//  destroyed is an item from a player's inventory, the player will not
		//  be notified of the item being destroyed.
		// void DestroyObject(object oDestroy, float fDelay=0.0f, int nDisplayFeedback=TRUE);
		
		public Delete(Nwn2Fitters fitters) : base(fitters)
		{
			statementType = StatementType.Action;
			parameterCount = 1;
			components = new List<StatementComponent>(2) 
			{ 
				new StatementComponent("delete"),
				new StatementComponent(fitters.OnlyDestroyableObjects)
			};
		}
		
		
		public override string GetCode(params string[] args)
		{
			if (args.Length != parameterCount) {
				throw new ArgumentException("Must pass exactly " + parameterCount + " parameters.","args");
			}			
			
			return String.Format("DestroyObject({0},0.0f,TRUE);",args);
		}
		
		
		public override string GetNaturalLanguage(params string[] args)
		{
			if (args.Length != parameterCount) {
				throw new ArgumentException("Must pass exactly " + parameterCount + " parameters.","args");
			}
			
			return String.Format("{0} is permanently removed from the game",args);
		}
		
		
		public override StatementBehaviour DeepCopy()
		{
			return new Delete(fitters);
		}
	}
}


