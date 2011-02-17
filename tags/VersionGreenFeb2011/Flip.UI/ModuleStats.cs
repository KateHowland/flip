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
 * This file added by Keiron Nicholson on 08/02/2011 at 14:45
 */

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Sussex.Flip.UI
{
	/// <summary>
	/// Description of ModuleStats.
	/// </summary>
	public class ModuleStats
	{
		private string name;		
		private int attachedScripts;
		private ScriptStats stats;
		
		public string Name {
			get { return name; }
			set { name = value; }
		}
		
		public int AttachedScripts {
			get { return attachedScripts; }
			set { attachedScripts = value; }
		}
		
		public ScriptStats CumulativeStats {
			get { return stats; }
			set { stats = value; }
		}		
					
		
		public ModuleStats()
		{
			name = String.Empty;
			attachedScripts = 0;
			stats = new ScriptStats();
		}
		
		
		public void Add(ModuleStats ms)
		{
			attachedScripts += ms.AttachedScripts;
			Add(ms.CumulativeStats);
		}
		
		
		public void Add(ScriptStats ss)
		{
			stats.Add(ss);
		}
		
		
		public override string ToString()
		{
			return String.Format("'{0}': {1} scripts attached. In total: {2}",Name,AttachedScripts,CumulativeStats);
		}
		
		
		public List<object> GetSpreadsheetRows()
		{
			List<object> rowNames = new List<object>();
			rowNames.Add("Module");
			rowNames.Add(String.Empty);
			
			rowNames.Add("Scripts");
			rowNames.Add(String.Empty);
			rowNames.AddRange(CumulativeStats.GetSpreadsheetRows());
			
			return rowNames;
		}
		
		
		public List<object> GetSpreadsheetData()
		{
			List<object> data = new List<object>();
			data.Add(name);
			data.Add(String.Empty);
			
			data.Add(attachedScripts);
			data.Add(String.Empty);
			data.AddRange(CumulativeStats.GetSpreadsheetData());
			
			return data;
		}
	}
}
