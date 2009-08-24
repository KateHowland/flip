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
 * This file added by Keiron Nicholson on 10/08/2009 at 18:08.
 */

using System;
using System.Drawing;
using NWN2Toolset.NWN2.Data;
using NWN2Toolset.NWN2.Data.Blueprints;
using NWN2Toolset.NWN2.Data.Templates;
using NWN2Toolset.NWN2.IO;
using OEIShared.Utils;
using OEIShared.OEIMath;
using Microsoft.DirectX;

namespace Sussex.Flip.Games.NeverwinterNightsTwo.Utils
{
	/// <summary>
	/// Provides functionality relating to Neverwinter Nights 2
	/// for testing purposes.
	/// </summary>
	public static class Nwn2TestingFunctions
	{		
		/// <summary>
		/// Creates a Neverwinter Nights 2 game module intended for testing purposes.
		/// </summary>
		/// <param name="name">The name to give the test module.</param>
		/// <param name="location">The serialisation form of the module.</param>
		/// <returns>A serialised Neverwinter Nights 2 game module.</returns>
		public static NWN2GameModule CreateTestModule(string name, ModuleLocationType location)
		{
			Nwn2ModuleFunctions.CreateModule(name,location);
			
			NWN2GameModule module = Nwn2ModuleFunctions.OpenModule(name,location);
			
			Size small = new Size(Nwn2AreaFunctions.MinimumAreaLength,Nwn2AreaFunctions.MinimumAreaLength);
			Size large = new Size(Nwn2AreaFunctions.MaximumAreaLength,Nwn2AreaFunctions.MaximumAreaLength);
			
			NWN2GameArea area1 = Nwn2AreaFunctions.CreateArea(ref module,"desert",true,large);
			NWN2GameArea area2 = Nwn2AreaFunctions.CreateArea(ref module,"castle",false,large);
			
			NWN2ObjectType type = NWN2ObjectType.Creature;
			
			string[] creatures = new string[]{"c_bronzedragon",
											  "c_cat",
											  "c_umber",
											  "c_mindflayer",
											  "c_bat",
											  "c_badger",
											  "c_badgerdire"};
			
			foreach (string s in creatures) {
				try {
					OEIResRef resRef = new OEIResRef(s);
					string tag = resRef.Value + DateTime.Now.Second;
					
					INWN2Blueprint blueprint = NWN2GlobalBlueprintManager.FindBlueprint(type,resRef);				
					if (blueprint == null) 
						throw new InvalidOperationException("Blueprint with ResRef '" + resRef.Value + 
						                                    "' not found.");
					
					bool accessible = Nwn2GameObjectFunctions.MustPlaceWithinPlayerAccessibleRegion(blueprint);
					Vector3 position = Nwn2AreaFunctions.GetRandomPosition(area1,accessible);
					
					Nwn2AreaFunctions.AddGameObject(area1,blueprint,tag,position);
				}
				catch (InvalidOperationException e) {
					System.Windows.Forms.MessageBox.Show(e.Message);
				}
			}
			
			area1.OEISerialize();
			area2.OEISerialize();
			
			Nwn2ModuleFunctions.SaveModule(module);
			
			return module;
		}
	}
}
