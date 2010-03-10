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
 * This file added by Keiron Nicholson on 30/10/2009 at 14:25.
 */

using System;

namespace Sussex.Flip.Games.NeverwinterNightsTwo
{
	/// <summary>
	/// Provides sample scripts for use in testing.
	/// </summary>
	public class SampleScriptProvider
	{
		#region Fields
		
		private string sing;	
		private string giveGold;	
		private string changeName;	
		private string brokenScript;
		
		#endregion
		
		#region Properties
		
		/// <summary>
		/// A script which causes the subject to sing
		/// '99 bottles of beer on the wall', not particularly
		/// tunefully.
		/// </summary>
		public string Sing {
			get { return sing; }
		}
		
			
		/// <summary>
		/// A script which gives the player 100 pieces of gold.
		/// </summary>
		public string GiveGold {
			get { return giveGold; }
		}
		
		
		/// <summary>
		/// A script which changes the first name of the subject.
		/// </summary>
		public string ChangeName {
			get { return changeName; }
		}
		
		
		/// <summary>
		/// An illegal version of the 'Sing' script which
		/// does not compile.
		/// </summary>
		public string BrokenScript {
			get { return brokenScript; }
		}
		
		#endregion
		
		#region Constructors
					
		/// <summary>
		/// Constructs a new <see cref="SampleScriptProvider"/> instance.
		/// </summary>
		public SampleScriptProvider()
		{
			sing = "void main() { int i = 99; for (i = 99; i > 0; i--) { " +
				   "string current = IntToString(i); string next = IntToString(i-1);" + 
				   "ActionSpeakString(\"\" + current + \" bottles of beer " + 
				   "on the wall, \" + current + \" bottles of beer, if one of the bottles " +
				   "should happen to fall, \" + next + \" bottles of beer on the wall!\"); ActionWait(3.0f); } }";
			giveGold = "void main() { GiveGoldToCreature(GetFirstPC(),100); }";
			changeName = "void main() { SetFirstName(OBJECT_SELF,\"Brian\"); }";
			brokenScript = sing + "}";
		}
		
		#endregion
	}
}
