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
 * This file added by Keiron Nicholson on 01/04/2010 at 13:50.
 */

using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Sussex.Flip.UI;
using Sussex.Flip.Games.NeverwinterNightsTwo.Utils;
using NWN2Toolset.NWN2.Data.Templates;

namespace Sussex.Flip.Games.NeverwinterNightsTwo.Behaviours
{
	/// <summary>
	/// Description of Blueprint.
	/// </summary>
	public class BlueprintBehaviour : Nwn2ObjectBehaviour
	{		
		protected string baseResRef;	
		protected string iconName;
		protected Nwn2Type type;
		
		
		public string ResRef {
			get { return Identifier; }
			set { Identifier = value; }
		}
		
		
		public string BaseResRef {
			get { return baseResRef; }
			set { baseResRef = value; }
		}
		
		
		public string IconName {
			get { return iconName; }
			set { iconName = value; }
		}
		
		
		public override Nwn2Type Nwn2Type {
			get { return type; }
		}
		
		
		public BlueprintBehaviour() : this(string.Empty,string.Empty,string.Empty,string.Empty,NWN2ObjectType.Light)
		{			
		}
		
		
		public BlueprintBehaviour(string resRef, string displayName, string baseResRef, string iconName, Nwn2Type type) : base(resRef,displayName)
		{		
			this.baseResRef = baseResRef;
			this.type = type;
		}
		
		
		public BlueprintBehaviour(string resRef, string displayName, string baseResRef, string iconName, NWN2ObjectType type) : this(resRef,displayName,baseResRef,iconName,Nwn2ScriptSlot.GetNwn2Type(type))
		{		
		}
		
		
		public override string GetCode()
		{			
			/* Not clear on when you would actually use a blueprint in NWScript,
			 * but in the only places I can find reference to it they seem to just
			 * use the ResRef. */
			
			return String.Format("\"{0}\"",ResRef);
		}
		
		
		public override string GetNaturalLanguage()
		{
			return String.Format("blueprint {0}",DisplayName);
		}
		
		
		public override ObjectBehaviour DeepCopy()
		{
			return new BlueprintBehaviour(Identifier,DisplayName,IconName,BaseResRef,type);
		}
		
		
		public override string GetDescriptionOfObjectType()
		{
			return String.Format(Nwn2Fitter.SubtypeFormat,Nwn2Fitter.BlueprintDescription,type);
		}
		
		
		public override void ReadXml(XmlReader reader)
		{
			if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "BlueprintBehaviour") {
				Identifier = reader["Identifier"];
				DisplayName = reader["DisplayName"];	
				type = (Nwn2Type)Enum.Parse(typeof(Nwn2Type),reader["Nwn2Type"]);
				BaseResRef = reader["BaseResRef"];
				IconName = reader["IconName"];
				reader.Read();
			}
		}
		
		
		public override void WriteXml(XmlWriter writer)
		{
			writer.WriteStartElement("BlueprintBehaviour");
			writer.WriteAttributeString("Identifier",Identifier);
			writer.WriteAttributeString("DisplayName",DisplayName);
			writer.WriteAttributeString("Nwn2Type",type.ToString());
			writer.WriteAttributeString("BaseResRef",BaseResRef);
			writer.WriteAttributeString("IconName",IconName);
			writer.WriteEndElement();
		}
	}
}
