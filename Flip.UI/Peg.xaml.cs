using System;
using System.Windows.Controls;
using System.Xml;
using System.Xml.Schema;

namespace Sussex.Flip.UI
{
    /// <summary>
    /// Interaction logic for Peg.xaml
    /// </summary>

    public partial class Peg : UserControl
    {  	    	
		protected DropZone dropZone;
		protected PegSlot slot;
		
		
		public DropZone DropZone {
			get { return dropZone; }
		}

		
		public PegSlot Slot {
			get { return slot; }
		}
		
    	
        public Peg(Fitter fitter)
        {       
            InitializeComponent();
            
            dropZone = new DropZone(fitter);
            Grid.SetRow(dropZone,2);
            Grid.SetColumn(dropZone,0);
            Grid.SetColumnSpan(dropZone,2);
            mainGrid.Children.Add(dropZone);
            
            slot = new PegSlot(fitter);
            slot.MinHeight = 70;
            slot.MinWidth = 130;
            Grid.SetRow(slot,0);
            Grid.SetRowSpan(slot,2);
            Grid.SetColumn(slot,1);
            mainGrid.Children.Add(slot);
        }  
		
    	
		public XmlSchema GetSchema()
		{
			return null;
		}
		
		
		public void ReadXml(XmlReader reader)
		{
			bool isEmpty = reader.IsEmptyElement;
			
			reader.ReadStartElement();
			
			if (!isEmpty) {
				reader.MoveToContent();
				slot.ReadXml(reader);
				reader.ReadEndElement();
			}
			
			reader.MoveToContent();
		}
		
		
		public void WriteXml(XmlWriter writer)
		{
			writer.WriteStartElement("Slot");
			slot.WriteXml(writer);
			writer.WriteEndElement();
		}
    }
}