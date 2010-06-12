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
		/// line of conversation is to be used as the trigger for a script.
		/// </summary>
		/// <param name="line">The line of dialogue to use as a trigger.</param>
		/// <param name="conversation">The conversation this line of dialogue is part of.</param>
		public delegate void ProvideTriggerDelegate(NWN2ConversationConnector line, NWN2GameConversation conversation);
		
		/// <summary>
		/// The delegate signature of a method to be called when a blueprint 
		/// (or blueprint collection) is to be used to create an instance 
		/// block (or blocks) in Flip.
		/// </summary>
		/// <param name="blueprints">The blueprints collection to create blocks from.</param>
		public delegate void CreateBlockFromBlueprintDelegate(NWN2BlueprintCollection blueprints);
		
		/// <summary>
		/// The method to call when a line of conversation is to be used as a trigger
		/// for a script.
		/// </summary>
		protected ProvideTriggerDelegate useDialogueAsTriggerDelegate = null;
		
		/// <summary>
		/// The method to call when a blueprint is to be used to create an instance 
		/// block in Flip.
		/// </summary>
		protected CreateBlockFromBlueprintDelegate createBlockFromBlueprintDelegate = null;
		
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
		/// <param name="useDialogueAsTriggerDelegate">A delegate which will be invoked when 
		/// the user tries to use a line of dialogue in the conversation editor as the trigger 
		/// to fire a script.</param>
		public ToolsetUIModifier(ProvideTriggerDelegate useDialogueAsTriggerDelegate, CreateBlockFromBlueprintDelegate createBlockFromBlueprintDelegate)
		{			
			if (useDialogueAsTriggerDelegate == null) throw new ArgumentNullException("useDialogueAsTriggerDelegate");
			if (createBlockFromBlueprintDelegate == null) throw new ArgumentNullException("createBlockFromBlueprintDelegate");
			
			this.useDialogueAsTriggerDelegate = useDialogueAsTriggerDelegate;
			this.createBlockFromBlueprintDelegate = createBlockFromBlueprintDelegate;
			
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
			//flipButton.ForeColor = System.Drawing.Color.DarkBlue;
			
			// TODO:
			// Keep the try/catch but centralise and sensiblise the path:
			try {
				flipButton.Icon = new System.Drawing.Icon(@"C:\Flip\object pics\Flip\fliplogo.ico");
			}
			catch (Exception) {}
			
			flipButton.ToolTipText = "Launch Flip to edit game scripts";
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
								
				winforms.MenuItem item = new winforms.MenuItem("Add Flip script");
				item.Name = "AddEditScriptMenuItem";
				
				item.Click += delegate
				{
					if (tree.SelectedNodes.Count == 0) {
						MessageBox.Show("Select a line of dialogue first.");
					}
					
					else if (tree.SelectedNodes.Count > 1) {
						MessageBox.Show("Select only one line of dialogue.");						
					}
					
					else {
						GTLTreeNode node = (GTLTreeNode)tree.SelectedNodes[0];
												
						NWN2ConversationConnector connector = node.Tag as NWN2ConversationConnector;
						
						if (connector == null) {
							MessageBox.Show("You can't add a Flip script to the root. Select a line of dialogue instead.");
						}
						
						else {							
							useDialogueAsTriggerDelegate.Invoke(connector,viewer.Conversation);
						}
					}
				};
				
				tree.ContextMenu.Popup += delegate
				{  
					if (tree.SelectedNodes.Count == 1) {
						GTLTreeNode node = (GTLTreeNode)tree.SelectedNodes[0];
												
						NWN2ConversationConnector connector = node.Tag as NWN2ConversationConnector;
						
						if (connector == null) {
							item.Enabled = false;
							item.Text = "Add Flip script";
						}
						
						else {
							item.Enabled = true;
							
							if (ScriptHelper.HasFlipScriptAttachedAsAction(connector)) {
								item.Text = "Edit Flip script";
							}
							else {
								item.Text = "Add Flip script";
							}
						}
					}					
					
					else {
						item.Enabled = false;
						item.Text = "Add Flip script";
					}
				};
				
				tree.ContextMenu.MenuItems.Add(item);
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
									createBlockFromBlueprintDelegate.Invoke(blueprints);
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
							MessageBox.Show(x.ToString());
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
