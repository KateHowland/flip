using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Sussex.Flip.UI
{
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
        }
        
        
        private void CloseWindow(object sender, RoutedEventArgs e)
        {
        	Close();
        }
        
        
        protected void LaunchWebsite(object sender, RoutedEventArgs e)
        {
        	Process.Start("http://www.flipproject.org.uk");
        }
        
        
        protected void LaunchEmail(object sender, RoutedEventArgs e)
        {
        	Process.Start("mailto:flip@sussex.ac.uk");
        }
    }
}