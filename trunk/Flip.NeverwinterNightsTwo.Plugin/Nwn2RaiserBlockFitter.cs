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
 * This file added by Keiron Nicholson on 22/03/2010 at 09:37.
 */

using System;
using System.Collections.Generic;
using Sussex.Flip.Games.NeverwinterNightsTwo.Utils;
using Sussex.Flip.UI;

namespace Sussex.Flip.Games.NeverwinterNightsTwo
{
	/// <summary>
	/// TODO Description of Nwn2EventRaiserBlockFitter.
	/// </summary>
	public class Nwn2RaiserBlockFitter : Fitter
	{		
		#region Fields
		
		/// <summary>
		/// A list of names of Nwn2ObjectTypes which raise events.
		/// </summary>
		protected static readonly List<string> instanceEventRaisers;
		
		#endregion
		
		#region Constructors
		
		/// <summary>
		/// Initialises the list of names of Nwn2ObjectTypes which raise events.
		/// </summary>
		static Nwn2RaiserBlockFitter()
		{
			instanceEventRaisers = new List<string> {
				"Creature",
				"Door",
				"Encounter",
				"Placeable",
				"Store",
				"Trigger"};
		}
				

		/// <summary>
		/// Constructs a new <see cref="Sussex.Flip.Games.NeverwinterNightsTwo.Nwn2EventRaiserBlockFitter"/> instance.
		/// </summary>
		public Nwn2RaiserBlockFitter() : base()
		{			
		}
		
		#endregion
		
		#region Methods
		
		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="moveable"></param>
		/// <returns></returns>
		public override bool Fits(Moveable moveable)
		{
			ObjectBlock block = moveable as ObjectBlock;		
			if (block == null) return false;
			else return CanRaiseEvents(block);
		}
		
		
		/// <summary>
		/// Gets whether a given ObjectBlock represents a NWN2 type
		/// which can raise events.
		/// </summary>
		/// <param name="block">The ObjectBlock which may raise events.</param>
		/// <returns>True if the given ObjectBlock represents a NWN2 type
		/// which can raise events; false otherwise.</returns>
		public static bool CanRaiseEvents(ObjectBlock block)
		{	
			return block.Type == "Module" || block.Type == "Area" || (block.Type == "Instance" && instanceEventRaisers.Contains(block.Subtype));
		}
		
		
		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="block"></param>
		/// <returns></returns>
		public static Nwn2Type? GetNwn2Type(ObjectBlock block)
		{	
			if (block.Type == "Module") return Nwn2Type.Module;
			
			else if (block.Type == "Area") return Nwn2Type.Area;
			
			else if (block.Type == "Instance" && instanceEventRaisers.Contains(block.Subtype)) {
				try {
					return (Nwn2Type)Enum.Parse(typeof(Nwn2Type),block.Subtype,true);
				}
				catch (ArgumentException) {
					return null;
				}
			}
			
			else return null;
		}
		
		
		/// <summary>
		/// Gets a list of events which can be raised by a NWN2 type
		/// represented by this block.
		/// </summary>
		/// <param name="block">The ObjectBlock which may raise events.</param>
		/// <returns>A list of events which can be raised by a NWN2 type
		/// represented by this block, or an empty list if the block does 
		/// not represent an NWN2 type.</returns>
		public static IList<string> GetEvents(ObjectBlock block)
		{	
			if (block == null) throw new ArgumentNullException("block");
			
			if (CanRaiseEvents(block)) {
				Nwn2Type? type = GetNwn2Type(block);
				if (type != null) return Nwn2ScriptSlot.GetScriptSlotNames(type.Value);
				else return new List<string>();
			}
			else {
				return new List<string>();
			}
		}
		
		
		public override string GetMoveableDescription()
		{
			return "event raiser";
		}
		
		#endregion
	}
}