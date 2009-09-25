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
	/// A facade for a Neverwinter Nights 2 area.
	/// </summary>
	public class Area : AreaBase
	{
		#region Constructors

		/// <summary>
		/// Constructs a new <see cref="Area"/> instance.
		/// </summary>
		public Area(NWN2GameArea area)
		{
			if (area == null) throw new ArgumentNullException("area"); 
			this.nwn2Area = area;
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
		public override BoundingBox3 GetAccessibleRegion()
		{
			BoundingBox3 bounds = nwn2Area.GetBoundsOfArea();

			if (nwn2Area.HasTerrain) {
				Vector3 min = new Vector3(bounds.Min.X + InaccessibleRegionWidth, 
				                          bounds.Min.Y + InaccessibleRegionWidth, 
				                          bounds.Min.Z);
				Vector3 max = new Vector3(bounds.Max.X + InaccessibleRegionWidth, 
				                          bounds.Max.Y + InaccessibleRegionWidth, 
				                          bounds.Max.Z);
				bounds = new BoundingBox3(min, max);
			}

			return bounds;
		}


		/// <summary>
		/// Gets a random position in the area.
		/// </summary>
		/// <param name="">True to return a position within the
		/// accessible region only; false to ignore this distinction.</param>
		/// <returns>A random position within the area.</returns>
		/// <remarks>Assumes the area to be a flat plane - the 
		/// Z field of the returned <see cref="Vector3"/>
		/// will always be 0.</remarks>
		public override Vector3 GetRandomPosition(bool accessible)
		{
			BoundingBox3 bounds;
			bounds = (accessible ? GetAccessibleRegion() : nwn2Area.GetBoundsOfArea());

			Random random = new Random();

			float x = random.Next((int)bounds.Min.X, (int)bounds.Max.X);
			float y = random.Next((int)bounds.Min.Y, (int)bounds.Max.Y);
			float z = 0;

			return new Vector3(x, y, z);
		}


		/// <summary>
		/// Creates an instance of a blueprint and adds it to the area.
		/// </summary>
		/// <param name="blueprint">The blueprint to create the game
		/// object from.</param>
		/// <param name="tag">The tag to give the newly-created object.</param>
		/// <returns>The newly-created game object.</returns>
		public override INWN2Instance AddGameObject(INWN2Blueprint blueprint, string tag)
		{
			if (blueprint == null) throw new ArgumentNullException("blueprint", "Need a blueprint to create object from."); 

			INWN2Instance instance = NWN2GlobalBlueprintManager.CreateInstanceFromBlueprint(blueprint);

			((INWN2Object)instance).Tag = tag;

			nwn2Area.AddInstance(instance);

			return instance;
		}


		/// <summary>
		/// Creates an instance of a blueprint and adds it to the area.
		/// </summary>
		/// <param name="blueprint">The blueprint to create the game
		/// object from.</param>
		/// <param name="tag">The tag to give the newly-created object.</param>
		/// <param name="position">The position within the area to place 
		/// the object.</param>
		/// <returns>The newly-created game object.</returns>
		public override INWN2Instance AddGameObject(INWN2Blueprint blueprint, string tag, Vector3 position)
		{
			INWN2Instance instance = AddGameObject(blueprint, tag);
			
			if (position != null) {
				INWN2SituatedInstance situated = instance as INWN2SituatedInstance;
				if (situated == null) {
					throw new ArgumentException("blueprint", "Instances of the given blueprint have " + 
					                            "no Position field - their position cannot be set.");
				}
				else {
					situated.Position = position;
				}
			}

			return instance;
		}
		
		
		/// <summary>
		/// Saves the module.
		/// </summary>
		public override void Save()
		{
			nwn2Area.OEISerialize();
		}
		
		#endregion
	}
}
