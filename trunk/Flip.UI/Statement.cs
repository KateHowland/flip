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
    	protected static Brush brush;
    	
    	    	
    	static Statement()
    	{
    		GradientStopCollection stops = new GradientStopCollection(3);
    		stops.Add(new GradientStop(Colors.Gray,-0.5));
    		stops.Add(new GradientStop(Colors.White,0.5));
    		stops.Add(new GradientStop(Colors.Gray,1.5));
    		brush = new LinearGradientBrush(stops,new Point(0,0),new Point(1,1));
    	}
    	    
    	
        public Statement()
        {        	
            InitializeComponent(); 
        }

        
        public void AddSlot(ObjectBlockSlot slot)
        {
        	mainPanel.Children.Add(slot);
        }

        
        public void AddLabel(StatementLabel label)
        {
        	mainPanel.Children.Add(label);
        }

        
        public void AddLabel(string text)
        {
        	StatementLabel label = new StatementLabel(text,GetBrush());
        	mainPanel.Children.Add(label);
        }
        
        
        protected virtual Brush GetBrush()
        {
        	return brush;
        }
		
		
		public override string ToString()
		{
			System.Text.StringBuilder sb = new StringBuilder();
			int count = mainPanel.Children.Count;
			for (int i = 0; i < count; i++) {
				ObjectBlockSlot slot = mainPanel.Children[i] as ObjectBlockSlot;
				StatementLabel label = mainPanel.Children[i] as StatementLabel;
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
			
			foreach (UIElement e in mainPanel.Children) {
				if (e is StatementLabel) {
					StatementLabel label = (StatementLabel)e;
					StatementLabel labelClone = label.DeepCopy();
					statement.AddLabel(labelClone);
				}
				else if (e is ObjectBlockSlot) {
					ObjectBlockSlot slot = (ObjectBlockSlot)e;
					ObjectBlockSlot slotClone = (ObjectBlockSlot)slot.DeepCopy();
					statement.AddSlot(slotClone);
					// TODO: think I kept this separate from ObjectBlockSlot.DeepCopy()
					// for a reason, but not sure what it was..?:
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
		
		
		public List<ObjectBlockSlot> GetSlots()
		{
			List<ObjectBlockSlot> slots = new List<ObjectBlockSlot>(2);
			foreach (UIElement element in mainPanel.Children) {
				ObjectBlockSlot slot = element as ObjectBlockSlot;
				if (slot != null) slots.Add(slot);
			}
			return slots;
		}
		
		
		public override string GetCode()
		{
			System.Text.StringBuilder code = new System.Text.StringBuilder("Statement(");
			
			foreach (ObjectBlockSlot slot in GetSlots()) {
				code.Append(String.Format("{0},",slot.GetCode()));
			}			
			code.Append(")");
			
			return code.ToString();
		}
		
		
		public override string GetNaturalLanguage()
		{
			return GetCode();
		}
    }
}