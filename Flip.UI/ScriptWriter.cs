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
		protected const string TargetAddressBegins = @"/* FLIP TARGET ADDRESS - DO NOT EDIT";
		protected const string TargetAddressEnds = @"FLIP TARGET ADDRESS - DO NOT EDIT */";
		protected static string[] separators = new string[] { FlipCodeBegins, FlipCodeEnds, TargetAddressBegins, TargetAddressEnds };
		
		
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
		
		
		protected void WriteFlipCodeBegins(StringBuilder code)
		{
			if (code == null) throw new ArgumentNullException("code");			
			code.AppendLine(FlipCodeBegins);
		}
		
		
		protected void WriteFlipCodeEnds(StringBuilder code)
		{
			if (code == null) throw new ArgumentNullException("code");			
			code.AppendLine(FlipCodeEnds);
		}
		
		
		protected void WriteAddressBegins(StringBuilder code)
		{
			if (code == null) throw new ArgumentNullException("code");			
			code.AppendLine(TargetAddressBegins);
		}
		
		
		protected void WriteAddressEnds(StringBuilder code)
		{
			if (code == null) throw new ArgumentNullException("code");			
			code.AppendLine(TargetAddressEnds);
		}
		
		
		protected void WriteNewLine(StringBuilder code)
		{
			if (code == null) throw new ArgumentNullException("code");			
			code.AppendLine();
		}
		
		
		protected void WriteAddress(StringBuilder code)
		{
			if (code == null) throw new ArgumentNullException("code");			
			code.AppendLine(triggerBar.GetAddress());
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
					
					WriteFlipCodeBegins(code);
					WriteFlipCode(xw);
					WriteNewLine(code);
					WriteFlipCodeEnds(code);
					WriteNewLine(code);
					WriteAddressBegins(code);
					WriteAddress(code);
					WriteAddressEnds(code);
					WriteNWScriptHeader(code);
					WriteNewLine(code);
					WriteNWScriptBody(code);
					tw.Flush();
					
					result = code.ToString();
				}
			}
			
			return result;
		}
		
		
		public static void ExtractFlipCodeFromNWScript(string nwn2Code, out string flipCode, out string address)
		{
			if (nwn2Code == null) throw new ArgumentNullException("nwn2Code");
			
			string[] parts = nwn2Code.Split(separators,StringSplitOptions.RemoveEmptyEntries);
			
			if (parts.Length < 2) {
				flipCode = null;
				address = null;
			}
			
			else {			
				flipCode = parts[0];				
				flipCode = flipCode.Trim(null); // remove leading and trailing white space characters
				flipCode = flipCode.Replace(Environment.NewLine,String.Empty); // remove new line characters
				
				address = parts[2];
				address = address.Trim(null); // remove leading and trailing white space characters
				address = address.Replace(Environment.NewLine,String.Empty); // remove new line characters
			}
		}
	}
}
