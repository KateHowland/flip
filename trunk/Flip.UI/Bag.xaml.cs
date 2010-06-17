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
    	protected WrapPanel wrapPanel;
    	protected StackPanel stackPanel;
    	
    	
    	static Bag()
    	{
            resourceDictionary = new ResourceDictionary();
    		
    		// Set up common styles for Moveables:
            Style baseStyle = new Style(typeof(Moveable));
            
            baseStyle.Setters.Add(new Setter(Moveable.MarginProperty,new Thickness(2)));
            
            Type[] moveableTypes = new Type[]
            {
            	typeof(ObjectBlock),
            	typeof(Statement),
            	typeof(IfControl),
            	typeof(IfElseControl),
            	typeof(DoWhileControl),
            	typeof(WhileControl),
            	typeof(NumberBlock),
            	typeof(StringBlock),
            	typeof(TriggerControl),
            	typeof(AndBlock),
            	typeof(OrBlock),
            	typeof(NotBlock)
            };           
            
            
            foreach (Type type in moveableTypes) {
            	Style style = new Style(type,baseStyle);
            	resourceDictionary.Add(style.TargetType,style);
            }
    	}
    	
    	
    	public Bag(string name) : this(name,false)
    	{    		
    	}
    	
    	
        public Bag(string name, bool wrap)
        {
            InitializeComponent();     
            this.Header = name;
            this.Resources = resourceDictionary;
            
            if (wrap) {
            	wrapPanel = new WrapPanel();
            	wrapPanel.AllowDrop = true;
            	wrapPanel.Background = Brushes.DarkSlateBlue;
            	
            	scroll.Content = wrapPanel;
            	
            	stackPanel = null;
            }
            
            else {
            	stackPanel = new StackPanel();
            	stackPanel.AllowDrop = true;
            	stackPanel.Background = Brushes.DarkSlateBlue;
            	
            	scroll.Content = stackPanel;
            	
            	wrapPanel = null;
            }
        }

        
        public UIElementCollection Children {
        	get { 
        		if (wrapPanel != null) return wrapPanel.Children;
        		else if (stackPanel != null) return stackPanel.Children;
        		else throw new InvalidOperationException("No panel.");
        	}
        }
        
        
        public void Add(Moveable moveable)
        {
        	if (moveable == null) throw new ArgumentNullException("moveable");     	
        	Children.Add(moveable);
        }
        
        
        public void Remove(Moveable moveable)
        {
        	if (moveable == null) throw new ArgumentNullException("moveable");     
        	if (Children.Contains(moveable)) Children.Remove(moveable);
        }
        
        
        public void Empty()
        {
        	Children.Clear();
        }
    }
}