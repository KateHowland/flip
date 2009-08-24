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
	/// Provides functionality relating to areas in 
	/// a Neverwinter Nights 2 game module.
	/// </summary>
	public static class Nwn2AreaFunctions
	{
		#region Constants
		
		/// <summary>
		/// In an exterior area, the distance from any edge
		/// of the player-walkable central region 
		/// to the corresponding edge of the game area itself,
		/// expressed in the units used by 
		/// <see cref="OEIShared.OEIMath.BoundingBox3"/>.
		/// </summary>
		/// <remarks>
		/// The region inbetween these two edges is inaccessible to
		/// moving objects, including the player. This inaccessible 
		/// region is present in exterior areas only.
		/// </remarks>
		public const int InaccessibleTerrainExtent = 80;
		
		
		/// <summary>
		/// The minimum allowable height/width of an area,
		/// expressed in the units used by
		/// <see cref="System.Drawing.Size"/>.
		/// </summary>
		public const int MinimumAreaLength = 8;
		
		
		/// <summary>
		/// The minimum allowable height/width of an area,
		/// expressed in the units used by
		/// <see cref="System.Drawing.Size"/>.
		/// </summary>
		public const int MaximumAreaLength = 16;
		
		#endregion
		
		#region Methods
				
		/// <summary>
		/// Creates a Neverwinter Nights 2 game area.
		/// </summary>
		/// <param name="module">The module to create the area within.</param>
		/// <param name="name">The name to give the area.</param>
		/// <param name="exterior">True to create an exterior area
		/// with terrain; false to create an interior area with tiles.</param>
		/// <returns>An empty Neverwinter Nights 2 area.</returns>
		public static NWN2GameArea CreateArea(ref NWN2GameModule module, 
		                                      string name, 
		                                      bool exterior,
		                                      Size size)
		{
			if (module == null) 
				throw new ArgumentNullException("module","Can only create an area within a module.");
		
			// TODO: Check that module.Repository.Name works for both types of module:
			NWN2GameArea area = new NWN2GameArea(name,
			                                     module.Repository.Name,
			                                     module.Repository);
			area.Tag = name;
			area.HasTerrain = exterior;
			area.Size = size;
			
			module.AddResource(area);
			
			area.OEISerialize();
			
			return area;
		}
		
		
		/// <summary>
		/// Get the region of a given game area which is
		/// accessible to the player and other creatures.
		/// </summary>
		/// <param name="area">The game area from which to retrieve
		/// the accessible region.</param>
		/// <returns>
		/// A <see cref="OEIShared.OEIMath.BoundingBox3"/> instance
		/// representing the boundaries of the player-accessible region
		/// of the given area. 
		/// </returns>
		/// <remarks>Interior areas have no inaccessible region,
		/// and will return the bounds of the full area.</remarks>
		public static BoundingBox3 GetAccessibleRegion(NWN2GameArea area)
		{
			BoundingBox3 bounds = area.GetBoundsOfArea();
			
			if (area.HasTerrain) {
				Vector3 min = new Vector3(bounds.Min.X + InaccessibleTerrainExtent,
				                          bounds.Min.Y + InaccessibleTerrainExtent,
				                          bounds.Min.Z);
				Vector3 max = new Vector3(bounds.Max.X + InaccessibleTerrainExtent,
				                          bounds.Max.Y + InaccessibleTerrainExtent,
				                          bounds.Max.Z);
				bounds = new BoundingBox3(min,max);
			}
			
			return bounds;
		}
		
		
		/// <summary>
		/// Get the distance from any edge
		/// of the player-walkable central region 
		/// to the corresponding edge of the game area itself.
		/// </summary>
		/// <param name="area">The game area to check
		/// the inaccessible region of.</param>
		/// <returns>
		/// The extent of the inaccessible terrain
		/// in the given area, expressed in the units used by 
		/// <see cref="OEIShared.OEIMath.BoundingBox3"/>.
		/// </returns>
		/// <remarks>Interior areas have no inaccessible region,
		/// and will return a value of 0.</remarks>
		public static int GetInaccessibleDistance(NWN2GameArea area)
		{
			return area.HasTerrain ? InaccessibleTerrainExtent : 0;
		}
		
		
		/// <summary>
		/// Get a random position within the accessible region 
		/// of a given game area.
		/// </summary>
		/// <param name="area">The game area.</param>
		/// <param name="">True to return a position within the
		/// player-accessible central region of an exterior area;
		/// false to ignore this distinction.</param>
		/// <returns>A random position within the accessible
		/// region of this area.</returns>
		/// <remarks>Assumes the area to be a flat plane - the 
		/// Z field of the returned <see cref="Vector3"/>
		/// will always be 0.</remarks>
		public static Vector3 GetRandomPosition(NWN2GameArea area, bool accessible)
		{
			BoundingBox3 bounds;
			bounds = (accessible ? GetAccessibleRegion(area) : area.GetBoundsOfArea());
			
			Random random = new Random();
			
			float x = random.Next((int)bounds.Min.X,(int)bounds.Max.X);
			float y = random.Next((int)bounds.Min.Y,(int)bounds.Max.Y);
			float z = 0;
			
			return new Vector3(x,y,z);
		}
		
		
		/// <summary>
		/// Creates an instance of a blueprint and adds it to a given area.
		/// </summary>
		/// <param name="area">The area to add the game object to.</param>
		/// <param name="blueprint">The blueprint to create the game
		/// object from.</param>
		/// <param name="tag">The tag to give the newly-created object.</param>
		/// <returns>The newly-created game object.</returns>
		public static INWN2Instance AddGameObject(NWN2GameArea area,
		                                          INWN2Blueprint blueprint,
		                                          string tag)
		{
			if (blueprint == null)
				throw new ArgumentNullException("blueprint","Need a blueprint to create object from.");	
			if (area == null)
				throw new ArgumentNullException("area","Need an area to add to.");
			
			INWN2Instance instance = NWN2GlobalBlueprintManager.CreateInstanceFromBlueprint(blueprint);
			
			((INWN2Object)instance).Tag = tag;
			
			area.AddInstance(instance);
			
			return instance;
		}
		
				
		/// <summary>
		/// Creates an instance of a blueprint and adds it to a given area.
		/// </summary>
		/// <param name="area">The area to add the game object to.</param>
		/// <param name="blueprint">The blueprint to create the game
		/// object from.</param>
		/// <param name="tag">The tag to give the newly-created object.</param>
		/// <param name="position">The position within the area to place 
		/// the object.</param>
		/// <returns>The newly-created game object.</returns>
		public static INWN2Instance AddGameObject(NWN2GameArea area,
		                                          INWN2Blueprint blueprint,
		                                          string tag,
		                                          Vector3 position)
		{
			if (position == null)
				throw new ArgumentNullException("position","Cannot set object's position to null.");
			
			INWN2Instance instance = AddGameObject(area,blueprint,tag);
			
			INWN2SituatedInstance situated = instance as INWN2SituatedInstance;			
			if (situated == null) {
				throw new ArgumentException("blueprint","Instances of the given blueprint have " +
				                            "no Position field - their position cannot be set.");
			} else {
				situated.Position = position;
			}
			
			return instance;
		}
		
		#endregion
	}
}
