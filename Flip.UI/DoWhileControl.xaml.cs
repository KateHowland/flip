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
    /// Interaction logic for DoWhileControl.xaml
    /// </summary>
    public partial class DoWhileControl : ConditionalControl
    {
    	protected ConditionSlot slot;
    	protected Spine consequenceSpine;
    	
    	
    	public override Statement Condition {
    		get { 
    			return slot.Contents as Statement; 
    		}
    		set {     			
    			if (value != null && value.StatementType != StatementType.Condition) {
    				throw new ArgumentException("Statement must have StatementType.Condition to be assigned as the condition of a ConditionalControl.");
    			}
    			slot.Contents = value;
    		}
    	}
    	
    	
		public override Spine Consequences {
			get {
				return consequenceSpine;
			}
    		set {
    			if (value == null) {
    				throw new ArgumentNullException("value");
    			}    			
    			
    			grid.Children.Remove(consequenceSpine);
    			
    			consequenceSpine = value;
        	
	        	consequenceSpine.Changed += delegate 
	        	{ 
	        		OnChanged(new EventArgs()); 
	        	};
	        	
	            Grid.SetRow(consequenceSpine,1);
	            Grid.SetColumn(consequenceSpine,0);
            	grid.Children.Add(consequenceSpine);
    		}
		}
    	
    	
        public DoWhileControl()
        {
        	slot = new ConditionSlot(new ConditionFitter());
        	slot.Padding = new Thickness(10);
        	
        	consequenceSpine = new Spine(new SpineFitter(),1,10);
        	
        	slot.Changed += delegate 
        	{ 
        		OnChanged(new EventArgs());
        	};
        	
        	consequenceSpine.Changed += delegate 
        	{ 
        		OnChanged(new EventArgs()); 
        	};
            
            InitializeComponent();
            
            Grid.SetRow(slot,3);
            Grid.SetColumn(slot,0);
            grid.Children.Add(slot);
                        
            Grid.SetRow(consequenceSpine,1);
            Grid.SetColumn(consequenceSpine,0);
            grid.Children.Add(consequenceSpine);
        }

        
		public override Moveable DeepCopy()
		{	
			DoWhileControl copy = new DoWhileControl();
			
			if (Condition != null) {
				copy.Condition = (Statement)Condition.DeepCopy();
			}
			
			copy.Consequences = Consequences.DeepCopy();
			
			return copy;
		}
    	
        
		public override string GetCode()
		{
			System.Text.StringBuilder code = new System.Text.StringBuilder();
			
			code.AppendLine("do {");
			code.AppendLine(consequenceSpine.GetCode());			
			code.AppendLine(String.Format("}} while ({0});",slot.GetCode()));
			
			return code.ToString();
		}
    	
        
		public override string GetNaturalLanguage()
		{
			string whileText = slot.GetNaturalLanguage();
			string doText = consequenceSpine.GetNaturalLanguage();
			
			return String.Format("this happens: {0}... and then keeps happening until {1}",doText,whileText);
		}
    }
}