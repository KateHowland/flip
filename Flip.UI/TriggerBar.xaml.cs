﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Effects;

namespace Sussex.Flip.UI
{
    /// <summary>
    /// Interaction logic for TriggerBar.xaml
    /// </summary>

    public partial class TriggerBar : UserControl, ITranslatable
    {
    	protected TriggerSlot triggerSlot;
    	protected Spine spine;
    	
    	
    	// TODO:
    	// needs to track changes, both when trigger is changed and when the contents of that trigger change
		public TriggerControl TriggerControl {
			get { return triggerSlot.Contents as TriggerControl; }
			set { 
				triggerSlot.Contents = value; 
			}
		}
    	
		public Spine Spine {
			get { return spine; }
		}
    	
    	
		public event EventHandler Changed;
		
		
		protected virtual void OnChanged(EventArgs e)
		{
			EventHandler handler = Changed;
			if (handler != null) {
				handler(this,e);
			}
		}
		
		
		/// <summary>
		/// Check whether this Flip component has all essential fields filled in,
		/// including those belonging to subcomponents, such that it can generate valid code.
		/// </summary>
		/// <returns>True if all essential fields have been given values; false otherwise.</returns>
		/// <remarks>Note that this method makes no attempt to judge whether the values
		/// are valid in their slots, only that those slots have been filled.</remarks>
		public bool IsComplete { 
			get { 
				return triggerSlot.IsComplete && spine.IsComplete;
			}
		}	
		
    	
        public TriggerBar(TriggerControl initialTrigger, Fitter fitter)
        {
        	if (initialTrigger == null) throw new ArgumentNullException("triggerControl");
        	
        	spine = new Spine(fitter,3);
        	Grid.SetRow(spine,0);
        	Grid.SetColumn(spine,0);
        	spine.Margin = new Thickness(14,0,0,0);
        	
        	triggerSlot = new TriggerSlot(new TriggerFitter());
        	
            InitializeComponent();
            
            triggerBarPanel.Children.Add(triggerSlot);
            
            spine.Extends = border.Height + 20;        	
        	Grid.SetZIndex(spine,1);
        	Grid.SetZIndex(border,2);            
            mainGrid.Children.Add(spine);
            
            Effect = new DropShadowEffect();
            
        	spine.Changed += delegate { OnChanged(new EventArgs()); };
        	
            TriggerControl = initialTrigger;
        }
        
        
        public string GetAddress()
        {
        	return triggerSlot.GetAddress();
        }


		public string GetCode()
		{
			System.Text.StringBuilder code = new System.Text.StringBuilder();
			
			code.AppendLine("#include \"ginc_param_const\"");
			code.AppendLine("#include \"ginc_actions\"");
			code.AppendLine("#include \"NW_I0_GENERIC\"");		
			code.AppendLine("#include \"flip_functions\"");	
			code.AppendLine("#include \"ginc_henchman\"");
			code.AppendLine();
			code.AppendLine("void main()");
			code.AppendLine("{");
			code.AppendLine(spine.GetCode());
			code.AppendLine("}");
			
			return code.ToString();
		}
    	
        
		public string GetNaturalLanguage()
		{
			System.Text.StringBuilder code = new System.Text.StringBuilder();
			
			code.AppendLine(triggerSlot.GetNaturalLanguage());
			code.AppendLine(spine.GetNaturalLanguage());
			
			return code.ToString();
		}
		
		
		public ScriptInformation GetScript()
		{
			ScriptInformation script = new ScriptInformation(TriggerControl,spine);
			return script;
		}
    }
}