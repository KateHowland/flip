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
		protected const string NaturalLanguageBegins = @"/* NATURAL LANGUAGE";
		protected const string NaturalLanguageEnds = @"NATURAL LANGUAGE */";	
		
		
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
		
		
		protected void WriteNaturalLanguageBegins(StringBuilder code)
		{
			if (code == null) throw new ArgumentNullException("code");			
			code.AppendLine(NaturalLanguageBegins);
		}
		
		
		protected void WriteNaturalLanguageEnds(StringBuilder code)
		{
			if (code == null) throw new ArgumentNullException("code");			
			code.AppendLine(NaturalLanguageEnds);
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
		
		
		protected void WriteNaturalLanguage(StringBuilder code)
		{
			if (code == null) throw new ArgumentNullException("code");			
			code.AppendLine(triggerBar.GetNaturalLanguage());
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
					WriteNewLine(code);
					WriteNaturalLanguageBegins(code);
					WriteNaturalLanguage(code);
					WriteNaturalLanguageEnds(code);
					WriteNWScriptHeader(code);
					WriteNewLine(code);
					WriteNWScriptBody(code);
					tw.Flush();
					
					result = code.ToString();
				}
			}
			
			return result;
		}
		
		
		private static string Parse(string body, string delimiter1, string delimiter2)
		{
			if (body == null) throw new ArgumentNullException("body");
			if (delimiter1 == null) throw new ArgumentNullException("delimiter1");
			if (delimiter2 == null) throw new ArgumentNullException("delimiter2");
			if (delimiter1 == String.Empty) throw new ArgumentException("Delimiter cannot be blank.",delimiter1);
			if (delimiter2 == String.Empty) throw new ArgumentException("Delimiter cannot be blank.",delimiter2);
			
			if (body.Length == 0) return null;
			
			int d1 = body.IndexOf(delimiter1);
			if (d1 == -1) return null;
			
			int start = d1 + delimiter1.Length;
			if (start == body.Length - 1) return null;
			
			int d2 = body.IndexOf(delimiter2,start);
			if (d2 == -1) return null;
			
			int length = d2 - start;
			
			return body.Substring(start,length);
		}
		
		
		public static void ParseNWScript(string nwn2Code, out string flipCode, out string address, out string naturalLanguage)
		{
			if (nwn2Code == null) throw new ArgumentNullException("nwn2Code");
			
			try {
				flipCode = Parse(nwn2Code,FlipCodeBegins,FlipCodeEnds);				
			}
			catch (Exception) {
				flipCode = null;
			}
			
			try {
				address = Parse(nwn2Code,TargetAddressBegins,TargetAddressEnds);
			}
			catch (Exception) {
				address = null;
			}
			
			try {
				naturalLanguage = Parse(nwn2Code,NaturalLanguageBegins,NaturalLanguageEnds);
			}
			catch (Exception) {
				naturalLanguage = null;
			}
			
			if (flipCode != null) {
				flipCode = flipCode.Trim(null); // remove leading and trailing white space characters
				flipCode = flipCode.Replace(Environment.NewLine,String.Empty); // remove new line characters
			}
			
			if (address != null) {
				address = address.Trim(null); // remove leading and trailing white space characters
				address = address.Replace(Environment.NewLine,String.Empty); // remove new line characters
			}
			
			if (naturalLanguage != null) {
				naturalLanguage = naturalLanguage.Trim(null); // remove leading and trailing white space characters
			}
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
