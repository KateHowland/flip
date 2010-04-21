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
 * This file added by Keiron Nicholson on 01/04/2010 at 10:10.
 */

using System;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml.Serialization;
using Sussex.Flip.Utils;

namespace Sussex.Flip.UI
{
	/// <summary>
	/// Description of ObjectBehaviour.
	/// </summary>
	[Serializable]
	[XmlRoot("theobjectbehaviour")]
	public abstract class ObjectBehaviour: DependencyObject, IDeepCopyable<ObjectBehaviour>, IEquatable<ObjectBehaviour>
	{
    	protected static DependencyProperty IdentifierProperty;
    	protected static DependencyProperty DisplayNameProperty;   
    	
    	
    	[XmlElement("theidentifier")]
    	public string Identifier {
    		get { return (string)base.GetValue(IdentifierProperty); }
    		set { base.SetValue(IdentifierProperty,value); }
    	}
    	
    	
    	[XmlElement("theDisplayName")]
    	public string DisplayName {
    		get { return (string)base.GetValue(DisplayNameProperty); }
    		set { base.SetValue(DisplayNameProperty,value); }
    	}
    	
    	
    	static ObjectBehaviour()
    	{
    		IdentifierProperty = DependencyProperty.Register("Identifier",typeof(string),typeof(ObjectBehaviour));
    		DisplayNameProperty = DependencyProperty.Register("DisplayName",typeof(string),typeof(ObjectBehaviour));
    	}
    	
    	
    	protected ObjectBehaviour() : this(String.Empty,String.Empty)
    	{    		
    	}
    	
    	
		public ObjectBehaviour(string identifier, string displayName)
		{
			if (identifier == null) throw new ArgumentNullException("identifier");
			if (displayName == null) throw new ArgumentNullException("displayName");
			
			Identifier = identifier;
			DisplayName = displayName;
		}
    	
    	
		public abstract string GetCode();
		public abstract string GetNaturalLanguage();	
		public abstract string GetDescriptionOfObjectType();
		public abstract ObjectBehaviour DeepCopy();		
		
		
		public virtual bool Equals(ObjectBehaviour other)
		{
			return other != null && other.GetType() == this.GetType() && other.Identifier == this.Identifier && other.DisplayName == this.DisplayName;
		}
	}
	
	
	public class DefaultObjectBehaviour : ObjectBehaviour
	{
		public override string GetCode()
		{
			return String.Empty;			
		}
		
		
		public override string GetNaturalLanguage()
		{
			return String.Empty;			
		}
		
		
		public override string GetDescriptionOfObjectType()
		{
			return String.Empty;
		}
		
		
		public override ObjectBehaviour DeepCopy()
		{
			return new DefaultObjectBehaviour();
		}
	}
}