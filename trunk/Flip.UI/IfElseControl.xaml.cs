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
    public partial class IfElseControl : ConditionalControl
    {
    	protected ConditionSlot slot;
    	protected Spine consequenceSpine;
    	protected Spine alternativeSpine;
    	protected Thickness margin;
    	
    	
    	public override Statement Condition {
    		get { 
    			return slot.Contents as Statement; 
    		}
    		set {     			
    			if (value != null && value.StatementType != StatementType.Condition) {
    				throw new ArgumentException("Statement must have StatementType.Condition to be assigned as the condition of a ConditionalControl.");
    			}
    			slot.Contents = value;
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
        	
        	slot = new ConditionSlot(new ConditionFitter());
        	slot.Padding = new Thickness(10);
        	
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
				copy.Condition = (Statement)Condition.DeepCopy();
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
			string ifText = slot.GetNaturalLanguage();
			string thenText = consequenceSpine.GetNaturalLanguage();
			string elseText = alternativeSpine.GetNaturalLanguage();
			
			if (elseText == String.Empty) {			
				return String.Format("if {0}, {1}",ifText,thenText);
			}			
			else {
				return String.Format("if {0}, {1}; otherwise, {2}",ifText,thenText,elseText);
			}
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
				writer.WriteStartElement("Statement");
				Condition.WriteXml(writer);
				writer.WriteEndElement();
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
				
				if (reader.LocalName != "Statement") throw new FormatException("Condition is not correctly specified.");
				
				Statement statement = new Statement();
				statement.ReadXml(reader);
				Condition = statement;
				reader.MoveToContent();
				
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
    }
}