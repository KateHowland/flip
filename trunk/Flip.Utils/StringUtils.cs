/*
 * Created by SharpDevelop.
 * User: kn70
 * Date: 13/10/2010
 * Time: 10:30
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace Sussex.Flip.Utils
{
	/// <summary>
	/// Description of StringUtils.
	/// </summary>
	public static class StringUtils
	{
		public static string Parse(string body, string delimiter1, string delimiter2)
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
		
		
		/// <summary>
		/// Check whether a character is a punctuation mark.
		/// </summary>
		/// <param name="text">The character to check</param>
		/// <returns>True if the character is one of .,:;()!?&-, false otherwise</returns>
		private static bool IsPunctuation(char character)
		{
			char[] punctuation = new char[] {'.',',',':',';','(',')','!','?','&','-'};
			foreach (char c in punctuation) {
				if (character == c) {
					return true;
				}
			}
			return false;
		}
		
        
        /// <summary>
        /// Truncates a line of text to within a given length, if possible stopping at the end of whole word.
        /// </summary>
        /// <param name="text">The text to truncate.</param>
        /// <param name="maxLength">The maximum length of the returned line.</param>
        /// <returns>The original line if its length is less than maxLength, otherwise a truncated version of the line - 
        /// to the end of a word if possible (in which case it will probably be shorter than maxLength), or to the
        /// maximum possible length of the line if not.</returns>
		public static string Truncate(string text, int maxLength)
		{
			if (text.Length > maxLength) {
				int lastSpace = text.LastIndexOf(' ',maxLength-10);
				if (lastSpace != -1) {
					if (lastSpace > 0 && IsPunctuation(text[lastSpace-1])) { // don't end on a punctuation mark if possible
						return text.Substring(0,lastSpace-1);
					}
					else {
						return text.Substring(0,lastSpace);
					}
				}
				else {
					return text.Substring(0,maxLength);
				}
			}
			else {
				return text;
			}
		}
	}
}
