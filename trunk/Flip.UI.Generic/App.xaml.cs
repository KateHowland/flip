﻿using System;
using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Media;
using System.Xml;
using Sussex.Flip.Core;
using Sussex.Flip.UI;
using Sussex.Flip.Games.NeverwinterNightsTwo;
using Sussex.Flip.Games.NeverwinterNightsTwo.Integration;

namespace Sussex.Flip.UI.Generic
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		public App()
		{
			FlipTranslator translator = new FakeTranslator();
			FlipAttacher attacher = new FakeAttacher(translator);			
			
			Nwn2Fitters fitters = new Nwn2Fitters();
			Nwn2StatementFactory statements = new Nwn2StatementFactory(fitters);	
			Nwn2TriggerFactory triggers = new Nwn2TriggerFactory(fitters);	
			Nwn2ImageProvider images = new Nwn2ImageProvider(new NarrativeThreadsHelper());
			Nwn2ObjectBlockFactory blocks = new Nwn2ObjectBlockFactory(images);
			
			Nwn2MoveableProvider provider = new Nwn2MoveableProvider(blocks,statements,triggers);
			
			FlipWindow window = new FlipWindow(attacher,provider,images);
			
			// HACK:
			// TODO:
			SerialisationHelper.customObjectAssembly = System.Reflection.Assembly.GetAssembly(typeof(Nwn2ObjectBlockFactory));
			
			window.Show();
		}
	}
}