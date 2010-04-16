using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Sussex.Flip.UI
{
    /// <summary>
    /// Interaction logic for TriggerBar.xaml
    /// </summary>

    public partial class TriggerBar : UserControl, ITranslatable
    {
    	protected TriggerControl triggerControl;
    	protected Spine spine;
    	
    	
		public event EventHandler Changed;
		
		
		protected virtual void OnChanged(EventArgs e)
		{
			EventHandler handler = Changed;
			if (handler != null) {
				handler(this,e);
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
    }
}