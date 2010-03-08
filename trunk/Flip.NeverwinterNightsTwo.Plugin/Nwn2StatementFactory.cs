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
	/// Description of StatementFactory.
	/// </summary>
	public class Nwn2StatementFactory : StatementFactory
	{			
		protected Fitter onlyCreatures;
		protected Fitter onlyDoors;
		protected Fitter onlyItems;
		protected Fitter onlyPlaceables;
		protected Fitter onlyStores;
		protected Fitter onlyTriggers;
		protected Fitter onlyWaypoints;
		protected Fitter onlyPlayers;
		protected Fitter onlyAreas;
		protected Fitter onlyModules;
		protected Fitter onlyCreaturesOrPlayers;
		protected Fitter onlyDoorsOrPlaceables;
		protected Fitter onlyInstances;
		protected LinearGradientBrush actionBrush = new LinearGradientBrush(Colors.Blue,Colors.Orange,45);
		protected LinearGradientBrush conditionBrush = new LinearGradientBrush(Colors.Yellow,Colors.Purple,45);
		protected LinearGradientBrush triggerBrush = new LinearGradientBrush(Colors.Red,Colors.Green,45);
		
		
		public Nwn2StatementFactory()
		{
			onlyCreatures = new SimpleFitter("Instance","Creature");
			onlyDoors = new SimpleFitter("Instance","Door");
			onlyItems = new SimpleFitter("Instance","Item");
			onlyPlaceables = new SimpleFitter("Instance","Placeable");
			onlyStores = new SimpleFitter("Instance","Store");
			onlyTriggers = new SimpleFitter("Instance","Trigger");
			onlyWaypoints = new SimpleFitter("Instance","Waypoint");
			onlyPlayers = new SimpleFitter("Player");
			onlyAreas = new SimpleFitter("Area");
			onlyModules = new SimpleFitter("Module");
			onlyCreaturesOrPlayers = new CreaturePlayerFitter();
			onlyDoorsOrPlaceables = new SimpleFitter("Instance",new List<string>{"Door","Placeable"});
			onlyInstances = new SimpleFitter("Instance");
		}
		
		
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
		
		
		public Statement Attacks()
		{			
			Statement statement = new Statement();
			statement.AddSlot(new StatementSlot("creature1",onlyCreaturesOrPlayers));
			statement.AddText(new StatementLabel("attacks",actionBrush));
			statement.AddSlot(new StatementSlot("creature2",onlyCreaturesOrPlayers));
			return statement;
		}
		
		
		public Statement PicksUp()
		{			
			Statement statement = new Statement();
			statement.AddSlot(new StatementSlot("creature1",onlyCreaturesOrPlayers));
			statement.AddText(new StatementLabel("picks up",actionBrush));
			statement.AddSlot(new StatementSlot("item1",onlyItems));
			return statement;
		}	
		
		
		public Statement Drops()
		{			
			Statement statement = new Statement();
			statement.AddSlot(new StatementSlot("creature1",onlyCreaturesOrPlayers));
			statement.AddText(new StatementLabel("drops",actionBrush));
			statement.AddSlot(new StatementSlot("item1",onlyItems));
			return statement;
		}	
		
		
		public Statement Equips()
		{			
			Statement statement = new Statement();
			statement.AddSlot(new StatementSlot("creature1",onlyCreaturesOrPlayers));
			statement.AddText(new StatementLabel("equips",actionBrush));
			statement.AddSlot(new StatementSlot("item1",onlyItems));
			return statement;
		}	
		
		
		public Statement Unequips()
		{			
			Statement statement = new Statement();
			statement.AddSlot(new StatementSlot("creature1",onlyCreaturesOrPlayers));
			statement.AddText(new StatementLabel("unequips",actionBrush));
			statement.AddSlot(new StatementSlot("item1",onlyItems));
			return statement;
		}	
		
		
		public Statement Grows()
		{			
			Statement statement = new Statement();
			statement.AddSlot(new StatementSlot("creature1",onlyCreatures));
			statement.AddText(new StatementLabel("grows",actionBrush));
			return statement;
		}	
		
		
		public Statement Shrinks()
		{			
			Statement statement = new Statement();
			statement.AddSlot(new StatementSlot("creature1",onlyCreatures));
			statement.AddText(new StatementLabel("shrinks",actionBrush));
			return statement;
		}	
		
		
		public Statement Opens()
		{			
			Statement statement = new Statement();
			statement.AddSlot(new StatementSlot("door1",onlyDoorsOrPlaceables));
			statement.AddText(new StatementLabel("opens",actionBrush));
			return statement;
		}	
		
		
		public Statement Closes()
		{			
			Statement statement = new Statement();
			statement.AddSlot(new StatementSlot("door1",onlyDoorsOrPlaceables));
			statement.AddText(new StatementLabel("closes",actionBrush));
			return statement;
		}	
		
		
		public Statement Locks()
		{			
			Statement statement = new Statement();
			statement.AddSlot(new StatementSlot("door1",onlyDoorsOrPlaceables));
			statement.AddText(new StatementLabel("locks",actionBrush));
			return statement;
		}	
		
		
		public Statement Unlocks()
		{			
			Statement statement = new Statement();
			statement.AddSlot(new StatementSlot("door1",onlyDoorsOrPlaceables));
			statement.AddText(new StatementLabel("unlocks",actionBrush));
			return statement;
		}	
		
		
		public Statement GetsXP()
		{			
			Statement statement = new Statement();
			statement.AddSlot(new StatementSlot("player1",onlyPlayers));
			statement.AddText(new StatementLabel("gets XP",actionBrush));
			return statement;
		}	
		
		
		public Statement Walks()
		{			
			Statement statement = new Statement();
			statement.AddSlot(new StatementSlot("creature1",onlyCreaturesOrPlayers));
			statement.AddText(new StatementLabel("walks",actionBrush));
			statement.AddSlot(new StatementSlot("instance1",onlyInstances));
			return statement;
		}	
		
		
		public Statement Runs()
		{			
			Statement statement = new Statement();
			statement.AddSlot(new StatementSlot("creature1",onlyCreaturesOrPlayers));
			statement.AddText(new StatementLabel("runs",actionBrush));
			statement.AddSlot(new StatementSlot("instance1",onlyInstances));
			return statement;
		}
		
		
		public Statement Teleports()
		{
			Statement statement = new Statement();
			statement.AddSlot(new StatementSlot("creature1",onlyCreaturesOrPlayers));
			statement.AddText(new StatementLabel("teleports",actionBrush));
			statement.AddSlot(new StatementSlot("instance1",onlyInstances));
			return statement;
		}
	}
}
