using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Sussex.Flip.UI
{
    /// <summary>
    /// Interaction logic for NotBlock.xaml
    /// </summary>
    public partial class NotBlock : BooleanBlock
    {
    	protected ConditionSlot slot1;
    	
    	
		public override bool IsComplete { 
    		get { return slot1.IsComplete; }
    	}
		
		
        public NotBlock()
        {
            InitializeComponent();
            
            BooleanExpressionFitter fitter = new BooleanExpressionFitter();
            
            slot1 = new ConditionSlot(fitter);
            
            TextBlock text = new TextBlock();
            text.Text = "NOT";
            text.FontSize = 18;
            text.Background = Brushes.Blue;
            text.Foreground = Brushes.HotPink;
            
            stackPanel.Children.Add(text);
            stackPanel.Children.Add(slot1);
        }
    	
		
    	public override void AssignImage(ImageProvider imageProvider)
    	{    	
			if (imageProvider == null) throw new ArgumentNullException("imageProvider");
			
			if (slot1.Contents != null) slot1.Contents.AssignImage(imageProvider);
    	}
    	    	
    	
		public override Moveable DeepCopy()
		{			
			NotBlock copy = new NotBlock();
			if (slot1.Contents != null) copy.slot1.Contents = slot1.Contents.DeepCopy();
			return copy;			
		}
		
		
		public override string GetCode()
		{
			return String.Format("(~({0}))",slot1.GetCode());
		}
		
		
		public override string GetNaturalLanguage()
		{			
			// TODO:
			// slot1 should be able to return different natural language depending on
			// whether it has a not appended. i.e. it should be 'player has sword'
			// or 'player doesn't have sword', not 'player has sword' or 'it is not the case that player has sword'.
			return String.Format("it is not the case that {0}",slot1.GetNaturalLanguage());
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
					slot1.Contents = (BooleanBlock)SerialisationHelper.GetObjectFromXmlInExecutingAssembly(reader);
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
    }
}