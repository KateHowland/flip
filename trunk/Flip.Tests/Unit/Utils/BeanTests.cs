/*
 * Flip - a visual programming language for scripting video games
 * Copyright (C) 2009 University of Sussex
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
 * This file added by Keiron Nicholson on 29/10/2009 at 16:52.
 */


using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Sussex.Flip.Utils;

namespace Sussex.Flip.Utils.Tests
{
	[TestFixture]
	public class BeanTests
	{
		private class TestClass
		{
			public int A;
			private float B;
			private string _c;			
			public string C {
				get { return _c; }
				set { _c = value; }
			}
			
			
			internal TestClass(int a, float b, string c)
			{
				A = a;
				B = b;
				C = c;
			}
		}
		
		
		private class AnotherTestClass
		{		
			private string _c;
			public string C {
				get { return _c; }
				set { _c = value; }
			}
			public int D;
			public object E;
			
			
			internal AnotherTestClass(string c, int d, object e)
			{
				C = c;
				D = d;
				E = e;
			}
		}
		
		
		[Test]
		public void CapturesObjectAsBean()
		{
			int a = 330;
			float b = 12.784f;
			string c = "paradigm";
			
			TestClass captive = new TestClass(a,b,c);
			
			Bean bean = new Bean(captive);
			
			Assert.IsTrue(bean.HasValue("A"));
			Assert.IsTrue(bean.HasValue("B"));
			Assert.IsTrue(bean.HasValue("C"));
			Assert.AreEqual(a.ToString(),bean.GetValue("A"));
			Assert.AreEqual(b.ToString(),bean.GetValue("B"));
			Assert.AreEqual(c,bean.GetValue("C"));
		}
		
		
		[Test]
		public void CapturesMultipleObjectsAsBean()
		{
			int a = 330;
			float b = 12.784f;
			string c = "paradigm";
			
			TestClass captive = new TestClass(a,b,c);
			
			string c2 = "porcupine";
			int d = 990;
			object e = new OperatingSystem(PlatformID.Unix,new Version());
			
			AnotherTestClass captive2 = new AnotherTestClass(c2,d,e);
			
			Bean bean = new Bean(captive);
			
			bool overwrite = false;
			
			bean.Capture(captive2,overwrite);
			
			Assert.IsTrue(bean.HasValue("A"));
			Assert.IsTrue(bean.HasValue("B"));
			Assert.IsTrue(bean.HasValue("C"));
			Assert.IsTrue(bean.HasValue("D"));
			Assert.IsTrue(bean.HasValue("E"));
			Assert.AreEqual(a.ToString(),bean.GetValue("A"));
			Assert.AreEqual(b.ToString(),bean.GetValue("B"));
			Assert.AreEqual(c,bean.GetValue("C"));
			Assert.AreEqual(d.ToString(),bean.GetValue("D"));
			Assert.AreEqual(e.ToString(),bean.GetValue("E"));
			
			overwrite = true;
			
			bean.Capture(captive2,overwrite);
			
			Assert.IsTrue(bean.HasValue("A"));
			Assert.IsTrue(bean.HasValue("B"));
			Assert.IsTrue(bean.HasValue("C"));
			Assert.IsTrue(bean.HasValue("D"));
			Assert.IsTrue(bean.HasValue("E"));
			Assert.AreEqual(a.ToString(),bean.GetValue("A"));
			Assert.AreEqual(b.ToString(),bean.GetValue("B"));
			Assert.AreEqual(c2,bean.GetValue("C"));
			Assert.AreEqual(d.ToString(),bean.GetValue("D"));
			Assert.AreEqual(e.ToString(),bean.GetValue("E"));
		}
	}
}
