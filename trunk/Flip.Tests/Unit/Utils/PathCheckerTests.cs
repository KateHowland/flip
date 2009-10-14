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
 * This file added by Keiron Nicholson on 01/10/2009 at 10:21.
 */

using System;
using System.IO;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Sussex.Flip.Utils;

namespace Sussex.Flip.Utils.Tests
{
	[TestFixture]
	public class PathCheckerTests
	{
		PathChecker pathChecker;
		
		
		[TestFixtureSetUp]
		public void Init()
		{
			pathChecker = new PathChecker();
		}
		
		
		[Test]
		public void ProvidesUnusedFilePath()
		{
			FileStream file1, file2;
			
			string dir = Path.GetTempPath();
			
			string filename = "Test file";
			string extension = ".xxx";
			
			string path1 = Path.Combine(dir,filename+extension);
			string path2 = Path.Combine(dir,filename+" (2)"+extension);
			string path3 = Path.Combine(dir,filename+" (3)"+extension);
			
			if (File.Exists(path1)) {
				File.Delete(path1);
			}
			if (File.Exists(path2)) {
				File.Delete(path2);
			}
			if (File.Exists(path3)) {
				File.Delete(path3);
			}
			
			// Should return the suggested path:
			Assert.AreEqual(path1,pathChecker.GetUnusedFilePath(path1));
			
			file1 = File.Create(path1);
			file1.Close();
			
			// Should return the path for a copy:
			Assert.AreEqual(path2,pathChecker.GetUnusedFilePath(path1));
			
			file2 = File.Create(path2);
			file2.Close();
			
			// Should return the path for a second copy:
			Assert.AreEqual(path3,pathChecker.GetUnusedFilePath(path1));
			
			File.Delete(path1);
			File.Delete(path2);
		}
		
		
		[Test]
		public void ProvidesUnusedDirectoryPath()
		{			
			string dir = Path.GetTempPath();
			
			string folder = "Test folder";
			
			string path1 = Path.Combine(dir,folder);
			string path2 = Path.Combine(dir,folder+" (2)");
			string path3 = Path.Combine(dir,folder+" (3)");
			
			if (Directory.Exists(path1)) {
				Directory.Delete(path1);
			}
			if (Directory.Exists(path2)) {
				Directory.Delete(path2);
			}
			if (Directory.Exists(path3)) {
				Directory.Delete(path3);
			}
			
			// Should return the suggested path:
			Assert.AreEqual(path1,pathChecker.GetUnusedDirectoryPath(path1));
			
			Directory.CreateDirectory(path1);
			
			// Should return the path for a copy:
			Assert.AreEqual(path2,pathChecker.GetUnusedDirectoryPath(path1));
			
			Directory.CreateDirectory(path2);
			
			// Should return the path for a second copy:
			Assert.AreEqual(path3,pathChecker.GetUnusedDirectoryPath(path1));
			
			Directory.Delete(path1);
			Directory.Delete(path2);
		}
	}
}
