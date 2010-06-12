using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
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
		private static OuterGlowBitmapEffect highlight;
		protected Dictionary<string,Bag> bags;
		
		
		static MoveablesPanel()
		{
			highlight = new OuterGlowBitmapEffect();
			highlight.GlowColor = Colors.HotPink;
		}


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
			
			RadioButton radioButton = new RadioButton();
			radioButton.GroupName = "bagSelectionGroup";
			radioButton.Content = bagName;
			radioButton.Checked += delegate 
			{  
				tabs.SelectedItem = bag;
				radioButton.BitmapEffect = highlight;
			};
			radioButton.Unchecked += delegate 
			{  
				radioButton.BitmapEffect = null;
			};
			bagButtons.Children.Add(radioButton);
		}
		
		
		public void DisplayBag(string bagName)
		{
			if (bagName == null) throw new ArgumentNullException("bagName"); 
			if (!bags.ContainsKey(bagName)) throw new ArgumentException("No bag named '" + bagName + "' exists.", "bag"); 

			foreach (RadioButton radioButton in bagButtons.Children) {
				if (radioButton.Content as string == bagName) {
					radioButton.IsChecked = true;
					return;
				}
			}
			
			throw new ApplicationException("No radio button found for bag '" + bagName + "'.");
		}


		public void AddMoveable(string bagName, Moveable moveable)
		{
			if (bagName == null) throw new ArgumentNullException("bagName"); 
			if (moveable == null) throw new ArgumentNullException("moveable"); 
			if (!bags.ContainsKey(bagName)) throw new ArgumentException("No bag named '" + bagName + "' exists.", "bag"); 

			Bag bag = bags[bagName];
			bag.Add(moveable);
		}


		public void AddMoveable(string bagName, Moveable moveable, bool bringIntoView)
		{
			AddMoveable(bagName,moveable);
			if (bringIntoView) {
				DisplayBag(bagName);
				moveable.BringIntoView();
			}
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
			
			RadioButton removing = null;
			foreach (RadioButton radioButton in bagButtons.Children) {
				if (radioButton.Content as string == bagName) {
					removing = radioButton;
					break;
				}
			}
			
			if (removing != null) bagButtons.Children.Remove(removing);
		}
		
		
		public UIElementCollection GetMoveables(string bagName)
		{
			if (bagName == null) throw new ArgumentNullException("bagName"); 
			if (!bags.ContainsKey(bagName)) throw new ArgumentException("No bag named '" + bagName + "' exists.", "bag"); 

			Bag bag = bags[bagName];
			return bag.Children;
		}
		
		
		public List<string> GetBags()
		{
			List<string> bagNames = new List<string>(bags.Count);
			foreach (string key in bags.Keys) bagNames.Add(key);
			return bagNames;
		}
		
		
		public bool HasMoveable(Moveable moveable)
		{
			foreach (string bagName in GetBags()) {
				if (HasMoveable(moveable,bagName)) return true;
			}
			return false;
		}
		
		
		public bool HasMoveable(Moveable moveable, string bagName)
		{
			if (moveable == null) throw new ArgumentNullException("moveable");
			
			UIElementCollection moveables = GetMoveables(bagName);
			return moveables.Contains(moveable);
		}
		
		
		public bool HasBag(string bag)
		{
			if (bag == null) throw new ArgumentNullException("bag");
			
			return GetBags().Contains(bag);
		}
	}
}
