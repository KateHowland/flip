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
		private static INwn2Session session = new Nwn2Session();
		
		
		/// <summary>
		/// Creates a Neverwinter Nights 2 game module intended for testing purposes.
		/// </summary>
		/// <param name="name">The name to give the test module.</param>
		/// <returns>A serialised Neverwinter Nights 2 game module.</returns>
		public static NWN2GameModule CreateTestModule(string name)
		{
			session.CreateModule(name,ModuleLocationType.Directory);
					
			session.OpenDirectoryModule(name);
			
			NWN2GameModule module = session.GetCurrentModule();
			
			Size small = new Size(Area.MinimumAreaLength,Area.MinimumAreaLength);
			Size large = new Size(Area.MaximumAreaLength,Area.MaximumAreaLength);
			
			AreaBase area1 = session.AddArea(module,"desert",true,large);
			AreaBase area2 = session.AddArea(module,"castle",false,large);
			
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
					
					Vector3 position = area1.GetRandomPosition(true);
					
					area1.AddGameObject(blueprint,tag,position);
				}
				catch (InvalidOperationException e) {
					System.Windows.Forms.MessageBox.Show(e.Message);
				}
			}
			
			area1.Save();
			area2.Save();
			
			session.SaveModule(module);
			
			return module;
		}
	}
}
