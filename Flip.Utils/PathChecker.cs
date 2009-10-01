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
 * This file added by Keiron Nicholson on 01/10/2009 at 10:17.
 */

using System;
using System.IO;

namespace Sussex.Flip.Utils
{
	/// <summary>
	/// Checks whether a given path is available.
	/// </summary>
	public class PathChecker
	{
		#region Methods
		
		/// <summary>
		/// Returns an available file path based on a given preferred path - either
		/// the preferred path itself, or the next available path
		/// (e.g. C:\file (2).txt, C:\file (3).txt)
		/// </summary>
		/// <param name="preferredPath">The preferred path. This path will be
		/// returned if it is available.</param>
		/// <returns>Returns an available file path.</returns>
		public string GetUnusedFilePath(string preferredPath)
		{
			if (preferredPath == null) throw new ArgumentNullException("preferredPath");
			if (preferredPath == String.Empty) throw new ArgumentException("preferredPath");
			
			string parent = Path.GetDirectoryName(preferredPath);
			string preferredFilename = Path.GetFileNameWithoutExtension(preferredPath);
			string extension = Path.GetExtension(preferredPath);
			
			string filename = preferredFilename;
			string path;
			
			int count = 2;
			
			while (File.Exists(path = Path.Combine(parent,filename+extension))) {
				filename = preferredFilename + " (" + count + ")";
				count++;
			}
			
			return path;
		}
		
		
		/// <summary>
		/// Returns an available directory path based on a given preferred path - either
		/// the preferred path itself, or the next available path
		/// (e.g. C:\folder (2), C:\folder (3))
		/// </summary>
		/// <param name="preferredPath">The preferred path. This path will be
		/// returned if it is available.</param>
		/// <returns>Returns an available directory path.</returns>
		public string GetUnusedDirectoryPath(string preferredPath)
		{			
			if (preferredPath == null) throw new ArgumentNullException("preferredPath");
			if (preferredPath == String.Empty) throw new ArgumentException("preferredPath");
			
			string parent = Path.GetDirectoryName(preferredPath);
			string preferredFolder = Path.GetFileName(preferredPath);
			
			string folder = preferredFolder;
			string path;
			
			int count = 2;
			
			while (Directory.Exists(path = Path.Combine(parent,folder))) {
				folder = preferredFolder + " (" + count + ")";
				count++;
			}
			
			return path;
		}
		
		#endregion
	}
}
