using System;
using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Media;
using System.Xml;
using Sussex.Flip.UI;
using Sussex.Flip.Games.NeverwinterNightsTwo;

namespace Flip.UI.Generic
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
			
			Nwn2StatementFactory statements = new Nwn2StatementFactory(fitters,actionBrush,conditionBrush);			
			Nwn2BlockFactory blocks = new Nwn2BlockFactory();
			
			Nwn2MoveableProvider provider = new Nwn2MoveableProvider(blocks,statements);			
			
			FlipWindow window = new FlipWindow(attacher,provider);
			
			window.Show();
		}
	}
}