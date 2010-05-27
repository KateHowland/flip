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
 * This file added by Keiron Nicholson on 27/05/2010 at 14:10.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using NWN2Toolset;
using NWN2Toolset.NWN2.Data.Blueprints;
using NWN2Toolset.NWN2.Data.Instances;
using NWN2Toolset.NWN2.Data.Templates;
using NWN2Toolset.NWN2.Data.TypedCollections;
using NWN2Toolset.Plugins;
using Sussex.Flip.Games.NeverwinterNightsTwo.Behaviours;
using Sussex.Flip.Games.NeverwinterNightsTwo.Utils;

namespace Sussex.Flip.Games.NeverwinterNightsTwo.Integration
{
	/// <summary>
	/// Description of NarrativeThreadsHelper.
	/// </summary>
	public class NarrativeThreadsHelper
	{
		#region Constants
		
		/// <summary>
		/// The name given to the Narrative Threads plugin.
		/// </summary>
		public const string PluginName = "Narrative Threads";
		
		#endregion
		
		#region Fields
		
		/// <summary>
		/// The path at which to find user-generated pictures for
		/// blueprints created by Narrative Threads.
		/// </summary>
		protected string imagesPath;
		
		/// <summary>
		/// The prefix given by Narrative Threads to the ResRef of
		/// blueprints it creates.
		/// </summary>
		protected string prefix;
		
		/// <summary>
		/// Types of blueprint which are typically created by Narrative Threads.
		/// </summary>
		protected List<Nwn2Type> associatedTypes;
		
		#endregion
		
		#region Properties
		
		/// <summary>
		/// The prefix given by Narrative Threads to the ResRef of
		/// blueprints it creates.
		/// </summary>
		public string SpecialBlueprintPrefix {
			get { return prefix; }
		}
		
		
		/// <summary>
		/// The path at which to find user-generated pictures for
		/// blueprints created by Narrative Threads.
		/// </summary>
		public string ImagesPath {
			get { return imagesPath; }
			set { imagesPath = value; }
		}
		
		
		/// <summary>
		/// Types of blueprint which are typically created by Narrative Threads.
		/// </summary>
		public List<Nwn2Type> AssociatedTypes {
			get { return associatedTypes; }
		}
		
		
		/// <summary>
		/// Checks whether the Neverwinter Nights 2 Toolset is currently running with
		/// the Narrative Threads plugin loaded.
		/// </summary>
		public bool IsLoaded {
			get { 
				if (!Nwn2ToolsetFunctions.ToolsetIsOpen()) return false;
				
				foreach (INWN2Plugin plugin in NWN2ToolsetMainForm.PluginHost.Plugins) {
					if (plugin.Name == PluginName) return true;
				}
				
				return false;
			}
		}
		
		#endregion
		
		#region Constructors
		
		/// <summary>
		/// Constructs a new <see cref="NarrativeThreadsHelper"/> instance.
		/// </summary>
		public NarrativeThreadsHelper()
		{
			prefix = "nt_";
			
			imagesPath = @"C:\Sussex University\Narrative Threads\Pictures\";
			
			associatedTypes = new List<Nwn2Type>
			{ 
				Nwn2Type.Creature,
				Nwn2Type.Door,
				Nwn2Type.Item,
				Nwn2Type.Placeable
			};				
		}
		
		#endregion
		
		#region Methods
				
		/// <summary>
		/// Checks whether a given behaviour represents an instance which
		/// appears to have been created by Narrative Threads.
		/// </summary>
		/// <param name="behaviour">The behaviour to check.</param>
		/// <returns>True if the behaviour represents something which appears to have 
		/// been created by Narrative Threads; false otherwise.</returns>
		public bool CreatedByNarrativeThreads(InstanceBehaviour behaviour)
		{
			if (behaviour == null) throw new ArgumentNullException("behaviour");				
			return CreatedByNarrativeThreads(behaviour.Nwn2Type,behaviour.ResRef);
		}
		
			
		/// <summary>
		/// Checks whether a given behaviour represents a blueprint which
		/// appears to have been created by Narrative Threads.
		/// </summary>
		/// <param name="behaviour">The behaviour to check.</param>
		/// <returns>True if the behaviour represents something which appears to have 
		/// been created by Narrative Threads; false otherwise.</returns>
		public bool CreatedByNarrativeThreads(BlueprintBehaviour behaviour)
		{
			if (behaviour == null) throw new ArgumentNullException("behaviour");				
			return CreatedByNarrativeThreads(behaviour.Nwn2Type,behaviour.ResRef);
		}
		
			
		/// <summary>
		/// Checks whether a given instance
		/// appears to have been created by Narrative Threads.
		/// </summary>
		/// <param name="instance">The instance to check.</param>
		/// <returns>True if the instance appears to have 
		/// been created by Narrative Threads; false otherwise.</returns>
		public bool CreatedByNarrativeThreads(INWN2Instance instance)
		{
			if (instance == null) throw new ArgumentNullException("instance");				
			return CreatedByNarrativeThreads(Nwn2ScriptSlot.GetNwn2Type(instance.ObjectType),instance.Template.ResRef.Value);
		}
		
			
		/// <summary>
		/// Checks whether a given blueprint
		/// appears to have been created by Narrative Threads.
		/// </summary>
		/// <param name="behaviour">The blueprint to check.</param>
		/// <returns>True if the blueprint appears to have 
		/// been created by Narrative Threads; false otherwise.</returns>
		public bool CreatedByNarrativeThreads(INWN2Blueprint blueprint)
		{
			if (blueprint == null) throw new ArgumentNullException("blueprint");				
			return CreatedByNarrativeThreads(Nwn2ScriptSlot.GetNwn2Type(blueprint.ObjectType),blueprint.Resource.ResRef.Value);
		}
		
		
		/// <summary>
		/// Checks whether the details of a given object indicate that it
		/// appears to have been created by Narrative Threads.
		/// </summary>
		/// <param name="type">The type of object.</param>
		/// <param name="resref">The resref of the object.</param>
		/// <returns>True if the object appears to have 
		/// been created by Narrative Threads; false otherwise.</returns>
		public bool CreatedByNarrativeThreads(Nwn2Type type, string resref)
		{
			if (resref == null) throw new ArgumentNullException("resref");	
			if (resref == String.Empty) throw new ArgumentException("resRef cannot be empty.","resref");			
			return associatedTypes.Contains(type) && resref.StartsWith(SpecialBlueprintPrefix);
		}
		
		
		/// <summary>
		/// Returns a subset of a given blueprint collection containing only those blueprints
		/// which appear to have been created by Narrative Threads.
		/// </summary>
		/// <param name="blueprints">A collection of blueprints.</param>
		/// <returns>A subset of the given collection of blueprints.</returns>
		public NWN2BlueprintCollection GetBlueprintsCreatedByNarrativeThreads(NWN2BlueprintCollection blueprints)
		{
			if (blueprints == null) throw new ArgumentNullException("blueprints");
			
			NWN2BlueprintCollection narrativeThreadsBlueprints = new NWN2BlueprintCollection();
			
			foreach (INWN2Blueprint blueprint in blueprints) {
				if (CreatedByNarrativeThreads(blueprint)) {
					narrativeThreadsBlueprints.Add(blueprint);
				}
			}
			
			return narrativeThreadsBlueprints;
		}
				
		
		/// <summary>
		/// Gets a Narrative Threads user-created image representing a given 
		/// Neverwinter Nights 2 blueprint, if one could be found.
		/// </summary>
		/// <param name="resref">The resref of the object/blueprint to check for images of.</param>
		/// <returns>An Image, or null if no image was found.</returns>
		public Image GetImageForResRef(string resRef)
		{			
			if (resRef == null) throw new ArgumentNullException("resRef");
			if (resRef == String.Empty) throw new ArgumentException("resRef cannot be empty.","resRef");
			
			string filename = resRef + ".bmp";
			string path = Path.Combine(ImagesPath,filename);
			
			return GetImage(path);
		}
				
		
		/// <summary>
		/// Checks whether a Narrative Threads user-created image representing a given 
		/// Neverwinter Nights blueprint is available.
		/// </summary>
		/// <param name="resref">The resref of the object/blueprint to check for images of.</param>
		/// <returns>True if an image was found; false otherwise.</returns>
		public bool HasImage(string resRef)
		{			
			if (resRef == null) throw new ArgumentNullException("resRef");
			if (resRef == String.Empty) throw new ArgumentException("resRef cannot be empty.","resRef");
			
			string filename = resRef + ".bmp";
			string path = Path.Combine(ImagesPath,filename);
			
			return File.Exists(path);
		}
		
		
		/// <summary>
		/// Gets an Image from an image file at the given path.
		/// </summary>
		/// <param name="path">The path to locate an image.</param>
		/// <returns>An Image, or null if an exception was raised.</returns>
		protected Image GetImage(string path)
		{			
			if (path == null) throw new ArgumentNullException("path");
			if (!File.Exists(path)) return null;
			
			try {
				Image image = new Image();
				Uri uri = new Uri(path);
				BitmapImage bmp = new BitmapImage(uri);
				image.Source = bmp;
				return image;
			}
			catch (Exception) {
				return null;
			}
		}		
		
		#endregion
	}
}
