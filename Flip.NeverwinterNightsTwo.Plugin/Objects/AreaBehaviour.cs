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
 * This file added by Keiron Nicholson on 01/04/2010 at 13:38.
 */

using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Sussex.Flip.Games.NeverwinterNightsTwo.Utils;
using Sussex.Flip.UI;

namespace Sussex.Flip.Games.NeverwinterNightsTwo.Behaviours
{
	/// <summary>
	/// Description of Area.
	/// </summary>
	public class AreaBehaviour : Nwn2ObjectBehaviour
	{
		protected bool isExterior;
		
		
		public override Nwn2Type Nwn2Type {
			get {
				return Nwn2Type.Area;
			}
		}
		
		
		public string Tag {
			get { return Identifier; }
			set { Identifier = value; }
		}
		
		
		public bool IsExterior {
			get { return isExterior; }
			set { isExterior = value; }
		}
				
		
		public AreaBehaviour() : this(string.Empty,string.Empty,true)
		{			
		}
		
		
		public AreaBehaviour(string tag, string displayName, bool isExterior) : base(tag,displayName)
		{		
			this.isExterior = isExterior;
		}
		
		
		public override string GetCode()
		{
			/* Does not appear possible to retrieve an area object in a single
			 * line of code. You can iterate through areas, and presumably
			 * retrieve a tag from the area object (which is just class 'object')
			 * in order to identify one, but that's too complex for GetCode().
			 * Currently no reason to believe this will be needed, as specific
			 * area blocks will simply be used for attaching events. */
			
			return String.Empty;
		}
		
		
		public override string GetNaturalLanguage()
		{
			return DisplayName;
		}
		
		
		public override ObjectBehaviour DeepCopy()
		{
			return new AreaBehaviour(Identifier,DisplayName,IsExterior);
		}
		
		
		public override string GetDescriptionOfObjectType()
		{
			return Nwn2Fitter.AreaDescription;
		}
		
		
		public override void ReadXml(XmlReader reader)
		{
			if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "AreaBehaviour") {
				Identifier = reader["Identifier"];
				DisplayName = reader["DisplayName"];
				isExterior = Boolean.Parse(reader["IsExterior"]);
				reader.Read();
			}
		}
		
		
		public override void WriteXml(XmlWriter writer)
		{
			writer.WriteStartElement("AreaBehaviour");
			writer.WriteAttributeString("Identifier",Identifier);
			writer.WriteAttributeString("DisplayName",DisplayName);
			writer.WriteAttributeString("IsExterior",isExterior.ToString());
			writer.WriteEndElement();
		}
		
		
		public override string ToString()
		{
			return String.Format("Area ({0}, DisplayName:'{1}', Tag: {2})",(isExterior ? "Exterior" : "Interior"),DisplayName,Tag);
		}
	}
}