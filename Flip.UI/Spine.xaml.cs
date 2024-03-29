﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Sussex.Flip.Utils;

namespace Sussex.Flip.UI
{
    /// <summary>
    /// Interaction logic for Spine.xaml
    /// </summary>

    public partial class Spine : UserControl, ITranslatable, IDeepCopyable<Spine>, IXmlSerializable
    {
		public event EventHandler Changed;
		
		
		protected virtual void OnChanged(EventArgs e)
		{
			EventHandler handler = Changed;
			if (handler != null) {
				handler(this,e);
			}
		}
		
		
    	protected Duration animationTime;
    	protected Fitter fitter;
    	protected uint minPegs = 1; // TODO settable but not below 1 
		
		
		/// <summary>
		/// Check whether this Flip component has all essential fields filled in,
		/// including those belonging to subcomponents, such that it can generate valid code.
		/// </summary>
		/// <returns>True if all essential fields have been given values; false otherwise.</returns>
		/// <remarks>Note that this method makes no attempt to judge whether the values
		/// are valid in their slots, only that those slots have been filled.</remarks>
		public bool IsComplete { 
			get { 
				foreach (Peg peg in GetFilledPegs()) {
					if (!peg.Slot.IsComplete) return false;
				}
				return true;
			}
		}		
		
		
		public bool IsEmpty {
			get {
				return GetFilledPegs().Count == 0;
			}
		}

    	
		public Fitter Fitter {
			get { return fitter; }
			set { fitter = value; }
		}
        
        
        public double Extends {
        	get { return extension.Height.Value; }
        	set { extension.Height = new GridLength(value); }
        }
        
        
        public UIElementCollection Pegs {
        	get { return pegsPanel.Children; }
        }
        
        
    	public Spine() : this(new SpineFitter())
        {        	
        }
        
        
    	public Spine(Fitter fitter) : this(fitter,3)
        {        	
        }
    	
    	
    	public Spine(Fitter fitter, uint pegs)
    	{
    		if (fitter == null) throw new ArgumentNullException("fitter");
    		this.fitter = fitter;
    		
        	animationTime = new Duration(new TimeSpan(1000000));
        	
    		InitializeComponent();
    		
    		pegs = Math.Max(minPegs,pegs);
    		
    		for (int i = 0; i < pegs; i++) {
    			AddPeg();
    		}
    	}
    	    	
    	
    	public Spine(Fitter fitter, uint pegs, double extends) : this(fitter,pegs)
    	{
    		Extends = extends;
    	}

        
        public Peg AddPeg()
        {
        	return AddPeg(false,Pegs.Count);
        }

        
        public Peg AddPeg(bool animate)
        {
        	return AddPeg(animate,Pegs.Count);
        }
        
                
        public Peg AddPeg(bool animate, int index)
        {
        	if (index > Pegs.Count) throw new ArgumentException("Index out of range.","index");
        	
        	Peg peg = new Peg(fitter);
        	peg.VerticalAlignment = VerticalAlignment.Top;
        	
        	ScaleTransform scale = null;        	
        	if (animate) {
	        	scale = new ScaleTransform(0,0);
	        	peg.LayoutTransform = scale;	        
        	}
        	
        	Pegs.Insert(index,peg);
        	
        	peg.DropZone.Drop += new DragEventHandler(AttachToSpine);
        	
        	if (animate) {
	        	DoubleAnimation anim = new DoubleAnimation(0,1,animationTime);
	        	anim.AutoReverse = false;
	        	anim.IsAdditive = false;
        		scale.BeginAnimation(ScaleTransform.ScaleXProperty,anim);
        		scale.BeginAnimation(ScaleTransform.ScaleYProperty,anim);
        	}
        	
        	peg.Slot.Changed += delegate 
        	{ 
        		OnChanged(new EventArgs()); 
        	};
        	
        	return peg;
        }

        
        /// <summary>
        /// Attach a Moveable to the spine, either onto a free adjacent peg,
        /// or onto a newly-created peg.
        /// </summary>
        protected void AttachToSpine(object sender, DragEventArgs e)
        {	
        	if (e.Handled) return;
        	if (!e.Data.GetDataPresent(typeof(Moveable))) return;
        	if (!(e.AllowedEffects == DragDropEffects.Copy || e.AllowedEffects == DragDropEffects.Move)) return;
        	
        	Moveable moveable = e.Data.GetData(typeof(Moveable)) as Moveable;
				
        	if (fitter.Fits(moveable)) {
        		
			   	DropZone dropZone = sender as DropZone;
			   	if (dropZone == null) return;
			        	
			    Peg above = UIHelper.TryFindParent<Peg>(dropZone);        		
			    if (above == null) return;
			        				        	
			    int index = Pegs.IndexOf(above);
			    if (index == -1) throw new InvalidOperationException("Peg not found on spine.");
			    index++;
			        			        	
			    Peg below;
			    if (index < Pegs.Count) {
			    	below = (Peg)Pegs[index];
			    }
			    else {
			    	below = null;
			    }
			        	
			    Peg target = null;
			        	
			    // If a moveable has been dropped just above or below
			    // itself, do nothing. Otherwise, try to use an empty
			    // peg, or create a new peg if there isn't one:
			    
			    bool pointlessMove = 
			    	e.AllowedEffects == DragDropEffects.Move &&
			    	((above != null && above.Slot.Contents == moveable) || (below != null && below.Slot.Contents == moveable));
			    
			    if (pointlessMove) {
			    	e.Handled = true;
			     	return;
			    }			        	
			    else if (below != null && below.Slot.Contents == null) {
			    	target = below;
			    }
			    else if (above != null && above.Slot.Contents == null) {
			    	target = above;
			    }
			   	else {
			    	target = AddPeg(false,index);
			    }
			        	
				if (e.AllowedEffects == DragDropEffects.Copy) {
					target.Slot.Contents = moveable.DeepCopy();
				}
				else if (e.AllowedEffects == DragDropEffects.Move) {
					moveable.Remove();
					target.Slot.Contents = moveable;
				}
			    
			    try {
			    	Log.WriteAction(LogAction.placed,"block",target.Slot.Contents.GetLogText() + " on " + target.Slot.GetLogText());
			    	//ActivityLog.Write(new Activity("PlacedBlock","Block",target.Slot.Contents.GetLogText(),"PlacedOn",target.Slot.GetLogText()));
			    }
			    catch (Exception) {}
			    
        		OnChanged(new EventArgs());
        	}
        	
        	e.Handled = true;
        }
        
        
        public void RemovePeg()
        {
        	RemovePeg(false);
        }
        
        
        public void RemovePeg(bool animate)
        {
        	if (Pegs.Count > 0) RemovePeg(animate,Pegs.Count-1);
        }
        
        
        public void RemovePeg(bool animate, int index)
        {
        	if (index >= Pegs.Count) throw new ArgumentException("Index out of range.","index");
        	        	
        	Peg peg = Pegs[index] as Peg;
        	
        	if (peg == null) throw new InvalidCastException("The element at index " + index + " was not a Peg.");
        	
        	if (animate) {
        		ScaleTransform scale = new ScaleTransform(1,1);
        		peg.LayoutTransform = scale;
        		
	        	DoubleAnimation anim = new DoubleAnimation(1,0,animationTime);
	        	anim.AutoReverse = false;
	        	anim.IsAdditive = false;
	        	anim.Completed += delegate { Pegs.Remove(peg); };
        		scale.BeginAnimation(ScaleTransform.ScaleXProperty,anim);
        		scale.BeginAnimation(ScaleTransform.ScaleYProperty,anim);
        	}
        	else {
        		Pegs.Remove(peg);
        	}
        	
        	OnChanged(new EventArgs());
        }
        
        
        public void RemovePeg(bool animate, Peg peg)
        {
        	int index = Pegs.IndexOf(peg);
        	if (index == -1) throw new ArgumentException("Peg is not present on this spine.","peg");
        	RemovePeg(animate,index);
        }
        
        
        public void SetPegCount(uint pegs)
        {
        	if (pegs < minPegs) pegs = minPegs;
        	
        	while (Pegs.Count < pegs) {
        		AddPeg(false);
        	}
        	
        	while (Pegs.Count > pegs) {
        		RemovePeg(false);
        	}
        }
		
		
		protected void ShrinkSpine(object sender, RoutedEventArgs e)
		{
			Shrink(true);
			try {
				string parentText;
				if (Pegs.Count > 0) parentText = ((Peg)Pegs[0]).Slot.GetLogText();
				else parentText = String.Empty;
				Log.WriteMessage("removed all empty pegs on " + parentText);
				//ActivityLog.Write(new Activity("ShrunkPegs","On",parentText));
			}
			catch (Exception) {}
		}
		
		
		protected void GrowSpine(object sender, RoutedEventArgs e)
		{
			Peg peg = AddPeg(true,Pegs.Count);
			try {
				Log.WriteAction(LogAction.added,"peg");
				//ActivityLog.Write(new Activity("AddedPeg","On",peg.Slot.GetLogText()));
			}
			catch (Exception) {}
		}
        
        
        public void Shrink(bool animate)
        {
        	if (Pegs.Count <= minPegs) return;
        	
        	List<Peg> removing = new List<Peg>();
        	
        	foreach (UIElement element in Pegs) {
        		Peg peg = element as Peg;
        		if (peg != null && peg.Slot.Contents == null) {
        			removing.Add(peg);
        			if (Pegs.Count <= (minPegs + removing.Count)) break;
        		}
        	}
        	
        	foreach (Peg peg in removing) {
        		RemovePeg(animate,peg);
        	}
        }
        
        
        public List<Peg> GetFilledPegs()
        {
        	List<Peg> filled = new List<Peg>();
        	foreach (Peg peg in Pegs) {
        		if (peg.Slot.Contents != null) filled.Add(peg);
        	}
        	return filled;
        }
    	
        
		public string GetCode()
		{
			System.Text.StringBuilder code = new System.Text.StringBuilder();
			foreach (Peg peg in Pegs) {
				if (peg.Slot.Contents != null) {
					code.AppendLine(String.Format("{0}",peg.Slot.Contents.GetCode()));
				}
			}
			return code.ToString();
		}
		
    	
		public string GetNaturalLanguage()
		{
			// can't use Pegs as an empty Peg might screw up our formatting:
			List<Peg> filledPegs = GetFilledPegs(); 
			
			if (filledPegs.Count == 0) return "nothing happens";
			
			else {
				System.Text.StringBuilder code = new System.Text.StringBuilder();
			
				for (int i = 0; i < filledPegs.Count; i++) {
					Peg peg = (Peg)filledPegs[i];
					
					bool last = (i == filledPegs.Count - 1);
					bool penultimate = (i == filledPegs.Count - 2);
						
					if (last && filledPegs.Count > 1) code.Append("and ");
						
					code.Append(peg.Slot.GetNaturalLanguage());
						
					if (last) {}//code.Append(".");
					else if (penultimate) code.Append(" ");
					else code.Append(", ");
				}
				
				return code.ToString();
			}
		}
		
		
		public Spine DeepCopy()
		{
			Spine copy = new Spine(fitter,(uint)Pegs.Count,Extends);
			copy.Margin = Margin;
			
			for (int i = 0; i < Pegs.Count; i++) {
				Peg origPeg = (Peg)Pegs[i];
				Peg copyPeg = (Peg)copy.Pegs[i];
					
				Moveable moveable = origPeg.Slot.Contents;
				if (moveable != null) {
					copyPeg.Slot.Contents = moveable.DeepCopy();
				}
			}
			
			return copy;
		}
		
		
		public void Clear()
		{
			foreach (Peg peg in Pegs) {
				if (peg.Slot.Contents != null) {
					peg.Slot.Contents = null;
				}
			}
		}
		
		
		public void AssignImage(ImageProvider imageProvider)
		{
			if (imageProvider == null) throw new ArgumentNullException("imageProvider");
			
			foreach (Peg peg in Pegs) {
				if (peg.Slot.Contents != null) peg.Slot.Contents.AssignImage(imageProvider);
			}
		}
		
    	
		public XmlSchema GetSchema()
		{
			return null;
		}
		
    	
		public void ReadXml(XmlReader reader)
		{		
			while (Pegs.Count > 0) RemovePeg(false);
			
			reader.MoveToContent(); // at <Spine>
			
			if (reader.IsEmptyElement) throw new FormatException("Spine does not define a Pegs collection.");
						
			reader.ReadStartElement(); // passed <Spine>
			reader.MoveToContent(); // at <Pegs>
			
			if (reader.LocalName != "Pegs") throw new FormatException("Spine does not define a Pegs collection.");
			
			bool isEmpty = reader.IsEmptyElement;
			
			reader.ReadStartElement(); // passed <Pegs>
			
			if (!isEmpty) { 
				
				reader.MoveToContent(); // at <Peg> or </Pegs>
				
				while (reader.LocalName != "Pegs") {
					
					Peg peg = AddPeg();
					peg.ReadXml(reader);
					reader.MoveToContent();
					
				}
				
				reader.ReadEndElement(); // passed </Pegs>
			}
			
			reader.MoveToContent();
			reader.ReadEndElement(); // passed </Spine>
		}
		
    	
		public void WriteXml(XmlWriter writer)
		{
			writer.WriteStartElement("Pegs");
			
			foreach (Peg peg in Pegs) {
				writer.WriteStartElement("Peg");
				peg.WriteXml(writer);
				writer.WriteEndElement();
			}
			
			writer.WriteEndElement();
		}
        
        
		public ScriptStats GetStatistics()
		{		
			ScriptStats s = new ScriptStats();
			
			List<Peg> pegs = GetFilledPegs();
			s.Line += pegs.Count;
			
			foreach (Peg peg in pegs) s.Add(peg.Slot.GetStatistics());
			
			return s;
		}
    }
}