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

namespace Sussex.Flip.UI
{
    /// <summary>
    /// Interaction logic for ObjectSlot.xaml
    /// </summary>

    public partial class ObjectSlot : UserControl
    {
    	protected static Fitter defaultFitter = new SimpleFitter();
    	
    	
    	protected string slotName;    	
		public string SlotName {
			get { return slotName; }
		}
    	
    	
    	protected Fitter objectFitter;    	
		public Fitter ObjectFitter {
			get { return objectFitter; }
		}
        
        
        public ObjectSlot(string name) : this(name,defaultFitter)
        {        	
        }
    	
    	
        public ObjectSlot(string name, Fitter fitter)
        {
            InitializeComponent();
            
            slotName = name;
            objectFitter = fitter;
            
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
        
        
        Thickness thin = new Thickness(0.5);
        Thickness thick = new Thickness(1);
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
					ObjectBlock block = e.Data.GetData(typeof(Moveable)) as ObjectBlock;
					if (block != null && Fits(block)) {
						if (e.AllowedEffects == DragDropEffects.Copy) {
							Attached = (ObjectBlock)block.Clone();
						}
						else if (e.AllowedEffects == DragDropEffects.Move) {
							Attached = block;
						}
					}
					e.Handled = true;
        		}
			}
			SetToStandardAppearance();
        }
        
        
        public ObjectBlock Attached {
        	get {
        		return slotBorder.Child as ObjectBlock;
        	}
        	set {
        		if (value != Attached) {
        			if (value != null) value.Remove();
        			slotBorder.Child = value;
        		}
        	}
        }
        
        
        
        public bool Fits(ObjectBlock block)
        {
        	return objectFitter.Fits(block);
        }
        
        
        public ObjectSlot Clone()
        {
        	return new ObjectSlot(slotName,objectFitter);
        }
    }
}