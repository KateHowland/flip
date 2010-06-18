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
    public partial class PegSlot : MoveableSlot
    {
    	#region Fields
    	
    	/// <summary>
    	/// A brush for indicating that no drag operation
    	/// has been detected over this slot.
    	/// </summary>
    	protected new static Brush defaultBrush;
    	
    	/// <summary>
    	/// A brush for indicating that a dragged
    	/// object can be dropped into this slot.
    	/// </summary>
    	protected new static Brush dropBrush;
    	
    	#endregion
    	
    	#region Constructors
    	
    	/// <summary>
    	/// Initialises brushes.
    	/// </summary>
    	static PegSlot()
    	{
    		defaultBrush = Brushes.Transparent;
    		dropBrush = MoveableSlot.dropBrush.Clone();
    		dropBrush.Opacity = 0.3;
    	}
    	
    	
		/// <summary>
		/// Constructs a new <see cref="PegSlot"/> instance.
		/// </summary>
		/// <param name="fitter">A fitter which decides whether a 
		/// given Moveable can fit into this slot.</param>
    	public PegSlot(Fitter fitter) : base(fitter)
        {
            InitializeComponent();
            SetDefaultAppearance();
            MoveableChanged += delegate 
            {  
            	if (Contents != null) Contents.HorizontalAlignment = HorizontalAlignment.Left;
            };
        }
        
    	#endregion
        
    	#region Methods
    	      
        /// <summary>
        /// Checks whether a given Moveable can be placed
        /// into this slot.
        /// </summary>
        /// <param name="moveable">The Moveable to check.</param>
        /// <returns>True if the given Moveable can fit
        /// in this slot; false otherwise.</returns>
        public override bool Fits(Moveable moveable)
        {
        	return Contents == null && fitter.Fits(moveable);
        }
        
        
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
        	border.Background = dropBrush;
        }
        
        
        /// <summary>
        /// Change the appearance of the control to indicate
        /// that it will not accept a drop.
        /// </summary>
        protected override void SetNoDropAppearance()
		{
        	SetDefaultAppearance();
		}
        
        
        /// <summary>
        /// Restore the default appearance of the control.
        /// </summary>
        protected override void SetDefaultAppearance()
        {  	
        	border.Background = defaultBrush;
        }
        
        
		/// <summary>
		/// Gets a deep copy of this instance.
		/// </summary>
		/// <returns>A deep copy of this instance.</returns>
        public override MoveableSlot DeepCopy()
        {
        	return new PegSlot(fitter);
        }
        
        #endregion
    }
}