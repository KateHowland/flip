///*
// * Flip - a visual programming language for scripting video games
// * Copyright (C) 2009, 2010 University of Sussex
// *
// * This program is free software: you can redistribute it and/or modify
// * it under the terms of the GNU General Public License as published by
// * the Free Software Foundation, either version 3 of the License, or
// * (at your option) any later version.
// *
// * This program is distributed in the hope that it will be useful,
// * but WITHOUT ANY WARRANTY; without even the implied warranty of
// * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// * GNU General Public License for more details.
// * 
// * You should have received a copy of the GNU General Public License
// * along with this program. If not, see <http://www.gnu.org/licenses/>.
// *
// * To contact the authors of this program, email flip@sussex.ac.uk.
// *
// * You can also write to Keiron Nicholson at the School of Informatics, 
// * University of Sussex, Sussex House, Brighton, BN1 9RH, United Kingdom.
// * 
// * This file added by Keiron Nicholson on 11/06/2010 at 15:50.
// */
//
//using System;
//using System.Collections.Generic;
//using System.Xml;
//
//namespace Sussex.Flip.Utils
//{
//	/// <summary>
//	/// Description of Activity.
//	/// </summary>
//	public class Activity
//	{
//		protected string name;
//		protected Dictionary<string,string> attributes;
//		
//		
//		public Activity(string activity)
//		{
//			if (activity == null) throw new ArgumentNullException("activity");
//			if (activity == String.Empty) throw new ArgumentException("activity");
//			
//			this.name = activity;
//			this.attributes = new Dictionary<string,string>();
//			
//			AddAttribute("Time",DateTime.Now.ToLongTimeString());
//		}
//		
//		
//		public Activity(string activity, string attribute1, string value1) : this(activity)
//		{
//			AddAttribute(attribute1,value1);
//		}
//		
//		
//		public Activity(string activity, string attribute1, string value1, string attribute2, string value2) : this(activity)
//		{
//			AddAttribute(attribute1,value1);
//			AddAttribute(attribute2,value2);
//		}
//		
//		
//		public Activity(string activity, string attribute1, string value1, string attribute2, string value2, string attribute3, string value3) : this(activity)
//		{
//			AddAttribute(attribute1,value1);
//			AddAttribute(attribute2,value2);
//			AddAttribute(attribute3,value3);
//		}
//		
//		
//		public void AddAttribute(string attribute, string value)
//		{
//			if (attribute == null) throw new ArgumentNullException("attribute");
//			if (attribute == String.Empty) throw new ArgumentException("attribute");
//			
//			attributes.Add(attribute,value);
//		}
//		
//		
//		public void WriteXML(XmlWriter writer)
//		{
//			if (writer == null) throw new ArgumentNullException("writer");
//			
//			writer.WriteStartElement(name);
//			
//			foreach (string attribute in attributes.Keys) {
//				string value = attributes[attribute];
//				writer.WriteAttributeString(attribute,value);
//			}
//			
//			writer.WriteEndElement();
//		}
//		
//		
//		public override string ToString()
//		{
//			try {
//				using (System.IO.StringWriter stringWriter = new System.IO.StringWriter()) {
//					using (XmlWriter writer = new XmlTextWriter(stringWriter)) {
//						WriteXML(writer);
//						writer.Flush();
//						return stringWriter.ToString();
//					}
//				}
//			}
//			catch (Exception x) {
//				return x.ToString();
//			}
//		}
//	}
//}