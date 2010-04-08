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
using NWN2Toolset.NWN2.Data;
using Sussex.Flip.Core;
using Sussex.Flip.Games.NeverwinterNightsTwo.Utils;

namespace Sussex.Flip.Games.NeverwinterNightsTwo
{
	/// <summary>
	/// Translates Flip source code into NWScript (for NWN2),
	/// compiles the generated script, and attaches the result
	/// to a user-created Neverwinter Nights 2 module. 
	/// </summary>
	public class NWScriptAttacher : FlipAttacher
	{
		#region Fields
		
		protected GameInformation game;
		protected INwn2Session session;
		
		#endregion
		
		#region Properties
		
		public override GameInformation Game {
			get { return game; }
		}
		
		#endregion
		
		#region Constructors
		
		/// <summary>
		/// Constructs a new <see cref="NWScriptAttacher"/> instance.
		/// </summary>
		/// <param name="translator">The translator which will
		/// be used to translate scripts before attaching them.</param>
		/// <param name="session">The helper class used for creating,
		/// compiling and attaching scripts.</param>
		public NWScriptAttacher(FlipTranslator translator, INwn2Session session) : base(translator)
		{
			if (session == null) throw new ArgumentNullException("session");
			
			this.game = new GameInformation("Neverwinter Nights 2");
			this.session = session;
		}
		
		#endregion
		
		#region Methods
		
		/// <summary>
		/// Translates Flip source into NWScript, compiles it, 
		/// and attaches the results to a Neverwinter Nights 2 module.
		/// </summary>
		/// <param name="source">The Flip source to be compiled.</param>
		/// <param name="eventName">The event to attach the source to. HACK.</param>
		public override void Attach(FlipScript source, string eventName) //HACK
		{
			if (source == null) throw new ArgumentNullException("source");
			if (eventName == null) throw new ArgumentNullException("eventName");//HACK
			if (eventName == String.Empty) throw new ArgumentException("eventName must not be empty.","eventName");//HACK
			if (!Nwn2ToolsetFunctions.ToolsetIsOpen()) throw new InvalidOperationException("Toolset must be open to attach scripts.");
			
			NWN2GameModule module = session.GetModule();
			if (module == null) {
				throw new InvalidOperationException("No module is currently open in the toolset.");
			}
						
			try {			
				string name = GetUnusedScriptName(source.Name);
					
				NWN2GameScript script = session.AddScript(name,source.Code);
				
				session.CompileScript(script);
				
				// TODO:
				// extract the slot and attaching object information from the script.
				
				// TODO:
				// temp:
				session.AttachScriptToModule(script,eventName);
			}
			catch (Exception e) {
				throw new ApplicationException("Failed to translate and attach script.\n\n" + e.ToString(),e);
			}
		}
		
		
		/// <summary>
		/// Get a script name which has not already been taken in the current module.
		/// </summary>
		/// <param name="preferred">The preferred name for the script,
		/// which will be amended based on availability.</param>
		/// <returns>An available script name.</returns>
		public string GetUnusedScriptName(string preferred)
		{
			if (preferred == null) throw new ArgumentNullException("preferred");
			
			string name = preferred;			
			int count = 2;
			
			while (session.HasUncompiled(name)) {
				name = preferred + " (" + count++ + ")";
			}
			
			return name;
		}
		
		#endregion
	}
}
