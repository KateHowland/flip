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
				return raiserSlot.Contents;
			}
			set {
				raiserSlot.Contents = value;
			}
		}
    	
    	
		public override EventBlock EventBlock {
			get {
				return eventSlot.Contents;
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
//            raiserSlot.MoveableChanging += delegate(object sender, MoveableEventArgs e) { 
//            	MessageBox.Show("MoveableChanging"); 
//            	EventBlock eb = e.Moveable as EventBlock;
//            	if (eb != null && eb.EventName == "OnInventoryDisturbed") e.Cancel = true;
//            };
            
//            raiserSlot.MoveableChanged += delegate { MessageBox.Show("MoveableChanged"); };
            
            
            InitializeComponent();
            
            mainPanel.Children.Add(raiserSlot);
            mainPanel.Children.Add(eventSlot);
        }
    }
}