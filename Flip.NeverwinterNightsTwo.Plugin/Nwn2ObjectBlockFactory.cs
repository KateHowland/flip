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
using Sussex.Flip.Games.NeverwinterNightsTwo.Utils;
using Sussex.Flip.UI;

namespace Sussex.Flip.Games.NeverwinterNightsTwo
{
	/// <summary>
	/// Creates ObjectBlocks representing Neverwinter Nights 2 objects.
	/// </summary>
	public class Nwn2ObjectBlockFactory
	{		
		#region Blocks
		
		/// <summary>
		/// Creates an ObjectBlock representing the player.
		/// </summary>
		/// <returns>An ObjectBlock representing the player.</returns>
		public ObjectBlock CreatePlayerBlock()
		{
			Image image = GetImage("Other","Player");
			if (image == null) image = GetImage("Placeholder","Default");
			
			return new ObjectBlock(image,new PlayerBehaviour());
		}
		
		
		/// <summary>
		/// Creates an ObjectBlock representing the module.
		/// </summary>
		/// <returns>An ObjectBlock representing the module.</returns>
		public ObjectBlock CreateModuleBlock()
		{
			Image image = GetImage("Other","Module");
			if (image == null) image = GetImage("Placeholder","Default");
			
			return new ObjectBlock(image,new ModuleBehaviour());
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
			
			Image image;
			if (area.IsExterior) image = GetImage("Other","Area_Exterior");
			else image = GetImage("Other","Area_Interior");
						
			if (image == null) image = GetImage("Placeholder","Default");
		
			ObjectBlock block = new ObjectBlock(image,area);
						
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
						
			string objectType = behaviour.Nwn2Type.ToString();
			
			Image image;
						
			// If this has an icon, use that:
			if (!String.IsNullOrEmpty(behaviour.IconName)) {
				image = GetIconImage(behaviour.IconName);
			}
			
			// Otherwise, try to get an image representing this blueprint:
			else {	
				image = GetImage(objectType,behaviour.ResRef);
			}
			
			// Failing that, try to retrieve an image of the blueprint it derives from:
			if (image == null) {
				image = GetImage(objectType,behaviour.BaseResRef);
			}
			
			// If neither is available, use a placeholder image:
			if (image == null) {
				if (behaviour.Nwn2Type == Nwn2Type.Item) {
					image = GetImage("Placeholder","Item");
				}
				else if (behaviour.Nwn2Type == Nwn2Type.Waypoint) {
					image = GetImage("Placeholder","Waypoint");
				}
				else {
					image = GetImage("Placeholder","Blueprint");
				}
			}
				
			return new ObjectBlock(image,behaviour);
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
			
			string objectType = behaviour.Nwn2Type.ToString();
			
			Image image;
						
			// If this has an icon, use that:
			if (!String.IsNullOrEmpty(behaviour.IconName)) {
				image = GetIconImage(behaviour.IconName);
			}
			
			// Otherwise, try to get an image representing this instance's template:
			else {	
				image = GetImage(objectType,behaviour.ResRef);
			}
			
			// If neither is available, use a placeholder image:
			if (image == null) {
				if (behaviour.Nwn2Type == Nwn2Type.Item) {
					image = GetImage("Placeholder","Item");
				}
				else if (behaviour.Nwn2Type == Nwn2Type.Waypoint) {
					image = GetImage("Placeholder","Waypoint");
				}
				else {
					image = GetImage("Placeholder","Instance");
				}
			}
			
			return new ObjectBlock(image,behaviour);
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
		
		#endregion
		
		#region Images
		
		/// <summary>
		/// Gets an Image from an image file at the given path.
		/// </summary>
		/// <param name="path">The path to locate an image.</param>
		/// <returns>An Image, or null if an exception was raised.</returns>
		protected Image GetImage(string path)
		{			
			if (path == null) throw new ArgumentNullException("path");
			if (!System.IO.File.Exists(path)) return null;
			
			try {
				Image image = new Image();
				Uri uri = new Uri(path);
				BitmapImage bmp = new BitmapImage(uri);
				image.Source = bmp;
				return image;
			}
			catch (Exception) {
				return null;
			}
		}
		
		
		/// <summary>
		/// Gets an image representing a particular Neverwinter Nights 2
		/// entity, of a given name and type.
		/// </summary>
		/// <param name="type">The type of entity to get an image of. Examples
		/// include 'Creature', 'Door', 'Placeholder' and 'Other'.</param>
		/// <param name="name">The name of the specific entity of the given
		/// type to get an image of. Usually the tag of an instance or the
		/// ResRef of a blueprint.</param>
		/// <returns>An Image, or null if an exception was raised.</returns>
		protected Image GetImage(string type, string name)
		{
			name = GetSimilar(type,name);
			
			// TODO:
			// We should ultimately be using a .dll collection
			// of image resources.
			string pathFormat = @"C:\Flip\object pics\{0}\{1}.bmp";
			string path = String.Format(pathFormat,type,name);
			return GetImage(path);
		}
		
		
		protected Image GetIconImage(string icon)
		{
			if (icon == null) throw new ArgumentNullException("icon");
			if (icon == String.Empty) throw new ArgumentException("Must provide an icon name.","icon");
			
			// TODO:
			// We should ultimately be using a .dll collection
			// of image resources.
			string pathFormat = @"C:\Flip\object pics\bmp icons\{0}.bmp";
			string path = String.Format(pathFormat,icon);
			Image image = GetImage(path);
			
			return image;
		}
		
		
		protected string GetSimilar(string type, string name)
		{
			if (type == "Creature") {
				if (name.StartsWith("c_elmwater")) return "c_elmwater";
				if (name.StartsWith("c_elmfire")) return "c_elmfire";
				if (name.StartsWith("c_elmearth")) return "c_elmearth";
				if (name.StartsWith("c_elmair")) return "c_elmair";
				
				if (name.StartsWith("c_ancom_badger")) return "c_badger";
				if (name.StartsWith("c_ancom_bear")) return "c_bear";
				if (name.StartsWith("c_ancom_boar")) return "c_boar";
				if (name.StartsWith("c_ancom_spider")) return "c_spider";
				if (name.StartsWith("c_ancom_wolf")) return "c_wolf";
				
				if (name.StartsWith("c_fam_bat")) return "c_bat";
				if (name.StartsWith("c_fam_beetle")) return "c_beetle";
				if (name.StartsWith("c_fam_cat")) return "c_cat";
				if (name.StartsWith("c_fam_pig")) return "c_pig";
				if (name.StartsWith("c_fam_rabbit")) return "c_rabbit";
				if (name.StartsWith("c_fam_rat")) return "c_rat";
				if (name.StartsWith("c_fam_spider")) return "c_spider";
				if (name.StartsWith("c_fam_weasel")) return "c_weasel";
				
				if (name.StartsWith("c_beetlestag")) return "c_beetle";
				if (name.StartsWith("c_faction_pig")) return "c_pig";
			}
			
			else if (type == "Door") {
				return "plc_dt_doorsc2";
			}		
			
			else if (type == "Placeholder") {
				if (name == "Instance") return "ia_sacredvengeance";
				if (name == "Blueprint") return "ia_sacredvengeance";
			}
			
			else if (type == "Other") {
				if (name == "Player") return "ig_hu_redknight";
				if (name == "Module") return "ig_hu_grumbar";
			}
			
			return name;
		}
		
		#endregion
	}
}
