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
    /// Interaction logic for PegSlot.xaml
    /// </summary>

    public partial class PegSlot : MoveableSlot
    {
    	#region Fields
    	
    	protected static RadialGradientBrush acceptingBrush;
    	protected static Brush defaultBrush;
    	
    	#endregion
    	
		#region Properties
        
    	/// <summary>
    	/// The Moveable held by this slot.
    	/// </summary>
		public override Moveable Contents {
			get {
				return border.Child as Moveable;
			}
			set {
				border.Child = value;
			}
		}
    	
    	#endregion
        
    	static PegSlot()
    	{
        	acceptingBrush = new RadialGradientBrush(Colors.Blue,Colors.LightBlue);
        	defaultBrush = Brushes.Transparent;
    	}
    	
    	
    	public PegSlot(Fitter fitter) : base(fitter)
        {
            InitializeComponent();            
            
            this.moveableFitter = fitter;
            
            SetDefaultAppearance();
        }
        
        
        /// <summary>
        /// Change the appearance of the control to indicate
        /// that it will accept a drop.
        /// </summary>
        protected override void SetSlottableAppearance()
        {    	
        	border.Background = acceptingBrush;
        }
        
        
        /// <summary>
        /// Restore the default appearance of the control.
        /// </summary>
        protected override void SetDefaultAppearance()
        {  	
        	border.Background = defaultBrush;
        }
        
        
        public override MoveableSlot DeepCopy()
        {
        	return new PegSlot(moveableFitter);
        }
    }
}