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
	/// <summary>
	/// Contains logic about whether a given Moveable meets
	/// some set of criteria.
	/// </summary>
	/// <remarks>Rename CriteriaChecker?</remarks>
	public abstract class Fitter
	{
		/// <summary>
		/// Gets whether a given Moveable meets the criteria
		/// embodied by this Fitter.
		/// </summary>
		/// <param name="moveable">The Moveable to check.</param>
		/// <returns>True if the given Moveable meets the
		/// criteria; false otherwise.</returns>
		public abstract bool Fits(Moveable moveable);
		
		
		/// <summary>
		/// Gets a natural language description for the type(s)
		/// of Moveable which will be accepted by this Fitter.
		/// </summary>
		/// <returns>A text string describing acceptable
		/// Moveables.</returns>
		public abstract string GetMoveableDescription();
		
		
		/// <summary>
		/// Gets whether a given Moveable represents an 'action' statement.
		/// </summary>
		/// <param name="moveable">The Moveable to check.</param>
		/// <returns>True if the Moveable is an 'action' statement; false otherwise.</returns>
		public static bool IsAction(Moveable moveable)
		{
			Statement statement = moveable as Statement;
			return statement != null && statement.StatementType == StatementType.Action;
		}
		
		
		/// <summary>
		/// Gets whether a given Moveable represents a boolean expression.
		/// </summary>
		/// <param name="moveable">The Moveable to check.</param>
		/// <returns>True if the Moveable is a boolean expression; false otherwise.</returns>
		public static bool IsBooleanExpression(Moveable moveable)
		{
			Statement statement = moveable as Statement;
			return statement != null && statement.StatementType == StatementType.Condition;
		}
		
		
		/// <summary>
		/// Gets whether a given Moveable represents a conditional construct.
		/// </summary>
		/// <param name="moveable">The Moveable to check.</param>
		/// <returns>True if the Moveable is a conditional construct; false otherwise.</returns>
		public static bool IsConditionalConstruct(Moveable moveable)
		{
			ConditionalControl conditionalConstruct = moveable as ConditionalControl;
			return conditionalConstruct != null;
		}
		
		
		/// <summary>
		/// Gets whether a given Moveable represents an event.
		/// </summary>
		/// <param name="moveable">The Moveable to check.</param>
		/// <returns>True if the Moveable is an event; false otherwise.</returns>
		public static bool IsEvent(Moveable moveable)
		{
			EventBlock eventBlock = moveable as EventBlock;
			return eventBlock != null;
		}
		
		
		/// <summary>
		/// Gets whether a given Moveable is an ObjectBlock.
		/// </summary>
		/// <param name="moveable">The Moveable to check.</param>
		/// <returns>True if the Moveable is an ObjectBlock; false otherwise.</returns>
		public static bool IsObjectBlock(Moveable moveable)
		{
			ObjectBlock objectBlock = moveable as ObjectBlock;
			return objectBlock != null;
		}
		
		
		/// <summary>
		/// Gets whether a given Moveable represents a number value or variable.
		/// </summary>
		/// <param name="moveable">The Moveable to check.</param>
		/// <returns>True if the Moveable is a number; false otherwise.</returns>
		public static bool IsNumber(Moveable moveable)
		{
			NumberBlock numberBlock = moveable as NumberBlock;
			return numberBlock != null;
		}
	}
}
