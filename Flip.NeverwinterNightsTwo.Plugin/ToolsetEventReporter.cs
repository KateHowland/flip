/*
 * Flip - a visual programming language for scripting video games
 * Copyright (C) 2009 University of Sussex
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
 * This file added by Keiron Nicholson on 14/01/2010 at 17:39.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NWN2Toolset;
using NWN2Toolset.NWN2.Data;
using NWN2Toolset.NWN2.Data.Blueprints;
using NWN2Toolset.NWN2.Data.Instances;
using NWN2Toolset.NWN2.Data.Templates;
using NWN2Toolset.NWN2.Data.TypedCollections;
using NWN2Toolset.NWN2.Views;
using TD.SandBar;
using OEIShared.Utils;
using Crownwood.DotNetMagic.Common;
using Crownwood.DotNetMagic.Controls;
using Sussex.Flip.Games.NeverwinterNightsTwo.Utils;

namespace Sussex.Flip.Games.NeverwinterNightsTwo.Plugin
{
	/// <summary>
	/// Raises events when certain things occur in a Neverwinter
	/// Nights 2 toolset session.
	/// </summary>
	/// <remarks>Call ToolsetEventReporter.Start() to begin
	/// tracking toolset activities and raising events.</remarks>
	public class ToolsetEventReporter
	{		
		#region Events
		
		/// <summary>
		/// Raised when the current module changes.
		/// </summary>
		public event ModuleChangedEventHandler ModuleChanged;
		
		/// <summary>
		/// Raised when a blueprint is added to the current module.
		/// </summary>
		/// <remarks>Campaign and global blueprint changes are
		/// not currently tracked.</remarks>
		public event EventHandler<BlueprintEventArgs> BlueprintAdded;
		
		/// <summary>
		/// Raised when a blueprint is removed from the current module.
		/// </summary>
		public event EventHandler<BlueprintEventArgs> BlueprintRemoved;
		
		/// <summary>
		/// Raised when a game object is added to an area in the current module.
		/// </summary>
		public event EventHandler<InstanceEventArgs> InstanceAdded;
				
		/// <summary>
		/// Raised when a game object is removed from an area in the current module.
		/// </summary>
		public event EventHandler<InstanceEventArgs> InstanceRemoved;
		
		/// <summary>
		/// Raised when an area is added to the current module.
		/// </summary>
		public event EventHandler<AreaEventArgs> AreaAdded;
		
		/// <summary>
		/// Raised when an area is removed from the current module.
		/// </summary>
		public event EventHandler<AreaEventArgs> AreaRemoved;
		
		/// <summary>
		/// Raised when a conversation is added to the current module.
		/// </summary>
		public event EventHandler<ConversationEventArgs> ConversationAdded;
		
		/// <summary>
		/// Raised when a conversation is removed from the current module.
		/// </summary>
		public event EventHandler<ConversationEventArgs> ConversationRemoved;
		
		/// <summary>
		/// Raised when a script is added to the current module.
		/// </summary>
		public event EventHandler<ScriptEventArgs> ScriptAdded;
		
		/// <summary>
		/// Raised when a script is removed from the current module.
		/// </summary>
		public event EventHandler<ScriptEventArgs> ScriptRemoved;
		
		/// <summary>
		/// Raised when an area is opened in the toolset.
		/// </summary>
		public event EventHandler<AreaEventArgs> AreaOpened;
		
		/// <summary>
		/// Raised when a conversation is opened in the toolset.
		/// </summary>
		public event EventHandler<ConversationEventArgs> ConversationOpened;
		
		/// <summary>
		/// Raised when a script is opened in the toolset.
		/// </summary>
		public event EventHandler<ScriptEventArgs> ScriptOpened;
		
		/// <summary>
		/// Raised when an area, conversation, script or some other
		/// viewed resource is closed in the toolset.
		/// </summary>
		public event EventHandler<ResourceViewerClosedEventArgs> ResourceViewerClosed;
				
		
		protected virtual void OnModuleChanged(object sender, ModuleChangedEventArgs e)
		{
			ModuleChangedEventHandler handler = ModuleChanged;
			if (handler != null) {
				handler(sender,e);
			}
		}
		
		protected virtual void OnBlueprintAdded(object sender, BlueprintEventArgs e)
		{
			EventHandler<BlueprintEventArgs> handler = BlueprintAdded;
			if (handler != null) {
				handler(sender,e);
			}
		}
		
		protected virtual void OnBlueprintRemoved(object sender, BlueprintEventArgs e)
		{
			EventHandler<BlueprintEventArgs> handler = BlueprintRemoved;
			if (handler != null) {
				handler(sender,e);
			}
		}
		
		protected virtual void OnInstanceAdded(object sender, InstanceEventArgs e)
		{
			EventHandler<InstanceEventArgs> handler = InstanceAdded;
			if (handler != null) {
				handler(sender,e);
			}
		}
		
		protected virtual void OnInstanceRemoved(object sender, InstanceEventArgs e)
		{
			EventHandler<InstanceEventArgs> handler = InstanceRemoved;
			if (handler != null) {
				handler(sender,e);
			}
		}
		
		protected virtual void OnAreaAdded(object sender, AreaEventArgs e)
		{
			EventHandler<AreaEventArgs> handler = AreaAdded;
			if (handler != null) {
				handler(sender,e);
			}
		}
		
		protected virtual void OnAreaRemoved(object sender, AreaEventArgs e)
		{
			EventHandler<AreaEventArgs> handler = AreaRemoved;
			if (handler != null) {
				handler(sender,e);
			}
		}
		
		protected virtual void OnConversationAdded(object sender, ConversationEventArgs e)
		{
			EventHandler<ConversationEventArgs> handler = ConversationAdded;
			if (handler != null) {
				handler(sender,e);
			}
		}
		
		protected virtual void OnConversationRemoved(object sender, ConversationEventArgs e)
		{
			EventHandler<ConversationEventArgs> handler = ConversationRemoved;
			if (handler != null) {
				handler(sender,e);
			}
		}
		
		protected virtual void OnScriptAdded(object sender, ScriptEventArgs e)
		{
			EventHandler<ScriptEventArgs> handler = ScriptAdded;
			if (handler != null) {
				handler(sender,e);
			}
		}
		
		protected virtual void OnScriptRemoved(object sender, ScriptEventArgs e)
		{
			EventHandler<ScriptEventArgs> handler = ScriptRemoved;
			if (handler != null) {
				handler(sender,e);
			}
		}
		
		protected virtual void OnAreaOpened(object sender, AreaEventArgs e)
		{
			EventHandler<AreaEventArgs> handler = AreaOpened;
			if (handler != null) {
				handler(sender,e);
			}
		}
		
		protected virtual void OnConversationOpened(object sender, ConversationEventArgs e)
		{
			EventHandler<ConversationEventArgs> handler = ConversationOpened;
			if (handler != null) {
				handler(sender,e);
			}
		}
		
		protected virtual void OnScriptOpened(object sender, ScriptEventArgs e)
		{
			EventHandler<ScriptEventArgs> handler = ScriptOpened;
			if (handler != null) {
				handler(sender,e);
			}
		}
		
		protected virtual void OnResourceViewerClosed(object sender, ResourceViewerClosedEventArgs e)
		{
			EventHandler<ResourceViewerClosedEventArgs> handler = ResourceViewerClosed;
			if (handler != null) {
				handler(sender,e);
			}
		}
		
		#endregion
				
		#region Methods
		
		/// <summary>
		/// Begins tracking toolset activities and raising associated events.
		/// </summary>
		public void Start()
		{
			/*
			 * Track when a module is opened, when resources are added to or removed from
			 * the module, when blueprints are added to or removed from the module, and 
			 * when objects are added to or removed from an area.
			 */
			
			WatchModule(NWN2ToolsetMainForm.App.Module);
				
			NWN2ToolsetMainForm.ModuleChanged += delegate(object oSender, ModuleChangedEventArgs e) 
			{  
				OnModuleChanged(oSender,e);				
				WatchModule(NWN2ToolsetMainForm.App.Module);
			};
			
			FieldInfo[] fields = typeof(NWN2ToolsetMainForm).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
			foreach (FieldInfo field in fields) {	
				/*
				 * Track when resource viewers are opening or closing.
				 */
				if (field.FieldType == typeof(TabbedGroups)) {
					TabbedGroups tg = (TabbedGroups)field.GetValue(NWN2ToolsetMainForm.App);
					CollectionChange opened = new CollectionChange(ViewerOpened);
					CollectionChange closed = new CollectionChange(ViewerClosed);
					
					tg.ActiveLeaf.TabPages.Inserted += opened;
					tg.ActiveLeaf.TabPages.Removed += closed;
					
					// ActiveLeaf is disposed whenever all viewers are closed, so attach handlers again:
					tg.ActiveLeafChanged += delegate
					{  
						tg.ActiveLeaf.TabPages.Inserted += opened;
						tg.ActiveLeaf.TabPages.Removed += closed;
					};
				}
			}
		}
		
		
		protected void WatchModule(NWN2GameModule mod)
		{
			// TODO: Can this be made to work with the NWN2GlobalBlueprintManager instead?
			// so that it can track ANY added/removed blueprints?
			
			if (mod != null) {	
				
				/* There seems to be a bug where the following events events fire at a later stage... they do
				 * fire when they should, but when the paused test is allowed to complete, they fire again. */
				foreach (NWN2BlueprintCollection bc in mod.BlueprintCollections) {
					bc.Inserted += delegate(OEICollectionWithEvents cList, int index, object value) 
					{
						OnBlueprintAdded(bc,new BlueprintEventArgs((INWN2Blueprint)value));
					};
					
					bc.Removed += delegate(OEICollectionWithEvents cList, int index, object value)
					{
						OnBlueprintRemoved(bc,new BlueprintEventArgs((INWN2Blueprint)value));
					};
				}
										
				List<OEIDictionaryWithEvents> dictionaries;
				dictionaries = new List<OEIDictionaryWithEvents> { mod.Areas, mod.Conversations, mod.Scripts };
					
				OEIDictionaryWithEvents.ChangeHandler dAdded = new OEIDictionaryWithEvents.ChangeHandler(ResourceInsertedIntoCollection);
				OEIDictionaryWithEvents.ChangeHandler dRemoved = new OEIDictionaryWithEvents.ChangeHandler(ResourceRemovedFromCollection);
				foreach (OEIDictionaryWithEvents dictionary in dictionaries) {
					dictionary.Inserted += dAdded;
					dictionary.Removed += dRemoved;
				}
			}
		}

		
		/// <summary>
		/// Raises events based on which viewer has been opened.
		/// </summary>
		protected void ViewerOpened(int index, object value)
		{
			TabPage page = (TabPage)value;
			INWN2Viewer viewer = page.Control as INWN2Viewer;
			if (viewer == null) return;
			
			if (viewer.ViewedResource is NWN2GameScript) {
				OnScriptOpened(page.Parent,new ScriptEventArgs((NWN2GameScript)viewer.ViewedResource));
			}
			else if (viewer.ViewedResource is NWN2GameConversation) {
				OnConversationOpened(page.Parent,new ConversationEventArgs((NWN2GameConversation)viewer.ViewedResource));
			}
			else if (viewer.ViewedResource is NWN2GameArea) {
				OnAreaOpened(page.Parent,new AreaEventArgs((NWN2GameArea)viewer.ViewedResource));
			}
		}

		
		/// <summary>
		/// Raises an event indicating that a resource viewer was closed.
		/// </summary>
		/// <remarks>Due to the viewer already being disposed, the client
		/// is only informed of the name of the resource, not the type.</remarks>
		protected void ViewerClosed(int index, object value)
		{					
			TabPage page = (TabPage)value;
			/* Even when handling Removing rather than Removed, the viewer object
			 * has already been disposed, so we can't tell what type of resource
			 * was closed. The responsibility for checking falls to the client. */
			OnResourceViewerClosed(page.Parent,new ResourceViewerClosedEventArgs(page.Title));
		}
		

		/// <summary>
		/// Raises an event indicating that a game object has been added to an area.
		/// </summary>
		protected void InstanceInsertedIntoCollection(OEICollectionWithEvents cList, int index, object value)
		{
			INWN2Instance ins = (INWN2Instance)value;
			NWN2GameArea area = (NWN2GameArea)cList.Tag;
			
			OnInstanceAdded(cList,new InstanceEventArgs(ins,area));
		}
		

		/// <summary>
		/// Raises an event indicating that a game object has been removed from an area.
		/// </summary>
		protected void InstanceRemovedFromCollection(OEICollectionWithEvents cList, int index, object value)
		{
			INWN2Instance ins = (INWN2Instance)value;
			NWN2GameArea area = (NWN2GameArea)cList.Tag;
			
			OnInstanceRemoved(cList,new InstanceEventArgs(ins,area));
		}

		
		/// <summary>
		/// Raises an event indicating that a script, area, or conversation has been added to the module.
		/// </summary>
		protected void ResourceInsertedIntoCollection(OEIDictionaryWithEvents cDictionary, object key, object value)
		{
			if (value is NWN2GameScript) {
				OnScriptAdded(cDictionary,new ScriptEventArgs((NWN2GameScript)value));
			}
			else if (value is NWN2GameConversation) {
				OnConversationAdded(cDictionary,new ConversationEventArgs((NWN2GameConversation)value));
			}
			else if (value is NWN2GameArea) {
				NWN2GameArea area = (NWN2GameArea)value;
				
				OnAreaAdded(cDictionary,new AreaEventArgs(area));
				
				OEICollectionWithEvents.ChangeHandler cAdded = new OEICollectionWithEvents.ChangeHandler(InstanceInsertedIntoCollection);
				OEICollectionWithEvents.ChangeHandler cRemoved = new OEICollectionWithEvents.ChangeHandler(InstanceRemovedFromCollection);
				
				foreach (NWN2InstanceCollection instances in area.AllInstances) {
					instances.Inserted += cAdded;
					instances.Removed += cRemoved;
				}
			}
		}

		
		/// <summary>
		/// Raises an event indicating that a script, area, or conversation has been removed from the module.
		/// </summary>
		protected void ResourceRemovedFromCollection(OEIDictionaryWithEvents cDictionary, object key, object value)
		{			
			if (value is NWN2GameScript) {
				OnScriptRemoved(cDictionary,new ScriptEventArgs((NWN2GameScript)value));
			}
			else if (value is NWN2GameConversation) {
				OnConversationRemoved(cDictionary,new ConversationEventArgs((NWN2GameConversation)value));
			}
			else if (value is NWN2GameArea) {
				OnAreaRemoved(cDictionary,new AreaEventArgs((NWN2GameArea)value));
			}
		}
		
		#endregion
	}
}
