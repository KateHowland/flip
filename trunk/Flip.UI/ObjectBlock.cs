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
    	protected static DependencyProperty DisplayImageProperty;
    	protected static DependencyProperty BehaviourProperty;
    	
    	
    	static ObjectBlock()
    	{
    		DefaultSize = new Size(55,65);    		
    		DisplayImageProperty = DependencyProperty.Register("DisplayImage",typeof(Image),typeof(ObjectBlock));
    		BehaviourProperty = DependencyProperty.Register("Behaviour",typeof(ObjectBehaviour),typeof(ObjectBlock));
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
    	
    	
    	public ObjectBehaviour Behaviour {
    		get { return (ObjectBehaviour)base.GetValue(BehaviourProperty); }
    		set { base.SetValue(BehaviourProperty,value); }
    	}
    	
    	
        public ObjectBlock(Image image, ObjectBehaviour behaviour)
        {
        	if (behaviour == null) throw new ArgumentNullException("behaviour");
        	
        	Behaviour = behaviour;
        	
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
        
        
		public override Moveable DeepCopy()
		{
			Image img = new Image();
			img.Source = DisplayImage.Source;
			ObjectBlock copy = new ObjectBlock(img,Behaviour.DeepCopy());
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
		
		
		public string GetDescriptionOfObjectType()
		{
			return Behaviour.GetDescriptionOfObjectType();
		}
		
		
		public override string ToString()
		{
			// TODO:
			// TEMP:
			return GetDescriptionOfObjectType();
		}
    }
}