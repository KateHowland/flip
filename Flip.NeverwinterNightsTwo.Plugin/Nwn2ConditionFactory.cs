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
	public class Nwn2ConditionFactory : StatementFactory
	{			
		protected Brush brush;
		protected Nwn2Fitters fitters;
		
		
		public Nwn2ConditionFactory(Nwn2Fitters fitters, Brush brush)
		{
			if (fitters == null) throw new ArgumentNullException("fitters");
			if (brush == null) brush = Brushes.Transparent;
			
			this.fitters = fitters;
			this.brush = brush;
		}
		
		
		public override List<Statement> GetStatements()
		{
			MethodInfo[] methods = GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance);
			List<Statement> statements = new List<Statement>(methods.Length-1);
			
			foreach (MethodInfo mi in methods) {
				if (mi.ReturnType == typeof(Statement)) {
					try { statements.Add((Statement)mi.Invoke(this,null)); }
					catch (TargetInvocationException e) {
						System.Windows.MessageBox.Show(e.ToString() + "\n\n" + e.InnerException);
					}
				}
			}
			
			return statements;
		}
		
		
		public Statement IsDead()
		{			
			Statement statement = new Statement();
			statement.AddSlot(new StatementSlot("creature1",fitters.OnlyCreaturesOrPlayers));
			statement.AddText(new StatementLabel("is dead",brush));
			return statement;
		}
		
		
		public Statement IsAlive()
		{			
			Statement statement = new Statement();
			statement.AddSlot(new StatementSlot("creature1",fitters.OnlyCreaturesOrPlayers));
			statement.AddText(new StatementLabel("is alive",brush));
			return statement;
		}
		
		
		public Statement IsCarrying()
		{			
			Statement statement = new Statement();
			statement.AddSlot(new StatementSlot("creature1",fitters.OnlyCreaturesOrPlayers));
			statement.AddText(new StatementLabel("is carrying",brush));
			statement.AddSlot(new StatementSlot("item1",fitters.OnlyItems));
			return statement;
		}
		
		
		public Statement IsOpen()
		{			
			Statement statement = new Statement();
			statement.AddSlot(new StatementSlot("door1",fitters.OnlyDoorsOrPlaceables));
			statement.AddText(new StatementLabel("is open",brush));
			return statement;
		}
		
		
		public Statement IsClosed()
		{			
			Statement statement = new Statement();
			statement.AddSlot(new StatementSlot("door1",fitters.OnlyDoorsOrPlaceables));
			statement.AddText(new StatementLabel("is closed",brush));
			return statement;
		}
		
		
		public Statement IsLocked()
		{			
			Statement statement = new Statement();
			statement.AddSlot(new StatementSlot("door1",fitters.OnlyDoorsOrPlaceables));
			statement.AddText(new StatementLabel("is locked",brush));
			return statement;
		}
		
		
		public Statement IsUnlocked()
		{			
			Statement statement = new Statement();
			statement.AddSlot(new StatementSlot("door1",fitters.OnlyDoorsOrPlaceables));
			statement.AddText(new StatementLabel("is unlocked",brush));
			return statement;
		}
	}
}
