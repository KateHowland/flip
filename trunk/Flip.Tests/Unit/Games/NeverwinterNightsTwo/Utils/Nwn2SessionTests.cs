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
using NWN2Toolset;

namespace Sussex.Flip.Games.NeverwinterNightsTwo.Utils.Tests
{	
	/// <summary>
	/// Tests the <see cref="Nwn2Session"/> class.
	/// </summary>
	[TestFixture]
	public class Nwn2SessionTests
	{
		#region Fields
		
		protected INwn2Service service;
		
		#endregion
		
		#region Setup
		
		[TestFixtureSetUp]
		public void Init()
		{			
//			try {
//			Nwn2ToolsetFunctions.RunNeverwinterNightsTwoToolset();
			
			ChannelFactory<INwn2Service> pipeChannelFactory = new ChannelFactory<INwn2Service>(new NetNamedPipeBinding(),
			                                                                                   "net.pipe://localhost/NamedPipeEndpoint");
			
			service = pipeChannelFactory.CreateChannel();
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
		public void CreatesDirectoryBasedModule()
		{			
			
			// Call some method to create a module of location directory and name X
			// Wait up to 10 seconds
			// Assert that folder exists
			// OEIUnserialize the folder and Assert that you get a valid NWN2GameModule object with properties
			Assert.Fail();
		}
		
		
		[Test]
		public void CreatesFileBasedModule()
		{
			// Call some method to create a module of location file and name X at some nonModules directory path
			// Wait up to 10 seconds
			// Assert that the file exists
			// OEIUnserialize the file and Assert that you get a valid NWN2GameModule object with properties
			Assert.Fail();
		}
		
		
		[Test]
		public void OpensDirectoryBasedModule()
		{
			Assert.Fail();
		}
		
		
		[Test]
		public void OpensFileBasedModule()
		{
			Assert.Fail();
		}
		
		
		[Test]
		public void SavesDirectoryBasedModule()
		{
			Assert.Fail();
		}
		
		
		[Test]
		public void SavesFileBasedModule()
		{
			Assert.Fail();
		}
		
		
		[Test]
		public void DoesNotCreateModuleIfNameIsAlreadyTaken()
		{
			Assert.Fail();
		}
		
		
		[Test]
		public void DoesNotCreateModuleIfNameIsInvalid()
		{
			Assert.Fail();
		}
		
		
		[Test]
		public void AddsAreaToModule()
		{
			Assert.Fail();
		}
		
		
		[Test]
		public void DoesNotAddAreaIfNameIsAlreadyTaken()
		{
			Assert.Fail();
		}
		
		
		[Test]
		public void AddsObjectToArea()
		{
			Assert.Fail();
		}
		
		#endregion
	}
}
