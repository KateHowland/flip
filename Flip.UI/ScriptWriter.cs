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

namespace Sussex.Flip.UI
{
	/// <summary>
	/// Description of ScriptWriter.
	/// </summary>
	public class ScriptWriter
	{
		protected const string FlipCodeBegins = @"/* FLIP CODE - DO NOT EDIT";
		protected const string FlipCodeEnds = @"FLIP CODE - DO NOT EDIT */";
		protected string[] separators = new string[] { FlipCodeBegins, FlipCodeEnds };
		
		
		protected TriggerBar triggerBar;
		
		
		public ScriptWriter(TriggerBar triggerBar)
		{
			if (triggerBar == null) throw new ArgumentNullException("triggerBar");
			
			this.triggerBar = triggerBar;			
		}
		
		
		public void WriteNWScriptCode(StringBuilder code)
		{
			if (code == null) throw new ArgumentNullException("code");
			
			WriteNWScriptHeader(code);
			code.AppendLine();
			WriteNWScriptBody(code);
		}
		
		
		protected void WriteNWScriptHeader(StringBuilder code)
		{
			if (code == null) throw new ArgumentNullException("code");
			
			code.AppendLine("#include \"ginc_param_const\"");
			code.AppendLine("#include \"ginc_actions\"");
			code.AppendLine("#include \"NW_I0_GENERIC\"");		
			code.AppendLine("#include \"flip_functions\"");	
			code.AppendLine("#include \"ginc_henchman\"");
		}
		
		
		protected void WriteNWScriptBody(StringBuilder code)
		{
			if (code == null) throw new ArgumentNullException("code");
			
			code.AppendLine("void main()");
			code.AppendLine("{");
			code.AppendLine(triggerBar.Spine.GetCode());
			code.AppendLine("}");
		}
		
		
		protected void WriteNWScriptOpenComments(StringBuilder code)
		{
			if (code == null) throw new ArgumentNullException("code");
			
			code.AppendLine(FlipCodeBegins);
		}
		
		
		protected void WriteNWScriptCloseComments(StringBuilder code)
		{
			if (code == null) throw new ArgumentNullException("code");
			
			code.AppendLine(FlipCodeEnds);
		}
		
		
		protected void WriteNWScriptBlankLine(StringBuilder code)
		{
			if (code == null) throw new ArgumentNullException("code");
			
			code.AppendLine();
		}
		
		
		public void WriteFlipCode(XmlWriter writer)
		{
			if (writer == null) throw new ArgumentNullException("writer");
			
			writer.WriteStartDocument();
					
			writer.WriteStartElement("Script");
			triggerBar.WriteXml(writer);
			writer.WriteEndElement();
					
			writer.WriteEndDocument();				
			writer.Flush();
		}
		
		
		public string GetNWScriptCode()
		{			
			StringBuilder code = new StringBuilder();			
			WriteNWScriptCode(code);			
			return code.ToString();
		}
		
		
		public string GetFlipCode()
		{		
			string code;
			
			using (TextWriter tw = new StringWriter()) {
				using (XmlWriter xw = XmlWriter.Create(tw)) {
					WriteFlipCode(xw);	
					code = tw.ToString();
				}				
			}
			
			return code;
		}
		
		
		public string GetCombinedCode()
		{
			StringBuilder code = new StringBuilder();
			string result;
			
			using (TextWriter tw = new StringWriter(code)) {
				using (XmlWriter xw = XmlWriter.Create(tw)) {
					
					WriteNWScriptOpenComments(code);
					WriteFlipCode(xw);
					WriteNWScriptBlankLine(code);
					WriteNWScriptCloseComments(code);
					WriteNWScriptBlankLine(code);
					WriteNWScriptHeader(code);
					WriteNWScriptBlankLine(code);
					WriteNWScriptBody(code);
					tw.Flush();
					
					result = code.ToString();
				}
			}
			
			return result;
		}
		
		
		public string ExtractFlipCodeFromNWScript(string nwscript)
		{
			if (nwscript == null) throw new ArgumentNullException("nwscript");
			
			string[] parts = nwscript.Split(separators,StringSplitOptions.RemoveEmptyEntries);
			
			if (parts.Length == 0) return null;
			
			string flip = parts[0];
			
			flip = flip.Trim(null); // remove leading and trailing white space characters
			flip = flip.Replace(Environment.NewLine,String.Empty); // remove new line characters
			
			return flip;
		}
	}
}
