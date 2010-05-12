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
				
		public void GetStatements(out List<Statement> statements, out List<Statement> actions, out List<Statement> conditions)
		{
			MethodInfo[] methods = GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance);
			
			statements = new List<Statement>(methods.Length-1);
			int count = methods.Length/2;
			actions = new List<Statement>(count);
			conditions = new List<Statement>(count);			
			
			foreach (MethodInfo method in methods) {
				
				if (method.ReturnType == typeof(Statement)) {						
					Statement s = method.Invoke(this,null) as Statement;
		
					if (s != null) {
						statements.Add(s);
						switch (s.StatementType) {
							case StatementType.Action:
								actions.Add(s);
								break;
							case StatementType.Condition:
								conditions.Add(s);
								break;
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
		
		#region Actions
		
		public Statement Attacks()
		{
			return new Statement(new Attacks(fitters));
		}
		
		
		public Statement PicksUp()
		{
			return new Statement(new PicksUp(fitters));
		}
		
		
		public Statement DisplayMessage()
		{
			return new Statement(new DisplayMessage(fitters));
		}
		
		
		public Statement GiveGold()
		{
			return new Statement(new GiveGold(fitters));
		}		
		
		
		public Statement TakeGold()
		{
			return new Statement(new TakeGold(fitters));
		}
		
		
		public Statement GiveXP()
		{
			return new Statement(new GiveXP(fitters));
		}
		
		
		public Statement Lock()
		{
			return new Statement(new Lock(fitters));
		}
		
		
		public Statement Unlock()
		{
			return new Statement(new Unlock(fitters));
		}
		
		
		public Statement IsDead()
		{
			return new Statement(new IsDead(fitters));
		}
		
			
			
//		
//		
//		[ActionStatement("Drops")]
//		public Statement Drops()
//		{			
//			Statement statement = new ActionStatement();
//			statement.AddParameter(new ObjectBlockSlot("creature1",fitters.OnlyCreaturesOrPlayers));
//			statement.AddLabel("drops");
//			statement.AddParameter(new ObjectBlockSlot("item1",fitters.OnlyItems));
//			return statement;
//		}	
//		
//		
//		[ActionStatement("Equips")]
//		public Statement Equips()
//		{			
//			Statement statement = new ActionStatement();
//			statement.AddParameter(new ObjectBlockSlot("creature1",fitters.OnlyCreaturesOrPlayers));
//			statement.AddLabel("equips");
//			statement.AddParameter(new ObjectBlockSlot("item1",fitters.OnlyItems));
//			return statement;
//		}	
//		
//		
//		[ActionStatement("Unequips")]
//		public Statement Unequips()
//		{			
//			Statement statement = new ActionStatement();
//			statement.AddParameter(new ObjectBlockSlot("creature1",fitters.OnlyCreaturesOrPlayers));
//			statement.AddLabel("unequips");
//			statement.AddParameter(new ObjectBlockSlot("item1",fitters.OnlyItems));
//			return statement;
//		}	
//		
//		
//		[ActionStatement("Grows")]
//		public Statement Grows()
//		{			
//			Statement statement = new ActionStatement();
//			statement.AddParameter(new ObjectBlockSlot("creature1",fitters.OnlyCreatures));
//			statement.AddLabel("grows");
//			return statement;
//		}	
//		
//		
//		[ActionStatement("Shrinks")]
//		public Statement Shrinks()
//		{			
//			Statement statement = new ActionStatement();
//			statement.AddParameter(new ObjectBlockSlot("creature1",fitters.OnlyCreatures));
//			statement.AddLabel("shrinks");
//			return statement;
//		}	
//		
//		
//		[ActionStatement("Opens")]
//		public Statement Opens()
//		{			
//			Statement statement = new ActionStatement();
//			statement.AddParameter(new ObjectBlockSlot("door1",fitters.OnlyDoorsOrPlaceables));
//			statement.AddLabel("opens");
//			return statement;
//		}	
//		
//		
//		[ActionStatement("Closes")]
//		public Statement Closes()
//		{			
//			Statement statement = new ActionStatement();
//			statement.AddParameter(new ObjectBlockSlot("door1",fitters.OnlyDoorsOrPlaceables));
//			statement.AddLabel("closes");
//			return statement;
//		}	
//		
//		
//		[ActionStatement("Locks")]
//		public Statement Locks()
//		{			
//			Statement statement = new ActionStatement();
//			statement.AddParameter(new ObjectBlockSlot("door1",fitters.OnlyDoorsOrPlaceables));
//			statement.AddLabel("locks");
//			return statement;
//		}	
//		
//		
//		[ActionStatement("Unlocks")]
//		public Statement Unlocks()
//		{			
//			Statement statement = new ActionStatement();
//			statement.AddParameter(new ObjectBlockSlot("door1",fitters.OnlyDoorsOrPlaceables));
//			statement.AddLabel("unlocks");
//			return statement;
//		}	
//		
//		
//		[ActionStatement("Gets XP")]
//		public Statement GetsXP()
//		{			
//			Statement statement = new ActionStatement();
//			statement.AddParameter(new ObjectBlockSlot("player1",fitters.OnlyPlayers));
//			statement.AddLabel("gets XP");
//			return statement;
//		}	
//		
//		
//		[ActionStatement("Walks")]
//		public Statement Walks()
//		{			
//			Statement statement = new ActionStatement();
//			statement.AddParameter(new ObjectBlockSlot("creature1",fitters.OnlyCreaturesOrPlayers));
//			statement.AddLabel("walks");
//			statement.AddParameter(new ObjectBlockSlot("instance1",fitters.OnlyInstances));
//			return statement;
//		}	
//		
//		
//		[ActionStatement("Runs")]
//		public Statement Runs()
//		{			
//			Statement statement = new ActionStatement();
//			statement.AddParameter(new ObjectBlockSlot("creature1",fitters.OnlyCreaturesOrPlayers));
//			statement.AddLabel("runs");
//			statement.AddParameter(new ObjectBlockSlot("instance1",fitters.OnlyInstances));
//			return statement;
//		}
//		
//		
//		[ActionStatement("Teleports")]
//		public Statement Teleports()
//		{
//			Statement statement = new ActionStatement();
//			statement.AddParameter(new ObjectBlockSlot("creature1",fitters.OnlyCreaturesOrPlayers));
//			statement.AddLabel("teleports");
//			statement.AddParameter(new ObjectBlockSlot("instance1",fitters.OnlyInstances));
//			return statement;
//		}
		
		#endregion
		
		#region Conditions
		
//		[ConditionStatement("Is dead")]
//		public Statement IsDead()
//		{			
//			Statement statement = new ConditionStatement();
//			statement.AddParameter(new ObjectBlockSlot("creature1",fitters.OnlyCreaturesOrPlayers));
//			statement.AddLabel("is dead");
//			return statement;
//		}
//		
//		
//		[ConditionStatement("Is alive")]
//		public Statement IsAlive()
//		{			
//			Statement statement = new ConditionStatement();
//			statement.AddParameter(new ObjectBlockSlot("creature1",fitters.OnlyCreaturesOrPlayers));
//			statement.AddLabel("is alive");
//			return statement;
//		}
//		
//		
//		[ConditionStatement("Is carrying")]
//		public Statement IsCarrying()
//		{			
//			Statement statement = new ConditionStatement();
//			statement.AddParameter(new ObjectBlockSlot("creature1",fitters.OnlyCreaturesOrPlayers));
//			statement.AddLabel("is carrying");
//			statement.AddParameter(new ObjectBlockSlot("item1",fitters.OnlyItems));
//			return statement;
//		}
//		
//		
//		[ConditionStatement("Is open")]
//		public Statement IsOpen()
//		{			
//			Statement statement = new ConditionStatement();
//			statement.AddParameter(new ObjectBlockSlot("door1",fitters.OnlyDoorsOrPlaceables));
//			statement.AddLabel("is open");
//			return statement;
//		}
//		
//		
//		[ConditionStatement("Is closed")]
//		public Statement IsClosed()
//		{			
//			Statement statement = new ConditionStatement();
//			statement.AddParameter(new ObjectBlockSlot("door1",fitters.OnlyDoorsOrPlaceables));
//			statement.AddLabel("is closed");
//			return statement;
//		}
//		
//		
//		[ConditionStatement("Is locked")]
//		public Statement IsLocked()
//		{			
//			Statement statement = new ConditionStatement();
//			statement.AddParameter(new ObjectBlockSlot("door1",fitters.OnlyDoorsOrPlaceables));
//			statement.AddLabel("is locked");
//			return statement;
//		}
//		
//		
//		[ConditionStatement("Is unlocked")]
//		public Statement IsUnlocked()
//		{			
//			Statement statement = new ConditionStatement();
//			statement.AddParameter(new ObjectBlockSlot("door1",fitters.OnlyDoorsOrPlaceables));
//			statement.AddLabel("is unlocked");
//			return statement;
//		}
		
		#endregion
	
		#endregion
	}
}
