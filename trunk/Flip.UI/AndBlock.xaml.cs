using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Sussex.Flip.UI
{
    /// <summary>
    /// Interaction logic for AndBlock.xaml
    /// </summary>

    public partial class AndBlock : BooleanBlock
    {
    	protected ConditionSlot slot1, slot2;
    	
    	
		public override bool IsComplete { 
    		get { return slot1.IsComplete && slot2.IsComplete; }
    	}
		
		
        public AndBlock()
        {
            InitializeComponent();
            
            BooleanExpressionFitter fitter = new BooleanExpressionFitter();
            
            slot1 = new ConditionSlot(fitter);
            slot2 = new ConditionSlot(fitter);
            
            slot1.Changed += delegate(object sender, EventArgs e) { OnChanged(e); };
            slot2.Changed += delegate(object sender, EventArgs e) { OnChanged(e); };
            
            TextBlock text = new TextBlock();
            text.Text = "AND";
            text.FontSize = 18;
            text.FontWeight = FontWeights.ExtraBold;
            text.Padding = new Thickness(8);
            text.Background = Brushes.Transparent;
            text.Foreground = Brushes.Yellow;
            text.VerticalAlignment = VerticalAlignment.Center;
            
            stackPanel.Children.Add(slot1);
            stackPanel.Children.Add(text);
            stackPanel.Children.Add(slot2);
        }
    	
		
    	public override void AssignImage(ImageProvider imageProvider)
    	{    	
			if (imageProvider == null) throw new ArgumentNullException("imageProvider");
			
			if (slot1.Contents != null) slot1.Contents.AssignImage(imageProvider);
			if (slot2.Contents != null) slot2.Contents.AssignImage(imageProvider);
    	}
    	    	
    	
		public override Moveable DeepCopy()
		{			
			AndBlock copy = new AndBlock();
			if (slot1.Contents != null) copy.slot1.Contents = slot1.Contents.DeepCopy();
			if (slot2.Contents != null) copy.slot2.Contents = slot2.Contents.DeepCopy();
			return copy;			
		}
		
		
		public override string GetCode()
		{
			return String.Format("({0} & {1})",slot1.GetCode(),slot2.GetCode());
		}
		
		
		public override string GetNaturalLanguage()
		{			
			return String.Format("{0} and {1}",slot1.GetNaturalLanguage(),slot2.GetNaturalLanguage());
		}
				
		
		public override XmlSchema GetSchema()
		{
			return null;
		}
		
		
		public override void WriteXml(XmlWriter writer)
		{
			WriteCoordinates(writer);
			writer.WriteAttributeString("Type",this.GetType().FullName);
			
			writer.WriteStartElement("Condition1");
			if (slot1.Contents != null) {
				if (!Fitter.IsBooleanExpression(slot1.Contents)) {
					throw new ApplicationException("Condition1 was not a Statement or BooleanBlock.");
				}
				
				if (slot1.Contents is Statement) writer.WriteStartElement("Statement");
				else if (slot1.Contents is BooleanBlock) writer.WriteStartElement("Boolean");
				
				slot1.Contents.WriteXml(writer);
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
			
			writer.WriteStartElement("Condition2");
			if (slot2.Contents != null) {
				if (!Fitter.IsBooleanExpression(slot2.Contents)) {
					throw new ApplicationException("Condition2 was not a Statement or BooleanBlock.");
				}
				
				if (slot2.Contents is Statement) writer.WriteStartElement("Statement");
				else if (slot2.Contents is BooleanBlock) writer.WriteStartElement("Boolean");
				
				slot2.Contents.WriteXml(writer);
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
		}
		
		
		public override void ReadXml(XmlReader reader)
		{
			reader.MoveToContent();		
			
			ReadCoordinates(reader);
			
			reader.ReadStartElement();
			
			reader.MoveToContent();
			
			bool isEmpty = reader.IsEmptyElement;
			
			reader.ReadStartElement();
			reader.MoveToContent();
			
			if (!isEmpty) {
				
				if (reader.LocalName == "Statement") {					
					Statement statement = new Statement();
					statement.ReadXml(reader);
					slot1.Contents = statement;
					reader.MoveToContent();
				}
				
				else if (reader.LocalName == "Boolean") {					
					slot1.Contents = FlipWindow.ChosenDeserialisationHelper.GetBooleanBlock(reader);//(BooleanBlock)SerialisationHelper.GetObjectFromXmlInExecutingAssembly(reader);
					reader.MoveToContent();
				}
				
				else {
					throw new FormatException("Condition is not correctly specified.");
				}
				
				reader.ReadEndElement();
				reader.MoveToContent();
			}
			
			isEmpty = reader.IsEmptyElement;
			
			reader.ReadStartElement();
			reader.MoveToContent();
			
			if (!isEmpty) {
				
				if (reader.LocalName == "Statement") {					
					Statement statement = new Statement();
					statement.ReadXml(reader);
					slot2.Contents = statement;
					reader.MoveToContent();
				}
				
				else if (reader.LocalName == "Boolean") {
					slot2.Contents = FlipWindow.ChosenDeserialisationHelper.GetBooleanBlock(reader);//(BooleanBlock)SerialisationHelper.GetObjectFromXmlInExecutingAssembly(reader);
					reader.MoveToContent();
				}
				
				else {
					throw new FormatException("Condition is not correctly specified.");
				}
				
				reader.ReadEndElement();
				reader.MoveToContent();
			}
			
			reader.ReadEndElement();
		}
		
		
		public override string GetLogText()
		{
			return "AND";
		}
    }
}