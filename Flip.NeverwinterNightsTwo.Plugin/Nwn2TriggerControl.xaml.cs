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
using Sussex.Flip.UI;

namespace Sussex.Flip.Games.NeverwinterNightsTwo
{
    /// <summary>
    /// Interaction logic for Nwn2TriggerControl.xaml
    /// </summary>

    public partial class Nwn2TriggerControl : TriggerControl
    {
    	protected ObjectBlockSlot raiserSlot;
    	protected EventBlockSlot eventSlot;
    	
    	    	
		public override ObjectBlock RaiserBlock {
			get {
				return raiserSlot.Contents as ObjectBlock;
			}
			set {
				raiserSlot.Contents = value;
			}
		}
    	
    	
		public override EventBlock EventBlock {
			get {
				return eventSlot.Contents as EventBlock;
			}
			set {
				eventSlot.Contents = value;
			}
		}
    	
    	
        public Nwn2TriggerControl()
        {
        	raiserSlot = new ObjectBlockSlot("raiser",new Nwn2RaiserBlockFitter());
            raiserSlot.Padding = new Thickness(10);
            eventSlot = new EventBlockSlot(new Nwn2EventBlockFitter(raiserSlot));
            
            raiserSlot.MoveableChanged += delegate(object sender, MoveableEventArgs e) 
            {  
            	if (e.Moveable == null) {
            	}
            	else {
            	}
            };
            
            eventSlot.MoveableChanged += delegate(object sender, MoveableEventArgs e) 
            {  
            	if (e.Moveable == null) {
            	}
            	else {
            	}
            };
            
            InitializeComponent();
            
            mainPanel.Children.Add(raiserSlot);
            mainPanel.Children.Add(eventSlot);
        }
    }
}