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
using NWN2Toolset.NWN2.Data.Templates;
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
		
		
		public Nwn2MoveableProvider(Nwn2BlockFactory blocks, Nwn2StatementFactory statements)
		{
			if (blocks == null) throw new ArgumentNullException("blocks");	
			if (statements == null) throw new ArgumentNullException("statements");	
			
			this.blocks = blocks;
			this.statements = statements;
		}
		
		
		public override void Populate(IMoveableManager manager)
		{
			if (manager == null) throw new ArgumentNullException("manager");
			
			CreateBags(manager);
			CreateStatements(manager);
			CreateBlocks(manager);
		}
		
		
		protected void CreateBags(IMoveableManager manager)
		{
			manager.AddBag("Actions");
			manager.AddBag("Conditions");
			manager.AddBag("Other");
			 
			string[] nwn2Types = Enum.GetNames(typeof(NWN2ObjectType));
			
			foreach (string nwn2Type in nwn2Types) {
				manager.AddBag(String.Format("{0} blueprints",nwn2Type));
			}
			foreach (string nwn2Type in nwn2Types) {
				manager.AddBag(String.Format("{0} instances",nwn2Type));
			}			
		}
		
		
		protected void CreateStatements(IMoveableManager manager)
		{	
			foreach (Statement statement in statements.GetActions()) {
				manager.AddMoveable("Actions",statement);
			}					
			foreach (Statement statement in statements.GetConditions()) {
				manager.AddMoveable("Conditions",statement);
			}
		}
		
		
		protected void CreateBlocks(IMoveableManager manager)
		{
			manager.AddMoveable("Other",blocks.CreatePlayerBlock());
			manager.AddMoveable("Other",blocks.CreateModuleBlock());
			PopulateBlueprints();
			PopulateInstances();
		}
		
		
		protected void PopulateBlueprints()
		{
//			foreach (NWN2ObjectType type in Enum.GetValues(typeof(NWN2ObjectType))) {
//				StackPanel panel = blueprintPanels[type];
//				foreach (INWN2Blueprint blueprint in NWN2GlobalBlueprintManager.GetBlueprintsOfType(type,true,true,true)) {
//					ObjectBlock block = factory.CreateBlueprintBlock(blueprint);
//					panel.Children.Add(block);
//				}
//			}
		}
		
		
		protected void PopulateInstances()
		{
//			NWN2GameArea activeArea = NWN2ToolsetMainForm.App.AreaContents.Area;
//			if (activeArea != null) {				
//				foreach (NWN2ObjectType type in Enum.GetValues(typeof(NWN2ObjectType))) {
//					StackPanel panel = instancePanels[type];
//					foreach (INWN2Instance instance in activeArea.GetInstancesForObjectType(type)) {
//						ObjectBlock block = factory.CreateInstanceBlock(instance);
//						panel.Children.Add(block);
//					}
//				}
//			}
		}
	}
}