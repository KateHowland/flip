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
using System.Windows;

namespace Sussex.Flip.UI
{
	/// <summary>
	/// Description of FlipStatsDictionary.
	/// </summary>
	public class ScriptStats
	{
		private int line, and, or, not, numberBlock, stringBlock, ifthen, ifthenelse, whileloop, dowhileloop, objectblock, action, condition, eventBlock = 0;
		private Dictionary<string,int> actions = new Dictionary<string,int>();		
		private Dictionary<string,int> conditions = new Dictionary<string,int>();		
		private Dictionary<string,int> events = new Dictionary<string,int>();
		
		
		public Dictionary<string,int> Actions {
			get { return actions; }
		}
		public Dictionary<string,int> Conditions {
			get { return conditions; }
		}
		public Dictionary<string,int> Events {
			get { return events; }
		}
		
		
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
			get {
				int count = 0;
				foreach (string key in actions.Keys) count += actions[key];
				return count;
			}
		}
		
		public int Condition {
			get {
				int count = 0;
				foreach (string key in conditions.Keys) count += conditions[key];
				return count;
			}
		}
		
		public int EventBlock {
			get {
				int count = 0;
				foreach (string key in events.Keys) count += events[key];
				return count;
			}
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
			
			Add(actions,stats.actions);
			Add(conditions,stats.conditions);
			Add(events,stats.events);
		}
		
		
		public static void Add(Dictionary<string,int> d1, Dictionary<string,int> d2)
		{
			if (d1 == null) throw new ArgumentNullException("d1");
			if (d2 == null) throw new ArgumentNullException("d2");
			
			foreach (string key in d2.Keys) {
				if (!d1.ContainsKey(key)) d1.Add(key,d2[key]);
				else d1[key] += d2[key];
			}			
		}
									
		
		public void Add(ObjectBlock block)
		{
			if (block == null) throw new ArgumentNullException("block");
			objectblock++; // could replace by discriminating between object block types
		}
		
		
		public void Add(Statement statement)
		{
			if (statement == null) throw new ArgumentNullException("statement");
						
			Dictionary<string,int> dict = (statement.StatementType == StatementType.Action) ? actions : conditions;
			
			string name = statement.Behaviour.GetType().Name;
			
			if (!dict.ContainsKey(name)) dict.Add(name,1);
			else dict[name]++;
		}	
		
		
		public void Add(TriggerControl trigger)
		{
			if (trigger == null) throw new ArgumentNullException("trigger");
			
			string name = trigger.GetType().Name;
			
			if (!events.ContainsKey(name)) events.Add(name,1);
			else events[name]++;
		}
		
		
		public void Add(ConditionalFrame frame)
		{
			if (frame == null) throw new ArgumentNullException("frame");
			
			string name = "DialogueConditionalCheck";
						
			if (!events.ContainsKey(name)) events.Add(name,1);
			else events[name]++;
		}
		
		
		public override string ToString()
		{
			return GetLongOutput(false);
		}
		
		
		public string GetDictionaryOutput(Dictionary<string,int> dict, bool showAll)
		{
			if (dict == null) throw new ArgumentNullException("dict");
			
			System.Text.StringBuilder s = new System.Text.StringBuilder();
			
			foreach (string key in dict.Keys) {
				int val = dict[key];
				if (showAll || val > 0) s.AppendLine(String.Format("{0}: {1}",key,val));
			}
			
			return s.ToString();
		}
		
		
		public string GetActionsOutput(bool showAll)
		{
			return GetDictionaryOutput(actions,showAll);
		}
		
		
		public string GetConditionsOutput(bool showAll)
		{
			return GetDictionaryOutput(conditions,showAll);
		}
		
		
		public string GetEventsOutput(bool showAll)
		{
			return GetDictionaryOutput(events,showAll);
		}
		
		
		public string GetShortOutput()
		{
			return String.Format("{0} lines long, with {1} action(s), {2} condition(s) and {3} boolean(s).",Line,Action,Condition,Boolean);
		}
		
		
		public string GetLongOutput(bool showAll)
		{
			System.Text.StringBuilder s = new System.Text.StringBuilder();
			
			if (showAll) s.AppendLine(GetBlocksOutput(showAll));
			else s.AppendLine(GetShortOutput());
			s.AppendLine();
			
			s.AppendLine("-Actions-");
			if (Actions.Keys.Count == 0) s.AppendLine("None." + Environment.NewLine);
			else s.AppendLine(GetActionsOutput(showAll));
			
			s.AppendLine("-Conditions-");
			if (Conditions.Keys.Count == 0) s.AppendLine("None." + Environment.NewLine);
			else s.AppendLine(GetConditionsOutput(showAll));
			
			s.AppendLine("-Events-");
			if (Events.Keys.Count == 0) s.AppendLine("None.");
			else s.AppendLine(GetEventsOutput(showAll));
			
			return s.ToString();
		}
		
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="showAll">True to list the value of every field; false
		/// to only list fields with values greater than zero.</param>
		/// <returns></returns>
		public string GetBlocksOutput(bool showAll)
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
