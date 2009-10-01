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
using System.IO;
using System.ServiceModel;
using System.Threading;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Sussex.Flip.Games.NeverwinterNightsTwo.Utils;
using Sussex.Flip.Utils;
using NWN2Toolset;
using NWN2Toolset.NWN2.IO;

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
		
		#endregion
		
		#region Setup
		
		[TestFixtureSetUp]
		public void Init()
		{					
			pathChecker = new PathChecker();
			
//			try {
//			Nwn2ToolsetFunctions.RunNeverwinterNightsTwoToolset();
			
			pipeChannelFactory = new ChannelFactory<INwn2Service>(new NetNamedPipeBinding(),"net.pipe://localhost/NamedPipeEndpoint");
//			
//			int waited = 0;
//			while (true) {
//				try {
//					service.GetCurrentModuleName();
//					System.Windows.MessageBox.Show("Finished loading.");
//					break;
//				}
//				catch (EndpointNotFoundException) {
//					Thread.Sleep(500);					
//					waited += 500;
//					service = pipeChannelFactory.CreateChannel();
//				}
//			}
//			
//			int seconds = waited < 1000 ? 0 : waited/1000;
//			
//			System.Windows.MessageBox.Show("Waited about " + seconds + " seconds.");
//			} catch (Exception e) {
//				System.Windows.MessageBox.Show(e.ToString());
//			}
		}
		
		
		[TestFixtureTearDown]
		public void Dispose()
		{
//			Nwn2ToolsetFunctions.KillNeverwinterNightsTwoToolset();
		}
		
		#endregion
		
		#region Tests
				
		[Test]
		public void CreatesDirectoryModule()
		{			
			INwn2Service service = GetService();
			
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
			
			foreach (FileInfo file in dir.GetFiles()) {
				file.Delete();
			}
			dir.Delete();
		}
		
		
		[Test]
		public void CreatesFileModule()
		{
			INwn2Service service = GetService();
			
			string name = "file module.mod";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedFilePath(path);
			
			service.CreateModule(path,ModuleLocationType.File);
			
			Assert.IsTrue(File.Exists(path),"Module file was not created.");
			
			File.Delete(path);
		}
		
		
		[Test]
		public void RefusesToCreateDirectoryModuleAtFilePath()
		{
			INwn2Service service = GetService();
			
			string name = "dir module.mod";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			string expectedException = "System.ArgumentException";
			
			path = pathChecker.GetUnusedFilePath(path);
					
			try {
				service.CreateModule(path,ModuleLocationType.Directory);
				Assert.Fail("Failed to raise " + expectedException + " when asked to " +
				            "create a directory module at a file path.");
			}
			catch (FaultException e) {
				if (!(e.ToString().Contains(expectedException))) {
					Assert.Fail("Failed to raise " + expectedException + " when asked to " +
					            "create a directory module at a file path.");
				}
				Assert.IsFalse(Directory.Exists(path),"Created a directory module given a file path.");
			}
		}
				
		
		[Test]
		public void RefusesToCreateFileModuleAtDirectoryPath()
		{
			INwn2Service service = GetService();
			
			string name = "file module";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			string expectedException = "System.ArgumentException";
			
			path = pathChecker.GetUnusedDirectoryPath(path);
					
			try {
				service.CreateModule(path,ModuleLocationType.File);
				Assert.Fail("Failed to raise " + expectedException + " when asked to " +
				            "create a file module at a directory path.");
			}
			catch (FaultException e) {
				if (!(e.ToString().Contains(expectedException))) {
					Assert.Fail("Failed to raise " + expectedException + " when asked to " +
					            "create a file module at a directory path.");
				}
				Assert.IsFalse(File.Exists(path),"Created a file module given a directory path.");
			}
		}
		
		
		[Test]
		public void OpensDirectoryModule()
		{
			INwn2Service service = GetService();
			
			string name = "dir module";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedDirectoryPath(path);
			name = Path.GetFileNameWithoutExtension(path);
					
			service.CreateModule(path,ModuleLocationType.Directory);
			
			service.OpenModule(path,ModuleLocationType.Directory);	
			
			Assert.AreEqual(name,service.GetCurrentModuleName());	
			
			service.CloseModule();
			
			DirectoryInfo dir = new DirectoryInfo(path);
			foreach (FileInfo file in dir.GetFiles()) {
				file.Delete();
			}
			dir.Delete();		
		}
		
		
		[Test]
		public void OpensFileModule()
		{
			INwn2Service service = GetService();
			
			string name = "file module.mod";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedFilePath(path);
			name = Path.GetFileNameWithoutExtension(path);
		
			service.CreateModule(path,ModuleLocationType.File);
			
			service.OpenModule(path,ModuleLocationType.File);	
			
			Assert.AreEqual(name,service.GetCurrentModuleName());
			
			service.CloseModule();
			
			File.Delete(path);
		}
		
		
		[Test]
		public void CloseModule()
		{
			INwn2Service service = GetService();
			
			string name = "test module.mod";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedFilePath(path);
			name = Path.GetFileNameWithoutExtension(path);
					
			service.CreateModule(path,ModuleLocationType.File);
			
			Assert.AreNotEqual(name,service.GetCurrentModuleName());
			Assert.AreNotEqual(path,service.GetCurrentModulePath());
			
			service.OpenModule(path, ModuleLocationType.File);
			
			Assert.AreEqual(name,service.GetCurrentModuleName());
			Assert.AreEqual(path,service.GetCurrentModulePath());
			
			service.CloseModule();
			
			Assert.AreNotEqual(name,service.GetCurrentModuleName());
			Assert.AreNotEqual(path,service.GetCurrentModulePath());
		}
		
		
		[Test]
		public void ReturnsCorrectModulePath()
		{
			INwn2Service service = GetService();			
			
			string name = "file module.mod";
			string parent = NWN2ToolsetMainForm.ModulesDirectory;
			string path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedFilePath(path);
			name = Path.GetFileNameWithoutExtension(path);
					
			service.CreateModule(path,ModuleLocationType.File);
			service.OpenModule(path, ModuleLocationType.File);
			
			Assert.AreEqual(path,service.GetCurrentModulePath());
							
			
			name = "dir module";
			path = Path.Combine(parent,name);
			
			path = pathChecker.GetUnusedDirectoryPath(path);
			name = Path.GetFileName(path);
					
			service.CreateModule(path,ModuleLocationType.Directory);
			service.OpenModule(path, ModuleLocationType.Directory);
			
			Assert.AreEqual(path,service.GetCurrentModulePath());
		}
		
				
//		
//		
//		[Test]
//		public void SavesDirectoryModule()
//		{
//			Assert.Fail();
//		}
//		
//		
//		[Test]
//		public void SavesFiledModule()
//		{
//			Assert.Fail();
//		}
//		
//		
//		[Test]
//		public void DoesNotCreateModuleIfNameIsAlreadyTaken()
//		{
//			Assert.Fail();
//		}
//		
//		
//		[Test]
//		public void DoesNotCreateModuleIfNameIsInvalid()
//		{
//			Assert.Fail();
//		}
//		
//		
//		[Test]
//		public void AddsAreaToModule()
//		{
//			Assert.Fail();
//		}
//		
//		
//		[Test]
//		public void DoesNotAddAreaIfNameIsAlreadyTaken()
//		{
//			Assert.Fail();
//		}
//		
//		
//		[Test]
//		public void AddsObjectToArea()
//		{
//			Assert.Fail();
//		}
		
		#endregion
		
		#region Methods
		
		/// <summary>
		/// Creates and returns a proxy for the service.
		/// </summary>
		private INwn2Service GetService()
		{
			return pipeChannelFactory.CreateChannel();
		}
		
		#endregion
	}
}
