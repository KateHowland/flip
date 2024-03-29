﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Sussex.Flip.UI
{
    /// <summary>
    /// Interaction logic for TriggerBar.xaml
    /// </summary>

    public partial class TriggerBar : UserControl, ITranslatable, IScriptFrame
    {
    	protected string currentScriptIsBasedOn;
    	protected TriggerSlot triggerSlot;
    	protected Spine spine;
    	protected Button saveButton;
    	
    	
		public TriggerControl TriggerControl {
			get { return triggerSlot.Contents as TriggerControl; }
			set { 
				if (triggerSlot.Contents != value) {
					triggerSlot.Contents = value;
					OnChanged(new EventArgs());
				}
			}
		}
    	
    	
		public Spine Spine {
			get { return spine; }
		}
    	
    	
    	public Button SaveButton {
    		get { return saveButton; }
    	}
    	
    	
		public string CurrentScriptIsBasedOn {
			get { return currentScriptIsBasedOn; }
			set { currentScriptIsBasedOn = value; }
		}
    	
    	
		public event EventHandler Changed;
		
		
		protected virtual void OnChanged(EventArgs e)
		{
			EventHandler handler = Changed;
			if (handler != null) {
				handler(this,e);
			}
		}
		
		
		/// <summary>
		/// Check whether this Flip component has all essential fields filled in,
		/// including those belonging to subcomponents, such that it can generate valid code.
		/// </summary>
		/// <returns>True if all essential fields have been given values; false otherwise.</returns>
		/// <remarks>Note that this method makes no attempt to judge whether the values
		/// are valid in their slots, only that those slots have been filled.</remarks>
		public bool IsComplete { 
			get { 
				return triggerSlot.IsComplete && spine.IsComplete;
			}
		}	
		
		
		public TriggerBar(Fitter fitter)
        {
        	if (fitter == null) throw new ArgumentNullException("fitter");
        	
        	currentScriptIsBasedOn = String.Empty;
        	
        	spine = new Spine(fitter,3);
        	Grid.SetRow(spine,0);
        	Grid.SetColumn(spine,0);
        	spine.Margin = new Thickness(14,0,0,0);
        	
        	triggerSlot = new TriggerSlot(new TriggerFitter());
        	triggerSlot.AllowDrop = true;  
        	triggerSlot.Margin = new Thickness(100,0,0,0);
        	
            InitializeComponent();      	
        	
            triggerBarPanel.Children.Add(triggerSlot);
            
            saveButton = new BigButton("Save");
			saveButton.Margin = new Thickness(35,5,5,5);
            
            triggerBarPanel.Children.Add(saveButton);
            
            spine.Extends = border.Height + 20;     
        	Grid.SetZIndex(spine,1);
        	Grid.SetZIndex(border,2);            
            mainGrid.Children.Add(spine);
            
            Effect = new DropShadowEffect();
            
        	triggerSlot.Changed += delegate { OnChanged(new EventArgs()); };
        	spine.Changed += delegate { OnChanged(new EventArgs()); };
        }
        
        
        public string GetAddress()
        {
        	return triggerSlot.GetAddress();
        }
    	
        
		public string GetNaturalLanguage()
		{
			string triggerNL = triggerSlot.GetNaturalLanguage();			
			string spineNL = spine.GetNaturalLanguage();
			
			if (triggerNL == "some event" && spineNL == "nothing happens") return String.Empty;
			
			else {
				System.Text.StringBuilder code = new System.Text.StringBuilder();
				
				if (triggerNL != "some event") code.AppendLine(triggerNL+":");
				
				if (spineNL != String.Empty) {
					try {
						// Capitalise:
						string firstCharacter = spineNL.Substring(0,1).ToUpper();
						if (spineNL.Length == 1) spineNL = firstCharacter;
						else spineNL = firstCharacter + spineNL.Substring(1,spineNL.Length-1);
					}
					catch (Exception) {
						spineNL = spine.GetNaturalLanguage();
					}
						
					code.Append(spineNL+".");
				}
				
				return code.ToString();
			}
		}
		
		
		public string GetCode()
		{
			return String.Empty;
		}
		
		
		public void Clear()
		{
			TriggerControl = null;
			Spine.Clear();
		}
		
    	
		public XmlSchema GetSchema()
		{
			return null;
		}
		
		
		public string GetLogText()
		{
			return "Script";
		}
		
		
		public void ReadXml(XmlReader reader)
		{
			reader.MoveToContent();
			
			if (reader.IsEmptyElement) throw new FormatException("Script is empty.");
			
			reader.ReadStartElement();
			CurrentScriptIsBasedOn = reader.GetAttribute("BasedOn");
			reader.MoveToContent();
			
			if (reader.LocalName != "Code") throw new FormatException("Script does not include Code.");
			
			Spine.ReadXml(reader);
			
			reader.MoveToElement();			
			reader.ReadEndElement();
			reader.MoveToElement();
		}
		
    	
		public void WriteXml(XmlWriter writer)
		{
			writer.WriteAttributeString("BasedOn",CurrentScriptIsBasedOn);
			writer.WriteStartElement("Code");
			Spine.WriteXml(writer);
			writer.WriteEndElement();
		}
		
		
		public void AssignImage(ImageProvider imageProvider)
		{
			if (imageProvider == null) throw new ArgumentNullException("imageProvider");
			
			Spine.AssignImage(imageProvider);
		}
        
        
		public ScriptStats GetStatistics()
		{
			ScriptStats s = Spine.GetStatistics();
			s.Add(triggerSlot.GetStatistics());
			return s;
		}
    }
}