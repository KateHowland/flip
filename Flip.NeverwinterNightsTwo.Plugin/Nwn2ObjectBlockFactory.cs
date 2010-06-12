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
 * This file added by Keiron Nicholson on 15/02/2010 at 14:54.
 */

using System;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using NWN2Toolset.NWN2.Data;
using NWN2Toolset.NWN2.Data.Blueprints;
using NWN2Toolset.NWN2.Data.Instances;
using NWN2Toolset.NWN2.Data.Templates;
using Sussex.Flip.Games.NeverwinterNightsTwo.Behaviours;
using Sussex.Flip.Games.NeverwinterNightsTwo.Integration;
using Sussex.Flip.Games.NeverwinterNightsTwo.Utils;
using Sussex.Flip.UI;

namespace Sussex.Flip.Games.NeverwinterNightsTwo
{
	/// <summary>
	/// Creates ObjectBlocks representing Neverwinter Nights 2 objects.
	/// </summary>
	public class Nwn2ObjectBlockFactory
	{	
		protected Nwn2ImageProvider images;
		
		
		public Nwn2ImageProvider ImageProvider {
			get { return images; }
		}
		
		
		public Nwn2ObjectBlockFactory()
		{
			images = new Nwn2ImageProvider(new NarrativeThreadsHelper());
		}
		
		
		public Nwn2ObjectBlockFactory(Nwn2ImageProvider images)
		{
			this.images = images;
		}
		
		
		/// <summary>
		/// Creates an ObjectBlock representing the player.
		/// </summary>
		/// <returns>An ObjectBlock representing the player.</returns>
		public ObjectBlock CreatePlayerBlock()
		{
			ObjectBlock block = new ObjectBlock(null,new PlayerBehaviour());
			block.AssignImage(images);
			return block;
		}
		
		
		/// <summary>
		/// Creates an ObjectBlock representing the module.
		/// </summary>
		/// <returns>An ObjectBlock representing the module.</returns>
		public ObjectBlock CreateModuleBlock()
		{
			ObjectBlock block = new ObjectBlock(null,new ModuleBehaviour());
			block.AssignImage(images);
			return block;
		}
		
		
		/// <summary>
		/// Creates an ObjectBlock representing a given area.
		/// </summary>
		/// <param name="area">The area to represent.</param>
		/// <returns>An ObjectBlock representing the given area.</returns>
		public ObjectBlock CreateAreaBlock(NWN2GameArea area)
		{
			if (area == null) throw new ArgumentNullException("area");
			
			AreaBehaviour behaviour = CreateAreaBehaviour(area);
			
			ObjectBlock block = CreateAreaBlock(behaviour);
			
			return block;
		}
		
		
		public AreaBehaviour CreateAreaBehaviour(NWN2GameArea area)
		{
			if (area == null) throw new ArgumentNullException("area");
			
			string tag = area.Tag;
			string displayName = area.DisplayName.GetSafeString(OEIShared.Utils.BWLanguages.CurrentLanguage).Value;
			if (String.IsNullOrEmpty(displayName)) {
				displayName = tag;
			}
		
			return new AreaBehaviour(tag,displayName,area.HasTerrain);
		}
		
		
		/// <summary>
		/// Creates an ObjectBlock representing a given area.
		/// </summary>
		/// <param name="area">An object representing the behaviour of
		/// and information about the ObjectBlock which is to be created.</param>
		/// <returns>An ObjectBlock representing the given area.</returns>
		public ObjectBlock CreateAreaBlock(AreaBehaviour area)
		{
			if (area == null) throw new ArgumentNullException("area");
			
			ObjectBlock block = new ObjectBlock(null,area);
			block.AssignImage(images);
			return block;
		}
		
			
		public ObjectBlock CreateBlueprintBlock(INWN2Blueprint blueprint)
		{
			if (blueprint == null) throw new ArgumentNullException("blueprint");
			
			BlueprintBehaviour behaviour = CreateBlueprintBehaviour(blueprint);
			
			ObjectBlock block = CreateBlueprintBlock(behaviour);
			
			return block;
		}
		
		
		public BlueprintBehaviour CreateBlueprintBehaviour(INWN2Blueprint blueprint)
		{
			if (blueprint == null) throw new ArgumentNullException("blueprint");
			
			string objectTypeString = blueprint.ObjectType.ToString();
			string resRef = blueprint.ResourceName.Value;	
			string baseResRef = blueprint.TemplateResRef.Value;
																		
			string displayName = GetDisplayName(blueprint);				
			if (displayName == String.Empty) {
				displayName = blueprint.Name;
			}
			
			string icon;
			if (blueprint is NWN2ItemTemplate) {
				icon = ((NWN2ItemTemplate)blueprint).Icon.ToString();
			}
			else {
				icon = String.Empty;
			}
				
			return new BlueprintBehaviour(resRef,displayName,baseResRef,icon,blueprint.ObjectType);
		}
		
		
		public ObjectBlock CreateBlueprintBlock(BlueprintBehaviour behaviour)
		{
			if (behaviour == null) throw new ArgumentNullException("behaviour");
				
			ObjectBlock block = new ObjectBlock(null,behaviour);
			block.AssignImage(images);
			return block;
		}
		
			
		public ObjectBlock CreateInstanceBlock(INWN2Instance instance)
		{
			if (instance == null) throw new ArgumentNullException("instance");
			
			InstanceBehaviour behaviour = CreateInstanceBehaviour(instance);
			
			ObjectBlock block = CreateInstanceBlock(behaviour);
			
			return block;
		}
		
			
		public ObjectBlock CreateInstanceBlock(INWN2Instance instance, NWN2GameArea area)
		{
			if (instance == null) throw new ArgumentNullException("instance");
			
			InstanceBehaviour behaviour = CreateInstanceBehaviour(instance,area);
			
			ObjectBlock block = CreateInstanceBlock(behaviour);
			
			return block;
		}
		
		
		public InstanceBehaviour CreateInstanceBehaviour(INWN2Instance instance, NWN2GameArea area)
		{
			if (instance == null) throw new ArgumentNullException("instance");
			
			string tag = ((INWN2Object)instance).Tag;
				
			string displayName = GetDisplayName(instance);
				
			if (displayName == String.Empty) displayName = tag;
			
			string areaTag;
			if (area != null) {
				areaTag = area.Tag;
			}
			else if (instance.Area != null) {
				areaTag = instance.Area.Tag;
			}
			else {
				areaTag = String.Empty;
			}
			
			string resRef = instance.Template.ResRef.Value;
						
			string icon;
			if (instance is NWN2ItemInstance) {
				icon = ((NWN2ItemInstance)instance).Icon.ToString();
			}
			else {
				icon = String.Empty;
			}
			
			return new InstanceBehaviour(tag,displayName,instance.ObjectType,areaTag,resRef,icon);
		}
		
		
		public InstanceBehaviour CreateInstanceBehaviour(INWN2Instance instance)
		{						
			if (instance == null) throw new ArgumentNullException("instance");	
			
			return CreateInstanceBehaviour(instance,instance.Area);
		}
		
		
		public ObjectBlock CreateInstanceBlock(InstanceBehaviour behaviour)
		{						
			if (behaviour == null) throw new ArgumentNullException("behaviour");
			
			ObjectBlock block = new ObjectBlock(null,behaviour);
			block.AssignImage(images);
			return block;
		}
		
			
		public ObjectBlock CreateInstanceBlockFromBlueprint(INWN2Blueprint blueprint)
		{
			if (blueprint == null) throw new ArgumentNullException("blueprint");
			
			InstanceBehaviour behaviour = CreateInstanceBehaviourFromBlueprint(blueprint);
			
			ObjectBlock block = CreateInstanceBlock(behaviour);
			
			return block;
		}
		
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="blueprint"></param>
		/// <returns></returns>
		/// <remarks>For use with Narrative Threads.</remarks>
		public InstanceBehaviour CreateInstanceBehaviourFromBlueprint(INWN2Blueprint blueprint)
		{
			if (blueprint == null) throw new ArgumentNullException("blueprint");
			
			string tag = ((INWN2Object)blueprint).Tag;
				
			string displayName = GetDisplayName(blueprint);
				
			if (displayName == String.Empty) displayName = tag;
			
			string areaTag = String.Empty;
			
			string resRef = blueprint.ResourceName.Value;
						
			string icon;
			if (blueprint is NWN2ItemBlueprint) {
				icon = ((NWN2ItemBlueprint)blueprint).Icon.ToString();
			}
			else {
				icon = String.Empty;
			}
			
			return new InstanceBehaviour(tag,displayName,blueprint.ObjectType,areaTag,resRef,icon);
		}
		
		
		public ObjectBlock CreateBlock(ObjectBehaviour behaviour)
		{
			if (behaviour == null) throw new ArgumentNullException("behaviour");
			
			if (behaviour is AreaBehaviour) return CreateAreaBlock((AreaBehaviour)behaviour);
			
			if (behaviour is BlueprintBehaviour) return CreateBlueprintBlock((BlueprintBehaviour)behaviour);
							
			if (behaviour is InstanceBehaviour) return CreateInstanceBlock((InstanceBehaviour)behaviour);
			
			if (behaviour is ModuleBehaviour) return CreateModuleBlock();
			
			if (behaviour is PlayerBehaviour) return CreatePlayerBlock();
			
			throw new ArgumentException("Passed behaviour was not of a type that can be processed by this factory.","behaviour");
		}
		
		
//		/ <summary>
//		/ Creates an ObjectBlock representing a given instance.
//		/ </summary>
//		/ <param name="instance">The instance to represent.</param>
//		/ <param name="instance">The area holding this instance.</param>
//		/ <returns>An ObjectBlock representing the given instance.</returns>
//		/ <remarks>The Area property of an instance is sometimes null, even if
//		/ the area has not actually been disposed. For this reason, CreateInstanceBlock
//		/ has an overload which allows you to pass in the holding area directly.</remarks>
		
		
		protected string GetDisplayName(INWN2Template template)
		{
			if (template == null) throw new ArgumentNullException("template");
			
			string displayName = null;
				
			if (template is NWN2CreatureTemplate) {
				NWN2CreatureTemplate creature = (NWN2CreatureTemplate)template;
				
				string firstName = creature.FirstName.GetSafeString(OEIShared.Utils.BWLanguages.CurrentLanguage).Value;
				string lastName = creature.LastName.GetSafeString(OEIShared.Utils.BWLanguages.CurrentLanguage).Value;
				
				if (String.IsNullOrEmpty(lastName)) {
					displayName = firstName;
				}
				else {
					displayName = String.Format("{0} {1}",firstName,lastName);
				}
			}
			         
			else {
				// Everything else uses LocalizedName, but this isn't defined on a superclass.				
				PropertyInfo property = template.GetType().GetProperty("LocalizedName",BindingFlags.Public | BindingFlags.Instance);
				
				if (property != null) {
					object obj = property.GetValue(template,null);
					
					if (obj != null) {
						displayName = obj.ToString();
						
						if (!String.IsNullOrEmpty(displayName)) {
							
							// Remove surrounding curly brackets from (almost all) doors:
							if (displayName.StartsWith("{") && displayName.EndsWith("}")) {
								displayName = displayName.Substring(1,displayName.Length-2);
							}
							// Remove surrounding quotes and commas from placed effects:
							if (displayName.StartsWith("\"") && displayName.EndsWith("\", ")) {
								displayName = displayName.Substring(1,displayName.Length-4);
							}
						}
					}
				}
			}
						
			if (displayName != null) return displayName;
			else return String.Empty;
		}
	}
}