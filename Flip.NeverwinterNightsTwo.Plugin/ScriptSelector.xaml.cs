using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Sussex.Flip.Games.NeverwinterNightsTwo.Utils;
using Sussex.Flip.UI;

namespace Sussex.Flip.Games.NeverwinterNightsTwo
{
    /// <summary>
    /// Interaction logic for ScriptSelector.xaml
    /// </summary>
    public partial class ScriptSelector : Window
    {	
    	protected List<ScriptTriggerTuple> tuples;
    	protected ScriptTriggerTuple selected;
    	
    	
		public ScriptTriggerTuple Selected {
			get { return selected; }
		}
    	
    	
    	public ScriptSelector() : this(new List<ScriptTriggerTuple>(0))
    	{    		
    	}
    	
    	
    	public ScriptSelector(List<ScriptTriggerTuple> tuples)
        {
    		if (tuples == null) throw new ArgumentNullException("tuples");
    		this.tuples = tuples;
    		
    		selected = null;
    		
    		//List<TriggerControl> triggers = GetDefaultTriggersFromAddresses();
            InitializeComponent();
            
            MouseButtonEventHandler scriptSelected = new MouseButtonEventHandler(OpenScript);
            scriptsListBox.MouseDoubleClick += scriptSelected;
            scriptsListBox.KeyDown += new KeyEventHandler(OpenScript);
            
            Thickness thickness = new Thickness(3);
            
            
            foreach (ScriptTriggerTuple tuple in tuples) {
            	// HACK
            	TriggerControl trigger = tuple.Trigger;
            	
            	trigger.LayoutTransform = new System.Windows.Media.ScaleTransform(0.7,0.7);
            	trigger.Padding = thickness;
            	scriptsListBox.Items.Add(trigger);
            }
        }
    	
    	
    	private void DeleteSelectedScript()
    	{
    		
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
        		Close();
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