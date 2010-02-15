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
 * This file added by Keiron Nicholson on 14/10/2009 at 17:52.
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using NWN2Toolset.NWN2.Data;
using NWN2Toolset.NWN2.Data.Instances;
using NWN2Toolset.NWN2.Data.Templates;

namespace Sussex.Flip.Games.NeverwinterNightsTwo.Utils.Tests
{
	[TestFixture]
	public class Nwn2ScriptSlotTests
	{
		[Test]
		public void ReturnsFullListOfScriptSlotsForEachType()
		{
			List<string> reflected;
			List<string> provided;	
			
			// NWN2ObjectTypes:
			foreach (NWN2ObjectType type in Enum.GetValues(typeof(NWN2ObjectType))) {
				if (type == NWN2ObjectType.Prefab) continue;
				
				reflected = new List<string>();
								
				foreach (PropertyInfo pi in GetInstanceType(type).GetProperties(BindingFlags.Public | BindingFlags.Instance)) {
					if (pi.Name.StartsWith("On")) reflected.Add(pi.Name);
				}
								
				provided = (List<string>)Nwn2ScriptSlot.GetScriptSlotNames(type);
				
				provided.Sort();
				reflected.Sort();
				
				Assert.IsNotNull(provided);
				Assert.IsTrue(reflected.SequenceEqual(provided));
			}	
				
			// NWN2GameModule:
			reflected = new List<string>();
			
			foreach (PropertyInfo pi in typeof(NWN2ModuleInformation).GetProperties(BindingFlags.Public | BindingFlags.Instance)) {
				if (pi.Name.StartsWith("On")) reflected.Add(pi.Name);
			}
				
			provided = (List<string>)Nwn2ScriptSlot.GetScriptSlotNames(Nwn2Type.Module);
			
			provided.Sort();
			reflected.Sort();
				
			Assert.IsNotNull(provided);
			Assert.IsTrue(reflected.SequenceEqual(provided));
				
			// NWN2GameArea:
			reflected = new List<string>();
			
			foreach (PropertyInfo pi in typeof(NWN2GameArea).GetProperties(BindingFlags.Public | BindingFlags.Instance)) {
				if (pi.Name.StartsWith("On")) reflected.Add(pi.Name);
			}
				
			provided = (List<string>)Nwn2ScriptSlot.GetScriptSlotNames(Nwn2Type.Area);
				
			provided.Sort();
			reflected.Sort();
				
			Assert.IsNotNull(provided);
			Assert.IsTrue(reflected.SequenceEqual(provided));
		}
		
		
		private Type GetInstanceType(NWN2ObjectType type)
		{
			switch (type) {
				case NWN2ObjectType.Creature:
					return typeof(NWN2CreatureInstance);
				case NWN2ObjectType.Door:
					return typeof(NWN2DoorInstance);
				case NWN2ObjectType.Encounter:
					return typeof(NWN2EncounterInstance);
				case NWN2ObjectType.Environment:
					return typeof(NWN2EnvironmentInstance);
				case NWN2ObjectType.Item:
					return typeof(NWN2ItemInstance);
				case NWN2ObjectType.Light:
					return typeof(NWN2LightInstance);
				case NWN2ObjectType.Placeable:
					return typeof(NWN2PlaceableInstance);
				case NWN2ObjectType.PlacedEffect:
					return typeof(NWN2PlacedEffectInstance);
				case NWN2ObjectType.Prefab:
					return null;
				case NWN2ObjectType.Sound:
					return typeof(NWN2SoundInstance);
				case NWN2ObjectType.StaticCamera:
					return typeof(NWN2StaticCameraInstance);
				case NWN2ObjectType.Store:
					return typeof(NWN2StoreInstance);
				case NWN2ObjectType.Tree:
					return typeof(NWN2TreeInstance);
				case NWN2ObjectType.Trigger:
					return typeof(NWN2TriggerInstance);
				case NWN2ObjectType.Waypoint:
					return typeof(NWN2WaypointInstance);
				default:
					return null;
			}
		}
	}
}
