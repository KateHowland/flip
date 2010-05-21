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
		
		
		public Statement EndGame()
		{
			return new Statement(new EndGame(fitters));
		}
		
		
		public Statement JumpsTo()
		{
			return new Statement(new JumpsTo(fitters));
		}
		
		
		public Statement WalksTo()
		{
			return new Statement(new WalksTo(fitters));
		}
		
		
		public Statement RunsTo()
		{
			return new Statement(new RunsTo(fitters));
		}
		
		
		public Statement OpenStore()
		{
			return new Statement(new OpenStore(fitters));
		}
		
		
		public Statement IsDead()
		{
			return new Statement(new IsDead(fitters));
		}
		
		
		public Statement OwnsItem()
		{
			return new Statement(new OwnsItem(fitters));
		}
		
		
		public Statement BecomesCommoner()
		{
			return new Statement(new BecomesCommoner(fitters));
		}
		
		
		public Statement BecomesDefender()
		{
			return new Statement(new BecomesDefender(fitters));
		}
		
		
		public Statement BecomesHostile()
		{
			return new Statement(new BecomesHostile(fitters));
		}
		
		
		public Statement MakeHenchman()
		{
			return new Statement(new MakeHenchman(fitters));
		}
		
		
		public Statement UnmakeHenchman()
		{
			return new Statement(new UnmakeHenchman(fitters));
		}
		
		
		public Statement Kill()
		{
			return new Statement(new Kill(fitters));
		}
		
		
		public Statement Heal()
		{
			return new Statement(new Heal(fitters));
		}
		
		
		public Statement Pause()
		{
			return new Statement(new Pause(fitters));
		}
		
		
		public Statement Unpause()
		{
			return new Statement(new Unpause(fitters));
		}
		
		
		public Statement SetTimeToNoon()
		{
			return new Statement(new SetTimeToNoon(fitters));
		}
		
		
		public Statement SetTimeToMidnight()
		{
			return new Statement(new SetTimeToMidnight(fitters));
		}
		
		
		public Statement SetTimeToSunrise()
		{
			return new Statement(new SetTimeToSunrise(fitters));
		}
		
		
		public Statement SetTimeToSunset()
		{
			return new Statement(new SetTimeToSunset(fitters));
		}
		
		
		public Statement Delete()
		{
			return new Statement(new Delete(fitters));
		}
		
		
		public Statement AreaTransition()
		{
			return new Statement(new AreaTransition(fitters));
		}
		
		
		public Statement HasGold()
		{
			return new Statement(new HasGold(fitters));
		}
		
		
		// TODO:
		// Won't work until code is put in to automatically include
		// flip_functions.nss in any module.
		// (Or some more clever way of doing this.. either the
		// Statement could name the functions it needs, or
		// the function collections it needs, or it could specify
		// the function it needs itself (in code), and it would
		// be appended to the start of the file.
//		public Statement HasEquippedItem()
//		{
//			return new Statement(new HasEquippedItem(fitters));
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
