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
 * This file added by Keiron Nicholson on 14/12/2010 at 21:12
 */

using System;
using OEIShared.Utils;

namespace Sussex.Flip.Games.NeverwinterNightsTwo.Utils
{
	/// <summary>
	/// Description of Nwn2Strings.
	/// </summary>
	public static class Nwn2Strings
	{
		public static OEIExoLocString GetOEIStringFromString(string text)
		{			
			OEIExoLocString str = new OEIExoLocString();
			OEIExoLocSubString substr = new OEIExoLocSubString();
			substr.Value = text;
			str.Strings.Add(substr);
			return str;
		}
		
		
		public static string GetStringFromOEIString(OEIExoLocString text)
		{
			if (text.Strings.Count == 0) {
				return String.Empty;
			}
			else {
				return text.Strings[0].Value;
			}
		}
	}
}
