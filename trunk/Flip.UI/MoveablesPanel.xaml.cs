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
		protected Dictionary<string,Bag> bags;


		public MoveablesPanel()
		{
			InitializeComponent();
			bags = new Dictionary<string,Bag>();
		}


		public void AddBag(string bagName)
		{
			AddBag(bagName,bagName);
		}


		public void AddBag(string bagName, string displayName)
		{
			if (bagName == null) throw new ArgumentNullException("name"); 
			if (bags.ContainsKey(bagName)) throw new ArgumentException("Bag named '" + bagName + "' already exists.", "name"); 
			
			Bag bag = new Bag(bagName);
			bags.Add(bagName,bag);

			tabs.Items.Add(bag);
		}


		public void AddMoveable(string bagName, Moveable moveable)
		{
			if (bagName == null) throw new ArgumentNullException("bagName"); 
			if (moveable == null) throw new ArgumentNullException("moveable"); 
			if (!bags.ContainsKey(bagName)) throw new ArgumentException("No bag named '" + bagName + "' exists.", "bag"); 

			Bag bag = bags[bagName];
			bag.Add(moveable);
		}
		
		
		public void RemoveMoveable(string bagName, Moveable moveable)
		{
			if (bagName == null) throw new ArgumentNullException("bagName"); 
			if (moveable == null) throw new ArgumentNullException("moveable"); 
			if (!bags.ContainsKey(bagName)) throw new ArgumentException("No bag named '" + bagName + "' exists.", "bag"); 

			Bag bag = bags[bagName];
			bag.Remove(moveable);
		}
		
		
		public void EmptyBag(string bagName)
		{
			if (bagName == null) throw new ArgumentNullException("bagName");
			if (!bags.ContainsKey(bagName)) throw new ArgumentException("No bag named '" + bagName + "' exists.", "bag"); 

			Bag bag = bags[bagName];
			bag.Empty();
		}
		
		
		public void RemoveBag(string bagName)
		{			
			if (bagName == null) throw new ArgumentNullException("bagName"); 
			if (!bags.ContainsKey(bagName)) throw new ArgumentException("No bag named '" + bagName + "' exists.", "bag"); 

			Bag bag = bags[bagName];
			bags.Remove(bagName);
			tabs.Items.Remove(bag);
		}
		
		
		public UIElementCollection GetMoveables(string bagName)
		{
			if (bagName == null) throw new ArgumentNullException("bagName"); 
			if (!bags.ContainsKey(bagName)) throw new ArgumentException("No bag named '" + bagName + "' exists.", "bag"); 

			Bag bag = bags[bagName];
			return bag.Children;
		}
	}
}
