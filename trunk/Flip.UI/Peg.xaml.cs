﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Effects;

namespace Sussex.Flip.UI
{
    /// <summary>
    /// Interaction logic for Peg.xaml
    /// </summary>

    public partial class Peg : UserControl
    {  	
    	protected RadialGradientBrush canDropBrush;
    	protected Brush noFeedbackBrush;
    	protected DropShadowEffect glow;
    	
    	
        public Peg()
        {        	
        	canDropBrush = new RadialGradientBrush(Colors.Blue,Colors.LightBlue);
        	noFeedbackBrush = Brushes.Transparent;
        	
            InitializeComponent();
                   	
            DragEnter += delegate(object sender, DragEventArgs e) 
            {  
            	if (!e.Handled) {
            		if (e.Data.GetDataPresent(typeof(Moveable))) {
            			Moveable moveable = (Moveable)e.Data.GetData(typeof(Moveable));
            			if (moveable != GetSlotContents() && Fits(moveable)) {
            				SetToCanDropAppearance();
            			}
            		}
            	}
            };
            DragLeave += delegate(object sender, DragEventArgs e) 
            {  
            	SetToStandardAppearance();
            };
        }

        
        protected void DroppedOnPegSpace(object sender, DragEventArgs e)
        {
        	if (!e.Handled) {
        		if (e.Data.GetDataPresent(typeof(Moveable))) {
        			Moveable moveable = (Moveable)e.Data.GetData(typeof(Moveable));
					if (Fits(moveable)) {
						if (e.AllowedEffects == DragDropEffects.Copy) SetSlotContents(moveable.Clone());
						else if (e.AllowedEffects == DragDropEffects.Move) SetSlotContents(moveable);
					}
					e.Handled = true;
        		}
			}
			SetToStandardAppearance();
        }
        
        
		public Peg Clone()
		{
			Peg peg = new Peg();		
			return peg;
		}
		
		
        protected void SetToCanDropAppearance()
        {
        	border.Background = canDropBrush;
        }
        
        
        protected void SetToStandardAppearance()
        {
        	border.Background = noFeedbackBrush;
        }
        
        
        public void SetSlotContents(Moveable moveable)
        {    	
        	if (moveable == null || GetSlotContents() == moveable) return;
        	moveable.Remove();
        	border.Child = moveable;
        }
        
        
        public Moveable GetSlotContents()
        {
        	if (border.Child == null) return null;
        	else return (Moveable)border.Child;
        }
        
        
        public bool Fits(Moveable moveable)
        {
        	return moveable is Statement;
        }
    }
}