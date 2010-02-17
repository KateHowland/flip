using System;
using System.IO;
using System.Windows;
using Sussex.Flip.UI;
using Sussex.Flip.Games.NeverwinterNightsTwo;
using Sussex.Flip.Games.NeverwinterNightsTwo.Plugin;
using Sussex.Flip.Games.NeverwinterNightsTwo.Utils;

namespace Sussex.Flip.Games.NeverwinterNightsTwo
{
	/// <summary>
	/// Interaction logic for FlipWindow.xaml
	/// </summary>
	public partial class Nwn2FlipWindow : Window
	{
		public Nwn2FlipWindow()
		{
			InitializeComponent();	
			
			mainCanvas.PreviewDrop += new DragEventHandler(DroppedOnCanvas);
			blockBox.PreviewDrop += new DragEventHandler(DroppedOnBlockBox);
						
			foreach (Statement statement in new StatementFactory().GetStatements()) {
				blockBox.Children.Add(statement);
			}
			
			AbstractNwn2BlockFactory factory = new Nwn2BlockFactory();
			
			ToolsetEventReporter reporter = new ToolsetEventReporter();
			reporter.Start();
			
			reporter.InstanceAdded += delegate(object sender, InstanceEventArgs e) 
			{  
				blockBox.Children.Add(factory.CreateInstanceBlock(e.Instance));
			};
			
			reporter.BlueprintAdded += delegate(object sender, BlueprintEventArgs e) 
			{  
				blockBox.Children.Add(factory.CreateBlueprintBlock(e.Blueprint));
			};
			
			reporter.AreaAdded += delegate(object sender, AreaEventArgs e)
			{  
				blockBox.Children.Add(factory.CreateAreaBlock(e.Area));
			};
		}
				

		private void DroppedOnCanvas(object sender, DragEventArgs e)
		{
			IDataObject data = e.Data;
			if (data.GetDataPresent(typeof(Moveable))) {
				Moveable moveable = (Moveable)data.GetData(typeof(Moveable));
				
				Point point = e.GetPosition(mainCanvas);
				double x = point.X - (moveable.ActualWidth/2);
				double y = point.Y - (moveable.ActualHeight/2);
				
				if (!mainCanvas.Children.Contains(moveable)) {
					moveable.Detach();
					mainCanvas.Children.Add(moveable);
				}					
				
				moveable.MoveTo(x,y);
			}
		}
				

		private void DroppedOnBlockBox(object sender, DragEventArgs e)
		{
			IDataObject data = e.Data;
			if (data.GetDataPresent(typeof(Moveable))) {
				Moveable moveable = (Moveable)data.GetData(typeof(Moveable));
								
				if (!blockBox.Children.Contains(moveable)) {
					moveable.Detach();
					blockBox.Children.Add(moveable);
				}		
			}
		}
	}
}