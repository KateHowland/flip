using System;
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
using Sussex.Flip.Utils;

namespace Sussex.Flip.UI
{
    /// <summary>
    /// Interaction logic for Peg.xaml
    /// </summary>

    public partial class Peg : UserControl, IDeepCopyable<Peg>
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
            			if (moveable != Attached && Fits(moveable)) {
            				SetToCanDropAppearance();
            			}
            		}
            	}
            };
            DragLeave += delegate(object sender, DragEventArgs e) 
            {  
            	SetToStandardAppearance();
            };
            
            // TODO:
            // including this directly in XAML frequently
            // causes a 'cannot find file' exception - why?
            DropZone dropZone = new DropZone();
            Grid.SetRow(dropZone,1);
            Grid.SetColumn(dropZone,0);
            Grid.SetColumnSpan(dropZone,2);
            mainGrid.Children.Add(dropZone);
        }

        
        protected void DroppedOnPegSpace(object sender, DragEventArgs e)
        {
        	if (!e.Handled) {
        		if (e.Data.GetDataPresent(typeof(Moveable))) {
        			Moveable moveable = (Moveable)e.Data.GetData(typeof(Moveable));
					if (Fits(moveable)) {
        				if (e.AllowedEffects == DragDropEffects.Copy) {
        					Attached = moveable.DeepCopy();
        				}
        				else if (e.AllowedEffects == DragDropEffects.Move) {
        					Attached = moveable;
        				}
					}
					e.Handled = true;
        		}
			}
			SetToStandardAppearance();
        }
        
        
		public Peg DeepCopy()
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
        
        
        public Moveable Attached {
        	get {
        		return border.Child as Moveable;
        	}
        	set {
        		if (value != Attached) {
        			if (value != null) value.Remove();
        			border.Child = value;
        		}
        	}
        }
        
        
        public bool Fits(Moveable moveable)
        {
        	return moveable is Statement;
        }
    }
}