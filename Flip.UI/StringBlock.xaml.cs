﻿using System;
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
    /// Interaction logic for StringBlock.xaml
    /// </summary>
    public partial class StringBlock : Moveable
    {
    	public static readonly Size DefaultSize;
    	protected static DependencyProperty ValueProperty;
    	protected TextShorteningConverter converter;
    	protected int maxLength;
    	
    	
    	public string Value {
    		get {
    			return (string)base.GetValue(ValueProperty);
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
    	
    	
    	static StringBlock()
    	{
    		DefaultSize = new Size(150,ObjectBlock.DefaultSize.Height);
            ValueProperty = DependencyProperty.Register("Value",typeof(string),typeof(StringBlock));
    	}
    	
    	
    	public StringBlock() : this(String.Empty,40)
        {
        }
    	
    	
    	public StringBlock(string value) : this(value,40)
    	{    		
    	}

    	
    	public StringBlock(string value, uint displayLength)
    	{
    		this.converter = new TextShorteningConverter(displayLength);
    		
    		Binding binding = new Binding("Value");
    		binding.Converter = converter;
    		binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;    		
    		
    		InitializeComponent();    	
    		    		
    		valueTextBlock.SetBinding(TextBlock.TextProperty,binding);
    		
    		Value = value;
    		Height = DefaultSize.Height;
    		Width = DefaultSize.Width;    		
    		maxLength = 140;
    	}
        
        
		public override Moveable DeepCopy()
		{
			return new StringBlock(Value,converter.Length);
		}
		
		
		public override string GetCode()
		{
			return String.Format("\"{0}\"",Value);
		}
		
		
		public override string GetNaturalLanguage()
		{
			return String.Format("\"{0}\"",Value);
		}
		
		
		public override string ToString()
		{
			return Value;
		}
		
    	
		public bool Equals(StringBlock other)
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
			
			if (!reader.IsEmptyElement) throw new FormatException("StringBlock should not have children.");
			
			try {
				Value = reader.GetAttribute("Value");
			}
			catch (Exception e) {
				throw new FormatException("Serialised StringBlock does not define a valid Value.",e);
			}
			
			reader.ReadStartElement();
		}
		
		
		public override void WriteXml(XmlWriter writer)
		{
			WriteCoordinates(writer);
			writer.WriteAttributeString("Value",Value);
		}
		
		
		protected void ChangeValue(object sender, RoutedEventArgs e)
		{
			new ChangeStringDialog(this,maxLength).ShowDialog();
		}
		
		
		public override string GetLogText()
		{
			return "String (" + Value + ")";
		}
        
        
		public override ScriptStats GetStatistics()
		{
			ScriptStats s = new ScriptStats();
			s.StringBlock++;
			return s;
		}
    }
}


