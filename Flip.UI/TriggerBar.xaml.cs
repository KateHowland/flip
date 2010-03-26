using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Sussex.Flip.UI
{
    /// <summary>
    /// Interaction logic for TriggerBar.xaml
    /// </summary>

    public partial class TriggerBar : UserControl
    {
    	protected TriggerControl triggerControl;
    	
    	
        public TriggerBar(TriggerControl triggerControl)
        {
        	if (triggerControl == null) throw new ArgumentNullException("triggerControl");
        	
        	Spine spine = new Spine(3);
        	Grid.SetRow(spine,0);
        	Grid.SetColumn(spine,0);
        	spine.Margin = new Thickness(14,0,0,0);
        	
            InitializeComponent();
            
            spine.Extends = border.Height + 20;        	
        	Grid.SetZIndex(spine,1);
        	Grid.SetZIndex(border,2);            
            mainGrid.Children.Add(spine);
            
            Effect = new DropShadowEffect();
            
            this.triggerControl = triggerControl;
            triggerBarPanel.Children.Add(triggerControl);
        }
        
        
        public string GetEvent()
        {
        	if (triggerControl.EventBlock == null) return null;
        	else return triggerControl.EventBlock.EventName;
        }
    }
}