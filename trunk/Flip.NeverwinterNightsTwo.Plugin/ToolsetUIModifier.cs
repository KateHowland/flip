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
 * This file added by Keiron Nicholson on 15/01/2010 at 14:09.
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows;
using NWN2Toolset;
using NWN2Toolset.NWN2.Data;
using NWN2Toolset.NWN2.Data.Blueprints;
using NWN2Toolset.NWN2.Data.ConversationData;
using NWN2Toolset.NWN2.Data.TypedCollections;
using NWN2Toolset.NWN2.Views;
using Crownwood.DotNetMagic.Common;
using Crownwood.DotNetMagic.Controls;
using Crownwood.DotNetMagic.Docking;
using GlacialComponents.Controls.GlacialTreeList;
using OEIShared.Utils;
using VisualHint.SmartPropertyGrid;
using TD.SandBar;
using winforms = System.Windows.Forms;

namespace Sussex.Flip.Games.NeverwinterNightsTwo
{
	/// <summary>
	/// Modifies aspects of the Neverwinter Nights 2 toolset user
	/// interface, to make it more suitable for working with Flip.
	/// </summary>
	public class ToolsetUIModifier
	{				
		#region Fields
				
		/// <summary>
		/// Controls access to toolset panels.
		/// </summary>
		protected DockingManager dockingManager = null;
		
		/// <summary>
		/// The private inner grid of the main property grid on the toolset interface.
		/// </summary>
		/// <remarks>Narrative Threads disposes of the main property grid, so always
		/// check that this control is not
		/// null or disposed before using it.</remarks>
		protected PropertyGrid mainPropertyInnerGrid = null;
		
		/// <summary>
		/// The user interface panels which allow users to access scripts.
		/// </summary>
		protected List<NWN2ModuleScriptList> scriptPanels = new List<NWN2ModuleScriptList>(2);
		
		/// <summary>
		/// The menu items which allow users to access scripts.
		/// </summary>
		protected List<TD.SandBar.MenuButtonItem> scriptMenuItems = new List<TD.SandBar.MenuButtonItem>(2);
		
		/// <summary>
		/// Reflected field information for the private
		/// property grid on class NWN2PropertyGrid.
		/// </summary>
		protected FieldInfo innerPropertyGridFieldInfo = null;
		
		/// <summary>
		/// The event handler for the main property grid.
		/// </summary>
		protected PropertyGrid.SelectedObjectChangedEventHandler mainGridHandler;
		
		/// <summary>
		/// The event handler for newly-created floating property grids.
		/// </summary>
		protected DockingManager.ContentHandler floatingGridHandler;
		
		/// <summary>
		/// True if UI elements relating to working with scripts (other than
		/// Flip) are currently visible and accessible; false otherwise.
		/// </summary>
		protected bool allowScriptAccess = true;
		
		/// <summary>
		/// The delegate signature of a method to be called when a particular
		/// line of conversation is to have a script attached.
		/// </summary>
		/// <param name="line">The line of dialogue to attach a script to.</param>
		/// <param name="conversation">The conversation this line of dialogue is part of.</param>
		public delegate void AddScriptToConversation(NWN2ConversationConnector line, NWN2GameConversation conversation);
				
		/// <summary>
		/// The delegate signature of a method to be called when a blueprint 
		/// (or blueprint collection) is to be used to create an instance 
		/// block (or blocks) in Flip.
		/// </summary>
		/// <param name="blueprints">The blueprints collection to create blocks from.</param>
		public delegate void CreateBlockFromBlueprintDelegate(NWN2BlueprintCollection blueprints);
		
		/// <summary>
		/// The delegate signature of a method to be called when the tag
		/// of an object changes, demanding an update to its block.
		/// </summary>
		public delegate void UpdateBlockWhenTagChangesDelegate(NWN2PropertyValueChangedEventArgs e);
		
		/// <summary>
		/// The method to call when a line of conversation is to have a script added.
		/// </summary>
		protected AddScriptToConversation addScriptToLine = null;
		
		/// <summary>
		/// The method to call when a line of conversation is to have a condition added.
		/// </summary>
		protected AddScriptToConversation addConditionToLine = null;
		
		/// <summary>
		/// The method to call when a blueprint is to be used to create an instance 
		/// block in Flip.
		/// </summary>
		protected CreateBlockFromBlueprintDelegate createBlockFromBlueprintDelegate = null;
		
		/// <summary>
		/// The method to call when the tag of an object changes.
		/// </summary>
		protected UpdateBlockWhenTagChangesDelegate updateBlockDelegate = null;
		
		/// <summary>
		/// TODO
		/// </summary>
		protected ToolBar objectsToolbar = null;
		
		protected NWN2BlueprintView blueprintView = null;
		
		#endregion
		
		#region Properties
		
		/// <summary>
		/// True if UI elements relating to working with scripts (other than
		/// Flip) are currently visible and accessible; false otherwise.
		/// </summary>
		/// <remarks>Note that open script viewers are not affected - clients
		/// should consider ensuring that all script viewers are closed before setting
		/// this property to False.</remarks>
		public bool AllowScriptAccess {
			get { return allowScriptAccess; }
			set {
				if (allowScriptAccess != value) {
					ControlAccessToScripts(value);
					allowScriptAccess = value;
				}
			}
		}
		
		#endregion
		
		#region Constructors
		
		/// <summary>
		/// Constructs a new <see cref="ToolsetUIModifier"/> instance.
		/// </summary>
		/// <param name="addScriptToLine">A method which will be invoked when 
		/// the user tries to add a script to a line of dialogue.</param>
		/// <param name="AddScriptToConversation">A method which will be invoked when 
		/// the user tries to add a condition to a line of dialogue.</param>
		/// <param name="createBlockFromBlueprintDelegate">A method which will be invoked when 
		/// a blueprint is to be used to create an instance block in Flip.</param>
		/// <param name="updateBlockDelegate">A method which will be invoked when 
		/// a blueprint is to be used to create an instance block in Flip.</param>
		public ToolsetUIModifier(AddScriptToConversation addScriptToLine, 
		                         AddScriptToConversation addConditionToLine,
		                         CreateBlockFromBlueprintDelegate createBlockFromBlueprintDelegate, 
		                         UpdateBlockWhenTagChangesDelegate updateBlockDelegate)
		{			
			if (addScriptToLine == null) throw new ArgumentNullException("addScriptToLine");
			if (addConditionToLine == null) throw new ArgumentNullException("addConditionToLine");
			if (createBlockFromBlueprintDelegate == null) throw new ArgumentNullException("createBlockFromBlueprintDelegate");
			if (updateBlockDelegate == null) throw new ArgumentNullException("updateBlockDelegate");
			
			this.addScriptToLine = addScriptToLine;
			this.addConditionToLine = addConditionToLine;
			this.createBlockFromBlueprintDelegate = createBlockFromBlueprintDelegate;
			this.updateBlockDelegate = updateBlockDelegate;
			
			FindFields();
			
			floatingGridHandler = new DockingManager.ContentHandler(HideScriptSlotsOnFloatingGrid);
			mainGridHandler = new PropertyGrid.SelectedObjectChangedEventHandler(HideScriptSlotsOnMainGrid);
		}
		
		#endregion
		
		#region Methods
		
		/// <summary>
		/// Makes changes to the standard Neverwinter Nights 2
		/// toolset user interface. Should only be called 
		/// when the toolset is first loaded.
		/// </summary>
		public void ModifyUI()
		{
			FieldInfo[] fields = typeof(NWN2ToolsetMainForm).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
			foreach (FieldInfo field in fields) {	
				/*
				 * Track when resource viewers are opening or closing.
				 */
				if (field.FieldType == typeof(TabbedGroups)) {
					TabbedGroups tg = (TabbedGroups)field.GetValue(NWN2ToolsetMainForm.App);					
					CollectionChange opened = new CollectionChange(ViewerOpened);					
					tg.ActiveLeaf.TabPages.Inserted += opened;
					
					// ActiveLeaf is disposed whenever all viewers are closed, so attach handlers again:
					tg.ActiveLeafChanged += delegate
					{  
						tg.ActiveLeaf.TabPages.Inserted += opened;
					};
				}
			}
			
			WatchForPropertyGridChanges();
		}
		
		
		protected void WatchForPropertyGridChanges()
		{
			dockingManager.ContentShown += delegate(Content c, EventArgs cea) 
			{  
				if (c.Control is NWN2PropertyGrid) {
					Watch((NWN2PropertyGrid)c.Control);
				}
			};
		}
		
		
		protected void Watch(NWN2PropertyGrid grid)
		{
			if (grid == null) throw new ArgumentNullException("grid");
			grid.ValueChanged += delegate(object sender, NWN2PropertyValueChangedEventArgs args) 
			{  
				try {
					updateBlockDelegate.Invoke(args);
				}
				catch (Exception x) {
					MessageBox.Show("Something went wrong when updating a block.\n\n" + x);
				}
			};
		}
		
		
		/// <summary>
		/// 
		/// </summary>
		public TD.SandBar.ButtonItem AddFlipButton()
		{
			if (objectsToolbar == null) return null;
			
			ButtonItem flipButton = new ButtonItem();
			flipButton.Text = "Flip";
			flipButton.BeginGroup = true;
			
			try {
				Stream s = Assembly.GetAssembly(typeof(Sussex.Flip.UI.AboutWindow)).GetManifestResourceStream("fliplogoicon");
				flipButton.Icon = new Icon(s);
			}
			catch (Exception) {}
			
			flipButton.ToolTipText = "Create and edit scripts";
			objectsToolbar.Items.Add(flipButton);
			return flipButton;
		}
		
		
		/// <summary>
		/// Notifies the client that a resource viewer has been opened.
		/// </summary>
		protected void ViewerOpened(int index, object value)
		{
			TabPage page = (TabPage)value;
			NWN2ConversationViewer viewer = page.Control as NWN2ConversationViewer;
			if (viewer == null) return;		
			
			try {
				GlacialTreeList tree = (GlacialTreeList)viewer.Controls["panelResults"].Controls["treeListResults"];
				if (tree == null) throw new ApplicationException("Couldn't find GlacialTreeList.");
								
				winforms.MenuItem addEditScript = new winforms.MenuItem("Add script");
				winforms.MenuItem deleteScript = new winforms.MenuItem("Delete script");
				winforms.MenuItem addEditCondition = new winforms.MenuItem("Add condition");
				winforms.MenuItem deleteCondition = new winforms.MenuItem("Delete condition");
				
				addEditScript.Click += delegate
				{
					if (tree.SelectedNodes.Count != 1) {
						MessageBox.Show("Select a single line of the conversation.");
					}
					
					else {
						try {
							GTLTreeNode node = (GTLTreeNode)tree.SelectedNodes[0];
													
							NWN2ConversationConnector connector = node.Tag as NWN2ConversationConnector;
							
							if (connector != null) addScriptToLine.Invoke(connector,viewer.Conversation);
						}
						catch (Exception x) {							
							MessageBox.Show("Something went wrong when adding/editing the script.\n\n" + x);
						}
					}
				};
				
				addEditCondition.Click += delegate
				{
					if (tree.SelectedNodes.Count != 1) {
						MessageBox.Show("Select a single line of the conversation.");
					}
					
					else {
						try {
							GTLTreeNode node = (GTLTreeNode)tree.SelectedNodes[0];
													
							NWN2ConversationConnector connector = node.Tag as NWN2ConversationConnector;
							
							if (connector != null) addConditionToLine.Invoke(connector,viewer.Conversation);
						}
						catch (Exception x) {							
							MessageBox.Show("Something went wrong when adding/editing the script.\n\n" + x);
						}
					}
				};
				
				deleteScript.Click += delegate
				{
					if (tree.SelectedNodes.Count != 1) {
						MessageBox.Show("Select a single line of the conversation.");
					}					
					
					else {
						try {
							GTLTreeNode node = (GTLTreeNode)tree.SelectedNodes[0];
													
							NWN2ConversationConnector connector = node.Tag as NWN2ConversationConnector;
							
							if (connector != null && connector.Actions.Count > 0) {
								        	
						 		MessageBoxResult result = MessageBox.Show("Delete the script on this line?","Delete?", MessageBoxButton.YesNo);
								if (result == MessageBoxResult.Yes) {
									connector.Actions.Clear();
									viewer.RefreshTreeItemForConnector(connector);		
								}									
							}							
						}
						catch (Exception x) {							
							MessageBox.Show("Something went wrong when deleting the script.\n\n" + x);
						}
					}
				};
				
				deleteCondition.Click += delegate
				{
					if (tree.SelectedNodes.Count != 1) {
						MessageBox.Show("Select a single line of the conversation.");
					}					
					
					else {
						try {
							GTLTreeNode node = (GTLTreeNode)tree.SelectedNodes[0];
													
							NWN2ConversationConnector connector = node.Tag as NWN2ConversationConnector;
							
							if (connector != null && connector.Conditions.Count > 0) {
								        	
						 		MessageBoxResult result = MessageBox.Show("Delete the condition on this line?","Delete?", MessageBoxButton.YesNo);
								if (result == MessageBoxResult.Yes) {
									connector.Conditions.Clear();
									viewer.RefreshTreeItemForConnector(connector);		
								}									
							}							
						}
						catch (Exception x) {							
							MessageBox.Show("Something went wrong when deleting the condition.\n\n" + x);
						}
					}
				};
				
				tree.ContextMenu.Popup += delegate
				{  
					if (tree.SelectedNodes.Count != 1) {
						addEditScript.Enabled = false;
						addEditCondition.Enabled = false;
						deleteScript.Enabled = false;
						deleteCondition.Enabled = false;
					}
					
					else {
						GTLTreeNode node = (GTLTreeNode)tree.SelectedNodes[0];
												
						NWN2ConversationConnector connector = node.Tag as NWN2ConversationConnector;
						
						if (connector == null) {
							addEditScript.Enabled = false;
							addEditCondition.Enabled = false;
						}						
						else {
							addEditScript.Enabled = true;
							addEditCondition.Enabled = true;
						}
						
						if (connector != null && ScriptHelper.HasFlipScriptAttachedAsAction(connector)) {
							addEditScript.Text = "Edit script";
							deleteScript.Enabled = true;
						}
						else {
							addEditScript.Text = "Add script";
							deleteScript.Enabled = false;
						}
						
						if (connector != null && ScriptHelper.HasFlipScriptAttachedAsCondition(connector)) {
							addEditCondition.Text = "Edit condition";
							deleteCondition.Enabled = true;	
						}
						else {
							addEditCondition.Text = "Add condition";
							deleteCondition.Enabled = false;	
						}
					}	
				};
				
				tree.ContextMenu.MenuItems.Add("-");
				tree.ContextMenu.MenuItems.Add(addEditScript);
				tree.ContextMenu.MenuItems.Add(deleteScript);
				tree.ContextMenu.MenuItems.Add("-");
				tree.ContextMenu.MenuItems.Add(addEditCondition);
				tree.ContextMenu.MenuItems.Add(deleteCondition);
			}
			catch (Exception x) {
				MessageBox.Show("Error when trying to integrate Flip with conversation editor.",x.ToString());
			}		
		}
		
		
//		/// <summary>
//		/// 
//		/// </summary>
//		/// <returns></returns>
//		/// <remarks>Use this method rather than NWN2BlueprintView.GetAllSelectedBlueprints() as
//		/// it returns the selected blueprint in EVERY category (bizarrely).</remarks>
//		protected NWN2BlueprintCollection GetSelectedBlueprints()
//		{
//			NWN2BlueprintCollection blueprints = new NWN2BlueprintCollection();
//			
//			if (blueprintView != null && blueprintView.ActivePalette != null) {	
//				foreach (object o in blueprintView.ActivePalette.Selection) {
//					INWN2Blueprint blueprint = o as INWN2Blueprint;
//					if (blueprint != null) {
//						blueprints.Add(blueprint);
//					}
//				}
//			}
//			
//			return blueprints;
//		}
		
		
		/// <summary>
		/// Gather references to toolset fields which are used by other plugin methods.
		/// </summary>
		protected void FindFields()
		{						
			foreach (FieldInfo field in typeof(NWN2PropertyGrid).GetFields(BindingFlags.Instance | BindingFlags.NonPublic)) {
				if (field.FieldType == typeof(PropertyGrid)) {
					innerPropertyGridFieldInfo = field;
				}
			}
				
			foreach (FieldInfo field in typeof(NWN2ToolsetMainForm).GetFields(BindingFlags.Instance | BindingFlags.NonPublic)) {
				
				if (field.FieldType == typeof(NWN2PropertyGrid)) {
					NWN2PropertyGrid mainPropertyGrid = (NWN2PropertyGrid)field.GetValue(NWN2ToolsetMainForm.App);
					/*
					 * If Narrative Threads is running the main property grid may already have been disposed,
					 * so check that it (and its inner grid) are not null or disposed whenever using them.
					 */
					if (mainPropertyGrid != null) {
						Watch(mainPropertyGrid);
						mainPropertyInnerGrid = (PropertyGrid)innerPropertyGridFieldInfo.GetValue(mainPropertyGrid);
					}
				}
				
				else if (field.FieldType == typeof(DockingManager)) {
					dockingManager = (DockingManager)field.GetValue(NWN2ToolsetMainForm.App);
				}
				
				else if (field.FieldType == typeof(TD.SandBar.ToolBar)) {
					ToolBar tb = (ToolBar)field.GetValue(NWN2ToolsetMainForm.App);
					if (tb.Text == "Object Manipulation") objectsToolbar = tb;
				}

				else if (field.FieldType == typeof(NWN2ModuleScriptList)) {
					scriptPanels.Add((NWN2ModuleScriptList)field.GetValue(NWN2ToolsetMainForm.App));
				}
				
				else if (field.FieldType == typeof(NWN2BlueprintView)) {
					blueprintView = (NWN2BlueprintView)field.GetValue(NWN2ToolsetMainForm.App);					
					
					winforms.ContextMenu menu = blueprintView.ActivePalette.ContextMenu;
										
					if (menu != null) {
						try {
							winforms.MenuItem item = new winforms.MenuItem("Create a Flip block from this blueprint");
							
							item.Click += delegate 
							{  
								NWN2BlueprintCollection blueprints = blueprintView.GetFocusedListSelectedBlueprints();
								
								if (blueprints.Count == 0) {
									MessageBox.Show("No blueprint selected.");
								}
								
								else if (blueprints.Count > 1) {
									MessageBox.Show("Select one blueprint at a time (you have " + blueprints.Count + " blueprints selected).");									
								}
								
								else {
									try {
										createBlockFromBlueprintDelegate.Invoke(blueprints);
									}
									catch (Exception x) {										
										MessageBox.Show("Something went wrong when trying to create a block from a blueprint.\n\n" + x);
									}
								}
							};
							
							menu.Popup += delegate 
							{  
								NWN2BlueprintCollection blueprints = blueprintView.GetFocusedListSelectedBlueprints();
								item.Enabled = blueprints != null && blueprints.Count > 0;
							};
							
							menu.MenuItems.Add("-");
							menu.MenuItems.Add(item);
						}
						catch (Exception x) {
							MessageBox.Show("Something went wrong when adding a new option to the Blueprints menu.\n\n" + x);
						}
					}
				}
				
				else if (field.FieldType == typeof(TD.SandBar.MenuButtonItem)) {
					TD.SandBar.MenuButtonItem mbi = (TD.SandBar.MenuButtonItem)field.GetValue(NWN2ToolsetMainForm.App);
					if (mbi.Text == "&Script" || mbi.Text == "Open Conversation/Script") {
						scriptMenuItems.Add(mbi);
					}
				}
					
				// No longer required to find mainPropertyInnerGrid as a result of Narrative Threads integration:
				if (dockingManager != null && 
				    //mainPropertyInnerGrid != null &&
				    innerPropertyGridFieldInfo != null && 
				    scriptPanels.Count == 2 &&
				    scriptMenuItems.Count == 2) return;
			}
			
			throw new ApplicationException("Failed to find a crucial field via reflection.");
		}
		
		
		/// <summary>
		/// Enable or disable access to script-related functionality in the toolset. This includes
		/// the script list panels, the ability to create or open scripts, and the script slot
		/// properties on game objects.
		/// </summary>
		/// <param name="allow">True to enable script-related functionality, false to disable.</param>
		protected void ControlAccessToScripts(bool allow)
		{	
			// Narrative Threads may have disposed the main property grid, so check that it exists:
			if (mainPropertyInnerGrid != null && !mainPropertyInnerGrid.IsDisposed) {
				// If blocking scripts, hide script slots on the main property grid whenever it changes:
				mainPropertyInnerGrid.SelectedObjectChanged -= mainGridHandler;
				if (!allow) mainPropertyInnerGrid.SelectedObjectChanged += mainGridHandler;		
			}
			
			// If blocking scripts, hide script slots on floating property grids when they first appear:
			dockingManager.ContentShown -= floatingGridHandler;
			if (!allow) dockingManager.ContentShown += floatingGridHandler;
			
			// Enable/disable user controls relating to script panels:
			foreach (NWN2ModuleScriptList panel in scriptPanels) {
				panel.Enabled = allow;
			}
			foreach (TD.SandBar.MenuButtonItem menuItem in scriptMenuItems) {
				menuItem.Enabled = allow;
			}
					
			// Refresh each open property grid, then hide script slots if appropriate:
			foreach (Content content in dockingManager.Contents) {
				if (content.Control is NWN2PropertyGrid) {
					PropertyGrid innerGrid = (PropertyGrid)innerPropertyGridFieldInfo.GetValue(content.Control);
					innerGrid.SelectedObjects = innerGrid.SelectedObjects;
					if (!allow) HideScriptSlots(innerGrid);
				}
			}
		}

		
		/// <summary>
		/// Hide script slot fields on a property grid.
		/// </summary>
		protected void HideScriptSlotsOnMainGrid(object sender, SelectedObjectChangedEventArgs e)
		{
			HideScriptSlots((PropertyGrid)sender);
		}
						

		/// <summary>
		/// Check whether the newly opened content is a property grid, and if so,
		/// hide its script slot fields.
		/// </summary>
		/// <param name="c">The content which was just opened.</param>
		/// <param name="cea">Arguments relating to this event.</param>
		protected void HideScriptSlotsOnFloatingGrid(Content c, EventArgs cea)
		{
			if (c.Control is NWN2PropertyGrid) {
				NWN2PropertyGrid grid = (NWN2PropertyGrid)c.Control;
				PropertyGrid innerGrid = (PropertyGrid)innerPropertyGridFieldInfo.GetValue(grid);
				HideScriptSlots(innerGrid);
			}
		}
		
		
		/// <summary>
		/// Hide script slot fields on a property grid.
		/// </summary>
		/// <param name="innerGrid">The property grid.</param>
		protected void HideScriptSlots(PropertyGrid innerGrid)
		{
			PropertyEnumerator enumerator = innerGrid.FirstProperty;
			do {
				if (enumerator.Property != null && enumerator.Property.Name.StartsWith("On")) {
					innerGrid.DeleteProperty(enumerator);
				}
				else {
					enumerator.MoveNext();
				}
			}
			while (enumerator != enumerator.RightBound);
		}
		
		#endregion
	}
}
