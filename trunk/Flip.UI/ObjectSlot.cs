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

namespace Sussex.Flip.UI
{
    /// <summary>
    /// Interaction logic for ObjectSlot.xaml
    /// </summary>

    public partial class ObjectSlot : UserControl
    {
    	static double width;
    	static double height;
    	static Fitter defaultFitter;
    	
    	static ObjectSlot()
    	{
    		ObjectBlock block = new ObjectBlock(null);
    		width = block.Width;
    		height = block.Height;
    		defaultFitter = new SimpleFitter();
    	}
    	
    	
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
            
            slotBorder.Width = width + slotBorder.BorderThickness.Left + slotBorder.BorderThickness.Right;
            slotBorder.Height = height + slotBorder.BorderThickness.Top + slotBorder.BorderThickness.Bottom;
            
            Drop += new DragEventHandler(DroppedOnSlotPanel);
        }
        

        private void DroppedOnSlotPanel(object sender, DragEventArgs e)
        {
			IDataObject data = e.Data;
			if (data.GetDataPresent(typeof(Moveable))) {
				ObjectBlock block = data.GetData(typeof(Moveable)) as ObjectBlock;
				if (block != null && Fits(block)) {
					SetSlotContents(block);
				}
			}
        }
        
        
        public void SetSlotContents(ObjectBlock block)
        {    	
        	if (GetSlotContents() == block) return;
        	
        	if (block != null) {
        		block.Detach();
        	}
        	
        	slotBorder.Child = block;
        }
        
        
        public ObjectBlock GetSlotContents()
        {
        	if (slotBorder.Child == null) return null;
        	else return (ObjectBlock)slotBorder.Child;
        }
        
        
        public bool Fits(ObjectBlock block)
        {
        	return objectFitter.Fits(block);
        }
    }
}