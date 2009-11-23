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
							
				Console.WriteLine("Waiting for toolset to load...");	
				
				WaitForToolsetToLoad(true);					
				
				NetNamedPipeBinding binding = new NetNamedPipeBinding();
				binding.MaxReceivedMessageSize = Int32.MaxValue;
				pipeChannelFactory = new ChannelFactory<INwn2Service>(binding,"net.pipe://localhost/NamedPipeEndpoint");									
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
			Console.WriteLine("Test suite completed. Closing toolset.");
			Nwn2ToolsetFunctions.KillNeverwinterNightsTwoToolset();
			
			if (File.Exists(precompiled99bottlesScriptPath)) File.Delete(precompiled99bottlesScriptPath);
			if (File.Exists(uncompiled99bottlesScriptPath)) File.Delete(uncompiled99bottlesScriptPath);
			
			Thread.Sleep(250);
		}
		
		#endregion
		
		#region Tests - Scripts
				
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
			
			Bean area = service.GetArea(areaName);
			Assert.IsNotNull(area);			
			
			string attachedScriptName = area.GetValue("OnExitScript");
			Assert.AreEqual(scriptName,attachedScriptName);
			
			service.ClearScriptSlotOnArea(areaName,"OnExitScript");
			
			area = service.GetArea(areaName);
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
			service.AddObject(areaName,NWN2ObjectType.Door,"plc_dc_basic01","door");
			
			string scriptName = "givegold";
			service.AddScript(scriptName,sampleScripts.GiveGold);
			service.CompileScript(scriptName);
			Assert.IsTrue(WaitForCompiledScriptToAppear(scriptName),"The compiled script file was never found.");
			
			Bean door = service.GetObjects(areaName,NWN2ObjectType.Door,"door")[0];
			Guid objectId = new Guid(door.GetValue("ObjectID"));
			
			service.AttachScriptToObject(scriptName,areaName,Nwn2EventRaiser.Door,objectId,"OnFailToOpen");
			
			door = service.GetObject(areaName,NWN2ObjectType.Door,objectId);			
			string attachedScriptName = door.GetValue("OnFailToOpen");
			Assert.AreEqual(scriptName,attachedScriptName);
			
			service.ClearScriptSlotOnObject(areaName,objectId,Nwn2EventRaiser.Door,"OnFailToOpen");
			
			door = service.GetObject(areaName,NWN2ObjectType.Door,objectId);			
			attachedScriptName = door.GetValue("OnFailToOpen");
			Assert.IsNotNull(attachedScriptName);
			Assert.IsEmpty(attachedScriptName);
			
			// Refuses to clear unknown script slot:		
			try {
				service.ClearScriptSlotOnObject(areaName,objectId,Nwn2EventRaiser.Door,"BishBashBosh");
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
			
			IList<Bean> beans = service.GetObjects(area,NWN2ObjectType.Creature,null);
			Assert.IsNotNull(beans);
			Assert.AreEqual(2,beans.Count);
					
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
		
			Assert.IsTrue(cat.HasValue("Strength"));
			Assert.AreEqual(3,int.Parse(cat.GetValue("Strength")));
		
			Assert.IsTrue(cat.HasValue("FactionID"));
			Assert.AreEqual(2,int.Parse(cat.GetValue("FactionID")));
		
			Assert.IsTrue(giant.HasValue("Strength"));
			Assert.AreEqual(31,int.Parse(giant.GetValue("Strength")));
		
			Assert.IsTrue(giant.HasValue("FactionID"));
			Assert.AreEqual(1,int.Parse(giant.GetValue("FactionID")));			
			
			beans = service.GetObjects(area,NWN2ObjectType.Item,null);
			Assert.IsNotNull(beans);
			Assert.AreEqual(1,beans.Count);
			
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
						
			Assert.IsTrue(sword.HasValue("LocalizedName"));
			Assert.AreEqual("Darksteel Greatsword",sword.GetValue("LocalizedName"));
			
			Assert.IsTrue(sword.HasValue("LocalizedDescriptionIdentified"));
			Assert.AreEqual("Darksteel is silvery in hue when polished or cut, but " +
			                "its exposed surfaces have a deep, gleaming purple luster. " +
			                "This alloy of meteoric iron and steel is tempered with rare, " +
			                "magical oils to give the metal its uncanny abilities. Darksteel " +
			                "is commonly used in the crafting of magic items related to storms " +
			                "or lightning.",sword.GetValue("LocalizedDescriptionIdentified"));
			
			Assert.IsTrue(sword.HasValue("Stolen"));
			Assert.AreEqual("False",sword.GetValue("Stolen"));
						
			beans = service.GetObjects(area,NWN2ObjectType.Creature,"giant");
			Assert.IsNotNull(beans);
			Assert.AreEqual(1,beans.Count);
			
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
			
			Assert.AreEqual(1,service.GetObjects(area,NWN2ObjectType.Creature,"cat7").Count);
			Assert.AreEqual(1,service.GetObjects(area,NWN2ObjectType.Item,"sword4").Count);
			Bean cat = service.GetObjects(area,NWN2ObjectType.Creature,"cat7")[0];
			Bean sword = service.GetObjects(area,NWN2ObjectType.Item,"sword4")[0];
			
			Assert.IsTrue(cat.HasValue("ObjectID"));
			Assert.IsTrue(sword.HasValue("ObjectID"));			
			string catID = cat.GetValue("ObjectID");
			string swordID = sword.GetValue("ObjectID");			
			Assert.IsNotEmpty(catID);
			Assert.IsNotEmpty(swordID);
			Assert.AreNotEqual(catID,swordID);
			
			Bean retrievedCat = service.GetObject(area,NWN2ObjectType.Creature,new Guid(catID));
			Bean retrievedSword = service.GetObject(area,NWN2ObjectType.Item,new Guid(swordID));
			
			Assert.AreEqual(cat,retrievedCat);
			Assert.AreEqual(sword,retrievedSword);
			
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
			
			IList<Bean> scripts = service.GetUncompiledScripts();
			Assert.AreEqual(0,scripts.Count);
			
			// This script compiles in the toolset as entered here (hand-tested):
			string scriptData = sampleScripts.Sing;
			string scriptName = "uncompiled";
			
			service.AddScript(scriptName,scriptData);
			
			scripts = service.GetUncompiledScripts();
			Assert.AreEqual(1,scripts.Count);
			Bean script = scripts[0];
			
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
			
			IList<Bean> scripts = service.GetUncompiledScripts();
			Assert.AreEqual(0,scripts.Count);
			
			// This script compiles in the toolset as entered here (hand-tested):
			string scriptData = sampleScripts.Sing;
			string scriptName = "uncompiled";
			
			service.AddScript(scriptName,scriptData);
			
			scripts = service.GetUncompiledScripts();
			Assert.AreEqual(1,scripts.Count);
			Bean script = scripts[0];
			
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
			Assert.AreEqual(0,service.GetUncompiledScripts().Count);
			
			service.AddScript(scriptName,scriptData);
			
			Assert.IsFalse(service.HasCompiled(scriptName));
			Assert.IsTrue(service.HasUncompiled(scriptName));
			Assert.AreEqual(1,service.GetUncompiledScripts().Count);
			
			service.CompileScript(scriptName);					
			Assert.IsTrue(WaitForCompiledScriptToAppear(scriptName),"The compiled script file was never found.");
			
			Assert.IsTrue(service.HasUncompiled(scriptName));
			Assert.AreEqual(1,service.GetUncompiledScripts().Count);
			
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
			Assert.AreEqual(0,service.GetUncompiledScripts().Count);
			
			service.AddScript(scriptName,scriptData);
			
			Assert.IsFalse(service.HasCompiled(scriptName));
			Assert.IsTrue(service.HasUncompiled(scriptName));
			Assert.AreEqual(1,service.GetUncompiledScripts().Count);
			
			service.CompileScript(scriptName);			
			Assert.IsTrue(WaitForCompiledScriptToAppear(scriptName),"The compiled script file was never found.");
			
			Assert.IsTrue(service.HasUncompiled(scriptName));
			Assert.AreEqual(1,service.GetUncompiledScripts().Count);
			
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
			
			// Check that ObjectIDs are unique (shouldn't be an issue, but just for safety):
			List<Guid> idlist = new List<Guid>(20);
			foreach (Bean bean in service.GetObjects(area,NWN2ObjectType.Creature,null)) {
				Assert.IsTrue(bean.HasValue("ObjectID"));
				Guid id = new Guid(bean.GetValue("ObjectID"));
				Assert.IsFalse(idlist.Contains(id));
				idlist.Add(id);
			}
			
			IList<Bean> cats = service.GetObjects(area,NWN2ObjectType.Creature,"cat9");
			Assert.AreEqual(1,cats.Count);
			Bean cat = cats[0];
			
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
			
			service.AttachScriptToObject(scriptName,area,Nwn2EventRaiser.Creature,catID,"OnSpawnIn");	
						
			cat = service.GetObject(area,NWN2ObjectType.Creature,catID);
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
			
			Bean before = service.GetArea(areaName);
			Assert.IsTrue(before.HasValue(scriptSlot));
			
			Assert.AreEqual(scriptName,before.GetValue(scriptSlot));
			
			service.SaveModule();
			service.CloseModule();
			service.OpenModule(path,ModuleLocationType.File);
				
			Bean after = service.GetArea(areaName);
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
			service.AddObject(area,NWN2ObjectType.Creature,"c_cat","cat");
			
			Bean cat = service.GetObjects(area,NWN2ObjectType.Creature,"cat")[0];						
			Guid catID = new Guid(cat.GetValue("ObjectID"));
			
			string scriptData = sampleScripts.Sing;
			string scriptName = "attachingscript";
			
			service.AddScript(scriptName,scriptData);
			
			service.CompileScript(scriptName);
			Assert.IsTrue(WaitForCompiledScriptToAppear(scriptName),"The compiled script file was never found.");
			
			string scriptSlot = "fake script slot";
			foreach (Nwn2EventRaiser eventRaiser in Enum.GetValues(typeof(Nwn2EventRaiser))) {
				Assert.IsTrue(!(Nwn2ScriptSlot.GetScriptSlotNames(eventRaiser).Contains(scriptSlot)));
			}
			
			try {
				service.AttachScriptToObject(scriptName,area,Nwn2EventRaiser.Creature,catID,scriptSlot);
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
			foreach (string scriptSlot in Nwn2ScriptSlot.GetScriptSlotNames(Nwn2EventRaiser.Module)) {
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
			Assert.IsNull(service.GetArea(wrongAreaName));
			
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
			service.AddArea(area1,true,AreaBase.SmallestAreaSize);		
			
			string area2 = "inside";
			service.AddArea(area2,false,AreaBase.SmallestAreaSize);	
			
			Bean area1Bean = service.GetArea(area1);
			Assert.IsNotNull(area1Bean);
			Assert.IsTrue(area1Bean.HasValue("HasTerrain"));
			Assert.AreEqual("True",area1Bean.GetValue("HasTerrain"));
			
			Bean area2Bean = service.GetArea(area2);
			Assert.IsNotNull(area2Bean);
			Assert.IsTrue(area2Bean.HasValue("HasTerrain"));
			Assert.AreEqual("False",area2Bean.GetValue("HasTerrain"));
			
			area1Bean = null;
			area2Bean = null;
			
			IList<Bean> areas = service.GetAreas();
			Assert.IsNotNull(areas);
			Assert.AreEqual(2,areas.Count);
			
			foreach (Bean b in areas) {
				string n = b.GetValue("Name");
				if (n == area1) area1Bean = b;
				else if (n == area2) area2Bean = b;
			}
						
			Assert.IsNotNull(area1Bean);
			Assert.IsTrue(area1Bean.HasValue("HasTerrain"));
			Assert.AreEqual("True",area1Bean.GetValue("HasTerrain"));
			
			Assert.IsNotNull(area2Bean);
			Assert.IsTrue(area2Bean.HasValue("HasTerrain"));
			Assert.AreEqual("False",area2Bean.GetValue("HasTerrain"));
			
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
			service.AddObject(area,NWN2ObjectType.Creature,"c_cat","cat");
			
			Bean cat = service.GetObjects(area,NWN2ObjectType.Creature,"cat")[0];
			Guid catID = new Guid(cat.GetValue("ObjectID"));
			
			string scriptData = sampleScripts.Sing;
			string scriptName = "uncompiled script";
			
			service.AddScript(scriptName,scriptData);
			
			Assert.IsTrue(service.HasUncompiled(scriptName));
			Assert.IsFalse(service.HasCompiled(scriptName));	
			
			try {
				service.AttachScriptToObject(scriptName,area,Nwn2EventRaiser.Creature,catID,"OnSpawnIn");
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
				service.AttachScriptToObject(scriptName,area,Nwn2EventRaiser.Creature,catID,"OnSpawnIn");
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
			service.AddObject(area,NWN2ObjectType.Creature,"c_cat","cat");
			
			IList<Bean> cats = service.GetObjects(area,NWN2ObjectType.Creature,"cat");
			Assert.AreEqual(1,cats.Count);
			Bean cat = cats[0];
			
			Guid catID = new Guid(cat.GetValue("ObjectID"));
						
			string scriptData = sampleScripts.Sing;
			string scriptName = "attachingscript";
			
			service.AddScript(scriptName,scriptData);
			service.CompileScript(scriptName);
			Assert.IsTrue(WaitForCompiledScriptToAppear(scriptName),"The compiled script file was never found.");
			service.AttachScriptToObject(scriptName,area,Nwn2EventRaiser.Creature,catID,"OnSpawnIn");	
			
			// Before...
			Assert.IsNotNull(service.GetUncompiledScript(scriptName));
			Assert.AreEqual(1,service.GetUncompiledScripts().Count);
			Assert.IsTrue(service.HasCompiled(scriptName));
			Assert.IsTrue(service.HasUncompiled(scriptName));
						
			cat = service.GetObject(area,NWN2ObjectType.Creature,catID);
			Assert.IsNotNull(cat);
			
			string catSpawnScript = cat.GetValue("OnSpawnIn");
			Assert.IsNotNull(catSpawnScript);
			Assert.AreEqual(scriptName,catSpawnScript);
			
			Assert.AreEqual(1,service.GetUncompiledScripts().Count);
			Bean script = service.GetUncompiledScript(scriptName);
			Assert.IsNotNull(script);
			Assert.AreEqual(sampleScripts.Sing,script["Data"]);
						
			service.SaveModule();
			service.CloseModule();
			service.OpenModule(path,ModuleLocationType.File);
			
			// And after...
			Assert.IsNotNull(service.GetUncompiledScript(scriptName));
			Assert.AreEqual(1,service.GetUncompiledScripts().Count);
			Assert.IsTrue(service.HasCompiled(scriptName));
			Assert.IsTrue(service.HasUncompiled(scriptName));
			
			cat = service.GetObject(area,NWN2ObjectType.Creature,catID);
			Assert.IsNotNull(cat);
			
			catSpawnScript = cat.GetValue("OnSpawnIn");
			Assert.IsNotNull(catSpawnScript);
			Assert.AreEqual(scriptName,catSpawnScript);
			
			Assert.AreEqual(1,service.GetUncompiledScripts().Count);
			script = service.GetUncompiledScript(scriptName);
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
			service.AddObject(area,NWN2ObjectType.Creature,"c_cat","cat");
			
			IList<Bean> cats = service.GetObjects(area,NWN2ObjectType.Creature,"cat");
			Assert.AreEqual(1,cats.Count);
			Bean cat = cats[0];
			
			Guid catID = new Guid(cat.GetValue("ObjectID"));
						
			string scriptData = sampleScripts.Sing;
			string scriptName = "attachingscript";
			
			service.AddScript(scriptName,scriptData);
			service.CompileScript(scriptName);		
			Assert.IsTrue(WaitForCompiledScriptToAppear(scriptName),"The compiled script file was never found.");	
			service.AttachScriptToObject(scriptName,area,Nwn2EventRaiser.Creature,catID,"OnSpawnIn");	
			
			// Before...
			Assert.IsNotNull(service.GetUncompiledScript(scriptName));
			Assert.AreEqual(1,service.GetUncompiledScripts().Count);
			Assert.IsTrue(service.HasCompiled(scriptName));
			Assert.IsTrue(service.HasUncompiled(scriptName));
						
			cat = service.GetObject(area,NWN2ObjectType.Creature,catID);
			Assert.IsNotNull(cat);
			
			string catSpawnScript = cat.GetValue("OnSpawnIn");
			Assert.IsNotNull(catSpawnScript);
			Assert.AreEqual(scriptName,catSpawnScript);
			
			Assert.AreEqual(1,service.GetUncompiledScripts().Count);
			Bean script = service.GetUncompiledScript(scriptName);
			Assert.IsNotNull(script);
			Assert.AreEqual(sampleScripts.Sing,script["Data"]);
						
			service.SaveModule();
			service.CloseModule();
			service.OpenModule(path,ModuleLocationType.Directory);
			
			// And after...
			Assert.IsNotNull(service.GetUncompiledScript(scriptName));
			Assert.AreEqual(1,service.GetUncompiledScripts().Count);
			Assert.IsTrue(service.HasCompiled(scriptName));
			Assert.IsTrue(service.HasUncompiled(scriptName));
						
			cat = service.GetObject(area,NWN2ObjectType.Creature,catID);
			Assert.IsNotNull(cat);
			
			catSpawnScript = cat.GetValue("OnSpawnIn");
			Assert.IsNotNull(catSpawnScript);
			Assert.AreEqual(scriptName,catSpawnScript);
			
			Assert.AreEqual(1,service.GetUncompiledScripts().Count);
			script = service.GetUncompiledScript(scriptName);
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
			Bean before = service.GetUncompiledScript(scriptName);
			Assert.IsNotNull(before);
			Assert.AreEqual(sampleScripts.Sing,before["Data"]);
			Assert.AreEqual(1,service.GetUncompiledScripts().Count);
						
			service.CloseModule();
			service.OpenModule(path,ModuleLocationType.File);
			
			// And after...
			Bean after = service.GetUncompiledScript(scriptName);
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
			Bean before = service.GetUncompiledScript(scriptName);
			Assert.IsNotNull(before);
			Assert.AreEqual(sampleScripts.Sing,before["Data"]);
			Assert.AreEqual(1,service.GetUncompiledScripts().Count);
						
			service.SaveModule();
			service.CloseModule();
			service.OpenModule(path,ModuleLocationType.File);
			
			// And after...
			Bean after = service.GetUncompiledScript(scriptName);
			Assert.IsNotNull(after);
			Assert.AreEqual(sampleScripts.Sing,after["Data"]);
			Assert.AreEqual(1,service.GetUncompiledScripts().Count);
			
			service.CloseModule();			
			Delete(path);
		}
		
		
		[Test]
		public void UnsavedScriptDoesNotPersistInDirectoryModule()
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
			Bean before = service.GetUncompiledScript(scriptName);
			Assert.IsNotNull(before);
			Assert.AreEqual(sampleScripts.Sing,before["Data"]);
			Assert.AreEqual(1,service.GetUncompiledScripts().Count);
						
			service.CloseModule();
			service.OpenModule(path,ModuleLocationType.Directory);
			
			// And after...
			Bean after = service.GetUncompiledScript(scriptName);			
			// Assert.IsNotNull(after); // unfortunately, the FILE will persist, but it should be empty:
			Assert.AreEqual(String.Empty,after["Data"]);
			
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
			Bean before = service.GetUncompiledScript(scriptName);
			Assert.IsNotNull(before);
			Assert.AreEqual(sampleScripts.Sing,before["Data"]);
			Assert.AreEqual(1,service.GetUncompiledScripts().Count);
			
			service.SaveModule();
			service.CloseModule();
			service.OpenModule(path,ModuleLocationType.Directory);
			
			// And after...
			Bean after = service.GetUncompiledScript(scriptName);
			Assert.IsNotNull(after);
			Assert.AreEqual(sampleScripts.Sing,after["Data"]);
			Assert.AreEqual(1,service.GetUncompiledScripts().Count);
			
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
			
			// GetUncompiledScript
			Assert.IsNotNull(givegoldBean = service.GetUncompiledScript(givegold));
			Assert.IsNotNull(changenameBean = service.GetUncompiledScript(changename));
			Assert.IsNull(_99bottlesBean = service.GetUncompiledScript(_99bottles));
			Assert.AreEqual(givegoldData,givegoldBean.GetValue("Data"));
			Assert.AreEqual(changenameData,changenameBean.GetValue("Data"));
						
			// GetUncompiledScripts
			givegoldBean = null;
			changenameBean = null;
			_99bottlesBean = null;
			IList<Bean> beans = service.GetUncompiledScripts();
			foreach (Bean bean in beans) {
				string n = bean.GetValue("Name");
				if (n == givegold) givegoldBean = bean;
				else if (n == changename) changenameBean = bean;
				else if (n == _99bottles) _99bottlesBean = bean;
			}			
			
			Assert.IsNotNull(givegoldBean);
			Assert.IsNotNull(changenameBean);
			Assert.IsNull(_99bottlesBean);
			Assert.AreEqual(givegoldData,givegoldBean.GetValue("Data"));
			Assert.AreEqual(changenameData,changenameBean.GetValue("Data"));
			
			// GetCompiledScript
			givegoldBean = null;
			changenameBean = null;
			_99bottlesBean = null;
			Assert.IsNotNull(givegoldBean = service.GetCompiledScript(givegold));
			Assert.IsNull(changenameBean = service.GetCompiledScript(changename));
			Assert.IsNotNull(_99bottlesBean = service.GetCompiledScript(_99bottles));
			
			// GetCompiledScripts
			givegoldBean = null;
			changenameBean = null;
			_99bottlesBean = null;
			beans = service.GetCompiledScripts();
			foreach (Bean bean in beans) {
				string n = bean.GetValue("Name");
				if (n == givegold) givegoldBean = bean;
				else if (n == changename) changenameBean = bean;
				else if (n == _99bottles) _99bottlesBean = bean;
			}			
			
			Assert.IsNotNull(givegoldBean);
			Assert.IsNull(changenameBean);
			Assert.IsNotNull(_99bottlesBean);
						
			service.CloseModule();			
			Delete(path);
		}
		
		#endregion
		
		#region Tests - I/O
		
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
		public void ReturnsCorrectModulePath()
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
			
			Size size = new Size(Area.MinimumAreaLength,Area.MinimumAreaLength);
			
			string area1 = "land of wind and ghosts";
			string area2 = "land of milk and honey";
			
			service.AddArea(area1,true,size);
			service.AddArea(area2,false,size);
			service.SaveModule(); // save after adding an area to a file module if the area must persist
			
			service.CloseModule();			
			service.OpenModule(path,ModuleLocationType.File);
			
			IList<Bean> areas = service.GetAreas();
			Assert.IsNotNull(areas);
			Assert.AreEqual(2,areas.Count);
						
			bool area1found = false;
			bool area2found = false;
			foreach (Bean b in areas) {
				string n = b.GetValue("Name");
				if (n == area1) area1found = true;
				else if (n == area2) area2found = true;
			}
			Assert.IsTrue(area1found);
			Assert.IsTrue(area2found);
			
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
			
			Size size = new Size(Area.MinimumAreaLength,Area.MinimumAreaLength);
			
			string area1 = "alaska";
			string area2 = "hawaii";
			
			service.AddArea(area1,true,size);
			service.AddArea(area2,false,size);
			
			service.CloseModule();
			service.OpenModule(path,ModuleLocationType.Directory);
			
			IList<Bean> areas = service.GetAreas();
			Assert.IsNotNull(areas);
			Assert.AreEqual(2,areas.Count);
						
			bool area1found = false;
			bool area2found = false;
			foreach (Bean b in areas) {
				string n = b.GetValue("Name");
				if (n == area1) area1found = true;
				else if (n == area2) area2found = true;
			}
			Assert.IsTrue(area1found);
			Assert.IsTrue(area2found);
			
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
			Size size = new Size(Area.MinimumAreaLength,Area.MinimumAreaLength);
			
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
			
			Assert.AreEqual(1,service.GetObjectCount(area,NWN2ObjectType.Creature,"giant"));
			Assert.AreEqual(2,service.GetObjectCount(area,NWN2ObjectType.Creature,"duplicatecloak"));
			Assert.AreEqual(1,service.GetObjectCount(area,NWN2ObjectType.Placeable,"inn"));
			Assert.AreEqual(1,service.GetObjectCount(area,NWN2ObjectType.PlacedEffect,"bonfire"));
			Assert.AreEqual(3,service.GetObjectCount(area,NWN2ObjectType.Creature,null));
			
			Assert.AreEqual(0,service.GetObjectCount(area,NWN2ObjectType.Creature,"bonfire"));
			Assert.AreEqual(0,service.GetObjectCount(area,NWN2ObjectType.Placeable,"giant"));
			Assert.AreEqual(0,service.GetObjectCount(area,NWN2ObjectType.PlacedEffect,"inn"));
			
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
			catch (FaultException e) {
				CreateService();
				System.Windows.MessageBox.Show(e.ToString() + e.Message + e.Reason);
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
			Assert.AreEqual(1,service.GetAreas().Count);
			Assert.IsNotNull(service.GetArea(area));
			Assert.AreEqual(3,service.GetObjectCount(area,NWN2ObjectType.Creature,null));
			
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
			Assert.AreEqual(1,service.GetAreas().Count);
			Assert.IsNotNull(service.GetArea(area));
			Assert.AreEqual(3,service.GetObjectCount(area,NWN2ObjectType.Creature,null));
			
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
			
			Assert.AreEqual(2,service.GetOpenAreas().Count);
			
			service.CloseArea(area2);			
			Assert.AreEqual(1,service.GetOpenAreas().Count);
			
			service.CloseArea(area1);			
			Assert.AreEqual(0,service.GetOpenAreas().Count);
			
			service.OpenArea(area1);			
			Assert.AreEqual(1,service.GetOpenAreas().Count);
			
			service.OpenArea(area2);			
			Assert.AreEqual(2,service.GetOpenAreas().Count);
						
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
			
			Assert.AreEqual(0,service.GetOpenScripts().Count);
			
			service.OpenScript(script1);			
			Assert.AreEqual(1,service.GetOpenScripts().Count);
			
			service.OpenScript(script2);			
			Assert.AreEqual(2,service.GetOpenScripts().Count);
			
			service.CloseScript(script2);			
			Assert.AreEqual(1,service.GetOpenScripts().Count);
			
			service.CloseScript(script1);			
			Assert.AreEqual(0,service.GetOpenScripts().Count);
						
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
			Assert.AreEqual(3,service.GetOpenAreas().Count);
			
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
			
			Assert.AreEqual(3,service.GetOpenAreas().Count);
						
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
			
			Assert.AreEqual("True",service.GetArea(area1)["Loaded"]);
			Assert.AreEqual("True",service.GetArea(area2)["Loaded"]);	
			
			service.CloseArea(area1);
			service.CloseArea(area2);
			
			Assert.AreEqual("False",service.GetArea(area1)["Loaded"]);
			Assert.AreEqual("False",service.GetArea(area2)["Loaded"]);
			
			service.OpenArea(area1);
						
			Assert.AreEqual("True",service.GetArea(area1)["Loaded"]);
			Assert.AreEqual("False",service.GetArea(area2)["Loaded"]);
			
			service.OpenArea(area2);
			
			Assert.AreEqual("True",service.GetArea(area1)["Loaded"]);
			Assert.AreEqual("True",service.GetArea(area2)["Loaded"]);
			
			service.CloseArea(area1);
			
			Assert.AreEqual("False",service.GetArea(area1)["Loaded"]);
			Assert.AreEqual("True",service.GetArea(area2)["Loaded"]);
			
			service.CloseArea(area2);
			
			Assert.AreEqual("False",service.GetArea(area1)["Loaded"]);
			Assert.AreEqual("False",service.GetArea(area2)["Loaded"]);
						
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
			
			Assert.AreEqual("True",service.GetUncompiledScript(script1)["Loaded"]);
			Assert.AreEqual("True",service.GetUncompiledScript(script2)["Loaded"]);
			
			service.ReleaseScript(script1);
			service.ReleaseScript(script2);
			
			Assert.AreEqual("False",service.GetUncompiledScript(script1)["Loaded"]);
			Assert.AreEqual("False",service.GetUncompiledScript(script2)["Loaded"]);
			
			service.OpenScript(script1);
			
			Assert.AreEqual("True",service.GetUncompiledScript(script1)["Loaded"]);
			Assert.AreEqual("False",service.GetUncompiledScript(script2)["Loaded"]);
			
			service.OpenScript(script2);
			
			Assert.AreEqual("True",service.GetUncompiledScript(script1)["Loaded"]);
			Assert.AreEqual("True",service.GetUncompiledScript(script2)["Loaded"]);
			
			service.CloseScript(script1);
			
			Assert.AreEqual("False",service.GetUncompiledScript(script1)["Loaded"]);
			Assert.AreEqual("True",service.GetUncompiledScript(script2)["Loaded"]);
			
			service.CloseScript(script2);
			
			Assert.AreEqual("False",service.GetUncompiledScript(script1)["Loaded"]);
			Assert.AreEqual("False",service.GetUncompiledScript(script2)["Loaded"]);
						
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
			Assert.AreEqual("True",service.GetUncompiledScript(script1)["Loaded"]);
			
			service.ReleaseScript(script1);			
			Assert.AreEqual("False",service.GetUncompiledScript(script1)["Loaded"]);
			
			service.DemandScript(script1);			
			Assert.AreEqual("True",service.GetUncompiledScript(script1)["Loaded"]);
			
			service.DemandScript(script1);			
			Assert.AreEqual("True",service.GetUncompiledScript(script1)["Loaded"]);
			
			service.ReleaseScript(script1);			
			Assert.AreEqual("True",service.GetUncompiledScript(script1)["Loaded"]);
			
			service.ReleaseScript(script1);			
			Assert.AreEqual("False",service.GetUncompiledScript(script1)["Loaded"]);
						
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
			Assert.AreEqual("True",service.GetArea(area)["Loaded"]);
			
			service.ReleaseArea(area);			
			Assert.AreEqual("False",service.GetArea(area)["Loaded"]);
			
			service.DemandArea(area);			
			Assert.AreEqual("True",service.GetArea(area)["Loaded"]);
			
			service.ReleaseArea(area);			
			Assert.AreEqual("False",service.GetArea(area)["Loaded"]);
			
			// Demand()s and Release()s 'stack up':
			service.DemandArea(area);			
			Assert.AreEqual("True",service.GetArea(area)["Loaded"]);
			service.DemandArea(area);			
			Assert.AreEqual("True",service.GetArea(area)["Loaded"]);
			service.ReleaseArea(area);	
			Assert.AreEqual("True",service.GetArea(area)["Loaded"]);	
			service.ReleaseArea(area);		
			Assert.AreEqual("False",service.GetArea(area)["Loaded"]);
						
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
