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
    /// Interaction logic for Bag.xaml
    /// </summary>

    public partial class Bag : TabItem
    {
    	protected static ResourceDictionary resourceDictionary;
    	
    	
    	static Bag()
    	{
            resourceDictionary = new ResourceDictionary();
    		
    		// Set up common styles for Moveables:
            Style baseStyle = new Style(typeof(Moveable));
            
            baseStyle.Setters.Add(new Setter(Moveable.MarginProperty,new Thickness(2)));
            
            Type[] moveableTypes = new Type[]
            {
            	typeof(EventBlock),
            	typeof(ObjectBlock),
            	typeof(Statement),
            	typeof(IfControl)
            };           
            
            
            foreach (Type type in moveableTypes) {
            	Style style = new Style(type,baseStyle);
            	resourceDictionary.Add(style.TargetType,style);
            }
    	}
    	
    	
        public Bag(string name)
        {
            InitializeComponent();     
            this.Header = name;
            this.Resources = resourceDictionary;
        }

        
        public UIElementCollection Children {
        	get { return wrapPanel.Children; }
        }
    }
}