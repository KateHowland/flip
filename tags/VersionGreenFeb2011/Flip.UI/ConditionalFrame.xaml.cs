/*
 * Flip - a visual programming language for scripting video games
 * Copyright (C) 2009, 2010 University of Sussex
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 *
 * To contact the authors of this program, email flip@sussex.ac.uk.
 *
 * You can also write to Keiron Nicholson at the School of Informatics, 
 * University of Sussex, Sussex House, Brighton, BN1 9RH, United Kingdom.
 * 
 * This file added by Keiron Nicholson on 07/12/2010 at 10:26.
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
using System.Xml.Schema;
using System.Xml.Serialization;
using Sussex.Flip.Utils;

namespace Sussex.Flip.UI
{
	/// <summary>
	/// Interaction logic for ConditionalFrame.xaml
	/// </summary>
	public partial class ConditionalFrame : UserControl, ITranslatable, IScriptFrame
	{
    	protected ConditionSlot slot;
    	protected string currentScriptIsBasedOn;
    	protected string address;
    	protected Button saveButton;//, finishButton;
		
       
		public bool IsComplete {
			get { return slot.IsComplete; }
		}
    	
    	
    	public Button SaveButton {
    		get { return saveButton; }
    	}
    	
    	
//    	public Button FinishButton {
//    		get { return finishButton; }
//    	}
    	
    	
		public ConditionSlot Slot {
			get { return slot; }
		}
    	
    	
		public string Address {
			get { return address; }
			set { address = value; }
		}
    	
    	
		public string CurrentScriptIsBasedOn {
			get { return currentScriptIsBasedOn; }
			set { currentScriptIsBasedOn = value; }
		}
    	
    	
		public string Dialogue {
			get { return dialogueTextBlock.Text; }
			set { 
				if (value == null) value = "...";		
				
				if (value.Length > 150) value = StringUtils.Truncate(value,150) + "...";
				
				if (value.Length < 100) dialogueTextBlock.FontSize = 22.0d;
				else if (value.Length < 50) dialogueTextBlock.FontSize = 24.0d;
				else if (value.Length < 30) dialogueTextBlock.FontSize = 28.0d;
				else dialogueTextBlock.FontSize = 20.0d;
				
				dialogueTextBlock.Text = String.Format("\"{0}\"",value);
			}
		}
    	
    	
		public event EventHandler Changed;
		
		
		protected virtual void OnChanged(EventArgs e)
		{
			EventHandler handler = Changed;
			if (handler != null) {
				handler(this,e);
			}
		}
		
    	
        public ConditionalFrame()
        {        	
        	currentScriptIsBasedOn = String.Empty;
        	address = String.Empty;
        	
            InitializeComponent();
            
            saveButton = new BigButton("Save");
            //finishButton = new BigButton("Finish");
			saveButton.Margin = new Thickness(0,15,15,15);
			//finishButton.Margin = new Thickness(0,15,0,15);
            buttonsPanel.Children.Add(saveButton);
            //buttonsPanel.Children.Add(finishButton);
            
            BooleanExpressionFitter fitter = new BooleanExpressionFitter();
            
            slot = new ConditionSlot(fitter);
            slot.AllowDrop = true;
            slot.Margin = new Thickness(10,10,10,0);
            slot.MinWidth = 550;
            slot.Height = 100;
            
            dragMessageTextBlock.AllowDrop = true;
            dragMessageTextBlock.IsHitTestVisible = false;
            
            slot.Changed += delegate(object sender, EventArgs e) 
            { 
        		if (slot.Contents == null) dragMessageTextBlock.Visibility = Visibility.Visible;        		
        		else dragMessageTextBlock.Visibility = Visibility.Hidden;
            	
            	OnChanged(e);
            };
            
            Grid.SetRow(slot,2);
            mainGrid.Children.Add(slot);
        }
    	
        
		public string GetNaturalLanguage()
		{
			string slotNL = slot.GetNaturalLanguage();
			if (slotNL == "some condition") return "Only say this line if...";			
			else return String.Format("Only say this line if {0}.",slotNL);
		}
		
		
		public string GetCode()
		{
			return String.Empty;
		}
        
        
        public string GetAddress()
        {
        	return address;
        }
		
		
		public void Clear()
		{
			if (slot.Contents != null) slot.Contents = null;
		}
		
    	
		public XmlSchema GetSchema()
		{
			return null;
		}
		
		
		public string GetLogText()
		{
			return "ConditionalScript";
		}
		
		
		public void ReadXml(XmlReader reader)
		{
			reader.MoveToContent();
			
			if (reader.IsEmptyElement) throw new FormatException("Script is empty.");
			
			reader.ReadStartElement();
			CurrentScriptIsBasedOn = reader.GetAttribute("BasedOn");
			reader.MoveToContent();
			
			if (reader.LocalName != "Code") throw new FormatException("Script does not include Code.");
			
			slot.ReadXml(reader);
			
			reader.MoveToElement();			
			reader.ReadEndElement();
			reader.MoveToElement();
		}
		
    	
		public void WriteXml(XmlWriter writer)
		{
			writer.WriteAttributeString("BasedOn",CurrentScriptIsBasedOn);
			writer.WriteStartElement("Code");
			slot.WriteXml(writer);
			writer.WriteEndElement();
		}
		
		
		public void AssignImage(ImageProvider imageProvider)
		{
			if (imageProvider == null) throw new ArgumentNullException("imageProvider");
			
			if (slot.Contents != null) slot.Contents.AssignImage(imageProvider);
		}
        
        
		public ScriptStats GetStatistics()
		{
			ScriptStats s = new ScriptStats();
			s.Add(this);
			if (slot.Contents != null) {
				s.Line = 1;
				s.Add(slot.GetStatistics());
			}
			return s;
		}
	}
}