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
 * This file added by Keiron Nicholson on 31/01/2011 at 12:40
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Sussex.Flip.Analysis
{
	/// <summary>
	/// Description of LogLineCollection.
	/// </summary>
	public class LogLineCollection : List<LogLine>
	{
		protected static string[] separator = new string[] { Environment.NewLine };
				
		
		public static LogLineCollection GetLogLineCollection(string log)
		{
			if (log == null) throw new ArgumentNullException("log");
			
			LogLineCollection c = new LogLineCollection();
			
			string[] split = log.Split(separator,StringSplitOptions.RemoveEmptyEntries);
			
			foreach (string s in split) {						
				LogLine logLine = LogLine.TryGetLogLine(s);
				if (logLine != null) c.Add(logLine);				
			}	
			
			return c;
		}
	}
}
