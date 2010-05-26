using System;
using System.Reflection;
using System.Windows;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Sussex.Flip.Games.NeverwinterNightsTwo.Behaviours;
using Sussex.Flip.Games.NeverwinterNightsTwo.Utils;
using Sussex.Flip.UI;

namespace Sussex.Flip.Games.NeverwinterNightsTwo
{
    public abstract partial class Nwn2ParameterlessTrigger : TriggerControl
    {
    	protected Nwn2AddressFactory addressFactory;
		
		
		/// <summary>
		/// Check whether this Flip component has all essential fields filled in,
		/// including those belonging to subcomponents, such that it can generate valid code.
		/// </summary>
		/// <returns>True if all essential fields have been given values; false otherwise.</returns>
		/// <remarks>Note that this method makes no attempt to judge whether the values
		/// are valid in their slots, only that those slots have been filled.</remarks>
		public override bool IsComplete { 
			get { 
				return true;
			}
		}	
    	
    	
		/// <summary>
		/// Constructs a new <see cref="Nwn2ParameterlessTrigger"/> instance.
		/// </summary>
        public Nwn2ParameterlessTrigger(string text)
        {
        	this.addressFactory = new Nwn2AddressFactory();
            
            InitializeComponent();
            
            this.textBlock.Text = text;
        }
        
        
		public override string GetCode()
		{			
			return String.Empty;
		}
		
		
		public override string ToString()
		{
			return GetNaturalLanguage();
		}
		
		
		public override XmlSchema GetSchema()
		{
			return null;
		}
		
		
		public override void Clear()
		{
		}
    }
}