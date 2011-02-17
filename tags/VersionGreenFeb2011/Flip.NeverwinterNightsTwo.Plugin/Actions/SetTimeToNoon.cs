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
 * This file added by Keiron Nicholson on 18/05/2010 at 17:24.
 */

using System;
using System.Collections.Generic;
using System.Text;
using Sussex.Flip.Utils;
using Sussex.Flip.UI;

namespace Sussex.Flip.Games.NeverwinterNightsTwo
{
	public class SetTimeToNoon : Nwn2StatementBehaviour
	{	
		// Set the time to the time specified.
		// - nHour should be from 0 to 23 inclusive
		// - nMinute should be from 0 to 59 inclusive
		// - nSecond should be from 0 to 59 inclusive
		// - nMillisecond should be from 0 to 999 inclusive
		// 1) Time can only be advanced forwards; attempting to set the time backwards
		//    will result in the day advancing and then the time being set to that
		//    specified, e.g. if the current hour is 15 and then the hour is set to 3,
		//    the day will be advanced by 1 and the hour will be set to 3.
		// 2) If values larger than the max hour, minute, second or millisecond are
		//    specified, they will be wrapped around and the overflow will be used to
		//    advance the next field, e.g. specifying 62 hours, 250 minutes, 10 seconds
		//    and 10 milliseconds will result in the calendar day being advanced by 2
		//    and the time being set to 18 hours, 10 minutes, 10 milliseconds.
		// void SetTime(int nHour,int nMinute,int nSecond,int nMillisecond);
				
		/// <summary>
		/// Checks whether the given statement is an asynchronous action (that is,
		/// it does not wait for the action to finish before executing the next command).
		/// </summary>
		public override bool IsAsynchronous { // DoesNotWaitForActionToFinishBeforeProceeding
			get { return false; }
		}
		
		
		public SetTimeToNoon()
		{
			statementType = StatementType.Action;
			parameterCount = 0;
			components = new List<StatementComponent>(1) 
			{ 
				new StatementComponent("turn clock to daytime"),
			};
		}
		
		
		public override string GetCode(params string[] args)
		{
			if (args.Length != parameterCount) {
				throw new ArgumentException("Must pass exactly " + parameterCount + " parameters.","args");
			}	
						
			return "SetTime(12,0,0,0);";
		}
		
		
		public override string GetNaturalLanguage(params string[] args)
		{
			if (args.Length != parameterCount) {
				throw new ArgumentException("Must pass exactly " + parameterCount + " parameters.","args");
			}
			
			return "the sky turns to daytime";
		}
		
		
		public override StatementBehaviour DeepCopy()
		{
			return new SetTimeToNoon();
		}
	}
}