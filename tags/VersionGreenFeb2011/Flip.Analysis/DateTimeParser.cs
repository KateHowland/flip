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
 * This file added by Keiron Nicholson on 18/01/2011 at 14:19
 */

using System;

namespace Sussex.Flip.Analysis
{
	/// <summary>
	/// Description of DateTimeParser.
	/// </summary>
	public class DateTimeParser
	{
		// [Thursday Jan 06 12:11:26] launched flip -from toolbar
		// [Sunday Jan 06 12:11:33] exited flip
		
		protected static char[] separators = new char[] { ' ' , ':' };
				
		
		public DateTimeParser()
		{
		}
		
		
		public DateTime GetDateTime(string logStr)
		{
			string dateTimeStr = GetDateTimeString(logStr); // "Sunday Jan 06 12:11:33"
						
			string[] parts = dateTimeStr.Split(separators,StringSplitOptions.None);
			if (parts.Length != 6) throw new ArgumentException("String is invalid.","logStr");
				
			// year, month, day, hour, minute, second
			int year = 1984;
			int month = GetMonth(parts[1]);
			int day = int.Parse(parts[2]);
			int hour = int.Parse(parts[3]);
			int minute = int.Parse(parts[4]);
			int second = int.Parse(parts[5]);
			
			DateTime dateTime = new DateTime(year,month,day,hour,minute,second);
			
			return dateTime;
		}
		
		
		public DateTime? TryGetDateTime(string logStr)
		{
			try {
				return GetDateTime(logStr);
			}
			catch (Exception) {
				return null;
			}
		}
		
		
		public string GetDateTimeString(string logStr)
		{
			if (logStr == null) throw new ArgumentNullException("logStr");
			if (logStr == String.Empty) throw new ArgumentException("String is invalid.","logStr");
			
			int index = logStr.IndexOf(']');
			if (index < 23) throw new ArgumentException("String is invalid.","logStr");
			
			return logStr.Substring(1,index-1);
		}
		
		
		public int GetMonth(string month)
		{
			if (month == null) throw new ArgumentNullException("month");
			
			switch (month) {
				case "Jan":
					return 1;
				case "Feb":
					return 2;
				case "Mar":
					return 3;
				case "Apr":
					return 4;
				case "May":
					return 5;
				case "Jun":
					return 6;
				case "Jul":
					return 7;
				case "Aug":
					return 8;
				case "Sep":
					return 9;
				case "Oct":
					return 10;
				case "Nov":
					return 11;
				case "Dec":
					return 12;
				default:
					throw new ArgumentException("String is invalid.",month);
			}
		}
	}
}
