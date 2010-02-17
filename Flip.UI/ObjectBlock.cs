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

namespace Sussex.Flip.UI
{
    /// <summary>
    /// Interaction logic for ObjectBlock.xaml
    /// </summary>
    public partial class ObjectBlock : Moveable
    {
    	protected string type;
    	protected string subtype;
    	protected string identifier;
    	protected string displayName;
    	// protected SourceProvider source;
    	// protected NaturalLanguageProvider natural;
    	protected object represented;
    	
    	
		public string Type {
			get { return type; }
		}
    	
    	
		public string Subtype {
			get { return subtype; }
		}
    	
    	
		public string Identifier {
			get { return identifier; }
		}
    	
    	
		public string DisplayName {
			get { return displayName; }
		}
    	
    	
		public object Represented {
			get { return represented; }
		}
    	
    	
		public Image Image {
			get { 
    			Image image = Face.Content as Image;
    			if (image == null) throw new InvalidOperationException("Content is not a valid image.");
    			else return image;
    		}
			set { 
    			Face.Content = value;
    		}
		}
    	
    	
        public ObjectBlock(Image image, object represented, string identifier, string type, string subtype, string displayName)
        {
            InitializeComponent();
            
            Image = image;
            this.represented = represented;
            this.identifier = identifier;
            this.type = type;
            this.subtype = subtype;
            
            ToolTip = ToString();
        }
    	
    	
        public ObjectBlock(Image image) : this(image,null,String.Empty,String.Empty,String.Empty,String.Empty)
        {
        }
        
        
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder("Name: ");
			sb.AppendLine(displayName);
			sb.Append("Identifier: ");
			sb.AppendLine(identifier);
			sb.Append("Type: ");
			sb.AppendLine(type);
			sb.Append("Subtype: ");
			sb.Append(subtype);
			return sb.ToString();
		}
    }
}