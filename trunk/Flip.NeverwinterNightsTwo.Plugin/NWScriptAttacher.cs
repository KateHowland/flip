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
		/// and attaches the results to Neverwinter Nights 2 module.
		/// </summary>
		/// <param name="source">The Flip source to be compiled.</param>
		/// <param name="target">A Neverwinter Nights 2 game module.</param>
		/// <remarks>The given module must currently be open in the 
		/// Neverwinter Nights 2 toolset.</remarks>
		public override void Attach(FlipScript source, object target)
		{
			NWN2GameModule module = target as NWN2GameModule;
			
			if (module == null) {
				throw new ArgumentException("Must pass object of type NWN2Toolset.NWN2.Data.NWN2GameModule to attach script.");
			}
			
			Attach(source,module);
		}		
		
		
		/// <summary>
		/// Translates Flip source into NWScript, compiles it, 
		/// and attaches the results to Neverwinter Nights 2 module.
		/// </summary>
		/// <param name="source">The Flip source to be compiled.</param>
		/// <param name="module">A Neverwinter Nights 2 game module.</param>
		/// <remarks>The given module must currently be open in the 
		/// Neverwinter Nights 2 toolset.</remarks>
		public void Attach(FlipScript source, NWN2GameModule module)
		{
			if (source == null) throw new ArgumentNullException("source");
			if (module == null) throw new ArgumentNullException("module");
			if (module != session.GetModule()) {
				throw new InvalidOperationException("The given module is not currently open in the toolset.");
			}
						
			try {
				string translated = translator.Translate(source);	
				
				NWN2GameScript script = session.AddScript(source.Name,source.Code);
				session.CompileScript(script);
				
				// TODO:
				// might not be necessary outside of the test class - it only 
				// was necessary in the first place because of weird behaviour -
				// test it and hopefully we can delete these commented out bits.
				//while (!WaitForCompiledScriptToAppear(script.Name));
				
				// TODO:
				// extract the slot and attaching object information from the script.
				
				// TODO:
				// temp:
				session.AttachScriptToModule(script,"OnPlayerRest");
				foreach (NWN2GameArea area in module.Areas.Values) {
					session.AttachScriptToArea(script,area,"OnClientEnterScript");
				}
			}
			catch (Exception e) {
				throw new ApplicationException("Failed to translate and attach script.",e);
			}
		}
		
		
//		/// <summary>
//		/// Checks for a compiled script with the given name in the current module,
//		/// timing out after a specified time.
//		/// </summary>
//		/// <param name="scriptName">The script name.</param>
//		/// <param name="timeout">The number of milliseconds to check for.</param>
//		/// <returns>True if the compiled script file was found, false otherwise.</returns>
//		protected bool WaitForCompiledScriptToAppear(string scriptName, int timeout)
//		{			
//			int original = timeout;
//			int interval = 1;
//			while (!session.HasCompiled(scriptName) && timeout >= 0) {
//				System.Threading.Thread.Sleep(interval);
//				timeout -= interval;
//			}
//			return session.HasCompiled(scriptName);
//		}
//		
//				
//		/// <summary>
//		/// Checks for a compiled script with the given name in the current module,
//		/// timing out after 1 second.
//		/// </summary>
//		/// <param name="scriptName">The script name.</param>
//		protected bool WaitForCompiledScriptToAppear(string scriptName)
//		{			
//			return WaitForCompiledScriptToAppear(scriptName,1000);
//		}
		
		#endregion
	}
}
