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
    /// Interaction logic for Noun.xaml
    /// </summary>
    public partial class Noun : Moveable
    {
		public Image Image {
			get { 
    			Image image = Face.Content as Image;
    			if (image == null) throw new InvalidOperationException("Content is not a valid image.");
    			else return image;
    		}
			set { 
    			Face.Content = value;
    		}
		}
    	
    	
        public Noun(Image image)
        {
            InitializeComponent();
            Image = image;
        }
    }
}