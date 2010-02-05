﻿using System;
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

namespace Sussex.Flip.UI
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		public Window1()
		{
			InitializeComponent();			
			MainCanvas.PreviewDrop += new DragEventHandler(DroppedOnCanvas);
			SelectionBox.PreviewDrop += new DragEventHandler(DroppedOnSelectionBox);
						
			foreach (Statement statement in new StatementFactory().GetStatements()) {
				SelectionBox.Children.Add(statement);
			}
			
			string dir = @"C:\Flip\object pics";
			try {
				NounProvider provider = new SampleNounProvider(dir);
			
				foreach (Noun noun in provider.GetNouns()) {
					SelectionBox.Children.Add(noun);
				}	
			}
			catch (System.IO.DirectoryNotFoundException) {
				//MessageBox.Show("No directory found at " + dir);
			}
		}
				

		private void DroppedOnCanvas(object sender, DragEventArgs e)
		{
			IDataObject data = e.Data;
			if (data.GetDataPresent(typeof(Moveable))) {
				Moveable moveable = (Moveable)data.GetData(typeof(Moveable));
				
				Point point = e.GetPosition(MainCanvas);
				double x = point.X - (moveable.ActualWidth/2);
				double y = point.Y - (moveable.ActualHeight/2);
				
				if (!MainCanvas.Children.Contains(moveable)) {
					moveable.Detach();
					MainCanvas.Children.Add(moveable);
				}					
				
				moveable.MoveTo(x,y);
			}
		}
				

		private void DroppedOnSelectionBox(object sender, DragEventArgs e)
		{
			IDataObject data = e.Data;
			if (data.GetDataPresent(typeof(Moveable))) {
				Moveable moveable = (Moveable)data.GetData(typeof(Moveable));
								
				if (!SelectionBox.Children.Contains(moveable)) {
					moveable.Detach();
					SelectionBox.Children.Add(moveable);
				}		
			}
		}
	}
}