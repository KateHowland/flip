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
 * This file added by Keiron Nicholson on 04/06/2010 at 09:40.
 */

using System;
using System.IO;
using NWN2Toolset;
using OEIShared.IO;
using OEIShared.Utils;

namespace Sussex.Flip.Games.NeverwinterNightsTwo
{
	/// <summary>
	/// A helper class which allows string values to be written to and
	/// read from text files in the current module's resource collection.
	/// </summary>
	public class ModuleTextFile
	{
		public const string AttachedScriptsLogName = "AttachedScriptsLog";
		
		
		protected ushort txtCode = BWResourceTypes.GetResourceType("txt");
		
		
		public ModuleTextFile()
		{
		}
		
		
		public void AddStringEntry(string entry, string file)
		{
			if (entry == null) throw new ArgumentNullException("entry");
			
			using (Stream stream = GetTextFileStream(file,true)) {
				
				if (stream == null) throw new ApplicationException("Failed to retrieve or create stream for file '" + file + "'.");
				
				using (StreamWriter writer = new StreamWriter(stream)) {
					writer.WriteLine(entry);
					writer.Flush();
				}				
			}		
		}
		
		
		/// <summary>
		/// Creates a text file in the current module under the given name.
		/// </summary>
		/// <param name="file">The name to create a file under.</param>
		/// <remarks>MUST save the module after calling this method, or
		/// the resource will not be found.</remarks>
		public void CreateTextFile(string file)
		{
			if (NWN2ToolsetMainForm.App.Module == null) throw new InvalidOperationException("Module not open.");
			if (file == null) throw new ArgumentNullException("file");
			if (file == String.Empty) throw new ArgumentException("File name cannot be empty.","file");
			
			OEIResRef resRef = new OEIResRef(file);
			
			IResourceEntry resource = NWN2ToolsetMainForm.App.Module.Repository.FindResource(resRef,txtCode);
			
			if (resource != null) throw new ArgumentException("File '" + file + "' already exists.","file");
			
			resource = NWN2ToolsetMainForm.App.Module.Repository.CreateResource(resRef,txtCode);
			
			if (resource == null) throw new ApplicationException("Failed to retrieve or create resource.");
		}
		
		
		public bool HasTextFile(string file)
		{
			if (NWN2ToolsetMainForm.App.Module == null) throw new InvalidOperationException("Module not open.");
			if (file == null) throw new ArgumentNullException("file");
			if (file == String.Empty) throw new ArgumentException("File name cannot be empty.","file");
						
			OEIResRef resRef = new OEIResRef(file);
			
			IResourceEntry resource = NWN2ToolsetMainForm.App.Module.Repository.FindResource(resRef,txtCode);
			
			return resource != null;
		}
		
		
		public Stream GetTextFileStream(string file, bool write)
		{
			if (NWN2ToolsetMainForm.App.Module == null) throw new InvalidOperationException("Module not open.");
			if (file == null) throw new ArgumentNullException("file");
			if (file == String.Empty) throw new ArgumentException("File name cannot be empty.","file");
						
			OEIResRef resRef = new OEIResRef(file);
			
			IResourceEntry resource = NWN2ToolsetMainForm.App.Module.Repository.FindResource(resRef,txtCode);
			
			if (resource == null) throw new ApplicationException("Failed to retrieve resource.");
			
			return resource.GetStream(write);
		}
		
		
		public string GetTextFileContents(string file)
		{
			string contents;
			
			using (Stream stream = GetTextFileStream(file,false)) {
				
				if (stream == null) throw new ApplicationException("Failed to retrieve resource.");
				
				using (StreamReader reader = new StreamReader(stream)) {
					contents = reader.ReadToEnd();
				}
			}
			
			return contents;
		}
	}
}
