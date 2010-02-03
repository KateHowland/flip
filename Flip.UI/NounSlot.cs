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
    /// Interaction logic for NounSlot.xaml
    /// </summary>

    public partial class NounSlot : UserControl
    {
        public NounSlot()
        {
            InitializeComponent();    
            
            Noun noun = new Noun(); // used to check size        
            SlotBorder.Width = noun.Width + SlotBorder.BorderThickness.Left + SlotBorder.BorderThickness.Right;
            SlotBorder.Height = noun.Height + SlotBorder.BorderThickness.Top + SlotBorder.BorderThickness.Bottom;
            
            Drop += new DragEventHandler(DroppedOnSlotPanel);
        }
        

        private void DroppedOnSlotPanel(object sender, DragEventArgs e)
        {
			IDataObject data = e.Data;
			if (data.GetDataPresent(typeof(Moveable))) {
				Moveable moveable = (Moveable)data.GetData(typeof(Moveable));
				if (moveable is Noun) {
					SetSlotContents((Noun)moveable);
				}
			}
        }

        
        public Noun EmptySlot()
        {
        	Noun block = (Noun)SlotBorder.Child;
        	SlotBorder.Child = null;
        	return block;
        }
        
        
        public void SetSlotContents(Noun block)
        {    	
        	if (GetSlotContents() == block) return;
        	
        	if (block != null) {
        		block.Detach();
        	}
        	
        	SlotBorder.Child = block;
        }
        
        
        public Noun GetSlotContents()
        {
        	if (SlotBorder.Child == null) return null;
        	else return (Noun)SlotBorder.Child;
        }
    }
}