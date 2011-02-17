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
 * This file added by Keiron Nicholson on 12/06/2010 at 14:50.
 */

using System;
using System.Collections.Generic;
using Sussex.Flip.Utils;
using Sussex.Flip.UI;

namespace Sussex.Flip.Games.NeverwinterNightsTwo
{
	public class CreateItemAtLocation : Nwn2StatementBehaviour
	{	
		// Create an object of the specified type at lLocation.
		// - nObjectType: OBJECT_TYPE_ITEM, OBJECT_TYPE_CREATURE, OBJECT_TYPE_PLACEABLE,
		//   OBJECT_TYPE_STORE, OBJECT_TYPE_WAYPOINT
		// - sTemplate
		// - lLocation
		// - bUseAppearAnimation
		// - sNewTag - if this string is not empty, it will replace the default tag from the template
		// object CreateObject(int nObjectType, string sTemplate, location lLocation, int bUseAppearAnimation=FALSE, string sNewTag="");
		
		// Requires flip_functions.nss
		// string GetSafeResRef(string sTag);
		// Uses this function to either find the object with the given tag and return its resref
		// (if the object exists) or to return the tag itself with the assumption that since
		// the object does not exist in game the tag comes from a Flip or Narrative Threads
		// block, in which case we can expect that the resref is probably the same as the tag.
				
		/// <summary>
		/// Checks whether the given statement is an asynchronous action (that is,
		/// it does not wait for the action to finish before executing the next command).
		/// </summary>
		public override bool IsAsynchronous { // DoesNotWaitForActionToFinishBeforeProceeding
			get { return false; }
		}
		
		
		public CreateItemAtLocation()
		{
			statementType = StatementType.Action;
			parameterCount = 2;
			components = new List<StatementComponent>(4) 
			{ 
				new StatementComponent("create"),
				new StatementComponent(fitters.OnlyItems),
				new StatementComponent("next to"),
				new StatementComponent(fitters.OnlyInstancesOrPlayers)
			};
		}
		
		
		public override string GetCode(params string[] args)
		{
			if (args.Length != parameterCount) {
				throw new ArgumentException("Must pass exactly " + parameterCount + " parameters.","args");
			}	
			
			string sTag;
			try {
				sTag = "\"" + args[0].Split(new char[]{'"'})[1] + "\"";
			}
			catch (Exception) {
				sTag = "\"FlipErrorCouldNotExtractTag\"";
			}
			
			string sTemplate = String.Format("GetSafeResRef({0})",sTag);
			string lLocation = String.Format("GetLocation({0})",args[1]);
			return String.Format("CreateObject(OBJECT_TYPE_ITEM,{0},{1},TRUE,{2});",sTemplate,lLocation,sTag);
		}
		
		
		public override string GetNaturalLanguage(params string[] args)
		{
			if (args.Length != parameterCount) {
				throw new ArgumentException("Must pass exactly " + parameterCount + " parameters.","args");
			}
			
			return String.Format("a copy of {0} appears at the location of {1}",args);
		}
		
		
		public override StatementBehaviour DeepCopy()
		{
			return new CreateItemAtLocation();
		}
	}
}