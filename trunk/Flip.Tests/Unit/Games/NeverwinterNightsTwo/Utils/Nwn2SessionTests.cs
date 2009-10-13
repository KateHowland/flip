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
using NWN2Toolset.NWN2.Data.Templates;

namespace Sussex.Flip.Games.NeverwinterNightsTwo.Utils.Tests
{	
	/// <summary>
	/// Tests the <see cref="Nwn2Session"/> class.
	/// </summary>
	[TestFixture]
	public sealed class Nwn2SessionTests
	{
		#region Fields
		
		private ChannelFactory<INwn2Service> pipeChannelFactory;
		
		private PathChecker pathChecker;
		
		private INwn2Service service;
		
		#endregion
		
		#region Setup
				
		[TestFixtureSetUp]
		public void Init()
		{					
			// Don't try to delete modules files at the start of the test fixture
			// as it causes IO access exceptions with the NWN2 toolset.
			
			pathChecker = new PathChecker();				
			
			Nwn2ToolsetFunctions.RunNeverwinterNightsTwoToolset();				
						
			Console.WriteLine("Waiting for toolset to load...");
			
			AutomationElement toolset = null;
			
			while (toolset == null) {
				foreach (AutomationElement e in AutomationElement.RootElement.FindAll(TreeScope.Children,Condition.TrueCondition)) {
					string name = (string)e.GetCurrentPropertyValue(AutomationElement.NameProperty);
					if (name.Contains("Obsidian Neverwinter Nights 2 Toolset:")) {
						toolset = e;
						break;
					}
				}
				Thread.Sleep(500);
			} 
			
			((WindowPattern)toolset.GetCurrentPattern(WindowPattern.Pattern)).SetWindowVisualState(WindowVisualState.Minimized);
					
			pipeChannelFactory = new ChannelFactory<INwn2Service>(new NetNamedPipeBinding(),"net.pipe://localhost/NamedPipeEndpoint");									
			CreateService();
			
			Console.WriteLine("Toolset loaded. Running test suite.");
		}
		
		
		[TestFixtureTearDown]
		public void Dispose()
		{
			Console.WriteLine("Test suite completed. Closing toolset.");
			Nwn2ToolsetFunctions.KillNeverwinterNightsTwoToolset();
			Thread.Sleep(250);
		}
		
		#endregion
		
		#region Tests
				
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
			
			ICollection<string> areas = service.GetAreas();
			Assert.IsNotNull(areas);
			Assert.AreEqual(2,areas.Count);
			Assert.IsTrue(areas.Contains(area1));
			Assert.IsTrue(areas.Contains(area2));
			
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
			
			ICollection<string> areas = service.GetAreas();
			Assert.IsNotNull(areas);
			Assert.AreEqual(2,areas.Count);
			Assert.IsTrue(areas.Contains(area1));
			Assert.IsTrue(areas.Contains(area2));
			
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
			Assert.IsTrue(service.GetAreas().Contains(area));
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
			Assert.IsTrue(service.GetAreas().Contains(area));
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
