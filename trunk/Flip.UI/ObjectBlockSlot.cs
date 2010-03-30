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
    public partial class ObjectBlockSlot : MoveableSlot
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
    	    	
    	#endregion
    	            
    	#region Constructors
    	
    	/// <summary>
    	/// Initialises thicknesses.
    	/// </summary>
    	static ObjectBlockSlot()
    	{
    		thin = new Thickness(0.5);
    		thick = new Thickness(1.0);
    	}
    	

		/// <summary>
		/// Constructs a new <see cref="ObjectBlockSlot"/> instance.
		/// </summary>
		/// <param name="slotName">The name of this slot.</param>
		/// <param name="fitter">A fitter which decides whether a 
		/// given Moveable can fit into this slot.</param>
    	public ObjectBlockSlot(string slotName, Fitter fitter) : this(fitter)
    	{
    		// Not sure if I'm going to use slot names or not, so here's
    		// a dummy constructor for all the instances I'm already
    		// creating using them.
    	}
    	
    			
    	/// <summary>
		/// Constructs a new <see cref="ObjectBlockSlot"/> instance.
		/// </summary>
		/// <param name="fitter">A fitter which decides whether a 
		/// given Moveable can fit into this slot.</param>
    	public ObjectBlockSlot(Fitter fitter) : base(fitter)
        {
            InitializeComponent();            
            
            this.fitter = fitter;
            
            border.Width = ObjectBlock.DefaultSize.Width + border.BorderThickness.Left + border.BorderThickness.Right;
            border.Height = ObjectBlock.DefaultSize.Height + border.BorderThickness.Top + border.BorderThickness.Bottom;
            
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
        	border.BorderBrush = dropBrush;
        	border.BorderThickness = thick;        	
        }
        
        
        /// <summary>
        /// Change the appearance of the control to indicate
        /// that it will not accept a drop.
        /// </summary>
        protected override void SetNoDropAppearance()
		{
        	border.BorderBrush = noDropBrush;
        	border.BorderThickness = thin;   
		}
        
        
        /// <summary>
        /// Restore the default appearance of the control.
        /// </summary>
        protected override void SetDefaultAppearance()
        {
        	border.BorderBrush = defaultBrush;
        	border.BorderThickness = thin;        	
        }
        
        
		/// <summary>
		/// Gets a deep copy of this instance.
		/// </summary>
		/// <returns>A deep copy of this instance.</returns>        
        public override MoveableSlot DeepCopy()
        {
        	return new ObjectBlockSlot(fitter);
        }
        
        #endregion
    }
}