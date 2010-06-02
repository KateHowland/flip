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
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Sussex.Flip.UI
{
    /// <summary>
    /// Interaction logic for Statement.xaml
    /// </summary>

    public partial class Statement : Moveable
    {
    	protected static Brush defaultBrush;
    	protected static Brush actionBrush;
    	protected static Brush conditionBrush;    	
    	
    	
    	protected StatementBehaviour behaviour;
    	
    	
    	public StatementBehaviour Behaviour {
    		get { return behaviour; }
    		set { 
    			if (behaviour != value) {
    				behaviour = value;
    				Initialise();
    				OnChanged(new EventArgs()); //? TODO not sure if it's appropriate to call this here since it should never happen, but I do in ObjectBlock    				
    			}
    		}
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
				foreach (BlockSlot slot in GetSlots()) {
					if (!slot.IsComplete) return false;
				}
				return true;
			}
		}		
    	
    	
    	public StatementType StatementType {
    		get { return behaviour.StatementType; }
    	}
    	
    	    	
    	static Statement()
    	{
    		GradientStopCollection stops = new GradientStopCollection(3);
    		stops.Add(new GradientStop(Colors.Gray,-0.5));
    		stops.Add(new GradientStop(Colors.White,0.5));
    		stops.Add(new GradientStop(Colors.Gray,1.5));
    		defaultBrush = new LinearGradientBrush(stops,new Point(0,0),new Point(1,1));
    		
    		stops = new GradientStopCollection(3);
    		stops.Add(new GradientStop(Colors.Green,-0.5));
    		stops.Add(new GradientStop(Colors.White,0.5));
    		stops.Add(new GradientStop(Colors.Green,1.5));
    		actionBrush = new LinearGradientBrush(stops,new Point(0,0),new Point(1,1));
    		
    		stops = new GradientStopCollection(3);
    		stops.Add(new GradientStop(Colors.Red,-0.5));
    		stops.Add(new GradientStop(Colors.White,0.5));
    		stops.Add(new GradientStop(Colors.Red,1.5));
    		conditionBrush = new LinearGradientBrush(stops,new Point(0,0),new Point(1,1));
    	}
    	
    	
    	/// <summary>
    	/// For deserialisation.
    	/// </summary>
    	public Statement()
    	{    	
        	InitializeComponent();
        	MouseDoubleClick += delegate { Behaviour = new DefaultStatementBehaviour(); };
    	}
    	
        
        public Statement(StatementBehaviour behaviour)
        {
        	if (behaviour == null) throw new ArgumentNullException("behaviour");
        	
        	InitializeComponent();
        	Behaviour = behaviour;
        	MouseDoubleClick += delegate { Behaviour = new DefaultStatementBehaviour(); };
        }
        
        
        protected void Initialise()
        {
        	if (behaviour == null) throw new InvalidOperationException("This Statement has not been assigned a behaviour.");
        	        	
        	mainPanel.Children.Clear();
        	
        	foreach (StatementComponent component in behaviour.GetComponents()) {
        		AddComponent(component);
        	}
        }
        
        
        protected void AddComponent(StatementComponent info)
        {
        	switch (info.ComponentType) {
        		case ComponentType.Attribute:
        			break;
        		case ComponentType.Label:
        			AddLabel(info.LabelText);
        			break;
        		case ComponentType.Parameter:
        			AddParameter(info.ParameterFitter);
        			break;
        	}
        	
        	OnChanged(new EventArgs());
        }
        
        
        protected void AddParameter(Fitter fitter)
        {        	
        	if (fitter == null) throw new ArgumentNullException("fitter","Can't add a parameter without providing a fitter.");
        	
        	BlockSlot parameter = new BlockSlot(fitter);
        	AddParameter(parameter);
        }

        
        protected void AddParameter(BlockSlot parameter)
        {
        	if (parameter == null) throw new ArgumentNullException("parameter","Can't add a null parameter.");
        	
        	parameter.Changed += delegate { 
        		OnChanged(new EventArgs()); 
        	};
        	
        	mainPanel.Children.Add(parameter);
        }

        
        protected void AddLabel(StatementLabel label)
        {
        	if (label == null) throw new ArgumentNullException("label","Can't add a null label.");
        	
        	mainPanel.Children.Add(label);
        }

        
        protected void AddLabel(string text)
        {
        	if (text == null) throw new ArgumentNullException("text","Can't add a label with null text.");
        	
        	StatementLabel label = new StatementLabel(text,GetBrush());
        	
        	AddLabel(label);
        }
        
        
        protected virtual Brush GetBrush()
        {
        	switch (StatementType) {
        		case StatementType.Action:
        			return actionBrush;
        		case StatementType.Condition:
        			return conditionBrush;
        		default:
        			return defaultBrush;
        	}
        }
		
		
		public override string ToString()
		{
			return GetNaturalLanguage();
		}
		
		
		public override Moveable DeepCopy()
		{
			Statement copy = new Statement(behaviour.DeepCopy());
			
			List<BlockSlot> slots = GetSlots();
			List<BlockSlot> copySlots = copy.GetSlots();
			
			if (slots.Count != copySlots.Count) 
				throw new ApplicationException(String.Format("Statement.DeepCopy() returned a copy with the wrong number of slots (expected {0} but found {1}.",
				                                             slots.Count,copySlots.Count));
				
			for (int i = 0; i < slots.Count; i++) {
				Moveable contents = slots[i].Contents;
				if (contents != null) copySlots[i].Contents = contents.DeepCopy();
				else copySlots[i].Contents = null;
			}
						
			return copy;
		}
		
		
		public List<BlockSlot> GetSlots()
		{
			List<BlockSlot> slots = new List<BlockSlot>(2);
			foreach (UIElement element in mainPanel.Children) {
				BlockSlot slot = element as BlockSlot;
				if (slot != null) slots.Add(slot);
			}
			return slots;
		}
		
		
		public override string GetCode()
		{
			if (behaviour == null) return "BEHAVIOUR_MISSING";
			
			List<BlockSlot> slots = GetSlots();
			string[] args = new string[slots.Count];
			
			foreach (BlockSlot slot in slots) {
				args[slots.IndexOf(slot)] = slot.GetCode();
			}
			
			return behaviour.GetCode(args);
		}
		
		
		public override string GetNaturalLanguage()
		{
			if (behaviour == null) return "BEHAVIOUR_MISSING";
			
			List<BlockSlot> slots = GetSlots();
			string[] args = new string[slots.Count];
			
			foreach (BlockSlot slot in slots) {
				args[slots.IndexOf(slot)] = slot.GetNaturalLanguage();
			}
			
			return behaviour.GetNaturalLanguage(args);
		}	
		
		
		public override void AssignImage(ImageProvider imageProvider)
		{
			if (imageProvider == null) throw new ArgumentNullException("imageProvider");
			
			foreach (BlockSlot slot in GetSlots()) {
				if (slot.Contents != null) slot.Contents.AssignImage(imageProvider);
			}
		}
			
		
		public override XmlSchema GetSchema()
		{
			return null;
		}
		
		
		public override void ReadXml(XmlReader reader)
		{
			reader.MoveToContent();		
			
			ReadCoordinates(reader);	
			
			if (reader.IsEmptyElement || !reader.ReadToDescendant("Behaviour")) {
				throw new FormatException("Statement does not specify a Behaviour, and could not be deserialised.");
			}
						
			Behaviour = (StatementBehaviour)SerialisationHelper.GetObjectFromXml(reader);
			
			reader.MoveToContent();
			
			if (reader.LocalName != "Slots") throw new FormatException("Statement does not specify Slots, and could not be deserialised.");
				
			bool collectionIsEmpty = reader.IsEmptyElement;			
			reader.ReadStartElement();
			
			if (!collectionIsEmpty) {
				
				List<BlockSlot> slots = GetSlots();
				
				reader.MoveToContent();
				
				while (reader.LocalName == "Slot") {
					
					bool slotIsEmpty = reader.IsEmptyElement;
					
					BlockSlot slot;
					int index;
					try {
						index = int.Parse(reader.GetAttribute("Index"));
						slot = slots[index];
					}
					catch (Exception e) {
						throw new FormatException("Slot did not define a valid Index value.",e);
					}
						
					reader.ReadStartElement();
					
					if (!slotIsEmpty) {
						reader.MoveToContent();			
						
						Moveable moveable;
						
						if (reader.LocalName == "ObjectBlock") moveable = new ObjectBlock();
						else if (reader.LocalName == "NumberBlock") moveable = new NumberBlock();
						else if (reader.LocalName == "StringBlock") moveable = new StringBlock();
						else throw new FormatException("Unrecognised Moveable type (" + reader.LocalName + ") or Moveable data not found.");
						
						moveable.ReadXml(reader);
						slot.Contents = moveable;
						
						reader.ReadEndElement();							
					}
					
					reader.MoveToContent();
				}
			
				reader.ReadEndElement(); // </Slots>
				reader.MoveToContent();
			}	
			
			reader.ReadEndElement(); // </Statement>
		}
		
		
		public override void WriteXml(XmlWriter writer)
		{
			if (behaviour == null) {
				throw new InvalidOperationException("The Statement being serialised has a null behaviour field.");
			}
			
			WriteCoordinates(writer);
			
			writer.WriteStartElement("Behaviour");
			writer.WriteAttributeString("Type",behaviour.GetType().FullName);
			behaviour.WriteXml(writer);
			writer.WriteEndElement();
			
			writer.WriteStartElement("Slots");
			
			List<BlockSlot> slots = GetSlots();
			
			foreach (BlockSlot slot in slots) {
				writer.WriteStartElement("Slot");
				writer.WriteAttributeString("Index",slots.IndexOf(slot).ToString());
				
				if (slot.Contents != null) {
					writer.WriteStartElement(slot.Contents.GetType().Name);
					slot.Contents.WriteXml(writer);
					writer.WriteEndElement();
				}
				
				writer.WriteEndElement();
			}
			
			writer.WriteEndElement();
		}
    }
}