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
			
			return new ObjectBlock(image,GetPlayerBlockBehaviour());
		}
		
		
		/// <summary>
		/// Creates an ObjectBlock representing the module.
		/// </summary>
		/// <returns>An ObjectBlock representing the module.</returns>
		public ObjectBlock CreateModuleBlock()
		{
			Image image = GetImage("Other","Module");
			if (image == null) image = GetImage("Placeholder","Default");
			
			return new ObjectBlock(image,GetModuleBlockBehaviour());
		}
		
		
		/// <summary>
		/// Creates an ObjectBlock representing a given area.
		/// </summary>
		/// <param name="area">The area to represent.</param>
		/// <returns>An ObjectBlock representing the given area.</returns>
		public ObjectBlock CreateAreaBlock(NWN2GameArea area)
		{
			if (area == null) throw new ArgumentNullException("area");
			
			Image image;
			if (area.HasTerrain) image = GetImage("Other","Area_Exterior");
			else image = GetImage("Other","Area_Interior");
						
			if (image == null) image = GetImage("Placeholder","Default");			
			
			string displayName = area.DisplayName.GetSafeString(OEIShared.Utils.BWLanguages.CurrentLanguage).Value;
			
			return new ObjectBlock(image,GetAreaBlockBehaviour(area.Tag,displayName));
		}
		
				
		/// <summary>
		/// Creates an ObjectBlock representing a given blueprint.
		/// </summary>
		/// <param name="blueprint">The blueprint to represent.</param>
		/// <returns>An ObjectBlock representing the given blueprint.</returns>
		public ObjectBlock CreateBlueprintBlock(INWN2Blueprint blueprint)
		{			
			if (blueprint == null) throw new ArgumentNullException("blueprint");
			
			string objectTypeString = blueprint.ObjectType.ToString();
			
			// First try to retrieve an image of this particular blueprint:
			string blueprintResRef = blueprint.ResourceName.Value;			
			Image image = GetImage(objectTypeString,blueprintResRef);
			
			// Failing that, try to retrieve an image of the blueprint it derives from:
			string baseResRef = blueprint.TemplateResRef.Value;
			if (image == null) image = GetImage(objectTypeString,baseResRef);
			
			// If neither is available, use a placeholder image:
			if (image == null) image = GetImage("Placeholder","Blueprint");// TODO: String.Format("Blueprint_{0}",objectType));
													
			string displayName = GetDisplayName(blueprint);
				
			if (displayName == String.Empty) displayName = blueprint.Name;
				
			ObjectBlock block = new ObjectBlock(image,GetBlueprintBlockBehaviour(blueprintResRef,displayName,blueprint.ObjectType));
			return block;
		}
		
		
		/// <summary>
		/// Creates an ObjectBlock representing a given instance.
		/// </summary>
		/// <param name="instance">The instance to represent.</param>
		/// <returns>An ObjectBlock representing the given instance.</returns>
		public ObjectBlock CreateInstanceBlock(INWN2Instance instance)
		{						
			if (instance == null) throw new ArgumentNullException("instance");
			
			string resRef = instance.Template.ResRef.Value;
			
			Image image = GetImage(instance.ObjectType.ToString(),resRef);
			if (image == null) image = GetImage("Placeholder","Instance");// TODO: String.Format("Instance_{0}",objectType));
			
			try {
				string tag = ((INWN2Object)instance).Tag;		
				
				string displayName = GetDisplayName(instance);
				
				if (displayName == String.Empty) displayName = tag;
				
				ObjectBlock block = new ObjectBlock(image,GetInstanceBlockBehaviour(tag,displayName,instance.ObjectType));
				
				return block;
			}
			catch (InvalidCastException e) {
				throw new ApplicationException("Creation of a visual block to represent " + instance.Name + 
				                               "failed. Could not cast to INWN2Object to retrieve tag.",e);
			}
		}
		
		
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
		
		#region Behaviours
		
		/// <summary>
		/// Gets an ObjectBehaviour representing the player.
		/// </summary>
		/// <returns>An ObjectBehaviour representing the player.</returns>
		protected ObjectBehaviour GetPlayerBlockBehaviour()
		{
			return new Player();
		}
		
		
		/// <summary>
		/// Gets an ObjectBehaviour representing the module.
		/// </summary>
		/// <returns>An ObjectBehaviour representing the module.</returns>
		protected ObjectBehaviour GetModuleBlockBehaviour()
		{
			return new Sussex.Flip.Games.NeverwinterNightsTwo.Behaviours.Module();
		}
		
		
		/// <summary>
		/// Gets an ObjectBehaviour representing a particular area.
		/// </summary>
		/// <param name="tag">The tag of the area.</param>
		/// <param name="displayName">The display name for this area.</param>
		/// <returns>An ObjectBehaviour representing a particular area.</returns>
		protected ObjectBehaviour GetAreaBlockBehaviour(string tag, string displayName)
		{
			return new Area(tag,displayName);
		}
		
		
		/// <summary>
		/// Gets an ObjectBehaviour representing a particular instance.
		/// </summary>
		/// <param name="tag">The tag of the instance.</param>
		/// <param name="displayName">The display name for this instance.</param>
		/// <param name="type">The type of this instance.</param>
		/// <returns>An ObjectBehaviour representing a particular instance.</returns>
		protected ObjectBehaviour GetInstanceBlockBehaviour(string tag, string displayName, NWN2ObjectType type)
		{
			return new Instance(tag,displayName,type);
		}
		
				
		/// <summary>
		/// Gets an ObjectBehaviour representing a particular blueprint.
		/// </summary>
		/// <param name="tag">The tag of the blueprint.</param>
		/// <param name="displayName">The display name for this blueprint.</param>
		/// <param name="type">The type of this blueprint.</param>
		/// <returns>An ObjectBehaviour representing a particular blueprint.</returns>
		protected ObjectBehaviour GetBlueprintBlockBehaviour(string resRef, string displayName, NWN2ObjectType type)
		{
			return new Blueprint(resRef,displayName,type);
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
			// TODO:
			// We should ultimately be using a .dll collection
			// of image resources.
			string pathFormat = @"C:\Flip\object pics\{0}\{1}.bmp";
			string path = String.Format(pathFormat,type,name);
			return GetImage(path);
		}
		
		#endregion
	}
}
