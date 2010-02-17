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
    	static ObjectSlot()
    	{
    		ObjectBlock block = new ObjectBlock(null);
    		width = block.Width;
    		height = block.Height;
    	}
    	
    	
        public ObjectSlot()
        {
            InitializeComponent();    
            
            SlotBorder.Width = width + SlotBorder.BorderThickness.Left + SlotBorder.BorderThickness.Right;
            SlotBorder.Height = height + SlotBorder.BorderThickness.Top + SlotBorder.BorderThickness.Bottom;
            
            Drop += new DragEventHandler(DroppedOnSlotPanel);
        }
        

        private void DroppedOnSlotPanel(object sender, DragEventArgs e)
        {
			IDataObject data = e.Data;
			if (data.GetDataPresent(typeof(Moveable))) {
				ObjectBlock block = data.GetData(typeof(Moveable)) as ObjectBlock;
				if (block != null) {
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
        	
        	SlotBorder.Child = block;
        }
        
        
        public ObjectBlock GetSlotContents()
        {
        	if (SlotBorder.Child == null) return null;
        	else return (ObjectBlock)SlotBorder.Child;
        }
    }
}