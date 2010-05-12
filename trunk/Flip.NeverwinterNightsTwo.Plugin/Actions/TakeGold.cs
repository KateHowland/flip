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
 * This file added by Keiron Nicholson on 12/05/2010 at 18:19.
 */

using System;
using System.Collections.Generic;
using Sussex.Flip.Utils;
using Sussex.Flip.UI;

namespace Sussex.Flip.Games.NeverwinterNightsTwo
{
	public class TakeGold : Nwn2StatementBehaviour
	{	
		// Take nAmount of gold from oCreatureToTakeFrom.
		// - nAmount
		// - oCreatureToTakeFrom: If this is not a valid creature, nothing will happen.
		// - bDestroy: If this is TRUE, the caller will not get the gold.  Instead, the
		//   gold will be destroyed and will vanish from the game.
		// - bDisplayFeedback: If set to FALSE, none of the normal chat messages will be sent.
		// void TakeGoldFromCreature(int nAmount, object oCreatureToTakeFrom, int bDestroy=FALSE, int bDisplayFeedback=TRUE);
		
		public TakeGold(Nwn2Fitters fitters) : base(fitters)
		{
			statementType = StatementType.Action;
			parameterCount = 2;
			components = new List<StatementComponent>(4) 
			{ 
				new StatementComponent("take"),
				new StatementComponent(fitters.OnlyNumbers),
				new StatementComponent("gold from"),
				new StatementComponent(fitters.OnlyCreaturesOrPlayers)
			};
		}
		
		
		public override string GetCode(params string[] args)
		{
			if (args.Length != parameterCount) {
				throw new ArgumentException("Must pass exactly " + parameterCount + " parameters.","args");
			}			
			
			return String.Format("TakeGoldFromCreature({0},{1},TRUE,TRUE);",args);
		}
		
		
		public override string GetNaturalLanguage(params string[] args)
		{
			if (args.Length != parameterCount) {
				throw new ArgumentException("Must pass exactly " + parameterCount + " parameters.","args");
			}
			
			if (args[0] == "some number") args[0] = "some number of";
						
			if (args[1] == "1") return String.Format("{1} loses 1 gold coin.",args);			
			else return String.Format("{1} loses {0} gold coins",args);
		}
		
		
		public override StatementBehaviour DeepCopy()
		{
			return new TakeGold(fitters);
		}
	}
}