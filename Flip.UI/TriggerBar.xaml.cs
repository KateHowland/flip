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
    	protected Spine spine;
        
        
        public Spine Spine {
        	get { return spine; }
        }
    	
    	
        public TriggerBar()
        {
        	string[] triggers = new string[] {"player sees creature", "player picks up item", "player is attacked", "player dies"};
        	spine = new Spine(3);
        	Grid.SetRow(spine,0);
        	Grid.SetColumn(spine,0);
        	spine.Margin = new Thickness(14,0,0,0);
        	
            InitializeComponent();
            
            spine.Extends = border.Height + 20;
        	
        	Grid.SetZIndex(spine,1);
        	Grid.SetZIndex(border,2);
            
            mainGrid.Children.Add(spine);
            
            Effect = new DropShadowEffect();
            
            TriggerSlot triggerSlot = new TriggerSlot();
            triggerBarPanel.Children.Add(triggerSlot);
        }
    }
}