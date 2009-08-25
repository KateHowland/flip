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
 * To contact the authors of this program, email k.nicholson@sussex.ac.uk.
 *
 * You can also write to Keiron Nicholson at the School of Informatics, 
 * University of Sussex, Sussex House, Brighton, BN1 9RH, United Kingdom.
 * 
 * This file added by Keiron Nicholson on 25/08/2009 at 09:48.
 */


using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace Sussex.Flip.Tests.Unit.Utils
{
	[TestFixture]
	public class Nwn2ModuleFunctionsTests
	{		
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
		public void ConvertsFileBasedModuleToDirectory()
		{
			Assert.Fail();
		}
		
		
		[Test]
		public void ConvertsDirectoryBasedModuleToFile()
		{
			Assert.Fail();
		}
	}
}
