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
using System.ServiceModel;

namespace Sussex.Flip.Games.NeverwinterNightsTwo.Plugin.Tests
{
	/// <summary>
	/// Tests the <see cref="FlipPlugin"/> class.
	/// </summary>
	/// <remarks>By contrast with the tests for class Nwn2Session,
	/// this tests the service methods of that class, as accessed
	/// by a client via a service contract.</remarks>
	[TestFixture]
	public class FlipPluginTests
	{
		#region Fields
		
		protected INwn2Session nwn2session;
		
		#endregion
		
		#region Setup
		
		[TestFixtureSetUp]
		public void Init()
		{			
			ChannelFactory<INwn2Session> pipeChannelFactory = new ChannelFactory<INwn2Session>(new NetNamedPipeBinding(),
			                                                                                   "net.pipe://localhost/NamedPipeEndpoint");
			
			nwn2session = pipeChannelFactory.CreateChannel();
		}
		
		
		[TestFixtureTearDown]
		public void Dispose()
		{
		}
		
		#endregion
		
		#region Tests
		
		[Test]
		public void ReturnsNameOfCurrentModule()
		{			
			nwn2session.OpenDirectoryBasedModule("rar");
			
			string name = nwn2session.GetCurrentModuleName();
			
			Assert.IsNotNull(name);
			Assert.AreEqual(name,"rar");			
		}
		
		#endregion
	}
}
