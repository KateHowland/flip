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
 * This file added by Keiron Nicholson on 18/01/2011 at 15:42
 */

using System;
using System.IO;
using System.Text;

namespace Sussex.Flip.Analysis
{
	/// <summary>
	/// Description of LogCombiner.
	/// </summary>
	public class LogCombiner
	{
		protected DateTimeParser parser;
		
		
		public LogCombiner()
		{
			parser = new DateTimeParser();
		}
		
		
		public string Combine(string path1, string path2)
		{
			if (!File.Exists(path1)) throw new ArgumentException("No file at path '" + path1 + "'.","path1");
			if (!File.Exists(path2)) throw new ArgumentException("No file at path '" + path2 + "'.","path2");
			
			StringBuilder combined = new StringBuilder();
			
			int count1 = 0, count2 = 0;
						
			using (StreamReader reader1 = new StreamReader(path1)) {
				using (StreamReader reader2 = new StreamReader(path2)) {
				
					string lineA = reader1.ReadLine();
					string lineB = reader2.ReadLine();
					
					do {
						
						if (lineA != null && lineB != null) {
														
							DateTime dtA = parser.GetDateTime(lineA);
							DateTime dtB = parser.GetDateTime(lineB);
														
							if (dtA.CompareTo(dtB) <= 0) {		
								count1++;						
								combined.AppendLine(lineA);
								lineA = reader1.ReadLine();
							}
							
							else {
								count2++;
								combined.AppendLine(lineB);
								lineB = reader2.ReadLine();
							}
														
						}
						
						else if (lineA != null && lineB == null) {
							count1++;
							combined.AppendLine(lineA);
							lineA = reader1.ReadLine();							
						}
						
						else if (lineB != null && lineA == null) {
							count2++;
							combined.AppendLine(lineB);
							lineB = reader2.ReadLine();					
						}
						
					}
					
					while (lineA != null && lineB != null);
				}
			}
			
			return combined.ToString();
		}
	}
}
