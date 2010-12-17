using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;
using Sussex.Flip.Utils;
using Sussex.Flip.Core;

namespace Sussex.Flip.UI
{
	/// <summary>
	/// Interaction logic for FlipWindow.xaml
	/// </summary>
	public partial class FlipWindow : Window
	{
		protected TriggerBar triggerBar;	
		protected ConditionalFrame conditionalFrame;
		protected MoveablesPanel blockBox;
		protected MoveableProvider provider;
		protected ImageProvider imageProvider;
		protected ScriptType mode;
						
		
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
		
		
		public ConditionalFrame ConditionalFrame {
			get { return conditionalFrame; }
		}
		
		
		/// <summary>
		/// The current Flip mode, either Standard or Conditional.
		/// </summary>
		public ScriptType Mode {
			get { return mode; }
		}
		
		
		protected SizedCanvas mainCanvas;
		public FlipWindow(MoveableProvider provider, 
		                  ImageProvider imageProvider, 
		                  OpenDeleteScriptDelegate openDeleteScriptDelegate,
		                  SaveScriptDelegate saveScriptDelegate,
		                  DeserialisationHelper deserialisationHelper,
		                  ScriptType mode)
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
						
			// Set up trigger bar:
			triggerBar = new TriggerBar(new SpineFitter());
			triggerBar.SaveButton.Click += SaveScript;			                                        
			Canvas.SetTop(triggerBar,30);
			Canvas.SetLeft(triggerBar,30);
			mainCanvas.Children.Add(triggerBar);			
			triggerBar.Changed += ScriptChanged;
			UpdateNaturalLanguageView(triggerBar);
						
			// Set up conditional frame:
			conditionalFrame = new ConditionalFrame();
			conditionalFrame.SaveButton.Click += SaveScript;	
			conditionalFrame.FinishButton.Click += LeaveConditionModeWithDialog;
			Canvas.SetTop(conditionalFrame,30);
			Canvas.SetLeft(conditionalFrame,30);
			mainCanvas.Children.Add(conditionalFrame);			
			conditionalFrame.Changed += ScriptChanged;
			UpdateNaturalLanguageView(conditionalFrame);
			
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
						//ActivityLog.Write(new Activity("PlacedBlock","Block",copied.GetLogText(),"PlacedOn","Canvas"));
						Log.WriteAction(LogAction.placed,"block",copied.GetLogText() + " on canvas");
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
			
			if (mode == ScriptType.Conditional) EnterConditionMode();
			else LeaveConditionMode();
		}
		
		
		public FlipWindow(MoveableProvider provider, 
		                  ImageProvider imageProvider, 
		                  OpenDeleteScriptDelegate openDeleteScriptDelegate,
		                  SaveScriptDelegate saveScriptDelegate,
		                  DeserialisationHelper deserialisationHelper) 
			: this(provider,imageProvider,openDeleteScriptDelegate,saveScriptDelegate,deserialisationHelper,ScriptType.Standard)
		{			
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
		/// Returns true if there is a complete script currently open, false otherwise.
		/// </summary>
		public bool IsComplete {
			get {
				return (Mode == ScriptType.Standard && TriggerBar.IsComplete) || 
					   (Mode == ScriptType.Conditional && ConditionalFrame.IsComplete);
			}
		}
		
		
		/// <summary>
		/// Return the script frame which is currently active, based on what type
		/// of script the user is creating (conditional or standard).
		/// </summary>
		/// <returns></returns>
		public IScriptFrame GetCurrentScriptFrame()
		{
			if (mode == ScriptType.Conditional) return conditionalFrame;
			else return triggerBar;
		}
		
		
		public void EnterConditionMode()
		{
			EnterConditionMode(String.Empty);
		}
		
		
		public void EnterConditionMode(string dialogue)
		{
			conditionalFrame.Dialogue = dialogue;
			
			Clear();
			
			mainMenu.IsEnabled = false;
			
			blockBox.DisplayBag(MoveableProvider.ConditionsBagName);
			
			triggerBar.IsEnabled = false;
			triggerBar.Visibility = Visibility.Hidden;
			
			conditionalFrame.IsEnabled = true;
			conditionalFrame.Visibility = Visibility.Visible;
			
			UpdateNaturalLanguageView(conditionalFrame);
			
			mode = ScriptType.Conditional;
		}
		
		
		public void LeaveConditionMode()
		{
			Clear();
			
			mainMenu.IsEnabled = true;
						
			blockBox.DisplayBag(MoveableProvider.ActionsBagName);
			
			triggerBar.IsEnabled = true;
			triggerBar.Visibility = Visibility.Visible;
			
			conditionalFrame.IsEnabled = false;
			conditionalFrame.Visibility = Visibility.Hidden;
			
			UpdateNaturalLanguageView(triggerBar);
			
			mode = ScriptType.Standard;
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
					bool cancelled = !SaveScript();					
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
			//ActivityLog.Write(new Activity("NewScript","CreatedVia","FileMenu","Event",String.Empty));
			Log.WriteAction(LogAction.added,"script","via file menu");
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
			//ActivityLog.Write(new Activity("ClosedScript"));
			Log.WriteAction(LogAction.closed,"script");
		}
		
		
		public void CloseScript()
		{			
			try {
				triggerBar.Spine.SetPegCount(3);
				
				triggerBar.CurrentScriptIsBasedOn = String.Empty;
				conditionalFrame.CurrentScriptIsBasedOn = String.Empty;
				
				Clear();				
			}
			catch (Exception x) {
				MessageBox.Show("Something went wrong when closing the script.\n\n" + x);
			}
		}
				
		#region Deprecated		
		
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
		
//		public void OpenFlipScript(string path)
//		{
//			if (String.IsNullOrEmpty(path)) throw new ArgumentException("Invalid path.","path");
//			if (!File.Exists(path)) throw new ArgumentException("Invalid path - file does not exist.","path");
//			
//			using (XmlReader reader = new XmlTextReader(path)) {
//				LoadFlipCodeFromReader(reader);
//			}			
//			
//			// Update the property 'CurrentScriptIsBasedOn', since the next time
//			// the script is saved it will be as a new script based on the one
//			// that we've just opened. (This essentially means that we never use
//			// the value we're deserialising in code - it's so we can analyse
//			// the script file at a later date.)
//			string scriptName = Path.GetFileNameWithoutExtension(path);
//			triggerBar.CurrentScriptIsBasedOn = scriptName;
//			conditionalFrame.CurrentScriptIsBasedOn = scriptName;
//			
//			IsDirty = false;
//		}
		
		#endregion
		
		
		public void OpenFlipScript(ScriptTriggerTuple tuple)
		{			
			if (tuple == null) throw new ArgumentNullException("tuple");
			
			CloseScript();
			
			ScriptType scriptType;
			
			if (tuple.Trigger != null) {
				scriptType = ScriptType.Standard;
			}
			
			else if (tuple.Script != null) {
				scriptType = tuple.Script.ScriptType;
			}
			
			else { // Both trigger and script are null, so do nothing.				
				return;
			}
						
			if (tuple.Script != null) {				
				using (TextReader tr = new StringReader(tuple.Script.Code)) {
					XmlReader reader = new XmlTextReader(tr);
					LoadFlipCodeFromReader(reader,scriptType);
				}				
			
				// Update the property 'CurrentScriptIsBasedOn', since the next time
				// the script is saved it will be as a new script based on the one
				// that we've just opened. (This essentially means that we never use
				// the value we're deserialising in code - it's so we can analyse
				// the script file at a later date.)
				triggerBar.CurrentScriptIsBasedOn = tuple.Script.Name;	
				conditionalFrame.CurrentScriptIsBasedOn = tuple.Script.Name;
			}
			
			if (tuple.Trigger != null) {
				SetTrigger(tuple.Trigger);
			}
			
			IsDirty = false;
		}
		
		
		protected void LoadFlipCodeFromReader(XmlReader reader, ScriptType scriptType)
		{
			if (reader == null) throw new ArgumentNullException("reader");
			
			CloseScript();
						
			reader.MoveToContent();		
			
			if (scriptType == ScriptType.Conditional) {
				conditionalFrame.ReadXml(reader);
				conditionalFrame.AssignImage(imageProvider);
			}
			
			else {
				triggerBar.ReadXml(reader);
				triggerBar.AssignImage(imageProvider);
			}
		}
		
		
		protected void ExitFlip(object sender, RoutedEventArgs e)
		{
			Close();
		}
		
		
		protected void CopyNaturalLanguage(object sender, RoutedEventArgs e)
		{
			System.Windows.Clipboard.SetText(nlTextBlock.Text);
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
				
				ITranslatable nlProvider = sender as ITranslatable;
				if (nlProvider == null) throw new InvalidOperationException("Sender does not implement ITranslatable.");
				
				string nl = UpdateNaturalLanguageView(nlProvider);
				
				if (nl != previousNaturalLanguageValue) {
					//ActivityLog.Write(new Activity("ScriptDump","NLOutput",nl));	
					Log.WriteMessage("script output: " + nl.Replace(Environment.NewLine,String.Empty)); // remove new line characters
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
		
				
		protected void SaveScript(object sender, RoutedEventArgs e)
		{			
			try {
				SaveScript();
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
		protected bool SaveScript()
		{			
			return saveScriptDelegate.Invoke(this);
		}
		
		
		protected void LeaveConditionModeWithDialog(object sender, RoutedEventArgs e)
		{		
			LeaveConditionModeWithDialog();
		}
		
		
		protected void LeaveConditionModeWithDialog()
		{
			if (IsDirty) {
				MessageBoxResult result = MessageBox.Show("Save this condition?",
											              "Save?",
											              MessageBoxButton.YesNoCancel,
											              MessageBoxImage.Question,
											              MessageBoxResult.Cancel);
				
				switch (result) {
					case MessageBoxResult.Cancel:
						return;
						
					case MessageBoxResult.Yes:	
						try {
							bool cancelled = !SaveScript();					
							if (cancelled) return;
						}
						catch (Exception x) {
							MessageBox.Show(String.Format("Something went wrong when saving.{0}{0}{1}",Environment.NewLine,x));
							return;
						}
						break;
				}
			}
			
			CloseScript();
			LeaveConditionMode();
			Close();
		}
		
		
		protected void Clear()
		{
			triggerBar.Clear();
			conditionalFrame.Clear();
			ClearCanvas();
			IsDirty = false;
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
							//ActivityLog.Write(new Activity("MovedWithinCanvas","Block",moveable.GetLogText()));	
							Log.WriteAction(LogAction.moved,"block",moveable.GetLogText() + " to different spot on canvas");
						}
						else {
							//ActivityLog.Write(new Activity("PlacedBlock","Block",moveable.GetLogText(),"PlacedOn","Canvas"));	
							Log.WriteAction(LogAction.placed,"block",moveable.GetLogText() + " on canvas");
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
							//ActivityLog.Write(new Activity("PlacedBlock","Block",moveable.GetLogText(),"PlacedOn","BackInBox"));
			    			Log.WriteAction(LogAction.placed,"block",moveable.GetLogText() + " back in box");
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