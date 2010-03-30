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
 * This file added by Keiron Nicholson on 29/03/2010 at 16:13.
 */

using System;
using System.Windows;
using System.Windows.Media;

namespace Sussex.Flip.UI
{
	public class ConditionStatement: Statement
	{
    	protected new static Brush brush;
    	
    	
    	static ConditionStatement()
    	{
    		GradientStopCollection stops = new GradientStopCollection(3);
    		stops.Add(new GradientStop(Colors.Red,-0.5));
    		stops.Add(new GradientStop(Colors.White,0.5));
    		stops.Add(new GradientStop(Colors.Red,1.5));
    		brush = new LinearGradientBrush(stops,new Point(0,0),new Point(1,1));
    	}
        
        
        protected override Brush GetBrush()
        {
        	return brush;
        }
    	
    	
		public ConditionStatement() : base()
		{
		}
		
		
		public override Moveable DeepCopy()
		{
			ConditionStatement statement = new ConditionStatement();		
			
			foreach (UIElement e in mainPanel.Children) {
				if (e is StatementLabel) {
					StatementLabel label = (StatementLabel)e;
					StatementLabel labelClone = label.DeepCopy();
					statement.AddLabel(labelClone);
				}
				else if (e is ObjectBlockSlot) {
					ObjectBlockSlot slot = (ObjectBlockSlot)e;
					ObjectBlockSlot slotClone = (ObjectBlockSlot)slot.DeepCopy();
					statement.AddSlot(slotClone);
					// TODO: think I kept this separate from ObjectBlockSlot.DeepCopy()
					// for a reason, but not sure what it was..?:
					if (slot.Contents != null) {
						slotClone.Contents = (ObjectBlock)slot.Contents.DeepCopy();
					}
				}
				else {
					throw new InvalidOperationException("Didn't recognise type '" + e.GetType() + "' when cloning ConditionStatement.");
				}
			}
			
			return statement;
		}
	}
}
