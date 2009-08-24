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
 * This file added by Keiron Nicholson on 29/07/2009 at 10:29.
 */
 
using System;
using System.Collections.Generic;
using Sussex.Flip.Games;
using Sussex.Flip.Language;

namespace Sussex.Flip.Games.NeverwinterNights
{
	/// <summary>
	/// Provides the ability to compile Flip source code into NWScript (NWN1)
	/// and attach the results to a Neverwinter Nights module.
	/// </summary>
	public class Nwn1FlipCompiler : FlipCompiler
	{
		#region Fields
		
		/// <summary>
		/// The name of the game software which compiled scripts will be used with.
		/// </summary>
		protected string game;
		
		/// <summary>
		/// The name of the scripting language used by the targeted game.
		/// </summary>
		protected string targetLanguage;
		
		#endregion
		
		#region Properties
		
		/// <summary>
		/// The name of the game software which compiled scripts will be used with.
		/// </summary>
		public override string Game { 
			get {
				return game;
			}
		}
		
		/// <summary>
		/// The name of the scripting language used by the targeted game.
		/// </summary>
		public override string TargetLanguage { 
			get {
				return targetLanguage;
			}
		}
		
		#endregion
		
		#region Constructors
		
		/// <summary>
		/// Constructs a new <see cref="NeverwinterNightsFlipCompiler"/> instance.
		/// </summary>
		public Nwn1FlipCompiler()
		{
			game = "Neverwinter Nights";
			targetLanguage = "NWScript (NWN1)";
		}
		
		#endregion
	
		#region Methods
		
		public override void CompileAndAttach(FlipScript script, object target)
		{
			throw new NotImplementedException();
		}
		
		#endregion
	}
}