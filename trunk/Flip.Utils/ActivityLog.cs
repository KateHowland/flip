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
 * This file added by Keiron Nicholson on 11/06/2010 at 15:34.
 */

using System;
using System.IO;
using System.Text;
using System.Xml;

namespace Sussex.Flip.Utils
{
	/// <summary>
	/// Description of ActivityLog.
	/// </summary>
	public static class ActivityLog
	{
		private static XmlWriter writer = null;
		private static object padlock = new object();
		
		
		public static void StartLog(string path)
		{
			if (path == null) throw new ArgumentNullException("path");
			if (path == String.Empty) throw new ArgumentException("Must pass a valid path.","path");
			if (!Directory.Exists(Path.GetDirectoryName(path))) throw new ArgumentException("The folder containing the selected path does not exist.");
			
			lock (padlock) {
				if (IsRunning()) throw new ApplicationException("There is a user log already being written.");
				
				DateTime now = DateTime.Now;
				
				writer = new XmlTextWriter(path,Encoding.Default);
				writer.WriteStartDocument();
				writer.WriteStartElement("Session");
				writer.WriteAttributeString("User",Environment.UserName);
				writer.WriteAttributeString("Date",now.ToLongDateString());
				writer.WriteAttributeString("Time",now.ToLongTimeString());
				writer.Flush();
			}
		}
				
		
		public static void StopLog()
		{
			lock (padlock) {
				if (!IsRunning()) return;
				
				writer.WriteEndElement();
				writer.WriteEndDocument();
				writer.Flush();
				writer.Close();
				writer = null;
			}
		}
			
		
		public static bool IsRunning()
		{
			return writer != null;
		}
		
		
		public static void Write(Activity activity)
		{
			if (IsRunning() && activity != null) {
				lock (padlock) {
					activity.WriteXML(writer);
					writer.Flush();
				}
			}
		}
		
		
		public static string GetFilename()
		{
			return String.Format("{0} {1}.xml",Environment.UserName,GetFilenameSafeDateTimeString(DateTime.Now));
		}
		
		
		private static string GetFilenameSafeDateTimeString(DateTime dt)
		{
			if (dt == null) return String.Empty;
			
			return String.Format("{0} {1}",dt.ToLongDateString(),dt.ToLongTimeString().Replace(":","."));
		}		
	}
}
