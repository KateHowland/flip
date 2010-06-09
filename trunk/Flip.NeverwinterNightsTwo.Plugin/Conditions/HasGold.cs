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
 * This file added by Keiron Nicholson on 21/05/2010 at 17:28.
 */

using System;
using System.Collections.Generic;
using Sussex.Flip.UI;

namespace Sussex.Flip.Games.NeverwinterNightsTwo
{
	public class HasGold : Nwn2StatementBehaviour
	{	
		// Get the amount of gold possessed by oTarget.
		// int GetGold(object oTarget=OBJECT_SELF);
				
		public HasGold()
		{
			statementType = StatementType.Condition;
			parameterCount = 2;
			components = new List<StatementComponent>(4) 
			{ 
				new StatementComponent(fitters.OnlyCreaturesOrPlayers),
				new StatementComponent("has at least"),
				new StatementComponent(fitters.OnlyNumbers),
				new StatementComponent("gold")
			};
		}
		
		
		public override string GetCode(params string[] args)
		{
			if (args.Length != parameterCount) {
				throw new ArgumentException("Must pass exactly " + parameterCount + " parameters.","args");
			}
			
			return String.Format("GetGold({0}) >= {1}",args);
		}
		
		
		public override string GetNaturalLanguage(params string[] args)
		{
			if (args.Length != parameterCount) {
				throw new ArgumentException("Must pass exactly " + parameterCount + " parameters.","args");
			}
			
			if (args[0] == "some number") args[0] = "some number of";
			
			return String.Format("{0} currently owns at least {1} gold pieces",args);
		}
		
		
		public override StatementBehaviour DeepCopy()
		{
			return new HasGold();
		}
	}
}