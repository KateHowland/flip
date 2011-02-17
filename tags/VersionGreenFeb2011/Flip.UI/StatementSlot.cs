using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Sussex.Flip.Utils;

namespace Sussex.Flip.UI
{
    /// <summary>
    /// A slot which TODO
    /// </summary>
    public partial class StatementSlot : UserControl, IDeepCopyable<StatementSlot>
    {
    	protected Thickness thin;
    	protected Thickness thick;
    	
    	
    	protected string slotName;    	
		public string SlotName {
			get { return slotName; }
		}
    	
    	
    	protected Fitter moveableFitter;    	
		public Fitter MoveableFitter {
			get { return moveableFitter; }
		}
        
        
        public StatementSlot(string name) : this(name,defaultFitter)
        {        	
        }
    	
    	
        public StatementSlot(string name, Fitter fitter)
        {
            InitializeComponent();
            
            thin = (Thickness)Resources["thin"];
            thick = (Thickness)Resources["thick"];
            
            slotName = name;
            moveableFitter = fitter;
            
            slotBorder.Width = ObjectBlock.DefaultSize.Width + slotBorder.BorderThickness.Left + slotBorder.BorderThickness.Right;
            slotBorder.Height = ObjectBlock.DefaultSize.Height + slotBorder.BorderThickness.Top + slotBorder.BorderThickness.Bottom;
            
            PreviewDrop += new DragEventHandler(DroppedOnSlotPanel);
            DragEnter += delegate(object sender, DragEventArgs e) 
            {  
            	if (!e.Handled) {
            		ObjectBlock block = e.Data.GetData(typeof(Moveable)) as ObjectBlock;
            		if (block != null && block != Attached && Fits(block)) {
            			SetToCanDropAppearance();
            		}
            	}
            };
            DragLeave += delegate(object sender, DragEventArgs e) 
            {  
            	SetToStandardAppearance();
            };
        }
        
        
        protected void SetToCanDropAppearance()
        {
        	slotBorder.BorderBrush = Brushes.Blue;
        	slotBorder.BorderThickness = thick;
        }
        
        
        protected void SetToStandardAppearance()
        {
        	slotBorder.BorderBrush = Brushes.Black;
        	slotBorder.BorderThickness = thin;
        }
        

        private void DroppedOnSlotPanel(object sender, DragEventArgs e)
        {
        	if (!e.Handled) {
        		if (e.Data.GetDataPresent(typeof(Moveable))) {
					Moveable moveable = e.Data.GetData(typeof(Moveable)) as Moveable;
					if (moveable != null && Fits(moveable)) {
						if (e.AllowedEffects == DragDropEffects.Copy) {
							Attached = (Moveable)moveable.DeepCopy();
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
        
        
        public Moveable Attached {
        	get {
        		return slotBorder.Child as Moveable;
        	}
        	set {
        		if (value != Attached) {
        			if (value != null) value.Remove();
        			slotBorder.Child = value;
        		}
        	}
        }        
        
        
        public bool Fits(Moveable moveable)
        {
        	return moveableFitter.Fits(moveable);
        }
        
        
        public StatementSlot DeepCopy()
        {
        	return new StatementSlot(slotName,moveableFitter);
        }
    }
}