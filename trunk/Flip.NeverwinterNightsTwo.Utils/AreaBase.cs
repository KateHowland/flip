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
 * This file added by Keiron Nicholson on 14/08/2009 at 14:14.
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.DirectX;
using NWN2Toolset.NWN2.Data;
using NWN2Toolset.NWN2.Data.Blueprints;
using NWN2Toolset.NWN2.Data.Instances;
using NWN2Toolset.NWN2.Data.Templates;
using OEIShared.OEIMath;

namespace Sussex.Flip.Games.NeverwinterNightsTwo.Utils
{
	/// <summary> 
	/// A facade for a Neverwinter Nights 2 area.
	/// </summary>
	public abstract class AreaBase
	{		
		#region Fields

		/// <summary>
		/// The wrapped Neverwinter Nights 2 area.
		/// </summary>
		protected NWN2GameArea nwn2Area;

		#endregion
		
		#region Properties

		/// <summary>
		/// The smallest possible height/width of an
		/// area. For interior areas, this is the number
		/// of tiles. For exterior areas, this is the 
		/// number of small grid squares, excluding
		/// the inaccessible outer region.
		/// </summary>
		/// <remarks>Although interior and exterior
		/// areas share the same minimum and maximum
		/// length boundaries, a 10x10 exterior area
		/// is much larger than a 10x10 interior area.
		/// Exterior area lengths are automatically
		/// rounded off to the nearest acceptable length
		/// (4, 8, 12, 16, 20, 24, 28 or 32).</remarks>
		public const byte MinimumAreaLength = 1;


		/// <summary>
		/// The largest possible height/width of an
		/// area. For interior areas, this is the number
		/// of tiles. For exterior areas, this is the 
		/// number of small grid squares, excluding
		/// the inaccessible outer region.
		/// </summary>
		/// <remarks>Although interior and exterior
		/// areas share the same minimum and maximum
		/// length boundaries, a 10x10 exterior area
		/// is much larger than a 10x10 interior area.
		/// Exterior area lengths are automatically
		/// rounded off to the nearest acceptable length
		/// (4, 8, 12, 16, 20, 24, 28 or 32).</remarks>
		public const byte MaximumAreaLength = 8;
		

		/// <summary>
		/// The wrapped Neverwinter Nights 2 area.
		/// </summary>
		public NWN2GameArea Nwn2Area { 
			get { return nwn2Area; }
		}


		/// <summary>
		/// The width of the portion of this area
		/// which is not accessible (cannot be reached
		/// by the player or other game creatures).
		/// </summary>
		public byte InaccessibleRegionWidth {
			get {
				try {
					if (nwn2Area.HasTerrain) return 80; 					
					else return 0;
				}
				catch (NullReferenceException) {
					throw new InvalidOperationException("Missing area.");
				}
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Gets the region of this area which is
		/// accessible to the player and other creatures.
		/// </summary>
		/// <returns>
		/// A <see cref="OEIShared.OEIMath.BoundingBox3"/> instance
		/// representing the boundaries of the player-accessible region.
		/// </returns>
		/// <remarks>Interior areas have no inaccessible region,
		/// and will return the bounds of the full area.</remarks>
		public abstract BoundingBox3 GetAccessibleRegion();


		/// <summary>
		/// Gets a random position in the area.
		/// </summary>
		/// <param name="">True to return a position within the
		/// accessible region only; false to ignore this distinction.</param>
		/// <returns>A random position within the area.</returns>
		/// <remarks>Assumes the area to be a flat plane - the 
		/// Z field of the returned <see cref="Vector3"/>
		/// will always be 0.</remarks>
		public abstract Vector3 GetRandomPosition(bool accessible);


		/// <summary>
		/// Creates an instance of a blueprint and adds it to the area.
		/// </summary>
		/// <param name="blueprint">The blueprint to create the game
		/// object from.</param>
		/// <param name="tag">The tag to give the newly-created object.
		/// Pass null to use the default tag.</param>
		/// <returns>The newly-created game object.</returns>
		public abstract INWN2Instance AddGameObject(INWN2Blueprint blueprint, string tag);


		/// <summary>
		/// Creates an instance of a blueprint and adds it to the area.
		/// </summary>
		/// <param name="blueprint">The blueprint to create the game
		/// object from.</param>
		/// <param name="tag">The tag to give the newly-created object.
		/// Pass null to use the default tag.</param>
		/// <param name="position">The position within the area to place 
		/// the object.</param>
		/// <returns>The newly-created game object.</returns>
		public abstract INWN2Instance AddGameObject(INWN2Blueprint blueprint, string tag, Vector3? position);
				
		
		/// <summary>
		/// Creates an instance of a blueprint and adds it to the area.
		/// </summary>
		/// <param name="type">The type of object to add.</param>
		/// <param name="resref">The resref of the blueprint to create the object from.</param>
		/// <param name="tag">The tag to give the newly-created object.
		/// Pass null to use the default tag.</param>
		/// <returns>The newly-created game object.</returns>
		public abstract INWN2Instance AddGameObject(NWN2ObjectType type, string resref, string tag);
			
			
		/// <summary>
		/// Creates an instance of a blueprint and adds it to the area.
		/// </summary>
		/// <param name="type">The type of object to add.</param>
		/// <param name="resref">The resref of the blueprint to create the object from.</param>
		/// <param name="tag">The tag to give the newly-created object.
		/// Pass null to use the default tag.</param>
		/// <param name="position">The position within the area to place 
		/// the object.</param>
		/// <returns>The newly-created game object.</returns>
		public abstract INWN2Instance AddGameObject(NWN2ObjectType type, string resref, string tag, Vector3? position);
		
		
		/// <summary>
		/// Gets the number of objects in this area matching a given description.
		/// </summary>
		/// <param name="type">The type of objects to count.</param>
		/// <param name="tag">Only objects with this tag will be counted.
		/// Pass null to ignore this criterion.</param>
		/// <returns>The objects matching the given description.</returns>
		public abstract List<INWN2Instance> GetObjects(NWN2ObjectType type, string tag);
		
		
		/// <summary>
		/// Saves the module.
		/// </summary>
		public abstract void Save();

		#endregion
	}
}