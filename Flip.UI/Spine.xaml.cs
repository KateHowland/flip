using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Sussex.Flip.UI
{
    /// <summary>
    /// Interaction logic for Spine.xaml
    /// </summary>

    public partial class Spine : UserControl
    {
    	protected Duration animationTime;
    	    	
    	
    	public Spine(int pegs, double extends) : this(pegs)
    	{
    		Extends = extends;
    	}
    	
    	
        public Spine(int pegs)
        {
        	animationTime = new Duration(new TimeSpan(1000000/2));
            InitializeComponent();
            for (int i = 0; i < pegs; i++) AddPeg();
        }
        
        
        public Spine() : this(3)
        {        	
        }

        
        public Peg AddPeg()
        {
        	return AddPeg(false,Pegs.Count);
        }

        
        public Peg AddPeg(bool animate)
        {
        	return AddPeg(animate,Pegs.Count);
        }
        
        
        //TODO:
        protected Fitter tempSharedFitter = new StatementFitter();
        
        public Peg AddPeg(bool animate, int index)
        {
        	if (index > Pegs.Count) throw new ArgumentException("Index out of range.","index");
        	
        	Peg peg = new Peg();
        	
        	ScaleTransform scale = null;        	
        	if (animate) {
	        	scale = new ScaleTransform(0,0);
	        	peg.LayoutTransform = scale;	        
        	}
        	
        	Pegs.Insert(index,peg);
        	
        	peg.Drop += new DragEventHandler(peg_Drop);
        	
        	// TODO:
        	// temp:
        	peg.MouseDoubleClick += delegate { 
        		if (Keyboard.IsKeyDown(Key.LeftShift)) RemovePeg(true,Pegs.IndexOf(peg));
        		else AddPeg(true,Pegs.IndexOf(peg)+1); 
        	};
        	
        	if (animate) {
	        	DoubleAnimation anim = new DoubleAnimation(0,1,animationTime);
	        	anim.AutoReverse = false;
	        	anim.IsAdditive = false;
        		scale.BeginAnimation(ScaleTransform.ScaleXProperty,anim);
        		scale.BeginAnimation(ScaleTransform.ScaleYProperty,anim);
        	}
        	
        	return peg;
        }

        
        protected void peg_Drop(object sender, DragEventArgs e)
        {   		
	        if (!e.Handled) {
        		
	        	if (e.Data.GetDataPresent(typeof(Moveable))) {
        			
					Moveable moveable = e.Data.GetData(typeof(Moveable)) as Moveable;
					Peg dropPeg = sender as Peg;
					
					if (dropPeg != null && moveable != null && tempSharedFitter.Fits(moveable)) {
						
						int index = this.Pegs.IndexOf(dropPeg);
						if (index == -1) {
							throw new InvalidOperationException("Dropped on Peg which was not attached to this spine.");
						}
												
						if (e.AllowedEffects == DragDropEffects.Copy) {
							Peg newPeg = AddPeg(false,index+1);
		       				newPeg.Slot.Contents = moveable.DeepCopy();
						}
						else if (e.AllowedEffects == DragDropEffects.Move) {
							Peg newPeg = AddPeg(false,index+1);
							moveable.Remove();
		       				newPeg.Slot.Contents = moveable;
						}
						
					}
					e.Handled = true;
	        	}
			}
        }
        
        
        public void RemovePeg()
        {
        	RemovePeg(false);
        }
        
        
        public void RemovePeg(bool animate)
        {
        	if (Pegs.Count == 0) throw new InvalidOperationException("No pegs to remove.");        	
        	RemovePeg(animate,Pegs.Count-1);
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
        }
        
        
        public double Extends {
        	get { return extension.Height.Value; }
        	set { extension.Height = new GridLength(value); }
        }
        
        
        public UIElementCollection Pegs {
        	get { return pegsPanel.Children; }
        }
    }
}