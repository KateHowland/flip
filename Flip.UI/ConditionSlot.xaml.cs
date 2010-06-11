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
 * This file added by Keiron Nicholson on 30/03/2010 at 11:39.
 */

using System;
using System.Windows;
using System.Windows.Media;

namespace Sussex.Flip.UI
{
	/// <summary>
	/// A slot which can hold a Condition.
	/// </summary>
	public partial class ConditionSlot : MoveableSlot
	{
    	#region Constructors
            	
		/// <summary>
		/// Constructs a new <see cref="ConditionSlot"/> instance.
		/// </summary>
		/// <param name="fitter">A fitter which decides whether a 
		/// given Moveable can fit into this slot.</param>
		public ConditionSlot(Fitter fitter) : base(fitter)
        {           
        	InitializeComponent();
        	SetDefaultAppearance();	 
        }
        
        #endregion
        
        #region Methods
        
        /// <summary>
        /// Gets the Moveable held by this slot.
        /// </summary>
        /// <returns>A Moveable, or null if the slot is empty.</returns>
        protected override Moveable GetMoveable()
        {
        	return border.Child as Moveable;
        }
                
        
        /// <summary>
        /// Sets the Moveable held by this slot.
        /// </summary>
        protected override void SetMoveable(Moveable moveable)
        {
        	border.Child = moveable;
        }
        
        
        /// <summary>
        /// Change the appearance of the control to indicate
        /// that it will accept a drop.
        /// </summary>
		protected override void SetDropAppearance()
		{
			border.BorderBrush = dropBrush;
		}
        
        
        /// <summary>
        /// Change the appearance of the control to indicate
        /// that it will not accept a drop.
        /// </summary>
        protected override void SetNoDropAppearance()
		{
        	border.BorderBrush = noDropBrush;
		}
        
                
        /// <summary>
        /// Restore the default appearance of the control.
        /// </summary>
		protected override void SetDefaultAppearance()
		{
			border.BorderBrush = defaultBrush;
		}
        
        
		/// <summary>
		/// Gets a deep copy of this instance.
		/// </summary>
		/// <returns>A deep copy of this instance.</returns>
		public override MoveableSlot DeepCopy()
		{
			ConditionSlot copy = new ConditionSlot(fitter);
			copy.Contents = this.Contents.DeepCopy();
			return copy;
		}
        
        #endregion
	}
}