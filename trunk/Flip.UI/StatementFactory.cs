/*
 * Flip - a visual programming language for scripting video games
 * Copyright (C) 2009 University of Sussex
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
 * This file added by Keiron Nicholson on 03/02/2010 at 12:56.
 */

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Sussex.Flip.UI
{
	/// <summary>
	/// TODO.
	/// Description of AbstractStatementFactory.
	/// </summary>
	public abstract class AbstractStatementFactory
	{
		public abstract List<Statement> GetStatements();
		
		public abstract Statement Attacks();	
		public abstract Statement PicksUp();	
		public abstract Statement Drops();	
		public abstract Statement Grows();	
		public abstract Statement Shrinks();		
	}	
	
	
	/// <summary>
	/// TODO.
	/// Description of StatementFactory.
	/// </summary>
	public class StatementFactory : AbstractStatementFactory
	{
		public override List<Statement> GetStatements()
		{
			MethodInfo[] methods = GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance);
			List<Statement> statements = new List<Statement>(methods.Length-1);
			
			foreach (MethodInfo mi in methods) {
				if (mi.ReturnType == typeof(Statement)) {
					statements.Add((Statement)mi.Invoke(this,null));
				}
			}
			
			return statements;
		}
		
		
		public override Statement Attacks()
		{			
			Statement statement = new Statement();
			statement.AddNounSlot("creature1");
			statement.AddTextBar("attacks");
			statement.AddNounSlot("creature2");
			return statement;
		}
		
		
		public override Statement PicksUp()
		{			
			Statement statement = new Statement();
			statement.AddNounSlot("creature1");
			statement.AddTextBar("picks up");
			statement.AddNounSlot("item1");
			return statement;
		}	
		
		
		public override Statement Drops()
		{			
			Statement statement = new Statement();
			statement.AddNounSlot("creature1");
			statement.AddTextBar("drops");
			statement.AddNounSlot("item1");
			return statement;
		}	
		
		
		public override Statement Grows()
		{			
			Statement statement = new Statement();
			statement.AddNounSlot("creature1");
			statement.AddTextBar("grows");
			return statement;
		}	
		
		
		public override Statement Shrinks()
		{			
			Statement statement = new Statement();
			statement.AddNounSlot("creature1");
			statement.AddTextBar("shrinks");
			return statement;
		}	
	}
}