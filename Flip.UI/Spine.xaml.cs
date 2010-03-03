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
    /// Interaction logic for Spine.xaml
    /// </summary>

    public partial class Spine : UserControl
    {
        public Spine(int pegs)
        {
            InitializeComponent();
            for (int i = 0; i < pegs; i++) AddPeg();
        }
        
        
        public Spine() : this(1)
        {        	
        }

        
        public void AddPeg()
        {
        	Peg peg = new Peg();
        	peg.Margin = new Thickness(0,10,0,10);
        	pegsPanel.Children.Add(peg);
        }
    }
}