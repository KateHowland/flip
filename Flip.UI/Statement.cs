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
		
		
		/// <summary>
		/// Check whether this Flip component has all essential fields filled in,
		/// including those belonging to subcomponents, such that it can generate valid code.
		/// </summary>
		/// <returns>True if all essential fields have been given values; false otherwise.</returns>
		/// <remarks>Note that this method makes no attempt to judge whether the values
		/// are valid in their slots, only that those slots have been filled.</remarks>
		public override bool IsComplete { 
			get { 
				foreach (ObjectBlockSlot slot in GetSlots()) {
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
    	
        
        public Statement(StatementBehaviour behaviour)
        {
        	if (behaviour == null) throw new ArgumentNullException("behaviour");
        	
        	InitializeComponent();
        	this.behaviour = behaviour;
        	Initialise();
        }
        
        
        protected void Initialise()
        {
        	if (behaviour == null) throw new InvalidOperationException("This Statement has not been assigned a behaviour.");
        	
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
        	
        	ObjectBlockSlot parameter = new ObjectBlockSlot(fitter);
        	AddParameter(parameter);
        }

        
        protected void AddParameter(ObjectBlockSlot parameter)
        {
        	if (parameter == null) throw new ArgumentNullException("parameter","Can't add a null parameter.");
        	
        	parameter.MoveableChanged += delegate { 
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
			
			List<ObjectBlockSlot> slots = GetSlots();
			List<ObjectBlockSlot> copySlots = copy.GetSlots();
				
			if (slots.Count != copySlots.Count) 
				throw new ApplicationException("Statement.DeepCopy() returned a copy with a different number of slots.");
				
			for (int i = 0; i < slots.Count; i++) {
				Moveable contents = slots[i].Contents;
				if (contents != null) copySlots[i].Contents = contents.DeepCopy();
				else copySlots[i].Contents = null;
			}
			
			return copy;
		}
		
		
		public List<ObjectBlockSlot> GetSlots()
		{
			List<ObjectBlockSlot> slots = new List<ObjectBlockSlot>(2);
			foreach (UIElement element in mainPanel.Children) {
				ObjectBlockSlot slot = element as ObjectBlockSlot;
				if (slot != null) slots.Add(slot);
			}
			return slots;
		}
		
		
		public override string GetCode()
		{
			if (behaviour == null) return "BEHAVIOUR_MISSING";
			
			List<ObjectBlockSlot> slots = GetSlots();
			string[] args = new string[slots.Count];
			
			foreach (ObjectBlockSlot slot in slots) {
				args[slots.IndexOf(slot)] = slot.GetCode();
			}
			
			return behaviour.GetCode(args);
		}
		
		
		public override string GetNaturalLanguage()
		{
			if (behaviour == null) return "BEHAVIOUR_MISSING";
			
			List<ObjectBlockSlot> slots = GetSlots();
			string[] args = new string[slots.Count];
			
			foreach (ObjectBlockSlot slot in slots) {
				args[slots.IndexOf(slot)] = slot.GetNaturalLanguage();
			}
			
			return behaviour.GetNaturalLanguage(args);
		}
			
		
		public override XmlSchema GetSchema()
		{
			return null;
		}
		
		
		public override void ReadXml(XmlReader reader)
		{
			throw new NotImplementedException();
		}
		
		
		public override void WriteXml(XmlWriter writer)
		{
			writer.WriteElementString("SomeStatement",String.Empty);
		}
    }
}