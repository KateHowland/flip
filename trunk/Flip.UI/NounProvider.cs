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
 * This file added by Keiron Nicholson on 03/02/2010 at 16:00.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Sussex.Flip.UI
{
	/// <summary>
	/// Description of NounProvider.
	/// </summary>
	public abstract class NounProvider
	{
		public abstract List<Noun> GetNouns();
	}
	
	
	public class SampleNounProvider : NounProvider
	{
		private string location = null;
		
		public SampleNounProvider(string location)
		{			
			if (location == null) throw new ArgumentNullException("location");
			if (!Directory.Exists(location)) throw new DirectoryNotFoundException("No directory found at " + location);
			
			this.location = location;
		}
		
		
		public override List<Noun> GetNouns()
		{		
			if (!Directory.Exists(location)) throw new DirectoryNotFoundException("No directory found at " + location);
			
			string[] filenames = Directory.GetFiles(location,"*.bmp");
			
			List<Noun> nouns = new List<Noun>(filenames.Length);
			
			Uri uri;
			BitmapImage bmp;
			Image img;
			
			foreach (string filename in filenames) {
				uri = new Uri(filename);
				bmp = new BitmapImage(uri);
				img = new Image();
				img.Source = bmp;
				//img.Effect; TODO: explore				
				nouns.Add(new Noun(img));
			}
			
			return nouns;
		}
	}
}