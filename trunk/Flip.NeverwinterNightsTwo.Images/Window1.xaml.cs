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

namespace Sussex.Flip.Games.NeverwinterNightsTwo.Images
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		public Window1()
		{
			InitializeComponent();
			
			Label l = new Label();
			
			ImageGetter getter = new ImageGetter();
			
			Loaded += delegate 
			{
				Image myImage = getter.GetImage("Creature","c_dogwolfwin");
				l.Content = myImage;
				jkjkj.Children.Add(l);
			};
						
			MouseDoubleClick += delegate 
			{  
				Image img = getter.GetImage("Creature","c_grobnar");
				l.Content = img;
			};
		}
	}
}