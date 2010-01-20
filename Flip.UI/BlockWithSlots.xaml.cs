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
    /// Interaction logic for BlockWithSlots.xaml
    /// </summary>

    public partial class BlockWithSlots : Moveable
    {
        public BlockWithSlots()
        {
            InitializeComponent();
        }

        
        public void AddSlot(string name, int index)
        {
        	SlotPanel slot = new SlotPanel();
        	slot.Name = name;
        	MainPanel.Children.Insert(index,slot);
        }

        
        public void AddSlot(string name)
        {
        	SlotPanel slot = new SlotPanel();
        	slot.Name = name;
        	MainPanel.Children.Add(slot);
        }
        
        
        public void AddLabel(Label label, int index)
        {
        	if (label == null) throw new ArgumentNullException("Label was null.","label");
        	
        	MainPanel.Children.Insert(index,label);
        }
        
        
        public void AddLabel(Label label)
        {
        	if (label == null) throw new ArgumentNullException("Label was null.","label");
        	
        	MainPanel.Children.Add(label);
        }
        
        
        public SimpleBlock GetSlotContents(string name)
        {
        	SlotPanel slot = GetSlotPanel(name);        	
        	if (slot == null) throw new ArgumentException("No slot named '" + name + "'.","name");
        	
        	return slot.GetSlotContents();
        }
        
        
        public void SetSlotContents(string name, SimpleBlock block)
        {
        	SlotPanel slot = GetSlotPanel(name);        	
        	if (slot == null) throw new ArgumentException("No slot named '" + name + "'.","name");
        	
        	slot.SetSlotContents(block);
        }
        
                
        private SlotPanel GetSlotPanel(string name)
        {
        	object o = FindName(name);
        	if (o is SlotPanel) return (SlotPanel)o;
        	else return null;
        }
    }
}