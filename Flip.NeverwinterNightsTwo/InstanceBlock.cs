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
 * This file added by Keiron Nicholson on 15/02/2010 at 15:25.
 */

using System;
using System.Collections.Generic;
using System.Windows.Controls;
using NWN2Toolset.NWN2.Data;
using NWN2Toolset.NWN2.Data.Instances;
using NWN2Toolset.NWN2.Data.Templates;
using OEIShared.Utils;
using Sussex.Flip.UI;
using Sussex.Flip.Games.NeverwinterNightsTwo.Utils;

namespace Sussex.Flip.Games.NeverwinterNightsTwo
{
	/// <summary>
	/// Description of InstanceBlock.
	/// </summary>
	public class InstanceBlock : Nwn2Noun
	{
		protected List<INWN2Instance> instances;
		
		public List<INWN2Instance> Instances {
			get { return instances; }
		}
		
		
		public string Nwn2Tag {
			get {
				if (instances == null || instances.Count == 0) throw new InvalidOperationException("InstanceBlock has no instances.");
				else if (!(instances[0] is INWN2Object)) throw new InvalidOperationException("Instance was not of type INWN2Object.");
				else return ((INWN2Object)instances[0]).Tag;
			}
		}
		
		
		public int Count {
			get {
				if (instances == null) throw new InvalidOperationException("InstanceBlock has no instances.");
				else return instances.Count;
			}
		}
		
		
		public NWN2GameArea Area {
			get {
				if (instances == null) throw new InvalidOperationException("InstanceBlock has no instances.");
				else return instances[0].Area;
			}
		}
		
		
		public InstanceBlock(Image image, INWN2Instance instance) : base(image,instance.ObjectType)
		{
			this.instances = new List<INWN2Instance>{instance};
		}
		
		
		public InstanceBlock(Image image, List<INWN2Instance> instances) : base(image,instances[0].ObjectType)
		{
			this.instances = instances;
		}
	}
}
