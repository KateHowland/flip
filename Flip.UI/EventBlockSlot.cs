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
    public partial class EventBlockSlot : MoveableSlot<EventBlock>
    {    	
    	#region Constructors
    	
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
    	/// The EventBlock held by this slot.
    	/// </summary>
		public override EventBlock Contents {
			get {
				return border.Child as EventBlock;
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
		public override MoveableSlot<EventBlock> DeepCopy()
		{
			EventBlockSlot copy = new EventBlockSlot(moveableFitter);
			copy.Contents = (EventBlock)this.Contents.DeepCopy();
			return copy;
		}
		
		#endregion
    }
}