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
 * This file added by Keiron Nicholson on 18/05/2010 at 10:42.
 */

using System;
using System.Collections.Generic;
using System.Text;
using Sussex.Flip.Utils;
using Sussex.Flip.UI;

namespace Sussex.Flip.Games.NeverwinterNightsTwo
{
	public class BecomesCommoner : Nwn2StatementBehaviour
	{	
		// requires: #include "NW_I0_GENERIC"
		
		// Make oCreatureToChange join one of the standard factions.
		// ** This will only work on an NPC **
		// - nStandardFaction: STANDARD_FACTION_*
		// void ChangeToStandardFaction(object oCreatureToChange, int nStandardFaction);
		
		// AssignCommand(oTarget, DetermineCombatRound());
		
		//int STANDARD_FACTION_HOSTILE  = 0;
		//int STANDARD_FACTION_COMMONER = 1;
		//int STANDARD_FACTION_MERCHANT = 2;
		//int STANDARD_FACTION_DEFENDER = 3;
						
		/// <summary>
		/// Checks whether the given statement is an asynchronous action (that is,
		/// it does not wait for the action to finish before executing the next command).
		/// </summary>
		public override bool IsAsynchronous { // DoesNotWaitForActionToFinishBeforeProceeding
			get { return false; }
		}
		
		
		public BecomesCommoner()
		{
			statementType = StatementType.Action;
			parameterCount = 1;
			components = new List<StatementComponent>(2) 
			{ 
				new StatementComponent(fitters.OnlyCreatures),
				new StatementComponent("becomes commoner")
			};
		}
		
		
		public override string GetCode(params string[] args)
		{
			if (args.Length != parameterCount) {
				throw new ArgumentException("Must pass exactly " + parameterCount + " parameters.","args");
			}	
			
			StringBuilder code = new StringBuilder();
			code.AppendLine(String.Format("ChangeToStandardFaction({0},STANDARD_FACTION_COMMONER);",args));
			code.AppendLine(String.Format("AssignCommand({0},DetermineCombatRound());",args));
			return code.ToString();
		}
		
		
		public override string GetNaturalLanguage(params string[] args)
		{
			if (args.Length != parameterCount) {
				throw new ArgumentException("Must pass exactly " + parameterCount + " parameters.","args");
			}
			
			return String.Format("{0} joins the Commoner faction",args);
		}
		
		
		public override StatementBehaviour DeepCopy()
		{
			return new BecomesCommoner();
		}
	}
}