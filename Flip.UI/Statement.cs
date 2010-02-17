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
    /// Interaction logic for Statement.xaml
    /// </summary>

    public partial class Statement : Moveable
    {
		private FontFamily font = new FontFamily("Times New Roman");
		private Brush pink = new LinearGradientBrush(Colors.Pink,Colors.Salmon,45);
		
		
        public Statement()
        {
            InitializeComponent();
        }
        
        
        public void AddChild(UIElement element)
        {
        	if (element == null) throw new ArgumentNullException("element");
        	MainPanel.Children.Add(element);
        }
        
        
        public void InsertChild(UIElement element, int index)
        {
        	if (element == null) throw new ArgumentNullException("element");
        	MainPanel.Children.Insert(index,element);
        }
        
        
        
        
        
        
        

        
        public void AddObjectSlot(string name, int index)
        {
        	ObjectSlot slot = new ObjectSlot();
        	slot.Name = name;
        	MainPanel.Children.Insert(index,slot);
        }

        
        public void AddObjectSlot(string name)
        {
        	ObjectSlot slot = new ObjectSlot();
        	slot.Name = name;
        	MainPanel.Children.Add(slot);
        }
        
        
        public void AddTextBar(string text, int index)
        {
        	TextBlock tb = CreateTextBlock(text,pink);
        	MainPanel.Children.Insert(index,tb);
        }

        
        public void AddTextBar(string text)
        {
        	TextBlock tb = CreateTextBlock(text,pink);
        	MainPanel.Children.Add(tb);
        }
        
        
        public ObjectBlock GetSlotContents(string name)
        {
        	ObjectSlot slot = GetSlotPanel(name);        	
        	if (slot == null) throw new ArgumentException("No slot named '" + name + "'.","name");
        	
        	return slot.GetSlotContents();
        }
        
        
        public void SetSlotContents(string name, ObjectBlock block)
        {
        	ObjectSlot slot = GetSlotPanel(name);        	
        	if (slot == null) throw new ArgumentException("No slot named '" + name + "'.","name");
        	
        	slot.SetSlotContents(block);
        }
        
                
        private ObjectSlot GetSlotPanel(string name)
        {
        	object o = FindName(name);
        	if (o is ObjectSlot) return (ObjectSlot)o;
        	else return null;
        }	
		
				
		public TextBlock CreateTextBlock(string text, Brush background)
		{
			TextBlock tb = new TextBlock();
			tb.Text = text;
			tb.FontSize = 16;
			tb.FontFamily = font; 
			tb.FontWeight = FontWeights.Bold;
			tb.Foreground = Brushes.Black;
			tb.Background = background;
			tb.TextAlignment = TextAlignment.Center;
			tb.VerticalAlignment = VerticalAlignment.Center;
			tb.HorizontalAlignment = HorizontalAlignment.Center;
			tb.Padding = new Thickness(10);
			tb.Width = 80;
			tb.Height = 35;
			return tb;
		}
    }
}