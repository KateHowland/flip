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
using System.Windows.Media.Effects;
using Sussex.Flip.Utils;

namespace Sussex.Flip.UI
{
    /// <summary>
    /// Interaction logic for Peg.xaml
    /// </summary>

    public partial class Peg : UserControl
    {  	    	
        public Peg()
        {       
            InitializeComponent();
            
            // TODO:
            // including this directly in XAML frequently
            // causes a 'cannot find file' exception - why?
            DropZone dropZone = new DropZone();
            Grid.SetRow(dropZone,1);
            Grid.SetColumn(dropZone,0);
            Grid.SetColumnSpan(dropZone,2);
            mainGrid.Children.Add(dropZone);
            
            // TODO:
            // including this directly in XAML frequently
            // causes a 'cannot find file' exception - why?
            PegSlot slot = new PegSlot(new StatementFitter());
            slot.Height = 70;
            slot.Width = 130;
            Grid.SetRow(slot,0);
            Grid.SetColumn(slot,1);
            mainGrid.Children.Add(slot);
        }
    }
}