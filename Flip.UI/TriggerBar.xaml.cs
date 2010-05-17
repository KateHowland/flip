using System;
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
    	protected TriggerControl triggerControl;
    	protected Spine spine;
    	
    	
		public TriggerControl TriggerControl {
			get { return triggerControl; }
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
				return triggerControl.IsComplete && spine.IsComplete;
			}
		}	
		
    	
        public TriggerBar(TriggerControl triggerControl, Fitter fitter)
        {
        	if (triggerControl == null) throw new ArgumentNullException("triggerControl");
        	
        	spine = new Spine(fitter,3);
        	Grid.SetRow(spine,0);
        	Grid.SetColumn(spine,0);
        	spine.Margin = new Thickness(14,0,0,0);
        	
            InitializeComponent();
            
            spine.Extends = border.Height + 20;        	
        	Grid.SetZIndex(spine,1);
        	Grid.SetZIndex(border,2);            
            mainGrid.Children.Add(spine);
            
            Effect = new DropShadowEffect();
            
            this.triggerControl = triggerControl;
            triggerBarPanel.Children.Add(triggerControl);
        	
        	spine.Changed += delegate { OnChanged(new EventArgs()); };
        	triggerControl.Changed += delegate { OnChanged(new EventArgs()); };
        }
        
        
        public string GetAddress()
        {
        	return triggerControl.GetAddress();
        }


		public string GetCode()
		{
			System.Text.StringBuilder code = new System.Text.StringBuilder();
			
			code.AppendLine("#include \"ginc_param_const\"");
			code.AppendLine("#include \"ginc_actions\"");
			code.AppendLine("#include \"flip_functions\"");
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
			
			code.AppendLine(triggerControl.GetNaturalLanguage());
			code.AppendLine(spine.GetNaturalLanguage());
			
			return code.ToString();
		}
		
		
		public ScriptInformation GetScript()
		{
			ScriptInformation script = new ScriptInformation(triggerControl,spine);
			return script;
		}
    }
}