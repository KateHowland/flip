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
    /// Interaction logic for TriggerBar.xaml
    /// </summary>

    public partial class TriggerBar : UserControl
    {
    	protected ObjectBlockSlot eventRaiserSlot;
    	protected EventBlockSlot eventSlot;
    	
    	
        public TriggerBar(TriggerBarFitter triggerBarFitter)
        {
        	if (triggerBarFitter == null) throw new ArgumentNullException("triggerBarFitter"); // HACK
        	if (triggerBarFitter.EventFitter == null) throw new ArgumentException("TriggerBarFitter has a null EventFitter.","triggerBarFitter"); // HACK
        	if (triggerBarFitter.EventRaiserFitter == null) throw new ArgumentException("TriggerBarFitter has a null EventRaiserFitter.","triggerBarFitter"); // HACK
        	
        	Spine spine = new Spine(3);
        	Grid.SetRow(spine,0);
        	Grid.SetColumn(spine,0);
        	spine.Margin = new Thickness(14,0,0,0);
        	
            InitializeComponent();
            
            spine.Extends = border.Height + 20;        	
        	Grid.SetZIndex(spine,1);
        	Grid.SetZIndex(border,2);            
            mainGrid.Children.Add(spine);
            
            Effect = new DropShadowEffect();
            
            eventRaiserSlot = new ObjectBlockSlot("eventraiser",triggerBarFitter.EventRaiserFitter);
            eventRaiserSlot.Padding = new Thickness(10);
            triggerBarPanel.Children.Add(eventRaiserSlot);
            
            eventSlot = new EventBlockSlot(triggerBarFitter.EventFitter);
            triggerBarPanel.Children.Add(eventSlot);
        }
        
        
//        public ObjectBlock GetEventRaiser()
//        {
//        	
//        }
//        
//        
//        public EventBlock GetEvent()
//        {
//        	try {
//        		return (EventBlock)eventSlot.Contents;
//        	}
//        	catch (
//        }
    }
}