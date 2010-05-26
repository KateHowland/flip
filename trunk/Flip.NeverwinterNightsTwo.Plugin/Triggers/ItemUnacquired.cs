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
 * This file added by Keiron Nicholson on 26/05/2010 at 13:59.
 */

using System;
using System.Xml;
using Sussex.Flip.UI;
using Sussex.Flip.Games.NeverwinterNightsTwo.Behaviours;
using Sussex.Flip.Games.NeverwinterNightsTwo.Utils;

namespace Sussex.Flip.Games.NeverwinterNightsTwo
{
	/// <summary>
	/// Description of ItemUnacquired.
	/// </summary>
	public class ItemUnacquired : Nwn2ParameterlessTrigger
	{
		public ItemUnacquired() : base("When an item is lost")			
		{ 
		}
					
		
		public override string GetNaturalLanguage()
		{
			return "Whenever somebody loses an item";
		}
		
		
		public override string GetAddress()
		{
			return addressFactory.GetModuleAddress("OnUnacquireItem").Value;
		}
		
		
		public override void ReadXml(XmlReader reader)
		{
			throw new NotImplementedException();
		}
		
		
		public override void WriteXml(XmlWriter writer)
		{
			throw new NotImplementedException();
		}
		
		
		public override Moveable DeepCopy()
		{
			return new ItemUnacquired();
		}
	}
}
