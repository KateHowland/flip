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
    /// Interaction logic for TriggerBar.xaml
    /// </summary>

    public partial class TriggerBar : UserControl
    {
        public TriggerBar()
        {
        	string[] triggers = new string[] {"player sees creature", "player picks up item", "player is attacked", "player dies"};
        	Spine spine = new Spine(5);
        	spine.Name = "spine";
        	Grid.SetRow(spine,0);
        	Grid.SetColumn(spine,0);
        	spine.Margin = new Thickness(14,0,0,0);
        	spine.Extends = 100;
        	
            InitializeComponent();
        	
        	Grid.SetZIndex(spine,1);
        	Grid.SetZIndex(border,2);
            
            mainGrid.Children.Add(spine);
            triggersComboBox.ItemsSource = triggers;
            
            Effect = new System.Windows.Media.Effects.DropShadowEffect();
        }
    }
}