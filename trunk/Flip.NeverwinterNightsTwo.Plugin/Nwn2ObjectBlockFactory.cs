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
using Sussex.Flip.UI;
using Sussex.Flip.Games.NeverwinterNightsTwo.Behaviours;
using Sussex.Flip.Games.NeverwinterNightsTwo.Utils;

namespace Sussex.Flip.Games.NeverwinterNightsTwo
{
	/// <summary>
	/// Description of Nwn2ObjectBlockFactory.
	/// </summary>
	public class Nwn2ObjectBlockFactory
	{		
		protected string pathFormat;
		
		
		public Nwn2ObjectBlockFactory()
		{
			pathFormat = @"C:\Flip\object pics\{0}\{1}.bmp";
		}
		
		
		public ObjectBlock CreatePlayerBlock()
		{
			Image image = GetImage("Other","Player");
			if (image == null) image = GetImage("Placeholder","Default");
			
			return new ObjectBlock(image,GetPlayerBlockBehaviour());
		}
		
		
		public ObjectBlock CreateModuleBlock()
		{
			Image image = GetImage("Other","Module");
			if (image == null) image = GetImage("Placeholder","Default");
			
			return new ObjectBlock(image,GetModuleBlockBehaviour());
		}
		
		
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
		
		
		public ObjectBlock CreateBlueprintBlock(INWN2Blueprint blueprint)
		{			
			if (blueprint == null) throw new ArgumentNullException("blueprint");
			
			throw new NotImplementedException();
			
//			// TODO: Check first for a picture of ResourceName.Value (the actual blueprint)
//			// then for TemplateResRef.Value (the blueprint its based on).
//			string objectType = blueprint.ObjectType.ToString();
//			Image image = GetImage(objectType,blueprint.TemplateResRef.Value);		
//			if (image == null) image = GetImage("Placeholder","Blueprint");//String.Format("Blueprint_{0}",objectType));
//			ObjectBlock block = new ObjectBlock(image,blueprint,blueprint.ResourceName.Value,"Blueprint",objectType,blueprint.Name);
//			return block;
		}
		
		
		public ObjectBlock CreateInstanceBlock(INWN2Instance instance)
		{						
			if (instance == null) throw new ArgumentNullException("instance");
			
			string resRef = instance.Template.ResRef.Value;
			
			Image image = GetImage(instance.ObjectType.ToString(),resRef);
			if (image == null) image = GetImage("Placeholder","Instance");// TODO: String.Format("Instance_{0}",objectType));
			
			try {
				string identifier = ((INWN2Object)instance).Tag;				
				ObjectBlock block = new ObjectBlock(image,GetInstanceBlockBehaviour(identifier,instance.Name,instance.ObjectType));
				return block;
			}
			catch (InvalidCastException e) {
				throw new ApplicationException("Creation of a visual block to represent " + instance.Name + 
				                               "failed. Could not cast to INWN2Object to retrieve tag.",e);
			}
		}
		
		
		public ObjectBlock CreateInstanceBlock(List<INWN2Instance> instances)
		{	
			if (instances == null) throw new ArgumentNullException("instances");
			
			throw new NotImplementedException();
			
//			// TODO safety check:
//			INWN2Instance instance = instances[0];
//			string objectType = instance.ObjectType.ToString();
//			Image image = GetImage(objectType,instance.Template.ResRef.Value);
//			if (image == null) image = GetImage("Placeholder","Instance");//String.Format("Instance_{0}",objectType));
//			ObjectBlock block = new ObjectBlock(image,instances,((INWN2Object)instance).Tag,"Instance",objectType,instances[0].Name);
//			return block;
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
		
		
		protected ObjectBehaviour GetPlayerBlockBehaviour()
		{
			return new Player();
		}
		
		
		protected ObjectBehaviour GetModuleBlockBehaviour()
		{
			return new Module();
		}
		
		
		protected ObjectBehaviour GetAreaBlockBehaviour(string tag, string displayName)
		{
			return new Area(tag,displayName);
		}
		
		
		protected ObjectBehaviour GetInstanceBlockBehaviour(string tag, string displayName, NWN2ObjectType type)
		{
			return new Instance(tag,displayName,type);
		}
		
		
		protected ObjectBehaviour GetBlueprintBlockBehaviour()
		{
			throw new NotImplementedException();
		}
	}
}
