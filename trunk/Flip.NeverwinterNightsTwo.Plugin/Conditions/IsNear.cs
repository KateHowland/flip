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
 * This file added by Keiron Nicholson on 21/05/2010 at 17:51.
 */

using System;
using System.Collections.Generic;
using Sussex.Flip.UI;

namespace Sussex.Flip.Games.NeverwinterNightsTwo
{
	public class IsNear : Nwn2StatementBehaviour
	{	
		// * Returns TRUE if oObject is a valid object.
		// int GetIsObjectValid(object oObject);
		
		// Get the distance in metres between oObjectA and oObjectB.
		// * Return value if either object is invalid: 0.0f
		// float GetDistanceBetween(object oObjectA, object oObjectB);
				
		/// <summary>
		/// Checks whether the given statement is an asynchronous action (that is,
		/// it does not wait for the action to finish before executing the next command).
		/// </summary>
		public override bool IsAsynchronous { // DoesNotWaitForActionToFinishBeforeProceeding
			get { return false; }
		}
		
		
		public IsNear()
		{
			statementType = StatementType.Condition;
			parameterCount = 3;
			components = new List<StatementComponent>(5) 
			{ 
				new StatementComponent(fitters.OnlyInstancesOrPlayers),
				new StatementComponent("is within"),
				new StatementComponent(fitters.OnlyNumbers),
				new StatementComponent("metres of"),
				new StatementComponent(fitters.OnlyInstancesOrPlayers)
			};
		}
		
		
		public override string GetCode(params string[] args)
		{
			if (args.Length != parameterCount) {
				throw new ArgumentException("Must pass exactly " + parameterCount + " parameters.","args");
			}
			
			string fDistance = String.Format("{0}.0f",args[1]);
			return String.Format("GetIsObjectValid({0}) && GetIsObjectValid({2}) && (GetDistanceBetween({0},{2}) <= {1})",args[0],fDistance,args[2]);
		}
		
		
		public override string GetNaturalLanguage(params string[] args)
		{
			if (args.Length != parameterCount) {
				throw new ArgumentException("Must pass exactly " + parameterCount + " parameters.","args");
			}
			
			if (args[1] == "some number") args[1] = "some number of";
			
			return String.Format("{0} is within {1} metres of {2} (assuming both exist)",args);
		}
		
		
		public override StatementBehaviour DeepCopy()
		{
			return new IsNear();
		}
	}
}