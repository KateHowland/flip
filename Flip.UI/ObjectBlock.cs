using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Sussex.Flip.UI
{
    /// <summary>
    /// Interaction logic for ObjectBlock.xaml
    /// </summary>
    public partial class ObjectBlock : Moveable, IEquatable<ObjectBlock>
    {
    	public static readonly Size DefaultSize;    	
    	protected static DependencyProperty DisplayImageProperty;
    	protected static DependencyProperty BehaviourProperty;
    	
    	
    	static ObjectBlock()
    	{
    		DefaultSize = new Size(55,75);    		
    		DisplayImageProperty = DependencyProperty.Register("DisplayImage",typeof(Image),typeof(ObjectBlock));
    		BehaviourProperty = DependencyProperty.Register("Behaviour",typeof(ObjectBehaviour),typeof(ObjectBlock));
    	}
		
		
		/// <summary>
		/// Check whether this Flip component has all essential fields filled in,
		/// including those belonging to subcomponents, such that it can generate valid code.
		/// </summary>
		/// <returns>True if all essential fields have been given values; false otherwise.</returns>
		/// <remarks>Note that this method makes no attempt to judge whether the values
		/// are valid in their slots, only that those slots have been filled.</remarks>
		public override bool IsComplete { 
			get { return true; } 
		}		
    	
    	
    	public string Identifier {
    		get { return Behaviour.Identifier; }
    		set { 
    			if (Behaviour.Identifier != value) {
    				Behaviour.Identifier = value;
    				OnChanged(new EventArgs());
    			}
    		}
    	}
    	
    	
    	public string DisplayName {
    		get { return Behaviour.DisplayName; }
    		set { 
    			if (Behaviour.DisplayName != value) {
    				Behaviour.DisplayName = value;
    				OnChanged(new EventArgs());
    			}
    		}
    	}
    	
    	
    	public Image DisplayImage {
    		get { return (Image)base.GetValue(DisplayImageProperty); }
    		set {    	
    			if (value == null) {
    				value = FindResource("defaultimg") as Image;
    				if (value == null) value = new Image();
    			}    				
    			
    			value.Stretch = Stretch.Fill;
    			value.StretchDirection = StretchDirection.Both;
    			
    			base.SetValue(DisplayImageProperty,value);
    			OnChanged(new EventArgs());
    		}
    	}
    	
    	
    	public ObjectBehaviour Behaviour {
    		get { return (ObjectBehaviour)base.GetValue(BehaviourProperty); }
    		set { 
    			if (Behaviour != value) {
    				base.SetValue(BehaviourProperty,value);
    				OnChanged(new EventArgs());
    				try {
    					ToolTip = Behaviour.GetToolTip();
    				}
    				catch (Exception) {}
    			}
    		}
    	}
		
		
		public Brush Colour {
			get { return blockEdge.Background; }
			set {
				if (value == null) value = Brushes.LightGray;
				blockEdge.Background = value; 
			}
		}
		
		
		/// <summary>
		/// Parameterless constructor for deserialisation.
		/// </summary>
		public ObjectBlock() : this(null,new DefaultObjectBehaviour())
		{			
		}
    	
    	
        public ObjectBlock(Image image, ObjectBehaviour behaviour)
        {
        	if (behaviour == null) throw new ArgumentNullException("behaviour");
        	
        	Behaviour = behaviour;
        	
            InitializeComponent();

    		DisplayImage = image;
    		
    		Height = DefaultSize.Height;
    		Width = DefaultSize.Width;
        }
        
        
		public override Moveable DeepCopy()
		{
			Image img = new Image();
			img.Source = DisplayImage.Source;
			ObjectBlock copy = new ObjectBlock(img,Behaviour.DeepCopy());
			copy.blockEdge.Background = blockEdge.Background; // for Narrative Threads special brushes
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
			return String.Format("Block ({0})",Behaviour);
		}
		
    	
		public bool Equals(ObjectBlock other)
		{
			return other != null && other.Behaviour.Equals(this.Behaviour);
		}		
		
		
		public override void AssignImage(ImageProvider imageProvider)
		{
			if (imageProvider == null) throw new ArgumentNullException("imageProvider");
			
			imageProvider.AssignImage(this);
		}
		
			
		public override XmlSchema GetSchema()
		{
			return null;
		}
		
		
		public override void ReadXml(XmlReader reader)
		{
			reader.MoveToContent();			
			
			if (reader.IsEmptyElement) throw new FormatException("ObjectBlock does not specify a Behaviour, and could not be deserialised.");
			
			ReadCoordinates(reader);
			
			reader.ReadStartElement();
			reader.MoveToContent();
			
			if (reader.LocalName != "Behaviour") throw new FormatException("ObjectBlock does not specify a Behaviour, and could not be deserialised.");
			
			Behaviour = FlipWindow.ChosenDeserialisationHelper.GetObjectBehaviour(reader);//(ObjectBehaviour)SerialisationHelper.GetObjectFromXml(reader);
			
			reader.ReadEndElement();
		}
		
		
		public override void WriteXml(XmlWriter writer)
		{
			if (Behaviour == null) {
				throw new InvalidOperationException("The ObjectBlock being serialised has a null Behaviour property.");
			}
			
			WriteCoordinates(writer);
			
			writer.WriteStartElement("Behaviour");
			writer.WriteAttributeString("Type",Behaviour.GetType().FullName);
			Behaviour.WriteXml(writer);
			writer.WriteEndElement();
		}
		
		
		public override string GetLogText()
		{
			if (Behaviour == null) return String.Empty;
			else return Behaviour.GetLogText();
		}
        
        
		public override Statistics GetStatistics()
		{
			Statistics s = new Statistics();
			s.Add(this);
			return s;
		}
    }
}