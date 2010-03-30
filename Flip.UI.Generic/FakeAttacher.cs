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
 * This file added by Keiron Nicholson on 10/03/2010 at 11:37.
 */

using System;
using Sussex.Flip.Core;

namespace Sussex.Flip.UI.Generic
{
	/// <summary>
	/// Implements the FlipAttacher interface
	/// but provides no functionality.
	/// </summary>
	public class FakeAttacher : FlipAttacher
	{
		#region Fields
		
		protected GameInformation game;
		
		#endregion
		
		#region Properties
		
		/// <summary>
		/// Provides information about the target game.
		/// </summary>
		public override GameInformation Game { 
			get { return game; }
		}
		
		#endregion
		
		#region Constructors
		
		public FakeAttacher(FlipTranslator translator) : base(translator)
		{
			this.game = new GameInformation("GoodGame");
		}
		
		#endregion
		
		#region Methods
				
		/// <summary>
		/// Translates Flip source into the language of the target game, compiles
		/// the generated code if necessary, and attaches the results to a
		/// module or level of that game.
		/// </summary>
		/// <param name="source">The Flip source to be compiled.</param>
		/// <param name="eventName">The event to attach the source to. HACK.</param>
		public override void Attach(FlipScript source, string eventName) //HACK
		{			
		}
		
		#endregion
	}
}
