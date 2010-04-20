﻿using System;
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
		
		
		/// <summary>
		/// Check whether this Flip component has all essential fields filled in,
		/// including those belonging to subcomponents, such that it can generate valid code.
		/// </summary>
		/// <returns>True if all essential fields have been given values; false otherwise.</returns>
		/// <remarks>Note that this method makes no attempt to judge whether the values
		/// are valid in their slots, only that those slots have been filled.</remarks>
		public override bool IsComplete { 
			get { return true; } 
		}	
    	
    	
    	public EventBehaviour Behaviour {
    		get { return (EventBehaviour)base.GetValue(BehaviourProperty); }
    		set { 
    			if (Behaviour != value) {
    				base.SetValue(BehaviourProperty,value); 
    				OnChanged(new EventArgs());
    			}
    		}
    	}
    	
    	
    	public string DisplayName {
    		get { return Behaviour.DisplayName; }
    		set { 
    			if (Behaviour.DisplayName != value) {
    				Behaviour.DisplayName = value;
    				OnChanged(new EventArgs());
    			}
    		}
    	}
    	
    	
    	public string EventName {
    		get { return Behaviour.EventName; }
    		set { 
    			if (Behaviour.EventName != value) {
    				Behaviour.EventName = value;
    				OnChanged(new EventArgs());
    			}
    		}
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