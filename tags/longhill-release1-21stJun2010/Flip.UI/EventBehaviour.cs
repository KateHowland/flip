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
 * This file added by Keiron Nicholson on 08/04/2010 at 16:38.
 */

using System;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Sussex.Flip.Utils;

namespace Sussex.Flip.UI
{
	/// <summary>
	/// Description of EventBehaviour.
	/// </summary>
	public class EventBehaviour: DependencyObject, IDeepCopyable<EventBehaviour>, IXmlSerializable
	{
    	protected static DependencyProperty EventNameProperty;
    	protected static DependencyProperty DisplayNameProperty;   
    	
    	
    	public string EventName {
    		get { return (string)base.GetValue(EventNameProperty); }
    		set { base.SetValue(EventNameProperty,value); }
    	}
    	
    	
    	public string DisplayName {
    		get { return (string)base.GetValue(DisplayNameProperty); }
    		set { base.SetValue(DisplayNameProperty,value); }
    	}
    	
    	
    	static EventBehaviour()
    	{
    		EventNameProperty = DependencyProperty.Register("EventName",typeof(string),typeof(EventBehaviour));
    		DisplayNameProperty = DependencyProperty.Register("DisplayName",typeof(string),typeof(EventBehaviour));
    	}
    	
    	
    	/// <summary>
    	/// Parameterless constructor for deserialisation.
    	/// </summary>
    	public EventBehaviour() : this(string.Empty,string.Empty)
    	{    		
    	}
    	
    	
		public EventBehaviour(string name, string displayName)
		{
			if (name == null) throw new ArgumentNullException("name");
			if (displayName == null) throw new ArgumentNullException("displayName");
			
			EventName = name;
			DisplayName = displayName;
		}
    	
    	
		public virtual string GetNaturalLanguage()
		{
			return DisplayName;	
		}
		
		
		public virtual EventBehaviour DeepCopy()
		{
			return new EventBehaviour(EventName,DisplayName);
		}
		
		
		public override string ToString()
		{
			return "EventBehaviour (" + EventName + ", " + DisplayName + ")";
		}
		
		
		public XmlSchema GetSchema()
		{
			return null;
		}
		
		
		public void ReadXml(XmlReader reader)
		{
			reader.MoveToContent();
			
			if (!reader.IsEmptyElement) {
				throw new FormatException("Behaviour should not have a child.");
			}
			
			EventName = reader["EventName"];
			DisplayName = reader["DisplayName"];						                     
			reader.ReadStartElement();
		}
		
		
		public void WriteXml(XmlWriter writer)
		{
			writer.WriteAttributeString("EventName",EventName);
			writer.WriteAttributeString("DisplayName",DisplayName);
		}
	}
}