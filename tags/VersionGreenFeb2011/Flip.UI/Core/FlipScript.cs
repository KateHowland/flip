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
 * This file added by Keiron Nicholson on 29/07/2009 at 13:23.
 */
 
using System;
using Sussex.Flip.Utils;
		
namespace Sussex.Flip.Core
{
	/// <summary>
	/// Represents a script written in Flip.
	/// </summary>
	public class FlipScript : IDeepCopyable<FlipScript>
	{		
		#region Fields
		
		protected string code;
		protected string name;
		protected ScriptType scriptType;
				
		#endregion
		
		#region Properties
		
		/// <summary>
		/// The source code of the script.
		/// </summary>
		public string Code {
			get { return code; }
			set { code = value; }
		}
		
		/// <summary>
		/// The name of the script.
		/// </summary>
		public string Name {
			get { return name; }
			set { name = value; }
		}
		
		/// <summary>
		/// The type of the script.
		/// </summary>
		public ScriptType ScriptType {
			get { return scriptType; }
			set { scriptType = value; }
		}
						
		#endregion
		
		#region Constructors
				
		/// <summary>
		/// Constructs a new <see cref="FlipScript"/> instance.
		/// </summary>
		public FlipScript() : this(String.Empty,ScriptType.Standard,String.Empty)
		{
		}
		
		
		/// <summary>
		/// Constructs a new <see cref="FlipScript"/> instance.
		/// </summary>
		/// <param name="code">The source code of the script.</param>
		/// <param name="name">The name of the script.</param>
		public FlipScript(string code, ScriptType scriptType, string name)
		{
			this.code = code;
			this.scriptType = scriptType;
			this.name = name;
		}
		
		#endregion	
		
		#region Methods
		
		public override string ToString()
		{
			return String.Format("{0} ({1})",Name,Code);
		}
		
		
		public FlipScript DeepCopy()
		{
			return new FlipScript(code,scriptType,name);
		}
		
		#endregion
	}
}
