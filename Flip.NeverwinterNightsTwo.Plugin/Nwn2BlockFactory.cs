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
 * This file added by Keiron Nicholson on 15/02/2010 at 14:54.
 */

using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using NWN2Toolset.NWN2.Data;
using NWN2Toolset.NWN2.Data.Blueprints;
using NWN2Toolset.NWN2.Data.Instances;
using Sussex.Flip.Games.NeverwinterNightsTwo.Utils;
using Sussex.Flip.UI;

namespace Sussex.Flip.Games.NeverwinterNightsTwo
{
	/// <summary>
	/// Description of Nwn2BlockFactory.
	/// </summary>
	public class Nwn2BlockFactory : AbstractNwn2BlockFactory
	{		
		protected string pathFormat;
		protected string placeholderPath;
		
		
		public Nwn2BlockFactory()
		{
			pathFormat = @"C:\Flip\object pics\{0}\{1}.bmp";
			placeholderPath = String.Format(pathFormat,"Other","NWN2LogoSmall"); // TODO handle filenotfound/path error
		}
		
		
		public override ObjectBlock CreatePlayerBlock()
		{
			return new ObjectBlock(GetImage(placeholderPath),null,null,"player",null,"Player");
		}
		
		
		public override ObjectBlock CreateModuleBlock()
		{
			return new ObjectBlock(GetImage(placeholderPath),null,null,"module",null,"Module");
		}
		
		
		public override ObjectBlock CreateTypeBlock(Nwn2Type type)
		{
			string typeString = type.ToString();
			return new ObjectBlock(GetImage(placeholderPath),null,null,"type",typeString,typeString);
		}
		
		
		public override ObjectBlock CreateAreaBlock(NWN2GameArea area)
		{
			return new ObjectBlock(GetImage(placeholderPath),area,area.Name,"area",(area.HasTerrain ? "exterior" : "interior"),area.Name);
		}
		
		
		public override ObjectBlock CreateBlueprintBlock(INWN2Blueprint blueprint)
		{			
			Image image;
			string imagePath = String.Format(pathFormat,blueprint.ObjectType.ToString(),blueprint.TemplateResRef.Value);
			
			if (System.IO.File.Exists(imagePath)) {
				image = GetImage(imagePath);
			}
			else {
				image = GetImage(placeholderPath);
			}
			
			return new ObjectBlock(image,blueprint,blueprint.ResourceName.Value,"blueprint",blueprint.ObjectType.ToString(),blueprint.Name);
		}
		
		
		public override ObjectBlock CreateInstanceBlock(INWN2Instance instance)
		{
			return new ObjectBlock(GetImage(placeholderPath),instance,((NWN2Toolset.NWN2.Data.Templates.INWN2Object)instance).Tag,"instance",instance.ObjectType.ToString(),instance.Name);
		}
		
		
		public override ObjectBlock CreateInstanceBlock(List<INWN2Instance> instances)
		{
			return new ObjectBlock(GetImage(placeholderPath),instances,((NWN2Toolset.NWN2.Data.Templates.INWN2Object)instances[0]).Tag,"instance",instances[0].ObjectType.ToString(),instances[0].Name);
		}
		
		
		protected Image GetImage(string path)
		{			
			Image image = new Image();
			Uri uri = new Uri(path);
			BitmapImage bmp = new BitmapImage(uri);
			image.Source = bmp;
			return image;
		}
	}
}
