/*
 * Flip - a visual programming language for scripting video games
 * Copyright (C) 2009 University of Sussex
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
 * This file added by Keiron Nicholson on 10/08/2009 at 17:27.
 */

using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using NWN2Toolset;
using NWN2Toolset.NWN2.Data;
using NWN2Toolset.NWN2.Data.Instances;
using OEIShared.IO;
using Sussex.Flip.Games.NeverwinterNightsTwo.Plugin;
using Sussex.Flip.Games.NeverwinterNightsTwo.Utils;

namespace Sussex.Flip.Games.NeverwinterNightsTwo.Plugin.Tests
{
	/// <summary>
	/// Tests the <see cref="FlipPlugin"/> class.
	/// </summary>
	[TestFixture]
	public class FlipPluginTests
	{
		#region Fields
		
		protected FlipPlugin flipPlugin;
		
		#endregion
		
		#region Setup
		
		[TestFixtureSetUp]
		public void Init()
		{			
			//flipPlugin = new FlipPlugin();
			
			
		}
		
		
		[TestFixtureTearDown]
		public void Dispose()
		{
		}
		
		#endregion
		
		#region Tests
		
		[Test]
		public void ReturnsObjectsOfSpecifiedTypeTagAndArea()
		{
			// Also check for:
			// 'goblin1' creature in wrong area - return 1 object not 2
			// 'goblin1' creature in any area - return 2 objects
			// ...more...
			// Also probably split into separate methods.
			// Also potentially deal with deleting module afterwards.
				
			
			// Create a module - this is currently not possible
			// as a result of Neverwinter Nights 2 not being installed.
			                        
//			// Create the module object:
//			string moduleName = "the module";
//            NWN2GameModule mod = new NWN2GameModule();
//            mod.Name = moduleName;
//            mod.LocationType = NWN2Toolset.NWN2.IO.ModuleLocationType.Directory;
//            mod.FileName = System.IO.Path.Combine(NWN2ToolsetMainForm.ModulesDirectory,mod.Name);
//            mod.ModuleInfo.XPScale = 0; // player will gain no experience


            // Write the module to disk:
            // Serialize(mod);

            //ModuleHelper.Open(moduleName,false); // don't open the Scratchpad till it's been created

                 

			
            NWN2GameModule module = new NWN2GameModule();
			
			
			string name = module.Name;
			string path = System.IO.Path.Combine(NWN2ToolsetMainForm.ModulesDirectory,name);
			DirectoryResourceRepository repository = new DirectoryResourceRepository(name);
			
			NWN2GameArea area1 = new NWN2GameArea("area1",path,repository);
			module.AddResource(area1);
			
			NWN2CreatureInstance goblin1 = new NWN2CreatureInstance();
			goblin1.Tag = "goblin1";
			area1.AddInstance(goblin1);
						
			NWN2CreatureInstance goblin2 = new NWN2CreatureInstance();
			goblin2.Tag = "goblin2";
			area1.AddInstance(goblin2);
			
			NWN2CreatureInstance goblin3 = new NWN2CreatureInstance();
			goblin3.Tag = "goblin3";
			area1.AddInstance(goblin3);
			
			// 'goblin1' tag recurs on a different object type:
			NWN2TreeInstance tree1 = new NWN2TreeInstance();
			tree1.Tag = "goblin1"; 
			area1.AddInstance(tree1);
			
			// 'item' tag recurs on the same object type:
			NWN2ItemInstance item1 = new NWN2ItemInstance();
			item1.Tag = "item";
			area1.AddInstance(item1);
			
			NWN2ItemInstance item2 = new NWN2ItemInstance();
			item2.Tag = "item";
			area1.AddInstance(item2);
			
			NWN2ItemInstance item3 = new NWN2ItemInstance();
			item3.Tag = "item";
			area1.AddInstance(item3);
			
			NWN2ItemInstance item4 = new NWN2ItemInstance();
			item4.Tag = "sword";
			area1.AddInstance(item4);
			
			// 'special' tag recurs on different object types:
			NWN2TreeInstance tree2 = new NWN2TreeInstance();
			tree2.Tag = "special";
			area1.AddInstance(tree2);
			
			NWN2EncounterInstance encounter1 = new NWN2EncounterInstance();
			encounter1.Tag = "special";
			area1.AddInstance(encounter1);
			
			NWN2DoorInstance door1 = new NWN2DoorInstance();
			door1.Tag = "special";
			area1.AddInstance(door1);
			
			Assert.IsNotNull(module);
			
			
			
			
			
			NWN2GameArea area = module.Areas["area1"];
			object[] objects;
			string tag;
			Nwn2EventRaiser type;
			int expected;

			// Get the single object which has tag 'goblin1' and is a creature:
			tag = "goblin1";
			type = Nwn2EventRaiser.Creature;
			expected = 1;
			
			objects = flipPlugin.GetObject(area,type,tag);
			
			Assert.IsNotNull(objects,"Returned null instead of an array of objects.");
			Assert.AreEqual(objects.Length,expected,expected.ToString() +
			                " object(s) should have been returned.");
			
			NWN2CreatureInstance creature = objects[0] as NWN2CreatureInstance;
			Assert.IsNotNull(creature,"A NWN2CreatureInstance object should have been returned.");			
			Assert.AreEqual(tag,creature.Tag,"A creature with tag '" + tag + "' " +
			               "should have been returned.");
			
			// Get the three objects which have tag 'item' and are items:
			tag = "item";
			type = Nwn2EventRaiser.Item;
			expected = 3;
			
			objects = flipPlugin.GetObject(area,type,tag);
			
			Assert.IsNotNull(objects,"Returned null instead of an array of objects.");
			Assert.AreEqual(objects.Length,expected,expected.ToString() +
			                " object(s) should have been returned.");
			
			foreach (object o in objects) {
				NWN2ItemInstance item = o as NWN2ItemInstance;
				Assert.IsNotNull(item,"NWN2ItemInstance objects should have been returned.");
				Assert.AreEqual(tag,item.Tag,"Items with tag '" + tag + "' " +
				                "should have been returned.");
			}			
		}
		
		#endregion
		
		#region Methods
		
		/// <summary>
		/// Create a sample Neverwinter Nights 2 game module
		/// to run tests with.
		/// </summary>
		/// <returns>A sample game module.</returns>
		protected NWN2GameModule CreateTestModule()
		{
			NWN2ToolsetMainForm.App.DoNewModule(false);
			System.Threading.Thread.Sleep(10000);
			NWN2GameModule module = NWN2ToolsetMainForm.App.Module;
			
			
			string name = module.Name;
			string path = System.IO.Path.Combine(NWN2ToolsetMainForm.ModulesDirectory,name);
			DirectoryResourceRepository repository = new DirectoryResourceRepository(name);
			
			NWN2GameArea area1 = new NWN2GameArea("area1",path,repository);
			module.AddResource(area1);
			
			NWN2CreatureInstance goblin1 = new NWN2CreatureInstance();
			goblin1.Tag = "goblin1";
			area1.AddInstance(goblin1);
						
			NWN2CreatureInstance goblin2 = new NWN2CreatureInstance();
			goblin2.Tag = "goblin2";
			area1.AddInstance(goblin2);
			
			NWN2CreatureInstance goblin3 = new NWN2CreatureInstance();
			goblin3.Tag = "goblin3";
			area1.AddInstance(goblin3);
			
			// 'goblin1' tag recurs on a different object type:
			NWN2TreeInstance tree1 = new NWN2TreeInstance();
			tree1.Tag = "goblin1"; 
			area1.AddInstance(tree1);
			
			// 'item' tag recurs on the same object type:
			NWN2ItemInstance item1 = new NWN2ItemInstance();
			item1.Tag = "item";
			area1.AddInstance(item1);
			
			NWN2ItemInstance item2 = new NWN2ItemInstance();
			item2.Tag = "item";
			area1.AddInstance(item2);
			
			NWN2ItemInstance item3 = new NWN2ItemInstance();
			item3.Tag = "item";
			area1.AddInstance(item3);
			
			NWN2ItemInstance item4 = new NWN2ItemInstance();
			item4.Tag = "sword";
			area1.AddInstance(item4);
			
			// 'special' tag recurs on different object types:
			NWN2TreeInstance tree2 = new NWN2TreeInstance();
			tree2.Tag = "special";
			area1.AddInstance(tree2);
			
			NWN2EncounterInstance encounter1 = new NWN2EncounterInstance();
			encounter1.Tag = "special";
			area1.AddInstance(encounter1);
			
			NWN2DoorInstance door1 = new NWN2DoorInstance();
			door1.Tag = "special";
			area1.AddInstance(door1);
			
			Assert.IsNotNull(module);
			
			return module;
		}
		
		#endregion
	}
}
