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
		protected VisualCollection visuals;
		protected ContentPresenter contentPresenter;
		protected AdornerLayer layer;
		protected Point position = new Point(0,0);
		
		
		public Point Position {
			get { return position; }
			set { 
				position = value; 
				layer.Update(AdornedElement);
			}
		}
		
		
		public MoveableAdorner(Moveable moveable, AdornerLayer layer, Point position) : base(moveable)
		{		
			this.layer = layer;
			this.position = position;
			
			visuals = new VisualCollection(this);
			contentPresenter = new ContentPresenter();
			visuals.Add(contentPresenter);
						
			Moveable clone = moveable.DeepCopy();
			clone.Opacity = 0.7;
			Content = clone;
			
			layer.Add(this);	
			
			IsHitTestVisible = false;						
			AllowDrop = true;
			
			AdornedElement.IsHitTestVisible = false;
		}
		
		
		protected override Size MeasureOverride(Size constraint)
		{
			contentPresenter.Measure(constraint);
			return contentPresenter.DesiredSize;
		}
		
		
		protected override Size ArrangeOverride(Size finalSize)
		{
			contentPresenter.Arrange(new Rect(0,0,finalSize.Width,finalSize.Height));
			return contentPresenter.RenderSize;
		}
		
		
		protected override Visual GetVisualChild(int index)
		{
			return visuals[index];
		}
		
		
		protected override int VisualChildrenCount {
			get { return visuals.Count; }
		}
		
		
		public object Content {
			get { return contentPresenter.Content; }
			set { contentPresenter.Content = value; }
		}
		
		
		public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
		{
			GeneralTransformGroup grp = new GeneralTransformGroup();
			//grp.Children.Add(transform);
			grp.Children.Add(new TranslateTransform((0-(RenderSize.Width/2)),(0-(RenderSize.Height/2))));
			grp.Children.Add(new TranslateTransform(position.X,position.Y));
			return grp;
		}
		
		
		public void Destroy()
		{
			layer.Remove(this);
			AdornedElement.IsHitTestVisible = true;			
		}
	}
}