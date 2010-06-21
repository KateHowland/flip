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
using System.Windows.Shapes;


namespace Sussex.Flip.Utils
{
    /// <summary>
    /// Interaction logic for ActivityLogWindow.xaml
    /// </summary>

    public partial class ActivityLogWindow : Window
    {
        public ActivityLogWindow()
        {
            InitializeComponent();
            
            ActivityLog.MessageWritten += delegate(object sender, ActivityEventArgs e) 
            {  
            	logTextBlock.Text += e.ToString() + "\n";
            };
        }

    }
}