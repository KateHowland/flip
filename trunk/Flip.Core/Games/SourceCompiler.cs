/*
 * Flip - a visual programming language for scripting video games
 * Copyright (C) 2009 University of Sussex
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
 * This file added by Keiron Nicholson on 29/07/2009 at 10:53.
 */
 
using System;

namespace Sussex.Flip.Games
{
	/// <summary>
	/// Provides the ability to compile code from some input language
	/// into the scripting language of a target game, and attach the results 
	/// to the appropriate point in a user-created module or level of that game.
	/// </summary>
	public abstract class SourceCompiler
	{
		#region Properties
		
		/// <summary>
		/// The name of the game software which compiled scripts will be used with.
		/// </summary>
		public abstract string Game { get; }
		
		/// <summary>
		/// The name of the language which will be compiled to the target language.
		/// </summary>
		public abstract string InputLanguage { get; }
		
		/// <summary>
		/// The name of the scripting language used by the targeted game.
		/// </summary>
		public abstract string TargetLanguage { get; }
		
		#endregion
		
		#region Constructors
		
		/// <summary>
		/// Constructs a new <see cref="SourceCompiler"/> instance.
		/// </summary>
		public SourceCompiler()
		{			
		}
		
		#endregion
		
		#region Methods
		
		/// <summary>
		/// Compiles code written in the input language of the
		/// compiler to the scripted language of the target game, and
		/// attaches the results to the appropriate point in a user-created 
		/// module or level of that game.
		/// </summary>
		/// <param name="script">An object representing a script written in the 
		/// input language.</param>
		/// <param name="target">An object representing the game module the 
		/// script is to be attached to, or some segment of it.</param>
		public abstract void CompileAndAttach(object script, object target);
		
		#endregion
	}
}
