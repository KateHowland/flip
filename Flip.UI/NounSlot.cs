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
    	static double nounWidth;
    	static double nounHeight;
    	static NounSlot()
    	{
    		Noun noun = new Noun(null);
    		nounWidth = noun.Width;
    		nounHeight = noun.Height;
    	}
    	
    	
        public NounSlot()
        {
            InitializeComponent();    
            
            SlotBorder.Width = nounWidth + SlotBorder.BorderThickness.Left + SlotBorder.BorderThickness.Right;
            SlotBorder.Height = nounHeight + SlotBorder.BorderThickness.Top + SlotBorder.BorderThickness.Bottom;
            
            Drop += new DragEventHandler(DroppedOnSlotPanel);
        }
        

        private void DroppedOnSlotPanel(object sender, DragEventArgs e)
        {
			IDataObject data = e.Data;
			if (data.GetDataPresent(typeof(Moveable))) {
				Noun noun = data.GetData(typeof(Moveable)) as Noun;
				if (noun != null) {
					SetSlotContents(noun);
				}
			}
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