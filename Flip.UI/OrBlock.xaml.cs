using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Sussex.Flip.UI
{
    /// <summary>
    /// Interaction logic for OrBlock.xaml
    /// </summary>

    public partial class OrBlock : BooleanBlock
    {
    	protected ConditionSlot slot1, slot2;
    	
    	
		public override bool IsComplete { 
    		get { return false; }
    	}
		
		
        public OrBlock()
        {
            InitializeComponent();
            
            BooleanExpressionFitter fitter = new BooleanExpressionFitter();
            
            slot1 = new ConditionSlot(fitter);
            slot2 = new ConditionSlot(fitter);
            
            TextBlock text = new TextBlock();
            text.Text = "OR";
            text.FontSize = 18;
            text.Background = Brushes.Blue;
            text.Foreground = Brushes.Orange;
            
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
			OrBlock copy = new OrBlock();
			if (slot1.Contents != null) copy.slot1.Contents = slot1.Contents.DeepCopy();
			if (slot2.Contents != null) copy.slot2.Contents = slot2.Contents.DeepCopy();
			return copy;			
		}
		
		
		public override string GetCode()
		{
			return String.Format("({0} | {1})",slot1.GetCode(),slot2.GetCode());
		}
		
		
		public override string GetNaturalLanguage()
		{			
			return String.Format("either {0} or {1}",slot1.GetNaturalLanguage(),slot2.GetNaturalLanguage());
		}
		
			
		public override XmlSchema GetSchema()
		{
			return null;
		}
		
		
		public override void WriteXml(XmlWriter writer)
		{
			WriteCoordinates(writer);
			
			writer.WriteStartElement("Condition1");
			if (slot1.Contents != null) {
				slot1.Contents.WriteXml(writer);
			}
			writer.WriteEndElement();
			
			writer.WriteStartElement("Condition2");
			if (slot2.Contents != null) {
				slot2.Contents.WriteXml(writer);
			}
			writer.WriteEndElement();
		}
		
		
		public override void ReadXml(XmlReader reader)
		{
			throw new NotImplementedException();
			
//			reader.MoveToContent();		
//			
//			ReadCoordinates(reader);
//			
//			reader.ReadStartElement(); // passed <OrBlock>
//			
//			reader.MoveToContent();
//			
//			if (reader.LocalName != "Condition1") throw new FormatException("Condition1 is not specified.");
//			
//			bool isEmpty = reader.IsEmptyElement;
//			
//			reader.ReadStartElement("Condition1"); // passed <Condition1>
//			reader.MoveToContent(); // at <the contents of Condition1> or <Condition2>
//			
//			if (!isEmpty) {
//				
//				if (reader.LocalName != "Statement") throw new FormatException("Condition is not correctly specified.");
//				
//				Statement statement = new Statement();
//				statement.ReadXml(reader);
//				Condition = statement;
//				reader.MoveToContent();
//				
//				reader.ReadEndElement(); // passed </Condition>
//				reader.MoveToContent(); // at <Consequences>
//			}
//			
//			if (reader.IsEmptyElement) throw new FormatException("Consequences is not correctly specified (no Spine).");
//			
//			reader.ReadStartElement("Consequences");
//			reader.MoveToContent();			
//			Consequences.ReadXml(reader);
//			reader.MoveToContent();
//			reader.ReadEndElement();
//			reader.MoveToContent();
//			
//			if (reader.IsEmptyElement) throw new FormatException("Alternative is not correctly specified (no Spine).");
//			
//			reader.ReadStartElement("Alternative");
//			reader.MoveToContent();			
//			Alternative.ReadXml(reader);
//			reader.MoveToContent();
//			reader.ReadEndElement();
//			
//			reader.MoveToContent();
//			reader.ReadEndElement();
		}
    }
}