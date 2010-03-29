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
		
		protected Nwn2Fitters fitters;
		
		#endregion	
		
		#region Constructors
		
		public Nwn2StatementFactory(Nwn2Fitters fitters)
		{
			if (fitters == null) throw new ArgumentNullException("fitters");			
			this.fitters = fitters;
		}
		
		#endregion
		
		#region Methods
				
		public void GetStatements(out List<Statement> statements, out List<ActionStatement> actions, out List<ConditionStatement> conditions)
		{
			MethodInfo[] methods = GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance);
			
			statements = new List<Statement>(methods.Length-1);
			int count = methods.Length/2;
			actions = new List<ActionStatement>(count);
			conditions = new List<ConditionStatement>(count);			
			
			foreach (MethodInfo method in methods) {
				if (method.ReturnType == typeof(Statement)) {	
					object s = method.Invoke(this,null);
					
					if (s is ActionStatement) {
						ActionStatement action = (ActionStatement)s;
						actions.Add(action);
						statements.Add(action);
					}
					else if (s is ConditionStatement) {
						ConditionStatement condition = (ConditionStatement)s;
						conditions.Add(condition);
						statements.Add(condition);
					}
					else {
						statements.Add((Statement)s);
					}
				}
			}
		}
		
		
		public List<Statement> GetStatements()
		{
			List<Statement> statements;
			List<ActionStatement> actions;
			List<ConditionStatement> conditions;
			GetStatements(out statements, out actions, out conditions);
			return statements;
		}
		
		
		public List<ActionStatement> GetActions()
		{
			List<Statement> statements;
			List<ActionStatement> actions;
			List<ConditionStatement> conditions;
			GetStatements(out statements, out actions, out conditions);
			return actions;
		}
		
		
		public List<ConditionStatement> GetConditions()
		{			
			List<Statement> statements;
			List<ActionStatement> actions;
			List<ConditionStatement> conditions;
			GetStatements(out statements, out actions, out conditions);
			return conditions;
		}
		
		#region Actions
		
		[ActionStatement("Attacks")]
		public Statement Attacks()
		{			
			Statement statement = new ActionStatement();
			statement.AddSlot(new ObjectBlockSlot("creature1",fitters.OnlyCreaturesOrPlayers));
			statement.AddLabel("attacks");
			statement.AddSlot(new ObjectBlockSlot("creature2",fitters.OnlyCreaturesOrPlayers));
			return statement;
		}
		
		
		[ActionStatement("Picks up")]
		public Statement PicksUp()
		{			
			Statement statement = new ActionStatement();
			statement.AddSlot(new ObjectBlockSlot("creature1",fitters.OnlyCreaturesOrPlayers));
			statement.AddLabel("picks up");
			statement.AddSlot(new ObjectBlockSlot("item1",fitters.OnlyItems));
			return statement;
		}	
		
		
		[ActionStatement("Drops")]
		public Statement Drops()
		{			
			Statement statement = new ActionStatement();
			statement.AddSlot(new ObjectBlockSlot("creature1",fitters.OnlyCreaturesOrPlayers));
			statement.AddLabel("drops");
			statement.AddSlot(new ObjectBlockSlot("item1",fitters.OnlyItems));
			return statement;
		}	
		
		
		[ActionStatement("Equips")]
		public Statement Equips()
		{			
			Statement statement = new ActionStatement();
			statement.AddSlot(new ObjectBlockSlot("creature1",fitters.OnlyCreaturesOrPlayers));
			statement.AddLabel("equips");
			statement.AddSlot(new ObjectBlockSlot("item1",fitters.OnlyItems));
			return statement;
		}	
		
		
		[ActionStatement("Unequips")]
		public Statement Unequips()
		{			
			Statement statement = new ActionStatement();
			statement.AddSlot(new ObjectBlockSlot("creature1",fitters.OnlyCreaturesOrPlayers));
			statement.AddLabel("unequips");
			statement.AddSlot(new ObjectBlockSlot("item1",fitters.OnlyItems));
			return statement;
		}	
		
		
		[ActionStatement("Grows")]
		public Statement Grows()
		{			
			Statement statement = new ActionStatement();
			statement.AddSlot(new ObjectBlockSlot("creature1",fitters.OnlyCreatures));
			statement.AddLabel("grows");
			return statement;
		}	
		
		
		[ActionStatement("Shrinks")]
		public Statement Shrinks()
		{			
			Statement statement = new ActionStatement();
			statement.AddSlot(new ObjectBlockSlot("creature1",fitters.OnlyCreatures));
			statement.AddLabel("shrinks");
			return statement;
		}	
		
		
		[ActionStatement("Opens")]
		public Statement Opens()
		{			
			Statement statement = new ActionStatement();
			statement.AddSlot(new ObjectBlockSlot("door1",fitters.OnlyDoorsOrPlaceables));
			statement.AddLabel("opens");
			return statement;
		}	
		
		
		[ActionStatement("Closes")]
		public Statement Closes()
		{			
			Statement statement = new ActionStatement();
			statement.AddSlot(new ObjectBlockSlot("door1",fitters.OnlyDoorsOrPlaceables));
			statement.AddLabel("closes");
			return statement;
		}	
		
		
		[ActionStatement("Locks")]
		public Statement Locks()
		{			
			Statement statement = new ActionStatement();
			statement.AddSlot(new ObjectBlockSlot("door1",fitters.OnlyDoorsOrPlaceables));
			statement.AddLabel("locks");
			return statement;
		}	
		
		
		[ActionStatement("Unlocks")]
		public Statement Unlocks()
		{			
			Statement statement = new ActionStatement();
			statement.AddSlot(new ObjectBlockSlot("door1",fitters.OnlyDoorsOrPlaceables));
			statement.AddLabel("unlocks");
			return statement;
		}	
		
		
		[ActionStatement("Gets XP")]
		public Statement GetsXP()
		{			
			Statement statement = new ActionStatement();
			statement.AddSlot(new ObjectBlockSlot("player1",fitters.OnlyPlayers));
			statement.AddLabel("gets XP");
			return statement;
		}	
		
		
		[ActionStatement("Walks")]
		public Statement Walks()
		{			
			Statement statement = new ActionStatement();
			statement.AddSlot(new ObjectBlockSlot("creature1",fitters.OnlyCreaturesOrPlayers));
			statement.AddLabel("walks");
			statement.AddSlot(new ObjectBlockSlot("instance1",fitters.OnlyInstances));
			return statement;
		}	
		
		
		[ActionStatement("Runs")]
		public Statement Runs()
		{			
			Statement statement = new ActionStatement();
			statement.AddSlot(new ObjectBlockSlot("creature1",fitters.OnlyCreaturesOrPlayers));
			statement.AddLabel("runs");
			statement.AddSlot(new ObjectBlockSlot("instance1",fitters.OnlyInstances));
			return statement;
		}
		
		
		[ActionStatement("Teleports")]
		public Statement Teleports()
		{
			Statement statement = new ActionStatement();
			statement.AddSlot(new ObjectBlockSlot("creature1",fitters.OnlyCreaturesOrPlayers));
			statement.AddLabel("teleports");
			statement.AddSlot(new ObjectBlockSlot("instance1",fitters.OnlyInstances));
			return statement;
		}
		
		#endregion
		
		#region Conditions
		
		[ConditionStatement("Is dead")]
		public Statement IsDead()
		{			
			Statement statement = new ConditionStatement();
			statement.AddSlot(new ObjectBlockSlot("creature1",fitters.OnlyCreaturesOrPlayers));
			statement.AddLabel("is dead");
			return statement;
		}
		
		
		[ConditionStatement("Is alive")]
		public Statement IsAlive()
		{			
			Statement statement = new ConditionStatement();
			statement.AddSlot(new ObjectBlockSlot("creature1",fitters.OnlyCreaturesOrPlayers));
			statement.AddLabel("is alive");
			return statement;
		}
		
		
		[ConditionStatement("Is carrying")]
		public Statement IsCarrying()
		{			
			Statement statement = new ConditionStatement();
			statement.AddSlot(new ObjectBlockSlot("creature1",fitters.OnlyCreaturesOrPlayers));
			statement.AddLabel("is carrying");
			statement.AddSlot(new ObjectBlockSlot("item1",fitters.OnlyItems));
			return statement;
		}
		
		
		[ConditionStatement("Is open")]
		public Statement IsOpen()
		{			
			Statement statement = new ConditionStatement();
			statement.AddSlot(new ObjectBlockSlot("door1",fitters.OnlyDoorsOrPlaceables));
			statement.AddLabel("is open");
			return statement;
		}
		
		
		[ConditionStatement("Is closed")]
		public Statement IsClosed()
		{			
			Statement statement = new ConditionStatement();
			statement.AddSlot(new ObjectBlockSlot("door1",fitters.OnlyDoorsOrPlaceables));
			statement.AddLabel("is closed");
			return statement;
		}
		
		
		[ConditionStatement("Is locked")]
		public Statement IsLocked()
		{			
			Statement statement = new ConditionStatement();
			statement.AddSlot(new ObjectBlockSlot("door1",fitters.OnlyDoorsOrPlaceables));
			statement.AddLabel("is locked");
			return statement;
		}
		
		
		[ConditionStatement("Is unlocked")]
		public Statement IsUnlocked()
		{			
			Statement statement = new ConditionStatement();
			statement.AddSlot(new ObjectBlockSlot("door1",fitters.OnlyDoorsOrPlaceables));
			statement.AddLabel("is unlocked");
			return statement;
		}
		
		#endregion
	
		#endregion
	}
}
