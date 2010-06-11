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
 * This file added by Keiron Nicholson on 30/03/2010 at 12:15.
 */

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Sussex.Flip.UI
{
	/// <summary>
	/// Description of ConditionalControl.
	/// </summary>
	public abstract class ConditionalControl : Moveable
	{		
//    	protected static ResourceDictionary resourceDictionary;
//    	
//    	
//    	static ConditionalControl()
//    	{
//            resourceDictionary = new ResourceDictionary();
//            
//            Style style = new Style(typeof(TextBlock));
//            style.Setters.Add(new Setter(TextBlock.FontSizeProperty,20.0));
//            style.Setters.Add(new Setter(TextBlock.FontWeightProperty,FontWeights.Bold));
//            style.Setters.Add(new Setter(TextBlock.FontFamilyProperty,new FontFamily("Courier")));
//            style.Setters.Add(new Setter(TextBlock.ForegroundProperty,Brushes.Pink));
//            style.Setters.Add(new Setter(TextBlock.BackgroundProperty,Brushes.Black));
//            style.Setters.Add(new Setter(TextBlock.HeightProperty,30.0));
//            style.Setters.Add(new Setter(TextBlock.HorizontalAlignmentProperty,HorizontalAlignment.Stretch));
//    		
//            resourceDictionary.Add(style.TargetType,style);
//    	}
//    	
//    	
//        public ConditionalControl()
//        {
//            Resources = resourceDictionary;
//        }
        
        
		/// <summary>
		/// The condition which must be met for the consequences to result.
		/// </summary>
		public abstract Statement Condition { get; set; }
		
		// TODO:
		// need to set consequences but should probably replace
		// this with a better assignment than the spine directly
		
		/// <summary>
		/// The actions to be taken if the condition is met.
		/// </summary>
		public abstract Spine Consequences { get; set; }
		
		
		public override XmlSchema GetSchema()
		{
			return null;
		}
		
		
		public override void WriteXml(XmlWriter writer)
		{
			WriteCoordinates(writer);
			
			writer.WriteStartElement("Condition");
			if (Condition != null) {
				writer.WriteStartElement("Statement");
				Condition.WriteXml(writer);
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
			
			writer.WriteStartElement("Consequences");
			if (Consequences != null) {
				writer.WriteStartElement("Spine");
				Consequences.WriteXml(writer);
				writer.WriteEndElement();
			}
			writer.WriteEndElement();	
		}
		
		
		public override void ReadXml(XmlReader reader)
		{
			reader.MoveToContent();		
			
			ReadCoordinates(reader);
			
			reader.ReadStartElement(); // passed <IfElseControl>
			
			reader.MoveToContent();
			
			if (reader.LocalName != "Condition") throw new FormatException("Condition is not specified.");
			
			bool isEmpty = reader.IsEmptyElement;
			
			reader.ReadStartElement("Condition"); // passed <Condition>
			reader.MoveToContent(); // at <Consequences> or <Statement>
			
			if (!isEmpty) {
				
				if (reader.LocalName != "Statement") throw new FormatException("Condition is not correctly specified.");
				
				Statement statement = new Statement();
				statement.ReadXml(reader);
				Condition = statement;
				reader.MoveToContent();
				
				reader.ReadEndElement(); // passed </Condition>
				reader.MoveToContent(); // at <Consequences>
			}
			
			if (reader.IsEmptyElement) throw new FormatException("Consequences is not correctly specified (no Spine).");
			
			reader.ReadStartElement("Consequences");
			reader.MoveToContent();			
			Consequences.ReadXml(reader);
			reader.MoveToContent();
			reader.ReadEndElement();
			reader.MoveToContent();
			
			reader.ReadEndElement();
		}
	}
}
