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
    	public string Text {
    		get { return textBlock.Text; }
    		set { textBlock.Text = value; }
    	}
    	
    	
    	public Brush BackgroundBrush {
    		get { return textBlock.Background.Clone(); }
    		set { textBlock.Background = value; }
    	}
        
        
        public StatementLabel(string text) : this(text,Brushes.Gray)
        {
        }
        
        
        public StatementLabel(string text, Brush background)
        {
            InitializeComponent();
        	Text = text;
        	BackgroundBrush = background;
        }
        
        
        public StatementLabel DeepCopy()
        {
        	return new StatementLabel(Text,BackgroundBrush);
        }
    }
}