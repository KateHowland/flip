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
    /// TODO
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
    	
    	
		/// <summary>
		/// Constructs a new <see cref="Nwn2TriggerControl"/> instance.
		/// </summary>
        public Nwn2TriggerControl()
        {
        	raiserSlot = new ObjectBlockSlot("raiser",new Nwn2RaiserBlockFitter());
            raiserSlot.Padding = new Thickness(10);
            eventSlot = new EventBlockSlot(new Nwn2EventBlockFitter(raiserSlot));
            
            InitializeComponent();
            
            raiserSlot.MoveableChanged += CheckEventFits;
            
            mainPanel.Children.Add(raiserSlot);
            mainPanel.Children.Add(eventSlot);
        }
        

        /// <summary>
        /// Check that the new event raiser is capable of raising the
        /// selected event, and if not, remove that event.
        /// </summary>
        protected void CheckEventFits(object sender, MoveableEventArgs e)
        {
           	if (eventSlot.Contents != null && !eventSlot.Fits(eventSlot.Contents)) {
           		eventSlot.Contents = null;
           	}
        }
        
        
		public override string GetCode()
		{			
			return String.Empty;
		}
		
		
		protected static Dictionary<string,string> naturalLanguageEventDescriptions;
		protected static string keyFormat = "{0}.{1}";
		
		
		static Nwn2TriggerControl()
		{
			 naturalLanguageEventDescriptions = new Dictionary<string,string>(81)
			 {
			 	{"Area.OnClientEnterScript","When the player (client) enters the area"},
			 	{"Area.OnEnterScript","When the player enters the area"},
			 	{"Area.OnExitScript","When the player exits the area"},
			 	{"Area.OnHeartbeat","Every few seconds"},
			 	{"Area.OnUserDefined","NATURAL LANGUAGE MISSING"}
			 };
		}
		
		
		public override string GetNaturalLanguage()
		{			
			string natural = null;
			
			string raiserName = raiserSlot.GetNaturalLanguage();
			string eventName = eventSlot.GetNaturalLanguage();
			
			ObjectBlock block = raiserSlot.Contents as ObjectBlock;
			
			if (block != null && Nwn2Fitter.IsArea(block)) {
				string key = String.Format(keyFormat,"Area",eventName);
				if (naturalLanguageEventDescriptions.ContainsKey(key)) {
					natural = naturalLanguageEventDescriptions[key];
				}
			}
			
			if (natural == null) {
				natural = String.Format("When {0} is raised by {1}, this happens: ",eventName,raiserName);
			}
			
			return natural;
		}
    }
}