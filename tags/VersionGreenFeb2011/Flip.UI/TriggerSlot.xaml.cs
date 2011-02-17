using System;
using System.Windows;
using System.Windows.Controls;

namespace Sussex.Flip.UI
{
	/// <summary>
	/// A slot which can hold an TriggerSlot.
	/// </summary>
    public partial class TriggerSlot : MoveableSlot
    {    	
    	#region Constructors
    	
		/// <summary>
		/// Constructs a new <see cref="TriggerSlot"/> instance.
		/// </summary>
		/// <param name="fitter">A fitter which decides whether a 
		/// given Moveable can fit into this slot.</param>
		public TriggerSlot(Fitter fitter) : base(fitter)
		{		
        	InitializeComponent();
        	SetDefaultAppearance();
        	MoveableChanged += delegate 
        	{  
        		if (Contents == null) {
        			dragMessageTextBlock.Visibility = Visibility.Visible;
        		}
        		else {
        			dragMessageTextBlock.Visibility = Visibility.Hidden;
        		}
        	};
        	Contents = null;
		}

		#endregion
        
    	#region Methods
        
    	public string GetAddress()
    	{
    		TriggerControl trigger = Contents as TriggerControl;
    		if (trigger != null) return trigger.GetAddress();
    		else return String.Empty;
    	}
    	
    	
        /// <summary>
        /// Gets the Moveable held by this slot.
        /// </summary>
        /// <returns>A Moveable, or null if the slot is empty.</returns>
        protected override Moveable GetMoveable()
        {
        	return border.Child as TriggerControl;
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
			border.BorderBrush = dropBrush;
		}
        
        
        /// <summary>
        /// Change the appearance of the control to indicate
        /// that it will not accept a drop.
        /// </summary>
        protected override void SetNoDropAppearance()
		{
        	border.BorderBrush = noDropBrush;
		}
        
                
        /// <summary>
        /// Restore the default appearance of the control.
        /// </summary>
		protected override void SetDefaultAppearance()
		{
			border.BorderBrush = defaultBrush;
		}
        
        
		/// <summary>
		/// Gets a deep copy of this instance.
		/// </summary>
		/// <returns>A deep copy of this instance.</returns>
		public override MoveableSlot DeepCopy()
		{
			TriggerSlot copy = new TriggerSlot(fitter);
			copy.Contents = this.Contents.DeepCopy();
			return copy;
		}
		
		#endregion
    }
}