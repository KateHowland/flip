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
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using NWN2Toolset;
using NWN2Toolset.Data;
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
			
		
		public const string ValuesBagName = "Values";
		public const string AreasBagName = "Areas";
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
			foreach (string nwn2Type in nwn2BlockTypes) {
				if (loadBlueprints) manager.AddBag(String.Format(BlueprintBagNamingFormat,nwn2Type),true);
				manager.AddBag(String.Format(InstanceBagNamingFormat,nwn2Type),true);
			}
			
			manager.AddBag(AreasBagName,true);
			manager.AddBag(ValuesBagName,true);
			
			manager.DisplayBag(ActionsBagName);	
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
					manager.AddMoveable(ActionsBagName,action);
				}
				catch (Exception) {}
			}					
			foreach (Statement condition in c) {
				try {
					manager.AddMoveable(ConditionsBagName,condition);
				}
				catch (Exception) {}
			}
		}
		
		
		protected void CreateSpecialBlocks()
		{		
			manager.AddMoveable(ValuesBagName,new NumberBlock(123));
			manager.AddMoveable(ValuesBagName,new StringBlock("abc"));
		}
		
		
		protected void CreateTriggers()
		{
			foreach (TriggerControl trigger in triggers.GetTriggers()) {
				manager.AddMoveable(EventsBagName,trigger);
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
			manager.AddMoveable("Creatures",blocks.CreatePlayerBlock());
			
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
				manager.AddMoveable(AreasBagName,block);
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
				 * been fully opened yet... usually takes a while! */			
				
				Action action = new Action
				(
					delegate()
					{		
						if (manager != null) {
							
							manager.EmptyBag(AreasBagName);
							
							foreach (NWN2ObjectType type in Enum.GetValues(typeof(NWN2ObjectType))) {
								string bag = String.Format(InstanceBagNamingFormat,type);
								if (manager.HasBag(bag)) {
									manager.EmptyBag(bag);
								}
							}
			
							manager.AddMoveable("Creatures",blocks.CreatePlayerBlock());
						}
					}
				);					
					
				uiThreadAccess.Dispatcher.Invoke(DispatcherPriority.Normal,action);					
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
				catch (Exception) {}
			};
			
						
			reporter.BlueprintAdded += delegate(object sender, BlueprintEventArgs e) 
			{  
				if (manager == null || e.Blueprint == null) return;
				
				if (nt.CreatedByNarrativeThreads(e.Blueprint)) {	
																
					Thread thread = new Thread(new ParameterizedThreadStart(CreateNarrativeThreadsBlock));	
					thread.IsBackground = true;
					thread.Start(e.Blueprint);
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
				thread.IsBackground = true;
				thread.Start(e.Area);			
			};
			
			
			reporter.ResourceViewerClosed += delegate(object sender, ResourceViewerClosedEventArgs e)
			{  
				if (manager == null) return;
				
				foreach (Moveable moveable in manager.GetMoveables(AreasBagName)) {
					ObjectBlock block = moveable as ObjectBlock;
					if (block == null) continue;
					AreaBehaviour area = block.Behaviour as AreaBehaviour;
					if (area == null) continue;
					
					// Assumes that there are no conversations or scripts with the same name as an
					// area, but there's no immediately apparent way around this:
					// (TODO: Could check that module doesn't have a script or conversation of the same name.)
					if (area.Identifier == e.ResourceName) {
						manager.RemoveMoveable(AreasBagName,moveable);
						break;
					}
				}
				
				if (NWN2ToolsetMainForm.App.Module != null && NWN2ToolsetMainForm.App.Module.Areas.ContainsCaseInsensitive(e.ResourceName)) {
					
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
								
								if (instance.AreaTag.ToLower() == e.ResourceName.ToLower()) {
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
			
			
			// Ensure that an area always has the same resource name and tag:
			reporter.AreaNameChanged += delegate(object oObject, NameChangedEventArgs eArgs) 
			{  
				NWN2GameArea area = eArgs.Item as NWN2GameArea;
				if (area == null) return;
				
				string blockHasTag = area.Tag;
				
				if (area.Tag != area.Name) area.Tag = area.Name;
				OEIShared.Utils.OEIExoLocString oeiStr = Nwn2Strings.GetOEIStringFromString(area.Name);
				if (area.DisplayName != oeiStr) area.DisplayName = oeiStr;
				
				// Note that this will only work for areas that are currently open...
				// we'll deal with changing the name of closed areas when they open.
				
				
				// Update the area block, if the area is open:				
				bool open = false;
				foreach (NWN2AreaViewer av in NWN2ToolsetMainForm.App.GetAllAreaViewers()) {
					if (av.Area == area) {
						open = true;
						break;
					}
				}
				if (!open) return;
							
				AreaBehaviour behaviour = blocks.CreateAreaBehaviour(area);
													
				if (manager.HasBag(AreasBagName)) {
					UIElementCollection existingBlocks = manager.GetMoveables(AreasBagName);
													
					bool updated = false;
						
					foreach (UIElement u in existingBlocks) {
						ObjectBlock existing = u as ObjectBlock;								
						if (existing == null) continue;
						AreaBehaviour existingBehaviour = existing.Behaviour as AreaBehaviour;
						if (existingBehaviour == null) continue;
																	
						// If you find an area with the same tag, replace its behaviour to update it:
						if (existingBehaviour.Tag == blockHasTag) {
							existing.Behaviour = behaviour;
							updated = true;
							break;
						}
					}
					
					if (!updated) {
						ObjectBlock block = blocks.CreateAreaBlock(behaviour);
						manager.AddMoveable(AreasBagName,block,false);
					}
				}
			};
			
			
			// If a script has its name changed, change it back:
			reporter.ScriptNameChanged += delegate(object oObject, NameChangedEventArgs eArgs) 
			{  					
				Thread thread = new Thread(new ParameterizedThreadStart(ReverseScriptNameChange));
				thread.IsBackground = false;
				thread.Start(eArgs);	
			};
		}
		
		
		string ignoreThisNewScriptName = null;
		public void ReverseScriptNameChange(object e)
		{
			NameChangedEventArgs eArgs = e as NameChangedEventArgs;
			if (eArgs == null) return;
			
			if (ignoreThisNewScriptName == eArgs.NewName) {
				ignoreThisNewScriptName = null;
				return;
			}
			
			NWN2GameScript script = eArgs.Item as NWN2GameScript;
			if (script != null) {
				Thread.Sleep(1000);
				try {
					ignoreThisNewScriptName = eArgs.OldName;
					script.Name = eArgs.OldName;
				}
				catch (Exception) { }
			}
		}
		
		
		public void CreateNarrativeThreadsBlock(object blueprint)
		{
			if (blueprint == null) throw new ArgumentNullException("blueprint");
			INWN2Blueprint bp = blueprint as INWN2Blueprint;
			if (bp == null) throw new ArgumentException("blueprint must implement interface INWN2Blueprint.","blueprint");
			CreateNarrativeThreadsBlock(bp);
		}
		
		
		public void CreateNarrativeThreadsBlock(INWN2Blueprint blueprint)
		{
			if (blueprint == null) throw new ArgumentNullException("blueprint");
			
			// Wait for up to 3 seconds for the image to appear:
			WaitForNarrativeThreadsImage(blueprint.Resource.ResRef.Value,3000);
			
			Action action = new Action
			(
				delegate()
				{					
					string bag = String.Format(InstanceBagNamingFormat,blueprint.ObjectType.ToString());
					
					if (manager.HasBag(bag)) {
						ObjectBlock block = blocks.CreateInstanceBlockFromBlueprint(blueprint);
						manager.AddMoveable(bag,block);		
					}
				}
			);					
					
			uiThreadAccess.Dispatcher.Invoke(DispatcherPriority.Normal,action);
		}
		
		
		protected void WaitForNarrativeThreadsImage(string resref, int wait)
		{			
			if (wait <= 0) return;
			
			int interval = 100;
			
			while (wait >= 0 && !nt.HasImage(resref)) {
				Thread.Sleep(interval);
				wait -= interval;
			}
		}
		
		
		protected System.Windows.Controls.Label uiThreadAccess = new System.Windows.Controls.Label();
		public void CreateBlocksWhenAreaIsReady(object area)
		{
			NWN2Toolset.NWN2.Data.NWN2GameArea nwn2Area = area as NWN2Toolset.NWN2.Data.NWN2GameArea;
			if (nwn2Area == null) throw new ArgumentException("Parameter was not of type NWN2Toolset.NWN2.Data.NWN2GameArea.","area");
			
			int wait = 3000;
			int interval = 100;
			
			while (wait >= 0 && nwn2Area.Tag == null) {
				Thread.Sleep(interval);
				wait -= interval;
			}
			
			if (nwn2Area.Tag == null) {
				throw new ApplicationException("The last-opened area took too long to load, and could not be shown as a Flip block.");
			}
			
			// Once loaded, ensure that area always has the same resource name and tag:	
			if (nwn2Area.Tag != nwn2Area.Name) nwn2Area.Tag = nwn2Area.Name;			
			OEIShared.Utils.OEIExoLocString oeiStr = Nwn2Strings.GetOEIStringFromString(nwn2Area.Name);
			if (nwn2Area.DisplayName != oeiStr) nwn2Area.DisplayName = oeiStr;
			
			Action action = new Action
			(
				delegate()
				{
					ObjectBlock areaBlock = blocks.CreateAreaBlock(nwn2Area);
					manager.AddMoveable(AreasBagName,areaBlock);
					
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