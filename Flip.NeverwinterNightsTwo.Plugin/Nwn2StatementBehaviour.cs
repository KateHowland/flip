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
 * This file added by Keiron Nicholson on 31/03/2010 at 15:33.
 */

using System;
using Sussex.Flip.UI;

namespace Sussex.Flip.Games.NeverwinterNightsTwo
{
	/// <summary>
	/// Description of Nwn2StatementBehaviour.
	/// </summary>
	public abstract class Nwn2StatementBehaviour : StatementBehaviour
	{
		protected static Nwn2Fitters fitters;
		
		
		public static Nwn2Fitters Fitters {
			get { return fitters; }
			set { fitters = value; }
		}
		
		
		static Nwn2StatementBehaviour()
		{
			fitters = new Nwn2Fitters();
		}
		
		
		public Nwn2StatementBehaviour()
		{
		}
		
		
		/// <summary>
		/// Checks whether the given statement is an asynchronous action (that is,
		/// it does not wait for the action to finish before executing the next command).
		/// </summary>
		public abstract bool IsAsynchronous { get; }
	}
}
