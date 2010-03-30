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
    }
}