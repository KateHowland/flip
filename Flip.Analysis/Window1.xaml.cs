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
		LogReader logReader;
		string filePath;
		
		public string FilePath {
			get { return filePath; }
			set { 
				filePath = value; 
				UpdateFileList(filePath);
			}
		}
		
		
		public Window1()
		{			
			// Access to the CollectionViewSource allows filters to be added/removed/refreshed:
			Loaded += delegate 
			{ 
				cvs = (CollectionViewSource)Resources["logLineSource"];
				
				logFileListView.SelectionChanged += DisplaySelectedLogs;
			};
			
			logReader = new LogReader();
		}

		
		protected void DisplaySelectedLogs(object sender, SelectionChangedEventArgs e)
		{
			List<string> logs = new List<string>(logFileListView.SelectedItems.Count);
			
			foreach (object o in logFileListView.SelectedItems) {
				string path = (string)o;
				string log = logReader.GetFileContents(path);
				logs.Add(log);
			}
			
			string combined = logReader.GetCombinedLog(logs);
			
			DisplayLog(combined);
		}
		

		protected void UpdateFileList(string path)
		{
			try {
				pathTextBlock.Text = path;
				string[] files = Directory.GetFiles(path);
				logFileListView.ItemsSource = files;
			}
			catch (Exception x) {
				MessageBox.Show("Tried to populate list of log files, but directory was invalid.\n\n" + x);
			}
		}
		
		
		public void DisplayLog(string log)
		{
			LogLines = new LogLineCollection(log);
		}
		
		
		public void SelectFolder(object sender, RoutedEventArgs e)
		{
			System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
			
			dialog.ShowNewFolderButton = false;
			
			if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
				FilePath = dialog.SelectedPath;
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