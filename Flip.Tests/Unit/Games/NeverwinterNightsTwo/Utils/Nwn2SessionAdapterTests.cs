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
 * This file added by Keiron Nicholson on 23/09/2009 at 16:19.
 */


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.ServiceModel;
using System.Threading;
using System.Windows.Automation;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Sussex.Flip.Games.NeverwinterNightsTwo.Utils;
using Sussex.Flip.Utils;
using NWN2Toolset;
using NWN2Toolset.NWN2.IO;
using NWN2Toolset.NWN2.Data;
using NWN2Toolset.NWN2.Data.Templates;
using OEIShared.IO.GFF;

namespace Sussex.Flip.Games.NeverwinterNightsTwo.Utils.Tests
{	
	/// <summary>
	/// Tests the <see cref="Nwn2SessionAdapter"/> class.
	/// </summary>
	/// <remarks>Not tested: whether callbacks fire when areas are removed,
	/// objects are removed, blueprints are added or removed, or script slots
	/// change.
	/// (If something hasn't been tested, it probably means that there was no service method
	/// available to allow automated testing, and it didn't seem worth implementing
	/// one for the test alone.)</remarks>
	[TestFixture]
	public sealed partial class Nwn2SessionAdapterTests
	{
		#region Fields
		
		private ChannelFactory<INwn2Service> pipeChannelFactory;
		
		private PathChecker pathChecker;
		
		private ResourceWriter resourceWriter;
		
		private SampleScriptProvider sampleScripts;
		
		private INwn2Service service;
		
		private string precompiled99bottlesScriptPath;
		
		private string uncompiled99bottlesScriptPath;
		
		private bool LaunchAndExitToolset = true;
		
		private Nwn2CallbacksForTesting testCallbacks;
		
		#endregion
		
		#region Setup
				
		[TestFixtureSetUp]
		public void Init()
		{			
			try {
				// Don't try to delete modules files at the start of the test fixture
				// as it causes IO access exceptions with the NWN2 toolset.
				// Update: No longer sure whether this was true, may have been
				// another issue...
				
				pathChecker = new PathChecker();
				resourceWriter = new ResourceWriter();
				sampleScripts = new SampleScriptProvider();
				
				precompiled99bottlesScriptPath = Path.Combine(Path.GetTempPath(),"99bottles.NCS");
				precompiled99bottlesScriptPath = pathChecker.GetUnusedFilePath(precompiled99bottlesScriptPath);
				uncompiled99bottlesScriptPath = Path.Combine(Path.GetTempPath(),"99bottles.NSS");
				uncompiled99bottlesScriptPath = pathChecker.GetUnusedFilePath(uncompiled99bottlesScriptPath);
				
				System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
				resourceWriter.Write("99bottles_Compiled",assembly,precompiled99bottlesScriptPath);
				resourceWriter.Write("99bottles_Uncompiled",assembly,uncompiled99bottlesScriptPath);
												
				if (LaunchAndExitToolset) {
					Console.WriteLine("Waiting for toolset to load...");
					WaitForToolsetToLoad(true);
				}
				
				testCallbacks = new Nwn2CallbacksForTesting();
				InstanceContext instanceContext = new InstanceContext(testCallbacks);
								
				NetNamedPipeBinding binding = new NetNamedPipeBinding();
				binding.MaxReceivedMessageSize = Int32.MaxValue;
				pipeChannelFactory = new DuplexChannelFactory<INwn2Service>(instanceContext,binding,"net.pipe://localhost/NamedPipeEndpoint");									
				CreateService();
				
				Console.WriteLine("Toolset loaded. Running test suite.");
			}
			catch (Exception e) {
				System.Windows.MessageBox.Show("Error setting up test suite. " + Environment.NewLine + Environment.NewLine + e);
				throw e;
			}
		}
		
		
		[TestFixtureTearDown]
		public void Dispose()
		{
			if (LaunchAndExitToolset) {
				Console.WriteLine("Test suite completed. Closing toolset.");
				Nwn2ToolsetFunctions.KillNeverwinterNightsTwoToolset();
			}
			
			if (File.Exists(precompiled99bottlesScriptPath)) File.Delete(precompiled99bottlesScriptPath);
			if (File.Exists(uncompiled99bottlesScriptPath)) File.Delete(uncompiled99bottlesScriptPath);
			
			Thread.Sleep(250);
		}
		
		#endregion
		
		#region Tests - Scripts
			
		[Test]
		public void NotifiesWhenModuleChanges()
		{
			testCallbacks.Callbacks.Clear();
			
			string name = "NotifiesWhenModuleChanges.mod";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedFilePath(path);
			
			string moduleName = Path.GetFileNameWithoutExtension(path);
			
			service.CreateModule(path,ModuleLocationType.File);
			service.OpenModule(path,ModuleLocationType.File);
			
			// When module changes, module name doesn't seem to be available yet, unless
			// it's a temp module... this isn't much of a problem, so just ignore it:
			string expected = "module changed";
			string actual = testCallbacks.Callbacks.Peek();
			Assert.IsTrue(actual.StartsWith(expected),"Expected to start with: '" + expected + "', Actual: '" + actual + "'");
			
			service.CloseModule();
			
			expected = "module changed... Now: temp";
			actual = testCallbacks.Callbacks.Peek();
			Assert.IsTrue(actual.StartsWith(expected),"Expected to start with: '" + expected + "', Actual: '" + actual + "'");
			expected = "Before: " + moduleName;
			Assert.IsTrue(actual.EndsWith(expected),"Expected to end with: '" + expected + "', Actual: '" + actual + "'");
			
			Delete(path);
		}
		
		
		[Test]
		public void NotifiesWhenAreaListChanges()
		{
			testCallbacks.Callbacks.Clear();
			
			string name = "NotifiesWhenAreaListChanges.mod";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedFilePath(path);
			
			string moduleName = Path.GetFileNameWithoutExtension(path);
			
			service.CreateModule(path,ModuleLocationType.File);
			service.OpenModule(path,ModuleLocationType.File);
			
			service.AddArea("area1",true,AreaFacade.SmallestAreaSize);
			testCallbacks.Callbacks.Pop(); // get rid of the 'area viewer opened' message
			Assert.AreEqual("added resource... Type: Area, Name: area1",testCallbacks.Callbacks.Peek());
			
			service.AddArea("area2",true,AreaFacade.SmallestAreaSize);		
			testCallbacks.Callbacks.Pop(); // get rid of the 'area viewer opened' message	
			Assert.AreEqual("added resource... Type: Area, Name: area2",testCallbacks.Callbacks.Peek());
			
			// Removing areas has not been tested as there is no service method for this.
			
			service.CloseModule();
			Delete(path);
		}
		
		
		[Test]
		public void NotifiesWhenScriptListChanges()
		{
			testCallbacks.Callbacks.Clear();
			
			string name = "NotifiesWhenScriptListChanges.mod";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedFilePath(path);
			
			string moduleName = Path.GetFileNameWithoutExtension(path);
			
			service.CreateModule(path,ModuleLocationType.File);
			service.OpenModule(path,ModuleLocationType.File);
			
			service.AddScript("givegold",sampleScripts.GiveGold);
			Assert.AreEqual("added resource... Type: Script, Name: givegold",testCallbacks.Callbacks.Peek());
			
			service.AddScript("sing",sampleScripts.Sing);
			Assert.AreEqual("added resource... Type: Script, Name: sing",testCallbacks.Callbacks.Peek());
			
			service.DeleteScript("givegold");
			Assert.AreEqual("removed resource... Type: Script, Name: givegold",testCallbacks.Callbacks.Peek());
			
			service.DeleteScript("sing");
			Assert.AreEqual("removed resource... Type: Script, Name: sing",testCallbacks.Callbacks.Peek());
			
			service.CloseModule();
			Delete(path);
		}
		
		
		[Test]
		public void NotifiesWhenObjectListChanges()
		{
			testCallbacks.Callbacks.Clear();
			
			string name = "NotifiesWhenObjectListChanges.mod";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedFilePath(path);
			
			string moduleName = Path.GetFileNameWithoutExtension(path);
			
			service.CreateModule(path,ModuleLocationType.File);
			service.OpenModule(path,ModuleLocationType.File);
			
			service.AddArea("area1",true,AreaFacade.SmallestAreaSize);
					
			string expected, actual;
			
			service.AddObject("area1",NWN2ObjectType.Creature,"c_cat","cat1");
			expected = "added object... Type: Creature, Tag: cat1";
			actual = testCallbacks.Callbacks.Peek();
			Assert.IsTrue(actual.StartsWith(expected),"Expected to start with: '" + expected + "', Actual: '" + actual + "'");
			
			service.AddObject("area1",NWN2ObjectType.Item,"mst_swgs_drk_3","sword");			
			expected = "added object... Type: Item, Tag: sword";
			actual = testCallbacks.Callbacks.Peek();
			Assert.IsTrue(actual.StartsWith(expected),"Expected to start with: '" + expected + "', Actual: '" + actual + "'");			
			
			// Removing objects has not been tested as there is no service method for this.
			
			service.CloseModule();
			Delete(path);
		}
		
		
		[Test]
		public void NotifiesWhenAreasOpenOrClose()
		{
			testCallbacks.Callbacks.Clear();
			
			string name = "NotifiesWhenAreasOpenOrClose.mod";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedFilePath(path);
			
			string moduleName = Path.GetFileNameWithoutExtension(path);
			
			service.CreateModule(path,ModuleLocationType.File);
			service.OpenModule(path,ModuleLocationType.File);
			
			service.AddArea("area1",true,AreaFacade.SmallestAreaSize);
			Assert.AreEqual("opened Area 'area1'",testCallbacks.Callbacks.Peek());
			
			service.AddArea("area2",true,AreaFacade.SmallestAreaSize);		
			Assert.AreEqual("opened Area 'area2'",testCallbacks.Callbacks.Peek());
			
			service.CloseArea("area2");		
			Assert.AreEqual("closed resource 'area2'",testCallbacks.Callbacks.Peek());
			
			service.OpenArea("area2");		
			Assert.AreEqual("opened Area 'area2'",testCallbacks.Callbacks.Peek());
			
			service.CloseModule();
			Delete(path);
		}
		
		
		[Test]
		public void NotifiesWhenScriptsOpenOrClose()
		{
			testCallbacks.Callbacks.Clear();
			
			string name = "NotifiesWhenScriptsOpenOrClose.mod";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedFilePath(path);
			
			string moduleName = Path.GetFileNameWithoutExtension(path);
			
			service.CreateModule(path,ModuleLocationType.File);
			service.OpenModule(path,ModuleLocationType.File);
			
			service.AddScript("script1",sampleScripts.GiveGold);
			service.AddScript("scriptx",sampleScripts.Sing);
			
			service.OpenScript("script1");
			Assert.AreEqual("opened Script 'script1'",testCallbacks.Callbacks.Peek());
			
			service.OpenScript("scriptx");
			Assert.AreEqual("opened Script 'scriptx'",testCallbacks.Callbacks.Peek());
			
			service.CloseScript("scriptx");		
			Assert.AreEqual("closed resource 'scriptx'",testCallbacks.Callbacks.Peek());
			
			service.CloseModule();
			Delete(path);
		}
		
		
		[Test]
		public void ScriptMethodsWorkRegardlessOfLoadedState()
		{
			string name = "ScriptMethodsWorkRegardlessOfLoadedState.mod";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedFilePath(path);
			
			service.CreateModule(path,ModuleLocationType.File);
			service.OpenModule(path,ModuleLocationType.File);
						
			string areaName = "area";
			service.AddArea(areaName,false,AreaFacade.SmallestAreaSize);			
			service.AddObject(areaName,NWN2ObjectType.Creature,"c_werewolf","werewolf");
			
			string scriptName = "givegold";
			
			// Scripts are not loaded upon creation:
			service.AddScript(scriptName,sampleScripts.GiveGold);
			Assert.AreEqual("False",service.GetScript(scriptName)["Loaded"]);
			service.CompileScript(scriptName);
			Assert.IsTrue(WaitForCompiledScriptToAppear(scriptName));	
			
			Assert.AreEqual("False",service.GetScript(scriptName)["Loaded"]); // CompileScript() doesn't Demand() the script		
			service.AttachScriptToArea(scriptName,areaName,"OnEnterScript");
			Bean area = service.GetArea(areaName,false);
			Assert.AreEqual(scriptName,area["OnEnterScript"]);
			
			service.DeleteScript(scriptName);
			
			// Try again with a loaded script:
			service.AddScript(scriptName,sampleScripts.GiveGold);
			service.DemandScript(scriptName);
			Assert.AreEqual("True",service.GetScript(scriptName)["Loaded"]);
			service.CompileScript(scriptName);
			Assert.IsTrue(WaitForCompiledScriptToAppear(scriptName));	
			
			Assert.AreEqual("True",service.GetScript(scriptName)["Loaded"]);
			service.AttachScriptToArea(scriptName,areaName,"OnExitScript");
			area = service.GetArea(areaName,false);
			Assert.AreEqual(scriptName,area["OnExitScript"]);
			
			service.CloseModule();
			Delete(path);	
		}
		
		
		[Test]
		public void GetsBlueprint()
		{
			string name = "GetsBlueprint.mod";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedFilePath(path);
			
			service.CreateModule(path,ModuleLocationType.File);
			service.OpenModule(path,ModuleLocationType.File);
			
			string blueprintResRef = "c_cat";
			NWN2ObjectType blueprintType = NWN2ObjectType.Creature;
			
			Bean cat = service.GetBlueprint(blueprintResRef,blueprintType,false);
			Assert.IsNotNull(cat);
			Assert.AreEqual("Creature",cat["ObjectType"]);
			Assert.AreEqual("Cats are typically kept for their abilities to dispose of vermin " +
			                "like rats and mice. They are also a common familiar of wizards and sorcerers.",
			                cat["LocalizedDescription"]);
			
			blueprintType = NWN2ObjectType.Door;
			Assert.IsNull(service.GetBlueprint(blueprintResRef,blueprintType,false));			
			
			service.CloseModule();
			Delete(path);
		}
		
			
		[Test]
		public void GetsBlueprints()
		{	
			string name = "GetsBlueprints.mod";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedFilePath(path);
			
			service.CreateModule(path,ModuleLocationType.File);
			service.OpenModule(path,ModuleLocationType.File);
						
			bool foundBlackDragon = false;
			bool foundSunkenFlagon = false;
			
			foreach (NWN2ObjectType type in Enum.GetValues(typeof(NWN2ObjectType))) {
				IList<string> resRefs = service.GetBlueprintResRefs(type);
				Assert.IsNotNull(resRefs);				
				IList<Bean> beans = new List<Bean>(resRefs.Count);
				foreach (string resRef in resRefs) {
					Bean bean = service.GetBlueprint(resRef,type,false);
					Assert.IsNotNull(bean);
					beans.Add(bean);
					if (!foundBlackDragon && type == NWN2ObjectType.Creature 
					    && bean.HasValue("LocalizedName") && bean["LocalizedName"] == "Black Dragon") 
					{
						foundBlackDragon = true;
					}
					if (!foundSunkenFlagon && type == NWN2ObjectType.Placeable 
					    && bean.HasValue("LocalizedName") && bean["LocalizedName"] == "Sunken Flagon") 
					{
						foundSunkenFlagon = true;
					}
				}
				
				int expected;
				switch (type) {
					case NWN2ObjectType.Creature:
						expected = 322;
						break;
					case NWN2ObjectType.Door:
						expected = 62;
						break;
					case NWN2ObjectType.Encounter:
						expected = 17;
						break;
					case NWN2ObjectType.Environment:
						expected = 0;
						break;
					case NWN2ObjectType.Item:
						expected = 2396;
						break;
					case NWN2ObjectType.Light:
						expected = 1;
						break;
					case NWN2ObjectType.Placeable:
						expected = 1090;
						break;
					case NWN2ObjectType.PlacedEffect:
						expected = 31;
						break;
					case NWN2ObjectType.Prefab:
						expected = 43;
						break;
					case NWN2ObjectType.Sound:
						expected = 374;
						break;
					case NWN2ObjectType.StaticCamera:
						expected = 1;
						break;
					case NWN2ObjectType.Store:
						expected = 15;
						break;
					case NWN2ObjectType.Tree:
						expected = 69;
						break;
					case NWN2ObjectType.Trigger:
						expected = 181;
						break;
					case NWN2ObjectType.Waypoint:
						expected = 27;
						break;
					default:
						throw new InvalidOperationException("Unrecognised value of NWN2ObjectType (" + type + ") in test.");
				}
				Assert.AreEqual(expected,resRefs.Count);
				Assert.AreEqual(expected,beans.Count);
			}
			
			Assert.IsTrue(foundBlackDragon,"Failed to find an expected blueprint (black dragon).");
			Assert.IsTrue(foundSunkenFlagon,"Failed to find an expected blueprint (sunken flagon).");
			
			service.CloseModule();
			Delete(path);
		}
		
		
		[Test]
		public void CompiledScriptAppearsWithinOneSecond()
		{
			string name = "CompiledScriptAppearsWithinOneSecond.mod";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedFilePath(path);
			
			service.CreateModule(path,ModuleLocationType.File);
			service.OpenModule(path,ModuleLocationType.File);
			
			string scriptName = "givegold";
			service.AddScript(scriptName,sampleScripts.GiveGold);
			
			int timeout = 1000;			
			service.CompileScript(scriptName);
			
			while (!service.HasCompiled(scriptName) && timeout >= 0) {
				Thread.Sleep(25);
				timeout -= 25;
			}
			Assert.IsTrue(service.HasCompiled(scriptName),"CompileScript() never produced a script file.");
			
			service.CloseModule();
			Delete(path);
		}
		
		
		[Test]
		public void ClearsScriptSlotOnModule()
		{
			string name = "ClearsScriptSlotOnModule.mod";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedFilePath(path);
			
			service.CreateModule(path,ModuleLocationType.File);
			service.OpenModule(path,ModuleLocationType.File);
			
			string scriptName = "givegold";
			service.AddScript(scriptName,sampleScripts.GiveGold);						
			service.CompileScript(scriptName);
			Assert.IsTrue(WaitForCompiledScriptToAppear(scriptName),"The compiled script file was never found.");
			
			service.AttachScriptToModule(scriptName,"OnCutsceneAbort");
			
			Bean module = service.GetModule();	
			
			string attachedScriptName = module.GetValue("OnCutsceneAbort");
			Assert.AreEqual(scriptName,attachedScriptName);
			
			service.ClearScriptSlotOnModule("OnCutsceneAbort");
			
			module = service.GetModule();
			attachedScriptName = module.GetValue("OnCutsceneAbort");
			Assert.IsNotNull(attachedScriptName);
			Assert.IsEmpty(attachedScriptName);
			
			// Refuses to clear unknown script slot:		
			try {
				service.ClearScriptSlotOnModule("BishBashBosh");
				Assert.Fail("Didn't raise a FaultException<ArgumentException> when asked to clear " +
			            	"a non-existent script slot.");
			}
			catch (FaultException<ArgumentException>) {
				// expected result
			}
			catch (FaultException) {
				CreateService();
				Assert.Fail("Didn't raise a FaultException<ArgumentException> when asked to clear " +
			            	"a non-existent script slot.");
			}
			
			service.CloseModule();
			Delete(path);
		}
		
		
		[Test]
		public void ClearsScriptSlotOnArea()
		{
			string name = "ClearsScriptSlotOnArea.mod";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedFilePath(path);
			
			service.CreateModule(path,ModuleLocationType.File);
			service.OpenModule(path,ModuleLocationType.File);
			
			string areaName = "forest";
			service.AddArea(areaName,true,AreaBase.SmallestAreaSize);
			
			string scriptName = "givegold";
			service.AddScript(scriptName,sampleScripts.GiveGold);
			service.CompileScript(scriptName);	
			Assert.IsTrue(WaitForCompiledScriptToAppear(scriptName),"The compiled script file was never found.");
						
			service.AttachScriptToArea(scriptName,areaName,"OnExitScript");
			
			Bean area = service.GetArea(areaName,false);
			Assert.IsNotNull(area);			
			
			string attachedScriptName = area.GetValue("OnExitScript");
			Assert.AreEqual(scriptName,attachedScriptName);
			
			service.ClearScriptSlotOnArea(areaName,"OnExitScript");
			
			area = service.GetArea(areaName,false);
			attachedScriptName = area.GetValue("OnExitScript");
			Assert.IsNotNull(attachedScriptName);
			Assert.IsEmpty(attachedScriptName);
			
			// Refuses to clear unknown script slot:		
			try {
				service.ClearScriptSlotOnArea(areaName,"BishBashBosh");
				Assert.Fail("Didn't raise a FaultException<ArgumentException> when asked to clear " +
			            	"a non-existent script slot.");
			}
			catch (FaultException<ArgumentException>) {
				// expected result
			}
			catch (FaultException) {
				CreateService();
				Assert.Fail("Didn't raise a FaultException<ArgumentException> when asked to clear " +
			            	"a non-existent script slot.");
			}
			
			service.CloseModule();
			Delete(path);
		}
		
		
		[Test]
		public void ClearsScriptSlotOnObject()
		{
			string name = "ClearsScriptSlotOnObject.mod";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedFilePath(path);
			
			service.CreateModule(path,ModuleLocationType.File);
			service.OpenModule(path,ModuleLocationType.File);
			
			string areaName = "forest";
			service.AddArea(areaName,true,AreaBase.SmallestAreaSize);			
			Guid doorID = service.AddObject(areaName,NWN2ObjectType.Door,"plc_dc_basic01","door");
			
			string scriptName = "givegold";
			service.AddScript(scriptName,sampleScripts.GiveGold);
			service.CompileScript(scriptName);
			Assert.IsTrue(WaitForCompiledScriptToAppear(scriptName),"The compiled script file was never found.");
			
			service.AttachScriptToObject(scriptName,areaName,NWN2ObjectType.Door,doorID,"OnFailToOpen");
			
			Bean door = service.GetObject(areaName,NWN2ObjectType.Door,doorID,false);			
			string attachedScriptName = door.GetValue("OnFailToOpen");
			Assert.AreEqual(scriptName,attachedScriptName);
			
			service.ClearScriptSlotOnObject(areaName,doorID,NWN2ObjectType.Door,"OnFailToOpen");
			
			door = service.GetObject(areaName,NWN2ObjectType.Door,doorID,false);			
			attachedScriptName = door.GetValue("OnFailToOpen");
			Assert.IsNotNull(attachedScriptName);
			Assert.IsEmpty(attachedScriptName);
			
			// Refuses to clear unknown script slot:		
			try {
				service.ClearScriptSlotOnObject(areaName,doorID,NWN2ObjectType.Door,"BishBashBosh");
				Assert.Fail("Didn't raise a FaultException<ArgumentException> when asked to clear " +
			            	"a non-existent script slot.");
			}
			catch (FaultException<ArgumentException>) {
				// expected result
			}
			catch (FaultException) {
				CreateService();
				Assert.Fail("Didn't raise a FaultException<ArgumentException> when asked to clear " +
			            	"a non-existent script slot.");
			}
			
			service.CloseModule();
			Delete(path);
		}
		
				
		[Test]
		public void DeletesScript()
		{		
			string name = "DeleteScript.mod";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedFilePath(path);
			
			service.CreateModule(path,ModuleLocationType.File);
			service.OpenModule(path,ModuleLocationType.File);
			
			string scriptData = sampleScripts.Sing;
			string scriptName = Path.GetFileNameWithoutExtension(precompiled99bottlesScriptPath);
			
			// Deletes both compiled and uncompiled if present:
			service.AddScript(scriptName,sampleScripts.Sing);
			service.CompileScript(scriptName);
			Assert.IsTrue(WaitForCompiledScriptToAppear(scriptName),"The compiled script file was never found.");
			
			Assert.IsTrue(service.HasUncompiled(scriptName));
			
			service.DeleteScript(scriptName);
						
			Assert.IsFalse(service.HasCompiled(scriptName));
			Assert.IsFalse(service.HasUncompiled(scriptName));
			
			// Deletes compiled if only compiled is present:
			File.Copy(precompiled99bottlesScriptPath,Path.Combine(service.GetModuleTempPath(),Path.GetFileName(precompiled99bottlesScriptPath)));
			
			Assert.IsTrue(service.HasCompiled(scriptName));
			Assert.IsFalse(service.HasUncompiled(scriptName));
			
			service.DeleteScript(scriptName);
			
			Assert.IsFalse(service.HasCompiled(scriptName));
			Assert.IsFalse(service.HasUncompiled(scriptName));
			
			// Deletes uncompiled if only uncompiled is present:
			service.AddScript(scriptName,sampleScripts.Sing);
			
			Assert.IsFalse(service.HasCompiled(scriptName));
			Assert.IsTrue(service.HasUncompiled(scriptName));
			
			service.DeleteScript(scriptName);
			
			Assert.IsFalse(service.HasCompiled(scriptName));
			Assert.IsFalse(service.HasUncompiled(scriptName));
			
			// Refuses to delete non-existent script:
			string nonExistentScriptName = "does not exist";
			Assert.IsFalse(service.HasUncompiled(nonExistentScriptName));				
			try {
				service.DeleteScript(nonExistentScriptName);
				Assert.Fail("Didn't raise a FaultException<ArgumentException> when asked to delete " +
			            	"a non-existent script.");
			}
			catch (FaultException<ArgumentException>) {
				// expected result
			}
			catch (FaultException) {
				CreateService();
				Assert.Fail("Didn't raise a FaultException<ArgumentException> when asked to delete " +
			            	"a non-existent script.");
			}
			
			service.CloseModule();
			Delete(path);
		}
		
		
		[Test]
		public void GetsSerialisedInfoAboutObjects()
		{
			string name = "GetsSerialisedInfoAboutObjects.mod";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedFilePath(path);
			
			service.CreateModule(path,ModuleLocationType.File);
			service.OpenModule(path,ModuleLocationType.File);
					
			string area = "area";
			service.AddArea(area,true,AreaBase.SmallestAreaSize);
			
			service.AddObject(area,NWN2ObjectType.Creature,"c_cat","cat");
			service.AddObject(area,NWN2ObjectType.Creature,"c_giantfire","giant");
			service.AddObject(area,NWN2ObjectType.Item,"mst_swgs_drk_3","sword");
			
			IList<Guid> ids = service.GetObjectIDs(area,NWN2ObjectType.Creature);
			Assert.IsNotNull(ids);
			Assert.AreEqual(2,ids.Count);
			IList<Bean> beans = new List<Bean>(ids.Count);
			foreach (Guid id in ids) {
				Bean bean = service.GetObject(area,NWN2ObjectType.Creature,id,false);
				Assert.IsNotNull(bean);
				beans.Add(bean);
			}
					
			Bean cat = null, giant = null;
			
			foreach (Bean bean in beans) {
				Assert.IsTrue(bean.HasValue("Tag"),"Bean did not serialise tag as it was expected to.");
				
				string tag = bean.GetValue("Tag");
				Assert.IsNotNull(tag,"Bean did not serialise tag as it was expected to.");
				
				if (tag == "cat") {
					cat = bean;
				}
				else if (tag == "giant") {
					giant = bean;
				}
				else {
					Assert.Fail("No bean provided an expected tag (tag was '" + tag + "'.");
				}
			}
		
			Assert.IsNotNull(cat);
			Assert.IsNotNull(giant);
		
			Assert.IsTrue(cat.HasValue("FactionID"));
			Assert.AreEqual(2,int.Parse(cat.GetValue("FactionID")));
		
			Assert.IsTrue(giant.HasValue("FactionID"));
			Assert.AreEqual(1,int.Parse(giant.GetValue("FactionID")));			
			
			ids = service.GetObjectIDs(area,NWN2ObjectType.Item);
			Assert.IsNotNull(ids);
			Assert.AreEqual(1,ids.Count);
			beans = new List<Bean>(ids.Count);
			foreach (Guid id in ids) {
				Bean bean = service.GetObject(area,NWN2ObjectType.Item,id,false);
				Assert.IsNotNull(bean);
				beans.Add(bean);
			}
			
			Bean sword = null;
			foreach (Bean bean in beans) {
				Assert.IsTrue(bean.HasValue("Tag"),"Bean did not serialise tag as it was expected to.");
				
				string tag = bean.GetValue("Tag");
				Assert.IsNotNull(tag,"Bean did not serialise tag as it was expected to.");
				
				if (tag == "sword") {
					sword = bean;
				}
				else {
					Assert.Fail("No bean provided an expected tag (tag was '" + tag + "'.");
				}
			}
						
			Assert.AreEqual("Darksteel Greatsword",sword.GetValue("LocalizedName"));
			
			Assert.AreEqual("Darksteel is silvery in hue when polished or cut, but " +
			                "its exposed surfaces have a deep, gleaming purple luster. " +
			                "This alloy of meteoric iron and steel is tempered with rare, " +
			                "magical oils to give the metal its uncanny abilities. Darksteel " +
			                "is commonly used in the crafting of magic items related to storms " +
			                "or lightning.",sword.GetValue("LocalizedDescriptionIdentified"));
						
			ids = service.GetObjectIDsByTag(area,NWN2ObjectType.Creature,"giant");
			Assert.IsNotNull(ids);
			Assert.AreEqual(1,ids.Count);
			
			service.CloseModule();
			Delete(path);
		}
		
		
		[Test]
		public void GetsObjectGivenGuid()
		{
			string name = "GetsObjectGivenGuid.mod";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedFilePath(path);
			
			service.CreateModule(path,ModuleLocationType.File);
			service.OpenModule(path,ModuleLocationType.File);
					
			string area = "area";
			service.AddArea(area,true,AreaBase.SmallestAreaSize);
			for (int i = 0; i < 20; i++) {
				service.AddObject(area,NWN2ObjectType.Creature,"c_cat","cat" + i);
				service.AddObject(area,NWN2ObjectType.Item,"mst_swgs_drk_3","sword" + i);	
			}			
			
			IList<Guid> ids = service.GetObjectIDsByTag(area,NWN2ObjectType.Creature,"cat7");
			Assert.AreEqual(1,ids.Count);
			Bean cat = service.GetObject(area,NWN2ObjectType.Creature,ids[0],false);
			
			ids = service.GetObjectIDsByTag(area,NWN2ObjectType.Item,"sword4");
			Assert.AreEqual(1,ids.Count);
			Bean sword = service.GetObject(area,NWN2ObjectType.Item,ids[0],false);
			
			Assert.IsTrue(cat.HasValue("ObjectID"));
			Assert.IsTrue(sword.HasValue("ObjectID"));			
			string catID = cat.GetValue("ObjectID");
			string swordID = sword.GetValue("ObjectID");			
			Assert.IsNotEmpty(catID);
			Assert.IsNotEmpty(swordID);
			Assert.AreNotEqual(catID,swordID);
			
			Bean retrievedCat = service.GetObject(area,NWN2ObjectType.Creature,new Guid(catID),false);
			Bean retrievedSword = service.GetObject(area,NWN2ObjectType.Item,new Guid(swordID),false);
			
			Assert.AreEqual(cat,retrievedCat);
			Assert.AreEqual(sword,retrievedSword);
			
			service.CloseModule();
			Delete(path);
		}
		
		
		[Test]
		public void ProvidesFullSerialisationIfRequested()
		{
			string name = "ProvidesBothFullAndLimitedBeans.mod";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedFilePath(path);
			
			service.CreateModule(path,ModuleLocationType.File);
			service.OpenModule(path,ModuleLocationType.File);
			
			string areaName = "area";		
			service.AddArea(areaName,false,AreaFacade.SmallestAreaSize);
			Guid catID = service.AddObject(areaName,NWN2ObjectType.Creature,"c_cat","cat");
			Bean cat = service.GetObject(areaName,NWN2ObjectType.Creature,catID,false);	
			Assert.IsFalse(cat.HasValue("Strength"));
			cat = service.GetObject(areaName,NWN2ObjectType.Creature,catID,true);
			Assert.IsTrue(cat.HasValue("Strength"));
			Assert.AreEqual("3",cat["Strength"]);			
			
			Bean area = service.GetArea(areaName,false);
			Assert.IsFalse(area.HasValue("AmbientSoundDaytime"));
			area = service.GetArea(areaName,true);
			Assert.IsTrue(area.HasValue("AmbientSoundDaytime"));
			Assert.AreEqual("(No Ambient Sound)",area["AmbientSoundDaytime"]);
			
			Bean blueprint = service.GetBlueprint("angelicaf",NWN2ObjectType.Tree,false);
			Assert.IsFalse(blueprint.HasValue("CastsShadow"));
			blueprint = service.GetBlueprint("angelicaf",NWN2ObjectType.Tree,true);
			Assert.IsTrue(blueprint.HasValue("CastsShadow"));
			Assert.AreEqual("CAST_ALL",blueprint["CastsShadow"]);	
									
			service.CloseModule();
			Delete(path);
		}
		
		
		[Test]
		public void AddsUncompiledScriptToFileModule()
		{
			string name = "AddsUncompiledScriptToFileModule.mod";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedFilePath(path);
			
			service.CreateModule(path,ModuleLocationType.File);
			service.OpenModule(path,ModuleLocationType.File);
			
			IList<string> scripts = service.GetScriptNames();
			Assert.AreEqual(0,scripts.Count);
			
			// This script compiles in the toolset as entered here (hand-tested):
			string scriptData = sampleScripts.Sing;
			string scriptName = "uncompiled";
			
			service.AddScript(scriptName,scriptData);
			
			scripts = service.GetScriptNames();
			Assert.AreEqual(1,scripts.Count);
			Bean script = service.GetScript(scriptName);
			Assert.IsNotNull(script);
			
			Assert.AreEqual(scriptName,script.GetValue("Name"));
			Assert.AreEqual(scriptName + ".nss",script.GetValue("VersionControlName").ToLower());
			Assert.AreEqual(scriptData,script.GetValue("Data"));
					
			try {
				service.AddScript(scriptName,scriptData);
				Assert.Fail("Didn't raise a FaultException<ArgumentException> when asked to add " +
			            	"a script with an existing name.");
			}
			catch (FaultException<ArgumentException>) {
				// expected result
			}
			catch (FaultException) {
				CreateService();
				Assert.Fail("Didn't raise a FaultException<ArgumentException> when asked to add " +
			            	"a script with an existing name.");
			}
									
			service.CloseModule();
			Delete(path);
		}
		
		
		[Test]
		public void AddsUncompiledScriptToDirectoryModule()
		{
			string name = "AddsUncompiledScriptToDirectoryModule";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedDirectoryPath(path);
			
			service.CreateModule(path,ModuleLocationType.Directory);
			service.OpenModule(path,ModuleLocationType.Directory);
			
			IList<string> scripts = service.GetScriptNames();
			Assert.AreEqual(0,scripts.Count);
			
			// This script compiles in the toolset as entered here (hand-tested):
			string scriptData = sampleScripts.Sing;
			string scriptName = "uncompiled";
			
			service.AddScript(scriptName,scriptData);
			
			scripts = service.GetScriptNames();
			Assert.AreEqual(1,scripts.Count);
			Bean script = service.GetScript(scriptName);
			Assert.IsNotNull(script);
			
			Assert.AreEqual(scriptName,script.GetValue("Name"));
			Assert.AreEqual(scriptName + ".nss",script.GetValue("VersionControlName").ToLower());
			Assert.AreEqual(scriptData,script.GetValue("Data"));
					
			try {
				service.AddScript(scriptName,scriptData);
				Assert.Fail("Didn't raise a FaultException<ArgumentException> when asked to add " +
			            	"a script with an existing name.");
			}
			catch (FaultException<ArgumentException>) {
				// expected result
			}
			catch (FaultException) {
				CreateService();
				Assert.Fail("Didn't raise a FaultException<ArgumentException> when asked to add " +
			            	"a script with an existing name.");
			}
									
			service.CloseModule();
			Delete(path);
		}
		
		
		[Test]
		public void ReturnsModuleTempPathForFileModule()
		{
			string name = "ReturnsModuleTempPathForFileModule.mod";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedFilePath(path);
			
			service.CreateModule(path,ModuleLocationType.File);
			service.OpenModule(path,ModuleLocationType.File);
			
			string temp = service.GetModuleTempPath();
			Assert.IsNotNull(temp);
			Assert.IsNotEmpty(temp);
			Assert.IsTrue(Directory.Exists(temp));
			string folder = Path.GetFileName(temp);
			Assert.IsTrue(folder.StartsWith("temp"));
			Assert.IsTrue(folder.EndsWith(Path.GetFileNameWithoutExtension(path)),"Was: " + folder);
			
			service.SaveModule();
			
			Assert.IsFalse(Directory.Exists(temp)); // the old temp directory should now be gone
			
			temp = service.GetModuleTempPath();
			
			Assert.IsNotNull(temp);
			Assert.IsNotEmpty(temp);
			Assert.IsTrue(Directory.Exists(temp));			
			Assert.AreEqual(Path.GetFileNameWithoutExtension(path),Path.GetFileNameWithoutExtension(temp));
			
			service.CloseModule();
			Delete(path);
		}
		
		
		[Test]
		public void ReturnsModuleTempPathForDirectoryModule()
		{
			string name = "ReturnsModuleTempPathForDirectoryModule";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedDirectoryPath(path);
			
			service.CreateModule(path,ModuleLocationType.Directory);
			service.OpenModule(path,ModuleLocationType.Directory);
			
			Assert.AreEqual(service.GetModulePath(),service.GetModuleTempPath());
			
			service.SaveModule();
			
			Assert.AreEqual(service.GetModulePath(),service.GetModuleTempPath());
			
			service.CloseModule();
			Delete(path);
		}
		
		
		[Test]
		public void ReturnsModuleTempPathForTemporaryModule()
		{
			string path = service.CreateAndOpenTemporaryModule();
			
			string temp = service.GetModuleTempPath();
			Assert.IsNotNull(temp);
			Assert.IsNotEmpty(temp);
			Assert.IsTrue(Directory.Exists(temp));
			string folder = Path.GetFileName(temp);
			Assert.IsTrue(folder.StartsWith("temp"),"Was: " + folder);
			
			service.CloseModule();
		}
		
		
		[Test]
		public void ReportsWhetherScriptHasBeenCompiled()
		{
			string name = "ReportsWhetherScriptHasBeenCompiled.mod";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedFilePath(path);
			
			service.CreateModule(path,ModuleLocationType.File);
			service.OpenModule(path,ModuleLocationType.File);	
						
			// First check for a pre-compiled script, so we're not relying on CompileScript() being implemented.
			Assert.IsTrue(File.Exists(precompiled99bottlesScriptPath),"A file necessary for running the unit test was missing.");
									
			string precompiledScriptName = Path.GetFileNameWithoutExtension(precompiled99bottlesScriptPath);			
			Assert.IsFalse(service.HasCompiled(precompiledScriptName));
			Assert.IsFalse(service.HasUncompiled(precompiledScriptName));
			
			string copiedPath = Path.Combine(service.GetModuleTempPath(),precompiledScriptName+".NCS");
			Assert.IsFalse(File.Exists(copiedPath));
			File.Copy(precompiled99bottlesScriptPath,copiedPath);
			Assert.IsTrue(File.Exists(copiedPath));
						
			Assert.IsTrue(service.HasCompiled(precompiledScriptName));
			Assert.IsFalse(service.HasUncompiled(precompiledScriptName));
			
			// Then add a new uncompiled script and compile it.
			string scriptData = sampleScripts.Sing;
			string scriptName = "uncompiled";
			
			Assert.IsFalse(service.HasCompiled(scriptName));
			Assert.IsFalse(service.HasUncompiled(scriptName));
			
			service.AddScript(scriptName,scriptData);
			
			Assert.IsFalse(service.HasCompiled(scriptName));
			Assert.IsTrue(service.HasUncompiled(scriptName));
			
			service.CompileScript(scriptName);	
			
			Assert.IsTrue(WaitForCompiledScriptToAppear(scriptName),"The compiled script file was never found.");
			
			Assert.IsTrue(service.HasUncompiled(scriptName));
			
			service.CloseModule();
			Delete(path);
		}
		
		
		[Test]
		public void CompilesScriptForFileModule()
		{
			string name = "CompilesScriptForFileModule.mod";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedFilePath(path);
			
			service.CreateModule(path,ModuleLocationType.File);
			service.OpenModule(path,ModuleLocationType.File);			
			
			string scriptData = sampleScripts.Sing;
			string scriptName = "uncompiled";
			
			Assert.IsFalse(service.HasCompiled(scriptName));
			Assert.IsFalse(service.HasUncompiled(scriptName));
			Assert.AreEqual(0,service.GetScriptNames().Count);
			
			service.AddScript(scriptName,scriptData);
			
			Assert.IsFalse(service.HasCompiled(scriptName));
			Assert.IsTrue(service.HasUncompiled(scriptName));
			Assert.AreEqual(1,service.GetScriptNames().Count);
			
			service.CompileScript(scriptName);					
			Assert.IsTrue(WaitForCompiledScriptToAppear(scriptName),"The compiled script file was never found.");
			
			Assert.IsTrue(service.HasUncompiled(scriptName));
			Assert.AreEqual(1,service.GetScriptNames().Count);
			
			try {
				scriptName = "imaginary script";
				service.CompileScript(scriptName);
				Assert.Fail("Didn't raise a FaultException<ArgumentException> when asked to compile a non-existent script.");
			}
			catch (FaultException<ArgumentException>) {
				// expected result
			}
			catch (FaultException) {
				CreateService();
				Assert.Fail("Didn't raise a FaultException<ArgumentException> when asked to compile a non-existent script.");
			}
			
			service.CloseModule();
			Delete(path);
		}
		
		
		[Test]
		public void CompilesScriptForDirectoryModule()
		{
			string name = "CompilesScriptForDirectoryModule";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedDirectoryPath(path);
			
			service.CreateModule(path,ModuleLocationType.Directory);
			service.OpenModule(path,ModuleLocationType.Directory);			
			
			string scriptData = sampleScripts.Sing;
			string scriptName = "uncompiled";
			
			Assert.IsFalse(service.HasCompiled(scriptName));
			Assert.IsFalse(service.HasUncompiled(scriptName));
			Assert.AreEqual(0,service.GetScriptNames().Count);
			
			service.AddScript(scriptName,scriptData);
			
			Assert.IsFalse(service.HasCompiled(scriptName));
			Assert.IsTrue(service.HasUncompiled(scriptName));
			Assert.AreEqual(1,service.GetScriptNames().Count);
			
			service.CompileScript(scriptName);			
			Assert.IsTrue(WaitForCompiledScriptToAppear(scriptName),"The compiled script file was never found.");
			
			Assert.IsTrue(service.HasUncompiled(scriptName));
			Assert.AreEqual(1,service.GetScriptNames().Count);
			
			try {
				scriptName = "imaginary script";
				service.CompileScript(scriptName);
				Assert.Fail("Didn't raise a FaultException<ArgumentException> when asked to compile a non-existent script.");
			}
			catch (FaultException<ArgumentException>) {
				// expected result
			}
			catch (FaultException) {
				CreateService();
				Assert.Fail("Didn't raise a FaultException<ArgumentException> when asked to compile a non-existent script.");
			}
			
			service.CloseModule();
			Delete(path);
		}
		
		
		[Test]
		public void RefusesToCompileIllegalScript()
		{
			string name = "RefusesToCompileIllegalScript.mod";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedFilePath(path);
			
			service.CreateModule(path,ModuleLocationType.File);
			service.OpenModule(path,ModuleLocationType.File);			
			
			string scriptData = sampleScripts.BrokenScript;
			string scriptName = "illegalscript";
						
			try {
				service.AddScript(scriptName,scriptData);
				service.CompileScript(scriptName);
				Assert.Fail("Didn't raise a FaultException<InvalidDataException> when asked to compile an illegal script.");
			}
			catch (FaultException<InvalidDataException>) {
				// expected result
			}
			catch (FaultException) {
				CreateService();
				Assert.Fail("Didn't raise a FaultException<InvalidDataException> when asked to compile an illegal script.");
			}
			
			service.CloseModule();
			Delete(path);
		}
				
		
		[Test]
		public void AttachesCompiledScriptToObject()
		{
			string name = "AttachesCompiledScriptToObject.mod";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedFilePath(path);
			
			service.CreateModule(path,ModuleLocationType.File);
			service.OpenModule(path,ModuleLocationType.File);
					
			string area = "area";
			service.AddArea(area,true,AreaBase.SmallestAreaSize);						
			for (int i = 0; i < 20; i++) {
				service.AddObject(area,NWN2ObjectType.Creature,"c_cat","cat" + i);
			}	
			
			IList<Guid> ids = service.GetObjectIDsByTag(area,NWN2ObjectType.Creature,"cat9");
			Assert.AreEqual(1,ids.Count);
			Bean cat = service.GetObject(area,NWN2ObjectType.Creature,ids[0],false);
			
			Assert.IsTrue(cat.HasValue("OnSpawnIn"));
			string catSpawnScript = cat.GetValue("OnSpawnIn");
			Assert.IsNotNull(catSpawnScript);
			string defaultScript = "nw_c2_default9";
			Assert.AreEqual(defaultScript,catSpawnScript);
			
			Assert.IsTrue(cat.HasValue("ObjectID"));
			string idVal = cat.GetValue("ObjectID");
			Assert.IsNotNull(idVal);
			Assert.IsNotEmpty(idVal);
			Guid catID = new Guid(idVal);	
			
			string scriptData = sampleScripts.Sing;
			string scriptName = "attachingscript";
			
			service.AddScript(scriptName,scriptData);
			service.CompileScript(scriptName);
			Assert.IsTrue(WaitForCompiledScriptToAppear(scriptName),"The compiled script file was never found.");
			
			service.AttachScriptToObject(scriptName,area,NWN2ObjectType.Creature,catID,"OnSpawnIn");	
						
			cat = service.GetObject(area,NWN2ObjectType.Creature,catID,false);
			Assert.IsNotNull(cat);
			
			Assert.IsTrue(cat.HasValue("OnSpawnIn"));
			catSpawnScript = cat.GetValue("OnSpawnIn");
			Assert.IsNotNull(catSpawnScript);
			Assert.AreEqual(scriptName,catSpawnScript);
			
			service.CloseModule();			
			Delete(path);
		}
		
		
		[Test]
		public void AttachesCompiledScriptToArea()
		{
			string name = "AttachesCompiledScriptToArea.mod";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedFilePath(path);
			
			service.CreateModule(path,ModuleLocationType.File);
			service.OpenModule(path,ModuleLocationType.File);
					
			string areaName = "area";
			service.AddArea(areaName,true,AreaBase.SmallestAreaSize);
						
			string scriptName = "givegold";
			string scriptData = sampleScripts.GiveGold;
			service.AddScript(scriptName,scriptData);
			
			service.CompileScript(scriptName);
			Assert.IsTrue(WaitForCompiledScriptToAppear(scriptName),"The compiled script file was never found.");
			
			string scriptSlot = "OnHeartbeat";			
			service.AttachScriptToArea(scriptName,areaName,scriptSlot);
			
			Bean before = service.GetArea(areaName,false);
			Assert.IsTrue(before.HasValue(scriptSlot));
			
			Assert.AreEqual(scriptName,before.GetValue(scriptSlot));
			
			service.SaveModule();
			service.CloseModule();
			service.OpenModule(path,ModuleLocationType.File);
				
			Bean after = service.GetArea(areaName,false);
			Assert.IsTrue(after.HasValue(scriptSlot));
			Assert.AreEqual(scriptName,after.GetValue(scriptSlot));
		
			service.CloseModule();			
			Delete(path);
		}
		
		
		[Test]
		public void AttachesCompiledScriptToModule()
		{
			string name = "AttachesCompiledScriptToModule.mod";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedFilePath(path);
			
			service.CreateModule(path,ModuleLocationType.File);
			service.OpenModule(path,ModuleLocationType.File);
						
			string scriptName = "givegold";
			string scriptData = sampleScripts.GiveGold;
			service.AddScript(scriptName,scriptData);
			service.CompileScript(scriptName);
			Assert.IsTrue(WaitForCompiledScriptToAppear(scriptName),"The compiled script file was never found.");
			
			string scriptSlot = "OnPlayerLevelUp";			
			service.AttachScriptToModule(scriptName,scriptSlot);
			
			Bean moduleBean = service.GetModule();	
			
			Assert.IsTrue(moduleBean.HasValue(scriptSlot));
			Assert.AreEqual(scriptName,moduleBean.GetValue(scriptSlot));
			
			service.SaveModule();
			service.CloseModule();
			service.OpenModule(path,ModuleLocationType.File);
			
			moduleBean = service.GetModule();
			Assert.IsTrue(moduleBean.HasValue(scriptSlot));
			Assert.AreEqual(scriptName,moduleBean.GetValue(scriptSlot));
		
			service.CloseModule();			
			Delete(path);
		}
		
		
		[Test]
		public void RefusesToAttachScriptToNonExistentScriptSlot()
		{
			string name = "RefusesToAttachScriptToNonExistentScriptSlot.mod";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedFilePath(path);
			
			service.CreateModule(path,ModuleLocationType.File);
			service.OpenModule(path,ModuleLocationType.File);
					
			string area = "desert";
			service.AddArea(area,true,AreaBase.SmallestAreaSize);
			Guid catID = service.AddObject(area,NWN2ObjectType.Creature,"c_cat","cat");
			Bean cat = service.GetObject(area,NWN2ObjectType.Creature,catID,false);
			
			string scriptData = sampleScripts.Sing;
			string scriptName = "attachingscript";
			
			service.AddScript(scriptName,scriptData);
			
			service.CompileScript(scriptName);
			Assert.IsTrue(WaitForCompiledScriptToAppear(scriptName),"The compiled script file was never found.");
			
			string scriptSlot = "fake script slot";
			foreach (Nwn2Type t in Enum.GetValues(typeof(Nwn2Type))) {
				Assert.IsTrue(!(Nwn2ScriptSlot.GetScriptSlotNames(t).Contains(scriptSlot)));
			}
			
			try {
				service.AttachScriptToObject(scriptName,area,NWN2ObjectType.Creature,catID,scriptSlot);
				Assert.Fail("Didn't raise a FaultException<ArgumentException> when asked to attach " +
			            	"to a script slot that doesn't exist.");
			}
			catch (FaultException<ArgumentException>) {
				// expected result
			}
			catch (FaultException) {
				CreateService();
				Assert.Fail("Didn't raise a FaultException<ArgumentException> when asked to attach " +
			            	"to a script slot that doesn't exist.");
			}
			
			try {
				service.AttachScriptToArea(scriptName,area,scriptSlot);
				Assert.Fail("Didn't raise a FaultException<ArgumentException> when asked to attach " +
			            	"to a script slot that doesn't exist.");
			}
			catch (FaultException<ArgumentException>) {
				// expected result
			}
			catch (FaultException) {
				CreateService();
				Assert.Fail("Didn't raise a FaultException<ArgumentException> when asked to attach " +
			            	"to a script slot that doesn't exist.");
			}
			
			try {
				service.AttachScriptToModule(scriptName,scriptSlot);
				Assert.Fail("Didn't raise a FaultException<ArgumentException> when asked to attach " +
			            	"to a script slot that doesn't exist.");
			}
			catch (FaultException<ArgumentException>) {
				// expected result
			}
			catch (FaultException) {
				CreateService();
				Assert.Fail("Didn't raise a FaultException<ArgumentException> when asked to attach " +
			            	"to a script slot that doesn't exist.");
			}
			
			service.CloseModule();			
			Delete(path);
		}
		
		
		[Test]
		public void ModuleBeanCapturesScriptInformation()
		{
			string name = "ModuleBeanCapturesScriptInformation.mod";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedFilePath(path);
			
			service.CreateModule(path,ModuleLocationType.File);
			service.OpenModule(path,ModuleLocationType.File);
			
			Bean moduleBean = service.GetModule();
			foreach (string scriptSlot in Nwn2ScriptSlot.GetScriptSlotNames(Nwn2Type.Module)) {
				Assert.IsTrue(moduleBean.HasValue(scriptSlot));
			}
			Assert.AreEqual("nw_o0_death",moduleBean.GetValue("OnPlayerDeath"));
		
			service.CloseModule();			
			Delete(path);
		}
		
		
		[Test]
		public void RefusesToAttachScriptToNonExistentArea()
		{
			string name = "RefusesToAttachScriptToNonExistentArea.mod";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedFilePath(path);
			
			service.CreateModule(path,ModuleLocationType.File);
			service.OpenModule(path,ModuleLocationType.File);
					
			string areaName = "area";
			service.AddArea(areaName,true,AreaBase.SmallestAreaSize);	
						
			string scriptName = "givegold";
			string scriptData = sampleScripts.GiveGold;
			service.AddScript(scriptName,scriptData);
			service.CompileScript(scriptName);
			Assert.IsTrue(WaitForCompiledScriptToAppear(scriptName),"The compiled script file was never found.");
			
			string scriptSlot = "OnHeartbeat";	
			string wrongAreaName = "non existent area";
			Assert.IsNull(service.GetArea(wrongAreaName,false));
			
			try {
				service.AttachScriptToArea(wrongAreaName,areaName,scriptSlot);
				Assert.Fail("Didn't raise a FaultException<ArgumentException> when asked to attach " +
			            	"an area that doesn't exist.");
			}
			catch (FaultException<ArgumentException>) {
				// expected result
			}
			catch (FaultException) {
				CreateService();
				Assert.Fail("Didn't raise a FaultException<ArgumentException> when asked to attach " +
			            	"an area that doesn't exist.");
			}
		
			service.CloseModule();			
			Delete(path);
		}
		
		
		[Test]
		public void ReturnsDataAboutAreas()
		{
			string name = "ReturnsDataAboutAreas.mod";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedFilePath(path);
			
			service.CreateModule(path,ModuleLocationType.File);
			service.OpenModule(path,ModuleLocationType.File);
					
			string area1 = "outside";
			service.AddArea(area1,true,AreaBase.LargestAreaSize);		
			
			string area2 = "inside";
			service.AddArea(area2,false,AreaBase.SmallestAreaSize);	
			
			Bean area1Bean = service.GetArea(area1,false);
			Assert.IsNotNull(area1Bean);
			Assert.IsTrue(area1Bean.HasValue("HasTerrain"));
			Assert.AreEqual("True",area1Bean.GetValue("HasTerrain"));
			
			Bean area2Bean = service.GetArea(area2,false);
			Assert.IsNotNull(area2Bean);
			Assert.IsTrue(area2Bean.HasValue("HasTerrain"));
			Assert.AreEqual("False",area2Bean.GetValue("HasTerrain"));
			
			IList<string> areas = service.GetAreaNames();
			Assert.IsNotNull(areas);
			Assert.AreEqual(2,areas.Count);
			
			area1Bean = service.GetArea(area1,false);
			area2Bean = service.GetArea(area2,false);
			  	
			Assert.IsNotNull(area1Bean);
			Assert.IsTrue(area1Bean.HasValue("HasTerrain"));
			Assert.AreEqual("True",area1Bean.GetValue("HasTerrain"));
			Assert.IsTrue(area1Bean.HasValue("Size"));
			Assert.AreEqual("{Width=32, Height=32}",area1Bean.GetValue("Size"));
			
			Assert.IsNotNull(area2Bean);
			Assert.IsTrue(area2Bean.HasValue("HasTerrain"));
			Assert.AreEqual("False",area2Bean.GetValue("HasTerrain"));
			Assert.IsTrue(area2Bean.HasValue("Size"));
			Assert.AreEqual("{Width=1, Height=1}",area2Bean.GetValue("Size"));
			
			service.CloseModule();			
			Delete(path);
		}
		
		
		[Test]
		public void ReturnsDataAboutModule()
		{
			string name = "ReturnsDataAboutModule.mod";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedFilePath(path);
			
			service.CreateModule(path,ModuleLocationType.File);
			service.OpenModule(path,ModuleLocationType.File);
			
			Bean module = service.GetModule();
			
			Assert.IsNotNull(module);
			Assert.IsTrue(module.HasValue("LocationType"));
			Assert.AreEqual("File",module.GetValue("LocationType"));
			Assert.IsTrue(module.HasValue("Name"));
			Assert.AreEqual(Path.GetFileNameWithoutExtension(path),module.GetValue("Name"));
			
			service.CloseModule();
			
			name = "ReturnsDataAboutModule directory";
			path = Path.Combine(parent,name);
			path = pathChecker.GetUnusedDirectoryPath(path);
			
			service.CreateModule(path,ModuleLocationType.Directory);
			service.OpenModule(path,ModuleLocationType.Directory);
			
			module = service.GetModule();
			Assert.IsNotNull(module);
			Assert.IsTrue(module.HasValue("LocationType"));
			Assert.AreEqual("Directory",module.GetValue("LocationType"));
			Assert.IsTrue(module.HasValue("Name"));
			Assert.AreEqual(Path.GetFileNameWithoutExtension(path),module.GetValue("Name"));
			
			service.CloseModule();			
			Delete(path);
		}
				
		
		[Test]
		public void RefusesToAttachUncompiledOrMissingScriptToObject()
		{
			string name = "RefusesToAttachUncompiledOrMissingScriptToObject.mod";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedFilePath(path);
			
			service.CreateModule(path,ModuleLocationType.File);
			service.OpenModule(path,ModuleLocationType.File);
					
			string area = "area";
			service.AddArea(area,true,AreaBase.SmallestAreaSize);							
			Guid catID = service.AddObject(area,NWN2ObjectType.Creature,"c_cat","cat");
			Bean cat = service.GetObject(area,NWN2ObjectType.Creature,catID,false);
			
			string scriptData = sampleScripts.Sing;
			string scriptName = "uncompiled script";
			
			service.AddScript(scriptName,scriptData);
			
			Assert.IsTrue(service.HasUncompiled(scriptName));
			Assert.IsFalse(service.HasCompiled(scriptName));	
			
			try {
				service.AttachScriptToObject(scriptName,area,NWN2ObjectType.Creature,catID,"OnSpawnIn");
				Assert.Fail("Didn't raise a FaultException<InvalidDataException> when asked to attach " +
			            	"an uncompiled script to an object.");
			}
			catch (FaultException<InvalidDataException>) {
				// expected result				
			}
			catch (FaultException) {
				CreateService();
				Assert.Fail("Didn't raise a FaultException<InvalidDataException> when asked to attach " +
			            	"an uncompiled script to an object.");
			}
			
			scriptName = "script that does not exist";
			Assert.IsFalse(service.HasCompiled(scriptName));
			Assert.IsFalse(service.HasUncompiled(scriptName));
			try {
				service.AttachScriptToObject(scriptName,area,NWN2ObjectType.Creature,catID,"OnSpawnIn");
				Assert.Fail("Didn't raise a FaultException<ArgumentException> when asked to attach " +
			            	"a script that doesn't exist.");
			}
			catch (FaultException<ArgumentException>) {
				// expected result
			}
			catch (FaultException) {
				CreateService();
				Assert.Fail("Didn't raise a FaultException<ArgumentException> when asked to attach " +
			            	"a script that doesn't exist.");
			}
			
			service.CloseModule();			
			Delete(path);
		}		
				
		
		[Test]
		public void RefusesToAttachUncompiledOrMissingScriptToArea()
		{
			string name = "RefusesToAttachUncompiledOrMissingScriptToArea.mod";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedFilePath(path);
			
			service.CreateModule(path,ModuleLocationType.File);
			service.OpenModule(path,ModuleLocationType.File);
					
			string area = "area";
			service.AddArea(area,true,AreaBase.SmallestAreaSize);	
			
			string scriptData = sampleScripts.Sing;
			string scriptName = "uncompiled script";
			
			service.AddScript(scriptName,scriptData);
			
			Assert.IsTrue(service.HasUncompiled(scriptName));
			Assert.IsFalse(service.HasCompiled(scriptName));	
			
			try {
				service.AttachScriptToArea(scriptName,area,"OnHeartbeat");
				Assert.Fail("Didn't raise a FaultException<InvalidDataException> when asked to attach " +
			            	"an uncompiled script to an area.");
			}
			catch (FaultException<InvalidDataException>) {
				// expected result				
			}
			catch (FaultException) {
				CreateService();
				Assert.Fail("Didn't raise a FaultException<InvalidDataException> when asked to attach " +
			            	"an uncompiled script to an area.");
			}
			
			scriptName = "script that does not exist";
			Assert.IsFalse(service.HasCompiled(scriptName));
			Assert.IsFalse(service.HasUncompiled(scriptName));
			try {
				service.AttachScriptToArea(scriptName,area,"OnHeartbeat");
				Assert.Fail("Didn't raise a FaultException<ArgumentException> when asked to attach " +
			            	"a script that doesn't exist.");
			}
			catch (FaultException<ArgumentException>) {
				// expected result
			}
			catch (FaultException) {
				CreateService();
				Assert.Fail("Didn't raise a FaultException<ArgumentException> when asked to attach " +
			            	"a script that doesn't exist.");
			}
			
			service.CloseModule();			
			Delete(path);
		}	
		
		
		[Test]
		public void ScriptsPersistInFileModule()
		{
			string name = "ScriptsPersistInFileModule.mod";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedFilePath(path);
			
			service.CreateModule(path,ModuleLocationType.File);
			service.OpenModule(path,ModuleLocationType.File);
					
			string area = "area";
			service.AddArea(area,true,AreaBase.SmallestAreaSize);
			Guid catID = service.AddObject(area,NWN2ObjectType.Creature,"c_cat","cat");
			Bean cat = service.GetObject(area,NWN2ObjectType.Creature,catID,false);
						
			string scriptData = sampleScripts.Sing;
			string scriptName = "attachingscript";
			
			service.AddScript(scriptName,scriptData);
			service.CompileScript(scriptName);
			Assert.IsTrue(WaitForCompiledScriptToAppear(scriptName),"The compiled script file was never found.");
			service.AttachScriptToObject(scriptName,area,NWN2ObjectType.Creature,catID,"OnSpawnIn");	
			
			// Before...
			Assert.IsNotNull(service.GetScript(scriptName));
			Assert.AreEqual(1,service.GetScriptNames().Count);
			Assert.IsTrue(service.HasCompiled(scriptName));
			Assert.IsTrue(service.HasUncompiled(scriptName));
						
			cat = service.GetObject(area,NWN2ObjectType.Creature,catID,false);
			Assert.IsNotNull(cat);
			
			string catSpawnScript = cat.GetValue("OnSpawnIn");
			Assert.IsNotNull(catSpawnScript);
			Assert.AreEqual(scriptName,catSpawnScript);
			
			Assert.AreEqual(1,service.GetScriptNames().Count);
			Bean script = service.GetScript(scriptName);
			Assert.IsNotNull(script);
			Assert.AreEqual(sampleScripts.Sing,script["Data"]);
						
			service.SaveModule();
			service.CloseModule();
			service.OpenModule(path,ModuleLocationType.File);
			
			// And after...
			Assert.IsNotNull(service.GetScript(scriptName));
			Assert.AreEqual(1,service.GetScriptNames().Count);
			Assert.IsTrue(service.HasCompiled(scriptName));
			Assert.IsTrue(service.HasUncompiled(scriptName));
			
			service.OpenArea(area);
			cat = service.GetObject(area,NWN2ObjectType.Creature,catID,false);
			Assert.IsNotNull(cat);
			
			catSpawnScript = cat.GetValue("OnSpawnIn");
			Assert.IsNotNull(catSpawnScript);
			Assert.AreEqual(scriptName,catSpawnScript);
			
			Assert.AreEqual(1,service.GetScriptNames().Count);
			script = service.GetScript(scriptName);
			Assert.IsNotNull(script);
			Assert.AreEqual(sampleScripts.Sing,script["Data"]);
			
			service.CloseModule();			
			Delete(path);
		}
		
				
		[Test]
		public void ScriptsPersistInDirectoryModule()
		{
			string name = "ScriptsPersistInDirectoryModule";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedDirectoryPath(path);
			
			service.CreateModule(path,ModuleLocationType.Directory);
			service.OpenModule(path,ModuleLocationType.Directory);
					
			string area = "area";
			service.AddArea(area,true,AreaBase.SmallestAreaSize);
			Guid catID = service.AddObject(area,NWN2ObjectType.Creature,"c_cat","cat");
			Bean cat = service.GetObject(area,NWN2ObjectType.Creature,catID,false);
						
			string scriptData = sampleScripts.Sing;
			string scriptName = "attachingscript";
			
			service.AddScript(scriptName,scriptData);
			service.CompileScript(scriptName);		
			Assert.IsTrue(WaitForCompiledScriptToAppear(scriptName),"The compiled script file was never found.");	
			service.AttachScriptToObject(scriptName,area,NWN2ObjectType.Creature,catID,"OnSpawnIn");	
			
			// Before...
			Assert.IsNotNull(service.GetScript(scriptName));
			Assert.AreEqual(1,service.GetScriptNames().Count);
			Assert.IsTrue(service.HasCompiled(scriptName));
			Assert.IsTrue(service.HasUncompiled(scriptName));
						
			cat = service.GetObject(area,NWN2ObjectType.Creature,catID,false);
			Assert.IsNotNull(cat);
			
			string catSpawnScript = cat.GetValue("OnSpawnIn");
			Assert.IsNotNull(catSpawnScript);
			Assert.AreEqual(scriptName,catSpawnScript);
			
			Assert.AreEqual(1,service.GetScriptNames().Count);
			Bean script = service.GetScript(scriptName);
			Assert.IsNotNull(script);
			Assert.AreEqual(sampleScripts.Sing,script["Data"]);
						
			service.SaveModule();
			service.CloseModule();
			service.OpenModule(path,ModuleLocationType.Directory);
			
			// And after...
			Assert.IsNotNull(service.GetScript(scriptName));
			Assert.AreEqual(1,service.GetScriptNames().Count);
			Assert.IsTrue(service.HasCompiled(scriptName));
			Assert.IsTrue(service.HasUncompiled(scriptName));
						
			service.OpenArea(area);
			cat = service.GetObject(area,NWN2ObjectType.Creature,catID,false);
			Assert.IsNotNull(cat);
			
			catSpawnScript = cat.GetValue("OnSpawnIn");
			Assert.IsNotNull(catSpawnScript);
			Assert.AreEqual(scriptName,catSpawnScript);
			
			Assert.AreEqual(1,service.GetScriptNames().Count);
			script = service.GetScript(scriptName);
			Assert.IsNotNull(script);
			Assert.AreEqual(sampleScripts.Sing,script["Data"]);
			
			service.CloseModule();			
			Delete(path);
		}
		
		
		[Test]
		public void UnsavedScriptDoesNotPersistInFileModule()
		{
			string name = "UncompiledScriptDoesNotPersistInFileModuleWithoutSave.mod";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedFilePath(path);
			
			service.CreateModule(path,ModuleLocationType.File);
			service.OpenModule(path,ModuleLocationType.File);
						
			string scriptData = sampleScripts.Sing;
			string scriptName = "attachingscript";
			
			service.AddScript(scriptName,scriptData);
			
			// Before...
			Bean before = service.GetScript(scriptName);
			Assert.IsNotNull(before);
			Assert.AreEqual(sampleScripts.Sing,before["Data"]);
			Assert.AreEqual(1,service.GetScriptNames().Count);
						
			service.CloseModule();
			service.OpenModule(path,ModuleLocationType.File);
			
			// And after...
			Bean after = service.GetScript(scriptName);
			Assert.IsNull(after);
			
			service.CloseModule();			
			Delete(path);
		}
		
		
		[Test]
		public void SavedScriptPersistsInFileModule()
		{
			string name = "UncompiledScriptPersistsInFileModuleWithoutSave.mod";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedFilePath(path);
			
			service.CreateModule(path,ModuleLocationType.File);
			service.OpenModule(path,ModuleLocationType.File);
						
			string scriptData = sampleScripts.Sing;
			string scriptName = "attachingscript";
			
			service.AddScript(scriptName,scriptData);
			
			// Before...
			Bean before = service.GetScript(scriptName);
			Assert.IsNotNull(before);
			Assert.AreEqual(sampleScripts.Sing,before["Data"]);
			Assert.AreEqual(1,service.GetScriptNames().Count);
						
			service.SaveModule();
			service.CloseModule();
			service.OpenModule(path,ModuleLocationType.File);
			
			// And after...
			Bean after = service.GetScript(scriptName);
			Assert.IsNotNull(after);
			Assert.AreEqual(sampleScripts.Sing,after["Data"]);
			Assert.AreEqual(1,service.GetScriptNames().Count);
			
			service.CloseModule();			
			Delete(path);
		}
		
		
		[Test]
		public void UnsavedScriptPersistsInDirectoryModule()
		{
			string name = "UncompiledScriptDoesNotPersistInDirectoryModuleWithoutSave";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedDirectoryPath(path);
			
			service.CreateModule(path,ModuleLocationType.Directory);
			service.OpenModule(path,ModuleLocationType.Directory);
						
			string scriptData = sampleScripts.Sing;
			string scriptName = "attachingscript";
			
			service.AddScript(scriptName,scriptData);
			
			// Before...
			Bean before = service.GetScript(scriptName);
			Assert.IsNotNull(before);
			Assert.AreEqual(sampleScripts.Sing,before["Data"]);
			Assert.AreEqual(1,service.GetScriptNames().Count);
						
			service.CloseModule();
			service.OpenModule(path,ModuleLocationType.Directory);
			
			// And after...
			Bean after = service.GetScript(scriptName);
			Assert.IsNotNull(after);
			Assert.AreEqual(sampleScripts.Sing,after["Data"]);
			Assert.AreEqual(1,service.GetScriptNames().Count);
			
			service.CloseModule();			
			Delete(path);
		}
		
		
		[Test]
		public void SavedScriptPersistsInDirectoryModule()
		{
			string name = "UncompiledScriptPersistsInDirectoryModuleWithSave";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedDirectoryPath(path);
			
			service.CreateModule(path,ModuleLocationType.Directory);
			service.OpenModule(path,ModuleLocationType.Directory);
						
			string scriptData = sampleScripts.Sing;
			string scriptName = "attachingscript";
			
			service.AddScript(scriptName,scriptData);
			
			// Before...
			Bean before = service.GetScript(scriptName);
			Assert.IsNotNull(before);
			Assert.AreEqual(sampleScripts.Sing,before["Data"]);
			Assert.AreEqual(1,service.GetScriptNames().Count);
			
			service.SaveModule();
			service.CloseModule();
			service.OpenModule(path,ModuleLocationType.Directory);
			
			// And after...
			Bean after = service.GetScript(scriptName);
			Assert.IsNotNull(after);
			Assert.AreEqual(sampleScripts.Sing,after["Data"]);
			Assert.AreEqual(1,service.GetScriptNames().Count);
			
			service.CloseModule();			
			Delete(path);
		}
		
		
		[Test]
		public void AllScriptChangesAreDiscardedInFileModule()
		{
			string name = "AllScriptChangesAreDiscardedInFileModule.mod";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedFilePath(path);
			
			service.CreateModule(path,ModuleLocationType.File);
			service.OpenModule(path,ModuleLocationType.File);
			
			string areaName = "area";
			service.AddArea(areaName,false,AreaFacade.SmallestAreaSize);
			service.SaveModule();
			
			string scriptName = "myscript";
			string scriptData = sampleScripts.Sing;		
			string scriptSlot = "OnEnterScript";
			service.AddScript(scriptName,scriptData);
			service.CompileScript(scriptName);
			Assert.IsTrue(WaitForCompiledScriptToAppear(scriptName),"The compiled script file was never found.");
			service.AttachScriptToArea(scriptName,areaName,scriptSlot);
			
			service.CloseModule();
			service.OpenModule(path,ModuleLocationType.File);
			
			// When discarding changes to a file module, newly added scripts do not persist...
			Assert.IsNull(service.GetScript(scriptName));
			
			// ...changes to existing scripts do not persist (not tested here)...
			
			// ...and changes to script slots do not persist.
			Bean area = service.GetArea(areaName,false);
			Assert.IsEmpty(area[scriptSlot]);
			
			service.CloseModule();			
			Delete(path);
		}
		
		
		[Test]
		public void SomeScriptChangesAreDiscardedInDirectoryModule()
		{
			string name = "SomeScriptChangesAreDiscardedInDirectoryModule";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedDirectoryPath(path);
			
			service.CreateModule(path,ModuleLocationType.Directory);
			service.OpenModule(path,ModuleLocationType.Directory);
			
			string areaName = "area";
			service.AddArea(areaName,false,AreaFacade.SmallestAreaSize);
			service.SaveModule();
			
			string scriptName = "myscript";
			string scriptData = sampleScripts.Sing;		
			string scriptSlot = "OnEnterScript";
			service.AddScript(scriptName,scriptData);
			service.CompileScript(scriptName);
			Assert.IsTrue(WaitForCompiledScriptToAppear(scriptName),"The compiled script file was never found.");
			service.AttachScriptToArea(scriptName,areaName,scriptSlot);
			
			service.CloseModule();
			service.OpenModule(path,ModuleLocationType.Directory);
			
			// When discarding changes to a directory module, newly added scripts persist...
			Bean script = service.GetScript(scriptName);
			Assert.IsNotNull(script);
			Assert.AreEqual(script["Data"],scriptData);
			
			// ...changes to existing scripts persist (not tested here)...
			
			// ...but changes to script slots do not persist.
			service.OpenArea(areaName);
			Bean area = service.GetArea(areaName,false);
			Assert.IsEmpty(area[scriptSlot]);
						
			service.CloseModule();			
			Delete(path);
		}
		
		
		[Test]
		public void GetsScriptNames()
		{
			string name = "GetsCompiledScriptNames";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedDirectoryPath(path);
			
			service.CreateModule(path,ModuleLocationType.Directory);
			service.OpenModule(path,ModuleLocationType.Directory);
			
			service.AddScript("givegold",sampleScripts.GiveGold);
			service.AddScript("sing",sampleScripts.Sing);
			
			IList<string> scriptNames = service.GetScriptNames();
			Assert.IsTrue(scriptNames.Contains("givegold"));
			Assert.IsTrue(scriptNames.Contains("sing"));
			Assert.AreEqual(2,scriptNames.Count);
			
			IList<string> compiledScriptNames = service.GetCompiledScriptNames();			
			Assert.AreEqual(0,compiledScriptNames.Count);
			
			service.CompileScript("givegold");
			Assert.IsTrue(WaitForCompiledScriptToAppear("givegold"),"The compiled script file was never found.");
			
			compiledScriptNames = service.GetCompiledScriptNames();
			Assert.AreEqual("givegold",compiledScriptNames[0]);
			Assert.AreEqual(1,compiledScriptNames.Count);			
						
			service.CloseModule();			
			Delete(path);
		}
		
		
		[Test]
		public void ReturnsDataAboutScripts()
		{
			string name = "ReturnsDataAboutScripts";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedDirectoryPath(path);
			
			service.CreateModule(path,ModuleLocationType.Directory);
			service.OpenModule(path,ModuleLocationType.Directory);
										
			string givegold = "givegold";
			string givegoldData = sampleScripts.GiveGold;
			string changename = "changename";
			string changenameData = sampleScripts.ChangeName;
			string _99bottles = Path.GetFileNameWithoutExtension(precompiled99bottlesScriptPath);
			
			// End up with 2 uncompiled scripts (givegold, changename) and 2 compiled scripts (givegold, 99bottles):
			service.AddScript(givegold,givegoldData);
			service.CompileScript(givegold);
			Assert.IsTrue(WaitForCompiledScriptToAppear(givegold),"The compiled script file was never found.");
			service.AddScript(changename,changenameData);
			Assert.IsTrue(File.Exists(precompiled99bottlesScriptPath),"A file necessary for running the unit test was missing.");
			
			// Place pre-compiled script directly into module:
			File.Copy(precompiled99bottlesScriptPath,Path.Combine(service.GetModuleTempPath(),Path.GetFileName(precompiled99bottlesScriptPath)));
			
			Bean givegoldBean, changenameBean, _99bottlesBean;
			
			// GetScript
			Assert.IsNotNull(givegoldBean = service.GetScript(givegold));
			Assert.IsNotNull(changenameBean = service.GetScript(changename));
			Assert.IsNull(_99bottlesBean = service.GetScript(_99bottles));
			Assert.AreEqual(givegoldData,givegoldBean.GetValue("Data"));
			Assert.AreEqual(changenameData,changenameBean.GetValue("Data"));
			
			// GetCompiledScript
			givegoldBean = null;
			changenameBean = null;
			_99bottlesBean = null;
			Assert.IsNotNull(givegoldBean = service.GetCompiledScript(givegold));
			Assert.IsNull(changenameBean = service.GetCompiledScript(changename));
			Assert.IsNotNull(_99bottlesBean = service.GetCompiledScript(_99bottles));
						
			service.CloseModule();			
			Delete(path);
		}
		
		
		[Test]
		public void ReturnsDataAboutScriptsForTemporaryModules()
		{
			service.CreateAndOpenTemporaryModule();
										
			string givegold = "givegold";
			string givegoldData = sampleScripts.GiveGold;
			string changename = "changename";
			string changenameData = sampleScripts.ChangeName;
			string _99bottles = Path.GetFileNameWithoutExtension(precompiled99bottlesScriptPath);
			
			// End up with 2 uncompiled scripts (givegold, changename) and 2 compiled scripts (givegold, 99bottles):
			service.AddScript(givegold,givegoldData);
			service.CompileScript(givegold);
			Assert.IsTrue(WaitForCompiledScriptToAppear(givegold),"The compiled script file was never found.");
			service.AddScript(changename,changenameData);
			Assert.IsTrue(File.Exists(precompiled99bottlesScriptPath),"A file necessary for running the unit test was missing.");
			
			// Place pre-compiled script directly into module:
			File.Copy(precompiled99bottlesScriptPath,Path.Combine(service.GetModuleTempPath(),Path.GetFileName(precompiled99bottlesScriptPath)));
			
			Bean givegoldBean, changenameBean, _99bottlesBean;
			
			// GetScript
			Assert.IsNotNull(givegoldBean = service.GetScript(givegold));
			Assert.IsNotNull(changenameBean = service.GetScript(changename));
			Assert.IsNull(_99bottlesBean = service.GetScript(_99bottles));
			Assert.AreEqual(givegoldData,givegoldBean.GetValue("Data"));
			Assert.AreEqual(changenameData,changenameBean.GetValue("Data"));
			
			// GetCompiledScript
			givegoldBean = null;
			changenameBean = null;
			_99bottlesBean = null;
			Assert.IsNotNull(givegoldBean = service.GetCompiledScript(givegold));
			Assert.IsNull(changenameBean = service.GetCompiledScript(changename));
			Assert.IsNotNull(_99bottlesBean = service.GetCompiledScript(_99bottles));
						
			service.CloseModule();	
		}
		
		#endregion
		
		#region Tests - I/O
				
		[Test]
		public void GetsCurrentArea()
		{
			string name = "GetsCurrentArea.mod";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedFilePath(path);
			name = Path.GetFileNameWithoutExtension(path);
		
			service.CreateModule(path,ModuleLocationType.File);
					
			Assert.AreEqual(null,service.GetCurrentArea());
			
			service.OpenModule(path,ModuleLocationType.File);	
			
			string area1 = "forest";
			string area2 = "castle";
			
			service.AddArea(area1,true,AreaFacade.SmallestAreaSize);			
			Assert.AreEqual(area1,service.GetCurrentArea());
			
			service.AddArea(area2,false,AreaFacade.SmallestAreaSize);
			Assert.AreEqual(area2,service.GetCurrentArea());
			
			service.CloseArea(area2);
			Assert.AreEqual(area1,service.GetCurrentArea());
			
			service.CloseArea(area1);
			Assert.AreEqual(null,service.GetCurrentArea());
			
			service.CloseModule();			
			Delete(path);
		}
		
		
		[Test]
		public void CreatesDirectoryModule()
		{			
			string name = "CreatesDirectoryModule";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedDirectoryPath(path);
					
			service.CreateModule(path,ModuleLocationType.Directory);
			
			DirectoryInfo dir = new DirectoryInfo(path);
			
			Assert.IsTrue(dir.Exists,"Module directory was not created.");
						
			foreach (string expectedFile in new string[] { "MODULE.IFO", "module.jrl", "repute.fac" }) {
				Assert.IsTrue(File.Exists(Path.Combine(path,expectedFile)),expectedFile + " was missing from module directory.");
			}
			
			Delete(path);
		}
		
		
		[Test]  
		public void CreatesFileModule()
		{			
			string name = "CreatesFileModule.mod";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedFilePath(path);
			
			service.CreateModule(path,ModuleLocationType.File);
			
			Assert.IsTrue(File.Exists(path),"Module file was not created.");
			
			Delete(path);
		}
		
		
		[Test]
		public void CreatesAndOpensTemporaryModule()
		{
			string path = service.CreateAndOpenTemporaryModule();
			
			Assert.IsTrue(Directory.Exists(path),"Module was not created.");			
			service.CloseModule();
			Assert.IsFalse(Directory.Exists(path),"Module was not automatically deleted.");
		}
		
		
		[Test]
		public void AddsAreaToTemporaryModule()
		{
			service.CreateAndOpenTemporaryModule();
			
			Size size = new Size(AreaFacade.MinimumAreaLength,AreaFacade.MinimumAreaLength);
			
			string area1 = "alaska";
			string area2 = "hawaii";
			
			service.AddArea(area1,true,size);
			service.AddArea(area2,false,size);
									
			IList<string> areas = service.GetAreaNames();
			Assert.IsNotNull(areas);
			Assert.AreEqual(2,areas.Count);
			foreach (string a in areas) {
				Assert.IsNotNull(service.GetArea(a,false));
			}
			
			service.CloseModule();	
		}
		
		
		[Test]
		public void AddsUncompiledScriptToTemporaryModule()
		{
			service.CreateAndOpenTemporaryModule();
			
			IList<string> scripts = service.GetScriptNames();
			Assert.AreEqual(0,scripts.Count);
			
			// This script compiles in the toolset as entered here (hand-tested):
			string scriptData = sampleScripts.Sing;
			string scriptName = "uncompiled";
			
			service.AddScript(scriptName,scriptData);
			
			scripts = service.GetScriptNames();
			Assert.AreEqual(1,scripts.Count);
			Bean script = service.GetScript(scriptName);
			Assert.IsNotNull(script);
			
			Assert.AreEqual(scriptName,script.GetValue("Name"));
			Assert.AreEqual(scriptName + ".nss",script.GetValue("VersionControlName").ToLower());
			Assert.AreEqual(scriptData,script.GetValue("Data"));
					
			try {
				service.AddScript(scriptName,scriptData);
				Assert.Fail("Didn't raise a FaultException<ArgumentException> when asked to add " +
			            	"a script with an existing name.");
			}
			catch (FaultException<ArgumentException>) {
				// expected result
			}
			catch (FaultException) {
				CreateService();
				Assert.Fail("Didn't raise a FaultException<ArgumentException> when asked to add " +
			            	"a script with an existing name.");
			}
									
			service.CloseModule();
		}
		
		
		[Test]
		public void CompilesScriptForTemporaryModule()
		{
			service.CreateAndOpenTemporaryModule();
			
			string scriptData = sampleScripts.Sing;
			string scriptName = "uncompiled";
			
			Assert.IsFalse(service.HasCompiled(scriptName));
			Assert.IsFalse(service.HasUncompiled(scriptName));
			Assert.AreEqual(0,service.GetScriptNames().Count);
			
			service.AddScript(scriptName,scriptData);
			
			Assert.IsFalse(service.HasCompiled(scriptName));
			Assert.IsTrue(service.HasUncompiled(scriptName));
			Assert.AreEqual(1,service.GetScriptNames().Count);
			
			service.CompileScript(scriptName);					
			Assert.IsTrue(WaitForCompiledScriptToAppear(scriptName),"The compiled script file was never found.");
			
			Assert.IsTrue(service.HasUncompiled(scriptName));
			Assert.AreEqual(1,service.GetScriptNames().Count);
			
			try {
				scriptName = "imaginary script";
				service.CompileScript(scriptName);
				Assert.Fail("Didn't raise a FaultException<ArgumentException> when asked to compile a non-existent script.");
			}
			catch (FaultException<ArgumentException>) {
				// expected result
			}
			catch (FaultException) {
				CreateService();
				Assert.Fail("Didn't raise a FaultException<ArgumentException> when asked to compile a non-existent script.");
			}
			
			service.CloseModule();
		}
		
		
		[Test]
		public void ReturnsModuleNameForTemporaryModule()
		{
			string path = service.CreateAndOpenTemporaryModule();
			string name = service.GetModuleName();
			
			Assert.AreEqual(Path.GetFileName(path),name);
			
			service.CloseModule();
		}
		
		
		[Test]
		public void DoesNotCreateDirectoryModuleAtFilePath()
		{
			string name = "DoesNotCreateDirectoryModuleAtFilePath.mod";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedFilePath(path);
					
			try {
				service.CreateModule(path,ModuleLocationType.Directory);
				Assert.Fail("Didn't raise a FaultException<ArgumentException> when asked to create a directory " +
			            	"module at a path only suitable for file modules.");
			}
			catch (FaultException<ArgumentException>) {
				// expected result
				Assert.IsFalse(Directory.Exists(path),"Created a directory module given a file path.");
			}
			catch (FaultException) {
				CreateService();
				Assert.Fail("Didn't raise a FaultException<ArgumentException> when asked to create a directory " +
			            	"module at a path only suitable for file modules.");
			}
		}
				
		
		[Test]
		public void DoesNotCreateFileModuleAtDirectoryPath()
		{
			string name = "DoesNotCreateFileModuleAtDirectoryPath";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedDirectoryPath(path);
					
			try {
				service.CreateModule(path,ModuleLocationType.File);
				Assert.Fail("Didn't raise a FaultException<ArgumentException> when asked to create a file " +
			            	"module at a path only suitable for directory modules.");
			}
			catch (FaultException<ArgumentException>) {
				// expected result
				Assert.IsFalse(File.Exists(path),"Created a file module given a directory path.");
			}
			catch (FaultException) {
				CreateService();
				Assert.Fail("Didn't raise a FaultException<ArgumentException> when asked to create a file " +
			            	"module at a path only suitable for directory modules.");
			}
		}
		
		
		[Test]
		public void DoesNotCreateFileModuleIfPathIsAlreadyTaken()
		{
			string name = "DoesNotCreateFileModuleIfPathIsAlreadyTaken.mod";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedFilePath(path);
			
			service.CreateModule(path,ModuleLocationType.File);
			
			try {
				service.CreateModule(path,ModuleLocationType.File);
				Assert.Fail("Didn't raise a FaultException<IOException> when asked to create a file module at an occupied path.");
			}
			catch (FaultException<IOException>) {
				// expected result
			}
			catch (FaultException) {
				CreateService();
				Assert.Fail("Didn't raise a FaultException<IOException> when asked to create a file module at an occupied path.");
			}
			
			Delete(path);
		}
		
		
		[Test]
		public void DoesNotCreateDirectoryModuleIfPathIsAlreadyTaken()
		{
			string name = "DoesNotCreateDirectoryModuleIfPathIsAlreadyTaken";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedDirectoryPath(path);
			
			service.CreateModule(path,ModuleLocationType.Directory);
			
			try {
				service.CreateModule(path,ModuleLocationType.Directory);
				Assert.Fail("Didn't raise a FaultException<IOException> when asked to create a directory module at an occupied path.");
			}
			catch (FaultException<IOException>) {
				// expected result
			}
			catch (FaultException) {
				CreateService();
				Assert.Fail("Didn't raise a FaultException<IOException> when asked to create a directory module at an occupied path.");
			}
			
			Delete(path);
		}
		
		
		[Test]
		public void OpensDirectoryModule()
		{
			string name = "OpensDirectoryModule";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedDirectoryPath(path);
			name = Path.GetFileNameWithoutExtension(path);
					
			service.CreateModule(path,ModuleLocationType.Directory);
									
			service.OpenModule(path,ModuleLocationType.Directory);	
			
			Assert.AreEqual(name,service.GetModuleName());	
			
			service.CloseModule();			
			Delete(path);
		}
		
		
		[Test]
		public void OpensFileModule()
		{
			string name = "OpensFileModule.mod";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedFilePath(path);
			name = Path.GetFileNameWithoutExtension(path);
		
			service.CreateModule(path,ModuleLocationType.File);
			
			service.OpenModule(path,ModuleLocationType.File);	
			
			Assert.AreEqual(name,service.GetModuleName());
			
			service.CloseModule();			
			Delete(path);
		}
		
		
		[Test]
		public void OpensFileModuleFromOutsideModulesDirectory()
		{
			string name = "OpensFileModuleFromOutsideModulesDirectory.mod";
			string parent = @"N:\WindowsProfile\Desktop";
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedFilePath(path);
			name = Path.GetFileNameWithoutExtension(path);
		
			service.CreateModule(path,ModuleLocationType.File);
			
			service.OpenModule(path,ModuleLocationType.File);	
			
			Assert.AreEqual(name,service.GetModuleName());
			
			service.CloseModule();			
			Delete(path);
		}
		
		
		[Test]
		public void CloseModule()
		{
			string name = "CloseModule.mod";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedFilePath(path);
			name = Path.GetFileNameWithoutExtension(path);
					
			service.CreateModule(path,ModuleLocationType.File);
			
			service.OpenModule(path,ModuleLocationType.File);
			
			string moduleName, modulePath;
			moduleName = service.GetModuleName();
			modulePath = service.GetModulePath();		
			Assert.IsNotNull(moduleName);
			Assert.IsNotNull(modulePath);
			Assert.AreEqual(name,moduleName);
			Assert.AreEqual(path,modulePath);
			
			service.CloseModule();
			
			moduleName = service.GetModuleName();
			modulePath = service.GetModulePath();				
			if (moduleName != null) {
				Assert.AreNotEqual(name,moduleName);
			}
			if (modulePath != null) {
				Assert.AreNotEqual(path,modulePath);
			}
			
			Delete(path);
		}
		
		
		[Test]
		public void ReturnsModulePath()
		{
			string name = "ReturnsCorrectModulePath.mod";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedFilePath(path);
			name = Path.GetFileNameWithoutExtension(path);
			
			service.GetModuleName();
					
			service.CreateModule(path,ModuleLocationType.File);
			service.OpenModule(path, ModuleLocationType.File);
			
			Assert.AreEqual(path,service.GetModulePath());
							
			service.CloseModule();			
			Delete(path);
			
			name = "dir returns correct path";
			path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedDirectoryPath(path);
			name = Path.GetFileName(path);
					
			service.CreateModule(path,ModuleLocationType.Directory);
			service.OpenModule(path, ModuleLocationType.Directory);
			
			Assert.AreEqual(path,service.GetModulePath());
			
			service.CloseModule();			
			Delete(path);
					
			service.CreateAndOpenTemporaryModule();
			
			string returnedPath = service.GetModulePath();
			
			Assert.IsTrue(Directory.Exists(returnedPath));
			Assert.IsTrue(returnedPath.Contains(Path.Combine(NWN2ToolsetMainForm.ModulesDirectory,"temp")));
			
			service.CloseModule();
		}
		
				
		[Test]
		public void AddsAreaToFileModule()
		{
			string name = "AddsAreaToFileModule.mod";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedFilePath(path);
			
			service.CreateModule(path,ModuleLocationType.File);
			
			service.OpenModule(path,ModuleLocationType.File);
			
			Size size = new Size(AreaFacade.MinimumAreaLength,AreaFacade.MinimumAreaLength);
			
			string area1 = "land of wind and ghosts";
			string area2 = "land of milk and honey";
			
			service.AddArea(area1,true,size);
			service.AddArea(area2,false,size);
			service.SaveModule(); // save after adding an area to a file module if the area must persist
			
			service.CloseModule();			
			service.OpenModule(path,ModuleLocationType.File);
									
			IList<string> areas = service.GetAreaNames();
			Assert.IsNotNull(areas);
			Assert.AreEqual(2,areas.Count);
			foreach (string a in areas) {
				Assert.IsNotNull(service.GetArea(a,false));
			}
			
			service.CloseModule();			
			Delete(path);
		}
				
		
		[Test]
		public void AddsAreaToDirectoryModule()
		{
			string name = "AddsAreaToDirectoryModule";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedDirectoryPath(path);
			
			service.CreateModule(path,ModuleLocationType.Directory);
			service.OpenModule(path,ModuleLocationType.Directory);
			
			Size size = new Size(AreaFacade.MinimumAreaLength,AreaFacade.MinimumAreaLength);
			
			string area1 = "alaska";
			string area2 = "hawaii";
			
			service.AddArea(area1,true,size);
			service.AddArea(area2,false,size);
			
			service.CloseModule();
			service.OpenModule(path,ModuleLocationType.Directory);
									
			IList<string> areas = service.GetAreaNames();
			Assert.IsNotNull(areas);
			Assert.AreEqual(2,areas.Count);
			foreach (string a in areas) {
				Assert.IsNotNull(service.GetArea(a,false));
			}
			
			service.CloseModule();			
			Delete(path);
		}
		
		
		[Test]
		public void DoesNotAddAreaIfNameIsAlreadyTaken()
		{
			string name = "DoesNotAddAreaIfNameIsAlreadyTaken.mod";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedFilePath(path);
			
			service.CreateModule(path,ModuleLocationType.File);
			service.OpenModule(path,ModuleLocationType.File);
			
			string areaName = "duplicate area";
			Size size = new Size(AreaFacade.MinimumAreaLength,AreaFacade.MinimumAreaLength);
			
			service.AddArea(areaName,true,size);
						
			try {
				service.AddArea(areaName,false,size);
				Assert.Fail("Didn't raise a FaultException<IOException> when attempting " +
				            "to add an area with a duplicate name.");
			}
			catch (FaultException<IOException>) {
				// expected result
			}
			catch (FaultException) {
				CreateService();
				Assert.Fail("Didn't raise a FaultException<IOException> when attempting " +
				            "to add an area with a duplicate name.");
			}
			
			service.CloseModule();			
			Delete(path);
		}		
		
		
		[Test]
		public void AddsObjectToArea()
		{
			string name = "AddsObjectToArea.mod";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedFilePath(path);
			
			service.CreateModule(path,ModuleLocationType.File);
			service.OpenModule(path,ModuleLocationType.File);
			
			Size size = new Size(8,8);			
			string area = "area";
			service.AddArea(area,true,size);
			
			NWN2ObjectType[] types = new NWN2ObjectType[] { NWN2ObjectType.Creature,
															NWN2ObjectType.Creature,
															NWN2ObjectType.Creature,
															NWN2ObjectType.Placeable,
															NWN2ObjectType.PlacedEffect };
			string[] resRefs = new string[] { "c_giantfire", "n_greycloak", "n_greycloak", "plc_bs_sunkenf", "n2_fx_bonfire" };
			string[] tags = new string[] { "giant", "duplicatecloak", "duplicatecloak", "inn", "bonfire" };
			
			for (int i = 0; i < types.Length; i++) {
				service.AddObject(area,types[i],resRefs[i],tags[i]);
			}
			
			Assert.AreEqual(1,service.GetObjectIDsByTag(area,NWN2ObjectType.Creature,"giant").Count);
			Assert.AreEqual(2,service.GetObjectIDsByTag(area,NWN2ObjectType.Creature,"duplicatecloak").Count);
			Assert.AreEqual(1,service.GetObjectIDsByTag(area,NWN2ObjectType.Placeable,"inn").Count);
			Assert.AreEqual(1,service.GetObjectIDsByTag(area,NWN2ObjectType.PlacedEffect,"bonfire").Count);
			Assert.AreEqual(3,service.GetObjectIDs(area,NWN2ObjectType.Creature).Count);
			
			Assert.AreEqual(0,service.GetObjectIDsByTag(area,NWN2ObjectType.Creature,"bonfire").Count);
			Assert.AreEqual(0,service.GetObjectIDsByTag(area,NWN2ObjectType.Placeable,"giant").Count);
			Assert.AreEqual(0,service.GetObjectIDsByTag(area,NWN2ObjectType.PlacedEffect,"inn").Count);
			
			service.CloseModule();			
			Delete(path);
		}
		
		
		[Test]
		public void DoesNotAddObjectWithUnknownResref()
		{
			string name = "DoesNotAddObjectWithUnknownResref.mod";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedFilePath(path);
			
			service.CreateModule(path,ModuleLocationType.File);
			service.OpenModule(path,ModuleLocationType.File);
			
			Size size = new Size(8,8);			
			string area = "area";
			service.AddArea(area,true,size);
			
			try {
				service.AddObject(area,NWN2ObjectType.Creature,"I am not a real resref","tag");
				Assert.Fail("Didn't raise a FaultException<ArgumentException> when asked to add an object with unknown resref.");
			}
			catch (FaultException<ArgumentException>) {
				// expected result
			}
			catch (FaultException) {
				CreateService();
				Assert.Fail("Didn't raise a FaultException<ArgumentException> when asked to add an object with unknown resref.");
			}
			
			service.CloseModule();			
			Delete(path);
		}
				
				
		[Test]
		public void SavesDirectoryModule()
		{
			string name = "SavesDirectoryModule";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedDirectoryPath(path);
			
			service.CreateModule(path,ModuleLocationType.Directory);
			service.OpenModule(path,ModuleLocationType.Directory);
			
			Size size = new Size(8,8);			
			string area = "desert"; // areas are identified by their name in lower-case
			service.AddArea(area,true,size);
						
			NWN2ObjectType[] types = new NWN2ObjectType[] { NWN2ObjectType.Creature,
															NWN2ObjectType.Creature,
															NWN2ObjectType.Creature,
															NWN2ObjectType.Placeable,
															NWN2ObjectType.PlacedEffect };
			string[] resRefs = new string[] { "c_giantfire", "n_greycloak", "n_greycloak", "plc_bs_sunkenf", "n2_fx_bonfire" };
			string[] tags = new string[] { "giant", "duplicatecloak", "duplicatecloak", "inn", "bonfire" };
			
			for (int i = 0; i < types.Length; i++) {
				service.AddObject(area,types[i],resRefs[i],tags[i]);
			}
			
			service.SaveModule();
			
			service.CloseModule();
			
			service.OpenModule(path,ModuleLocationType.Directory);
						
			Assert.AreEqual(service.GetModuleName(),Path.GetFileNameWithoutExtension(path));
			Assert.AreEqual(1,service.GetAreaNames().Count);
			Assert.IsNotNull(service.GetArea(area,false));			
			service.OpenArea(area);
			Assert.AreEqual(3,service.GetObjectIDs(area,NWN2ObjectType.Creature).Count);
			
			service.CloseModule();			
			Delete(path);
		}
		
		
		[Test]
		public void SavesFileModule()
		{
			string name = "SavesFileModule.mod";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedFilePath(path);
			
			service.CreateModule(path,ModuleLocationType.File);
			service.OpenModule(path,ModuleLocationType.File);
			
			Size size = new Size(8,8);			
			string area = "desert"; // areas are identified by their name in lower-case
			service.AddArea(area,true,size);
						
			NWN2ObjectType[] types = new NWN2ObjectType[] { NWN2ObjectType.Creature,
															NWN2ObjectType.Creature,
															NWN2ObjectType.Creature,
															NWN2ObjectType.Placeable,
															NWN2ObjectType.PlacedEffect };
			string[] resRefs = new string[] { "c_giantfire", "n_greycloak", "n_greycloak", "plc_bs_sunkenf", "n2_fx_bonfire" };
			string[] tags = new string[] { "giant", "duplicatecloak", "duplicatecloak", "inn", "bonfire" };
			
			for (int i = 0; i < types.Length; i++) {
				service.AddObject(area,types[i],resRefs[i],tags[i]);
			}
			
			service.SaveModule();
			
			service.CloseModule();
			
			service.OpenModule(path,ModuleLocationType.File);
						
			Assert.AreEqual(service.GetModuleName(),Path.GetFileNameWithoutExtension(path));
			Assert.AreEqual(1,service.GetAreaNames().Count);
			Assert.IsNotNull(service.GetArea(area,false));
			service.OpenArea(area);
			Assert.AreEqual(3,service.GetObjectIDs(area,NWN2ObjectType.Creature).Count);
			
			service.CloseModule();			
			Delete(path);
		}
		
		
		[Test]
		public void RefusesToOpenNonExistentDirectoryModule()
		{
			string name = "RefusesToOpenNonExistentDirectoryModule";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedDirectoryPath(path);
			
			try {
				service.OpenModule(path,ModuleLocationType.Directory);	
				Assert.Fail("Didn't raise a FaultException<IOException> when asked to open a non-existent directory module.");
			}
			catch (FaultException<IOException>) {
				// expected result
			}
			catch (FaultException) {
				CreateService();
				Assert.Fail("Didn't raise a FaultException<IOException> when asked to open a non-existent directory module " +
				            "(raised a different exception).");
			}
		}
		
		
		[Test]
		public void RefusesToOpenNonExistentFileModule()
		{
			string name = "RefusesToOpenNonExistentFileModule.mod";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedFilePath(path);
			
			try {
				service.OpenModule(path,ModuleLocationType.File);	
				Assert.Fail("Didn't raise a FaultException<IOException> when asked to open a non-existent file module.");
			}
			catch (FaultException<IOException>) {
				// expected result
			}
			catch (FaultException) {
				CreateService();
				Assert.Fail("Didn't raise a FaultException<IOException> when asked to open a non-existent file module " +
				            "(raised a different exception).");
			}
		}
		
		
		[Test]
		public void OpensAndClosesAreas()
		{
			string name = "OpensAndClosesAreas.mod";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedFilePath(path);
			
			service.CreateModule(path,ModuleLocationType.File);
			service.OpenModule(path,ModuleLocationType.File);
			
			string area1 = "desert";
			string area2 = "castle";
			service.AddArea(area1,true,AreaBase.SmallestAreaSize);	
			service.AddArea(area2,false,AreaBase.SmallestAreaSize);
			
			Assert.AreEqual(2,service.GetOpenAreaNames().Count);
			
			service.CloseArea(area2);			
			Assert.AreEqual(1,service.GetOpenAreaNames().Count);
			
			service.CloseArea(area1);			
			Assert.AreEqual(0,service.GetOpenAreaNames().Count);
			
			service.OpenArea(area1);			
			Assert.AreEqual(1,service.GetOpenAreaNames().Count);
			
			service.OpenArea(area2);			
			Assert.AreEqual(2,service.GetOpenAreaNames().Count);
						
			service.CloseModule();			
			Delete(path);
		}
		
		
		[Test]
		public void OpensAndClosesScripts()
		{
			string name = "OpensAndClosesScripts.mod";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedFilePath(path);
			
			service.CreateModule(path,ModuleLocationType.File);
			service.OpenModule(path,ModuleLocationType.File);
			
			string script1 = "givegold";
			string script2 = "99bottles";
			service.AddScript(script1,sampleScripts.GiveGold);
			service.AddScript(script2,sampleScripts.Sing);		
			
			Assert.AreEqual(0,service.GetOpenScriptNames().Count);
			
			service.OpenScript(script1);			
			Assert.AreEqual(1,service.GetOpenScriptNames().Count);
			
			service.OpenScript(script2);			
			Assert.AreEqual(2,service.GetOpenScriptNames().Count);
			
			service.CloseScript(script2);			
			Assert.AreEqual(1,service.GetOpenScriptNames().Count);
			
			service.CloseScript(script1);			
			Assert.AreEqual(0,service.GetOpenScriptNames().Count);
						
			service.CloseModule();			
			Delete(path);
		}
		
		
		[Test]
		public void RefusesToOpenMoreThanThreeAreas()
		{
			string name = "RefusesToOpenMoreThanThreeAreas.mod";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedFilePath(path);
			
			service.CreateModule(path,ModuleLocationType.File);
			service.OpenModule(path,ModuleLocationType.File);
			
			string area1 = "desert";
			string area2 = "castle";
			string area3 = "forest";
			string area4 = "island";
			service.AddArea(area1,true,AreaBase.SmallestAreaSize);	
			service.CloseArea(area1);
			service.AddArea(area2,false,AreaBase.SmallestAreaSize);	
			service.CloseArea(area2);	
			service.AddArea(area3,true,AreaBase.SmallestAreaSize);	
			service.CloseArea(area3);
			service.AddArea(area4,true,AreaBase.SmallestAreaSize);	
			service.CloseArea(area4);
			
			service.OpenArea(area1);			
			service.OpenArea(area2);
			service.OpenArea(area3);
			Assert.AreEqual(3,service.GetOpenAreaNames().Count);
			
			try {
				service.OpenArea(area4);
				Assert.Fail("Didn't raise a FaultException<InvalidOperationException> when asked " +
				            "to open a fourth area (where 3 is the maximum).");
			}
			catch (FaultException<InvalidOperationException>) {
				// expected result
			}
			catch (FaultException) {
				CreateService();
				Assert.Fail("Didn't raise a FaultException<InvalidOperationException> when asked " +
				            "to open a fourth area (where 3 is the maximum).");
			}
			
			Assert.AreEqual(3,service.GetOpenAreaNames().Count);
						
			service.CloseModule();			
			Delete(path);
		}
		
		
		[Test]
		public void ReportsWhetherAreaIsOpen()
		{
			string name = "ReportsWhetherAreaIsOpen.mod";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedFilePath(path);
			
			service.CreateModule(path,ModuleLocationType.File);
			service.OpenModule(path,ModuleLocationType.File);
			
			string area1 = "desert";
			string area2 = "castle";
			service.AddArea(area1,true,AreaBase.SmallestAreaSize);	
			service.AddArea(area2,false,AreaBase.SmallestAreaSize);	
			
			Assert.IsTrue(service.AreaIsOpen(area1));
			Assert.IsTrue(service.AreaIsOpen(area2));
			
			service.CloseArea(area1);
			service.CloseArea(area2);
			
			Assert.IsFalse(service.AreaIsOpen(area1));
			Assert.IsFalse(service.AreaIsOpen(area2));
			
			service.OpenArea(area1);
			
			Assert.IsTrue(service.AreaIsOpen(area1));
			Assert.IsFalse(service.AreaIsOpen(area2));
			
			service.OpenArea(area2);
			
			Assert.IsTrue(service.AreaIsOpen(area1));
			Assert.IsTrue(service.AreaIsOpen(area2));
			
			service.CloseArea(area1);
			
			Assert.IsFalse(service.AreaIsOpen(area1));
			Assert.IsTrue(service.AreaIsOpen(area2));
			
			service.CloseArea(area2);
			
			Assert.IsFalse(service.AreaIsOpen(area1));
			Assert.IsFalse(service.AreaIsOpen(area2));
						
			service.CloseModule();			
			Delete(path);
		}
		
		
		[Test]
		public void ReportsWhetherScriptIsOpen()
		{
			string name = "ReportsWhetherScriptIsOpen.mod";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedFilePath(path);
			
			service.CreateModule(path,ModuleLocationType.File);
			service.OpenModule(path,ModuleLocationType.File);
						
			string script1 = "givegold";
			string script2 = "99bottles";
			service.AddScript(script1,sampleScripts.GiveGold);			
			service.AddScript(script2,sampleScripts.Sing);	
			
			Assert.IsFalse(service.ScriptIsOpen(script1));
			Assert.IsFalse(service.ScriptIsOpen(script2));
			
			service.OpenScript(script1);
			
			Assert.IsTrue(service.ScriptIsOpen(script1));
			Assert.IsFalse(service.ScriptIsOpen(script2));
			
			service.OpenScript(script2);
			
			Assert.IsTrue(service.ScriptIsOpen(script1));
			Assert.IsTrue(service.ScriptIsOpen(script2));
			
			service.CloseScript(script1);
			
			Assert.IsFalse(service.ScriptIsOpen(script1));
			Assert.IsTrue(service.ScriptIsOpen(script2));
			
			service.CloseScript(script2);
			
			Assert.IsFalse(service.ScriptIsOpen(script1));
			Assert.IsFalse(service.ScriptIsOpen(script2));
						
			service.CloseModule();			
			Delete(path);
		}
		
		
		[Test]
		public void ReportsWhetherAreaResourceIsLoaded()
		{
			string name = "ReportsWhetherAreaResourceIsLoaded.mod";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedFilePath(path);
			
			service.CreateModule(path,ModuleLocationType.File);
			service.OpenModule(path,ModuleLocationType.File);
			
			string area1 = "desert";
			string area2 = "castle";
			service.AddArea(area1,true,AreaBase.SmallestAreaSize);	
			service.AddArea(area2,false,AreaBase.SmallestAreaSize);		
			
			Assert.AreEqual("True",service.GetArea(area1,false)["Loaded"]);
			Assert.AreEqual("True",service.GetArea(area2,false)["Loaded"]);	
			
			service.CloseArea(area1);
			service.CloseArea(area2);
			
			Assert.AreEqual("False",service.GetArea(area1,false)["Loaded"]);
			Assert.AreEqual("False",service.GetArea(area2,false)["Loaded"]);
			
			service.OpenArea(area1);
						
			Assert.AreEqual("True",service.GetArea(area1,false)["Loaded"]);
			Assert.AreEqual("False",service.GetArea(area2,false)["Loaded"]);
			
			service.OpenArea(area2);
			
			Assert.AreEqual("True",service.GetArea(area1,false)["Loaded"]);
			Assert.AreEqual("True",service.GetArea(area2,false)["Loaded"]);
			
			service.CloseArea(area1);
			
			Assert.AreEqual("False",service.GetArea(area1,false)["Loaded"]);
			Assert.AreEqual("True",service.GetArea(area2,false)["Loaded"]);
			
			service.CloseArea(area2);
			
			Assert.AreEqual("False",service.GetArea(area1,false)["Loaded"]);
			Assert.AreEqual("False",service.GetArea(area2,false)["Loaded"]);
						
			service.CloseModule();			
			Delete(path);
		}
		
		
		[Test]
		public void ReportsWhetherScriptResourceIsLoaded()
		{
			string name = "ReportsWhetherScriptResourceIsLoaded.mod";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedFilePath(path);
			
			service.CreateModule(path,ModuleLocationType.File);
			service.OpenModule(path,ModuleLocationType.File);
						
			string script1 = "givegold";
			string script2 = "99bottles";
			service.AddScript(script1,sampleScripts.GiveGold);
			service.AddScript(script2,sampleScripts.Sing);	
			
			Assert.AreEqual("False",service.GetScript(script1)["Loaded"]);
			Assert.AreEqual("False",service.GetScript(script2)["Loaded"]);
			
			service.OpenScript(script1);
			
			Assert.AreEqual("True",service.GetScript(script1)["Loaded"]);
			Assert.AreEqual("False",service.GetScript(script2)["Loaded"]);
			
			service.OpenScript(script2);
			
			Assert.AreEqual("True",service.GetScript(script1)["Loaded"]);
			Assert.AreEqual("True",service.GetScript(script2)["Loaded"]);
			
			service.CloseScript(script1);
			
			Assert.AreEqual("False",service.GetScript(script1)["Loaded"]);
			Assert.AreEqual("True",service.GetScript(script2)["Loaded"]);
			
			service.CloseScript(script2);
			
			Assert.AreEqual("False",service.GetScript(script1)["Loaded"]);
			Assert.AreEqual("False",service.GetScript(script2)["Loaded"]);
						
			service.CloseModule();			
			Delete(path);
		}
		
		
		[Test]
		public void DemandsAndReleasesScript()
		{
			string name = "DemandsAndReleasesScript.mod";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedFilePath(path);
			
			service.CreateModule(path,ModuleLocationType.File);
			service.OpenModule(path,ModuleLocationType.File);
						
			string script1 = "givegold";
			service.AddScript(script1,sampleScripts.GiveGold);		
			Assert.AreEqual("False",service.GetScript(script1)["Loaded"]);
			
			service.DemandScript(script1);			
			Assert.AreEqual("True",service.GetScript(script1)["Loaded"]);
			
			service.DemandScript(script1);			
			Assert.AreEqual("True",service.GetScript(script1)["Loaded"]);
			
			service.ReleaseScript(script1);			
			Assert.AreEqual("True",service.GetScript(script1)["Loaded"]);
			
			service.ReleaseScript(script1);			
			Assert.AreEqual("False",service.GetScript(script1)["Loaded"]);
						
			try {
				service.DemandScript("nonexistentscript");
				Assert.Fail("Didn't raise a FaultException<ArgumentException> when asked to demand a non-existent script.");
			}
			catch (FaultException<ArgumentException>) {
				// expected result
			}
			catch (FaultException) {
				CreateService();
				Assert.Fail("Didn't raise a FaultException<ArgumentException> when asked to demand a non-existent script.");
			}
						
			try {
				service.ReleaseScript("nonexistentscript");
				Assert.Fail("Didn't raise a FaultException<ArgumentException> when asked to release a non-existent script.");
			}
			catch (FaultException<ArgumentException>) {
				// expected result
			}
			catch (FaultException) {
				CreateService();
				Assert.Fail("Didn't raise a FaultException<ArgumentException> when asked to release a non-existent script.");
			}
			
			service.CloseModule();			
			Delete(path);
		}
		
		
		[Test]
		public void DemandsAndReleasesArea()
		{
			string name = "DemandsAndReleasesArea.mod";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedFilePath(path);
			
			service.CreateModule(path,ModuleLocationType.File);
			service.OpenModule(path,ModuleLocationType.File);
						
			string area = "forest";
			service.AddArea(area,true,AreaBase.SmallestAreaSize);			
			Assert.AreEqual("True",service.GetArea(area,false)["Loaded"]);
			
			service.ReleaseArea(area);			
			Assert.AreEqual("False",service.GetArea(area,false)["Loaded"]);
			
			service.DemandArea(area);			
			Assert.AreEqual("True",service.GetArea(area,false)["Loaded"]);
			
			service.ReleaseArea(area);			
			Assert.AreEqual("False",service.GetArea(area,false)["Loaded"]);
			
			// Demand()s and Release()s 'stack up':
			service.DemandArea(area);			
			Assert.AreEqual("True",service.GetArea(area,false)["Loaded"]);
			service.DemandArea(area);			
			Assert.AreEqual("True",service.GetArea(area,false)["Loaded"]);
			service.ReleaseArea(area);	
			Assert.AreEqual("True",service.GetArea(area,false)["Loaded"]);	
			service.ReleaseArea(area);		
			Assert.AreEqual("False",service.GetArea(area,false)["Loaded"]);
						
			try {
				service.DemandArea("nonexistent");
				Assert.Fail("Didn't raise a FaultException<ArgumentException> when asked to demand a non-existent area.");
			}
			catch (FaultException<ArgumentException>) {
				// expected result
			}
			catch (FaultException) {
				CreateService();
				Assert.Fail("Didn't raise a FaultException<ArgumentException> when asked to demand a non-existent area.");
			}
						
			try {
				service.ReleaseArea("nonexistent");
				Assert.Fail("Didn't raise a FaultException<ArgumentException> when asked to release a non-existent area.");
			}
			catch (FaultException<ArgumentException>) {
				// expected result
			}
			catch (FaultException) {
				CreateService();
				Assert.Fail("Didn't raise a FaultException<ArgumentException> when asked to release a non-existent area.");
			}
			
			service.CloseModule();			
			Delete(path);
		}
		
		#endregion
		
		#region Methods
				
		/// <summary>
		/// Creates and returns a proxy for the service.
		/// </summary>
		private void CreateService()
		{
			service = pipeChannelFactory.CreateChannel();
		}
				
		
		/// <summary>
		/// Runs the toolset, waits until it has loaded, and minimises it.
		/// </summary>
		/// <param name="minimise">True to minimise the toolset upon loading, 
		/// false otherwise.</param>
		private void WaitForToolsetToLoad(bool minimise)
		{			
			Nwn2ToolsetFunctions.RunNeverwinterNightsTwoToolset();	
			
			AutomationElement toolset = null;
			
			while (toolset == null) {
				foreach (AutomationElement e in AutomationElement.RootElement.FindAll(TreeScope.Children,Condition.TrueCondition)) {						
					try {
						string name = (string)e.GetCurrentPropertyValue(AutomationElement.NameProperty);
						if (name.Contains("Obsidian Neverwinter Nights 2 Toolset:")) {
							toolset = e;
							break;
						}
					}
					catch (ElementNotAvailableException) {
						/* From MSDN:
						 * Raised when an attempt is made to access an UI Automation element 
						 * corresponding to a part of the user interface that is no longer available.
						 * 
						 * This exception can be raised if the element was in a dialog box that was closed, 
						 * or an application that was closed or terminated. 
						 */
					}
					
				}
				Thread.Sleep(500);
			} 
			
			((WindowPattern)toolset.GetCurrentPattern(WindowPattern.Pattern)).SetWindowVisualState(WindowVisualState.Minimized);
		}
		
		
		/// <summary>
		/// Deletes the file or directory at the given path.
		/// </summary>
		/// <param name="path">The path to delete.</param>
		private void Delete(string path)
		{
			if (File.Exists(path)) {
				File.Delete(path);
			}
			else {
				DirectoryInfo dir = new DirectoryInfo(path);
				if (!dir.Exists) {
					throw new ArgumentException("No file or directory found at the given path (" + path + ").","path");
				}
				foreach (FileInfo file in dir.GetFiles()) {
					file.Delete();
				}
				dir.Delete();
			}
		}
		
		
		/// <summary>
		/// Checks for a compiled script with the given name in the current module,
		/// timing out after a specified time.
		/// </summary>
		/// <param name="scriptName">The script name.</param>
		/// <param name="timeout">The number of milliseconds to check for.</param>
		/// <returns>True if the compiled script file was found, false otherwise.</returns>
		private bool WaitForCompiledScriptToAppear(string scriptName, int timeout)
		{			
			int original = timeout;
			int interval = 1;
			while (!service.HasCompiled(scriptName) && timeout >= 0) {
				Thread.Sleep(interval);
				timeout -= interval;
			}
			return service.HasCompiled(scriptName);
		}
		
				
		/// <summary>
		/// Checks for a compiled script with the given name in the current module,
		/// timing out after 1 second.
		/// </summary>
		/// <param name="scriptName">The script name.</param>
		private bool WaitForCompiledScriptToAppear(string scriptName)
		{			
			return WaitForCompiledScriptToAppear(scriptName,1000);
		}
		
		#endregion
	}
}
