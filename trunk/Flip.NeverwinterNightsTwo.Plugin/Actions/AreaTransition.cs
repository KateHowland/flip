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
 * This file added by Keiron Nicholson on 21/05/2010 at 16:29.
 */

using System;
using System.Collections.Generic;
using Sussex.Flip.Utils;
using Sussex.Flip.UI;
using Sussex.Flip.Games.NeverwinterNightsTwo.Behaviours;

namespace Sussex.Flip.Games.NeverwinterNightsTwo
{
	public class AreaTransition : Nwn2StatementBehaviour
	{	
		// Remarks: this, unlike JumpsTo, can be used to transition to a new
		// area. 		
		// (Also: JumpsTo is an instruction, whereas this executes immediately.)
		// TODO
		// Have only tested this with waypoints, and have not confirmed what happens
		// if there are multiple objects with the same tag. Presumably it's just
		// the first one it finds, by whatever criteria it uses to search.
		
		//RWT-OEI 01/04/06
		//This script function moves a party to an object in a new area.
		//When a party is moved with this function, all players undergoing
		//the area transition will remain on the loading screen until everyone
		//changing area has finished loading the area on the client.
		//The area's OnClientEnter event will fire once all of the party members
		//have finished loading the new area
		//NOTE: If the party is already in oDestination's area, the party will be
		//jumped to oDestination without firing OnClientEnter. Party members that
		//are uncommandable will still be jumped, but party members that are dead
		//will not.
		// void JumpPartyToArea( object oPartyMember, object oDestination );
		
		// Uses:
		// object GetObjectInArea(string sTag, object oTargetArea)
		
		// Requires flip_functions.nss.
		
		
		/// <summary>
		/// Checks whether the given statement is an asynchronous action (that is,
		/// it does not wait for the action to finish before executing the next command).
		/// </summary>
		public override bool IsAsynchronous { 
			get { return false; }
		}
		
		
		public AreaTransition()
		{
			statementType = StatementType.Action;
			parameterCount = 2;
			components = new List<StatementComponent>(4) 
			{ 
				new StatementComponent("teleport player to"),
				new StatementComponent(fitters.OnlyAreas),
				new StatementComponent("at the location of"),
				new StatementComponent(fitters.OnlyInstances)
			};
		}
		
		
		public override string GetCode(params string[] args)
		{
			if (args.Length != parameterCount) {
				throw new ArgumentException("Must pass exactly " + parameterCount + " parameters.","args");
			}			
			
			string sTag = InstanceBehaviour.GetTagFromCode(args[1]);
			string oDestination = String.Format("GetObjectInArea({0},{1})",sTag,args[0]);
			return String.Format("JumpPartyToArea({0},{1});",PlayerBehaviour.NWScript_GetPlayer,oDestination);
		}
		
		
		public override string GetNaturalLanguage(params string[] args)
		{
			if (args.Length != parameterCount) {
				throw new ArgumentException("Must pass exactly " + parameterCount + " parameters.","args");
			}
			
			if (args[1] == "something") return String.Format("the player teleports to the location of some object in {0}",args);
			else return String.Format("the player teleports to the location of {1} in {0}",args);
		}
		
		
		public override StatementBehaviour DeepCopy()
		{
			return new AreaTransition();
		}
	}
}