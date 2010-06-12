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
 * This file added by Keiron Nicholson on 01/04/2010 at 16:03.
 */

using System;
using System.Collections.Generic;
using Sussex.Flip.UI;
using Sussex.Flip.Games.NeverwinterNightsTwo.Behaviours;
using NWN2Toolset.NWN2.Data.Templates;

namespace Sussex.Flip.Games.NeverwinterNightsTwo
{
	/// <summary>
	/// Description of Nwn2Fitter.
	/// </summary>
	public abstract class Nwn2Fitter : Fitter
	{
		#region Constants
		
		/// <summary>
		/// The description of an ObjectBlock representing the player.
		/// </summary>
		public const string PlayerDescription = "Nwn2.Player";
		
		/// <summary>
		/// The description of an ObjectBlock representing the module.
		/// </summary>
		public const string ModuleDescription = "Nwn2.Module";
		
		/// <summary>
		/// The description of an ObjectBlock representing an area.
		/// </summary>
		public const string AreaDescription = "Nwn2.Area";
		
		/// <summary>
		/// The description of an ObjectBlock representing a blueprint.
		/// </summary>
		public const string BlueprintDescription = "Nwn2.Blueprint";
		
		/// <summary>
		/// The description of an ObjectBlock representing an instance.
		/// </summary>
		public const string InstanceDescription = "Nwn2.Instance";
		
		/// <summary>
		/// The description of an ObjectBlock representing a wildcard.
		/// </summary>
		public const string WildcardDescription = "Nwn2.Wildcard";
		
		/// <summary>
		/// The format for specifying a subtype of an instance or blueprint.
		/// </summary>
		public const string SubtypeFormat = "{0}.{1}";
				
		/// <summary>
		/// A list of NWN2ObjectTypes which raise events.
		/// </summary>
		protected static readonly List<NWN2ObjectType> eventRaisers;
		
		#endregion
		
		#region Constructors
		
		/// <summary>
		/// Initialises the list of names of Nwn2ObjectTypes which raise events.
		/// </summary>
		static Nwn2Fitter()
		{
			eventRaisers = new List<NWN2ObjectType> {
				NWN2ObjectType.Creature,
				NWN2ObjectType.Door,
				NWN2ObjectType.Encounter,
				NWN2ObjectType.Placeable,
				NWN2ObjectType.Store,
				NWN2ObjectType.Trigger};
		}
		
		#endregion
		
		#region Methods
		
		/// <summary>
		/// Gets whether a given Moveable is a wildcard.
		/// </summary>
		/// <param name="moveable">The Moveable to check.</param>
		/// <returns>True if the Moveable is a wildcard; false otherwise.</returns>
		public static bool IsWildcard(Moveable moveable)
		{
			if (moveable == null) throw new ArgumentNullException("moveable");
			
			ObjectBlock block = moveable as ObjectBlock;
			return block != null && block.GetDescriptionOfObjectType() == WildcardDescription;
		}
		
		
		/// <summary>
		/// Gets whether a given Moveable represents the player.
		/// </summary>
		/// <param name="moveable">The Moveable to check.</param>
		/// <returns>True if the Moveable represents the player; false otherwise.</returns>
		public static bool IsPlayer(Moveable moveable)
		{
			if (moveable == null) throw new ArgumentNullException("moveable");
			
			ObjectBlock block = moveable as ObjectBlock;
			return block != null && block.GetDescriptionOfObjectType() == PlayerDescription;
		}
		
		
		/// <summary>
		/// Gets whether a given Moveable represents the module.
		/// </summary>
		/// <param name="moveable">The Moveable to check.</param>
		/// <returns>True if the Moveable represents the module; false otherwise.</returns>
		public static bool IsModule(Moveable moveable)
		{
			if (moveable == null) throw new ArgumentNullException("moveable");
			
			ObjectBlock block = moveable as ObjectBlock;
			return block != null && block.GetDescriptionOfObjectType() == ModuleDescription;
		}
		
		
		/// <summary>
		/// Gets whether a given Moveable represents an area.
		/// </summary>
		/// <param name="moveable">The Moveable to check.</param>
		/// <returns>True if the Moveable represents an area; 
		/// false otherwise.</returns>
		public static bool IsArea(Moveable moveable)
		{
			if (moveable == null) throw new ArgumentNullException("moveable");
			
			ObjectBlock block = moveable as ObjectBlock;
			return block != null && block.GetDescriptionOfObjectType() == AreaDescription;
		}
		
		
		/// <summary>
		/// Gets whether a given Moveable represents an instance.
		/// </summary>
		/// <param name="moveable">The Moveable to check.</param>
		/// <returns>True if the Moveable represents an instance; 
		/// false otherwise.</returns>
		public static bool IsInstance(Moveable moveable)
		{
			if (moveable == null) throw new ArgumentNullException("moveable");
			
			ObjectBlock block = moveable as ObjectBlock;
			return block != null && block.GetDescriptionOfObjectType().StartsWith(InstanceDescription);
		}
		
		
		/// <summary>
		/// Gets whether a given Moveable represents an instance of a particular type.
		/// </summary>
		/// <param name="moveable">The Moveable to check.</param>
		/// <param name="type">The type of instance we're looking for.</param>
		/// <returns>True if the Moveable represents an instance of the given type; 
		/// false otherwise.</returns>
		public static bool IsInstance(Moveable moveable, NWN2ObjectType type)
		{
			if (moveable == null) throw new ArgumentNullException("moveable");
			
			ObjectBlock block = moveable as ObjectBlock;
			string desc = String.Format(SubtypeFormat,InstanceDescription,type.ToString());
			                            
			return block != null && block.GetDescriptionOfObjectType() == desc;
		}
		
		
		/// <summary>
		/// Gets whether a given Moveable represents an instance of one of a number of
		/// given types.
		/// </summary>
		/// <param name="moveable">The Moveable to check.</param>
		/// <param name="types">The types of instance we're looking for.</param>
		/// <returns>True if the Moveable represents an instance of one of the given types; 
		/// false otherwise.</returns>
		public static bool IsInstance(Moveable moveable, List<NWN2ObjectType> types)
		{
			if (moveable == null) throw new ArgumentNullException("moveable");
			
			ObjectBlock block = moveable as ObjectBlock;
			if (block == null) return false;			
			
			foreach (NWN2ObjectType type in types) {
				string desc = String.Format(SubtypeFormat,InstanceDescription,type.ToString());
				if (block.GetDescriptionOfObjectType() == desc) {
					return true;
				}
			}
			                            
			return false;
		}
		
		
		/// <summary>
		/// Gets whether a given Moveable represents a blueprint.
		/// </summary>
		/// <param name="moveable">The Moveable to check.</param>
		/// <returns>True if the Moveable represents a blueprint; 
		/// false otherwise.</returns>
		public static bool IsBlueprint(Moveable moveable)
		{
			if (moveable == null) throw new ArgumentNullException("moveable");
			
			ObjectBlock block = moveable as ObjectBlock;
			return block != null && block.GetDescriptionOfObjectType().StartsWith(BlueprintDescription);
		}
		
		
		/// <summary>
		/// Gets whether a given Moveable represents a blueprint of a particular type.
		/// </summary>
		/// <param name="moveable">The Moveable to check.</param>
		/// <param name="type">The type of blueprint we're looking for.</param>
		/// <returns>True if the Moveable represents a blueprint of the given type; 
		/// false otherwise.</returns>
		public static bool IsBlueprint(Moveable moveable, NWN2ObjectType type)
		{
			if (moveable == null) throw new ArgumentNullException("moveable");
			
			ObjectBlock block = moveable as ObjectBlock;
			string desc = String.Format(SubtypeFormat,BlueprintDescription,type.ToString());
			                            
			return block != null && block.GetDescriptionOfObjectType() == desc;
		}
		
		
		/// <summary>
		/// Gets whether a given Moveable represents a blueprint of one of a number of
		/// given types.
		/// </summary>
		/// <param name="moveable">The Moveable to check.</param>
		/// <param name="types">The types of blueprint we're looking for.</param>
		/// <returns>True if the Moveable represents a blueprint of one of the given types; 
		/// false otherwise.</returns>
		public static bool IsBlueprint(Moveable moveable, List<NWN2ObjectType> types)
		{
			if (moveable == null) throw new ArgumentNullException("moveable");
			
			ObjectBlock block = moveable as ObjectBlock;
			if (block == null) return false;			
			
			foreach (NWN2ObjectType type in types) {
				string desc = String.Format(SubtypeFormat,BlueprintDescription,type.ToString());
				if (block.GetDescriptionOfObjectType() == desc) {
					return true;
				}
			}
			                            
			return false;
		}
				
		
		/// <summary>
		/// Gets whether a given Moveable represents a Neverwinter Nights 2 type
		/// which can raise events.
		/// </summary>
		/// <param name="block">The Moveable which may raise events.</param>
		/// <returns>True if the given Moveable represents a type
		/// which can raise events; false otherwise.</returns>
		public static bool CanRaiseEvents(Moveable moveable)
		{	
			if (moveable == null) throw new ArgumentNullException("moveable");
			
			return IsModule(moveable) || IsArea(moveable) || IsInstance(moveable,eventRaisers);
		}
		
		#endregion
	}
}
