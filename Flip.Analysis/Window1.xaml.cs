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
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;
using Microsoft.Win32;

namespace Sussex.Flip.Analysis
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		/// <summary>
		/// The task collection that is currently open.
		/// </summary>
		public LogLineCollection LogLines {
			get { return (LogLineCollection)DataContext; }
			set { 
				DataContext = value;				
//				if (value != null) {
//					value.CollectionChanged += UpdateFilters;
//				}
			}
		} 
		
		
		CollectionViewSource cvs;
		LogCombiner combiner;
		
		
		public Window1()
		{			
			// Access to the CollectionViewSource allows filters to be added/removed/refreshed:
			Loaded += delegate { cvs = (CollectionViewSource)Resources["logLineSource"]; };
			
			combiner = new LogCombiner();
		}
		
		
		public void Translate(object sender, RoutedEventArgs e)
		{
			OpenFileDialog dialog = new OpenFileDialog();
			bool? result = dialog.ShowDialog();
			
			// TODO options on dialog.
			
			if (result.HasValue && result.Value) {
				
				string file1 = dialog.FileName;
				
				result = dialog.ShowDialog();
								
				if (result.HasValue && result.Value) {
					
					string file2 = dialog.FileName;
					
					string log = combiner.Combine(file1,file2);
					
					LogLines = new LogLineCollection(log);
				}
			}
		}
		
		
		/// <summary>
		/// Apply a filter that will hide any tasks not containing 
		/// a user-entered search string.
		/// </summary>
		protected void SearchActivated(object sender, RoutedEventArgs e)
		{
			cvs.Filter -= new FilterEventHandler(SearchFilter);
			cvs.Filter += new FilterEventHandler(SearchFilter);
		}
				
		
		/// <summary>
		/// Stop filtering by search string.
		/// </summary>
		protected void SearchDeactivated(object sender, RoutedEventArgs e)
		{
			cvs.Filter -= new FilterEventHandler(SearchFilter);
		}
				
		
		/// <summary>
		/// Respond to the search string changing.
		/// </summary>
		protected void SearchStringChanged(object sender, RoutedEventArgs e)
		{
			if (FilteringBySearchString()) cvs.View.Refresh();			
		}
		
		
		protected void SearchFilter(object sender, FilterEventArgs e)
		{			
			if (FilteringBySearchString() && searchStringBox.Text.Length > 0) {
				
				string logLine = (string)e.Item;
				
				if (!logLine.Contains(searchStringBox.Text)) {
					
					e.Accepted = false;
					
					// Never set e.Accepted to true, or you may override the results of another filter.
				}
			}
		}
		
		
		protected bool FilteringBySearchString()
		{
			return searchFilterCheckBox.IsChecked.HasValue && searchFilterCheckBox.IsChecked.Value;
		}
	}
}