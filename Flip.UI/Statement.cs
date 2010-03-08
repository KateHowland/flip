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
		protected bool tested = false;
		
		
        public Statement()
        {
            InitializeComponent(); 
        }

        
        public void AddSlot(ObjectSlot slot)
        {
        	MainPanel.Children.Add(slot);          
            ToolTip = ToString();
        }

        
        public void AddTextBar(string text)
        {
        	TextBlock tb = CreateTextBlock(text,pink);
        	MainPanel.Children.Add(tb);          
            ToolTip = ToString();
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
		
		
		public override string ToString()
		{
			System.Text.StringBuilder sb = new StringBuilder();
			int count = MainPanel.Children.Count;
			for (int i = 0; i < count; i++) {
				ObjectSlot slot = MainPanel.Children[i] as ObjectSlot;
				TextBlock text = MainPanel.Children[i] as TextBlock;
				if (slot != null) sb.Append(slot.SlotName);
				else if (text != null) sb.Append(text.Text);
				else sb.Append("?");				
				if (i < count - 1) sb.Append(" ");
			}
			return sb.ToString();
		}
		
		
		public override Moveable Clone()
		{
			Statement statement = new Statement();		
			
			foreach (UIElement e in MainPanel.Children) {
				if (e is TextBlock) {
					TextBlock tb = (TextBlock)e;
					statement.AddTextBar(tb.Text);
				}
				else if (e is ObjectSlot) {
					ObjectSlot slot = (ObjectSlot)e;
					ObjectSlot slotClone = slot.Clone();
					statement.AddSlot(slotClone);
					
					if (slot.Attached != null) {
						slotClone.Attached = (ObjectBlock)slot.Attached.Clone();
					}
				}
				else {
					throw new InvalidOperationException("Didn't recognise type '" + e.GetType() + "' when cloning Statement.");
				}
			}
			
			return statement;
		}
    }
}