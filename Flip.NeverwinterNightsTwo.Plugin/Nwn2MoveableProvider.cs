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
 * This file added by Keiron Nicholson on 15/03/2010 at 14:00.
 */

using System;
using System.Collections.Generic;
using System.Windows.Media;
using NWN2Toolset.NWN2.Data.Blueprints;
using NWN2Toolset.NWN2.Data.Templates;
using Sussex.Flip.Games.NeverwinterNightsTwo.Utils;
using Sussex.Flip.UI;

namespace Sussex.Flip.Games.NeverwinterNightsTwo
{
	/// <summary>
	/// Description of Nwn2MoveableProvider.
	/// </summary>
	public class Nwn2MoveableProvider : MoveableProvider
	{
		protected Nwn2ObjectBlockFactory blocks;
		protected Nwn2StatementFactory statements;
		protected Nwn2EventBlockFactory events;
		protected IMoveableManager manager; // only set on Populate() call
		protected static string[] nwn2Types;
			
		
		protected const string ActionsBagName = "Actions";
		protected const string ConditionsBagName = "Conditions";
		protected const string OtherBagName = "Other";
		protected const string EventsBagName = "Events";
		protected const string BlueprintBagNamingFormat = "{0} blueprints";
		protected const string InstanceBagNamingFormat = "{0} instances";
		
		
		static Nwn2MoveableProvider()
		{
			nwn2Types = Enum.GetNames(typeof(NWN2ObjectType));
		}
		
		
		public Nwn2MoveableProvider(Nwn2ObjectBlockFactory blocks, Nwn2StatementFactory statements, Nwn2EventBlockFactory events)
		{
			if (blocks == null) throw new ArgumentNullException("blocks");	
			if (statements == null) throw new ArgumentNullException("statements");	
			if (events == null) throw new ArgumentNullException("events");	
			
			this.blocks = blocks;
			this.statements = statements;
			this.events = events;
			this.manager = null;
		}
		
		
		public Nwn2MoveableProvider(Nwn2ObjectBlockFactory blocks, 
		                            Nwn2StatementFactory statements, 
		                            Nwn2EventBlockFactory events, 
		                            ToolsetEventReporter reporter) : this(blocks,statements,events)
		{
			if (reporter != null) {
				TrackToolsetChanges(reporter);
			}
		}
		
		
		protected override void CreateMoveables(IMoveableManager manager)
		{
			if (manager == null) throw new ArgumentNullException("manager");
			
			this.manager = manager;
			
			CreateStatements();
			CreateBlocks();
			CreateEvents();
		}
		
		
		protected void CreateStatements()
		{	
			manager.AddBag(ActionsBagName);
			manager.AddBag(ConditionsBagName);
			
			List<Statement> s,a,c;
			statements.GetStatements(out s, out a, out c);
			
			foreach (Statement action in a) {
				try {
					manager.AddMoveable(ActionsBagName,action);
				}
				catch (Exception e) {
					System.Windows.MessageBox.Show(e.ToString() + "\n\n" + action.Parent.ToString());
				}
			}					
			foreach (Statement condition in c) {
				try {
					manager.AddMoveable(ConditionsBagName,condition);
				}
				catch (Exception e) {					
					System.Windows.MessageBox.Show(e.ToString() + "\n\n" + condition.Parent.ToString());
				}
			}
		}
		
		
		protected void CreateBlocks()
		{
			manager.AddBag(OtherBagName);
			
			manager.AddMoveable(OtherBagName,blocks.CreatePlayerBlock());
			manager.AddMoveable(OtherBagName,blocks.CreateModuleBlock());
			CreateBlueprints();
			CreateInstances();
		}
		
		
		protected void CreateEvents()
		{
			manager.AddBag(EventsBagName);
			
			foreach (EventBlock eventBlock in events.GetEvents()) {
				manager.AddMoveable(EventsBagName,eventBlock);
			}
		}
		
		
		protected void CreateBlueprints()
		{
			foreach (string nwn2Type in nwn2Types) {
				manager.AddBag(String.Format(BlueprintBagNamingFormat,nwn2Type));
			}		
			
			if (!Utils.Nwn2ToolsetFunctions.ToolsetIsOpen()) return;
			
			foreach (NWN2ObjectType type in Enum.GetValues(typeof(NWN2ObjectType))) {				
				foreach (INWN2Blueprint blueprint in NWN2GlobalBlueprintManager.GetBlueprintsOfType(type,true,true,true)) {
					ObjectBlock block = blocks.CreateBlueprintBlock(blueprint);
					manager.AddMoveable(String.Format(BlueprintBagNamingFormat,type.ToString()),block);
				}
			}
		}
		
		
		protected void CreateInstances()
		{
			foreach (string nwn2Type in nwn2Types) {
				manager.AddBag(String.Format(InstanceBagNamingFormat,nwn2Type));
			}	
			
			if (!Utils.Nwn2ToolsetFunctions.ToolsetIsOpen()) return;
			
			NWN2Toolset.NWN2.Data.NWN2GameArea activeArea = NWN2Toolset.NWN2ToolsetMainForm.App.AreaContents.Area;
			if (activeArea != null) {				
				foreach (NWN2ObjectType type in Enum.GetValues(typeof(NWN2ObjectType))) {
					foreach (NWN2Toolset.NWN2.Data.Instances.INWN2Instance instance in activeArea.GetInstancesForObjectType(type)) {
						ObjectBlock block = blocks.CreateInstanceBlock(instance);
						manager.AddMoveable(String.Format(InstanceBagNamingFormat,type.ToString()),block);
					}
				}
			}
		}
		
		
		protected void TrackToolsetChanges(ToolsetEventReporter reporter)
		{			
			if (!reporter.IsRunning) reporter.Start();
			
			reporter.InstanceAdded += delegate(object sender, InstanceEventArgs e) 
			{  	
				if (manager == null) return;
				ObjectBlock block = blocks.CreateInstanceBlock(e.Instance);
				manager.AddMoveable(String.Format(InstanceBagNamingFormat,e.Instance.ObjectType.ToString()),block);
			};
			
			reporter.BlueprintAdded += delegate(object sender, BlueprintEventArgs e) 
			{  
				if (manager == null) return;
				ObjectBlock block = blocks.CreateBlueprintBlock(e.Blueprint);
				manager.AddMoveable(String.Format(BlueprintBagNamingFormat,e.Blueprint.ObjectType.ToString()),block);
			};
			
			reporter.AreaAdded += delegate(object sender, AreaEventArgs e)
			{  
				if (manager == null) return;
				ObjectBlock block = blocks.CreateAreaBlock(e.Area);
				manager.AddMoveable(OtherBagName,block);
			};
		}
	}
}