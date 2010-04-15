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
    /// Interaction logic for IfControl.xaml
    /// </summary>

    public partial class IfControl : ConditionalControl
    {
    	protected ConditionSlot slot;
    	protected Spine spine;
    	
    	
        public IfControl()
        {
        	slot = new ConditionSlot(new ConditionFitter());
        	slot.Padding = new Thickness(10);
        	
        	spine = new Spine(new SpineFitter(),1,10);
        	
        	slot.Changed += delegate 
        	{ 
        		OnChanged(new EventArgs());
        	};
        	
        	spine.Changed += delegate 
        	{ 
        		OnChanged(new EventArgs()); 
        	};
            
            InitializeComponent();
            
            Grid.SetRow(slot,1);
            Grid.SetColumn(slot,0);
            grid.Children.Add(slot);
                        
            Grid.SetRow(spine,3);
            Grid.SetColumn(spine,0);
            grid.Children.Add(spine);
        }

        
		public override Moveable DeepCopy()
		{
			IfControl copy = new IfControl();
			return copy;
		}
    	
        
		public override string GetCode()
		{
			System.Text.StringBuilder code = new System.Text.StringBuilder();
			
			code.AppendLine(String.Format("if ({0}) {{",slot.GetCode()));
			code.AppendLine(spine.GetCode());			
			code.AppendLine("}");
			
			return code.ToString();
		}
    	
        
		public override string GetNaturalLanguage()
		{
			System.Text.StringBuilder code = new System.Text.StringBuilder();
						
			code.AppendLine(String.Format("if {0},",slot.GetNaturalLanguage()));
			code.AppendLine(spine.GetNaturalLanguage());			
			code.Append(".");
			
			return code.ToString();
		}
    }
}