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
 * This file added by Keiron Nicholson on 21/04/2010 at 16:56.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace Sussex.Flip.Utils
{
	/// <summary>
	/// Description of Serialisation.
	/// </summary>
	public class Serialiser
	{
		protected Dictionary<Type,XmlSerializer> serialisers;
		
		
		public Serialiser()
		{
			serialisers = new Dictionary<Type,XmlSerializer>();
		}
		
		
		public void Serialise(string path, object subject)
		{
			if (subject == null) throw new ArgumentNullException("subject");
			if (path == null) throw new ArgumentNullException("path");
						
			XmlSerializer xmlSerialiser = GetSerialiser(subject.GetType());
			
			using (StreamWriter writer = File.CreateText(path)) {
				xmlSerialiser.Serialize(writer,subject);
			}
		}
		
		
		public object Deserialise(string path, Type type)
		{
			if (path == null) throw new ArgumentNullException("path");
			if (type == null) throw new ArgumentNullException("type");
			
			XmlSerializer xmlSerialiser = GetSerialiser(type);
			
			using (FileStream stream = File.Open(path,FileMode.Open)) {
				return xmlSerialiser.Deserialize(stream);
			}
		}
		
		
		public bool CanDeserialise(string path, Type type)
		{
			if (path == null) throw new ArgumentNullException("path");
			if (type == null) throw new ArgumentNullException("type");
			
			XmlSerializer xmlSerialiser = GetSerialiser(type);
			
			using (XmlTextReader xmlReader = new XmlTextReader(path)) {
				return xmlSerialiser.CanDeserialize(xmlReader);
			}
		}
		
		
		protected XmlSerializer GetSerialiser(Type type)
		{
			if (type == null) throw new ArgumentNullException("type");
			
			XmlSerializer xmlSerialiser;
			
			if (serialisers.ContainsKey(type)) {
				xmlSerialiser = serialisers[type];
			}
			else {
				xmlSerialiser = new XmlSerializer(type);
				serialisers.Add(type,xmlSerialiser);
			}
			
			return xmlSerialiser;
		}
	}
}
