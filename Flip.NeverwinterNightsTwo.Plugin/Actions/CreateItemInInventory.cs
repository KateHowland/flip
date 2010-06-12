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
 * This file added by Keiron Nicholson on 12/06/2010 at 15:03.
 */


using System;
using System.Collections.Generic;
using Sussex.Flip.Utils;
using Sussex.Flip.UI;

namespace Sussex.Flip.Games.NeverwinterNightsTwo
{
	public class CreateItemInInventory : Nwn2StatementBehaviour
	{	
		// Create an item with the template sItemTemplate in oTarget's inventory.
		// - nStackSize: This is the stack size of the item to be created
		// * Return value: The object that has been created.  On error, this returns
		//   OBJECT_INVALID.
		// - RWT-OEI 03/13/07 If sNewTag is not left empty, it will set the tag of the newly created object.
		// - RWT-OEI 08/14/07 If bDisplayFeedback is changed to false, no feedback will be given to the player.
		// object CreateItemOnObject(string sItemTemplate, object oTarget=OBJECT_SELF, int nStackSize=1, string sNewTag="", int bDisplayFeedback=1);
		
		// Requires flip_functions.nss
		// string GetSafeResRef(string sTag);
		// Uses this function to either find the object with the given tag and return its resref
		// (if the object exists) or to return the tag itself with the assumption that since
		// the object does not exist in game the tag comes from a Flip or Narrative Threads
		// block, in which case we can expect that the resref is probably the same as the tag.
		
		public CreateItemInInventory()
		{
			statementType = StatementType.Action;
			parameterCount = 2;
			components = new List<StatementComponent>(4) 
			{ 
				new StatementComponent("give copy of item"),
				new StatementComponent(fitters.OnlyItems),
				new StatementComponent("to"),
				new StatementComponent(fitters.OnlyCreaturesOrPlayers)
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
			return String.Format("CreateItemOnObject({0},{1},1,{2},1);",sTemplate,args[1],sTag);
		}
		
		
		public override string GetNaturalLanguage(params string[] args)
		{
			if (args.Length != parameterCount) {
				throw new ArgumentException("Must pass exactly " + parameterCount + " parameters.","args");
			}
			
			return String.Format("a new copy of {0} appears in the inventory of {1}",args);
		}
		
		
		public override StatementBehaviour DeepCopy()
		{
			return new CreateItemInInventory();
		}
	}
}