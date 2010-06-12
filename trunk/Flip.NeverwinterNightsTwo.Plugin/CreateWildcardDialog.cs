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
using Sussex.Flip.UI;

namespace Sussex.Flip.Games.NeverwinterNightsTwo
{
    /// <summary>
    /// Interaction logic for CreateWildcardDialog.xaml
    /// </summary>

    public partial class CreateWildcardDialog : Window
    {
    	protected string wildcardTag;
    	
    	
		public string WildcardTag {
			get { return wildcardTag; }
		}
    	
    	    	
    	public CreateWildcardDialog()
    	{
    		this.wildcardTag = null;
    		
    		InitializeComponent();
    		
    		valueTextBox.MaxLength = 40;
    		
            valueTextBox.KeyDown += new KeyEventHandler(HitEnterKey);
    	}
    	
    	
    	protected void HitEnterKey(object sender, KeyEventArgs e)
    	{
    		if (e.Key == Key.Enter) CreateBlock(sender,e);
    	}
    	
    	
    	protected void CreateBlock(object sender, RoutedEventArgs e)
    	{    		
    		if (valueTextBox.Text.Length == 0) {
    			MessageBox.Show("Your tag can't be blank.");
    		}
    		
    		else if (ContainsInvalidCharacters(valueTextBox.Text)) {
    			MessageBox.Show("Your tag can only contain letters (a-z), numbers (0-9) and underscores (_).");    			
    		}
    		
    		else {
	    		wildcardTag = valueTextBox.Text;
	    		Close();
    		}
    	}
    	
    	
    	/// <summary>
    	/// 
    	/// </summary>
    	/// <param name="str"></param>
    	/// <returns>True if the string contains only numbers, letters or the underscore; false otherwise.</returns>
    	public static bool ContainsInvalidCharacters(string str)
    	{
    		if (str == null) throw new ArgumentNullException("str");
    		
    		char letter;
    		for (int i = 0; i < str.Length; i++) {
    			letter = str[i];
    			int ascii = Convert.ToInt32(letter);
    			
                if (!(ascii >= 48 && ascii <= 57) &&
                    !(ascii >= 65 && ascii <= 90) &&
                    !(ascii >= 97 && ascii <= 122) &&
                    !(ascii == 95))
                {
                    return true;
                }
    		}
    		
    		return false;
    	}
    	
    	
    	protected void CancelCreation(object sender, RoutedEventArgs e)
    	{
    		wildcardTag = null;
    		Close();
    	}    	
    }
}