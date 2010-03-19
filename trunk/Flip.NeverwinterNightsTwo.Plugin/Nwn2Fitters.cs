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
 * This file added by Keiron Nicholson on 10/03/2010 at 09:49.
 */

using System;
using System.Collections.Generic;
using Sussex.Flip.UI;

namespace Sussex.Flip.Games.NeverwinterNightsTwo
{
	/// <summary>
	/// Description of Nwn2Fitters.
	/// </summary>
	public class Nwn2Fitters
	{
		#region Fields
		
		protected Fitter onlyCreatures;
		protected Fitter onlyDoors;		
		protected Fitter onlyItems;		
		protected Fitter onlyPlaceables;		
		protected Fitter onlyStores;		
		protected Fitter onlyTriggers;		
		protected Fitter onlyWaypoints;		
		protected Fitter onlyPlayers;
		protected Fitter onlyAreas;		
		protected Fitter onlyModules;		
		protected Fitter onlyCreaturesOrPlayers;		
		protected Fitter onlyDoorsOrPlaceables;		
		protected Fitter onlyInstances;		
		protected Fitter onlyEventRaisers;		
		
		#endregion
		
		#region Properties
		
		public Fitter OnlyCreatures {
			get { return onlyCreatures; }
		}		
		
		public Fitter OnlyDoors {
			get { return onlyDoors; }
		}
		
		public Fitter OnlyItems {
			get { return onlyItems; }
		}
		
		public Fitter OnlyPlaceables {
			get { return onlyPlaceables; }
		}
		
		public Fitter OnlyStores {
			get { return onlyStores; }
		}
		
		public Fitter OnlyTriggers {
			get { return onlyTriggers; }
		}
		
		public Fitter OnlyWaypoints {
			get { return onlyWaypoints; }
		}
		
		public Fitter OnlyPlayers {
			get { return onlyPlayers; }
		}
		
		public Fitter OnlyAreas {
			get { return onlyAreas; }
		}
		
		public Fitter OnlyModules {
			get { return onlyModules; }
		}
		
		public Fitter OnlyCreaturesOrPlayers {
			get { return onlyCreaturesOrPlayers; }
		}
		
		public Fitter OnlyDoorsOrPlaceables {
			get { return onlyDoorsOrPlaceables; }
		}
		
		public Fitter OnlyInstances {
			get { return onlyInstances; }
		}
		
		public Fitter OnlyEventRaisers {
			get { return onlyEventRaisers; }
		}
		
		#endregion
		
		#region Constructors
		
		public Nwn2Fitters()
		{
			onlyCreatures = new ObjectBlockFitter("Instance","Creature");
			onlyDoors = new ObjectBlockFitter("Instance","Door");
			onlyItems = new ObjectBlockFitter("Instance","Item");
			onlyPlaceables = new ObjectBlockFitter("Instance","Placeable");
			onlyStores = new ObjectBlockFitter("Instance","Store");
			onlyTriggers = new ObjectBlockFitter("Instance","Trigger");
			onlyWaypoints = new ObjectBlockFitter("Instance","Waypoint");
			onlyPlayers = new ObjectBlockFitter("Player");
			onlyAreas = new ObjectBlockFitter("Area");
			onlyModules = new ObjectBlockFitter("Module");
			onlyCreaturesOrPlayers = new CreaturePlayerFitter();
			onlyDoorsOrPlaceables = new ObjectBlockFitter("Instance",new List<string>{"Door","Placeable"});
			onlyInstances = new ObjectBlockFitter("Instance");
			onlyEventRaisers = new ObjectBlockFitter(new List<string>{"Area","Module","Instance"});
		}
		
		#endregion
	}
}
