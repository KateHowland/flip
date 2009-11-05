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
		
		private string compiledTestScriptPath;
		
		private string uncompiledTestScriptPath;
		
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
				
				compiledTestScriptPath = Path.Combine(Path.GetTempPath(),"99.ncs");
				compiledTestScriptPath = pathChecker.GetUnusedFilePath(compiledTestScriptPath);
				uncompiledTestScriptPath = Path.Combine(Path.GetTempPath(),"99.nss");
				uncompiledTestScriptPath = pathChecker.GetUnusedFilePath(uncompiledTestScriptPath);
				
				System.Windows.MessageBox.Show(compiledTestScriptPath);
				System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
				resourceWriter.Write("99bottles_Compiled",assembly,compiledTestScriptPath);
				resourceWriter.Write("99bottles_Uncompiled",assembly,uncompiledTestScriptPath);
							
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
			
			if (File.Exists(compiledTestScriptPath)) File.Delete(compiledTestScriptPath);
			if (File.Exists(uncompiledTestScriptPath)) File.Delete(uncompiledTestScriptPath);
			
			Thread.Sleep(250);
		}
		
		#endregion
		
		#region Tests - Scripts
				
		[Test]
		public void GetsSerialisedInfoAboutObjects()
		{
			string name = "gets info.mod";
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
			
			service.SaveModule();
			
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
			string name = "gets object given guid.mod";
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
			service.SaveModule();
			
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
			string name = "adds uncompiled script to file module.mod";
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
			
			service.AddUncompiledScript(scriptName,scriptData);
			service.SaveModule();
			
			scripts = service.GetUncompiledScripts();
			Assert.AreEqual(1,scripts.Count);
			Bean script = scripts[0];
			
			Assert.AreEqual(scriptName,script.GetValue("Name"));
			Assert.AreEqual(scriptName + ".nss",script.GetValue("VersionControlName").ToLower());
			Assert.AreEqual(scriptData,script.GetValue("Data"));
					
			try {
				service.AddUncompiledScript(scriptName,scriptData);
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
			string name = "adds uncompiled script to file module";
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
			
			service.AddUncompiledScript(scriptName,scriptData);
			service.SaveModule();
			
			scripts = service.GetUncompiledScripts();
			Assert.AreEqual(1,scripts.Count);
			Bean script = scripts[0];
			
			Assert.AreEqual(scriptName,script.GetValue("Name"));
			Assert.AreEqual(scriptName + ".nss",script.GetValue("VersionControlName").ToLower());
			Assert.AreEqual(scriptData,script.GetValue("Data"));
					
			try {
				service.AddUncompiledScript(scriptName,scriptData);
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
			string name = "module for temp path checking.mod";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedFilePath(path);
			
			service.CreateModule(path,ModuleLocationType.File);
			service.OpenModule(path,ModuleLocationType.File);
			
			string temp = service.GetCurrentModuleTempPath();
			Assert.IsNotNull(temp);
			Assert.IsNotEmpty(temp);
			Assert.IsTrue(Directory.Exists(temp));
			string folder = Path.GetFileName(temp);
			Assert.IsTrue(folder.StartsWith("temp"));
			Assert.IsTrue(folder.EndsWith(Path.GetFileNameWithoutExtension(path)),"Was: " + folder);
			
			service.SaveModule();
			
			Assert.IsFalse(Directory.Exists(temp)); // the old temp directory should now be gone
			
			temp = service.GetCurrentModuleTempPath();
			
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
			string name = "dir module for temp path checking";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedDirectoryPath(path);
			
			service.CreateModule(path,ModuleLocationType.Directory);
			service.OpenModule(path,ModuleLocationType.Directory);
			
			Assert.AreEqual(service.GetCurrentModulePath(),service.GetCurrentModuleTempPath());
			
			service.SaveModule();
			
			Assert.AreEqual(service.GetCurrentModulePath(),service.GetCurrentModuleTempPath());
			
			service.CloseModule();
			Delete(path);
		}
		
		
		[Test]
		public void ReportsWhetherScriptHasBeenCompiled()
		{
			string name = "reports compiled script.mod";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedFilePath(path);
			
			service.CreateModule(path,ModuleLocationType.File);
			service.OpenModule(path,ModuleLocationType.File);	
						
			// First check for a pre-compiled script, so we're not relying on CompileScript() being implemented.
			string precompiledScriptName = "99bottles";
			string filename = precompiledScriptName + ".NCS";
			string resourcesPath = @"C:\Libraries\Flip unit test resources";
			string precompiledScriptPath = Path.Combine(resourcesPath,filename);
			Assert.IsTrue(File.Exists(precompiledScriptPath),"A file necessary for running the unit test was missing.");
			
			string tempPath = service.GetCurrentModuleTempPath();
			Assert.IsTrue(Directory.Exists(tempPath));
						
			Assert.IsFalse(service.HasCompiled(precompiledScriptName));
			Assert.IsFalse(service.HasUncompiled(precompiledScriptName));
			
			string copiedPath = Path.Combine(tempPath,filename);
			Assert.IsFalse(File.Exists(copiedPath));
			File.Copy(precompiledScriptPath,copiedPath);
			Assert.IsTrue(File.Exists(copiedPath));
						
			Assert.IsTrue(service.HasCompiled(precompiledScriptName));
			Assert.IsFalse(service.HasUncompiled(precompiledScriptName));
			
			// Then add a new uncompiled script and compile it.
			string scriptData = sampleScripts.Sing;
			string scriptName = "uncompiled";
			
			Assert.IsFalse(service.HasCompiled(scriptName));
			Assert.IsFalse(service.HasUncompiled(scriptName));
			
			service.AddUncompiledScript(scriptName,scriptData);
			service.SaveModule();
			
			Assert.IsFalse(service.HasCompiled(scriptName));
			Assert.IsTrue(service.HasUncompiled(scriptName));
			
			service.CompileScript(scriptName);		
			service.SaveModule();
			
			Assert.IsTrue(service.HasCompiled(scriptName));
			Assert.IsTrue(service.HasUncompiled(scriptName));
			
			service.CloseModule();
			Delete(path);
		}
		
		
		[Test]
		public void CompilesScriptForFileModule()
		{
			string name = "compiles script file.mod";
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
			
			service.AddUncompiledScript(scriptName,scriptData);
			service.SaveModule();			
			
			Assert.IsFalse(service.HasCompiled(scriptName));
			Assert.IsTrue(service.HasUncompiled(scriptName));
			Assert.AreEqual(1,service.GetUncompiledScripts().Count);
			
			service.CompileScript(scriptName);
			service.SaveModule();
									
			Assert.IsTrue(service.HasCompiled(scriptName));
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
			string name = "compiles script dir";
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
			
			service.AddUncompiledScript(scriptName,scriptData);
			service.SaveModule();			
			
			Assert.IsFalse(service.HasCompiled(scriptName));
			Assert.IsTrue(service.HasUncompiled(scriptName));
			Assert.AreEqual(1,service.GetUncompiledScripts().Count);
			
			service.CompileScript(scriptName);
			service.SaveModule();
									
			Assert.IsTrue(service.HasCompiled(scriptName));
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
			string name = "refuses to compile illegal script.mod";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedFilePath(path);
			
			service.CreateModule(path,ModuleLocationType.File);
			service.OpenModule(path,ModuleLocationType.File);			
			
			string scriptData = sampleScripts.BrokenScript;
			string scriptName = "illegalscript";
						
			try {
				service.AddUncompiledScript(scriptName,scriptData);
				service.SaveModule();
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
			string name = "attaches script.mod";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedFilePath(path);
			
			service.CreateModule(path,ModuleLocationType.File);
			service.OpenModule(path,ModuleLocationType.File);
					
			string area = "area";
			service.AddArea(area,true,AreaBase.SmallestAreaSize);
			
			service.SaveModule();	
						
			for (int i = 0; i < 20; i++) {
				service.AddObject(area,NWN2ObjectType.Creature,"c_cat","cat" + i);
			}			
			service.SaveModule();			
			
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
			
			service.AddUncompiledScript(scriptName,scriptData);
			service.SaveModule();			
			service.CompileScript(scriptName);
			service.SaveModule();
			Assert.IsTrue(service.HasCompiled(scriptName));
			
			service.AttachScriptToObject(scriptName,area,Nwn2EventRaiser.Creature,catID,"OnSpawnIn");	
			service.SaveModule();
						
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
			service.SaveModule();
						
			string scriptName = "givegold";
			string scriptData = sampleScripts.GiveGold;
			service.AddUncompiledScript(scriptName,scriptData);
			service.SaveModule();
			service.CompileScript(scriptName);
			service.SaveModule();
			
			string scriptSlot = "OnHeartbeat";			
			service.AttachScriptToArea(scriptName,areaName,scriptSlot);
			service.SaveModule();
			
			Bean areaBean = service.GetArea(areaName);
			Assert.IsTrue(areaBean.HasValue(scriptSlot));
			Assert.AreEqual(scriptName,areaBean.GetValue(scriptSlot));
			
			service.CloseModule();
			service.OpenModule(path,ModuleLocationType.File);
			
			areaBean = service.GetArea(areaName);
			Assert.IsTrue(areaBean.HasValue(scriptSlot));
			Assert.AreEqual(scriptName,areaBean.GetValue(scriptSlot));
		
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
			service.AddUncompiledScript(scriptName,scriptData);
			service.SaveModule();
			service.CompileScript(scriptName);
			service.SaveModule();
			
			string scriptSlot = "OnPlayerLevelUp";			
			service.AttachScriptToModule(scriptName,scriptSlot);
			service.SaveModule();
			
			Bean moduleBean = service.GetModule();			
			Assert.IsTrue(moduleBean.HasValue(scriptSlot));
			Assert.AreEqual(scriptName,moduleBean.GetValue(scriptSlot));
			
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
			service.SaveModule();	
			service.AddObject(area,NWN2ObjectType.Creature,"c_cat","cat");
			service.SaveModule();		
			
			Bean cat = service.GetObjects(area,NWN2ObjectType.Creature,"cat")[0];						
			Guid catID = new Guid(cat.GetValue("ObjectID"));
			
			string scriptData = sampleScripts.Sing;
			string scriptName = "attachingscript";
			
			service.AddUncompiledScript(scriptName,scriptData);
			service.SaveModule();			
			service.CompileScript(scriptName);
			service.SaveModule();
			Assert.IsTrue(service.HasCompiled(scriptName));
			
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
			service.SaveModule();
						
			string scriptName = "givegold";
			string scriptData = sampleScripts.GiveGold;
			service.AddUncompiledScript(scriptName,scriptData);
			service.SaveModule();
			service.CompileScript(scriptName);
			service.SaveModule();
			
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
			service.SaveModule();
			
			string area2 = "inside";
			service.AddArea(area2,false,AreaBase.SmallestAreaSize);	
			service.SaveModule();
			
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
			path = pathChecker.GetUnusedFilePath(path);
			
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
			string name = "refuses to attach uncompiled script.mod";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedFilePath(path);
			
			service.CreateModule(path,ModuleLocationType.File);
			service.OpenModule(path,ModuleLocationType.File);
					
			string area = "area";
			service.AddArea(area,true,AreaBase.SmallestAreaSize);			
			service.SaveModule();	
						
			service.AddObject(area,NWN2ObjectType.Creature,"c_cat","cat");
			service.SaveModule();		
			
			Bean cat = service.GetObjects(area,NWN2ObjectType.Creature,"cat")[0];
			Guid catID = new Guid(cat.GetValue("ObjectID"));
			
			string scriptData = sampleScripts.Sing;
			string scriptName = "uncompiled script";
			
			service.AddUncompiledScript(scriptName,scriptData);
			service.SaveModule();
			
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
			string name = "refuses to attach uncompiled script to area.mod";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedFilePath(path);
			
			service.CreateModule(path,ModuleLocationType.File);
			service.OpenModule(path,ModuleLocationType.File);
					
			string area = "area";
			service.AddArea(area,true,AreaBase.SmallestAreaSize);			
			service.SaveModule();	
			
			string scriptData = sampleScripts.Sing;
			string scriptName = "uncompiled script";
			
			service.AddUncompiledScript(scriptName,scriptData);
			service.SaveModule();
			
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
		public void AddsPreCompiledScriptToFileModule()
		{
			string name = "adds precompiled script.mod";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedFilePath(path);
			
			service.CreateModule(path,ModuleLocationType.File);
			service.OpenModule(path,ModuleLocationType.File);
			
			IList<Bean> scripts = service.GetUncompiledScripts();
			Assert.AreEqual(0,scripts.Count);
			
			string filename = "99bottles.NCS";
			string resourcesPath = @"C:\Libraries\Flip unit test resources";
			string compiledScriptPath = Path.Combine(resourcesPath,filename);
			Assert.IsTrue(File.Exists(compiledScriptPath));
			
			string scriptName = Path.GetFileNameWithoutExtension(filename);
			
			Assert.IsFalse(service.HasCompiled(scriptName));
			Assert.IsFalse(service.HasUncompiled(scriptName));
			
			service.AddCompiledScript(compiledScriptPath);
			service.SaveModule();
			
			Assert.IsTrue(service.HasCompiled(scriptName));
			Assert.IsFalse(service.HasUncompiled(scriptName));
			
			scripts = service.GetUncompiledScripts();
			Assert.AreEqual(0,service.GetUncompiledScripts().Count);
			
			// Try to add missing files, nonsense files and uncompiled scripts:
			string nonsensePath = Path.Combine(resourcesPath,"idonotexist.NCS");
			Assert.IsFalse(File.Exists(nonsensePath));
			try {
				service.AddCompiledScript(nonsensePath);
				Assert.Fail("Didn't raise a FaultException<IOException> when asked to add " +
			            	"a non-existent script to the module.");
			}
			catch (FaultException<IOException>) {
				// expected result
			}
			catch (FaultException) {
				CreateService();
				Assert.Fail("Didn't raise a FaultException<IOException> when asked to add " +
			            	"a non-existent script to the module.");
			}
			
			string uncompiledScriptPath = Path.Combine(resourcesPath,"99bottles.NSS");
			Assert.IsTrue(File.Exists(uncompiledScriptPath));
			try {
				service.AddCompiledScript(uncompiledScriptPath);
				Assert.Fail("Didn't raise a FaultException<IOException> when asked to add " +
			            	"an uncompiled script with AddCompiledScript.");
			}
			catch (FaultException<IOException>) {
				// expected result
			}
			catch (FaultException) {
				CreateService();
				Assert.Fail("Didn't raise a FaultException<IOException> when asked to add " +
			            	"an uncompiled script with AddCompiledScript.");
			}
			
			string wrongFileTypePath = Path.Combine(resourcesPath,"New Text Document.txt");
			Assert.IsTrue(File.Exists(wrongFileTypePath));
			try {
				service.AddCompiledScript(wrongFileTypePath);
				Assert.Fail("Didn't raise a FaultException<IOException> when asked to add " +
			            	"a file of inappropriate type to the module.");
			}
			catch (FaultException<IOException>) {
				// expected result
			}
			catch (FaultException) {
				CreateService();
				Assert.Fail("Didn't raise a FaultException<IOException> when asked to add " +
			            	"a file of inappropriate type to the module.");
			}
				
			service.CloseModule();
			Delete(path);
		}
		
		
		[Test]
		public void AddsPreCompiledScriptToDirectoryModule()
		{
			string name = "adds precompiled script to directory";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedDirectoryPath(path);
			
			service.CreateModule(path,ModuleLocationType.Directory);
			service.OpenModule(path,ModuleLocationType.Directory);
			
			IList<Bean> scripts = service.GetUncompiledScripts();
			Assert.AreEqual(0,scripts.Count);
			
			string filename = "99bottles.NCS";
			string resourcesPath = @"C:\Libraries\Flip unit test resources";
			string compiledScriptPath = Path.Combine(resourcesPath,filename);
			Assert.IsTrue(File.Exists(compiledScriptPath));
			
			string scriptName = Path.GetFileNameWithoutExtension(filename);
			
			Assert.IsFalse(service.HasCompiled(scriptName));
			Assert.IsFalse(service.HasUncompiled(scriptName));
			
			service.AddCompiledScript(compiledScriptPath);
			service.SaveModule();
			
			Assert.IsTrue(service.HasCompiled(scriptName));
			Assert.IsFalse(service.HasUncompiled(scriptName));
			
			scripts = service.GetUncompiledScripts();
			Assert.AreEqual(0,service.GetUncompiledScripts().Count);
			
			// Try to add missing files, nonsense files and uncompiled scripts:
			string nonsensePath = Path.Combine(resourcesPath,"idonotexist.NCS");
			Assert.IsFalse(File.Exists(nonsensePath));
			try {
				service.AddCompiledScript(nonsensePath);
				Assert.Fail("Didn't raise a FaultException<IOException> when asked to add " +
			            	"a non-existent script to the module.");
			}
			catch (FaultException<IOException>) {
				// expected result
			}
			catch (FaultException) {
				CreateService();
				Assert.Fail("Didn't raise a FaultException<IOException> when asked to add " +
			            	"a non-existent script to the module.");
			}
			
			string uncompiledScriptPath = Path.Combine(resourcesPath,"99bottles.NSS");
			Assert.IsTrue(File.Exists(uncompiledScriptPath));
			try {
				service.AddCompiledScript(uncompiledScriptPath);
				Assert.Fail("Didn't raise a FaultException<IOException> when asked to add " +
			            	"an uncompiled script with AddCompiledScript.");
			}
			catch (FaultException<IOException>) {
				// expected result
			}
			catch (FaultException) {
				CreateService();
				Assert.Fail("Didn't raise a FaultException<IOException> when asked to add " +
			            	"an uncompiled script with AddCompiledScript.");
			}
			
			string wrongFileTypePath = Path.Combine(resourcesPath,"New Text Document.txt");
			Assert.IsTrue(File.Exists(wrongFileTypePath));
			try {
				service.AddCompiledScript(wrongFileTypePath);
				Assert.Fail("Didn't raise a FaultException<IOException> when asked to add " +
			            	"a file of inappropriate type to the module.");
			}
			catch (FaultException<IOException>) {
				// expected result
			}
			catch (FaultException) {
				CreateService();
				Assert.Fail("Didn't raise a FaultException<IOException> when asked to add " +
			            	"a file of inappropriate type to the module.");
			}
				
			service.CloseModule();
			Delete(path);
		}		
		
		
		[Test]
		public void ScriptsPersistInFileModule()
		{
			string name = "script persists in file module.mod";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedFilePath(path);
			
			service.CreateModule(path,ModuleLocationType.File);
			service.OpenModule(path,ModuleLocationType.File);
					
			string area = "area";
			service.AddArea(area,true,AreaBase.SmallestAreaSize);
			
			service.SaveModule();	
			
			service.AddObject(area,NWN2ObjectType.Creature,"c_cat","cat");
			
			service.SaveModule();	
			
			IList<Bean> cats = service.GetObjects(area,NWN2ObjectType.Creature,"cat");
			Assert.AreEqual(1,cats.Count);
			Bean cat = cats[0];
			
			Guid catID = new Guid(cat.GetValue("ObjectID"));
						
			string scriptData = sampleScripts.Sing;
			string scriptName = "attachingscript";
			
			service.AddUncompiledScript(scriptName,scriptData);
			service.SaveModule();			
			service.CompileScript(scriptName);
			service.SaveModule();			
			service.AttachScriptToObject(scriptName,area,Nwn2EventRaiser.Creature,catID,"OnSpawnIn");	
			service.SaveModule();
			
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
			
			service.CloseModule();			
			Delete(path);
		}
		
				
		[Test]
		public void ScriptsPersistInDirectoryModule()
		{
			string name = "script persists in directory module";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedDirectoryPath(path);
			
			service.CreateModule(path,ModuleLocationType.Directory);
			service.OpenModule(path,ModuleLocationType.Directory);
					
			string area = "area";
			service.AddArea(area,true,AreaBase.SmallestAreaSize);
			
			service.SaveModule();	
			
			service.AddObject(area,NWN2ObjectType.Creature,"c_cat","cat");
			
			service.SaveModule();	
			
			IList<Bean> cats = service.GetObjects(area,NWN2ObjectType.Creature,"cat");
			Assert.AreEqual(1,cats.Count);
			Bean cat = cats[0];
			
			Guid catID = new Guid(cat.GetValue("ObjectID"));
						
			string scriptData = sampleScripts.Sing;
			string scriptName = "attachingscript";
			
			service.AddUncompiledScript(scriptName,scriptData);
			service.SaveModule();			
			service.CompileScript(scriptName);
			service.SaveModule();			
			service.AttachScriptToObject(scriptName,area,Nwn2EventRaiser.Creature,catID,"OnSpawnIn");	
			service.SaveModule();
			
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
			
			service.CloseModule();			
			Delete(path);
		}
		
		
		[Test]
		public void ReturnsDataAboutScripts()
		{
			string name = "returns data about scripts";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedDirectoryPath(path);
			
			service.CreateModule(path,ModuleLocationType.Directory);
			service.OpenModule(path,ModuleLocationType.Directory);
										
			string givegold = "givegold";
			string givegoldData = sampleScripts.GiveGold;
			string changename = "changename";
			string changenameData = sampleScripts.ChangeName;
			string _99bottles = "99bottles";
			
			// End up with 2 uncompiled scripts (givegold, changename) and 2 compiled scripts (givegold, 99bottles):
			service.AddUncompiledScript(givegold,givegoldData);
			service.SaveModule();			
			service.CompileScript(givegold);
			service.SaveModule();
			service.AddUncompiledScript(changename,changenameData);
			service.SaveModule();
			string filename = _99bottles + ".NCS";
			string resourcesPath = @"C:\Libraries\Flip unit test resources";
			string compiledScriptPath = Path.Combine(resourcesPath,filename);
			service.AddCompiledScript(compiledScriptPath);
			service.SaveModule();	
			
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
			string name = "dir module";
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
			string name = "file module.mod";
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
			string name = "dir module.mod";
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
			string name = "file module";
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
			string name = "file duplicate.mod";
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
			string name = "dir duplicate";
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
			string name = "dir module";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedDirectoryPath(path);
			name = Path.GetFileNameWithoutExtension(path);
					
			service.CreateModule(path,ModuleLocationType.Directory);
									
			service.OpenModule(path,ModuleLocationType.Directory);	
			
			Assert.AreEqual(name,service.GetCurrentModuleName());	
			
			service.CloseModule();			
			Delete(path);
		}
		
		
		[Test]
		public void OpensFileModule()
		{
			string name = "file module.mod";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedFilePath(path);
			name = Path.GetFileNameWithoutExtension(path);
		
			service.CreateModule(path,ModuleLocationType.File);
			
			service.OpenModule(path,ModuleLocationType.File);	
			
			Assert.AreEqual(name,service.GetCurrentModuleName());
			
			service.CloseModule();			
			Delete(path);
		}
		
		
		[Test]
		public void OpensFileModuleFromOutsideModulesDirectory()
		{
			string name = "file module from outside path.mod";
			string parent = @"N:\WindowsProfile\Desktop";
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedFilePath(path);
			name = Path.GetFileNameWithoutExtension(path);
		
			service.CreateModule(path,ModuleLocationType.File);
			
			service.OpenModule(path,ModuleLocationType.File);	
			
			Assert.AreEqual(name,service.GetCurrentModuleName());
			
			service.CloseModule();			
			Delete(path);
		}
		
		
		[Test]
		public void CloseModule()
		{
			string name = "test module.mod";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedFilePath(path);
			name = Path.GetFileNameWithoutExtension(path);
					
			service.CreateModule(path,ModuleLocationType.File);
			
			service.OpenModule(path,ModuleLocationType.File);
			
			string moduleName, modulePath;
			moduleName = service.GetCurrentModuleName();
			modulePath = service.GetCurrentModulePath();		
			Assert.IsNotNull(moduleName);
			Assert.IsNotNull(modulePath);
			Assert.AreEqual(name,moduleName);
			Assert.AreEqual(path,modulePath);
			
			service.CloseModule();
			
			moduleName = service.GetCurrentModuleName();
			modulePath = service.GetCurrentModulePath();				
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
			string name = "file returns correct path.mod";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedFilePath(path);
			name = Path.GetFileNameWithoutExtension(path);
			
			service.GetCurrentModuleName();
					
			service.CreateModule(path,ModuleLocationType.File);
			service.OpenModule(path, ModuleLocationType.File);
			
			Assert.AreEqual(path,service.GetCurrentModulePath());
							
			service.CloseModule();
			
			Delete(path);
			
			name = "dir returns correct path";
			path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedDirectoryPath(path);
			name = Path.GetFileName(path);
					
			service.CreateModule(path,ModuleLocationType.Directory);
			service.OpenModule(path, ModuleLocationType.Directory);
			
			Assert.AreEqual(path,service.GetCurrentModulePath());
			
			service.CloseModule();			
			Delete(path);
		}
		
				
		[Test]
		public void AddsAreaToFileModule()
		{
			string name = "area adding file.mod";
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
			
			// When adding to a file module, we DO need to save the module for the area to persist:
			service.SaveModule();
			
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
			string name = "area adding directory";
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
			
			// When adding to a directory module, we DON'T need to save the module for the area to persist.
			
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
			string name = "area duplication.mod";
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
			string name = "object adding.mod";
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
			
			service.SaveModule();
			
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
			string name = "unknown resref object adding.mod";
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
			string name = "saves directory module";
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
						
			Assert.AreEqual(service.GetCurrentModuleName(),Path.GetFileNameWithoutExtension(path));
			Assert.AreEqual(1,service.GetAreas().Count);
			Assert.IsNotNull(service.GetArea(area));
			Assert.AreEqual(3,service.GetObjectCount(area,NWN2ObjectType.Creature,null));
			
			service.CloseModule();			
			Delete(path);
		}
		
		
		[Test]
		public void SavesFileModule()
		{
			string name = "saves file module.mod";
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
						
			Assert.AreEqual(service.GetCurrentModuleName(),Path.GetFileNameWithoutExtension(path));
			Assert.AreEqual(1,service.GetAreas().Count);
			Assert.IsNotNull(service.GetArea(area));
			Assert.AreEqual(3,service.GetObjectCount(area,NWN2ObjectType.Creature,null));
			
			service.CloseModule();			
			Delete(path);
		}
		
		
		[Test]
		public void RefusesToOpenNonExistentDirectoryModule()
		{
			string name = "dir module that does not exist";
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
			string name = "file module that does not exist.mod";
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
		
		#endregion
	}
}
