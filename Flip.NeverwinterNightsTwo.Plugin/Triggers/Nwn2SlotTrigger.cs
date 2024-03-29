﻿using System;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Sussex.Flip.Games.NeverwinterNightsTwo.Behaviours;
using Sussex.Flip.Games.NeverwinterNightsTwo.Utils;
using Sussex.Flip.UI;

namespace Sussex.Flip.Games.NeverwinterNightsTwo
{
    public partial class Nwn2SlotTrigger : TriggerControl
    {
    	protected BlockSlot raiserSlot;
    	protected Nwn2AddressFactory addressFactory;
    	protected static ScaleTransform scaleTransform;
    	
    	
    	static Nwn2SlotTrigger()
    	{
    		scaleTransform = new ScaleTransform(0.8,0.8);
    	}
		
		
		/// <summary>
		/// Check whether this Flip component has all essential fields filled in,
		/// including those belonging to subcomponents, such that it can generate valid code.
		/// </summary>
		/// <returns>True if all essential fields have been given values; false otherwise.</returns>
		/// <remarks>Note that this method makes no attempt to judge whether the values
		/// are valid in their slots, only that those slots have been filled.</remarks>
		public override bool IsComplete { 
			get { 
				return raiserSlot.IsComplete;
			}
		}		
    	
    	    	
		public ObjectBlock RaiserBlock {
			get { return raiserSlot.Contents as ObjectBlock; }
			set { raiserSlot.Contents = value; }
		}
    	
    	
		/// <summary>
		/// Constructs a new <see cref="Nwn2TriggerControl"/> instance.
		/// </summary>
        public Nwn2SlotTrigger(Fitter raiserFitter, string text1, string text2)
        {
        	if (raiserFitter == null) throw new ArgumentNullException("raiserFitter");
        	
        	this.addressFactory = new Nwn2AddressFactory();
        	
        	raiserSlot = new BlockSlot("raiser",raiserFitter);
        	raiserSlot.BorderThickness = new Thickness(1);
        	raiserSlot.Padding = new Thickness(4);
        	raiserSlot.LayoutTransform = scaleTransform;
            
        	InitializeComponent();
            
            this.text1.Text = text1;
            this.text2.Text = text2;
            
            raiserSlot.MoveableChanged += delegate 
            {
            	OnChanged(new EventArgs()); 
            };
                        
            mainPanel.Children.Insert(1,raiserSlot);
        }
        
        
		public override string GetCode()
		{			
			return String.Empty;
		}
		
		
		public override string ToString()
		{
			return GetNaturalLanguage();
		}
		
		
		public override void AssignImage(ImageProvider imageProvider)
		{
			if (RaiserBlock != null) RaiserBlock.AssignImage(imageProvider);
		}
		
		
		public override XmlSchema GetSchema()
		{
			return null;
		}
		
		
		public override void Clear()
		{
			RaiserBlock = null;
		}
		
		
		public override string GetAddress()
		{
			return String.Empty;
		}
		
		
		public override void WriteXml(XmlWriter writer)
		{			
		}
		
		
		public override void ReadXml(XmlReader reader)
		{
		}
		
		
		public override string GetNaturalLanguage()
		{
			return String.Empty;
		}
		
		
		public override Moveable DeepCopy()
		{
			Nwn2SlotTrigger copy = new Nwn2SlotTrigger(raiserSlot.Fitter,text1.Text,text2.Text);
			copy.RaiserBlock = RaiserBlock;
			return copy;
		}
				
		
		public override string GetLogText()
		{
			return "Event (" + GetNaturalLanguage() + ")";
		}
        
        
		public override ScriptStats GetStatistics()
		{
			ScriptStats s = new ScriptStats();
			s.Add(this);
			s.Add(raiserSlot.GetStatistics());
			return s;
		}
    }
}