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
 * This file added by Keiron Nicholson on 17/02/2010 at 14:37.
 */

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using Sussex.Flip.UI;

namespace Sussex.Flip.Games.NeverwinterNightsTwo
{	
	/// <summary>
	/// TODO.
	/// Description of Nwn2ConditionFactory.
	/// </summary>
	public class Nwn2StatementFactory
	{			
		#region Fields
		
		protected Brush actionBrush;
		protected Brush conditionBrush;
		protected Nwn2Fitters fitters;
		
		#endregion	
		
		#region Constructors
		
		public Nwn2StatementFactory(Nwn2Fitters fitters) : this(fitters,null,null)
		{			
		}
		
		
		public Nwn2StatementFactory(Nwn2Fitters fitters, Brush actionBrush, Brush conditionBrush)
		{
			if (fitters == null) throw new ArgumentNullException("fitters");
			if (actionBrush == null) actionBrush = Brushes.Transparent;
			if (conditionBrush == null) conditionBrush = Brushes.Transparent;
			
			this.fitters = fitters;
			this.actionBrush = actionBrush;
			this.conditionBrush = conditionBrush;
		}
		
		#endregion
		
		#region Methods
				
		public void GetStatements(out List<Statement> statements, out List<Statement> actions, out List<Statement> conditions)
		{
			MethodInfo[] methods = GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance);
			
			statements = new List<Statement>(methods.Length-1);
			int count = methods.Length/2;
			actions = new List<Statement>(count);
			conditions = new List<Statement>(count);			
			
			foreach (MethodInfo method in methods) {
				if (method.ReturnType == typeof(Statement)) {						
					Statement statement = (Statement)method.Invoke(this,null);
					statements.Add(statement);
						
					foreach (object o in method.GetCustomAttributes(true)) {
						if (o is ActionStatementAttribute) {
							actions.Add(statement);
						}
						else if (o is ConditionStatementAttribute) {
							conditions.Add(statement);
						}
					}							
				}
			}
		}
		
		
		public List<Statement> GetStatements()
		{
			List<Statement> statements, actions, conditions;
			GetStatements(out statements, out actions, out conditions);
			return statements;
		}
		
		
		public List<Statement> GetActions()
		{
			List<Statement> statements, actions, conditions;
			GetStatements(out statements, out actions, out conditions);
			return actions;
		}
		
		
		public List<Statement> GetConditions()
		{			
			List<Statement> statements, actions, conditions;
			GetStatements(out statements, out actions, out conditions);
			return conditions;
		}
		
		#region Actions
		
		[ActionStatement("Attacks")]
		public Statement Attacks()
		{			
			Statement statement = new Statement();
			statement.AddSlot(new StatementSlot("creature1",fitters.OnlyCreaturesOrPlayers));
			statement.AddText(new StatementLabel("attacks",actionBrush));
			statement.AddSlot(new StatementSlot("creature2",fitters.OnlyCreaturesOrPlayers));
			return statement;
		}
		
		
		[ActionStatement("Picks up")]
		public Statement PicksUp()
		{			
			Statement statement = new Statement();
			statement.AddSlot(new StatementSlot("creature1",fitters.OnlyCreaturesOrPlayers));
			statement.AddText(new StatementLabel("picks up",actionBrush));
			statement.AddSlot(new StatementSlot("item1",fitters.OnlyItems));
			return statement;
		}	
		
		
		[ActionStatement("Drops")]
		public Statement Drops()
		{			
			Statement statement = new Statement();
			statement.AddSlot(new StatementSlot("creature1",fitters.OnlyCreaturesOrPlayers));
			statement.AddText(new StatementLabel("drops",actionBrush));
			statement.AddSlot(new StatementSlot("item1",fitters.OnlyItems));
			return statement;
		}	
		
		
		[ActionStatement("Equips")]
		public Statement Equips()
		{			
			Statement statement = new Statement();
			statement.AddSlot(new StatementSlot("creature1",fitters.OnlyCreaturesOrPlayers));
			statement.AddText(new StatementLabel("equips",actionBrush));
			statement.AddSlot(new StatementSlot("item1",fitters.OnlyItems));
			return statement;
		}	
		
		
		[ActionStatement("Unequips")]
		public Statement Unequips()
		{			
			Statement statement = new Statement();
			statement.AddSlot(new StatementSlot("creature1",fitters.OnlyCreaturesOrPlayers));
			statement.AddText(new StatementLabel("unequips",actionBrush));
			statement.AddSlot(new StatementSlot("item1",fitters.OnlyItems));
			return statement;
		}	
		
		
		[ActionStatement("Grows")]
		public Statement Grows()
		{			
			Statement statement = new Statement();
			statement.AddSlot(new StatementSlot("creature1",fitters.OnlyCreatures));
			statement.AddText(new StatementLabel("grows",actionBrush));
			return statement;
		}	
		
		
		[ActionStatement("Shrinks")]
		public Statement Shrinks()
		{			
			Statement statement = new Statement();
			statement.AddSlot(new StatementSlot("creature1",fitters.OnlyCreatures));
			statement.AddText(new StatementLabel("shrinks",actionBrush));
			return statement;
		}	
		
		
		[ActionStatement("Opens")]
		public Statement Opens()
		{			
			Statement statement = new Statement();
			statement.AddSlot(new StatementSlot("door1",fitters.OnlyDoorsOrPlaceables));
			statement.AddText(new StatementLabel("opens",actionBrush));
			return statement;
		}	
		
		
		[ActionStatement("Closes")]
		public Statement Closes()
		{			
			Statement statement = new Statement();
			statement.AddSlot(new StatementSlot("door1",fitters.OnlyDoorsOrPlaceables));
			statement.AddText(new StatementLabel("closes",actionBrush));
			return statement;
		}	
		
		
		[ActionStatement("Locks")]
		public Statement Locks()
		{			
			Statement statement = new Statement();
			statement.AddSlot(new StatementSlot("door1",fitters.OnlyDoorsOrPlaceables));
			statement.AddText(new StatementLabel("locks",actionBrush));
			return statement;
		}	
		
		
		[ActionStatement("Unlocks")]
		public Statement Unlocks()
		{			
			Statement statement = new Statement();
			statement.AddSlot(new StatementSlot("door1",fitters.OnlyDoorsOrPlaceables));
			statement.AddText(new StatementLabel("unlocks",actionBrush));
			return statement;
		}	
		
		
		[ActionStatement("Gets XP")]
		public Statement GetsXP()
		{			
			Statement statement = new Statement();
			statement.AddSlot(new StatementSlot("player1",fitters.OnlyPlayers));
			statement.AddText(new StatementLabel("gets XP",actionBrush));
			return statement;
		}	
		
		
		[ActionStatement("Walks")]
		public Statement Walks()
		{			
			Statement statement = new Statement();
			statement.AddSlot(new StatementSlot("creature1",fitters.OnlyCreaturesOrPlayers));
			statement.AddText(new StatementLabel("walks",actionBrush));
			statement.AddSlot(new StatementSlot("instance1",fitters.OnlyInstances));
			return statement;
		}	
		
		
		[ActionStatement("Runs")]
		public Statement Runs()
		{			
			Statement statement = new Statement();
			statement.AddSlot(new StatementSlot("creature1",fitters.OnlyCreaturesOrPlayers));
			statement.AddText(new StatementLabel("runs",actionBrush));
			statement.AddSlot(new StatementSlot("instance1",fitters.OnlyInstances));
			return statement;
		}
		
		
		[ActionStatement("Teleports")]
		public Statement Teleports()
		{
			Statement statement = new Statement();
			statement.AddSlot(new StatementSlot("creature1",fitters.OnlyCreaturesOrPlayers));
			statement.AddText(new StatementLabel("teleports",actionBrush));
			statement.AddSlot(new StatementSlot("instance1",fitters.OnlyInstances));
			return statement;
		}
		
		#endregion
		
		#region Conditions
		
		[ConditionStatement("Is dead")]
		public Statement IsDead()
		{			
			Statement statement = new Statement();
			statement.AddSlot(new StatementSlot("creature1",fitters.OnlyCreaturesOrPlayers));
			statement.AddText(new StatementLabel("is dead",conditionBrush));
			return statement;
		}
		
		
		[ConditionStatement("Is alive")]
		public Statement IsAlive()
		{			
			Statement statement = new Statement();
			statement.AddSlot(new StatementSlot("creature1",fitters.OnlyCreaturesOrPlayers));
			statement.AddText(new StatementLabel("is alive",conditionBrush));
			return statement;
		}
		
		
		[ConditionStatement("Is carrying")]
		public Statement IsCarrying()
		{			
			Statement statement = new Statement();
			statement.AddSlot(new StatementSlot("creature1",fitters.OnlyCreaturesOrPlayers));
			statement.AddText(new StatementLabel("is carrying",conditionBrush));
			statement.AddSlot(new StatementSlot("item1",fitters.OnlyItems));
			return statement;
		}
		
		
		[ConditionStatement("Is open")]
		public Statement IsOpen()
		{			
			Statement statement = new Statement();
			statement.AddSlot(new StatementSlot("door1",fitters.OnlyDoorsOrPlaceables));
			statement.AddText(new StatementLabel("is open",conditionBrush));
			return statement;
		}
		
		
		[ConditionStatement("Is closed")]
		public Statement IsClosed()
		{			
			Statement statement = new Statement();
			statement.AddSlot(new StatementSlot("door1",fitters.OnlyDoorsOrPlaceables));
			statement.AddText(new StatementLabel("is closed",conditionBrush));
			return statement;
		}
		
		
		[ConditionStatement("Is locked")]
		public Statement IsLocked()
		{			
			Statement statement = new Statement();
			statement.AddSlot(new StatementSlot("door1",fitters.OnlyDoorsOrPlaceables));
			statement.AddText(new StatementLabel("is locked",conditionBrush));
			return statement;
		}
		
		
		[ConditionStatement("Is unlocked")]
		public Statement IsUnlocked()
		{			
			Statement statement = new Statement();
			statement.AddSlot(new StatementSlot("door1",fitters.OnlyDoorsOrPlaceables));
			statement.AddText(new StatementLabel("is unlocked",conditionBrush));
			return statement;
		}
		
		#endregion
	
		#endregion
	}
}
