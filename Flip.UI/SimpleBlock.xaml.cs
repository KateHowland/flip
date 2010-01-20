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
    /// Interaction logic for SimpleBlock.xaml
    /// </summary>

    public partial class SimpleBlock : Moveable
    {
        public SimpleBlock(Brush brush)
        {
            InitializeComponent();
            SetFace(brush);
        }
        
        
    	public SimpleBlock() : this(new LinearGradientBrush(Colors.Gray,Colors.LightGray,45))
        {
        }
        
        
        public void SetFace(Brush brush)
        {
        	Face.Background = brush;
        }
    }
}