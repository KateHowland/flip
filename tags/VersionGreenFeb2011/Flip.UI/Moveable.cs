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
 * This file added by Keiron Nicholson on 20/01/2010 at 13:22.
 */

using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Sussex.Flip.Utils;

namespace Sussex.Flip.UI
{
	/// <summary>
	/// Description of Moveable.
	/// </summary>
	public abstract class Moveable : UserControl, IDeepCopyable<Moveable>, ITranslatable, IXmlSerializable
	{
		public event EventHandler Changed;
		
		
		public static Moveable CopiedToClipboard = null;
		
		
		protected virtual void OnChanged(EventArgs e)
		{
			EventHandler handler = Changed;
			if (handler != null) {
				handler(this,e);
			}
		}
		
		
		public Moveable()
		{
			HorizontalAlignment = HorizontalAlignment.Center;
			VerticalAlignment = VerticalAlignment.Center;
			
			if (ContextMenu == null) ContextMenu = new ContextMenu();
			MenuItem copy = new MenuItem();
			copy.Header = "Copy";
			copy.Click += delegate 
			{ 
				CopiedToClipboard = this.DeepCopy();
			};
			ContextMenu.Items.Add(copy);
		}
		
		
		public void MoveTo(Point position)
		{
			Canvas.SetLeft(this,position.X);
			Canvas.SetTop(this,position.Y);
		}
		
		
		public Point Position {
			get { 
				return new Point(Canvas.GetLeft(this),Canvas.GetTop(this));
			}
		}
    	
    	
    	public void Remove()
    	{
    		FrameworkElement f = this;
    		
    		while (f != null) {
    			
    			MoveableSlot slot = f as MoveableSlot;
    			if (slot != null && slot.Contents == this) {
    				slot.Contents = null;
					//ActivityLog.Write(new Activity("RemovedBlock","Block",this.GetLogText(),"RemovedFrom",slot.GetLogText()));
					Log.WriteAction(LogAction.removed,"block",this.GetLogText() + " from " + slot.GetLogText());
    				return;
    			}
    			
    			else f = f.Parent as FrameworkElement;
    		}
    		
    		if (Parent is Panel) {
    			Panel panel = (Panel)Parent;
    			if (panel.Children.Contains(this)) {
    				panel.Children.Remove(this);
    				
    				if (panel is SizedCanvas) {
    					//ActivityLog.Write(new Activity("RemovedBlock","Block",this.GetLogText(),"RemovedFrom","Canvas"));
						Log.WriteAction(LogAction.removed,"block",this.GetLogText() + " from canvas");
    				}
    				else {
    					//ActivityLog.Write(new Activity("RemovedBlock","Block",this.GetLogText(),"RemovedFrom","Unknown (" + panel + ")"));
						Log.WriteAction(LogAction.removed,"block",this.GetLogText() + " from unknown (" + panel + ")");
    				}
    			}
    		}
    		
    		else if (Parent is Decorator) {
    			Decorator decorator = (Decorator)Parent;
    			if (decorator.Child == this) {
    				decorator.Child = null;
    				//ActivityLog.Write(new Activity("RemovedBlock","Block",this.GetLogText(),"RemovedFrom","Unknown (" + decorator + ")"));
					Log.WriteAction(LogAction.removed,"block",this.GetLogText() + " from unknown (" + decorator + ")");
    			}
    		}
    		
    		else if (Parent == null) {
    			return;
//    			throw new InvalidOperationException("The element which was to be detached from its " +
//    			                                    "parent has a null parent.");
    		}
    		
    		else {
    			throw new InvalidOperationException("The element which was to be detached from its " +
    			                                    "parent has a parent of type " + Parent.GetType() +
    			                                    ", which the application does not know how to detach from.");
    		}
    	}
    	
    	
    	public abstract void AssignImage(ImageProvider imageProvider);
		
		
		internal static Moveable CreateMoveable(string name)
		{
			string type = String.Format("Sussex.Flip.UI.{0}",name);
			return (Moveable)System.Reflection.Assembly.GetExecutingAssembly().CreateInstance(type);
		}
    	
    	
    	protected void WriteCoordinates(XmlWriter writer)
    	{
			if (writer == null) throw new ArgumentNullException("writer");
			
			Point p = Position;
			writer.WriteAttributeString("X",p.X.ToString());
			writer.WriteAttributeString("Y",p.Y.ToString());
    	}
    	
			
		protected void ReadCoordinates(XmlReader reader)
		{
			if (reader == null) throw new ArgumentNullException("reader");
			
			string xStr = reader.GetAttribute("X");
			string yStr = reader.GetAttribute("Y");
			
			try {
				if (xStr != null && yStr != null) {
					double x = double.Parse(xStr);
					double y = double.Parse(yStr);
					MoveTo(new Point(x,y));
				}
			}
			catch (Exception) { }
		}
    	
		
		public abstract Moveable DeepCopy();
		public abstract string GetCode();
		public abstract string GetNaturalLanguage();
		public abstract bool IsComplete { get; }		
		public abstract XmlSchema GetSchema();
		public abstract void ReadXml(XmlReader reader);
		public abstract void WriteXml(XmlWriter writer);
		
		
		public abstract string GetLogText();
        
        
		public abstract ScriptStats GetStatistics();
	}
}