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
 * This file added by Keiron Nicholson on 17/05/2010 at 12:20.
 */

using System;
using System.Collections.Generic;
using Sussex.Flip.Utils;
using Sussex.Flip.UI;

namespace Sussex.Flip.Games.NeverwinterNightsTwo
{
	public class JumpsTo : Nwn2StatementBehaviour
	{	
		// Jump to an object ID, or as near to it as possible.
		// void ActionJumpToObject(object oToJumpTo, int bWalkStraightLineToPoint=TRUE);
		
		public JumpsTo()
		{
			statementType = StatementType.Action;
			parameterCount = 2;
			components = new List<StatementComponent>(3) 
			{ 
				new StatementComponent(fitters.OnlyCreaturesOrPlayers),
				new StatementComponent("teleports to"),
				new StatementComponent(fitters.OnlyInstancesOrPlayers)
			};
		}
		
		
		public override string GetCode(params string[] args)
		{
			if (args.Length != parameterCount) {
				throw new ArgumentException("Must pass exactly " + parameterCount + " parameters.","args");
			}	
			
			return String.Format("AssignCommand({0},ActionJumpToObject({1},TRUE));",args);
		}
		
		
		public override string GetNaturalLanguage(params string[] args)
		{
			if (args.Length != parameterCount) {
				throw new ArgumentException("Must pass exactly " + parameterCount + " parameters.","args");
			}
			
			if (args[1] == "something") return String.Format("{0} teleports to some location",args);
			else return String.Format("{0} teleports to the location of {1}",args);
		}
		
		
		public override StatementBehaviour DeepCopy()
		{
			return new JumpsTo();
		}
	}
}