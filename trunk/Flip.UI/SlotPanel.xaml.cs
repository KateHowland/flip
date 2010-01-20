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
    /// Interaction logic for SlotPanel.xaml
    /// </summary>

    public partial class SlotPanel : UserControl
    {
        public SlotPanel()
        {
            InitializeComponent();            
            Drop += new DragEventHandler(DroppedOnSlotPanel);
        }
        

        private void DroppedOnSlotPanel(object sender, DragEventArgs e)
        {
			IDataObject data = e.Data;
			if (data.GetDataPresent(typeof(Moveable))) {
				Moveable moveable = (Moveable)data.GetData(typeof(Moveable));
				if (moveable is SimpleBlock) {
					SetSlotContents((SimpleBlock)moveable);
				}
			}
        }

        
        public SimpleBlock EmptySlot()
        {
        	SimpleBlock removing = (SimpleBlock)SlotBorder.Child;
        	SlotBorder.Child = null;
        	return removing;
        }
        
        
        public void SetSlotContents(SimpleBlock block)
        {    	
        	if (GetSlotContents() == block) return;
        	
        	if (block != null) {
        		block.Detach();
        	}
        	
        	SlotBorder.Child = block;
        }
        
        
        public SimpleBlock GetSlotContents()
        {
        	if (SlotBorder.Child == null) return null;
        	else return (SimpleBlock)SlotBorder.Child;
        }
    }
}