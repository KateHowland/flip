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
 * This file added by Keiron Nicholson on 29/07/2009 at 10:37.
 */
 
using System;
using System.Collections.Generic;
using NWN2Toolset.NWN2.Data;
using NWN2Toolset.NWN2.Data.Templates;
using Sussex.Flip.Games;
using Sussex.Flip.Language;
using Sussex.Flip.Games.NeverwinterNightsTwo.Utils;

namespace Sussex.Flip.Games.NeverwinterNightsTwo
{
	/// <summary>
	/// Provides the ability to compile Flip source code into NWScript (NWN2)
	/// and attach the results to a Neverwinter Nights 2 module.
	/// </summary>
	public class Nwn2FlipCompiler : FlipCompiler
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
		/// Constructs a new <see cref="NeverwinterNightsTwoFlipCompiler"/> instance.
		/// </summary>
		public Nwn2FlipCompiler()
		{
			game = "Neverwinter Nights 2";
			targetLanguage = "NWScript (NWN2)";
		}
		
		#endregion
			
		#region Methods
		
		/// <summary>
		/// Compiles Flip code to the scripted language of the target game, and
		/// attaches the results to the appropriate point in a user-created 
		/// module or level of that game.
		/// </summary>
		/// <param name="script">The Flip script to be attached.</param>
		/// <param name="target">An object representing the game level, area or 
		/// module it is to be attached to.</param>
		public override void CompileAndAttach(FlipScript script, object target)
		{
			NWN2GameArea area = target as NWN2GameArea;
			
			if (area == null) throw new ArgumentException("Target must be an instance of class NWN2GameArea.","target");
			
			CompileAndAttach(script,area);
		}
		
		
		/// <summary>
		/// Compiles Flip code to the scripted language of the target game, and
		/// attaches the results to the appropriate point in a user-created 
		/// module or level of that game.
		/// </summary>
		/// <param name="script">The Flip script to be attached.</param>
		/// <param name="area">An area in a Neverwinter Nights 2 module
		/// to which a compiled version of this script is to be attached.</param>
		public void CompileAndAttach(FlipScript script, string area)
		{	
			Nwn2Type type;
			string tag;
			string eventName;
			
			GetTrigger(script,out type,out tag,out eventName);
			
			// 2. Ask the Flip plugin to return.....
			
			// X. Problem: Can't actually send the NWN2GameModule/NWN2GameArea
			// /anything else objects between processes, I don't think.
		}
		
		
		/// <summary>
		/// Parses a FlipScript instance and identifies the 
		/// object and event it is to be attached to.
		/// </summary>
		/// <param name="script">The Flip script to be parsed.</param>
		/// <param name="type">The type of the object which defines
		/// the event the script is to be attached to.</param>
		/// <param name="tag">The tag of the object which defines
		/// the event the script is to be attached to.</param>
		/// <param name="eventName">The name of the event the script
		/// is to be attached to.</param>
		/// <remarks>If the script is to be attached to a module, 
		/// 'tag' will contain the name of that module. The same 
		/// applies if the script is to be attached to an area.
		/// </remarks>
		public static void GetTrigger(FlipScript script, 
		        		              out Nwn2Type type, 
		                		      out string tag, 
		                       		  out string eventName)
		{
			throw new NotImplementedException();
		}
			
		#endregion
	}
}