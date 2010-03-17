using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Sussex.Flip.UI
{
    /// <summary>
    /// Interaction logic for EventBlock.xaml
    /// </summary>

    public partial class EventBlock : Moveable
    {
    	protected static DependencyProperty DisplayNameProperty;
    	protected static DependencyProperty EventNameProperty;
    	
    	
    	public string DisplayName {
    		get { return (string)base.GetValue(DisplayNameProperty); }
    		set { base.SetValue(DisplayNameProperty,value); }
    	}
    	
    	    	
    	public string EventName {
    		get { return (string)base.GetValue(EventNameProperty); }
    		set { base.SetValue(EventNameProperty,value); }
    	}
    	
    	
    	static EventBlock()
    	{
    		DisplayNameProperty = DependencyProperty.Register("DisplayName",typeof(string),typeof(EventBlock));
    		EventNameProperty = DependencyProperty.Register("EventName",typeof(string),typeof(EventBlock));
    	}
    	
    	
    	public EventBlock(string eventName) : this(eventName,eventName)
        {
        }
    	
    	
        public EventBlock(string eventName, string displayName)
        {
        	if (eventName == null) throw new ArgumentNullException("eventName");
        	if (displayName == null) throw new ArgumentNullException("displayName");
        	
        	EventName = eventName;
            DisplayName = displayName;
            InitializeComponent();
        }

        
		public override Moveable DeepCopy()
		{
			return new EventBlock(DisplayName);
		}
    }
}