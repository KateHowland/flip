using System;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Sussex.Flip.UI
{
    /// <summary>
    /// Interaction logic for IfElseControl.xaml
    /// </summary>
    public partial class IfElseControl : ControlStructure
    {
    	protected ConditionSlot slot;
    	protected Spine consequenceSpine;
    	protected Spine alternativeSpine;
    	protected Thickness margin;
    	    	
    	
    	public override Moveable Condition {
    		get { 
    			return slot.Contents;
    		}
    		set {     		
    			if (value == null || (value is Statement && ((Statement)value).StatementType == StatementType.Condition) || value is BooleanBlock) {
    				slot.Contents = value;
    			}
    			else {
    				throw new ArgumentException("Can only assign a conditional statement or a boolean expression as the condition of a control structure.");
    			}
    		}
    	}
        
    	
		public override Spine Consequences {
			get {
				return consequenceSpine;
			}
    		set {
    			if (value == null) {
    				throw new ArgumentNullException("value");
    			}    			
    			
    			grid.Children.Remove(consequenceSpine);
    			
    			consequenceSpine = value;
        	
	        	consequenceSpine.Changed += delegate 
	        	{ 
	        		OnChanged(new EventArgs()); 
	        	};          
	        	
	            Grid.SetRow(consequenceSpine,0);
	            Grid.SetColumn(consequenceSpine,0);
	            Grid.SetZIndex(consequenceSpine,-1);
            	consequenceSpine.Extends = border1.Height + 20; 
            	consequenceSpine.Margin = margin;
	            grid.Children.Add(consequenceSpine);
    		}
		}
    	
    	
		public Spine Alternative {
			get {
				return alternativeSpine;
			}
    		set {
    			if (value == null) {
    				throw new ArgumentNullException("value");
    			}    			
    			
    			grid.Children.Remove(alternativeSpine);
    			
    			alternativeSpine = value;
        	
	        	alternativeSpine.Changed += delegate 
	        	{ 
	        		OnChanged(new EventArgs()); 
	        	};
	        	
	            Grid.SetRow(alternativeSpine,1);
	            Grid.SetColumn(alternativeSpine,0);
	            Grid.SetZIndex(alternativeSpine,-1);
            	alternativeSpine.Extends = border2.Height + 20;
            	alternativeSpine.Margin = margin;
	            grid.Children.Add(alternativeSpine);
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
				return Condition != null && Condition.IsComplete && Consequences.IsComplete && Alternative.IsComplete;
			}
		}		
    	
    	
        public IfElseControl()
        {
        	margin = new Thickness(14,0,0,0);
        	
        	slot = new ConditionSlot(new BooleanExpressionFitter());
        	slot.MinWidth = 100;
        	slot.Padding = new Thickness(4);
        	
        	consequenceSpine = new Spine(new SpineFitter(),1);
        	alternativeSpine = new Spine(new SpineFitter(),1);
        	
        	slot.Changed += delegate 
        	{ 
        		OnChanged(new EventArgs());
        	};
        	
        	consequenceSpine.Changed += delegate 
        	{ 
        		OnChanged(new EventArgs()); 
        	};
        	
        	alternativeSpine.Changed += delegate 
        	{ 
        		OnChanged(new EventArgs()); 
        	};
            
            InitializeComponent();
                        
            stackPanel.Children.Insert(1,slot);
                   
            Consequences = consequenceSpine;
            Alternative = alternativeSpine;
        }

        
		public override Moveable DeepCopy()
		{	
			IfElseControl copy = new IfElseControl();
			
			if (Condition != null) {
				copy.Condition = Condition.DeepCopy();
			}
			
			copy.Consequences = Consequences.DeepCopy();
			copy.Alternative = Alternative.DeepCopy();
			
			return copy;
		}
    	
        
		public override string GetCode()
		{
			System.Text.StringBuilder code = new System.Text.StringBuilder();
			
			code.AppendLine(String.Format("if ({0}) {{",slot.GetCode()));
			code.AppendLine(consequenceSpine.GetCode());			
			code.AppendLine("}");		
			code.AppendLine("else {");
			code.AppendLine(alternativeSpine.GetCode());			
			code.AppendLine("}");		
			
			return code.ToString();
		}
    	
        
		public override string GetNaturalLanguage()
		{
			string ifText;
			if (slot.Contents == null) ifText = "if some condition is true, then";
			else ifText = String.Format("if {0}, then",slot.GetNaturalLanguage());
			
			string thenText;
			if (consequenceSpine.IsEmpty) thenText = " something happens, otherwise";
			else thenText = String.Format(" {0}, otherwise",consequenceSpine.GetNaturalLanguage());
			
			string elseText;
			if (alternativeSpine.IsEmpty) elseText = " something else happens";
			else elseText = " " + alternativeSpine.GetNaturalLanguage();
			
			return ifText + thenText + elseText;
		}
		
		
		public override void AssignImage(ImageProvider imageProvider)
		{
			if (imageProvider == null) throw new ArgumentNullException("imageProvider");
			
			if (Condition != null) Condition.AssignImage(imageProvider);
			if (Consequences != null) Consequences.AssignImage(imageProvider);
			if (Alternative != null) Alternative.AssignImage(imageProvider);
		}
		
		
		public override XmlSchema GetSchema()
		{
			return null;
		}
		
		
		public override void WriteXml(XmlWriter writer)
		{
			WriteCoordinates(writer);
			
			writer.WriteStartElement("Condition");
			if (Condition != null) {
				if (Condition is Statement) {
					writer.WriteStartElement("Statement");
					Condition.WriteXml(writer);
					writer.WriteEndElement();
				}
				else if (Condition is BooleanBlock) {
					writer.WriteStartElement("Boolean");
					Condition.WriteXml(writer);
					writer.WriteEndElement();
				}
				else {
					throw new ApplicationException("Condition was not a Statement or BooleanBlock.");
				}
			}
			writer.WriteEndElement();
			
			writer.WriteStartElement("Consequences");
			if (Consequences != null) {
				writer.WriteStartElement("Spine");
				Consequences.WriteXml(writer);
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
			
			writer.WriteStartElement("Alternative");
			if (Alternative != null) {
				writer.WriteStartElement("Spine");
				Alternative.WriteXml(writer);
				writer.WriteEndElement();
			}
			writer.WriteEndElement();			
		}
		
		
		public override void ReadXml(XmlReader reader)
		{
			reader.MoveToContent();		
			
			ReadCoordinates(reader);
			
			reader.ReadStartElement(); // passed <IfElseControl>
			
			reader.MoveToContent();
			
			if (reader.LocalName != "Condition") throw new FormatException("Condition is not specified.");
			
			bool isEmpty = reader.IsEmptyElement;
			
			reader.ReadStartElement("Condition"); // passed <Condition>
			reader.MoveToContent(); // at <Consequences> or <Statement>
			
			if (!isEmpty) {
				
				if (reader.LocalName == "Statement") {					
					Statement statement = new Statement();
					statement.ReadXml(reader);
					Condition = statement;
					reader.MoveToContent();
				}
				
				else if (reader.LocalName == "Boolean") {
					Condition = FlipWindow.ChosenDeserialisationHelper.GetBooleanBlock(reader);//(BooleanBlock)SerialisationHelper.GetObjectFromXmlInExecutingAssembly(reader);
					reader.MoveToContent();
				}
				
				else {
					throw new FormatException("Condition is not correctly specified.");
				}
				
				reader.ReadEndElement(); // passed </Condition>
				reader.MoveToContent(); // at <Consequences>
			}
			
			if (reader.IsEmptyElement) throw new FormatException("Consequences is not correctly specified (no Spine).");
			
			reader.ReadStartElement("Consequences");
			reader.MoveToContent();			
			Consequences.ReadXml(reader);
			reader.MoveToContent();
			reader.ReadEndElement();
			reader.MoveToContent();
			
			if (reader.IsEmptyElement) throw new FormatException("Alternative is not correctly specified (no Spine).");
			
			reader.ReadStartElement("Alternative");
			reader.MoveToContent();			
			Alternative.ReadXml(reader);
			reader.MoveToContent();
			reader.ReadEndElement();
			
			reader.MoveToContent();
			reader.ReadEndElement();
		}
		
		
		public override string GetLogText()
		{
			return "If..Then..Else";
		}
        
        
		public override Statistics GetStatistics()
		{		
			Statistics s = new Statistics();
			s.IfThenElse++;
			s.Add(slot.GetStatistics());
			s.Add(Consequences.GetStatistics());
			s.Add(Alternative.GetStatistics());
			return s;
		}
    }
}