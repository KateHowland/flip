using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Sussex.Flip.Utils;

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
        	animationTime = new Duration(new TimeSpan(1000000));
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
        	
        	peg.DropZone.Drop += new DragEventHandler(Expand);
        	
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

        
        /// <summary>
        /// When a Moveable is dropped under a peg, create a new peg
        /// underneath that peg, and attach the Moveable to it.
        /// </summary>
        protected void Expand(object sender, DragEventArgs e)
        {	
		    if (!e.Handled) {
	        	
		       	if (e.Data.GetDataPresent(typeof(Moveable))) {        		
	        		
					Moveable moveable = e.Data.GetData(typeof(Moveable)) as Moveable;
					
					if (!(e.AllowedEffects == DragDropEffects.Copy || e.AllowedEffects == DragDropEffects.Move)) {
						return;
					}
					
					if (tempSharedFitter.Fits(moveable)) {
						
			        	DropZone dropZone = (DropZone)sender;
			        	if (dropZone == null) return;
			        	
			        	Peg dropPeg = UIHelper.TryFindParent<Peg>(dropZone);        		
			        	if (dropPeg == null) return;
			        	
			        	int index = Pegs.IndexOf(dropPeg);
			        	if (index == -1) throw new InvalidOperationException("Peg not found on spine.");
			        	index++;
			        	
			        	Peg pegToUse = null;
			        	
			        	// If there's an empty peg below the drop zone, use that, otherwise create a new one:
			        	if (index <= Pegs.Count && ((Peg)Pegs[index]).Slot.Contents == null) {
			        		pegToUse = (Peg)Pegs[index];
			        	}
			        	else {
			        		pegToUse = AddPeg(false,index);
			        	}
			        	
						if (e.AllowedEffects == DragDropEffects.Copy) {
				      		pegToUse.Slot.Contents = moveable.DeepCopy();
						}
						else if (e.AllowedEffects == DragDropEffects.Move) {
							moveable.Remove();
				      		pegToUse.Slot.Contents = moveable;
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