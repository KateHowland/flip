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
        public Statement()
        {
            InitializeComponent(); 
        }

        
        public void AddSlot(ObjectBlockSlot slot)
        {
        	MainPanel.Children.Add(slot);
        }

        
        public void AddText(StatementLabel label)
        {
        	MainPanel.Children.Add(label);
        }
		
		
		public override string ToString()
		{
			System.Text.StringBuilder sb = new StringBuilder();
			int count = MainPanel.Children.Count;
			for (int i = 0; i < count; i++) {
				ObjectBlockSlot slot = MainPanel.Children[i] as ObjectBlockSlot;
				StatementLabel label = MainPanel.Children[i] as StatementLabel;
				if (slot != null) sb.Append(slot.ToString());
				else if (label != null) sb.Append(label.ToString());
				else sb.Append("?");				
				if (i < count - 1) sb.Append(" ");
			}
			return sb.ToString();
		}
		
		
		public override Moveable DeepCopy()
		{
			Statement statement = new Statement();		
			
			foreach (UIElement e in MainPanel.Children) {
				if (e is StatementLabel) {
					StatementLabel label = (StatementLabel)e;
					StatementLabel labelClone = label.DeepCopy();
					statement.AddText(labelClone);
				}
				else if (e is ObjectBlockSlot) {
					ObjectBlockSlot slot = (ObjectBlockSlot)e;
					ObjectBlockSlot slotClone = (ObjectBlockSlot)slot.DeepCopy();
					statement.AddSlot(slotClone);
					
					if (slot.Contents != null) {
						slotClone.Contents = (ObjectBlock)slot.Contents.DeepCopy();
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