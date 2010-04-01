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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Sussex.Flip.Utils;

namespace Sussex.Flip.UI
{
	/// <summary>
	/// Description of ObjectBehaviour.
	/// </summary>
	public abstract class ObjectBehaviour: DependencyObject, IDeepCopyable<ObjectBehaviour>
	{
		public abstract string GetCode();
		public abstract string GetNaturalLanguage();		
		public abstract ObjectBehaviour DeepCopy();
	}
	
	
	public class TempObjectBehaviour : ObjectBehaviour
	{
    	protected static DependencyProperty TypeProperty;
    	protected static DependencyProperty SubtypeProperty;
    	protected static DependencyProperty IdentifierProperty;
    	protected static DependencyProperty DisplayNameProperty;
    	
    	
    	static TempObjectBehaviour()
    	{
    		TypeProperty = DependencyProperty.Register("Type",typeof(string),typeof(TempObjectBehaviour));
    		SubtypeProperty = DependencyProperty.Register("Subtype",typeof(string),typeof(TempObjectBehaviour));
    		IdentifierProperty = DependencyProperty.Register("Identifier",typeof(string),typeof(TempObjectBehaviour));
    		DisplayNameProperty = DependencyProperty.Register("DisplayName",typeof(string),typeof(TempObjectBehaviour));
    	}
    	
    	
    	public string Type {
    		get { return (string)base.GetValue(TypeProperty); }
    		set { base.SetValue(TypeProperty,value); }
    	}
    	
    	
    	public string Subtype {
    		get { return (string)base.GetValue(SubtypeProperty); }
    		set { base.SetValue(SubtypeProperty,value); }
    	}
    	
    	
    	public string Identifier {
    		get { return (string)base.GetValue(IdentifierProperty); }
    		set { base.SetValue(IdentifierProperty,value); }
    	}
    	
    	
    	public string DisplayName {
    		get { return (string)base.GetValue(DisplayNameProperty); }
    		set { base.SetValue(DisplayNameProperty,value); }
    	}
		
		
		public TempObjectBehaviour(string type, string subtype, string identifier, string displayName)
		{
			Type = type;
			Subtype = subtype;
			Identifier = identifier;
			DisplayName = displayName;
		}
		
		
		public override string GetCode()
		{
			return "NO CODE";
		}
		
		
		public override string GetNaturalLanguage()
		{
			return "NO NATURAL LANGUAGE";
		}
		
		
		public override ObjectBehaviour DeepCopy()
		{
			return new TempObjectBehaviour(Type,Subtype,Identifier,DisplayName);
		}
		
		
		public override string ToString()
		{
			// TODO
			return base.ToString();
		}
	}
}