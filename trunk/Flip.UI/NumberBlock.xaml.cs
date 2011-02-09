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
    /// Interaction logic for NumberBlock.xaml
    /// </summary>

    public partial class NumberBlock : Moveable
    {
    	public static readonly Size DefaultSize;
    	protected static DependencyProperty ValueProperty;
    	
    	
    	protected Int32 min, max;
    	
    	
    	public Int32 Value {
    		get {
    			return (Int32)base.GetValue(ValueProperty);
    		}
    		set {
    			if (Value != value) {
    				base.SetValue(ValueProperty,value);
    				OnChanged(new EventArgs());
    			}
    		}
    	}
    	
    	
		public override bool IsComplete {
    		get { return true; }
		}
    	
    	
    	static NumberBlock()
    	{
    		DefaultSize = ObjectBlock.DefaultSize;
            ValueProperty = DependencyProperty.Register("Value",typeof(Int32),typeof(NumberBlock));
    	}
    	
    	
    	public NumberBlock() : this(0)
        {
        }

    	
    	public NumberBlock(Int32 number)
    	{    		
    		min = 0;
    		max = 999;
    		
    		if (number < min) number = min;
    		if (number > max) number = max;
    		
    		InitializeComponent();
    		Value = number;
    		Height = DefaultSize.Height;
    		Width = DefaultSize.Width;
    	}
        
        
		public override Moveable DeepCopy()
		{
			return new NumberBlock(Value);
		}
		
		
		public override string GetCode()
		{
			return Value.ToString();
		}
		
		
		public override string GetNaturalLanguage()
		{
			return Value.ToString();
		}
		
		
		public override string ToString()
		{
			return Value.ToString();
		}
		
    	
		public bool Equals(NumberBlock other)
		{
			return other != null && other.Value == Value;
		}		
		
		
		public override void AssignImage(ImageProvider imageProvider)
		{
		}	
		
			
		public override XmlSchema GetSchema()
		{
			return null;
		}
		
		
		public override void ReadXml(XmlReader reader)
		{
			reader.MoveToContent();
			
			ReadCoordinates(reader);
			
			if (!reader.IsEmptyElement) throw new FormatException("NumberBlock should not have children.");
			
			try {
				Value = Int32.Parse(reader.GetAttribute("Value"));
			}
			catch (Exception e) {
				throw new FormatException("Serialised NumberBlock does not define a valid Value.",e);
			}
			
			reader.ReadStartElement();
		}
		
		
		public override void WriteXml(XmlWriter writer)
		{
			WriteCoordinates(writer);
			writer.WriteAttributeString("Value",Value.ToString());
		}
		
		
		protected void ChangeValue(object sender, RoutedEventArgs e)
		{
			new ChangeNumberDialog(this,min,max).ShowDialog();
		}
		
		
		public override string GetLogText()
		{
			return "Number (" + Value + ")";
		}
        
        
		public override Statistics GetStatistics()
		{
			Statistics s = new Statistics();
			s.NumberBlock++;
			return s;
		}
    }
}


