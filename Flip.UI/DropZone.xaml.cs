using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Sussex.Flip.UI
{
    /// <summary>
    /// Interaction logic for DropZone.xaml
    /// </summary>

    public partial class DropZone : MoveableSlot
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
    	static DropZone()
    	{
    		defaultBrush = Brushes.Transparent;
    		dropBrush = MoveableSlot.dropBrush.Clone();
    		dropBrush.Opacity = 0.3;
    	}
    	
    	
    	public DropZone(Fitter fitter) : base(fitter)
        {
            InitializeComponent();            
            SetDefaultAppearance();
        }
    	
    	#endregion
                
    	#region Methods
    	
        /// <summary>
        /// Gets the Moveable held by this slot.
        /// </summary>
        /// <returns>A Moveable, or null if the slot is empty.</returns>
        /// <remarks>A DropZone is always empty - the responsibility
        /// for holding a dropped Moveable is passed to a new peg.</remarks>
        protected override Moveable GetMoveable()
        {
        	return null;
        }
                
        
        /// <summary>
        /// Sets the Moveable held by this slot.
        /// </summary>
        /// <remarks>A DropZone is always empty - the responsibility
        /// for holding a dropped Moveable is passed to a new peg.</remarks>
        protected override void SetMoveable(Moveable moveable)
        {
        }
        
        
        /// <summary>
        /// Change the appearance of the control to indicate
        /// that it will accept a drop.
        /// </summary>
        protected override void SetDropAppearance()
        {    	
        	Background = dropBrush;
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
        	Background = defaultBrush;
        }
        
        
        /// <summary>
        /// Accepts dropped Moveable objects, if they fit this slot.
        /// </summary>
		protected override void AcceptDrop(object sender, DragEventArgs e)
		{
			SetDefaultAppearance();
			e.Handled = false;
		}
        
        
		/// <summary>
		/// Gets a deep copy of this instance.
		/// </summary>
		/// <returns>A deep copy of this instance.</returns>
        public override MoveableSlot DeepCopy()
        {
        	return new DropZone(fitter);
        }
        
        #endregion
    }
}