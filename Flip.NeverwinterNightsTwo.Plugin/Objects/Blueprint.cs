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
	/// Description of Blueprint.
	/// </summary>
	public class Blueprint : Nwn2ObjectBehaviour
	{		
		protected NWN2ObjectType type;
		
		
		public string ResRef {
			get { return Identifier; }
			set { Identifier = value; }
		}
		
		
		protected Blueprint() : this(string.Empty,string.Empty,NWN2ObjectType.Light)
		{			
		}
		
		
		public Blueprint(string resRef, string displayName, NWN2ObjectType type) : base(resRef,displayName)
		{		
			this.type = type;
		}
		
		
		public override string GetCode()
		{			
			/* Not clear on when you would actually use a blueprint in NWScript,
			 * but in the only places I can find reference to it they seem to just
			 * use the ResRef. */
			
			return String.Format("\"{0}\"",ResRef);
		}
		
		
		public override string GetNaturalLanguage()
		{
			return String.Format("blueprint {0}",DisplayName);
		}
		
		
		public override ObjectBehaviour DeepCopy()
		{
			return new Blueprint(Identifier,DisplayName,type);
		}
		
		
		public override string GetDescriptionOfObjectType()
		{
			return String.Format(Nwn2Fitter.SubtypeFormat,Nwn2Fitter.BlueprintDescription,type);
		}
		
		
		public override Nwn2Type GetNwn2Type()
		{
			return Nwn2ScriptSlot.GetNwn2Type(type);
		}
	}
}
