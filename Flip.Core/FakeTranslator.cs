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
 * This file added by Keiron Nicholson on 29/07/2009 at 10:53.
 */
 
using System;
using System.IO;

namespace Sussex.Flip.Core
{
	/// <summary>
	/// Implements the FlipTranslator interface
	/// but provides no functionality.
	/// </summary>
	public class FakeTranslator : FlipTranslator
	{
		#region Properties
		
		/// <summary>
		/// The name of the language generated by this translator.
		/// </summary>
		public override string TargetLanguage { 
			get { return "GoodLanguage"; }
		}
		
		#endregion
		
		#region Methods
		
		/// <summary>
		/// Translates Flip source into the target language and
		/// returns the results as a string.
		/// </summary>
		/// <param name="script">The Flip source to be translated.</param>
		/// <returns>The source code of the target language.</returns>
		public override string Translate(FlipScript script)
		{
			return script.Code;
		}
		
		
		/// <summary>
		/// Translates Flip source into the target language and 
		/// generates a file containing the results.
		/// </summary>
		/// <param name="script">The Flip source to be translated.</param>
		/// <param name="path">The path at which to generate the output file.</param>
		public override void Translate(FlipScript script, string path)
		{
			WriteToFile(script.Code,path);
		}
				
		
		/// <summary>
		/// Write a text string to a file at a given path.
		/// </summary>
		/// <param name="text">The text to write.</param>
		/// <param name="path">The path to create the file at.</param>
		protected void WriteToFile(string text, string path)
		{
			using (StreamWriter writer = File.CreateText(path)) {
				writer.Write(text);
				writer.Flush();
			}
		}
		
		#endregion
	}
}