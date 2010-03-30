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
    /// Interaction logic for IfControl.xaml
    /// </summary>

    public partial class IfControl : Moveable
    {
        public IfControl()
        {
            InitializeComponent();
        }

        
		public override Moveable DeepCopy()
		{
			return new IfControl();
		}
    }
}