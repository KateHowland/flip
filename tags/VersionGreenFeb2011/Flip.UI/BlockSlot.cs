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
using Sussex.Flip.Utils;

namespace Sussex.Flip.UI
{
    /// <summary>
    /// A slot which TODO
    /// </summary>
    public partial class BlockSlot : MoveableSlot
    {
    	#region Fields
    	
    	/// <summary>
    	/// Used to set a thin border.
    	/// </summary>
    	protected static Thickness thin;
    	
    	/// <summary>
    	/// Used to set a thick border.
    	/// </summary>
    	protected static Thickness thick;
    	
    	/// <summary>
    	/// Used to pad the explanation text.
    	/// </summary>
    	protected static Thickness textThickness;
    	    	
    	#endregion
    	            
    	#region Constructors
    	
    	/// <summary>
    	/// Initialises thicknesses.
    	/// </summary>
    	static BlockSlot()
    	{
    		thin = new Thickness(0.5);
    		thick = new Thickness(1.0);
    		textThickness = new Thickness(3.0);
    	}
    	

		/// <summary>
		/// Constructs a new <see cref="BlockSlot"/> instance.
		/// </summary>
		/// <param name="slotName">The name of this slot.</param>
		/// <param name="fitter">A fitter which decides whether a 
		/// given Moveable can fit into this slot.</param>
    	public BlockSlot(string slotName, Fitter fitter) : this(fitter)
    	{
    		// Not sure if I'm going to use slot names or not, so here's
    		// a dummy constructor for all the instances I'm already
    		// creating using them.
    	}
    	
    			
    	/// <summary>
		/// Constructs a new <see cref="BlockSlot"/> instance.
		/// </summary>
		/// <param name="fitter">A fitter which decides whether a 
		/// given Moveable can fit into this slot.</param>
    	public BlockSlot(Fitter fitter) : base(fitter)
        {
            InitializeComponent();            
            
            this.fitter = fitter;            
            
            border.MinWidth = ObjectBlock.DefaultSize.Width + border.BorderThickness.Left + border.BorderThickness.Right;
            border.MinHeight = ObjectBlock.DefaultSize.Height + border.BorderThickness.Top + border.BorderThickness.Bottom;
            
            string text = fitter.GetMoveableDescription();
            if (text == "something") text = "some thing";
            
            fitterText.Text = text;
            fitterText.Foreground = Brushes.DarkSlateBlue;
            fitterText.MaxWidth = border.MinWidth;
            fitterText.MaxHeight = border.MinHeight;
            fitterText.Padding = textThickness;
            
            SetDefaultAppearance();
        }
    	
    	#endregion
    	    	    	
        #region Methods
        		
        /// <summary>
        /// Gets the Moveable held by this slot.
        /// </summary>
        /// <returns>A Moveable, or null if the slot is empty.</returns>
        protected override Moveable GetMoveable()
        {
        	return border.Child as Moveable;
        }
                
        
        /// <summary>
        /// Sets the Moveable held by this slot.
        /// </summary>
        protected override void SetMoveable(Moveable moveable)
        {
        	border.Child = moveable;
        }
        
        
        /// <summary>
        /// Change the appearance of the control to indicate
        /// that it will accept a drop.
        /// </summary>
        protected override void SetDropAppearance()
        {
        	border.Background = Brushes.LightGray;
        	border.BorderBrush = dropBrush;
        	border.BorderThickness = thick;        	
        	fitterText.FontWeight = FontWeights.Bold;
        }
        
        
        /// <summary>
        /// Change the appearance of the control to indicate
        /// that it will not accept a drop.
        /// </summary>
        protected override void SetNoDropAppearance()
		{
        	border.Background = Brushes.LightGray;
        	border.BorderBrush = noDropBrush;
        	border.BorderThickness = thin;   
        	fitterText.FontWeight = FontWeights.Normal;
		}
        
        
        /// <summary>
        /// Restore the default appearance of the control.
        /// </summary>
        protected override void SetDefaultAppearance()
        {
        	if (Contents == null) {
	        	border.Background = Brushes.LightGray;
	        	border.BorderBrush = defaultBrush;
	        	fitterText.Visibility = Visibility.Visible;
        	}
        	else {
        		border.Background = Brushes.Transparent;
        		border.BorderBrush = Brushes.Transparent;
	        	fitterText.Visibility = Visibility.Hidden;
        	}
        	
        	fitterText.FontWeight = FontWeights.Normal;
	        border.BorderThickness = thin;
        }
        
        
		/// <summary>
		/// Gets a deep copy of this instance.
		/// </summary>
		/// <returns>A deep copy of this instance.</returns>        
        public override MoveableSlot DeepCopy()
        {
        	return new BlockSlot(fitter);
        }
        
        #endregion
    }
}