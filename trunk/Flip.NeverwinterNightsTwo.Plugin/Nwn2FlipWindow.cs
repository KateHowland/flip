using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using NWN2Toolset;
using NWN2Toolset.NWN2.Data;
using NWN2Toolset.NWN2.Data.Blueprints;
using NWN2Toolset.NWN2.Data.Instances;
using NWN2Toolset.NWN2.Data.Templates;
using NWN2Toolset.NWN2.Data.TypedCollections;
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
		protected AbstractNwn2BlockFactory factory = new Nwn2BlockFactory();
		protected Dictionary<NWN2ObjectType,StackPanel> blueprintPanels;
		protected Dictionary<NWN2ObjectType,StackPanel> instancePanels;
		
		
		public Nwn2FlipWindow()
		{
			InitializeComponent();	
			
			AbstractNwn2BlockFactory factory = new Nwn2BlockFactory();
						
			blueprintPanels = new Dictionary<NWN2ObjectType,StackPanel>(15);
			instancePanels = new Dictionary<NWN2ObjectType,StackPanel>(15);
			
			foreach (NWN2ObjectType type in Enum.GetValues(typeof(NWN2ObjectType))) {				
				string typestr = type.ToString();
				
				StackPanel sp = new StackPanel();
				sp.Name = typestr + "BlueprintsPanel";
				sp.Background = System.Windows.Media.Brushes.Gray;
				sp.AllowDrop = true;
				blueprintPanels.Add(type,sp);
				
				ScrollViewer sv = new ScrollViewer();
				sv.Content = sp;
				
				TabItem t = new TabItem();
				t.Header = typestr + " (B)";
				t.Content = sv;
				tabs.Items.Add(t);
				
				sp = new StackPanel();	
				sp.Name = typestr + "InstancesPanel";
				sp.Background = System.Windows.Media.Brushes.DarkGray;
				sp.AllowDrop = true;
				instancePanels.Add(type,sp);
				
				sv = new ScrollViewer();
				sv.Content = sp;
				
				t = new TabItem();
				t.Header = typestr + " (I)";
				t.Content = sv;
				tabs.Items.Add(t);
			}
							
			Populate();	
			
			
			
			
				
				
			ToolsetEventReporter reporter = new ToolsetEventReporter();
			reporter.Start();
			
			reporter.InstanceAdded += delegate(object sender, InstanceEventArgs e) 
			{  				
				instancePanels[e.Instance.ObjectType].Children.Add(factory.CreateInstanceBlock(e.Instance));
			};
			
			
			
			
			
			
			reporter.BlueprintAdded += delegate(object sender, BlueprintEventArgs e) 
			{  
				ObjectBlock block = factory.CreateBlueprintBlock(e.Blueprint);
				blueprintPanels[e.Blueprint.ObjectType].Children.Add(block);
			};
			
			
//				MouseMove += delegate(object sender2, MouseEventArgs mea) 
//				{  
//					Point p = mea.GetPosition(mainGrid);
//					adorner.UpdatePosition(p);
//				};
			
			
			
			reporter.AreaAdded += delegate(object sender, AreaEventArgs e)
			{  
				OtherObjectsPanel.Children.Add(factory.CreateAreaBlock(e.Area));
			};
						
			mainCanvas.Drop += new DragEventHandler(DroppedOnCanvas);
		}
				

		protected void DroppedOnCanvas(object sender, DragEventArgs e)
		{
			if (!e.Handled && e.Data.GetDataPresent(typeof(Moveable))) {
				Moveable moveable = (Moveable)e.Data.GetData(typeof(Moveable));
				
				Point point = e.GetPosition(mainCanvas);
				double x = point.X - (moveable.ActualWidth/2);
				double y = point.Y - (moveable.ActualHeight/2);
				
				if (!(moveable.Parent == mainCanvas)) {
					moveable.Detach();
					mainCanvas.Children.Add(moveable);					
				}					
				
				moveable.MoveTo(x,y);
				
				ClearAdorner(moveable);
			}
		}
				

		protected void DroppedOnBlockBox(object sender, DragEventArgs e)
		{
			if (!e.Handled && e.Data.GetDataPresent(typeof(Moveable))) {				
				Moveable moveable = (Moveable)e.Data.GetData(typeof(Moveable));
				if (moveable.Parent == mainCanvas) {
					moveable.Detach();
				}
				ClearAdorner(moveable);
			}
		}
		
		
		protected void ClearAdorner(Moveable moveable)
		{
			AdornerLayer layer = AdornerLayer.GetAdornerLayer(moveable);
			if (layer != null) {
				Adorner[] adorners = layer.GetAdorners(moveable);
				if (adorners != null) {
					foreach (Adorner adorner in adorners) {
						if (adorner is MoveableAdorner) layer.Remove(adorner);
					}
				}
			}
		}
		
		
		protected void Populate()
		{
			PopulateActions();
			
			OtherObjectsPanel.Children.Add(factory.CreatePlayerBlock());
			OtherObjectsPanel.Children.Add(factory.CreateModuleBlock());
			
			PopulateBlueprints();
			PopulateInstances();
		}
		
		
		private void PopulateBlueprints()
		{
			foreach (NWN2ObjectType type in Enum.GetValues(typeof(NWN2ObjectType))) {
				StackPanel panel = blueprintPanels[type];
				foreach (INWN2Blueprint blueprint in NWN2GlobalBlueprintManager.GetBlueprintsOfType(type,true,true,true)) {
					ObjectBlock block = factory.CreateBlueprintBlock(blueprint);
					panel.Children.Add(block);
					// FIXME:
				block.MouseDoubleClick += delegate(object sender, MouseButtonEventArgs e) 
				{  
					Moveable moveable = block.Clone();
					mainCanvas.Children.Add(moveable);
					moveable.MoveTo(100,100);
				};
				}
			}
		}
		
		
		private void PopulateInstances()
		{
			NWN2GameArea activeArea = NWN2ToolsetMainForm.App.AreaContents.Area;
			if (activeArea != null) {				
				foreach (NWN2ObjectType type in Enum.GetValues(typeof(NWN2ObjectType))) {
					StackPanel panel = instancePanels[type];
					foreach (INWN2Instance instance in activeArea.GetInstancesForObjectType(type)) {
						ObjectBlock block = factory.CreateInstanceBlock(instance);
						panel.Children.Add(block);
				// FIXME:
				block.MouseDoubleClick += delegate(object sender, MouseButtonEventArgs e) 
				{  
					Moveable moveable = block.Clone();
					mainCanvas.Children.Add(moveable);
					moveable.MoveTo(100,100);
				};
					}
				}
			}
		}
		
		
		protected void PopulateActions()
		{				
			foreach (Statement action in new Nwn2StatementFactory().GetStatements()) {
				ActionsPanel.Children.Add(action);
				
				
				// FIXME:
				action.MouseDoubleClick += delegate(object sender, MouseButtonEventArgs e) 
				{  
					Moveable moveable = action.Clone();
					mainCanvas.Children.Add(moveable);
					moveable.MoveTo(100,100);
				};
			}
		}
	}
}