using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Sussex.Flip.UI
{
    /// <summary>
    /// Interaction logic for DropZone.xaml
    /// </summary>

    public partial class DropZone : UserControl
    {    	
        public DropZone()
        {
            InitializeComponent();
        }

        
        protected void DropZoneDragEnter(object sender, DragEventArgs e)
        {   	
        	grid.Background = Brushes.Blue;
        }

	
        protected void DropZoneDragLeave(object sender, DragEventArgs e)
        {
        	grid.Background = Brushes.Transparent;
        }
    }
}