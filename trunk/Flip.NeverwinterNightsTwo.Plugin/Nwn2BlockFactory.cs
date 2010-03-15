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
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using NWN2Toolset.NWN2.Data;
using NWN2Toolset.NWN2.Data.Blueprints;
using NWN2Toolset.NWN2.Data.Instances;
using NWN2Toolset.NWN2.Data.Templates;
using Sussex.Flip.Games.NeverwinterNightsTwo.Utils;
using Sussex.Flip.UI;

namespace Sussex.Flip.Games.NeverwinterNightsTwo
{
	/// <summary>
	/// Description of Nwn2BlockFactory.
	/// </summary>
	public class Nwn2BlockFactory
	{		
		protected string pathFormat;
		
		
		// TODO:
		// ObjectBlock should have a generic image to use (as an embedded resource)
		// if the image it's given is rubbish (or it isn't given one)
		// (should also have a parameterless constructor?)
		
		
		public Nwn2BlockFactory()
		{
			pathFormat = @"C:\Flip\object pics\{0}\{1}.bmp";
		}
		
		
		public ObjectBlock CreatePlayerBlock()
		{
			Image image = GetImage("Other","Player");
			if (image == null) image = GetImage("Placeholder","Default");	
			return new ObjectBlock(image,null,null,"Player",null,"Player");
		}
		
		
		public ObjectBlock CreateModuleBlock()
		{
			Image image = GetImage("Other","Module");
			if (image == null) image = GetImage("Placeholder","Default");
			return new ObjectBlock(image,null,null,"Module",null,"Module");
		}
		
		
		public ObjectBlock CreateTypeBlock(Nwn2Type type)
		{
			string t = type.ToString();
			string filename = String.Format("Type_{0}",t);
			Image image = GetImage("Other","Type");//filename);
			if (image == null) image = GetImage("Placeholder","Default");
			return new ObjectBlock(image,null,null,"Type",t,t);
		}
		
		
		public ObjectBlock CreateAreaBlock(NWN2GameArea area)
		{
			string terrain = area.HasTerrain ? "Exterior" : "Interior";
			string filename = String.Format("Area_{0}",terrain);
			Image image = GetImage("Other",filename);
			if (image == null) image = GetImage("Placeholder","Default");
			return new ObjectBlock(image,area,area.Name,"Area",terrain,area.Name);
		}
		
		
		public ObjectBlock CreateBlueprintBlock(INWN2Blueprint blueprint)
		{			
			// TODO: Check first for a picture of ResourceName.Value (the actual blueprint)
			// then for TemplateResRef.Value (the blueprint its based on).
			string objectType = blueprint.ObjectType.ToString();
			Image image = GetImage(objectType,blueprint.TemplateResRef.Value);		
			if (image == null) image = GetImage("Placeholder","Blueprint");//String.Format("Blueprint_{0}",objectType));
			ObjectBlock block = new ObjectBlock(image,blueprint,blueprint.ResourceName.Value,"Blueprint",objectType,blueprint.Name);
			return block;
		}
		
		
		public ObjectBlock CreateInstanceBlock(INWN2Instance instance)
		{			
			string objectType = instance.ObjectType.ToString();
			Image image = GetImage(objectType,instance.Template.ResRef.Value);
			if (image == null) image = GetImage("Placeholder","Instance");//String.Format("Instance_{0}",objectType));
			// TODO safety check:
			ObjectBlock block = new ObjectBlock(image,instance,((INWN2Object)instance).Tag,"Instance",objectType,instance.Name);
			return block;
		}
		
		
		public ObjectBlock CreateInstanceBlock(List<INWN2Instance> instances)
		{
			// TODO safety check:
			INWN2Instance instance = instances[0];
			string objectType = instance.ObjectType.ToString();
			Image image = GetImage(objectType,instance.Template.ResRef.Value);
			if (image == null) image = GetImage("Placeholder","Instance");//String.Format("Instance_{0}",objectType));
			ObjectBlock block = new ObjectBlock(image,instances,((INWN2Object)instance).Tag,"Instance",objectType,instances[0].Name);
			return block;
		}
		
		
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
		
		
		protected Image GetImage(string type, string name)
		{
			string path = String.Format(pathFormat,type,name);
			return GetImage(path);
		}
	}
}
