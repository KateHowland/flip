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
    /// Interaction logic for TriggerSlot.xaml
    /// </summary>

    public partial class TriggerSlot : MoveableSlot
    {    	
    	protected Brush defaultBorderBrush = Brushes.Black;
    	protected Brush dropBorderBrush = Brushes.Blue;
    	
    	
        public TriggerSlot()
        {
            InitializeComponent();
            SetDefaultAppearance();
        }

        
		public override ObjectBlock Contents {
			get {
				return border.Child as ObjectBlock;
			}
			set {
				border.Child = value;
			}
		}
        
        
		protected override void SetSlottableAppearance()
		{
			border.BorderBrush = dropBorderBrush;
		}
        
        
		protected override void SetDefaultAppearance()
		{
			border.BorderBrush = defaultBorderBrush;
		}
        
        
		public override MoveableSlot DeepCopy()
		{
			TriggerSlot copy = new TriggerSlot();
			copy.Contents = (ObjectBlock)this.Contents.DeepCopy();
			return copy;
		}
    }
}