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
	/// Interaction logic for MoveablesPanel.xaml
	/// </summary>

	public partial class MoveablesPanel : UserControl, IMoveableManager
	{
		protected Dictionary<string, UIElementCollection> bags;


		public MoveablesPanel()
		{
			InitializeComponent();
			bags = new Dictionary<string, UIElementCollection>();
		}


		public void AddBag(string bagName)
		{
			AddBag(bagName,bagName);
		}


		public void AddBag(string bagName, string displayName)
		{
			if (bagName == null) throw new ArgumentNullException("name"); 
			if (bags.ContainsKey(bagName)) throw new ArgumentException("Bag named '" + bagName + "' already exists.", "name"); 

			WrapPanel p = new WrapPanel();
			p.Background = Brushes.DarkBlue;
			bags.Add(bagName,p.Children);
			p.AllowDrop = true;

			ScrollViewer sv = new ScrollViewer();
			sv.Focusable = false;
			sv.Content = p;

			TabItem t = new TabItem();
			t.Header = displayName;
			t.Content = sv;

			tabs.Items.Add(t);
		}


		public void AddMoveable(string bagName, Moveable moveable)
		{
			if (bagName == null) throw new ArgumentNullException("bagName"); 
			if (moveable == null) throw new ArgumentNullException("moveable"); 
			if (!bags.ContainsKey(bagName)) throw new ArgumentException("No bag named '" + bagName + "' exists.", "bag"); 

			UIElementCollection bag = bags[bagName];
			bag.Add(moveable);
		}
	}
}
