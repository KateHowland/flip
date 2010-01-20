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
		}
		
		
		private int count = 1;
		private void AddSimpleBlock(object sender, RoutedEventArgs e)
		{
			SimpleBlock block = new SimpleBlock(new LinearGradientBrush(Colors.Pink,Colors.Salmon,45));
			block.Face.Content = "Block " + count++;
			SelectionBox.Children.Add(block);
		}
		
		
		private void AddBlockWithSlots(object sender, RoutedEventArgs e)
		{
			BlockWithSlots block = new BlockWithSlots();
			
			block.AddSlot("CreatureX");
			
			Label label = new Label();
			label.Content = "attacks";
			label.VerticalAlignment = VerticalAlignment.Center;
			label.HorizontalAlignment = HorizontalAlignment.Center;
			block.AddLabel(label);
			
			block.AddSlot("CreatureY");
			
			SelectionBox.Children.Add(block);
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