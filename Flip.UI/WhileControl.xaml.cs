﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Sussex.Flip.UI
{
    /// <summary>
    /// Interaction logic for WhileControl.xaml
    /// </summary>
    public partial class WhileControl : ControlStructure
    {
    	protected ConditionSlot slot;
    	protected Spine consequenceSpine;
    	
    	
    	public override Moveable Condition {
    		get { 
    			return slot.Contents;
    		}
    		set {     		
    			if (value == null || (value is Statement && ((Statement)value).StatementType == StatementType.Condition) || value is BooleanBlock) {
    				slot.Contents = value;
    			}
    			else {
    				throw new ArgumentException("Can only assign a conditional statement or a boolean expression as the condition of a control structure.");
    			}
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
	        	
	            Grid.SetRow(consequenceSpine,3);
	            Grid.SetColumn(consequenceSpine,0);
            	grid.Children.Add(consequenceSpine);
    		}
		}
		
		
		/// <summary>
		/// Check whether this Flip component has all essential fields filled in,
		/// including those belonging to subcomponents, such that it can generate valid code.
		/// </summary>
		/// <returns>True if all essential fields have been given values; false otherwise.</returns>
		/// <remarks>Note that this method makes no attempt to judge whether the values
		/// are valid in their slots, only that those slots have been filled.</remarks>
		public override bool IsComplete { 
			get { 
				return Condition != null && Condition.IsComplete && Consequences.IsComplete;
			}
		}		
    	
    	
        public WhileControl()
        {
        	slot = new ConditionSlot(new BooleanExpressionFitter());
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
            
            Grid.SetRow(slot,1);
            Grid.SetColumn(slot,0);
            grid.Children.Add(slot);
                        
            Grid.SetRow(consequenceSpine,3);
            Grid.SetColumn(consequenceSpine,0);
            grid.Children.Add(consequenceSpine);
        }

        
		public override Moveable DeepCopy()
		{	
			WhileControl copy = new WhileControl();
			
			if (Condition != null) {
				copy.Condition = Condition.DeepCopy();
			}
			
			copy.Consequences = Consequences.DeepCopy();
			
			return copy;
		}
    	
        
		public override string GetCode()
		{
			System.Text.StringBuilder code = new System.Text.StringBuilder();
			
			code.AppendLine(String.Format("while ({0}) {{",slot.GetCode()));
			code.AppendLine(consequenceSpine.GetCode());			
			code.AppendLine("}");		
			
			return code.ToString();
		}
    	
        
		public override string GetNaturalLanguage()
		{
			string whileText = slot.GetNaturalLanguage();
			string doText = consequenceSpine.GetNaturalLanguage();
			
			return String.Format("while {0}, {1}",whileText,doText);
		}
		
		
		public override void AssignImage(ImageProvider imageProvider)
		{
			if (imageProvider == null) throw new ArgumentNullException("imageProvider");
			
			if (Condition != null) Condition.AssignImage(imageProvider);
			if (Consequences != null) Consequences.AssignImage(imageProvider);
		}
		
		
		public override string GetLogText()
		{
			return "While";
		}
        
        
		public override ScriptStats GetStatistics()
		{		
			ScriptStats s = new ScriptStats();
			s.WhileLoop++;
			s.Add(slot.GetStatistics());
			s.Add(Consequences.GetStatistics());
			return s;
		}
    }
}