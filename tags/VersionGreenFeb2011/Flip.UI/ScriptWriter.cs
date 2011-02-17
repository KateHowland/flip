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
 * This file added by Keiron Nicholson on 03/06/2010 at 14:45.
 */

using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Sussex.Flip.UI
{
	/// <summary>
	/// Description of ScriptWriter.
	/// </summary>
	public class ScriptWriter : AbstractScriptWriter
	{
		protected TriggerBar triggerBar;
		
		
		public ScriptWriter(TriggerBar triggerBar)
		{
			if (triggerBar == null) throw new ArgumentNullException("triggerBar");			
			this.triggerBar = triggerBar;
		}
		
		
		protected override void WriteNWScriptBody(StringBuilder code)
		{
			if (code == null) throw new ArgumentNullException("code");
			
			code.AppendLine("void main()");
			code.AppendLine("{");
			code.AppendLine(triggerBar.Spine.GetCode());
			code.AppendLine("}");
		}
		
		
		protected override void WriteAddress(StringBuilder code)
		{
			if (code == null) throw new ArgumentNullException("code");			
			code.AppendLine(triggerBar.GetAddress());
		}
		
		
		protected override void WriteNaturalLanguage(StringBuilder code)
		{
			if (code == null) throw new ArgumentNullException("code");			
			code.AppendLine(triggerBar.GetNaturalLanguage());
		}
		
		
		public override void WriteFlipCode(XmlWriter writer)
		{
			if (writer == null) throw new ArgumentNullException("writer");
			
			writer.WriteStartDocument();
					
			writer.WriteStartElement("Script");
			triggerBar.WriteXml(writer);
			writer.WriteEndElement();
					
			writer.WriteEndDocument();				
			writer.Flush();
		}
		
		
		protected static string TriggerBreak = ":" + Environment.NewLine;
		
		
		public static string RemoveTrigger(string naturalLanguage)
		{
			if (String.IsNullOrEmpty(naturalLanguage)) return naturalLanguage;
			
			int index = naturalLanguage.IndexOf(TriggerBreak);
			
			if (index == -1) return naturalLanguage;
			
			else return naturalLanguage.Remove(0,index+TriggerBreak.Length);
		}
	}
}
