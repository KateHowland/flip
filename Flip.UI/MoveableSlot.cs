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
using Sussex.Flip.Utils;

namespace Sussex.Flip.UI
{
	/// <summary>
	/// A slot which can hold a Moveable object.
	/// </summary>
	public abstract class MoveableSlot : UserControl, IDeepCopyable<MoveableSlot>
    {
		#region Fields
		
    	/// <summary>
    	/// Decides whether a given Moveable can fit into this slot.
    	/// </summary>
    	protected Fitter moveableFitter; 
    	
    	#endregion
    	
    	#region Properties
    	
    	/// <summary>
    	/// The Moveable held by this slot.
    	/// </summary>
        public abstract Moveable Contents { get; set; }
        
        
    	/// <summary>
    	/// Decides whether a given Moveable can fit into this slot.
    	/// </summary>
		public Fitter MoveableFitter {
			get { return moveableFitter; }
			set { moveableFitter = value; }
		}
        
    	#endregion
    	
    	#region Constructors
        
		/// <summary>
		/// Constructs a new <see cref="MoveableSlot"/> instance.
		/// </summary>
		/// <param name="fitter">A fitter which decides whether a 
		/// given Moveable can fit into this slot.</param>
        public MoveableSlot(Fitter fitter)
        {            
            moveableFitter = fitter;
            
            PreviewDrop += ReplaceSlotContents;
            DragEnter += HandleDragEnter;
            DragLeave += HandleDragLeave;
        }
        
        #endregion
        
        #region Methods
        
        /// <summary>
        /// Change the appearance of the control to indicate
        /// that it will accept a drop.
        /// </summary>
        protected abstract void SetSlottableAppearance();
        
        
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
            	if (moveable != null && moveable != Contents && Fits(moveable)) {
            		SetSlottableAppearance();
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
        protected virtual void ReplaceSlotContents(object sender, DragEventArgs e)
        {
        	if (!e.Handled) {
        		if (e.Data.GetDataPresent(typeof(Moveable))) {
					Moveable moveable = e.Data.GetData(typeof(Moveable)) as Moveable;
					if (moveable != null && moveable != Contents && Fits(moveable)) {
						
						if (e.AllowedEffects == DragDropEffects.Copy) {
							Contents = (Moveable)moveable.DeepCopy();
						}
						else if (e.AllowedEffects == DragDropEffects.Move) {
							moveable.Remove();
							Contents = moveable;
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
        public bool Fits(Moveable moveable)
        {
        	return moveableFitter.Fits(moveable);
        }
        
        
		/// <summary>
		/// Gets a deep copy of this instance.
		/// </summary>
		/// <returns>A deep copy of this instance.</returns>
        public abstract MoveableSlot DeepCopy();
        
        #endregion
    }
}
