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
	}
}
