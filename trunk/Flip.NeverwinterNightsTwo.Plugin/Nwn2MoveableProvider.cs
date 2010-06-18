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
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using NWN2Toolset;
using NWN2Toolset.NWN2.Data;
using NWN2Toolset.NWN2.Data.Blueprints;
using NWN2Toolset.NWN2.Data.Instances;
using NWN2Toolset.NWN2.Data.Templates;
using NWN2Toolset.NWN2.Data.TypedCollections;
using NWN2Toolset.NWN2.Views;
using Sussex.Flip.Games.NeverwinterNightsTwo;
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
		protected bool loadBlueprints = false;
		protected Nwn2ObjectBlockFactory blocks;
		protected Nwn2StatementFactory statements;
		protected Nwn2TriggerFactory triggers;
		protected static string[] nwn2BlockTypes;
			
		
		public const string ActionsBagName = "Actions";
		public const string ConditionsBagName = "Conditions";
		public const string OtherBagName = "Special";
		public const string TriggersBagName = "Events";
		public const string BlueprintBagNamingFormat = "{0} blueprints";
		public const string InstanceBagNamingFormat = "{0}s";
		
		
		static Nwn2MoveableProvider()
		{
			// Excludes StaticCamera, Environment, Tree, Prefab as you'll never want to use these instances.
			nwn2BlockTypes = new string[] { "Creature", 
											"Door", 
											"Encounter", 
											"Item", 
											"Light", 
											"Placeable", 
											"PlacedEffect",
											"Sound", 
											"Store", 
											"Trigger", 
											"Waypoint"
			};
		}
		
		
		public Nwn2MoveableProvider(Nwn2ObjectBlockFactory blocks, Nwn2StatementFactory statements, Nwn2TriggerFactory triggers)
		{
			if (blocks == null) throw new ArgumentNullException("blocks");	
			if (statements == null) throw new ArgumentNullException("statements");	
			if (triggers == null) throw new ArgumentNullException("triggers");	
			
			this.blocks = blocks;
			this.statements = statements;
			this.triggers = triggers;
			this.manager = null;
		}
		
		
		public Nwn2MoveableProvider(Nwn2ObjectBlockFactory blocks,
		                            Nwn2StatementFactory statements, 
		                            Nwn2TriggerFactory triggers,
		                            ToolsetEventReporter reporter) : this(blocks,statements,triggers)
		{
			if (reporter != null) {
				TrackToolsetChanges(reporter);
			}
		}
		
		
		protected override void CreateBags()
		{
			manager.AddBag(ActionsBagName,false);
			manager.AddBag(ConditionsBagName,false);
			manager.AddBag(TriggersBagName,false);
			manager.AddBag(OtherBagName,true);	
			
			foreach (string nwn2Type in nwn2BlockTypes) {
				if (loadBlueprints) manager.AddBag(String.Format(BlueprintBagNamingFormat,nwn2Type),true);
				manager.AddBag(String.Format(InstanceBagNamingFormat,nwn2Type),true);
			}
			
			manager.DisplayBag("Control");	
		}
		
		
		protected override void PopulateBags()
		{
			CreateStatements();
			CreateSpecialBlocks();
			if (loadBlueprints) CreateBlueprints();
			CreateInstancesFromOpenAreas();
			CreateTriggers();
			CreateAreas();
		}
				
		
		protected void CreateStatements()
		{	
			List<Statement> s,a,c;
			statements.GetStatements(out s, out a, out c);
			
			foreach (Statement action in a) {
				try {
//					Nwn2StatementBehaviour sb = (Nwn2StatementBehaviour)action.Behaviour;
//					if (sb.IsAsynchronous) {
//						action.BorderBrush = Brushes.Black;
//						action.BorderThickness = new Thickness(10);
//					}
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
			manager.AddMoveable(OtherBagName,new NumberBlock(0));
			manager.AddMoveable(OtherBagName,new StringBlock("click 'change' to edit this Word Block"));
		}
		
		
		protected void CreateTriggers()
		{
			foreach (TriggerControl trigger in triggers.GetTriggers()) {
				manager.AddMoveable(TriggersBagName,trigger);
			}
		}
		
		
		protected void CreateBlueprints()
		{
			if (!Utils.Nwn2ToolsetFunctions.ToolsetIsOpen()) return;
			
			foreach (NWN2ObjectType type in Enum.GetValues(typeof(NWN2ObjectType))) {
				
				try {
					string bag = String.Format(BlueprintBagNamingFormat,type.ToString());
					if (!manager.HasBag(bag)) continue;
					
					foreach (INWN2Blueprint blueprint in NWN2GlobalBlueprintManager.GetBlueprintsOfType(type,true,true,true)) {
						ObjectBlock block = blocks.CreateBlueprintBlock(blueprint);
						manager.AddMoveable(bag,block);
					}
				}
				catch (Exception e) {
					MessageBox.Show("Failed to populate bag of " + type + " blueprints.\n\n" + e);
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
					
				string bag = String.Format(InstanceBagNamingFormat,type.ToString());
				if (!manager.HasBag(bag)) continue;
				
				NWN2InstanceCollection instances = area.GetInstancesForObjectType(type);
				
				if (instances == null) return;
					
				foreach (INWN2Instance instance in instances) {
					ObjectBlock block = blocks.CreateInstanceBlock(instance,area);
					manager.AddMoveable(bag,block);
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
		
		
		Sussex.Flip.Games.NeverwinterNightsTwo.Integration.NarrativeThreadsHelper nt 
			= new Sussex.Flip.Games.NeverwinterNightsTwo.Integration.NarrativeThreadsHelper();
		
		
		protected void TrackToolsetChanges(ToolsetEventReporter reporter)
		{			
			if (!reporter.IsRunning) reporter.Start();
			
			
			reporter.ModuleChanged += delegate(object oSender, ModuleChangedEventArgs eArgs) 
			{  
				/* Fires when a module closes, but that doesn't mean that the new module has
				 * been fully opened yet... usually takes a while!
				 * 
				 * BlueprintAdded fires for every module blueprint when you open a new module,
				 * so we don't need to look for new blueprints in ModuleChanged. However,
				 * BlueprintRemoved does NOT fire when you close a module, so we do need
				 * to clear any Narrative Threads blueprints when the module changes.
				 * 
				 */				
				
				if (manager != null) {
					foreach (NWN2ObjectType type in Enum.GetValues(typeof(NWN2ObjectType))) {
						string bag = String.Format(InstanceBagNamingFormat,type);
						if (manager.HasBag(bag)) {
							manager.EmptyBag(bag);
						}
					}
					
					List<ObjectBlock> areas = new List<ObjectBlock>();
					foreach (Moveable moveable in manager.GetMoveables(OtherBagName)) {
						ObjectBlock block = moveable as ObjectBlock;
						if (block == null) continue;
						if (block.Behaviour is AreaBehaviour) areas.Add(block);
					}
					
					foreach (ObjectBlock area in areas) {
						manager.RemoveMoveable(OtherBagName,area);
					}
				}
			};
			
			
			reporter.InstanceAdded += delegate(object sender, InstanceEventArgs e) 
			{  	
				if (manager == null) return;
				
				string bag = String.Format(InstanceBagNamingFormat,e.Instance.ObjectType.ToString());
				if (!manager.HasBag(bag)) return;
				
				ObjectBlock block = blocks.CreateInstanceBlock(e.Instance,e.Area);
				manager.AddMoveable(bag,block);
			};
			
			
			reporter.InstanceRemoved += delegate(object sender, InstanceEventArgs e) 
			{  
				if (manager == null || e.Instance == null) return;
				
				string bag = String.Format(InstanceBagNamingFormat,Nwn2ScriptSlot.GetNwn2Type(e.Instance.ObjectType));				
				if (!manager.HasBag(bag)) return;
				
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
				if (manager == null || e.Blueprint == null) return;
				
				if (nt.CreatedByNarrativeThreads(e.Blueprint)) {	
					
					Action action = new Action
					(
						delegate()
						{					
							string bag = String.Format(InstanceBagNamingFormat,e.Blueprint.ObjectType.ToString());
							
							if (manager.HasBag(bag)) {						
								ObjectBlock block = blocks.CreateInstanceBlockFromBlueprint(e.Blueprint);
								
								block.BorderBrush = Brushes.Gold;
								block.BorderThickness = new Thickness(5);
								
								manager.AddMoveable(bag,block);		
							}
						}
					);					
					
					uiThreadAccess.Dispatcher.Invoke(DispatcherPriority.Normal,action);
				}
				
				if (loadBlueprints) {					
				
					Action action = new Action
					(
						delegate()
						{
							string bag = String.Format(BlueprintBagNamingFormat,e.Blueprint.ObjectType.ToString());
							
							if (manager.HasBag(bag)) {		
								ObjectBlock block = blocks.CreateBlueprintBlock(e.Blueprint);
								manager.AddMoveable(bag,block);
							}
						}
					);					
					
					uiThreadAccess.Dispatcher.Invoke(DispatcherPriority.Normal,action);				
				}
				
			};
			
			
			reporter.BlueprintRemoved += delegate(object sender, BlueprintEventArgs e) 
			{  
				if (manager == null || e.Blueprint == null) return;
				
				if (nt.CreatedByNarrativeThreads(e.Blueprint)) {			
				
					Action action = new Action
					(
						delegate()
						{
							string bag = String.Format(InstanceBagNamingFormat,e.Blueprint.ObjectType.ToString());
							
							if (manager.HasBag(bag)) {		
																
								ObjectBlock lost = blocks.CreateInstanceBlockFromBlueprint(e.Blueprint);
								ObjectBlock removing = null;
								
								foreach (ObjectBlock block in manager.GetMoveables(bag)) {
									if (block.Equals(lost)) {
										removing = block;
										break;
									}
								}
								
								if (removing != null) {
									manager.RemoveMoveable(bag,removing);
								}
							}
						}
					);					
					
					uiThreadAccess.Dispatcher.Invoke(DispatcherPriority.Normal,action);				
				}
				
				if (loadBlueprints) {					
				
					Action action = new Action
					(
						delegate()
						{
							string bag = String.Format(BlueprintBagNamingFormat,e.Blueprint.ObjectType.ToString());
							
							if (manager.HasBag(bag)) {		
																
								ObjectBlock lost = blocks.CreateBlueprintBlock(e.Blueprint);
								ObjectBlock removing = null;
								
								foreach (ObjectBlock block in manager.GetMoveables(bag)) {
									if (block.Equals(lost)) {
										removing = block;
										break;
									}
								}
								
								if (removing != null) {
									manager.RemoveMoveable(bag,removing);
								}
							}
						}
					);					
					
					uiThreadAccess.Dispatcher.Invoke(DispatcherPriority.Normal,action);				
				}
			};
			
			
			reporter.AreaOpened += delegate(object sender, AreaEventArgs e) 
			{  
				if (manager == null) return;	
				
				Thread thread = new Thread(new ParameterizedThreadStart(CreateBlocksWhenAreaIsReady));
				thread.Start(e.Area);			
			};
			
			
			reporter.ResourceViewerClosed += delegate(object sender, ResourceViewerClosedEventArgs e)
			{  
				if (manager == null) return;
				
				bool isArea = false;
				
				foreach (Moveable moveable in manager.GetMoveables(OtherBagName)) {
					ObjectBlock block = moveable as ObjectBlock;
					if (block == null) continue;
					AreaBehaviour area = block.Behaviour as AreaBehaviour;
					if (area == null) continue;
					
					// Assumes that there are no conversations or scripts with the same name as an
					// area, but there's no immediately apparent way around this:
					// (TODO: Could check that module doesn't have a script or conversation of the same name.)
					if (area.Identifier == e.ResourceName) {
						manager.RemoveMoveable(OtherBagName,moveable);
						isArea = true;
						break;
					}
				}
				
				if (isArea) {
					
					// At this point we think it's an area that's been closed, so remove
					// any instances associated with that area:
							
					foreach (NWN2ObjectType type in Enum.GetValues(typeof(NWN2ObjectType))) {
						string bag = String.Format(InstanceBagNamingFormat,type);
						if (manager.HasBag(bag)) {
							
							List<Moveable> removing = new List<Moveable>();
							
							foreach (Moveable moveable in manager.GetMoveables(bag)) {
								ObjectBlock block = moveable as ObjectBlock;
								if (block == null) continue;
								InstanceBehaviour instance = block.Behaviour as InstanceBehaviour;
								if (instance == null) continue;
								
								if (instance.AreaTag == e.ResourceName) {
									removing.Add(moveable);
								}
							}
							
							foreach (Moveable moveable in removing) {
								manager.RemoveMoveable(bag,moveable);
							}
						}
					}
				}
			};
		}
		
		
		protected System.Windows.Controls.Label uiThreadAccess = new System.Windows.Controls.Label();
		public void CreateBlocksWhenAreaIsReady(object area)
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
					ObjectBlock areaBlock = blocks.CreateAreaBlock(nwn2Area);
					manager.AddMoveable(OtherBagName,areaBlock);
					
					foreach (NWN2InstanceCollection instanceCollection in nwn2Area.AllInstances) {
						
						if (instanceCollection.Count == 0) continue;
						
						string bag = String.Format(InstanceBagNamingFormat,instanceCollection[0].ObjectType);
						
						if (!manager.HasBag(bag)) continue;
						
						foreach (INWN2Instance instance in instanceCollection) {
							ObjectBlock instanceBlock = blocks.CreateInstanceBlock(instance,nwn2Area);
							manager.AddMoveable(bag,instanceBlock);
						}
					}
				}
			);
			
			uiThreadAccess.Dispatcher.Invoke(DispatcherPriority.Normal,action);
		}
	}
}