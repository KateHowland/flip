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
				sv.Focusable = false;
				
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
				sv.Focusable = false;
				
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
				ObjectBlock block = factory.CreateInstanceBlock(e.Instance);
				instancePanels[e.Instance.ObjectType].Children.Add(block);
			};
			
			reporter.BlueprintAdded += delegate(object sender, BlueprintEventArgs e) 
			{  
				ObjectBlock block = factory.CreateBlueprintBlock(e.Blueprint);
				blueprintPanels[e.Blueprint.ObjectType].Children.Add(block);
			};
			
			reporter.AreaAdded += delegate(object sender, AreaEventArgs e)
			{  
				ObjectBlock block = factory.CreateAreaBlock(e.Area);
				OtherObjectsPanel.Children.Add(block);
			};
			
			mainCanvas.Drop += DroppedOnCanvas;
			MouseDown += GetDragSource;
			MouseMove += StartDrag;			
			
			PreviewDragEnter += delegate(object sender, DragEventArgs e) 
			{  
				Moveable moveable = (Moveable)e.Data.GetData(typeof(Moveable));
				if (moveable != null && adorner == null) {
					AdornerLayer layer = AdornerLayer.GetAdornerLayer(mainGrid);
					Point p = e.GetPosition(this);
					adorner = new MoveableAdorner(moveable,layer,p);
				}
			};
			
			PreviewDragOver += delegate(object sender, DragEventArgs e) 
			{  
				Moveable moveable = (Moveable)e.Data.GetData(typeof(Moveable));
				if (moveable != null && adorner != null) {
					adorner.Position = e.GetPosition(this);
				}
			};
			
			PreviewDragLeave += delegate(object sender, DragEventArgs e) 
			{  
				Moveable moveable = (Moveable)e.Data.GetData(typeof(Moveable));
				if (moveable != null && adorner != null) {
					adorner.Destroy();
	    			adorner = null;
				}
			};
			
			PreviewDrop += delegate(object sender, DragEventArgs e) 
			{  
				Moveable moveable = (Moveable)e.Data.GetData(typeof(Moveable));
				if (moveable != null && adorner != null) {
					adorner.Destroy();
	    			adorner = null;
				}
			};
		}
		
		
    	Point? dragPos = null;
		MoveableAdorner adorner = null;
		Moveable dragging = null;
    	
    	
    	private void StartDrag(object sender, MouseEventArgs e)
    	{    	
    		if (dragPos != null) {
    			Point currentPos = e.GetPosition(null);
    			Vector moved = dragPos.Value - currentPos;
    			
    			if (e.LeftButton == MouseButtonState.Pressed &&
    			    (Math.Abs(moved.X) > SystemParameters.MinimumHorizontalDragDistance ||
    			     Math.Abs(moved.Y) > SystemParameters.MinimumVerticalDragDistance)) {
    				    				
    				DragDropEffects effects = IsInBlockBox(dragging) ? DragDropEffects.Copy : DragDropEffects.Move;
    				DataObject dataObject = new DataObject(typeof(Moveable),dragging);
    				DragDrop.DoDragDrop(dragging,dataObject,effects);
    				
    				dragging = null;
    				dragPos = null;
    			}
    		}
    	}    	
    	

    	private void GetDragSource(object sender, MouseEventArgs e)
    	{
    		FrameworkElement f = e.OriginalSource as FrameworkElement;
    		    		
    		if (f == null) return;
    		
    		while (!(f is Moveable) && (f = f.Parent as FrameworkElement) != null);
    		
    		if (f is Moveable) {
    			dragPos = e.GetPosition(null);
    			dragging = (Moveable)f;
    		}
    	}
		
		
		protected void DroppedOnCanvas(object sender, DragEventArgs e)
		{		
			if (!e.Handled) {
				
				Moveable moveable = null;
				Size size = new Size(0,0);
				
				if (e.Data.GetDataPresent(typeof(Moveable))) {
					moveable = (Moveable)e.Data.GetData(typeof(Moveable));
					size = moveable.RenderSize; // use the original's size as the clone has not been drawn yet
					if (e.AllowedEffects == DragDropEffects.Copy) {
						moveable = moveable.Clone();
					}
				}				
				else if (e.Data.GetDataPresent(typeof(NWN2InstanceCollection))) {
					NWN2InstanceCollection instances = (NWN2InstanceCollection)e.Data.GetData(typeof(NWN2InstanceCollection));
					if (instances.Count > 0) {
						moveable = factory.CreateInstanceBlock(instances[0]);
						size = moveable.RenderSize;
					}
				}				
				else if (e.Data.GetDataPresent(typeof(NWN2BlueprintCollection))) {
					try {
						NWN2BlueprintCollection blueprints = (NWN2BlueprintCollection)e.Data.GetData(typeof(NWN2BlueprintCollection));
						if (blueprints.Count > 0) {
							moveable = factory.CreateBlueprintBlock(blueprints[0]);
							size = moveable.RenderSize;
						}
					}
					catch (System.Runtime.InteropServices.COMException) {
						/*
						 * Weird error occurs here - even though GetDataPresent() returns true,
						 * actually trying to retrieve the data raises this nasty exception.
						 * TODO:
						 * Look for the blueprints directly in the toolset instead.
						 */
					}
				}			
				
				if (moveable != null) {		
					PlaceInWorkspace(moveable);		
					
					Point position = e.GetPosition(this);
					position.X -= (size.Width/2);
					position.Y -= (size.Height/2);
					moveable.MoveTo(position);
				}
			}
		}
				

		protected void DroppedOnBlockBox(object sender, DragEventArgs e)
		{
			if (!e.Handled) {
				if (e.Data.GetDataPresent(typeof(Moveable))) {
					Moveable moveable = (Moveable)e.Data.GetData(typeof(Moveable));
					if (!IsInBlockBox(moveable)) moveable.Detach();
				}
			}
		}
		
		
		public bool IsInBlockBox(Moveable moveable)
		{
			FrameworkElement element = moveable.Parent as FrameworkElement;
			while (element != null && element != tabs) {
				element = element.Parent as FrameworkElement;
			}
			return element == tabs;
		}
		
		
		protected int zIndex = 0;
		public void PlaceInWorkspace(Moveable moveable)
		{
			try {
				Canvas.SetZIndex(moveable,++zIndex);
			}
			catch (ArithmeticException) {
				foreach (UIElement element in mainCanvas.Children) {
					Canvas.SetZIndex(element,0);
					zIndex = 0;
				}
			}
			
			if (!(moveable.Parent == mainCanvas)) {
				moveable.Detach();
				mainCanvas.Children.Add(moveable);				
			}
		}
		
		
		protected void Populate()
		{
			PopulateActions();
			
			ObjectBlock block = factory.CreatePlayerBlock();
			OtherObjectsPanel.Children.Add(block);
			
			block = factory.CreateModuleBlock();
			OtherObjectsPanel.Children.Add(block);
			
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
					}
				}
			}
		}
		
		
		protected void PopulateActions()
		{				
			foreach (Statement action in new Nwn2StatementFactory().GetStatements()) {
				ActionsPanel.Children.Add(action);
			}
		}
	}
}