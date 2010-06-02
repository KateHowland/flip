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
 * This file added by Keiron Nicholson on 31/03/2010 at 10:50.
 */

using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Sussex.Flip.Utils;

namespace Sussex.Flip.UI
{
	/// <summary>
	/// Description of ActionBehaviour.
	/// </summary>
	public abstract class StatementBehaviour: IDeepCopyable<StatementBehaviour>, IXmlSerializable
	{		
		protected int parameterCount;
		protected List<StatementComponent> components;
		protected StatementType statementType;
		
		
		public StatementType StatementType {
			get { return statementType; }
		}
		
		
		public virtual List<StatementComponent> GetComponents()
		{
			return components;
		}
		
		
		public virtual int GetParameterCount()
		{
			return parameterCount;
		}
		
		
		public abstract string GetCode(params string[] args);
		public abstract string GetNaturalLanguage(params string[] args);
		public abstract StatementBehaviour DeepCopy();
		
			
		// TODO:
		// temp... these actually need to be overriden properly, but for now...
		public XmlSchema GetSchema()
		{
			return null;
		}
		
		
		public void ReadXml(XmlReader reader)
		{
			reader.MoveToContent();			
			bool isEmpty = reader.IsEmptyElement;			
			reader.ReadStartElement();
			
			if (!isEmpty) {
				statementType = (StatementType)Enum.Parse(typeof(StatementType),reader["StatementType"]);
				
				int count = int.Parse(reader["ComponentCount"]);
				components = new List<StatementComponent>(count);
				
				for (int i = 0; i < count; i++) {
					components.Add(new StatementComponent("component " + i));
				}
				
				reader.ReadEndElement();
			}
		}
		
		
		public void WriteXml(XmlWriter writer)
		{
			writer.WriteAttributeString("StatementType",StatementType.ToString());
			writer.WriteAttributeString("ComponentCount",components.Count.ToString());
		}
	}
}
