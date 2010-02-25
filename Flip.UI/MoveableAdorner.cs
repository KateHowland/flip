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
 * This file added by Keiron Nicholson on 24/02/2010 at 09:54.
 */

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Sussex.Flip.UI
{
	/// <summary>
	/// Description of MoveableAdorner.
	/// </summary>
	public class MoveableAdorner : Adorner
	{
		protected Moveable elementToShow;
		protected Point position;
		
		
		public MoveableAdorner(Moveable moveable) : base(moveable)
		{			
			elementToShow = moveable.Clone();
			elementToShow.Opacity = 0.8;
			
			position = new Point(0,0);
		}		
		
		
		protected override Size ArrangeOverride(Size finalSize)
		{
			elementToShow.Arrange(new Rect(finalSize));
			return finalSize;
		}
		
		
		protected override Size MeasureOverride(Size constraint)
		{
			elementToShow.Measure(constraint);
			return constraint;
		}
		
		
		protected override Visual GetVisualChild(int index)
		{
			return elementToShow;
		}
		
		
		protected override int VisualChildrenCount {
			get { return 1; }
		}
		
		
		public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
		{
			GeneralTransformGroup grp = new GeneralTransformGroup();
			//grp.Children.Add(transform);
			grp.Children.Add(new TranslateTransform((0-(RenderSize.Width/2)),(0-(RenderSize.Height/2))));
			grp.Children.Add(new TranslateTransform(position.X,position.Y));
			return grp;
		}
		
		
		public void UpdatePosition(Point p)
		{
			position = p;
			AdornerLayer parentLayer = Parent as AdornerLayer;
			if (parentLayer != null) {
				parentLayer.Update(AdornedElement);
			}
		}
	}
}
