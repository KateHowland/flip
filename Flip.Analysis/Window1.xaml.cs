/*
 * Created by SharpDevelop.
 * User: kn70
 * Date: 01/09/2010
 * Time: 16:00
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;

namespace Sussex.Flip.Analysis
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		/*
		 * Application for analysing Flip scripts and logs.
		 * 
		 * Some data to capture and present for each user:
		 * - how many scripts they ended up with
		 * - total time spent with Flip open
		 * - total number of actions taken
		 * - total number of illegal saves
		 * - a timeline-based walkthrough of their Flip actions
		 * 
		 * ...and for each script:
		 * - the number of times it was edited
		 * - the number of illegal saves
		 *  
		 * ...and for each version of the script:		 
		 * - the natural language representation		 
		 * - the number of lines in the script
		 * - which types of block were used and how many
		 */
		
		public Window1()
		{
		}
	}
}