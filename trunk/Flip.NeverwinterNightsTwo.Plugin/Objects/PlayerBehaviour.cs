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
 * This file added by Keiron Nicholson on 01/04/2010 at 13:20.
 */

using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Sussex.Flip.Games.NeverwinterNightsTwo.Utils;
using Sussex.Flip.UI;

namespace Sussex.Flip.Games.NeverwinterNightsTwo.Behaviours
{
	/// <summary>
	/// Description of Player.
	/// </summary>
	public class PlayerBehaviour : Nwn2ObjectBehaviour
	{
		public const string NWScript_GetPlayer = "GetFirstPC()";
		
		
		protected static string behaviourType;
			
				
    	public override string BehaviourType { 
			get { return behaviourType; }
		}
			
		
		static PlayerBehaviour()
		{
			behaviourType = "Sussex.Flip.Games.NeverwinterNightsTwo.Behaviours.PlayerBehaviour";
		}
		
		
		public PlayerBehaviour() : base(String.Empty,"player")
		{						
		}
		
		
		public override Nwn2Type Nwn2Type {
			get {
				return Nwn2Type.Player;
			}
		}
		
		
		public override string GetCode()
		{
			return NWScript_GetPlayer;
		}
		
		
		public override string GetNaturalLanguage()
		{
			return "the player";
		}
		
		
		public override ObjectBehaviour DeepCopy()
		{
			return new PlayerBehaviour();
		}
		
		
		public override string GetDescriptionOfObjectType()
		{
			return Nwn2Fitter.PlayerDescription;
		}
		
		
		public override string ToString()
		{
			return GetNaturalLanguage();
		}
		
		
		public override void ReadXml(XmlReader reader)
		{
			reader.MoveToContent();
			
			if (!reader.IsEmptyElement) {
				throw new FormatException("Behaviour should not have a child.");
			}
			
			Identifier = reader["Identifier"];
			DisplayName = reader["DisplayName"];				                     
			reader.ReadStartElement();
		}
		
		
		public override void WriteXml(XmlWriter writer)
		{
			writer.WriteAttributeString("Identifier",Identifier);
			writer.WriteAttributeString("DisplayName",DisplayName);
		}
	}
}