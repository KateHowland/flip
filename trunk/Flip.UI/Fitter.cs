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
 * This file added by Keiron Nicholson on 17/02/2010 at 13:11.
 */

using System;

namespace Sussex.Flip.UI
{
	// TODO better name. CriteriaChecker? 
	// MoveableFitter? 
	/// <summary>
	/// Contains logic about whether a given Moveable meets
	/// some set of criteria.
	/// </summary>
	public abstract class Fitter
	{
		/// <summary>
		/// Gets whether a given Moveable meets the criteria
		/// embodied by this Fitter.
		/// </summary>
		/// <param name="moveable">TODO</param>
		/// <returns>TODO</returns>
		public abstract bool Fits(Moveable moveable);
		
		
		/// <summary>
		/// Gets a natural language description for the type(s)
		/// of Moveable which will be accepted by this Fitter.
		/// </summary>
		/// <returns>A text string describing acceptable
		/// Moveables.</returns>
		public abstract string GetMoveableDescription();
		
		
		public static bool IsAction(Moveable moveable)
		{
			Statement statement = moveable as Statement;
			return statement != null && statement.StatementType == StatementType.Action;
		}
		
		
		public static bool IsBooleanExpression(Moveable moveable)
		{
			Statement statement = moveable as Statement;
			return statement != null && statement.StatementType == StatementType.Condition;
		}
		
		
		public static bool IsConditionalConstruct(Moveable moveable)
		{
			ConditionalControl conditionalConstruct = moveable as ConditionalControl;
			return conditionalConstruct != null;
		}
		
		
		public static bool IsEvent(Moveable moveable)
		{
			EventBlock eventBlock = moveable as EventBlock;
			return eventBlock != null;
		}
		
		
		public static bool IsObject(Moveable moveable)
		{
			ObjectBlock objectBlock = moveable as ObjectBlock;
			return objectBlock != null;
		}
	}
}
