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
 * This file added by Keiron Nicholson on 02/06/2010 at 13:19.
 */

using System;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Sussex.Flip.Games.NeverwinterNightsTwo.Behaviours;
using Sussex.Flip.Games.NeverwinterNightsTwo.Integration;
using Sussex.Flip.Games.NeverwinterNightsTwo.Utils;
using Sussex.Flip.UI;

namespace Sussex.Flip.Games.NeverwinterNightsTwo
{
	/// <summary>
	/// Description of Nwn2ImageProvider.
	/// </summary>
	public class Nwn2ImageProvider : ImageProvider
	{
		protected string picturesPathFormat, iconsPathFormat;
		protected NarrativeThreadsHelper nt;
		
		
		public Nwn2ImageProvider() : this(null)
		{			
		}
		
		
		public Nwn2ImageProvider(NarrativeThreadsHelper nt)
		{
			picturesPathFormat = @"C:\Flip\object pics\{0}\{1}.bmp";
			iconsPathFormat = @"C:\Flip\object pics\bmp icons\{0}.bmp";
			this.nt = nt;
		}
				
				
		/// <summary>
		/// Gets an Image from an image file at the given path.
		/// </summary>
		/// <param name="path">The path to locate an image.</param>
		/// <returns>An Image, or null if an exception was raised.</returns>
		public Image GetImage(string path)
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
		
		
		public Image GetImage(InstanceBehaviour behaviour)
		{
			Image image;						
			string objectType = behaviour.Nwn2Type.ToString();
			
			// First, try to get a Narrative Threads user-created image:
			if (nt != null && nt.CreatedByNarrativeThreads(behaviour) && nt.HasImage(behaviour.ResRef)) {
				image = nt.GetImageForResRef(behaviour.ResRef);		
			}
						
			// Or if this has an icon, use that:
			else if (!String.IsNullOrEmpty(behaviour.IconName)) {
				image = GetIconImage(behaviour.IconName);
			}
			
			// Otherwise, try to get a generic image representing this instance's template:
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
			
			return image;
		}
		
		
		public Image GetImage(BlueprintBehaviour behaviour)
		{
			Image image;						
			string objectType = behaviour.Nwn2Type.ToString();
			
			// First, try to get a Narrative Threads user-created image:
			if (nt != null && nt.CreatedByNarrativeThreads(behaviour) && nt.HasImage(behaviour.ResRef)) {
				image = nt.GetImageForResRef(behaviour.ResRef);		
			}
			
			// Or if this has an icon, use that:
			else if (!String.IsNullOrEmpty(behaviour.IconName)) {
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
			
			return image;
		}
		
		
		public Image GetImage(AreaBehaviour behaviour)
		{
			Image image;
			
			if (behaviour.IsExterior) {
				image = GetImage("Other","Area_Exterior");
			}
			else {
				image = GetImage("Other","Area_Interior");
			}
						
			if (image == null) image = GetImage("Placeholder","Default");
		
			return image;
		}
		
		
		public Image GetPlayerImage()
		{
			Image image;
			
			image = GetImage("Other","Player");
			
			if (image == null) image = GetImage("Placeholder","Default");
			
			return image;
		}
		
		
		public Image GetModuleImage()
		{
			Image image;
			
			image = GetImage("Other","Module");
			
			if (image == null) image = GetImage("Placeholder","Default");
			
			return image;
		}
		
		
		public override void AssignImage(ObjectBlock block)
		{
			if (block == null) throw new ArgumentNullException("block");
			
			ObjectBehaviour b = block.Behaviour;
			
			if (b is InstanceBehaviour) block.DisplayImage = GetImage((InstanceBehaviour)b);
			else if (b is BlueprintBehaviour) block.DisplayImage = GetImage((BlueprintBehaviour)b);
			else if (b is AreaBehaviour) block.DisplayImage = GetImage((AreaBehaviour)b);
			else if (b is PlayerBehaviour) block.DisplayImage = GetPlayerImage();
			else if (b is ModuleBehaviour) block.DisplayImage = GetModuleImage();
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
		public Image GetImage(string type, string name)
		{
			name = GetSimilar(type,name);
			
			// TODO:
			// We should ultimately be using a .dll collection
			// of image resources.
			string path = String.Format(picturesPathFormat,type,name);
			return GetImage(path);
		}
		
		
		public Image GetIconImage(string icon)
		{
			if (icon == null) throw new ArgumentNullException("icon");
			if (icon == String.Empty) throw new ArgumentException("Must provide an icon name.","icon");
			
			// TODO:
			// We should ultimately be using a .dll collection
			// of image resources.
			string path = String.Format(iconsPathFormat,icon);
			Image image = GetImage(path);
			
			return image;
		}
		
		
		public string GetSimilar(string type, string name)
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
				
				if (name.StartsWith("c_dogwolf")) return "c_wolf";
				if (name.StartsWith("n_wolf")) return "c_wolf";
				if (name.StartsWith("c_beardire")) return "c_direbear";
				if (name.StartsWith("c_boardire")) return "c_boar";
				
				if (name.StartsWith("c_lizman")) return "c_lizman";
				if (name.StartsWith("c_summ_balor")) return "c_balor";
				if (name.StartsWith("c_orc")) return "c_orc";
				if (name.StartsWith("c_summ_imp")) return "c_imp";
				if (name.StartsWith("c_fiendrat")) return "c_fiendrat";
				if (name.StartsWith("c_ratdire")) return "c_fiendrat";
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
	}
}
