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
	/// Factory which provides actions and conditions for Neverwinter Nights 2 game scripting.
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
		
		#endregion
		
		#region Actions
					
		public Statement GiveGold()
		{
			return new Statement(new GiveGold());
		}		
		
		
		public Statement TakeGold()
		{
			return new Statement(new TakeGold());
		}
		
		
		public Statement GiveXP()
		{
			return new Statement(new GiveXP());
		}
		
		
		public Statement Attacks()
		{
			return new Statement(new Attacks());
		}
		
		
		public Statement PicksUp()
		{
			return new Statement(new PicksUp());
		}
		
		
		public Statement DisplayMessage()
		{
			return new Statement(new DisplayMessage());
		}
		
		
		public Statement Lock()
		{
			return new Statement(new Lock());
		}
		
		
		public Statement Unlock()
		{
			return new Statement(new Unlock());
		}
		
		
		public Statement JumpsTo()
		{
			return new Statement(new JumpsTo());
		}
		
		
		public Statement WalksTo()
		{
			return new Statement(new WalksTo());
		}
		
		
		public Statement RunsTo()
		{
			return new Statement(new RunsTo());
		}
		
		
		public Statement OpenStore()
		{
			return new Statement(new OpenStore());
		}
				
		
		public Statement HasGold()
		{
			return new Statement(new HasGold());
		}		
		
		
		public Statement IsDead()
		{
			return new Statement(new IsDead());
		}
		
		
		public Statement OwnsItem()
		{
			return new Statement(new OwnsItem());
		}
		
		
		public Statement HasEquippedItem()
		{
			return new Statement(new HasEquippedItem());
		}	
		
		
		public Statement IsNear()
		{
			return new Statement(new IsNear());
		}	
		
		
		public Statement IsLocked()
		{
			return new Statement(new IsLocked());
		}
		
		
		public Statement BecomesCommoner()
		{
			return new Statement(new BecomesCommoner());
		}
		
		
		public Statement BecomesDefender()
		{
			return new Statement(new BecomesDefender());
		}
		
		
		public Statement BecomesHostile()
		{
			return new Statement(new BecomesHostile());
		}
		
		
		public Statement MakeHenchman()
		{
			return new Statement(new MakeHenchman());
		}
		
		
		public Statement UnmakeHenchman()
		{
			return new Statement(new UnmakeHenchman());
		}
		
		
		public Statement Kill()
		{
			return new Statement(new Kill());
		}
		
		
		public Statement Heal()
		{
			return new Statement(new Heal());
		}
		
		
		public Statement SetTimeToNoon()
		{
			return new Statement(new SetTimeToNoon());
		}
		
		
		public Statement SetTimeToMidnight()
		{
			return new Statement(new SetTimeToMidnight());
		}
		
		
		public Statement SetTimeToSunrise()
		{
			return new Statement(new SetTimeToSunrise());
		}
		
		
		public Statement SetTimeToSunset()
		{
			return new Statement(new SetTimeToSunset());
		}
		
		
		public Statement Pause()
		{
			return new Statement(new Pause());
		}
		
		
		public Statement Unpause()
		{
			return new Statement(new Unpause());
		}
		
		
		public Statement EndGame()
		{
			return new Statement(new EndGame());
		}
		
		
		public Statement Delete()
		{
			return new Statement(new Delete());
		}
		
		
		public Statement AreaTransition()
		{
			return new Statement(new AreaTransition());
		}	
		
		
		public Statement CreateCreatureAtLocation()
		{
			return new Statement(new CreateCreatureAtLocation());
		}
		
		
		public Statement CreateItemAtLocation()
		{
			return new Statement(new CreateItemAtLocation());
		}
		
		
		public Statement CreatePlaceableAtLocation()
		{
			return new Statement(new CreatePlaceableAtLocation());
		}
		
		
		public Statement CreateItemInInventory()
		{
			return new Statement(new CreateItemInInventory());
		}	
		
		#endregion
	}
}
