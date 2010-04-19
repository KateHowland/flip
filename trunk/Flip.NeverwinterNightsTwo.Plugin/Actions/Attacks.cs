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
 * This file added by Keiron Nicholson on 31/03/2010 at 10:50.
 */

using System;
using System.Collections.Generic;
using System.Text;
using Sussex.Flip.Utils;
using Sussex.Flip.UI;

namespace Sussex.Flip.Games.NeverwinterNightsTwo
{
	public class Attacks : Nwn2StatementBehaviour
	{	
		// Attack oAttackee.
		// - bPassive: If this is TRUE, attack is in passive mode.
		// void ActionAttack(object oAttackee, int bPassive=FALSE);
		
		public Attacks(Nwn2Fitters fitters) : base(fitters)
		{
			statementType = StatementType.Action;
			parameterCount = 2;
			components = new List<StatementComponent>(3) 
			{ 
				new StatementComponent(fitters.OnlyCreaturesOrPlayers),
				new StatementComponent("attacks"),
				new StatementComponent(fitters.OnlyCreaturesOrPlayers)
			};
		}
		
		
		public override string GetCode(params string[] args)
		{
			if (args.Length != parameterCount) {
				throw new ArgumentException("Must pass exactly " + parameterCount + " parameters.","args");
			}
			
			/* 
			 * Become hostile if you're attacking the player - otherwise, maintain your faction.
			 * 
			 * This results in:
			 * Player attacks Defender - player attacks defender, who goes hostile
			 * Player attacks Hostile - player attacks hostile
			 * Commoner/Defender attacks Player - commoner/defender goes hostile and attacks player
			 * Hostile attacks Player - hostile attacks player
			 * Commoner attacks Commoner - commoner attacks commoner, but stops quickly. Both maintain their faction.
			 * Hostile attacks Hostile - hostile attacks hostile, but stops quickly. Both maintain their faction.
			 */
			
			string attacker = args[0];
			string attackee = args[1];
			
			int changeToHostileFaction;
			if (attackee == Behaviours.Player.NWScript_GetPlayer) {
				changeToHostileFaction = 1;
			}
			else {
				changeToHostileFaction = 0;
			}
			
			StringBuilder code = new StringBuilder();
			
			if (changeToHostileFaction == 1) {
				code.AppendLine(String.Format("ChangeToStandardFaction({0},0);",attacker));
			}
			code.AppendLine(String.Format("AssignCommand({0},ActionAttack({1},FALSE));",attacker,attackee));	
			
			return code.ToString();
		}
		

		public override string GetNaturalLanguage(params string[] args)
		{
			if (args.Length != parameterCount) {
				throw new ArgumentException("Must pass exactly " + parameterCount + " parameters.","args");
			}
			
			return String.Format("{0} attacks {1}",args);
		}
		
		
		// TODO:
		// consider adding to superclass, using as tooltip/documentation
		public string GetNotes()
		{
			return "Anything that attacks the player will become Hostile, and stay that way. Anything that " +
				"attacks anything else will keep its faction. Note that an attacker of the same faction as its " +
				"target will get bored of attacking quickly. Simply assigning 'attack' multiple times doesn't currently " +
				"do anything to improve this. Using a (yet to be implemented) While loop might.";
		}
		
		
		public override StatementBehaviour DeepCopy()
		{
			return new Attacks(fitters);
		}
	}
}
