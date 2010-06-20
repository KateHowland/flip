﻿using System;
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
            
            Image image = GetImage(@"C:\Sussex University\Flip\Pictures\System\logo.jpg");
            
            if (image != null) {
	            image.Width = 200;
	            image.Margin = new Thickness(2,8,2,2);
	            panel.Children.Insert(0,image);
            }
            else {
            	TextBlock tb = new TextBlock();
            	tb.Text = "Flip";
            	tb.FontSize = 36;
            	tb.FontWeight = FontWeights.Bold;
            	tb.HorizontalAlignment = HorizontalAlignment.Center;
            	panel.Children.Insert(0,tb);
            }
            
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
		
		
		/// <summary>
		/// Gets an Image from an image file at the given path.
		/// </summary>
		/// <param name="path">The path to locate an image.</param>
		/// <returns>An Image, or null if an exception was raised.</returns>
		protected Image GetImage(string path)
		{			
			if (path == null) throw new ArgumentNullException("path");
			if (!File.Exists(path)) return null;
			
			try {
				Image image = new Image();
				Uri uri = new Uri(path);
				
				BitmapImage bmp = new BitmapImage();
				bmp.BeginInit();
				bmp.UriSource = uri;
				bmp.CacheOption = BitmapCacheOption.OnLoad;
				bmp.EndInit();
				
				image.Source = bmp;				
				return image;
			}
			catch (Exception) {
				return null;
			}
		}		
    }
}