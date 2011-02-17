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
 * This file added by Keiron Nicholson on 01/02/2011 at 15:32
 */

using System;
using System.Collections.Generic;

namespace Sussex.Flip.Analysis
{
	/// <summary>
	/// Description of LogLineComparer.
	/// </summary>
	public class LogLineComparer : IComparer<string>
	{
		public int Compare(LogLine x, LogLine y)
		{
			return x.CompareTo(y);
		}
		
		
		public int Compare(string x, string y)
		{
			LogLine x1 = LogLine.TryGetLogLine(x);
			LogLine y1 = LogLine.TryGetLogLine(y);
			
			if (x1 == null) throw new ArgumentException("The string to be compared was not a LogLine.","x");
			if (y1 == null) throw new ArgumentException("The string to be compared was not a LogLine.","y");
			
			return x1.CompareTo(y1); 
		}
	}
}
