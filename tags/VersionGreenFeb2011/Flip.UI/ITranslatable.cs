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
 * This file added by Keiron Nicholson on 30/03/2010 at 14:39.
 */

using System;
using System.Collections.Generic;

namespace Sussex.Flip.UI
{
	/// <summary>
	/// Description of ITranslatable.
	/// </summary>
	public interface ITranslatable
	{
		// TODO: this probably needs to take in the FlipTranslator object
		// so that Moveable is actually doing 'return translator.GetCode(this)'...
		// but FlipTranslator is currently in a different project
		// so the structure of the solution will need reworked first.
		// Good enough for now.
		string GetCode();
		string GetNaturalLanguage();
		
		/// <summary>
		/// Check whether this Flip component has all essential fields filled in,
		/// including those belonging to subcomponents, such that it can generate valid code.
		/// </summary>
		/// <returns>True if all essential fields have been given values; false otherwise.</returns>
		/// <remarks>Note that this method makes no attempt to judge whether the values
		/// are valid in their slots, only that those slots have been filled.</remarks>
		bool IsComplete { get; }
		
		ScriptStats GetStatistics();
	}
}
