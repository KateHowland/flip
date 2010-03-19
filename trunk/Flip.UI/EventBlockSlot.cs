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
	/// A slot which can hold an EventBlock.
	/// </summary>
    public partial class EventBlockSlot : MoveableSlot
    {    	
    	#region Fields
    	
    	/// <summary>
    	/// The default border brush.
    	/// </summary>
    	protected static Brush defaultBorderBrush;
    	
    	/// <summary>
    	/// The border brush that indicates a dragged
    	/// object can be dropped into this slot.
    	/// </summary>
    	protected static Brush dropBorderBrush;
    	
    	#endregion
    	
    	#region Constructors
    	
    	/// <summary>
    	/// Initialises brushes.
    	/// </summary>
    	static EventBlockSlot()
    	{
    		defaultBorderBrush = Brushes.Black;
    		dropBorderBrush = Brushes.Blue;
    	}
    	
    	
		/// <summary>
		/// Constructs a new <see cref="EventBlockSlot"/> instance.
		/// </summary>
		/// <param name="fitter">A fitter which decides whether a 
		/// given Moveable can fit into this slot.</param>
		public EventBlockSlot(Fitter fitter) : base(fitter)
		{		
        	InitializeComponent();
        	SetDefaultAppearance();	
		}

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
        
    	#region Methods
        
        /// <summary>
        /// Change the appearance of the control to indicate
        /// that it will accept a drop.
        /// </summary>
		protected override void SetSlottableAppearance()
		{
			border.BorderBrush = dropBorderBrush;
		}
        
                
        /// <summary>
        /// Restore the default appearance of the control.
        /// </summary>
		protected override void SetDefaultAppearance()
		{
			border.BorderBrush = defaultBorderBrush;
		}
        
        
		/// <summary>
		/// Gets a deep copy of this instance.
		/// </summary>
		/// <returns>A deep copy of this instance.</returns>
		public override MoveableSlot DeepCopy()
		{
			EventBlockSlot copy = new EventBlockSlot(moveableFitter);
			copy.Contents = (Moveable)this.Contents.DeepCopy();
			return copy;
		}
		
		#endregion
    }
}