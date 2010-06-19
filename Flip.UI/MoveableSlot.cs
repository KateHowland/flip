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
 * This file added by Keiron Nicholson on 17/03/2010 at 14:18.
 */

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml;
using System.Xml.Schema;
using Sussex.Flip.Utils;

namespace Sussex.Flip.UI
{
	/// <summary>
	/// A slot which can hold a Moveable object.
	/// </summary>
	public abstract class MoveableSlot : UserControl, IDeepCopyable<MoveableSlot>, ITranslatable
    {
		#region Fields
		
    	/// <summary>
    	/// Decides whether a given Moveable can fit into this slot.
    	/// </summary>
    	protected Fitter fitter;
    	
    	/// <summary>
    	/// A brush for indicating that no drag operation
    	/// has been detected over this slot.
    	/// </summary>
    	protected static Brush defaultBrush;
    	
    	/// <summary>
    	/// A brush for indicating that a dragged
    	/// object cannot be dropped into this slot.
    	/// </summary>
    	protected static Brush noDropBrush;
    	
    	/// <summary>
    	/// A brush for indicating that a dragged
    	/// object can be dropped into this slot.
    	/// </summary>
    	protected static Brush dropBrush;
    	
    	#endregion
    	
    	#region Properties
    	
    	/// <summary>
    	/// The Moveable held by this slot.
    	/// </summary>
    	public Moveable Contents { 
    		get { 
    			return GetMoveable(); 
    		}
    		set { 
    			if (Contents != null) {
    				Contents.Changed -= changeTracker;
    			}
    			
    			SetMoveable(value); 
    			
    			OnMoveableChanged(new MoveableEventArgs(value));
    			
    			SetDefaultAppearance();
    			
    			if (Contents != null) {
    				Contents.Changed += changeTracker;
    			}
    		}
    	}
				

		/// <summary>
		/// Check whether this Flip component has all essential fields filled in,
		/// including those belonging to subcomponents, such that it can generate valid code.
		/// </summary>
		/// <returns>True if all essential fields have been given values; false otherwise.</returns>
		/// <remarks>Note that this method makes no attempt to judge whether the values
		/// are valid in their slots, only that those slots have been filled.</remarks>
		public virtual bool IsComplete { 
			get { 
				return Contents != null && Contents.IsComplete;
			}
		}	
		
    	
    	protected EventHandler changeTracker;
    	void AnnounceChange(object sender, EventArgs e)
    	{
    		OnChanged(new EventArgs());
    	}
        
        
    	/// <summary>
    	/// Decides whether a given Moveable can fit into this slot.
    	/// </summary>
		public Fitter Fitter {
			get { return fitter; }
			set { fitter = value; }
		}
        
    	#endregion
    	
    	#region Events
    	
    	public event EventHandler<MoveableEventArgs> MoveableChanged;
    	
		protected virtual void OnMoveableChanged(MoveableEventArgs e)
		{
			EventHandler<MoveableEventArgs> handler = MoveableChanged;
			if (handler != null) {
				handler(this,e);
			}
		}
		
		
    	public event EventHandler Changed;
    	
		protected virtual void OnChanged(EventArgs e)
		{
			EventHandler handler = Changed;
			if (handler != null) {
				handler(this,e);
			}
		}
    	
    	#endregion
    	
    	#region Constructors
        
    	/// <summary>
    	/// Initialises shared brushes.
    	/// </summary>
    	static MoveableSlot()
    	{    		
    		defaultBrush = Brushes.Black;
    		noDropBrush = Brushes.Gray;
    		dropBrush = Brushes.Blue;
    	}
    	
    	
		/// <summary>
		/// Constructs a new <see cref="MoveableSlot"/> instance.
		/// </summary>
		/// <param name="fitter">A fitter which decides whether a 
		/// given Moveable can fit into this slot.</param>
        public MoveableSlot(Fitter fitter)
        {   
        	if (fitter == null) throw new ArgumentNullException("fitter");
        	
            this.fitter = fitter;
            
        	changeTracker = new EventHandler(AnnounceChange);
        	
            Drop += AcceptDrop;
            DragEnter += HandleDragEnter;
            DragLeave += HandleDragLeave;
            
            MoveableChanged += delegate { OnChanged(new EventArgs()); };
        }
        
        #endregion
        
        #region Methods
        
        /// <summary>
        /// Gets the Moveable held by this slot.
        /// </summary>
        /// <returns>A Moveable, or null if the slot is empty.</returns>
        protected abstract Moveable GetMoveable();
                
        
        /// <summary>
        /// Sets the Moveable held by this slot.
        /// </summary>
        protected abstract void SetMoveable(Moveable moveable);
        
        
        /// <summary>
        /// Change the appearance of the control to indicate
        /// that it will accept a drop.
        /// </summary>
        protected abstract void SetDropAppearance();
        
        
        /// <summary>
        /// Change the appearance of the control to indicate
        /// that it will not accept a drop.
        /// </summary>
        protected abstract void SetNoDropAppearance();
        
        
        /// <summary>
        /// Restore the default appearance of the control.
        /// </summary>
        protected abstract void SetDefaultAppearance();
        
        
		/// <summary>
        /// If the dragged object can fit into this slot,
        /// change the appearance of the control to indicate
        /// that it will accept a drop.
		/// </summary>
        protected virtual void HandleDragEnter(object sender, DragEventArgs e)
        {
            if (!e.Handled) {
            	Moveable moveable = e.Data.GetData(typeof(Moveable)) as Moveable; 
            	if (moveable != null && moveable != Contents) {
            		if (Fits(moveable)) SetDropAppearance();
            		else SetNoDropAppearance();
            	}
            }
        }
        

        /// <summary>
        /// Restore the default appearance of the control.
        /// </summary>
        protected virtual void HandleDragLeave(object sender, DragEventArgs e)
        {
        	SetDefaultAppearance();
        }
        

        /// <summary>
        /// Accepts dropped Moveable objects, if they fit this slot.
        /// </summary>
        protected virtual void AcceptDrop(object sender, DragEventArgs e)
        {
        	if (!e.Handled) {
        		if (e.Data.GetDataPresent(typeof(Moveable))) {
					Moveable moveable = e.Data.GetData(typeof(Moveable)) as Moveable;
					if (moveable != null && moveable != Contents && Fits(moveable)) {
						
						if (e.AllowedEffects == DragDropEffects.Copy) {
							Contents = moveable.DeepCopy();
							ActivityLog.Write(new Activity("CopyPastedBlockToSlot","Block",Contents.GetLogRepresentation(),"PlacedIn",this.GetLogRepresentation()));
						}
						else if (e.AllowedEffects == DragDropEffects.Move) {
							moveable.Remove();
							Contents = moveable;
							ActivityLog.Write(new Activity("PlacedBlockInSlot","Block",Contents.GetLogRepresentation(),"PlacedIn",this.GetLogRepresentation()));
						}
					}
					e.Handled = true;
        		}
			}
			SetDefaultAppearance();
        }
        
                
        /// <summary>
        /// Checks whether a given Moveable can be placed
        /// into this slot.
        /// </summary>
        /// <param name="moveable">The Moveable to check.</param>
        /// <returns>True if the given Moveable can fit
        /// in this slot; false otherwise.</returns>
        public virtual bool Fits(Moveable moveable)
        {
        	return fitter.Fits(moveable);
        }
        
        
		/// <summary>
		/// Gets a deep copy of this instance.
		/// </summary>
		/// <returns>A deep copy of this instance.</returns>
        public abstract MoveableSlot DeepCopy();
        
        
		public string GetCode()
		{
			if (Contents == null) return String.Empty;
			else return Contents.GetCode();
		}
		
		
		public string GetNaturalLanguage()
		{
			if (Contents == null) return fitter.GetMoveableDescription();
			else return Contents.GetNaturalLanguage();
		}
		
    	
		public XmlSchema GetSchema()
		{
			return null;
		}
		
		
		public void ReadXml(XmlReader reader)
		{
			reader.MoveToContent();
			
			bool isEmpty = reader.IsEmptyElement;
			
			reader.ReadStartElement();
			
			if (!isEmpty) {
				reader.MoveToContent();
				Moveable moveable = Moveable.CreateMoveable(reader.LocalName);
				moveable.ReadXml(reader);
				Contents = moveable;
				reader.ReadEndElement();
			}
				
			reader.MoveToContent();
		}
		
    	
		public void WriteXml(XmlWriter writer)
		{
			if (Contents != null) {
				writer.WriteStartElement(Contents.GetType().Name);
				Contents.WriteXml(writer);
				writer.WriteEndElement();
			}
		}
		
		
		public virtual string GetLogRepresentation()
		{
			string parentDescription;
			
			Moveable moveable = UIHelper.TryFindParent<Moveable>(this);
			if (moveable != null) parentDescription = moveable.GetLogRepresentation();
			else {
				DependencyObject parent = UIHelper.GetParentObject(this);
				TriggerBar triggerBar = parent as TriggerBar;
				if (triggerBar != null) {
					parentDescription = triggerBar.GetLogRepresentation();
				}
				else if (parent != null) parentDescription = parent.ToString();
				else parentDescription = "missing parent";
			}
			
			return "Slot on " + parentDescription;
		}
		
		#endregion
    }
}
