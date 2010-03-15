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
            try {
            	InitializeComponent();
            
            	grow = new DoubleAnimation(30,100,new Duration(new TimeSpan(1000)));
        		shrink = new DoubleAnimation(100,30,new Duration(new TimeSpan(1000)));
            }
        	catch (Exception e) {
        		System.Windows.MessageBox.Show(e.ToString());
        	}
        }

        
        DoubleAnimation grow;
        DoubleAnimation shrink;
        protected void DropZoneDragEnter(object sender, DragEventArgs e)
        {
            try {
        		grid.BeginAnimation(HeightProperty,grow);
            }
        	catch (Exception x) {
        		System.Windows.MessageBox.Show(x.ToString());
        	}
        }

	
        protected void DropZoneDragLeave(object sender, DragEventArgs e)
        {
            try {
        		grid.BeginAnimation(HeightProperty,shrink);
            }
        	catch (Exception x) {
        		System.Windows.MessageBox.Show(x.ToString());
        	}
        }
    }
}