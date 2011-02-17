using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Sussex.Flip.Games.NeverwinterNightsTwo.Utils;
using Sussex.Flip.UI;
using Sussex.Flip.Utils;

namespace Sussex.Flip.Games.NeverwinterNightsTwo
{
    /// <summary>
    /// Interaction logic for ScriptSelector.xaml
    /// </summary>
    public partial class ScriptSelector : Window
    {	
    	protected List<ScriptTriggerTuple> tuples;
    	protected FlipWindow window;
    	protected INwn2Session session;
    	protected ScriptTriggerTuple selected;
    	
    	
    	private static Thickness thickness = new Thickness(3);
    	    	
    	
    	public ScriptSelector(List<ScriptTriggerTuple> tuples, INwn2Session session, FlipWindow window)
        {
    		if (tuples == null) throw new ArgumentNullException("tuples");
    		if (session == null) throw new ArgumentNullException("session");
    		if (window == null) throw new ArgumentNullException("window");
    		this.tuples = tuples;
    		this.session = session;
    		this.window = window;
    		
    		selected = null;
    		
            InitializeComponent();
            
            MouseButtonEventHandler scriptSelected = new MouseButtonEventHandler(OpenScript);
            scriptsListBox.MouseDoubleClick += scriptSelected;
            scriptsListBox.KeyDown += new KeyEventHandler(OpenScript);
            
            foreach (ScriptTriggerTuple tuple in tuples) {
            	try {
	            	TriggerControl trigger = tuple.Trigger;
	            	
	            	if (trigger == null) continue;
	            	
	            	trigger.Padding = thickness;
	            	scriptsListBox.Items.Add(trigger);
            	}
            	catch (Exception) {}
            }
        }
    	
    	
    	protected void DeleteSelectedScript()
    	{
        	TriggerControl t = scriptsListBox.SelectedItem as TriggerControl;
        	
        	if (t == null) {
        		MessageBox.Show("Select a script to delete.");
        	}
        	else {
        		MessageBoxResult result = MessageBox.Show("Are you sure you want to permanently delete this script?",
        		                                          "Delete script?",
        		                                          MessageBoxButton.YesNo,
        		                                          MessageBoxImage.Warning,
        		                                          MessageBoxResult.No);
        		
        		if (result == MessageBoxResult.No) return;
        		
        		// HACK
        		// Should actually be adding ScriptTriggerTuple directly
        		// to the ListBox and representing it with a DataTemplate
        		// so that only the TriggerControl is displayed, but
        		// that's too finicky to get into just now, so add the
        		// TriggerControl directly and search for the ScriptSlotTuple
        		// that possesses it.
        		
        		selected = GetOwner(t);
        		
        		try {
					session.DeleteScript(selected.Script.Name);					
					scriptsListBox.Items.Remove(scriptsListBox.SelectedItem);
						
					if (selected.Trigger != null) 
						Log.WriteAction(LogAction.deleted,"script",selected.Script.Name + " (was attached to '" + selected.Trigger.GetLogText() + "')");
					else
						Log.WriteAction(LogAction.deleted,"script",selected.Script.Name);
        		}
        		catch (Exception x) {
        			MessageBox.Show("Something went wrong when deleting script.\n" + x);
        		}
        	}
    	}

    	
    	protected void OpenScript(object sender, KeyEventArgs e)
    	{
    		if (e.Key == Key.Enter) OpenSelectedScript();
    		else if (e.Key == Key.Delete) DeleteSelectedScript();
    	}
    	
    	
        protected void OpenScript(object sender, EventArgs e)
        {
        	OpenSelectedScript();
        }
    	
    	
        protected void DeleteScript(object sender, EventArgs e)
        {
        	DeleteSelectedScript();
        }
        
        
        protected void OpenSelectedScript()
        {
        	TriggerControl t = scriptsListBox.SelectedItem as TriggerControl;
        	
        	if (t == null) {
        		MessageBox.Show("Select a script to open.");
        	}
        	else {
        		// HACK
        		// Should actually be adding ScriptTriggerTuple directly
        		// to the ListBox and representing it with a DataTemplate
        		// so that only the TriggerControl is displayed, but
        		// that's too finicky to get into just now, so add the
        		// TriggerControl directly and search for the ScriptSlotTuple
        		// that possesses it.
        		
        		selected = GetOwner(t);        		
        		
        		try {
	        		if (selected.Trigger is BlankTrigger) selected.Trigger = null;							
					window.OpenFlipScript(selected.DeepCopy());
					
					if (selected.Trigger != null) 
						Log.WriteAction(LogAction.opened,"script",selected.Script.Name + " (attached to '" + selected.Trigger.GetLogText() + "')");
					else 
						Log.WriteAction(LogAction.opened,"script",selected.Script.Name);
					
        			Close();
        		}
        		catch (Exception x) {
        			MessageBox.Show("Something went wrong when opening script.\n" + x);
        		}        		
        	}
        }
        
        
        protected ScriptTriggerTuple GetOwner(TriggerControl triggerControl)
        {
        	foreach (ScriptTriggerTuple tuple in tuples) {
        		if (tuple.Trigger == triggerControl) return tuple;        		
        	}
        	return null;
        }
        
        
        protected void CancelOpen(object sender, EventArgs e)
        {
        	Close();
        }
    }
}