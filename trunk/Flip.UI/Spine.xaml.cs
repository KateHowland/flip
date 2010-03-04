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
    /// Interaction logic for Spine.xaml
    /// </summary>

    public partial class Spine : UserControl
    {
    	public Spine(int pegs, double extent) : this(pegs)
    	{
    		Extends = extent;
    	}
    	
    	
        public Spine(int pegs)
        {
            InitializeComponent();
            for (int i = 0; i < pegs; i++) AddPeg();
        }
        
        
        public Spine() : this(5)
        {        	
        }

        
        public Peg AddPeg()
        {
        	Peg peg = new Peg();
        	Pegs.Add(peg);
        	return peg;
        }

        
        public Peg AddPeg(int index)
        {
        	Peg peg = new Peg();
        	Pegs.Insert(index,peg);
        	return peg;
        }
        
        
        public void RemovePeg()
        {
        	int count = Pegs.Count;
        	if (count > 0) Pegs.Remove(Pegs[count-1]);
        }
        
        
        public void RemovePeg(int index)
        {
        	int count = Pegs.Count;
        	if (count - 1 > index) Pegs.Remove(Pegs[count-1]);
        }
        
        
        public double Extends {
        	get { return extension.Height.Value; }
        	set { extension.Height = new GridLength(value); }
        }
        
        
        public UIElementCollection Pegs {
        	get { return pegsPanel.Children; }
        }
    }
}