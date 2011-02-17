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

    public partial class ChangeStringDialog : Window
    {
    	protected StringBlock block;
    	protected Int32 maxLength;
    	
    	
    	public ChangeStringDialog(StringBlock block) : this(block,140)
    	{    		
    	}
    	
    	
    	public ChangeStringDialog(StringBlock block, Int32 maxLength)
    	{
    		if (block == null) throw new ArgumentNullException("block");
    		
    		this.block = block;
    		this.maxLength = maxLength;
    		
    		InitializeComponent();
    		
    		valueTextBox.MaxLength = maxLength;
    		
    		valueTextBox.Text = block.Value;
    		
            valueTextBox.KeyDown += new KeyEventHandler(HitEnterKey);
    	}
    	
    	
    	protected void HitEnterKey(object sender, KeyEventArgs e)
    	{
    		if (e.Key == Key.Enter) AcceptChange(sender,e);
    	}
    	
    	
    	protected void AcceptChange(object sender, RoutedEventArgs e)
    	{
    		if (ContainsInvalidCharacters(valueTextBox.Text)) {
    			MessageBox.Show("You cannot use double quotation marks (\").");    			
    		}
    		
    		else {
    			string oldValue = block.Value;
	    		block.Value = valueTextBox.Text;
				//ActivityLog.Write(new Activity("ChangedValueOfStringBlock","OldValue",oldValue,"NewValue",block.Value));
				Log.WriteAction(LogAction.edited,"stringblock","changed value from " + oldValue + " to " + block.Value);
	    		Close();
    		}
    	}
    	
    	
    	public static bool ContainsInvalidCharacters(string str)
    	{
    		return str.Contains("\"");
    	}
    	
    	
    	protected void CancelChange(object sender, RoutedEventArgs e)
    	{
    		Close();
    	}    	
    }
}