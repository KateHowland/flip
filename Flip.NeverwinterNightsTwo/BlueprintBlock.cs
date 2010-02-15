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
 * This file added by Keiron Nicholson on 15/02/2010 at 15:03.
 */

using System;
using System.Windows.Controls;
using NWN2Toolset.NWN2.Data.Blueprints;
using OEIShared.Utils;
using Sussex.Flip.UI;
using Sussex.Flip.Games.NeverwinterNightsTwo.Utils;

namespace Sussex.Flip.Games.NeverwinterNightsTwo
{
	/// <summary>
	/// Description of BlueprintBlock.
	/// </summary>
	public class BlueprintBlock : Nwn2Noun
	{
		protected INWN2Blueprint blueprint;
		protected OEIResRef resourceName;
		protected OEIResRef templateResRef;
		
		
		public INWN2Blueprint Blueprint {
			get { return blueprint; }
		}
		
		
		public OEIResRef ResourceName {
			get { 
				if (blueprint == null) throw new InvalidOperationException("BlueprintBlock has no blueprint.");
				else return blueprint.ResourceName; 
			}
		}
		
		
		public OEIResRef TemplateResRef {
			get { 
				if (blueprint == null) throw new InvalidOperationException("BlueprintBlock has no blueprint.");
				else return blueprint.TemplateResRef; 
			}
		}		
		
		
		public BlueprintBlock(Image image, INWN2Blueprint blueprint) : base(image,blueprint.ObjectType)
		{
			this.blueprint = blueprint;
		}
	}
}
