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
	/// Description of FlipStatsDictionary.
	/// </summary>
	public struct ScriptStats
	{
		private int line, and, or, not, numberBlock, stringBlock, ifthen, ifthenelse, whileloop, dowhileloop, objectblock, action, condition, eventBlock;
				
		
		public int Line {
			get { return line; }
			set { line = value; }
		}
		
		public int And {
			get { return and; }
			set { and = value; }
		}
		
		public int Or {
			get { return or; }
			set { or = value; }
		}
		
		public int Not {
			get { return not; }
			set { not = value; }
		}	
		
		public int NumberBlock {
			get { return numberBlock; }
			set { numberBlock = value; }
		}
		
		public int StringBlock {
			get { return stringBlock; }
			set { stringBlock = value; }
		}
		
		public int IfThen {
			get { return ifthen; }
			set { ifthen = value; }
		}
		
		public int IfThenElse {
			get { return ifthenelse; }
			set { ifthenelse = value; }
		}
		
		public int WhileLoop {
			get { return whileloop; }
			set { whileloop = value; }
		}
		
		public int DoWhileLoop {
			get { return dowhileloop; }
			set { dowhileloop = value; }
		}
		
		public int ObjectBlock {
			get { return objectblock; }
			set { objectblock = value; }
		}	
		
		public int Action {
			get { return action; }
			set { action = value; }
		}
		
		public int Condition {
			get { return condition; }
			set { condition = value; }
		}
		
		public int EventBlock {
			get { return eventBlock; }
			set { eventBlock = value; }
		}
		
		public int Boolean {
			get { return And + Or + Not; }
		}
									
		
		public void Add(ScriptStats stats)
		{
			line += stats.line;
			and += stats.and;
			or += stats.or;
			not += stats.not;
			numberBlock += stats.numberBlock;
			stringBlock += stats.stringBlock;
			ifthen += stats.ifthen;
			ifthenelse += stats.ifthenelse;
			whileloop += stats.whileloop;
			dowhileloop += stats.dowhileloop;
			objectblock += stats.objectblock;
			action += stats.action;
			condition += stats.condition;
			eventBlock += stats.eventBlock;
		}
									
		
		public void Add(ObjectBlock block)
		{
			if (block == null) throw new ArgumentNullException("block");
			objectblock++; // could replace by discriminating between object block types
		}
		
		
//		public void (Statement statement)
//		{
//			if (statement == null) throw new ArgumentNullException("statement");
//			
//			if (statement.StatementType == StatementType.Action) {
//				
//			}
//			else if (statement.StatementType == StatementType.Condition) {
//				
//			}
//		}		
		
		
		public override string ToString()
		{
			return GetTidyString();
		}
		
		
		public string GetTidyString()
		{
			return String.Format("{0} lines long, with {1} action(s), {2} condition(s) and {3} boolean(s).",Line,Action,Condition,Boolean);
		}
		
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="showAll">True to list the value of every field; false
		/// to only list fields with values greater than zero.</param>
		/// <returns></returns>
		public string GetString(bool showAll)
		{
			System.Text.StringBuilder s = new System.Text.StringBuilder();
						
			bool first = true;
			
			foreach (PropertyInfo p in typeof(ScriptStats).GetProperties(BindingFlags.Public|BindingFlags.Instance)) {
				if (p.PropertyType == typeof(int)) {
					
					int val = (int)p.GetValue(this,null);
					
					if (showAll || val > 0) {
					
						if (!first) s.Append(", ");
						else first = false;
						
						s.Append(String.Format("{0} ({1})",p.Name,val));
						
					}
				}
			}
			
			return s.ToString();			
		}
	}
}
