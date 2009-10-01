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
 * This file added by Keiron Nicholson on 25/09/2009 at 15:36.
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.ServiceModel;
using NWN2Toolset.NWN2.Data;
using NWN2Toolset.NWN2.IO;

namespace Sussex.Flip.Games.NeverwinterNightsTwo.Utils
{
	/// <summary>
	/// A service providing access to a Neverwinter Nights 2 toolset session.
	/// </summary>
	[ServiceBehavior(IncludeExceptionDetailInFaults=true)]
	public class Nwn2SessionAdapter : INwn2Service
	{
		#region Fields
		
		/// <summary>
		/// The session this adapter wraps.
		/// </summary>
		protected INwn2Session session;
		
		#endregion
		
		#region Constructors
				
		/// <summary>
		/// Constructs a new <see cref="Nwn2SessionAdapter"/> instance.
		/// </summary>
		/// <remarks>This default constructor wraps a Nwn2Session object.</remarks>
		public Nwn2SessionAdapter()
		{
			session = new Nwn2Session();
		}
		
		
		/// <summary>
		/// Constructs a new <see cref="Nwn2SessionAdapter"/> instance.
		/// </summary>
		/// <param name="session">The session this adapter wraps.</param>
		public Nwn2SessionAdapter(INwn2Session session)
		{
			if (session == null) throw new ArgumentNullException("session");			
			this.session = session;
		}
		
		#endregion
		
		#region INwn2Service methods
		
		/// <summary>
		/// Creates a Neverwinter Nights 2 game module.
		/// </summary>
		/// <param name="path">The path to create the module at. If 'location'
		/// is set to ModuleLocationType.Directory, this must be the path for
		/// a folder to be created within NWN2Toolset.NWN2ToolsetMainForm.ModulesDirectory.</param>
		/// <param name="location">The serialisation form of the module.</param>
		public void CreateModule(string path, NWN2Toolset.NWN2.IO.ModuleLocationType location)
		{
			session.CreateModule(path,location);
		}
		
		
		/// <summary>
		/// Opens a Neverwinter Nights 2 game module.
		/// </summary>
		/// <param name="name">The path of the module to open.</param>
		/// <param name="location">The serialisation form of the module.</param>
		public void OpenModule(string path, ModuleLocationType location)
		{
			session.OpenModule(path,location);
		}
		
		
		/// <summary>
		/// Saves changes to the current game module.
		/// </summary>
		/// <remarks>Saves to the default modules directory.</remarks>
		public void SaveModule()
		{
			session.SaveModule(session.GetCurrentModule());
		}
		
		
		/// <summary>
		/// Closes the current module.
		/// </summary>
		public void CloseModule()
		{
			session.CloseModule();
		}
		
		
		/// <summary>
		/// Gets the name of the module that is currently open in the toolset.
		/// </summary>
		/// <returns>The name of the current module, or null if no module is open.</returns>
		public string GetCurrentModuleName()
		{
			NWN2GameModule module = session.GetCurrentModule();
			if (module == null) return null;
			else return module.Name;
		}		
		
		
		/// <summary>
		/// Gets the absolute path of the module that is currently open in the toolset.
		/// </summary>
		/// <returns>The absolute path of the current module, or null if no module is open.</returns>
		public string GetCurrentModulePath()
		{
			return session.GetCurrentModulePath();
		}
		
		
		/// <summary>
		/// Gets the location type of the module that is currently open in the toolset.
		/// </summary>
		/// <returns>The location type of the current module.</returns>
		public ModuleLocationType GetCurrentModuleLocation()
		{
			return session.GetCurrentModule().LocationType;
		}
		
		
		/// <summary>
		/// Creates a Neverwinter Nights 2 area
		/// in the current module.
		/// <param name="name">The name to give the area.</param>
		/// <param name="exterior">True to create an exterior area
		/// with terrain; false to create an interior area with tiles.</param>
		/// <param name="size">The size of area to create.</param>
		public void AddArea(string name, bool exterior, Size size)
		{
			session.AddArea(name,exterior,size);
		}
		
		
		/// <summary>
		/// Gets a list of names of every area in the 
		/// current module.
		/// </summary>
		/// <returns>A list of area names, or null
		/// if there is no module open.</returns>
		public ICollection<string> GetAreas()
		{
			NWN2GameModule module = session.GetCurrentModule();
						
			if (module == null) return null;
			else {
				List<string> areas = new List<string>(module.Areas.Count);
				foreach (string key in module.Areas.Keys) {
					areas.Add(key);
				}
				return areas;
			}
		}
		
		#endregion
	}
}
