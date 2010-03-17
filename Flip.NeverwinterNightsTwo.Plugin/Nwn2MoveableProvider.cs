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
		protected Nwn2BlockFactory blocks;
		protected Nwn2StatementFactory statements;
		protected IMoveableManager manager; // only set on Populate() call
		
		protected const string ActionsBagName = "Actions";
		protected const string ConditionsBagName = "Conditions";
		protected const string OtherBagName = "Other";
		protected const string EventsBagName = "Events";
		protected const string BlueprintBagNamingFormat = "{0} blueprints";
		protected const string InstanceBagNamingFormat = "{0} instances";
		
		
		public Nwn2MoveableProvider(Nwn2BlockFactory blocks, Nwn2StatementFactory statements)
		{
			if (blocks == null) throw new ArgumentNullException("blocks");	
			if (statements == null) throw new ArgumentNullException("statements");	
			
			this.blocks = blocks;
			this.statements = statements;
			this.manager = null;
		}
		
		
		public Nwn2MoveableProvider(Nwn2BlockFactory blocks, Nwn2StatementFactory statements, ToolsetEventReporter reporter) : this(blocks,statements)
		{
			if (reporter != null) {
				TrackToolsetChanges(reporter);
			}
		}
		
		
		public override void Populate(IMoveableManager manager)
		{
			if (manager == null) throw new ArgumentNullException("manager");
			
			this.manager = manager;
			
			CreateBags();
			CreateStatements();
			CreateBlocks();
			CreateEvents();
		}
		
		
		protected void CreateBags()
		{
			manager.AddBag(ActionsBagName);
			manager.AddBag(ConditionsBagName);
			manager.AddBag(OtherBagName);
			manager.AddBag(EventsBagName);
			 
			string[] nwn2Types = Enum.GetNames(typeof(NWN2ObjectType));
			
			foreach (string nwn2Type in nwn2Types) {
				manager.AddBag(String.Format(BlueprintBagNamingFormat,nwn2Type));
			}
			foreach (string nwn2Type in nwn2Types) {
				manager.AddBag(String.Format(InstanceBagNamingFormat,nwn2Type));
			}			
		}
		
		
		protected void CreateStatements()
		{	
			foreach (Statement statement in statements.GetActions()) {
				manager.AddMoveable(ActionsBagName,statement);
			}					
			foreach (Statement statement in statements.GetConditions()) {
				manager.AddMoveable(ConditionsBagName,statement);
			}
		}
		
		
		protected void CreateBlocks()
		{
			manager.AddMoveable(OtherBagName,blocks.CreatePlayerBlock());
			manager.AddMoveable(OtherBagName,blocks.CreateModuleBlock());
			PopulateBlueprints();
			PopulateInstances();
		}
		
		
		protected void CreateEvents()
		{
			EventBlock eb = new EventBlock("dies");			
			manager.AddMoveable(EventsBagName,eb);
			eb = new EventBlock("is attacked");			
			manager.AddMoveable(EventsBagName,eb);
			eb = new EventBlock("is damaged");			
			manager.AddMoveable(EventsBagName,eb);	
			eb = new EventBlock("sees something");			
			manager.AddMoveable(EventsBagName,eb);
		}
		
		
		protected void PopulateBlueprints()
		{
			if (!Utils.Nwn2ToolsetFunctions.ToolsetIsOpen()) return;
			
			foreach (NWN2ObjectType type in Enum.GetValues(typeof(NWN2ObjectType))) {				
				foreach (INWN2Blueprint blueprint in NWN2GlobalBlueprintManager.GetBlueprintsOfType(type,true,true,true)) {
					ObjectBlock block = blocks.CreateBlueprintBlock(blueprint);
					manager.AddMoveable(String.Format(BlueprintBagNamingFormat,type.ToString()),block);
				}
			}
		}
		
		
		protected void PopulateInstances()
		{
			// TODO:
			// check this works when working with the actual toolset and not just Flip.UI.Generic:
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