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
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Sussex.Flip.Analysis
{
	/// <summary>
	/// Description of LogReader.
	/// </summary>
	public class LogReader
	{
		protected DateTimeParser parser;
		protected string[] newLine = new string[] { Environment.NewLine };
		
		
		public LogReader()
		{
			parser = new DateTimeParser();
		}
		
		
		public string GetFileContents(string path)
		{
			if (!File.Exists(path)) throw new ArgumentException("No file at path '" + path + "'.","path");
					
			string log;
			
			using (StreamReader reader = new StreamReader(path)) {
				log = reader.ReadToEnd();
			}
			
			return log;
		}
				
		
		public LogLineCollection GetCollectedLog(List<string> texts)
		{
			if (texts == null) throw new ArgumentNullException("texts");
						
			LogLineCollection all = new LogLineCollection();
						
			foreach (string text in texts) {
				
				LogLineCollection c = LogLineCollection.GetLogLineCollection(text);
				
				all.AddRange(c);
			}
			
			all.Sort();
			
			return all;
		}
	}
}
