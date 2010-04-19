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
using Sussex.Flip.Utils;

namespace Sussex.Flip.UI
{
    /// <summary>
    /// Interaction logic for StatementLabel.xaml
    /// </summary>

    public partial class StatementLabel : UserControl, IDeepCopyable<StatementLabel>
    {
    	protected static ResourceDictionary resourceDictionary;
    	
    	
    	public string Text {
    		get { return textBlock.Text; }
    		set { textBlock.Text = value; }
    	}
    	
    	
    	public Brush BackgroundBrush {
    		get { return textBlock.Background.Clone(); }
    		set { textBlock.Background = value; }
    	}
    	
    	
    	static StatementLabel()
    	{    		
            resourceDictionary = new ResourceDictionary();
            
            Style style = new Style(typeof(TextBlock));
            style.Setters.Add(new Setter(TextBlock.FontSizeProperty,16.0));
            style.Setters.Add(new Setter(TextBlock.FontFamilyProperty,new FontFamily("Helvetica")));
            style.Setters.Add(new Setter(TextBlock.ForegroundProperty,Brushes.Black));
            style.Setters.Add(new Setter(TextBlock.WidthProperty,80.0));
            style.Setters.Add(new Setter(TextBlock.PaddingProperty,new Thickness(10)));
            style.Setters.Add(new Setter(TextBlock.HorizontalAlignmentProperty,HorizontalAlignment.Center));
            style.Setters.Add(new Setter(TextBlock.VerticalAlignmentProperty,VerticalAlignment.Center));
            style.Setters.Add(new Setter(TextBlock.TextAlignmentProperty,TextAlignment.Center));
    		
            resourceDictionary.Add(style.TargetType,style);
    	}
        
        
        public StatementLabel(string text) : this(text,Brushes.Gray)
        {
        }
        
        
        public StatementLabel(string text, Brush background)
        {
            InitializeComponent();
        	Text = text;
        	BackgroundBrush = background;
        	Resources = resourceDictionary;
        }
        
        
        public StatementLabel DeepCopy()
        {
        	return new StatementLabel(Text,BackgroundBrush);
        }
    }
}