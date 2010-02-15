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

namespace Sussex.Flip.Games.NeverwinterNightsTwo
{
	/// <summary>
	/// Description of Nwn2NounFactory.
	/// </summary>
	public class Nwn2NounFactory : AbstractNwn2NounFactory
	{		
		private Image placeholder;
		
		
		public Nwn2NounFactory()
		{
			placeholder = new Image();
			Uri uri = new Uri(@"C:\Flip\object pics\NWN2LogoSmall.jpg");
			BitmapImage bmp = new BitmapImage(uri);
			placeholder.Source = bmp;
		}
		
		
		public override Nwn2Noun CreatePlayerBlock()
		{
			Image image = placeholder;
			return new Nwn2Noun(placeholder,Nwn2EventRaiser.Area);//TODO
		}
		
		
		public override Nwn2Noun CreateModuleBlock()
		{
			Image image = placeholder;
			return new Nwn2Noun(placeholder,Nwn2EventRaiser.Module);
		}
		
		
		public override Nwn2Noun CreateTypeBlock(Nwn2EventRaiser type)
		{
			Image image = placeholder;
			return new TypeBlock(image,type);
		}
		
		
		public override Nwn2Noun CreateAreaBlock(NWN2GameArea area)
		{
			Image image = placeholder;
			return new AreaBlock(image,area);
		}
		
		
		public override Nwn2Noun CreateBlueprintBlock(INWN2Blueprint blueprint)
		{
			Image image = placeholder;
			return new BlueprintBlock(image,blueprint);
		}
		
		
		public override Nwn2Noun CreateInstanceBlock(INWN2Instance instance)
		{
			Image image = placeholder;
			return new InstanceBlock(image,instance);
		}
		
		
		public override Nwn2Noun CreateInstanceBlock(List<INWN2Instance> instances)
		{
			Image image = placeholder;
			return new InstanceBlock(image,instances);
		}
	}
}
