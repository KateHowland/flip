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
 * This file added by Keiron Nicholson on 17/05/2010 at 11:01.
 */

using System;
using System.Collections.Generic;
using Sussex.Flip.Utils;
using Sussex.Flip.UI;

namespace Sussex.Flip.Games.NeverwinterNightsTwo
{
	public class EndGame : Nwn2StatementBehaviour
	{	
		// End the currently running game, play sEndMovie then return all players to the
		// game's main menu.
		// void EndGame(string sEndMovie);
		
		/// <summary>
		/// HACK:
		/// Temp constructor for deserialising. 
		/// </summary>
		public EndGame() : this(new Nwn2Fitters())
		{
			
		}
		
		
		public EndGame(Nwn2Fitters fitters) : base(fitters)
		{
			statementType = StatementType.Action;
			parameterCount = 0;
			components = new List<StatementComponent>(1) 
			{ 
				new StatementComponent("end game"),
			};
		}
		
		
		public override string GetCode(params string[] args)
		{
			if (args.Length != parameterCount) {
				throw new ArgumentException("Must pass exactly " + parameterCount + " parameters.","args");
			}			
			
			string sEndMovie = "\"credits\"";
			return String.Format("EndGame({0});",sEndMovie);
		}
		
		
		public override string GetNaturalLanguage(params string[] args)
		{
			if (args.Length != parameterCount) {
				throw new ArgumentException("Must pass exactly " + parameterCount + " parameters.","args");
			}
			
			return "the game ends";
		}
		
		
		public override StatementBehaviour DeepCopy()
		{
			return new EndGame(fitters);
		}
	}
}