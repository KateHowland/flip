using System;
using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Media;
using System.Xml;
using Sussex.Flip.UI;
using Sussex.Flip.Games.NeverwinterNightsTwo;

namespace Sussex.Flip.UI.Generic
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		public App()
		{
//			InitializeComponent();
			
			Sussex.Flip.Core.FakeTranslator translator = new Sussex.Flip.Core.FakeTranslator();
			Sussex.Flip.Core.FlipAttacher attacher = new Sussex.Flip.Core.FakeAttacher(translator);			
			
			Nwn2Fitters fitters = new Nwn2Fitters();
			Brush actionBrush = new LinearGradientBrush(Colors.LightGreen,Colors.Green,45);
			Brush conditionBrush = new LinearGradientBrush(Colors.Lavender,Colors.Salmon,45);	
			Brush eventBrush = new LinearGradientBrush(Colors.Maroon,Colors.Firebrick,45);	
			
			Nwn2StatementFactory statements = new Nwn2StatementFactory(fitters,actionBrush,conditionBrush);			
			Nwn2ObjectBlockFactory blocks = new Nwn2ObjectBlockFactory();
			Nwn2EventBlockFactory events = new Nwn2EventBlockFactory(eventBrush);
			
			Nwn2MoveableProvider provider = new Nwn2MoveableProvider(blocks,statements,events);			
			
			FlipWindow window = new FlipWindow(attacher,provider);
			
			window.Show();
		}
	}
}