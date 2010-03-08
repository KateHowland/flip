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
		private static FontFamily font = new FontFamily("Helvetica");
		protected static Thickness thickness10 = new Thickness(10);
		protected static Thickness thickness0point5 = new Thickness(0.5);
		
		
        public Statement()
        {
            InitializeComponent(); 
        }

        
        public void AddSlot(StatementSlot slot)
        {
        	MainPanel.Children.Add(slot);          
            ToolTip = ToString();
        }

        
        public void AddText(StatementLabel label)
        {
        	MainPanel.Children.Add(label);          
            ToolTip = ToString();
        }
		
		
		public override string ToString()
		{
			System.Text.StringBuilder sb = new StringBuilder();
			int count = MainPanel.Children.Count;
			for (int i = 0; i < count; i++) {
				StatementSlot slot = MainPanel.Children[i] as StatementSlot;
				StatementLabel label = MainPanel.Children[i] as StatementLabel;
				if (slot != null) sb.Append(slot.SlotName);
				else if (label != null) sb.Append(label.Text);
				else sb.Append("?");				
				if (i < count - 1) sb.Append(" ");
			}
			return sb.ToString();
		}
		
		
		public override Moveable Clone()
		{
			Statement statement = new Statement();		
			
			foreach (UIElement e in MainPanel.Children) {
				if (e is StatementLabel) {
					StatementLabel label = (StatementLabel)e;
					StatementLabel labelClone = label.Clone();
					statement.AddText(labelClone);
				}
				else if (e is StatementSlot) {
					StatementSlot slot = (StatementSlot)e;
					StatementSlot slotClone = slot.Clone();
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