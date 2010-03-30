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
    	public static readonly Size DefaultSize;
    	
    	protected static DependencyProperty TypeProperty;
    	protected static DependencyProperty SubtypeProperty;
    	protected static DependencyProperty IdentifierProperty;
    	protected static DependencyProperty DisplayNameProperty;
    	protected static DependencyProperty DisplayImageProperty;
    	protected static DependencyProperty RepresentedObjectProperty;
    	
    	
    	static ObjectBlock()
    	{
    		DefaultSize = new Size(55,65);
    		
    		TypeProperty = DependencyProperty.Register("Type",typeof(string),typeof(ObjectBlock));
    		SubtypeProperty = DependencyProperty.Register("Subtype",typeof(string),typeof(ObjectBlock));
    		IdentifierProperty = DependencyProperty.Register("Identifier",typeof(string),typeof(ObjectBlock));
    		DisplayNameProperty = DependencyProperty.Register("DisplayName",typeof(string),typeof(ObjectBlock));
    		DisplayImageProperty = DependencyProperty.Register("DisplayImage",typeof(Image),typeof(ObjectBlock));
    		RepresentedObjectProperty = DependencyProperty.Register("RepresentedObject",typeof(object),typeof(ObjectBlock));
    	}
    	
    	
    	public string Type {
    		get { return (string)base.GetValue(TypeProperty); }
    		set { base.SetValue(TypeProperty,value); }
    	}
    	
    	
    	public string Subtype {
    		get { return (string)base.GetValue(SubtypeProperty); }
    		set { base.SetValue(SubtypeProperty,value); }
    	}
    	
    	
    	public string Identifier {
    		get { return (string)base.GetValue(IdentifierProperty); }
    		set { base.SetValue(IdentifierProperty,value); }
    	}
    	
    	
    	public string DisplayName {
    		get { return (string)base.GetValue(DisplayNameProperty); }
    		set { base.SetValue(DisplayNameProperty,value); }
    	}
    	
    	
    	public Image DisplayImage {
    		get { return (Image)base.GetValue(DisplayImageProperty); }
    		set { base.SetValue(DisplayImageProperty,value); }
    	}
    	
    	
    	public object RepresentedObject {
    		get { return (object)base.GetValue(RepresentedObjectProperty); }
    		set { base.SetValue(RepresentedObjectProperty,value); }
    	}
    	
    	
        public ObjectBlock(Image image, object represented, string identifier, string type, string subtype, string displayName)
        {
            InitializeComponent();
            
            if (image == null) {
            	image = new Image();
            	Image res = FindResource("defaultimg") as Image;
            	if (res != null) {
            		image.Source = res.Source;
            	}            	
            }

    		Type = type;
    		Subtype = subtype;
    		Identifier = identifier;
    		DisplayName = displayName;
    		DisplayImage = image;
    		RepresentedObject = represented;
    		
    		Height = DefaultSize.Height;
    		Width = DefaultSize.Width;
            
    		ToolTip = ToString();
            
//            Template = (ControlTemplate)Resources["objectBlockControlTemplate"];
        }
    	
    	
        public ObjectBlock(Image image) : this(image,null,String.Empty,String.Empty,String.Empty,String.Empty)
        {
        }
        
        
		public override Moveable DeepCopy()
		{
			Image img = new Image();
			img.Source = DisplayImage.Source;
			ObjectBlock copy = new ObjectBlock(img,RepresentedObject,Identifier,Type,Subtype,DisplayName);
			return copy;
		}
    	   	    	
    	
    	// protected ObjectBlockBehaviour behaviour;
    	// behaviour.GetSource();
    	// behaviour.GetNaturalLanguage();
		
		
		public override string GetCode()
		{
			return Identifier;
		}
		
		
		public override string GetNaturalLanguage()
		{
			return DisplayName;
		}
    }
}