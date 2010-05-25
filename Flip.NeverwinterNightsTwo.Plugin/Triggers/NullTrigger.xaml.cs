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
    /// <summary>
    /// TODO
    /// </summary>
    public partial class NullTrigger : TriggerControl
    {
		/// <summary>
		/// Check whether this Flip component has all essential fields filled in,
		/// including those belonging to subcomponents, such that it can generate valid code.
		/// </summary>
		public override bool IsComplete { 
			get { 
				return false;
			}
		}	
    	
    	
		/// <summary>
		/// Constructs a new <see cref="NullTrigger"/> instance.
		/// </summary>
		/// <param name="describer">Used to describe the event specified on this
		/// trigger control in natural language.</param>
        public NullTrigger()
        {
            InitializeComponent();
        }
        
        
		public override string GetCode()
		{			
			return String.Empty;
		}
					
		
		public override string GetNaturalLanguage()
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
		
		
		public override void ReadXml(XmlReader reader)
		{		
			reader.MoveToContent();
			
			if (!reader.IsEmptyElement) throw new FormatException("NullTrigger should be empty.");
			
			reader.ReadStartElement();
		}
				
		
		public override void WriteXml(XmlWriter writer)
		{
		}
		
		
		public override Moveable DeepCopy()
		{
			return new NullTrigger();
		}
		
		
		public override void Clear()
		{
		}
		
		
		public override string GetAddress()
		{
			return String.Empty;
		}
    }
}