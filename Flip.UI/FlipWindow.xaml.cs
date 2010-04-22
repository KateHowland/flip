﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Sussex.Flip.Core;

namespace Sussex.Flip.UI
{
	/// <summary>
	/// Interaction logic for FlipWindow.xaml
	/// </summary>
	public partial class FlipWindow : Window
	{
		protected TriggerBar triggerBar;
		protected FlipAttacher attacher;		
		protected MoveablesPanel blockBox;
		protected MoveableProvider provider;
		
		
		public FlipWindow(FlipAttacher attacher, MoveableProvider provider, TriggerControl triggerControl)
		{
			if (attacher == null) throw new ArgumentNullException("attacher");
			if (provider == null) throw new ArgumentNullException("provider");
        	if (triggerControl == null) throw new ArgumentNullException("triggerControl");
			
			this.attacher = attacher;
			this.provider = provider;
			
			InitializeComponent();
			
			blockBox = new MoveablesPanel();
			Grid.SetRow(blockBox,1);
			Grid.SetColumn(blockBox,1);
			blockBox.PreviewDrop += ReturnMoveableToBox;
			
			provider.Populate(blockBox);
				
			mainGrid.Children.Add(blockBox);
						
			mainCanvas.Drop += DroppedOnCanvas;
			MouseDown += GetDragSource;
			MouseMove += StartDrag;			
			
			PreviewDragEnter += CreateAdorner;
			PreviewDragOver += UpdateAdorner;			
			PreviewDragLeave += DestroyAdorner;			
			PreviewDrop += DestroyAdorner;			
			
			Fitter spineFitter = new SpineFitter();
			triggerBar = new TriggerBar(triggerControl,spineFitter);
			Canvas.SetTop(triggerBar,30);
			Canvas.SetLeft(triggerBar,30);
			mainCanvas.Children.Add(triggerBar);
			
			triggerBar.Changed += UpdateNaturalLanguage;
			DisplayCodeAndNaturalLanguage(triggerBar);
			
			
			string title = "*******Temp********";
			blockBox.AddBag(title);
		}
		
		
		Sussex.Flip.Utils.Serialiser serialiser = new Sussex.Flip.Utils.Serialiser();
		string raiserPath = @"C:\Flip\raiser.txt";
		string eventPath = @"C:\Flip\event.txt";
		
		protected void SaveTrigger(object sender, RoutedEventArgs e)
		{
			ObjectBlock raiser = triggerBar.TriggerControl.RaiserBlock;
			EventBlock eventBlock = triggerBar.TriggerControl.EventBlock;
			
			if (raiser != null && eventBlock != null) {	
				serialiser.Serialise(raiserPath,raiser.Behaviour);
				serialiser.Serialise(eventPath,eventBlock.Behaviour);				
			}
				
			else {				
				MessageBox.Show("Trigger incomplete.");					
			}
		}
		
		
		protected void OpenTrigger(object sender, RoutedEventArgs e)
		{
			EventBehaviour eb = serialiser.Deserialise(eventPath,typeof(EventBehaviour)) as EventBehaviour;
			if (eb != null) {
				EventBlock block = new EventBlock(eb);
				triggerBar.TriggerControl.EventBlock = block;
			}
			
			ObjectBlock raiser = provider.GetMoveableFromSerialised(raiserPath) as ObjectBlock;
			if (raiser != null) {
				triggerBar.TriggerControl.RaiserBlock = raiser;
			}
		}
		

		protected void UpdateNaturalLanguage(object sender, EventArgs e)
		{
			DisplayCodeAndNaturalLanguage(triggerBar);
		}
		
		
		protected void DisplayCodeAndNaturalLanguage(ITranslatable translatable)
		{
			if (translatable == null) return;
			this.naturalLanguageTextBlock.Text = translatable.GetNaturalLanguage();
			this.targetCodeTextBlock.Text = (translatable.IsComplete ? "Complete." : "Incomplete.") + "\n\n" + translatable.GetCode();
		}
		
		
		protected void Dropped(object sender, DragEventArgs e)
		{
			if (!e.Handled) e.Handled = true;
		}
		
		
		protected void CompileAndAttach(object sender, RoutedEventArgs e)
		{			
			if (!triggerBar.IsComplete) {
				MessageBox.Show("Your script isn't complete! Make sure you've filled in the trigger, and " +
				                "filled in all the gaps of any actions, conditions or other blocks you've attached " +
				                "to your script before trying to compile.");
				return;
			}
			
			string code = triggerBar.GetCode();
			
			FlipScript script = new FlipScript(code);			
			
			script.Name = "EmptyScript";
			
			string address = triggerBar.GetAddress();
			
			try {
				attacher.Attach(script,address);
			}
			catch (Exception ex) {
				MessageBox.Show(ex.ToString());
			}
		}
		
		
		protected void RefreshBlocks(object sender, RoutedEventArgs e)
		{
			provider.Refresh(blockBox);
			MessageBox.Show("Refreshed.");
		}	
		
		
		protected void CreateAdorner(object sender, DragEventArgs e)
		{
			Moveable moveable = (Moveable)e.Data.GetData(typeof(Moveable));
			if (moveable != null && adorner == null) {
				AdornerLayer layer = AdornerLayer.GetAdornerLayer(mainGrid);
				Point p = e.GetPosition(this);
				adorner = new MoveableAdorner(moveable,layer,p);
			}
		}
		
		
		protected void UpdateAdorner(object sender, DragEventArgs e)
		{
			Moveable moveable = (Moveable)e.Data.GetData(typeof(Moveable));
			if (moveable != null && adorner != null) {
				adorner.Position = e.GetPosition(this);
			}
		}
		
		
		protected void DestroyAdorner(object sender, DragEventArgs e)
		{
			Moveable moveable = (Moveable)e.Data.GetData(typeof(Moveable));
			if (moveable != null && adorner != null) {
				adorner.Destroy();
	    		adorner = null;
			}
		}
		
		
    	Point? dragPos = null;
		MoveableAdorner adorner = null;
		Moveable dragging = null;
    	
    	
    	private void StartDrag(object sender, MouseEventArgs e)
    	{    	
    		try {
	    		if (dragPos != null) {
	    			Point currentPos = e.GetPosition(null);
	    			Vector moved = dragPos.Value - currentPos;
	    			
	    			if (e.LeftButton == MouseButtonState.Pressed &&
	    			    (Math.Abs(moved.X) > SystemParameters.MinimumHorizontalDragDistance ||
	    			     Math.Abs(moved.Y) > SystemParameters.MinimumVerticalDragDistance)) {
	    				    				
	    				DragDropEffects effects = blockBox.HasMoveable(dragging) ? DragDropEffects.Copy : DragDropEffects.Move;
	    				DataObject dataObject = new DataObject(typeof(Moveable),dragging);
	    				DragDrop.DoDragDrop(dragging,dataObject,effects);
	    				
	    				dragging = null;
	    				dragPos = null;
	    			}
	    		}
    		}
    		catch (Exception ex) {
    			MessageBox.Show(String.Format("Failed to carry out drag-drop operation.{0}{1}",Environment.NewLine,ex.ToString()));
    		}
    	}    	
    	

    	protected void GetDragSource(object sender, MouseEventArgs e)
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
				Size size = new Size();
				
				if (e.Data.GetDataPresent(typeof(Moveable))) {
					moveable = (Moveable)e.Data.GetData(typeof(Moveable));
					size = moveable.RenderSize; // use the original's size as the clone has not been drawn yet
					if (e.AllowedEffects == DragDropEffects.Copy) {
						moveable = moveable.DeepCopy();
					}
				}	
				// HACK:
//				else if (e.Data.GetDataPresent(typeof(NWN2InstanceCollection))) {
//					NWN2InstanceCollection instances = (NWN2InstanceCollection)e.Data.GetData(typeof(NWN2InstanceCollection));
//					if (instances.Count > 0) {
//						moveable = factory.CreateInstanceBlock(instances[0]);
//						size = ObjectBlock.DefaultSize;
//					}
//				}				
//				else if (e.Data.GetDataPresent(typeof(NWN2BlueprintCollection))) {
//					try {
//						NWN2BlueprintCollection blueprints = (NWN2BlueprintCollection)e.Data.GetData(typeof(NWN2BlueprintCollection));
//						if (blueprints.Count > 0) {
//							moveable = factory.CreateBlueprintBlock(blueprints[0]);
//							size = ObjectBlock.DefaultSize;
//						}
//					}
//					catch (System.Runtime.InteropServices.COMException) {
//						/*
//						 * Weird error occurs here - even though GetDataPresent() returns true,
//						 * actually trying to retrieve the data raises this nasty exception.
//						 * TODO:
//						 * Look for the blueprints directly in the toolset instead.
//						 */
//					}
//				}			
				
				if (moveable != null) {		
					PlaceInWorkspace(moveable);
					
					Point position = e.GetPosition(this);
					position.X -= (size.Width/2);
					position.Y -= (size.Height/2);
					moveable.MoveTo(position);
				}
			}
		}


		protected void ReturnMoveableToBox(object sender, DragEventArgs e)
		{
			if (!e.Handled) {
				if (e.Data.GetDataPresent(typeof(Moveable))) {
					Moveable moveable = (Moveable)e.Data.GetData(typeof(Moveable));
					if (!blockBox.HasMoveable(moveable)) {
						moveable.Remove();
					}
					e.Handled = true;
				}
			}
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
				moveable.Remove();
				mainCanvas.Children.Add(moveable);				
			}
		}
	}
}