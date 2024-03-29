﻿/*
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
 * This file added by Keiron Nicholson on 01/02/2011 at 15:20
 */

using System;

namespace Sussex.Flip.Analysis
{
	/// <summary>
	/// Description of LogLine.
	/// </summary>
	public class LogLine : IComparable
	{
		DateTime time;		
		string text;		
		static DateTimeParser parser;
		
		
		public DateTime Time {
			get { return time; }
			set { time = value; }
		}
		
		
		public string Text {
			get { return text; }
			set { text = value; }
		}
		
		
		static LogLine()
		{
			parser = new DateTimeParser();
		}
		
		
		public LogLine(DateTime time, string text)
		{
			this.time = time;
			this.text = text;
		}
		
		
		public int CompareTo(object obj)
		{
			LogLine logLine = (LogLine)obj;
			return time.CompareTo(logLine.time);
		}
		
		
		public static LogLine TryGetLogLine(string line)
		{
			if (line == null) return null;
			
			DateTime? time = parser.TryGetDateTime(line);
			if (time == null) return null;
			
			string timeStr = parser.GetDateTimeString(line);
			
			string text;
			try {
				text = line.Remove(0,timeStr.Length+3);
			}
			catch (Exception) {
				text = String.Empty;
			}
			
			return new LogLine(time.Value,text);
		}
		
		
		public override string ToString()
		{
			return time.ToLongTimeString() + ": " + text;
		}
	}
}
