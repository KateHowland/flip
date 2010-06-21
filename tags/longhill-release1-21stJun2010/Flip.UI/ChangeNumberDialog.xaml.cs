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
using System.Windows.Shapes;
using Sussex.Flip.Utils;

namespace Sussex.Flip.UI
{
    /// <summary>
    /// Interaction logic for ChangeNumberDialog.xaml
    /// </summary>

    public partial class ChangeNumberDialog : Window
    {
    	protected NumberBlock block;
    	protected Int32 min, max;
    	
    	
    	public ChangeNumberDialog(NumberBlock block) : this(block,Int32.MinValue,Int32.MaxValue)
    	{    		
    	}
    	
    	
    	public ChangeNumberDialog(NumberBlock block, Int32 min, Int32 max)
    	{
    		if (block == null) throw new ArgumentNullException("block");
    		if (min > max) throw new ArgumentException("Min must not be greater than max.","max");
    		
    		this.block = block;
    		this.min = min;
    		this.max = max;
    		
    		InitializeComponent();
    		
    		valueTextBox.MaxLength = Math.Max(min.ToString().Length,max.ToString().Length);
    		
    		valueTextBox.Text = block.Value.ToString();
    		
            valueTextBox.KeyDown += new KeyEventHandler(HitEnterKey);
    	}
    	
    	
    	protected void HitEnterKey(object sender, KeyEventArgs e)
    	{
    		if (e.Key == Key.Enter) AcceptChange(sender,e);
    	}
    	
    	
    	protected void AcceptChange(object sender, RoutedEventArgs e)
    	{
    		Int32 newValue;
    		
    		if (Int32.TryParse(valueTextBox.Text, out newValue)) {
    			
    			if (newValue < min || newValue > max) {
    				MessageBox.Show(String.Format("Number must be between {0} and {1}.",min,max));
    			}
    			else {
    				string oldValue = block.Value.ToString();
	    			block.Value = newValue;
	    			ActivityLog.Write(new Activity("ChangedValueOfNumberBlock","OldValue",oldValue,"NewValue",block.Value.ToString()));
    				Close();
    			}
    		}
    		else {
    			MessageBox.Show(String.Format("{0} is not a valid number.",valueTextBox.Text));
    		}
    	}
    	
    	
    	protected void CancelChange(object sender, RoutedEventArgs e)
    	{
    		Close();
    	}    	
    }
}