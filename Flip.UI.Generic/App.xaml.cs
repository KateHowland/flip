using System;
using System.Windows;
using System.Data;
using System.Xml;
using System.Configuration;

namespace Flip.UI.Generic
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		public App()
		{
			//InitializeComponent();
			Sussex.Flip.Core.FakeTranslator t = new Sussex.Flip.Core.FakeTranslator();
			Sussex.Flip.Core.FlipAttacher a = new Sussex.Flip.Core.FakeAttacher(t);
			Sussex.Flip.UI.FlipWindow w = new Sussex.Flip.UI.FlipWindow(a);
			w.Show();
		}
	}
}