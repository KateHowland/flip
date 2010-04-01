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
    	
    	
    	protected static DependencyProperty BehaviourProperty;
    	
    	
    	static ObjectBlock()
    	{
    		DefaultSize = new Size(55,65);
    		
    		TypeProperty = DependencyProperty.Register("Type",typeof(string),typeof(ObjectBlock));
    		SubtypeProperty = DependencyProperty.Register("Subtype",typeof(string),typeof(ObjectBlock));
    		IdentifierProperty = DependencyProperty.Register("Identifier",typeof(string),typeof(ObjectBlock));
    		DisplayNameProperty = DependencyProperty.Register("DisplayName",typeof(string),typeof(ObjectBlock));
    		DisplayImageProperty = DependencyProperty.Register("DisplayImage",typeof(Image),typeof(ObjectBlock));
    		
    		BehaviourProperty = DependencyProperty.Register("Behaviour",typeof(TempObjectBehaviour),typeof(ObjectBlock));
    	}
    	
    	
    	public string Type {
    		get { return Behaviour.Type; }
    		set { Behaviour.Type = value; }
    	}
    	
    	
    	public string Subtype {
    		get { return Behaviour.Subtype; }
    		set { Behaviour.Subtype = value; }
    	}
    	
    	
    	public string Identifier {
    		get { return Behaviour.Identifier; }
    		set { Behaviour.Identifier = value; }
    	}
    	
    	
    	public string DisplayName {
    		get { return Behaviour.DisplayName; }
    		set { Behaviour.DisplayName = value; }
    	}
    	
    	
    	public Image DisplayImage {
    		get { return (Image)base.GetValue(DisplayImageProperty); }
    		set { base.SetValue(DisplayImageProperty,value); }
    	}
    	
    	
    	public TempObjectBehaviour Behaviour {
    		get { return (TempObjectBehaviour)base.GetValue(BehaviourProperty); }
    		set { base.SetValue(BehaviourProperty,value); }
    	}
    	
    	
        public ObjectBlock(Image image, object represented, string identifier, string type, string subtype, string displayName)
        {
        	Behaviour = new TempObjectBehaviour(type,subtype,identifier,displayName);
        	
            InitializeComponent();
            
            if (image == null) {
            	image = new Image();
            	Image res = FindResource("defaultimg") as Image;
            	if (res != null) {
            		image.Source = res.Source;
            	}            	
            }

    		DisplayImage = image;
    		
    		Height = DefaultSize.Height;
    		Width = DefaultSize.Width;
            
    		ToolTip = ToString();
        }
    	
    	
        public ObjectBlock(Image image) : this(image,null,String.Empty,String.Empty,String.Empty,String.Empty)
        {
        }
        
        
		public override Moveable DeepCopy()
		{
			Image img = new Image();
			img.Source = DisplayImage.Source;
			ObjectBlock copy = new ObjectBlock(img,null,Identifier,Type,Subtype,DisplayName);
			copy.Behaviour = (TempObjectBehaviour)Behaviour.DeepCopy(); //HACK
			return copy;
		}
		
		
		public override string GetCode()
		{
			return Behaviour.GetCode();
		}
		
		
		public override string GetNaturalLanguage()
		{
			return Behaviour.GetNaturalLanguage();
		}
    }
}