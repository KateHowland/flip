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
 * This file added by Keiron Nicholson on 30/10/2009 at 16:35.
 */

using System;
using System.IO;
using System.Reflection;

namespace Sussex.Flip.Utils
{
	/// <summary>
	/// Writes resources to the file system.
	/// </summary>
	public class ResourceWriter
	{
		/// <summary>
		/// Writes a resource to a given path.
		/// </summary>
		/// <param name="resource">The name of the resource.</param>
		/// <param name="path">The path to copy the resource to.</param>
		public void Write(string resource, Assembly assembly, string path)
		{		
			using (System.IO.Stream res = assembly.GetManifestResourceStream(resource)) {
				
				if (res == null) throw new IOException("Could not locate resource " + resource + ".");
				
				byte[] buffer = new byte[327];
				int chunkLength;
				
				using (System.IO.FileStream fs = System.IO.File.Create(path)) {			
					while ((chunkLength = res.Read(buffer,0,buffer.Length)) > 0) {
						fs.Write(buffer,0,chunkLength);
					}
					fs.Flush();
				}
			}
		}
	}
}
