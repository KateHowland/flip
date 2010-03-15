﻿using System;
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
        DoubleAnimation grow;
        DoubleAnimation shrink;
    	
    	
        public DropZone()
        {
            InitializeComponent();
            grow = new DoubleAnimation(30,100,new Duration(new TimeSpan(0,0,1)));
        	shrink = new DoubleAnimation(100,30,new Duration(new TimeSpan(0,0,1)));
        }

        
        protected void DropZoneDragEnter(object sender, DragEventArgs e)
        {
            grid.BeginAnimation(HeightProperty,grow);
        }

	
        protected void DropZoneDragLeave(object sender, DragEventArgs e)
        {
            grid.BeginAnimation(HeightProperty,shrink);
        }
    }
}