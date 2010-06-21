using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Serialization;
using Sussex.Flip.Core;
using Sussex.Flip.Utils;

namespace Sussex.Flip.UI
{
	/// <summary>
	/// Interaction logic for FlipWindow.xaml
	/// </summary>
	public partial class FlipWindow : Window
	{
		protected TriggerBar triggerBar;	
		protected MoveablesPanel blockBox;
		protected MoveableProvider provider;
		protected ImageProvider imageProvider;
				
		
		/// <summary>
		/// The delegate signature of a method to be called which will ask the user 
		/// to select a Flip-created script and open or delete that script.
		/// </summary>
		public delegate void OpenDeleteScriptDelegate();
				
		
		/// <summary>
		/// The delegate signature of a method to be called which will 
		/// save the current script to the target game artifact.
		/// </summary>
		public delegate bool SaveScriptDelegate(FlipWindow window);
		
		
		/// <summary>
		/// The method to call when the user wants to open or delete a script.
		/// </summary>
		protected OpenDeleteScriptDelegate openDeleteScriptDelegate;
		
		
		/// <summary>
		/// The method to call when the user wants to save a script.
		/// </summary>
		protected SaveScriptDelegate saveScriptDelegate;
		
		
		// TODO: Bad implementation, replace with something better.
		/// <summary>
		/// Provides deserialisation of custom ObjectBehaviours and StatementBehaviours.
		/// </summary>
		public static DeserialisationHelper ChosenDeserialisationHelper;
		
		
		public MoveablesPanel BlockBox {
			get { return blockBox; }
		}			
		
		
		public MenuItem FileMenu {
			get { return fileMenu; }
		}		
		
		
		public MenuItem EditMenu {
			get { return editMenu; }
		}		
		
		
		public MenuItem DevelopmentMenu {
			get { return developmentMenu; }
		}		
		
		
		public MenuItem AboutMenu {
			get { return aboutMenu; }
		}
		
		
		public TriggerBar TriggerBar {
			get { return triggerBar; }
		}
		
		
		protected SizedCanvas mainCanvas;
		public FlipWindow(MoveableProvider provider, 
		                  ImageProvider imageProvider, 
		                  OpenDeleteScriptDelegate openDeleteScriptDelegate,
		                  SaveScriptDelegate saveScriptDelegate,
		                  DeserialisationHelper deserialisationHelper)
		{
			if (provider == null) throw new ArgumentNullException("provider");
			if (imageProvider == null) throw new ArgumentNullException("imageProvider");
			if (openDeleteScriptDelegate == null) throw new ArgumentNullException("openDeleteScriptDelegate");
			if (saveScriptDelegate == null) throw new ArgumentNullException("saveScriptDelegate");
			if (deserialisationHelper == null) throw new ArgumentNullException("deserialisationHelper");
			
			this.provider = provider;
			this.imageProvider = imageProvider;
			this.openDeleteScriptDelegate = openDeleteScriptDelegate;
			this.saveScriptDelegate = saveScriptDelegate;
			ChosenDeserialisationHelper = deserialisationHelper;
			
			InitializeComponent();
						
			/*
			 * New SizedCanvas which unlike Canvas reports a desired size
			 * (by overriding MeasureOverride) based on the positions of
			 * its children, and hence can be used with a ScrollViewer.
			 */ 
			mainCanvas = new SizedCanvas();
			mainCanvas.Name = "mainCanvas";
			mainCanvas.Background = Brushes.Transparent;
			mainCanvas.AllowDrop = true;
			scrollViewer.Content = mainCanvas;
			
			blockBox = new MoveablesPanel();
			Grid.SetRow(blockBox,1);
			Grid.SetColumn(blockBox,1);
			Grid.SetRowSpan(blockBox,2);
			blockBox.PreviewDrop += ReturnMoveableToBox;
			
			provider.Populate(blockBox);
				
			mainGrid.Children.Add(blockBox);
							
			/*
			 * Was MouseDown but ScrollViewer suppresses this event
			 * so now using PreviewMouseDown. GetDragSource only
			 * gets a reference to what was clicked on and where,
			 * so there's no reason to believe this change would
			 * affect any other program logic, and it doesn't seem to.
			 */ 			
			PreviewMouseDown += GetDragSource;			
			MouseMove += StartDrag;
			mainCanvas.Drop += DroppedOnCanvas;	
			
			PreviewDragEnter += CreateAdorner;
			PreviewDragOver += UpdateAdorner;			
			PreviewDragLeave += DestroyAdorner;			
			PreviewDrop += DestroyAdorner;	
						
			triggerBar = new TriggerBar(new SpineFitter());
			triggerBar.SaveButton.Click += SaveScriptToModule;
			                                        
			Canvas.SetTop(triggerBar,30);
			Canvas.SetLeft(triggerBar,30);
			mainCanvas.Children.Add(triggerBar);
			
			triggerBar.Changed += ScriptChanged;
			UpdateNaturalLanguageView(triggerBar);
			
			if (mainCanvas.ContextMenu == null) mainCanvas.ContextMenu = new ContextMenu();
			MenuItem paste = new MenuItem();
			paste.Header = "Paste";
			paste.Click += delegate
			{				
				try {
					if (Moveable.CopiedToClipboard != null) {
						Moveable copied = Moveable.CopiedToClipboard.DeepCopy();	
						Point position = Mouse.GetPosition(this);
						copied.MoveTo(position);				
						PlaceInWorkspace(copied);
						ActivityLog.Write(new Activity("PlacedBlock","Block",copied.GetLogText(),"PlacedOn","Canvas"));
					}
				}
				catch (Exception x) {
					MessageBox.Show("Something went wrong when copy-pasting.\n\n" + x);
				}
			};
			mainCanvas.ContextMenu.Items.Add(paste);
			mainCanvas.ContextMenuOpening += delegate 
			{  
				paste.IsEnabled = (Moveable.CopiedToClipboard != null);
			};
		}
		
		
		protected bool isDirty = false;
		
		public bool IsDirty {
			get { return isDirty; }
			set { 
				if (isDirty != value) {
					isDirty = value;
				}
			}
		}
		
		
		/// <summary>
		/// 
		/// </summary>
		/// <returns>Returns false if the user cancelled; true otherwise.</returns>
		public MessageBoxResult AskWhetherToSaveCurrentScript()
		{
			if (!IsDirty) {
				CloseScript();
				return MessageBoxResult.None;
			}
			
			MessageBoxResult result = MessageBox.Show("Save this script before closing?",
										              "Save changes?",
										              MessageBoxButton.YesNoCancel,
										              MessageBoxImage.Question,
										              MessageBoxResult.Cancel);
			
			if (result == MessageBoxResult.Yes) {			
				try {
					bool cancelled = !SaveScriptToModule();					
					if (cancelled) return MessageBoxResult.Cancel;
				}
				catch (Exception x) {
					MessageBox.Show(String.Format("Something went wrong when saving script to module.{0}{0}{1}",Environment.NewLine,x));
					return MessageBoxResult.Cancel;
				}	
					
				CloseScript();	
			}
			
			else if (result == MessageBoxResult.No) {
				CloseScript();
			}
			
			return result;
		}
		
		
		protected void NewScript(object sender, RoutedEventArgs e)
		{
			if (AskWhetherToSaveCurrentScript() == MessageBoxResult.Cancel) return;
			ActivityLog.Write(new Activity("NewScript","CreatedVia","FileMenu","Event",String.Empty));
		}
		
		
		protected void OpenScriptFromModule(object sender, RoutedEventArgs e)
		{			
			if (AskWhetherToSaveCurrentScript() == MessageBoxResult.Cancel) return;
			
			try {
				openDeleteScriptDelegate.Invoke();
			}
			catch (Exception x) {
				MessageBox.Show(String.Format("Something went wrong when opening or deleting a script.{0}{0}{1}",Environment.NewLine,x));
			}
		}
		
		
		protected void CloseScript(object sender, RoutedEventArgs e)
		{
			if (AskWhetherToSaveCurrentScript() == MessageBoxResult.Cancel) return;
			ActivityLog.Write(new Activity("ClosedScript"));
		}
		
		
		public void CloseScript()
		{			
			try {
				Clear();
				
				triggerBar.Spine.SetPegCount(3);
				
				triggerBar.CurrentScriptIsBasedOn = String.Empty;
				
				IsDirty = false;	
			}
			catch (Exception x) {
				MessageBox.Show("Something went wrong when closing the script.\n\n" + x);
			}
		}
		
		
//		/// <summary>
//		/// Deprecated.
//		/// </summary>
//		/// <param name="sender"></param>
//		/// <param name="e"></param>
//		protected void SaveScriptToFile(object sender, RoutedEventArgs e)
//		{
//			try {
//				SaveScriptToFile();
//			}
//			catch (Exception x) {
//				MessageBox.Show(x.ToString());
//			}
//		}
		
		
//		/// <summary>
//		/// Deprecated.
//		/// </summary>
//		/// <returns>Returns false if the user cancelled; true otherwise.</returns>
//		protected bool SaveScriptToFile()
//		{
//			Microsoft.Win32.SaveFileDialog dialog = new Microsoft.Win32.SaveFileDialog();
//			dialog.AddExtension = true;
//			dialog.CheckFileExists = false;
//			dialog.CheckPathExists = true;
//			dialog.CreatePrompt = false;
//			dialog.DefaultExt = ".txt";
//			dialog.DereferenceLinks = false;			
//			dialog.Filter = Sussex.Flip.Utils.FileExtensionFilters.TXT_ALL;
//			dialog.FilterIndex = 0;			
//			dialog.InitialDirectory = @"C:\Sussex University\Flip\";
//			dialog.OverwritePrompt = true;
//			dialog.RestoreDirectory = false;
//			dialog.Title = "Save script";
//			dialog.ValidateNames = true;
//			
//			bool? result = dialog.ShowDialog(this);			
//			if (!result.HasValue || !result.Value) return false;
//			
//			string path = dialog.FileName;
//			
//			try {
//				XmlWriterSettings settings = new XmlWriterSettings();
//				settings.CloseOutput = true;
//				settings.Indent = true;
//				settings.NewLineOnAttributes = false;
//				
//				using (XmlWriter writer = XmlWriter.Create(path,settings)) {
//					new ScriptWriter(triggerBar).WriteFlipCode(writer);
//				}
//				
//				return true;
//			}
//			catch (Exception x) {
//				throw new ApplicationException("Failed to save script.",x);
//			}
//		}
		
		
//		protected void OpenScriptFromFile(object sender, RoutedEventArgs e)
//		{
//			if (AskWhetherToSaveCurrentScript() == MessageBoxResult.Cancel) return;
//			
//			Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
//			dialog.CheckFileExists = true;
//			dialog.CheckPathExists = true;
//			dialog.DefaultExt = ".txt";
//			dialog.DereferenceLinks = true;
//			dialog.Filter = Sussex.Flip.Utils.FileExtensionFilters.TXT_ALL;
//			dialog.FilterIndex = 0;
//			dialog.InitialDirectory = @"C:\Sussex University\Flip\";
//			dialog.Multiselect = false;
//			dialog.Title = "Open script";
//			dialog.ValidateNames = true;
//			
//			bool? result = dialog.ShowDialog(this);			
//			if (!result.HasValue || !result.Value) return;
//			
//			string path = dialog.FileName;
//			
//			try {
//				OpenFlipScript(path);
//			}
//			catch (Exception x) {
//				MessageBox.Show(x.ToString());
//			}
//		}
		
		
		public void OpenFlipScript(string path)
		{
			if (String.IsNullOrEmpty(path)) throw new ArgumentException("Invalid path.","path");
			if (!File.Exists(path)) throw new ArgumentException("Invalid path - file does not exist.","path");
			
			using (XmlReader reader = new XmlTextReader(path)) {
				LoadFlipCodeFromReader(reader);
			}			
			
			// Update the property 'CurrentScriptIsBasedOn', since the next time
			// the script is saved it will be as a new script based on the one
			// that we've just opened. (This essentially means that we never use
			// the value we're deserialising in code - it's so we can analyse
			// the script file at a later date.)
			string scriptName = Path.GetFileNameWithoutExtension(path);
			triggerBar.CurrentScriptIsBasedOn = scriptName;
			
			IsDirty = false;
		}
		
		
		public void OpenFlipScript(ScriptTriggerTuple tuple)
		{			
			if (tuple == null) throw new ArgumentNullException("tuple");
			
			CloseScript();
			
			if (tuple.Script != null) {				
				using (TextReader tr = new StringReader(tuple.Script.Code)) {
					XmlReader reader = new XmlTextReader(tr);
					LoadFlipCodeFromReader(reader);
				}				
			
				// Update the property 'CurrentScriptIsBasedOn', since the next time
				// the script is saved it will be as a new script based on the one
				// that we've just opened. (This essentially means that we never use
				// the value we're deserialising in code - it's so we can analyse
				// the script file at a later date.)
				triggerBar.CurrentScriptIsBasedOn = tuple.Script.Name;				
			}
			
			if (tuple.Trigger != null) {
				SetTrigger(tuple.Trigger);
			}
			
			IsDirty = false;
		}
		
		
		protected void LoadFlipCodeFromReader(XmlReader reader)
		{
			if (reader == null) throw new ArgumentNullException("reader");
			
			CloseScript();
						
			reader.MoveToContent();				
			triggerBar.ReadXml(reader);
			triggerBar.AssignImage(imageProvider);
		}
		
		
		protected void ExitFlip(object sender, RoutedEventArgs e)
		{
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
		

		string previousNaturalLanguageValue = String.Empty;
		protected void ScriptChanged(object sender, EventArgs e)
		{
			try {
				if (!IsDirty) IsDirty = true;
				
				string nl = UpdateNaturalLanguageView(triggerBar);
				
				if (nl != previousNaturalLanguageValue) {
					ActivityLog.Write(new Activity("ScriptDump","NLOutput",nl));	
					previousNaturalLanguageValue = nl;
				}
			}
			catch (Exception x) {
				MessageBox.Show("Something went wrong when responding to the script changing.\n\n" + x);
			}
		}
		
		
		protected string UpdateNaturalLanguageView(ITranslatable translatable)
		{
			if (translatable == null) return null;
			string nl = translatable.GetNaturalLanguage();
			this.nlTextBlock.Text = nl;
			return nl;
		}
		
		
		protected void Dropped(object sender, DragEventArgs e)
		{
			if (!e.Handled) e.Handled = true;
		}
		
				
		protected void SaveScriptToModule(object sender, RoutedEventArgs e)
		{			
			try {
				SaveScriptToModule();
			}
			catch (Exception x) {
				MessageBox.Show(String.Format("Something went wrong when saving script to module.{0}{0}{1}",Environment.NewLine,x));
			}
		}
			
				
		/// <summary>
		/// 
		/// </summary>
		/// <returns>Returns false if the script was incomplete and the save operation was aborted;
		/// true otherwise.</returns>
		protected bool SaveScriptToModule()
		{			
			return saveScriptDelegate.Invoke(this);
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
		
		
		protected void ClearCanvas(object sender, RoutedEventArgs e)
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
    		catch (Exception x) {
    			MessageBox.Show(String.Format("Something went wrong when drag-dropping.{0}{1}",Environment.NewLine,x.ToString()));
    		}
    	}    	
    	

    	protected void GetDragSource(object sender, MouseEventArgs e)
    	{
    		try {
	    		FrameworkElement f = e.OriginalSource as FrameworkElement;
	    		    		
	    		if (f == null) return;
	    		
	    		while (!(f is Moveable) && (f = f.Parent as FrameworkElement) != null);
	    		
	    		if (f is Moveable) {
	    			dragPos = e.GetPosition(null);
	    			dragging = (Moveable)f;
	    		}
    		}
    		catch (Exception x) {
    			MessageBox.Show(String.Format("Something went wrong when identifying the source of a drag-drop.{0}{1}",Environment.NewLine,x.ToString()));
    		}
    	}			
		
		
		protected void DroppedOnCanvas(object sender, DragEventArgs e)
		{		
			try {
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
	//				// HACK:
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
	//					catch (System.Runtime.InteropServices.COMException x) {
	//						
	//						MessageBox.Show(x.ToString());
	//						/*
	//						 * Weird error occurs here - even though GetDataPresent() returns true,
	//						 * actually trying to retrieve the data raises this nasty exception.
	//						 * TODO:
	//						 * Look for the blueprints directly in the toolset instead.
	//						 */
	//					}
	//				}			
					
					if (moveable != null) {		
						bool movingWithinCanvas = moveable.Parent == mainCanvas;
						
						PlaceInWorkspace(moveable);
						
						Point position = e.GetPosition(mainCanvas);
						position.X -= (size.Width/2);
						position.Y -= (size.Height/2);
						moveable.MoveTo(position);
						
						if (movingWithinCanvas) {
							ActivityLog.Write(new Activity("MovedWithinCanvas","Block",moveable.GetLogText()));	
						}
						else {
							ActivityLog.Write(new Activity("PlacedBlock","Block",moveable.GetLogText(),"PlacedOn","Canvas"));						
						}
					}
				}
			}
			catch (Exception x) {
				MessageBox.Show("Something went wrong when handling a drop on the canvas.\n\n" + x);
			}
		}
		

		protected void ReturnMoveableToBox(object sender, DragEventArgs e)
		{
			try {
				if (!e.Handled) {
					if (e.Data.GetDataPresent(typeof(Moveable))) {
						Moveable moveable = (Moveable)e.Data.GetData(typeof(Moveable));
						if (!blockBox.HasMoveable(moveable)) {
							moveable.Remove();
							ActivityLog.Write(new Activity("PlacedBlock","Block",moveable.GetLogText(),"PlacedOn","BackInBox"));
						}
						e.Handled = true;
					}
				}
			}
			catch (Exception x) {
				MessageBox.Show("Something went wrong when handling a drop on the block box.\n\n" + x);
			}
		}
		
		
		protected int zIndex = 0;
		public void PlaceInWorkspace(Moveable moveable)
		{
			try {
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
				
				mainCanvas.InvalidateMeasure();
			}
			catch (Exception x) {
				MessageBox.Show("Something went wrong when placing a moveable on the canvas.\n\n" + x);
			} 
		}
	}
}