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
 * This file added by Keiron Nicholson on 18/05/2010 at 10:58.
 */

using System;
using System.Collections.Generic;
using Sussex.Flip.Utils;
using Sussex.Flip.UI;

namespace Sussex.Flip.Games.NeverwinterNightsTwo
{
	public class MakeHenchman : Nwn2StatementBehaviour
	{	
		// requires: #include "ginc_henchman"
		
		// Wrapper function for AddHenchman that adds some extra functionality.
		// oHench is added as a henchman of oMaster.
		//  - bForce - if set, this will temporarily up the max henchman to allow the henchman in the party (is immediately set back to what it was).
		//  - bOverrideBehavior - if set, oHench's event handling scripts will be replaced with some stock henchman ones to get some easy default henchman behavior
		// Return Value: 1 on success, 0 on error
		// notes - returns 0 if there is no room in party to add henchman, though this is not technically an error
		// int HenchmanAdd(object oMaster, object oHench, int bForce=0, int bOverrideBehavior=0)
		
		public MakeHenchman()
		{
			statementType = StatementType.Action;
			parameterCount = 2;
			components = new List<StatementComponent>(3) 
			{ 
				new StatementComponent(fitters.OnlyCreatures),
				new StatementComponent("becomes protector of"),
				new StatementComponent(fitters.OnlyCreaturesOrPlayers),
			};
		}
		
		
		public override string GetCode(params string[] args)
		{
			if (args.Length != parameterCount) {
				throw new ArgumentException("Must pass exactly " + parameterCount + " parameters.","args");
			}	
			
			// Existing event-handler scripts will be overridden.
			// TODO There should be some kind of warning about this.
			return String.Format("HenchmanAdd({1},{0},1,1);",args);
		}
		
		
		public override string GetNaturalLanguage(params string[] args)
		{
			if (args.Length != parameterCount) {
				throw new ArgumentException("Must pass exactly " + parameterCount + " parameters.","args");
			}
			
			return String.Format("{0} starts to follow and protect {1}",args);
		}
		
		
		public override StatementBehaviour DeepCopy()
		{
			return new MakeHenchman();
		}
	}
}