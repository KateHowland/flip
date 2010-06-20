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
 * This file added by Keiron Nicholson on 12/06/2010 at 18:02.
 */

using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Sussex.Flip.UI;
using Sussex.Flip.Games.NeverwinterNightsTwo.Utils;
using NWN2Toolset.NWN2.Data.Templates;

namespace Sussex.Flip.Games.NeverwinterNightsTwo.Behaviours
{
	/// <summary>
	/// Description of Instance.
	/// </summary>
	public class WildcardBehaviour : InstanceBehaviour
	{
		public WildcardBehaviour() : this(String.Empty)
		{			
		}
		
		
		public WildcardBehaviour(string tag)
		{
			if (tag == null) throw new ArgumentNullException("tag");
			
			Tag = tag;
			ResRef = tag;
			DisplayName = tag;
			AreaTag = String.Empty;
			type = Nwn2Type.Prefab;
			IconName = null;
		}
		
		
		public override ObjectBehaviour DeepCopy()
		{
			return new WildcardBehaviour(Tag);
		}
		
		
		public override string GetDescriptionOfObjectType()
		{
			return Nwn2Fitter.WildcardDescription;
		}
		
		
		public override bool Equals(ObjectBehaviour other)
		{	
			if (!base.Equals(other)) return false;
						
			WildcardBehaviour w = other as WildcardBehaviour;
			
			return w != null && w.Tag == this.Tag;
		}
		
		
		public override string ToString()
		{
			return String.Format("Wildcard (\"{0}\")",Tag);
		}
		
		
		public override string GetLogText()
		{
			return "Wildcard (Tag: " + Tag + ")";
		}
	}
}