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
	public class Instance : ObjectBehaviour
	{
		protected NWN2ObjectType type;
		
		
		public Instance(string tag, string displayName, NWN2ObjectType type) : base(tag,displayName)
		{		
			this.type = type;
		}
		
		
		public override string GetCode()
		{			
			// MAP 3/28/2009
			// Efficient search for finding an object of a given tag and type combination.
			// nObjectType must be a proper OBJECT_TYPE const.
			// Returns OBJECT_INVALID if no match is found.
			// object GetObjectByTagAndType(string sTag, int nObjectType, int nTh);
			
			string sTag = Identifier;
			NWScriptObjectType? nwScriptType = NWScriptHelper.GetNWScriptConstant(type);
			
			if (nwScriptType == null) return "INVALID";				
				
			int nObjectType = (int)nwScriptType;
			int nTh = 0; // currently assuming we are referring to only the first object found with a given tag
			
			return String.Format("GetObjectByTagAndType({0},{1},{2})",sTag,nObjectType,nTh);
		}
		
		
		public override string GetNaturalLanguage()
		{
			return DisplayName;
		}
		
		
		public override ObjectBehaviour DeepCopy()
		{
			return new Instance(Identifier,DisplayName,type);
		}
		
		
		public override string GetDescriptionOfObjectType()
		{
			return String.Format("Nwn2.Instance.{0}",type);
		}
	}
}
