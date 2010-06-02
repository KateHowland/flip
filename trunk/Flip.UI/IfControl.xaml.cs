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
    /// Interaction logic for IfControl.xaml
    /// </summary>
    public partial class IfControl : ConditionalControl
    {
    	protected ConditionSlot slot;
    	protected Spine spine;
    	
    	
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
				return spine;
			}
    		set {
    			if (value == null) {
    				throw new ArgumentNullException("value");
    			}    			
    			
    			grid.Children.Remove(spine);
    			
    			spine = value;
        	
	        	spine.Changed += delegate 
	        	{ 
	        		OnChanged(new EventArgs()); 
	        	};
	        	
	            Grid.SetRow(spine,3);
	            Grid.SetColumn(spine,0);
            	grid.Children.Add(spine);
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
    	
    	
        public IfControl()
        {
        	slot = new ConditionSlot(new ConditionFitter());
        	slot.Padding = new Thickness(10);
        	
        	spine = new Spine(new SpineFitter(),1,10);
        	
        	slot.Changed += delegate 
        	{ 
        		OnChanged(new EventArgs());
        	};
        	
        	spine.Changed += delegate 
        	{ 
        		OnChanged(new EventArgs()); 
        	};
            
            InitializeComponent();
            
            Grid.SetRow(slot,1);
            Grid.SetColumn(slot,0);
            grid.Children.Add(slot);
                        
            Grid.SetRow(spine,3);
            Grid.SetColumn(spine,0);
            grid.Children.Add(spine);
        }

        
		public override Moveable DeepCopy()
		{	
			IfControl copy = new IfControl();
			
			if (Condition != null) {
				copy.Condition = (Statement)Condition.DeepCopy();
			}
			
			copy.Consequences = Consequences.DeepCopy();
			
			return copy;
		}
    	
        
		public override string GetCode()
		{
			System.Text.StringBuilder code = new System.Text.StringBuilder();
			
			code.AppendLine(String.Format("if ({0}) {{",slot.GetCode()));
			code.AppendLine(spine.GetCode());			
			code.AppendLine("}");
			
			return code.ToString();
		}
    	
        
		public override string GetNaturalLanguage()
		{
			return String.Format("if {0}, {1}",slot.GetNaturalLanguage(),spine.GetNaturalLanguage());
		}
		
		
		public override void AssignImage(ImageProvider imageProvider)
		{
			if (imageProvider == null) throw new ArgumentNullException("imageProvider");
			
			if (Condition != null) Condition.AssignImage(imageProvider);
			if (Consequences != null) Consequences.AssignImage(imageProvider);
		}
    }
}