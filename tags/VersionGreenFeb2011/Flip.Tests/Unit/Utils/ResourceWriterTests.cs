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
 * This file added by Keiron Nicholson on 05/11/2009 at 10:14.
 */


using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Sussex.Flip.Utils;

namespace Sussex.Flip.Utils.Tests
{
	[TestFixture]
	public class ResourceWriterTests
	{
		#region Fields
		
		ResourceWriter rw;
		PathChecker pathChecker;		
		
		#endregion
		
		
		[TestFixtureSetUp]
		public void Init()
		{
			rw = new ResourceWriter();
			pathChecker = new PathChecker();			
		}
		
		
		[Test]
		public void WritesResourceToDisk()
		{
			string resourceName = "licence";
			Assembly assembly = Assembly.GetExecutingAssembly();
			string[] resources = assembly.GetManifestResourceNames();
			Assert.IsFalse(resources.Length==0);
			Assert.IsTrue(new List<string>(resources).Contains(resourceName));
			
			string output = Path.Combine(Path.GetTempPath(),"licence.txt");
			output = pathChecker.GetUnusedFilePath(output);
			
			Assert.IsFalse(File.Exists(output));
			rw.Write(resourceName,assembly,output);
			Assert.IsTrue(File.Exists(output));
			
			string contents = File.ReadAllText(output);
			Assert.IsTrue(contents.StartsWith(" GNU GENERAL PUBLIC LICENSE"));
			Assert.IsTrue(contents.EndsWith("<http://www.gnu.org/philosophy/why-not-lgpl.html>."));
			
			File.Delete(output);
		}
		
		
		[Test]
		public void RefusesToWriteUnknownResource()
		{
			string resourceName = "nonexistent";
			Assembly assembly = Assembly.GetExecutingAssembly();
			string[] resources = assembly.GetManifestResourceNames();
			Assert.IsFalse(new List<string>(resources).Contains(resourceName));
			
			string output = Path.Combine(Path.GetTempPath(),"nonexistent.xyz");
			output = pathChecker.GetUnusedFilePath(output);
			
			Assert.IsFalse(File.Exists(output));
			try {
				rw.Write(resourceName,assembly,output);
			}
			catch (IOException) 
			{
				Assert.IsFalse(File.Exists(output));
				return;
			}
			
			if (File.Exists(output)) File.Delete(output);
			Assert.Fail("Failed to raise an IOException when trying to write an unknown resource.");			
		}
	}
}
