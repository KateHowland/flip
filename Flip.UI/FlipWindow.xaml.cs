using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;
using System.Xml.Serialization;
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
		
		
		public FlipWindow(FlipAttacher attacher, MoveableProvider provider)
		{
			if (attacher == null) throw new ArgumentNullException("attacher");
			if (provider == null) throw new ArgumentNullException("provider");
			
			this.attacher = attacher;
			this.provider = provider;
			
			InitializeComponent();
			
			blockBox = new MoveablesPanel();
			Grid.SetRow(blockBox,1);
			Grid.SetColumn(blockBox,1);
			Grid.SetRowSpan(blockBox,2);
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
						
			triggerBar = new TriggerBar(provider.GetDefaultTrigger(),new SpineFitter());
			Canvas.SetTop(triggerBar,30);
			Canvas.SetLeft(triggerBar,30);
			mainCanvas.Children.Add(triggerBar);
			
			triggerBar.Changed += ScriptChanged;
			UpdateNaturalLanguageView(triggerBar);
			
			Loaded += delegate { UpdateTitle(); };
		}
		
		
		protected bool isDirty = false;
		
		public bool IsDirty {
			get { return isDirty; }
			set { 
				if (isDirty != value) {
					isDirty = value;
					UpdateTitle();
				}
			}
		}
		
		
		string filename = "filename";
		protected void UpdateTitle()
		{
			if (isDirty) Title = String.Format("Flip: {0}*",filename);
			else Title = String.Format("Flip: {0}",filename);
		}
		
		
		protected bool CloseCurrentScript()
		{
			if (!IsDirty) return true;
			
			MessageBoxResult result = MessageBox.Show("Save this script before closing?",
										              "Save changes?",
										              MessageBoxButton.YesNoCancel,
										              MessageBoxImage.Question,
										              MessageBoxResult.Cancel);
				                
			switch (result) {
					
				case MessageBoxResult.Yes:
					SaveScriptToFile();
					CloseScript();
					return true;
					
				case MessageBoxResult.No:
					CloseScript();
					return true;
					
				default:
					return false;
			}
		}
		
		
		protected void NewScript(object sender, RoutedEventArgs e)
		{
			if (!CloseCurrentScript()) return;
		}
		
		
		protected void OpenScriptFromModule(object sender, RoutedEventArgs e)
		{			
			if (!CloseCurrentScript()) return;
			
			MessageBox.Show("Open file dialog should run here. Not implemented.");
		}
		
		
		protected void CloseScript(object sender, RoutedEventArgs e)
		{
			if (!CloseCurrentScript()) return;
		}
		
		
		protected void CloseScript()
		{			
			Clear();
			IsDirty = false;
			
			MessageBox.Show("Empty script file should be opened here. Not implemented.");
		}
		
		
		protected void SaveScriptToFile(object sender, RoutedEventArgs e)
		{
			SaveScriptToFile();
		}
		
		
		protected void SaveScriptToFile()
		{
			Microsoft.Win32.SaveFileDialog dialog = new Microsoft.Win32.SaveFileDialog();
			dialog.AddExtension = true;
			dialog.CheckFileExists = false;
			dialog.CheckPathExists = true;
			dialog.CreatePrompt = false;
			dialog.DefaultExt = ".txt";
			dialog.DereferenceLinks = false;			
			dialog.Filter = Sussex.Flip.Utils.FileExtensionFilters.TXT_ALL;
			dialog.FilterIndex = 0;			
			dialog.InitialDirectory = @"C:\Sussex University\Flip\";
			dialog.OverwritePrompt = true;
			dialog.RestoreDirectory = false;
			dialog.Title = "Save script";
			dialog.ValidateNames = true;
			
			bool? result = dialog.ShowDialog(this);			
			if (!result.HasValue || !result.Value) return;
			
			string path = dialog.FileName;
			
			try {
				XmlWriterSettings settings = new XmlWriterSettings();
				settings.CloseOutput = true;
				settings.Indent = true;
				settings.NewLineOnAttributes = false;
				
				using (XmlWriter writer = XmlWriter.Create(path,settings)) {
					writer.WriteStartDocument();				
					
					writer.WriteStartElement("Moveables");
					
					foreach (UIElement child in mainCanvas.Children) {
						
						Moveable moveable = child as Moveable;
						if (moveable == null) continue;
						
						string elementName;
						
						if (moveable is ObjectBlock) elementName = "ObjectBlock";
						else if (moveable is Statement) elementName = "Statement";
						else throw new FormatException("Unknown Moveable type (" + moveable.GetType() + ").");
						
						if (moveable != null) {
							writer.WriteStartElement(elementName);
							moveable.WriteXml(writer);
							writer.WriteEndElement();
						}
						
						
					}
									
					writer.WriteEndElement();
					
					writer.WriteEndDocument();				
					writer.Flush();
				}
				
				MessageBox.Show("Saved.");
			}
			catch (Exception x) {
				MessageBox.Show(x.ToString());
			}
		}
		
		
		int position = 40;
		protected void OpenScriptFromFile(object sender, RoutedEventArgs e)
		{
			Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
			dialog.CheckFileExists = true;
			dialog.CheckPathExists = true;
			dialog.DefaultExt = ".txt";
			dialog.DereferenceLinks = true;
			dialog.Filter = Sussex.Flip.Utils.FileExtensionFilters.TXT_ALL;
			dialog.FilterIndex = 0;
			dialog.InitialDirectory = @"C:\Sussex University\Flip\";
			dialog.Multiselect = false;
			dialog.Title = "Open script";
			dialog.ValidateNames = true;
			
			bool? result = dialog.ShowDialog(this);			
			if (!result.HasValue || !result.Value) return;
			
			string path = dialog.FileName;
			
			try {
				XmlReader reader = new XmlTextReader(path);
				
				reader.MoveToContent();
				
				bool isEmpty = reader.IsEmptyElement;
				
				reader.ReadStartElement("Moveables");
				
				if (!isEmpty) {
					
					reader.MoveToContent();
				
					while (reader.LocalName == "ObjectBlock" || reader.LocalName == "Statement") {
					
						Moveable moveable;
						
						if (reader.LocalName == "ObjectBlock") moveable = new ObjectBlock();
						else if (reader.LocalName == "Statement") moveable = new Statement();
						else throw new FormatException("Unrecognised Moveable type (" + reader.LocalName + ") or Moveable data not found.");
						
						moveable.ReadXml(reader);
						position += 10;
						Canvas.SetRight(moveable,position);
						Canvas.SetBottom(moveable,position);
						mainCanvas.Children.Add(moveable);
						
						reader.MoveToContent();
					}
					
					reader.ReadEndElement();
				}
			}
			catch (Exception x) {
				MessageBox.Show(x.ToString());
			}
		}
		
		
		protected void ExitFlip(object sender, RoutedEventArgs e)
		{
			if (!CloseCurrentScript()) return;
			
			Close();
		}
		
		
		protected void DisplayAboutScreen(object sender, RoutedEventArgs e)
		{
			new AboutWindow().ShowDialog();
		}
		
		
		public void SetTrigger(TriggerControl trigger)
		{
			if (trigger == null) throw new ArgumentNullException("trigger");
			
			triggerBar.TriggerControl = trigger;
		}
		

		protected void ScriptChanged(object sender, EventArgs e)
		{
			if (!IsDirty) IsDirty = true;
			
			UpdateNaturalLanguageView(triggerBar);
		}
		
		
		protected void UpdateNaturalLanguageView(ITranslatable translatable)
		{
			if (translatable == null) return;
			this.nlTextBlock.Text = translatable.GetNaturalLanguage();
			
			//this.targetCodeTextBlock.Text = (translatable.IsComplete ? "Complete." : "Incomplete.") + "\n\n" + translatable.GetCode();			
		}
		
		
		protected void Dropped(object sender, DragEventArgs e)
		{
			if (!e.Handled) e.Handled = true;
		}
		
		
		protected void SaveScriptToModule(object sender, RoutedEventArgs e)
		{			
			if (!triggerBar.IsComplete) {
				MessageBox.Show("Your script isn't complete! Make sure you've filled in the trigger, and " +
				                "filled in all the gaps of any actions, conditions or other blocks you've attached " +
				                "to your script before trying to compile.");
				return;
			}
			
			string code = triggerBar.GetCode();
			
			FlipScript script = new FlipScript(code);			
			
			script.Name = "flipscript";
			
			string address = triggerBar.GetAddress();
			
			try {
				attacher.Attach(script,address);
				IsDirty = false;
				MessageBox.Show("Script was saved successfully.");
			}
			catch (Exception ex) {
				MessageBox.Show(String.Format("Script could not be saved - something went wrong.\n\n{0}",ex));
			}
		}
		
		
		protected void Clear()
		{
			triggerBar.Clear();
			ClearCanvas();
		}
		
		
		protected void ClearCanvas()
		{			
			List<Moveable> moveables = new List<Moveable>(mainCanvas.Children.Count);
			foreach (UIElement element in mainCanvas.Children) {
				Moveable moveable = element as Moveable;
				if (moveable != null) moveables.Add(moveable);
			}
			foreach (Moveable moveable in moveables) {
				mainCanvas.Children.Remove(moveable);
			}
		}
		
		
		protected void ViewCode(object sender, RoutedEventArgs e)
		{
			MessageBox.Show(triggerBar.GetCode());
		}
		
		
		private void ClearCanvas(object sender, RoutedEventArgs e)
		{
			ClearCanvas();
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
	    				    				
	    				DragDropEffects effects;
	    				
	    				// Shift-drag to copy a Moveable instead of moving it:
	    				if (blockBox.HasMoveable(dragging) || Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)) {
	    					effects = DragDropEffects.Copy;
	    				}
	    				else {
	    					effects = DragDropEffects.Move;
	    				}
	    				
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