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
 * This file added by Keiron Nicholson on 14/10/2009 at 12:12.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace Sussex.Flip.Utils
{
	/// <summary>
	/// A bean representing information about an object for
	/// sending in serialised form.
	/// </summary>
	[DataContract]
	public class Bean
	{
		#region Fields
		
		private Dictionary<string,string> body;
		
		private static BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
		
		#endregion
		
		#region Properties
		
		/// <summary>
		/// The collection of serialised values for
		/// the bean.
		/// </summary>
		[DataMember]
		public Dictionary<string, string> Body {
			get { return body; }
			set { body = value; }
		}
		
		#endregion
		
		#region Constructors
					
		/// <summary>
		/// Constructs a new <see cref="Bean"/> instance.
		/// </summary>			
		public Bean()
		{
			body = new Dictionary<string, string>();
		}
		
		
		/// <summary>
		/// Constructs a new <see cref="Bean"/> instance.
		/// </summary>
		/// <param name="capturing">An object which will have
		/// its properties and fields serialised as string values
		/// and stored on the bean.</param>
		public Bean(object capturing)
		{
			if (capturing == null) throw new ArgumentNullException("capturing");
			
			body = new Dictionary<string,string>();
			
			Capture(capturing,false);
		}
		
		
		/// <summary>
		/// Constructs a new <see cref="Bean"/> instance.
		/// </summary>
		/// <param name="capturing">An object which will have
		/// its properties and fields serialised as string values
		/// and stored on the bean.</param>
		/// <param name="captureFields">A list of fields on the
		/// object which should be captured - if this list is not
		/// null, only fields which appear on the list will be
		/// captured.</param>
		public Bean(object capturing, IList<string> captureFields)
		{
			if (capturing == null) throw new ArgumentNullException("capturing");
			
			body = new Dictionary<string,string>();
			
			Capture(capturing,false,captureFields);
		}
		
		#endregion
		
		#region Methods
		
		/// <summary>
		/// Captures the fields and properties of the given object
		/// in serialised form.
		/// </summary>
		/// <param name="capturing">An object which will have
		/// its properties and fields serialised as string values
		/// and stored on the bean.</param>
		/// <param name="overwrite">True to overwrite any existing
		/// values with the same key; false otherwise.</param>
		public void Capture(object capturing, bool overwrite)
		{			
			foreach (MemberInfo mi in capturing.GetType().GetMembers(flags))	{				
				CaptureMember(capturing,mi,overwrite);
			}
		}
		
		
		/// <summary>
		/// Captures a subset of the fields and properties 
		/// of the given object
		/// in serialised form.
		/// </summary>
		/// <param name="capturing">An object which will have
		/// its properties and fields serialised as string values
		/// and stored on the bean.</param>
		/// <param name="overwrite">True to overwrite any existing
		/// values with the same key; false otherwise.</param>
		/// <param name="captureFields">A list of fields on the
		/// object which should be captured - if this list is not
		/// null, only fields which appear on the list will be
		/// captured.</param>
		public void Capture(object capturing, bool overwrite, IList<string> fields)
		{
			if (fields == null) {
				Capture(capturing,overwrite);
				return;
			}
			
			foreach (string field in fields) {
				foreach (MemberInfo member in capturing.GetType().GetMember(field,flags)) {
					CaptureMember(capturing,member,overwrite);
				}
			}
		}
		
		
		/// <summary>
		/// Captures a given field or property of the object in serialised form.
		/// </summary>
		/// <param name="capturing">An object which will have
		/// its properties and fields serialised as string values
		/// and stored on the bean.</param>
		/// <param name="member">The field or property to capture.</param>
		/// <param name="overwrite">True to overwrite any existing
		/// values with the same key; false otherwise.</param>
		private void CaptureMember(object capturing, MemberInfo member, bool overwrite)
		{	
			object o;
			switch (member.MemberType) {
				case MemberTypes.Property:
					o = ((PropertyInfo)member).GetValue(capturing,null);
					break;
				case MemberTypes.Field:
					o = ((FieldInfo)member).GetValue(capturing);
					break;
				default:
					return;
			}
			
			string val;
			
			if (o == null) val = String.Empty;
			else val = o.ToString();
			
			int count = 1;
			string key = member.Name;
			
			if (!body.ContainsKey(key)) {
				body.Add(key,val);
			}
			else {
				if (overwrite) {
					body[key] = val;
				}
				else {
					while (body.ContainsKey(key)) {
						key = member.Name + ++count;
					}
					body.Add(key,val);
				}
			}
		}
		
		
		/// <summary>
		/// Checks whether a value is stored for a particular key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>True if a value is stored for this key, false
		/// otherwise.</returns>
		public bool HasValue(string key)
		{
			if (key == null) throw new ArgumentNullException("key");
			if (key == String.Empty) throw new ArgumentException("key");
			
			return body.ContainsKey(key);
		}
		
		
		/// <summary>
		/// Gets the value stored for a particular key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>The value stored for this key.</returns>
		public string GetValue(string key)
		{
			if (key == null) throw new ArgumentNullException("key");
			if (key == String.Empty) throw new ArgumentException("key");
			
			if (!body.ContainsKey(key)) throw new ArgumentException("This bean has not captured the field '" + key + "'.","key");
			else return body[key];
		}
		
		
		/// <summary>
		/// Sets the value stored for a particular key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="val">The value to store for this key.</param>
		public void SetValue(string key, string val)
		{
			if (key == null) throw new ArgumentNullException("key");
			if (key == String.Empty) throw new ArgumentException("key");
			if (val == null) throw new ArgumentNullException("val");
			if (val == String.Empty) throw new ArgumentException("val");
			
			if (body.ContainsKey(key)) body[key] = val;
			else body.Add(key,val);
		}
		
		
		public override bool Equals(object obj)
		{
			return body.SequenceEqual(((Bean)obj).Body);
		}
		
		
		public override int GetHashCode()
		{
			return body.GetHashCode();
		}
		
		
		public override string ToString()
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			string val = null;
			foreach (string key in body.Keys) {
				body.TryGetValue(key,out val);
				sb.AppendLine(key + ": " + val);
			}
			return sb.ToString();
		}
		
		
		public string this[string key]
		{
			get {
				return GetValue(key);
			}
			set {
				SetValue(key,value);
			}
		}
		
		#endregion
	}
}
