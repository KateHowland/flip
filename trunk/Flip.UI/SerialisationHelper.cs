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
 * This file added by Keiron Nicholson on 26/04/2010 at 11:39.
 */

using System;
using System.Reflection;
using System.Runtime;
using System.Xml;
using System.Xml.Serialization;

namespace Sussex.Flip.UI
{
	/// <summary>
	/// Description of SerialisationHelper.
	/// </summary>
	public abstract class SerialisationHelper
	{
		// TODO:
		// Need to define a custom interface here which defines
		// Type as a field.
		
		
		public static Assembly customObjectAssembly = null;
		public static IXmlSerializable GetObjectFromXml(XmlReader reader)
		{
			if (reader == null) throw new ArgumentNullException("reader");
			
			reader.MoveToContent();	
			
			string type = reader.GetAttribute("Type");
			if (String.IsNullOrEmpty(type)) {
				throw new ArgumentException("Could not read Type attribute from XmlReader.","reader");
			}			
			
			IXmlSerializable obj;
				
			try {
				Assembly assembly = customObjectAssembly;
				obj = (IXmlSerializable)assembly.CreateInstance(type);
			}
			catch (Exception x) {
				throw new ArgumentException("Could not create object of type " + type + ".",x);
			}
			
			if (obj == null) {
				throw new ArgumentException("Could not create object of type " + type + " - type was not recognised.");
			}
			
			obj.ReadXml(reader);
			
			return obj;
		}
	}
}
