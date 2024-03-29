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
	public interface IMoveableManager
	{
		void AddBag(string bagName);
		void AddBag(string bagName, bool wrap);
		void AddBag(string bagName, string displayName, bool wrap);
		void AddBag(string bagName, string displayName, bool wrap, bool top);
		void DisplayBag(string bagName);
		void RemoveBag(string bagName);
		void AddMoveable(string bagName, Moveable moveable);
		void AddMoveable(string bagName, Moveable moveable, bool bringIntoView);
		void RemoveMoveable(string bagName, Moveable moveable);
		void EmptyBag(string bagName);
		UIElementCollection GetMoveables(string bagName);
		List<string> GetBags();
		bool HasMoveable(Moveable moveable);
		bool HasMoveable(Moveable moveable, string bagName);
		bool HasBag(string bag);
	}
}
