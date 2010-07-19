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
 * This file added by Keiron Nicholson on 09/07/2010 at 10:33.
 */

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Sussex.Flip.Games.NeverwinterNightsTwo.Images
{
	/// <summary>
	/// Description of ImageGetter.
	/// </summary>
	public class ImageGetter
	{
//		protected static ResourceDictionary images;
		
		
		static ImageGetter()
		{
//			images = new ResourceDictionary();
//			images.Add("c_elf",GetImage(new Uri("Pictures/Creature/c_elf.bmp", UriKind.Relative)));
		}
		
		
		public Image GetImage(string type, string name)
		{
			return GetImage(type,name,true);
		}
		
		
		public Image GetImage(string type, string name, bool similar)
		{
			if (String.IsNullOrEmpty(type) || String.IsNullOrEmpty(name)) return null;
			
			if (similar) SubstituteSimilar(ref type, ref name);		
		
			Uri uri = new Uri(String.Format("pack://application:,,,/Flip.NeverwinterNightsTwo.Images;component/Pictures/{0}/{1}.bmp",type,name),UriKind.Absolute);
						
			//Uri uri = new Uri(String.Format("Pictures/{0}/{1}.bmp",type,name),UriKind.Relative);
			
			return GetImage(uri);
		}
		
		
		public Image GetImage(Uri uri)
		{
			try {
				Image image = new Image();	
				
				BitmapImage bmp = new BitmapImage();
				bmp.BeginInit();
				bmp.UriSource = uri;
				bmp.EndInit();
				
				image.Source = bmp;				
				return image;
				
			}
			catch (Exception x) {
				MessageBox.Show("Failed to get Image for Uri '" + uri + "'.\n\n" + x);
				return null;
			}
		}
		
		
		protected void SubstituteSimilar(ref string type, ref string name)
		{
			if (type == "Creature") {
				if (name.StartsWith("c_elmwater")) name = "c_elmwater";
				if (name.StartsWith("c_elmfire")) name = "c_elmfire";
				if (name.StartsWith("c_elmearth")) name = "c_elmearth";
				if (name.StartsWith("c_elmair")) name = "c_elmair";
				
				if (name.StartsWith("c_ancom_badger")) name = "c_badger";
				if (name.StartsWith("c_ancom_bear")) name = "c_bear";
				if (name.StartsWith("c_ancom_boar")) name = "c_boar";
				if (name.StartsWith("c_ancom_spider")) name = "c_spider";
				if (name.StartsWith("c_ancom_wolf")) name = "c_wolf";
				
				if (name.StartsWith("c_fam_bat")) name = "c_bat";
				if (name.StartsWith("c_fam_beetle")) name = "c_beetle";
				if (name.StartsWith("c_fam_cat")) name = "c_cat";
				if (name.StartsWith("c_fam_pig")) name = "c_pig";
				if (name.StartsWith("c_fam_rabbit")) name = "c_rabbit";
				if (name.StartsWith("c_fam_rat")) name = "c_rat";
				if (name.StartsWith("c_fam_spider")) name = "c_spider";
				if (name.StartsWith("c_fam_weasel")) name = "c_weasel";
				
				if (name.StartsWith("c_beetlestag")) name = "c_beetle";
				if (name.StartsWith("c_faction_pig")) name = "c_pig";
				
				if (name.StartsWith("c_dogwolf")) name = "c_wolf";
				if (name.StartsWith("n_wolf")) name = "c_wolf";
				if (name.StartsWith("c_beardire")) name = "c_direbear";
				if (name.StartsWith("c_boardire")) name = "c_boar";
				
				if (name.StartsWith("c_lizman")) name = "c_lizman";
				if (name.StartsWith("c_summ_balor")) name = "c_balor";
				if (name.StartsWith("c_orc")) name = "c_orc";
				if (name.StartsWith("c_summ_imp")) name = "c_imp";
				if (name.StartsWith("c_fiendrat")) name = "c_fiendrat";
				if (name.StartsWith("c_ratdire")) name = "c_fiendrat";
			}
			
			else if (type == "Door") {
				name = "plc_dt_doorsc2";
			}		
			
			else if (type == "Placeholder") {
				if (name == "Instance") name = "ia_sacredvengeance";
				if (name == "Blueprint") name = "ia_sacredvengeance";
			}
			
			else if (type == "Other") {
				if (name == "Player") name = "ig_hu_redknight";
				if (name == "Module") name = "ig_hu_grumbar";
			}
		}
	}
}
