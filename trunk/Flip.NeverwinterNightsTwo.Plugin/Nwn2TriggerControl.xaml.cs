﻿using System;
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
    public partial class Nwn2TriggerControl : TriggerControl
    {
    	protected BlockSlot raiserSlot;
    	protected EventBlockSlot eventSlot;
    	protected EventDescriber describer;
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
				return raiserSlot.IsComplete && eventSlot.IsComplete;
			}
		}		
    	
    	    	
		public override ObjectBlock RaiserBlock {
			get { return raiserSlot.Contents as ObjectBlock; }
			set { raiserSlot.Contents = value; }
		}
    	
    	
		public override EventBlock EventBlock {
			get { return eventSlot.Contents as EventBlock; }
			set { eventSlot.Contents = value; }
		}
    	
    	
		public EventDescriber Describer {
			get { return describer; }
			set { 
				if (describer != value) {
					describer = value;
					OnChanged(new EventArgs());
				}
			}
		}
    	
    	
		/// <summary>
		/// Constructs a new <see cref="Nwn2TriggerControl"/> instance.
		/// </summary>
		/// <param name="describer">Used to describe the event specified on this
		/// trigger control in natural language.</param>
        public Nwn2TriggerControl(EventDescriber describer)
        {
        	if (describer == null) throw new ArgumentNullException("describer");
        	
        	this.describer = describer;
        	
        	this.addressFactory = new Nwn2AddressFactory();
        	
        	raiserSlot = new BlockSlot("raiser",new Nwn2RaiserBlockFitter());
            raiserSlot.Padding = new Thickness(10);
            eventSlot = new EventBlockSlot(new Nwn2EventBlockFitter(raiserSlot));
            
            InitializeComponent();
            
            raiserSlot.MoveableChanged += delegate 
            {
            	OnChanged(new EventArgs()); 
            	CheckEventFits();
            };
            
            eventSlot.MoveableChanged += delegate 
            { 
            	OnChanged(new EventArgs()); 
            };
                        
            mainPanel.Children.Add(raiserSlot);
            mainPanel.Children.Add(eventSlot);
        }
        
        
        public Nwn2TriggerControl() : this(new EventDescriber())
        {        	
        }
        

        /// <summary>
        /// Check that the new event raiser is capable of raising the
        /// selected event, and if not, remove that event.
        /// </summary>
        protected void CheckEventFits()
        {
           	if (EventBlock != null && !eventSlot.Fits(EventBlock)) {
           		EventBlock = null;
           	}
        }
        
        
		public override string GetCode()
		{			
			return String.Empty;
		}
					
		
		public override string GetNaturalLanguage()
		{			
			string natural = null;
			
			string raiserName = raiserSlot.GetNaturalLanguage();
			string eventName = eventSlot.GetNaturalLanguage();
			
			ObjectBlock block = raiserSlot.Contents as ObjectBlock;
			if (block == null || !(block.Behaviour is Nwn2ObjectBehaviour) || eventSlot.Contents == null) {
				natural = "When some event is raised";
			}
			
			else {
				Nwn2ObjectBehaviour behaviour = (Nwn2ObjectBehaviour)block.Behaviour;
				
				switch (behaviour.Nwn2Type) {
						
					case Nwn2Type.Area:
						if (String.IsNullOrEmpty(raiserName)) raiserName = "a particular area";
						natural = describer.GetAreaEventDescription(raiserName,eventName);
						break;
						
					case Nwn2Type.Creature:
						if (String.IsNullOrEmpty(raiserName)) raiserName = "a particular creature";
						natural = describer.GetCreatureEventDescription(raiserName,eventName);
						break;
						
					case Nwn2Type.Door:
						if (String.IsNullOrEmpty(raiserName)) raiserName = "a particular door";
						natural = describer.GetOpenableEventDescription(raiserName,eventName);
						break;
						
					case Nwn2Type.Encounter:
						if (String.IsNullOrEmpty(raiserName)) raiserName = "a particular encounter";
						natural = describer.GetEncounterEventDescription(raiserName,eventName);
						break;
						
					case Nwn2Type.Module:		
						natural = describer.GetModuleEventDescription(eventName);
						break;
						
					case Nwn2Type.Placeable:
						if (String.IsNullOrEmpty(raiserName)) raiserName = "a particular placeable";
						natural = describer.GetOpenableEventDescription(raiserName,eventName);
						break;
						
					case Nwn2Type.Store:
						if (String.IsNullOrEmpty(raiserName)) raiserName = "a particular store";
						natural = describer.GetStoreEventDescription(raiserName,eventName);
						break;
						
					case Nwn2Type.Trigger:
						if (String.IsNullOrEmpty(raiserName)) raiserName = "a particular trigger";
						natural = describer.GetTriggerEventDescription(raiserName,eventName);
						break;					
				}
			}
			
			if (natural == null) {
				natural = String.Format("When {0} is raised by {1}, this happens: ",eventName,raiserName);
			}
			
			return natural;
		}
		
		
		public override string ToString()
		{
			return GetNaturalLanguage();
		}
		
		
		public override string GetAddress()
		{
			if (EventBlock == null || RaiserBlock == null) return null;
			
			Nwn2Address address;
			
			if (Nwn2Fitter.IsModule(RaiserBlock)) {			
				address = addressFactory.GetModuleAddress(EventBlock.EventName);
			}
			
			else if (Nwn2Fitter.IsArea(RaiserBlock)) {			
				string areaTag = RaiserBlock.Identifier;				
				address = addressFactory.GetAreaAddress(EventBlock.EventName,areaTag);
			}
			
			else if (Nwn2Fitter.IsInstance(RaiserBlock)) {
				InstanceBehaviour instanceBehaviour = (InstanceBehaviour)RaiserBlock.Behaviour;
				
				string areaTag = instanceBehaviour.AreaTag;
				Nwn2Type targetType = instanceBehaviour.Nwn2Type;
				string instanceTag = instanceBehaviour.Tag;
				
				address = addressFactory.GetInstanceAddress(EventBlock.EventName,areaTag,targetType,instanceTag);
			}
			
			else {
				address = null;
			}
			
			return address.Value;
		}
		
		
		public override XmlSchema GetSchema()
		{
			return null;
		}
		
		
		public override void ReadXml(XmlReader reader)
		{		
			reader.MoveToContent();
			
			if (reader.IsEmptyElement) {
				reader.ReadStartElement();
				RaiserBlock = null;
				EventBlock = null;
			}
			
			else {
				
				if (!reader.ReadToDescendant("EventRaiser")) {
					throw new FormatException("EventRaiser information was missing.");
				}
				
				if (reader.IsEmptyElement) {
					reader.ReadStartElement();
					RaiserBlock = null;
				}
				
				else {
					if (!reader.ReadToDescendant("ObjectBlock")) {
						throw new FormatException("EventRaiser is non-empty, but no ObjectBlock is defined.");
					}
					ObjectBlock block = new ObjectBlock();
					block.ReadXml(reader);
					RaiserBlock = block;
					reader.ReadEndElement(); // should be reading end element of EventRaiser
				}
				
				reader.MoveToContent();
				
				if (reader.LocalName != "EventName") {
					throw new FormatException("EventName information was missing.");
				}
				
				if (reader.IsEmptyElement) {
					reader.ReadStartElement();
					EventBlock = null;
				}
				
				else {
					if (!reader.ReadToDescendant("EventBlock")) {
						throw new FormatException("EventRaiser is non-empty, but no EventBlock is defined.");
					}
					EventBlock block = new EventBlock();
					block.ReadXml(reader);
					EventBlock = block;
					reader.ReadEndElement(); // should be reading end element of EventName
				}
				
				reader.MoveToContent();
				reader.ReadEndElement();
			}
		}
				
		
		public override void WriteXml(XmlWriter writer)
		{
			writer.WriteStartElement("EventRaiser");
			if (RaiserBlock != null) {
				writer.WriteStartElement("ObjectBlock");
				RaiserBlock.WriteXml(writer);
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
			
			writer.WriteStartElement("EventName");
			if (EventBlock != null) {
				writer.WriteStartElement("EventBlock");
				EventBlock.WriteXml(writer);
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
		}
    }
}