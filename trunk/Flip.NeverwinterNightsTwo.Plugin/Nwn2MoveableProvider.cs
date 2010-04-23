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
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using NWN2Toolset.NWN2.Data;
using NWN2Toolset.NWN2.Data.Blueprints;
using NWN2Toolset.NWN2.Data.Instances;
using NWN2Toolset.NWN2.Data.Templates;
using NWN2Toolset.NWN2.Data.TypedCollections;
using NWN2Toolset.NWN2.Views;
using Sussex.Flip.Games.NeverwinterNightsTwo.Behaviours;
using Sussex.Flip.Games.NeverwinterNightsTwo.Utils;
using Sussex.Flip.UI;
using Sussex.Flip.Utils;

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
		
		
		protected override void CreateBags()
		{
			manager.AddBag(ActionsBagName);
			manager.AddBag(ConditionsBagName);
			manager.AddBag(EventsBagName);
			manager.AddBag(OtherBagName);	
			
			foreach (string nwn2Type in nwn2Types) {
				manager.AddBag(String.Format(BlueprintBagNamingFormat,nwn2Type));
				manager.AddBag(String.Format(InstanceBagNamingFormat,nwn2Type));
			}				
		}
		
		
		protected override void PopulateBags()
		{
			CreateStatements();
			CreateSpecialBlocks();
			CreateBlueprints();
			CreateInstancesFromOpenAreas();
			CreateEvents();
			CreateAreas();
		}
				
		
		protected void CreateStatements()
		{	
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
		
		
		protected void CreateSpecialBlocks()
		{		
			manager.AddMoveable(OtherBagName,blocks.CreatePlayerBlock());
			manager.AddMoveable(OtherBagName,blocks.CreateModuleBlock());
		}
		
		
		protected void CreateEvents()
		{			
			foreach (EventBlock eventBlock in events.GetEvents()) {
				manager.AddMoveable(EventsBagName,eventBlock);
			}
		}
		
		
		protected void CreateBlueprints()
		{
			if (!Utils.Nwn2ToolsetFunctions.ToolsetIsOpen()) return;
			
			foreach (NWN2ObjectType type in Enum.GetValues(typeof(NWN2ObjectType))) {				
				foreach (INWN2Blueprint blueprint in NWN2GlobalBlueprintManager.GetBlueprintsOfType(type,true,true,true)) {
					ObjectBlock block = blocks.CreateBlueprintBlock(blueprint);
					manager.AddMoveable(String.Format(BlueprintBagNamingFormat,type.ToString()),block);
				}
			}
		}
		
		
		protected void CreateInstancesFromOpenAreas()
		{
			if (!Utils.Nwn2ToolsetFunctions.ToolsetIsOpen()) return;
			
			foreach (NWN2AreaViewer viewer in NWN2Toolset.NWN2ToolsetMainForm.App.GetAllAreaViewers()) {
				if (viewer.Area != null) {
					CreateInstancesFromArea(viewer.Area);
				}
			}
		}
		
		
		protected void CreateInstancesFromArea(NWN2GameArea area)
		{
			if (!Utils.Nwn2ToolsetFunctions.ToolsetIsOpen()) return;
			
			if (area == null) {
				throw new ArgumentNullException("area");
			}
			if (!NWN2Toolset.NWN2ToolsetMainForm.App.Module.Areas.Contains(area)) {
				throw new ArgumentException("Given area is not part of this module.","area");
			}
			if (!area.Loaded) {
				throw new ArgumentException("Area must be open in the toolset to create instances from it.","area");
			}
				
			foreach (NWN2ObjectType type in Enum.GetValues(typeof(NWN2ObjectType))) {
					
				NWN2InstanceCollection instances = area.GetInstancesForObjectType(type);
				
				if (instances == null) return;
					
				foreach (INWN2Instance instance in instances) {
					ObjectBlock block = blocks.CreateInstanceBlock(instance,area);
					manager.AddMoveable(String.Format(InstanceBagNamingFormat,type.ToString()),block);
				}
			}
		}
		
		
		protected void CreateAreas()
		{
			if (!Utils.Nwn2ToolsetFunctions.ToolsetIsOpen()) return;
			if (NWN2Toolset.NWN2ToolsetMainForm.App.Module == null) return;
			
			foreach (NWN2GameArea area in NWN2Toolset.NWN2ToolsetMainForm.App.Module.Areas.Values) {
				ObjectBlock block = blocks.CreateAreaBlock(area);
				manager.AddMoveable(OtherBagName,block);
			}
		}
		
		
		protected void TrackToolsetChanges(ToolsetEventReporter reporter)
		{			
			if (!reporter.IsRunning) reporter.Start();
			
			reporter.InstanceAdded += delegate(object sender, InstanceEventArgs e) 
			{  	
				if (manager == null) {
					return;
				}
				ObjectBlock block = blocks.CreateInstanceBlock(e.Instance,e.Area);
				manager.AddMoveable(String.Format(InstanceBagNamingFormat,e.Instance.ObjectType.ToString()),block);
			};
			
			reporter.InstanceRemoved += delegate(object sender, InstanceEventArgs e) 
			{  
				if (manager == null || e.Instance == null) return;
				string bag = String.Format(InstanceBagNamingFormat,Nwn2ScriptSlot.GetNwn2Type(e.Instance.ObjectType));
				
				try {
					UIElementCollection collection = manager.GetMoveables(bag);
					
					ObjectBlock lost = blocks.CreateInstanceBlock(e.Instance,e.Area);
					
					foreach (ObjectBlock block in collection) {
						if (block.Equals(lost)) {
							manager.RemoveMoveable(bag,block);
							return;
						}
					}
				}
				catch (Exception ex) {
					System.Windows.MessageBox.Show(ex.ToString());
				}
			};
						
			reporter.BlueprintAdded += delegate(object sender, BlueprintEventArgs e) 
			{  
				if (manager == null) return;
				ObjectBlock block = blocks.CreateBlueprintBlock(e.Blueprint);
				manager.AddMoveable(String.Format(BlueprintBagNamingFormat,e.Blueprint.ObjectType.ToString()),block);
			};
			
			reporter.BlueprintRemoved += delegate(object sender, BlueprintEventArgs e) 
			{  
				if (manager == null || e.Blueprint == null) return;
				string bag = String.Format(BlueprintBagNamingFormat,Nwn2ScriptSlot.GetNwn2Type(e.Blueprint.ObjectType));
				
				try {
					UIElementCollection collection = manager.GetMoveables(bag);
					
					ObjectBlock lost = blocks.CreateBlueprintBlock(e.Blueprint);
					
					foreach (ObjectBlock block in collection) {
						if (block.Equals(lost)) {
							manager.RemoveMoveable(bag,block);
							return;
						}
					}
				}
				catch (Exception ex) {
					System.Windows.MessageBox.Show(ex.ToString());
				}
			};
			
			reporter.AreaOpened += delegate(object sender, AreaEventArgs e) 
			{  
				if (manager == null) return;	
				
				Thread thread = new Thread(new ParameterizedThreadStart(CreateBlockWhenAreaIsReady));
				thread.Start(e.Area);			
			};
			
			reporter.ResourceViewerClosed += delegate(object sender, ResourceViewerClosedEventArgs e)
			{  
				if (manager == null) return;
				
				System.Windows.Controls.UIElementCollection moveables = manager.GetMoveables(OtherBagName);
				
				foreach (Moveable moveable in moveables) {
					
					ObjectBlock block = moveable as ObjectBlock;
					
					// Assumes that there are no conversations or scripts with the same name as an
					// area, but there's no immediately apparent way around this:
					if (block != null && block.Identifier == e.ResourceName) {
						manager.RemoveMoveable(OtherBagName,block);
						return;
					}
				}
			};
		}
		
		
		protected System.Windows.Controls.Label uiThreadAccess = new System.Windows.Controls.Label();
		public void CreateBlockWhenAreaIsReady(object area)
		{
			NWN2Toolset.NWN2.Data.NWN2GameArea nwn2Area = area as NWN2Toolset.NWN2.Data.NWN2GameArea;
			if (nwn2Area == null) throw new ArgumentException("Parameter was not of type NWN2Toolset.NWN2.Data.NWN2GameArea.","area");
			
			int wait = 3000;
			int interval = 100;
			
			while (wait >= 0 && nwn2Area.Tag == null) {
				System.Threading.Thread.Sleep(interval);
				wait -= interval;
			}
			
			if (nwn2Area.Tag == null) {
				throw new ApplicationException("The last-opened area took too long to load, and could not be shown as a Flip block.");
			}
			
			Action action = new Action
			(
				delegate()
				{
					ObjectBlock block = blocks.CreateAreaBlock(nwn2Area);
					manager.AddMoveable(OtherBagName,block);
				}
			);
			
			uiThreadAccess.Dispatcher.Invoke(DispatcherPriority.Normal,action);
		}
				
		
		Serialiser serialiser = new Serialiser();	
		
		public override Moveable GetMoveableFromSerialised(string path)
		{			
			if (path == null) throw new ArgumentNullException("path");
			
			if (serialiser.CanDeserialise(path,typeof(AreaBehaviour))) {				
				AreaBehaviour behaviour = (AreaBehaviour)serialiser.Deserialise(path,typeof(AreaBehaviour));
				ObjectBlock block = blocks.CreateAreaBlock(behaviour);
				return block;				
			}
			
			else if (serialiser.CanDeserialise(path,typeof(BlueprintBehaviour))) {				
				BlueprintBehaviour behaviour = (BlueprintBehaviour)serialiser.Deserialise(path,typeof(BlueprintBehaviour));
				ObjectBlock block = blocks.CreateBlueprintBlock(behaviour);
				return block;				
			}
			
			else if (serialiser.CanDeserialise(path,typeof(InstanceBehaviour))) {				
				InstanceBehaviour behaviour = (InstanceBehaviour)serialiser.Deserialise(path,typeof(InstanceBehaviour));
				ObjectBlock block = blocks.CreateInstanceBlock(behaviour);
				return block;				
			}
			
			if (serialiser.CanDeserialise(path,typeof(ModuleBehaviour))) {			
				ObjectBlock block = blocks.CreateModuleBlock();
				return block;				
			}
			
			else if (serialiser.CanDeserialise(path,typeof(PlayerBehaviour))) {		
				ObjectBlock block = blocks.CreatePlayerBlock();
				return block;				
			}
			
			else if (serialiser.CanDeserialise(path,typeof(ObjectBlock))) {
				
				
				ObjectBlock block = (ObjectBlock)serialiser.Deserialise(path,typeof(ObjectBlock));				
				return block;
			}
			
//			else if (serialiser.CanDeserialise(path,typeof(EventBehaviour))) {			
//				EventBehaviour behaviour = (EventBehaviour)serialiser.Deserialise(path,typeof(EventBehaviour));
//				EventBlock block = blocks.CreateEventBlock(behaviour);
//				return block;				
//			}
			
			else {
				throw new ArgumentException("Could not deserialise the target to a known type.","path");
			}
		}
		
		
		public override void GetTriggerFromSerialised(TriggerControl triggerControl, string path)
		{
			if (triggerControl == null) throw new ArgumentNullException("triggerControl");
			if (path == null) throw new ArgumentNullException("path");
			
			Nwn2TriggerControl tc = serialiser.Deserialise(path,typeof(Nwn2TriggerControl)) as Nwn2TriggerControl;
			
			//HACK:
			if (tc.RaiserBlock != null) {
				triggerControl.RaiserBlock = (ObjectBlock)tc.RaiserBlock.DeepCopy();
			}
			
			if (tc.EventBlock != null) {
				triggerControl.EventBlock = (EventBlock)tc.EventBlock.DeepCopy();
			}
		}
	}
}