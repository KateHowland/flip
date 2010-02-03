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
        public Noun()
        {
            InitializeComponent();
        }
        
        
        public Noun(Image image) : this()
        {
            SetImage(image);
        }
        
        
        public void SetImage(Image image)
        {
        	Face.Content = image;
        }
    }
}