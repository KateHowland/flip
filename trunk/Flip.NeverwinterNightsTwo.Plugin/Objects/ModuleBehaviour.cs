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
 * This file added by Keiron Nicholson on 01/04/2010 at 13:26.
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
	/// Description of Module.
	/// </summary>
	public class ModuleBehaviour : Nwn2ObjectBehaviour
	{
		public ModuleBehaviour() : base(String.Empty,"module")
		{						
		}
		
		
		public override Nwn2Type Nwn2Type {
			get {
				return Nwn2Type.Module;
			}
		}		
		
		
		public override string GetCode()
		{
			return "GetModule()";
		}
		
		
		public override string GetNaturalLanguage()
		{
			return "the module";
		}
		
		
		public override ObjectBehaviour DeepCopy()
		{
			return new ModuleBehaviour();
		}
		
		
		public override string GetDescriptionOfObjectType()
		{
			return Nwn2Fitter.ModuleDescription;
		}
		
		
		public override void ReadXml(XmlReader reader)
		{
			if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "ModuleBehaviour") {
				Identifier = reader["Identifier"];
				DisplayName = reader["DisplayName"];	
				reader.Read();
			}
		}
		
		
		public override void WriteXml(XmlWriter writer)
		{
			writer.WriteAttributeString("Identifier",Identifier);
			writer.WriteAttributeString("DisplayName",DisplayName);
		}
	}
}
