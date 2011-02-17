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
	/// Description of Instance.
	/// </summary>
	public class InstanceBehaviour : Nwn2ObjectBehaviour
	{
		protected string resRef;
		protected string iconName;
		protected Nwn2Type type;		
		protected string areaTag;
		
		
		public override Nwn2Type Nwn2Type {
			get { return type; }
		}
				
		public string Tag {
			get { return Identifier; }
			set { Identifier = value; }
		}
		
		public string AreaTag {
			get { return areaTag; }
			set { areaTag = value; }
		}
		
		
		public string ResRef {
			get { return resRef; }
			set { resRef = value; }
		}
		
		
		public string IconName {
			get { return iconName; }
			set { iconName = value; }
		}
		
		
		public InstanceBehaviour() : this(string.Empty,string.Empty,NWN2ObjectType.Prefab,string.Empty,string.Empty,string.Empty)
		{			
		}
		
		
		public InstanceBehaviour(string tag, string displayName, Nwn2Type type, string areaTag, string resRef, string iconName) : base(tag,displayName)
		{		
			this.type = type;
			this.areaTag = areaTag;
			this.resRef = resRef;
			this.iconName = iconName;
		}
		
		
		public InstanceBehaviour(string tag, string displayName, NWN2ObjectType type, string areaTag, string resRef, string iconName) : this(tag,displayName,Nwn2ScriptSlot.GetNwn2Type(type),areaTag,resRef,iconName)
		{		
		}
		
		
		public override string GetCode()
		{			
			// Previous used GetObjectByTagAndType(), but this function appears to be broken.
			
			// Get the nNth object with the specified tag.
			// - sTag
			// - nNth: the nth object with this tag may be requested
			// * Returns OBJECT_INVALID if the object cannot be found.
			// Note: The module cannot be retrieved by GetObjectByTag(), use GetModule() instead.
			//object GetObjectByTag(string sTag, int nNth=0);
			
			int nTh = 0; // currently assuming we are referring to only the first object found with a given tag
			
			return String.Format("GetObjectByTag(\"{0}\",{1})",Tag,nTh);
		}
		
		
		public static string GetTagFromCode(string code)
		{
			if (String.IsNullOrEmpty(code)) return String.Empty;
			
			try {
				return "\"" + code.Split(new char[]{'"'})[1] + "\"";
			}
			catch (Exception) {
				return "\"FlipErrorCouldNotExtractTag\"";
			}
		}
		
		
		public override string GetNaturalLanguage()
		{
			return DisplayName;
		}
		
		
		public override ObjectBehaviour DeepCopy()
		{
			return new InstanceBehaviour(Identifier,DisplayName,type,areaTag,resRef,iconName);
		}
		
		
		public override string GetDescriptionOfObjectType()
		{
			return String.Format(Nwn2Fitter.SubtypeFormat,Nwn2Fitter.InstanceDescription,type);
		}
		
		
		public override object GetToolTip()
		{
			return Tag;
		}
		
		
		public override bool Equals(ObjectBehaviour other)
		{	
			if (!base.Equals(other)) return false;
						
			InstanceBehaviour i = other as InstanceBehaviour;
			
			return i != null;// && i.areaTag == this.areaTag;
		}
		
		
		public override string ToString()
		{
			return String.Format("{0}, {1}, {2}, {3}, {4}",Tag,DisplayName,ResRef,AreaTag,IconName);
		}
		
		
		public override void ReadXml(XmlReader reader)
		{
			reader.MoveToContent();
			
			if (!reader.IsEmptyElement) {
				throw new FormatException("Behaviour should not have a child.");
			}
			
			Identifier = reader["Identifier"];
			DisplayName = reader["DisplayName"];
			ResRef = reader["ResRef"];
			IconName = reader["IconName"];
			AreaTag = reader["AreaTag"];
			
			string nwn2TypeString = reader["Nwn2Type"];
			if (!String.IsNullOrEmpty(nwn2TypeString) && Nwn2ScriptSlot.Nwn2TypeNames.Contains(nwn2TypeString)) {
				type = (Nwn2Type)Enum.Parse(typeof(Nwn2Type),nwn2TypeString);
			}            
			
			reader.ReadStartElement();
		}
		
		
		public override void WriteXml(XmlWriter writer)
		{
			writer.WriteAttributeString("Identifier",Identifier);
			writer.WriteAttributeString("DisplayName",DisplayName);
			writer.WriteAttributeString("Nwn2Type",Enum.GetName(typeof(Nwn2Type),type));
			writer.WriteAttributeString("AreaTag",AreaTag);
			writer.WriteAttributeString("ResRef",ResRef);
			writer.WriteAttributeString("IconName",IconName);
		}
		
		
		public override string GetLogText()
		{
			return String.Format("{0} (DisplayName: {1}, Tag: {2}",Nwn2Type,DisplayName,Tag);
		}
	}
}
