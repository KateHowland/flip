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
    	protected static DependencyProperty BehaviourProperty;
    	
    	
    	public EventBehaviour Behaviour {
    		get { return (EventBehaviour)base.GetValue(BehaviourProperty); }
    		set { base.SetValue(BehaviourProperty,value); }
    	}
    	
    	
    	public string DisplayName {
    		get { return Behaviour.DisplayName; }
    		set { Behaviour.DisplayName = value; }
    	}
    	
    	    	
    	public string EventName {
    		get { return Behaviour.EventName; }
    		set { Behaviour.EventName = value; }
    	}
    	
    	
    	static EventBlock()
    	{
    		BehaviourProperty = DependencyProperty.Register("Behaviour",typeof(EventBehaviour),typeof(EventBlock));
    	}
    	
    	
    	public EventBlock(string eventName) : this(eventName,eventName)
        {
        }
    	
    	
        public EventBlock(string eventName, string displayName)
        {
        	if (eventName == null) throw new ArgumentNullException("eventName");
        	if (displayName == null) throw new ArgumentNullException("displayName");
        	
        	Behaviour = new EventBehaviour(eventName,displayName);
        	
            InitializeComponent();
        }
        
        
        public EventBlock(EventBehaviour behaviour)
        {
        	if (behaviour == null) throw new ArgumentNullException("behaviour");
        	
        	Behaviour = behaviour;
        	
            InitializeComponent();
        }

        
		public override Moveable DeepCopy()
		{
			return new EventBlock(Behaviour);
		}
		
		
		public override string GetCode()
		{
			return EventName;
		}
		
		
		public override string GetNaturalLanguage()
		{
			return DisplayName;
		}
    }
}