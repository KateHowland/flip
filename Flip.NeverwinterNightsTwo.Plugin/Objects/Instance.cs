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
 * This file added by Keiron Nicholson on 01/04/2010 at 13:50.
 */

using System;
using Sussex.Flip.UI;
using Sussex.Flip.Games.NeverwinterNightsTwo.Utils;
using NWN2Toolset.NWN2.Data.Templates;

namespace Sussex.Flip.Games.NeverwinterNightsTwo.Behaviours
{
	/// <summary>
	/// Description of Instance.
	/// </summary>
	public class Instance : Nwn2ObjectBehaviour
	{
		protected NWN2ObjectType type;		
		protected string areaTag;
		
				
		public string Tag {
			get { return Identifier; }
		}
				
		public NWN2ObjectType Type {
			get { return type; }
		}
		
		public string AreaTag {
			get { return areaTag; }
		}
		
		
		public Instance(string tag, string displayName, NWN2ObjectType type, string areaTag) : base(tag,displayName)
		{		
			this.type = type;
			this.areaTag = areaTag;
		}
		
		
		public override string GetCode()
		{			
			// Previous used GetObjectByTagAndType(), but this function appears to be broken.
			
			// Get the nNth object with the specified tag.
			// - sTag
			// - nNth: the nth object with this tag may be requested
			// * Returns OBJECT_INVALID if the object cannot be found.
			// Note: The module cannot be retrieved by GetObjectByTag(), use GetModule() instead.
			//object GetObjectByTag(string sTag, int nNth=0);
			
			int nTh = 0; // currently assuming we are referring to only the first object found with a given tag
			
			return String.Format("GetObjectByTag(\"{0}\",{1})",Tag,nTh);
		}
		
		
		public override string GetNaturalLanguage()
		{
			return DisplayName;
		}
		
		
		public override ObjectBehaviour DeepCopy()
		{
			return new Instance(Identifier,DisplayName,type,areaTag);
		}
		
		
		public override string GetDescriptionOfObjectType()
		{
			return String.Format(Nwn2Fitter.SubtypeFormat,Nwn2Fitter.InstanceDescription,type);
		}
		
		
		public override Nwn2Type GetNwn2Type()
		{
			return Nwn2ScriptSlot.GetNwn2Type(type);
		}
	}
}
