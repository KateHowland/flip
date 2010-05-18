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
 * This file added by Keiron Nicholson on 12/05/2010 at 18:23.
 */
 
using System;
using System.Collections.Generic;
using Sussex.Flip.Utils;
using Sussex.Flip.UI;

namespace Sussex.Flip.Games.NeverwinterNightsTwo
{
	public class GiveXP : Nwn2StatementBehaviour
	{	
		// Gives nXpAmount to oCreature.
		// void GiveXPToCreature(object oCreature, int nXpAmount);
		
		public GiveXP(Nwn2Fitters fitters) : base(fitters)
		{
			statementType = StatementType.Action;
			parameterCount = 2;
			components = new List<StatementComponent>(4) 
			{ 
				new StatementComponent("give"),
				new StatementComponent(fitters.OnlyNumbers),
				new StatementComponent("XP to"),
				new StatementComponent(fitters.OnlyCreaturesOrPlayers)
			};
		}
		
		
		public override string GetCode(params string[] args)
		{
			if (args.Length != parameterCount) {
				throw new ArgumentException("Must pass exactly " + parameterCount + " parameters.","args");
			}			
			
			return String.Format("GiveXPToCreature({1},{0});",args);
		}
		
		
		public override string GetNaturalLanguage(params string[] args)
		{
			if (args.Length != parameterCount) {
				throw new ArgumentException("Must pass exactly " + parameterCount + " parameters.","args");
			}
			
			if (args[0] == "some number") args[0] = "some number of";
									
			if (args[1] == "1") return String.Format("{1} loses 1 experience point",args);			
			else return String.Format("{1} gets {0} experience points",args);
		}
		
		
		public override StatementBehaviour DeepCopy()
		{
			return new GiveXP(fitters);
		}
	}
}