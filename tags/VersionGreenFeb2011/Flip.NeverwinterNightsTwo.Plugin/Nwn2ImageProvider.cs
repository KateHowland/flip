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
 * This file added by Keiron Nicholson on 02/06/2010 at 13:19.
 */

using System;
using System.IO;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Sussex.Flip.Games.NeverwinterNightsTwo.Behaviours;
using Sussex.Flip.Games.NeverwinterNightsTwo.Images;
using Sussex.Flip.Games.NeverwinterNightsTwo.Integration;
using Sussex.Flip.Games.NeverwinterNightsTwo.Utils;
using Sussex.Flip.UI;

namespace Sussex.Flip.Games.NeverwinterNightsTwo
{
	/// <summary>
	/// Description of Nwn2ImageProvider.
	/// </summary>
	public class Nwn2ImageProvider : ImageProvider
	{
		protected NarrativeThreadsHelper nt;
		protected ImageGetter imageGetter;
		
		
		public Nwn2ImageProvider() : this(null)
		{			
		}
		
		
		public Nwn2ImageProvider(NarrativeThreadsHelper nt)
		{
			this.nt = nt;			
			imageGetter = new ImageGetter();
		}
		
		
		public Image GetImage(InstanceBehaviour behaviour)
		{
			Image image;						
			string objectType = behaviour.Nwn2Type.ToString();
			
			// First, try to get a Narrative Threads user-created image:
			if (nt != null && nt.CreatedByNarrativeThreads(behaviour) && nt.HasImage(behaviour.ResRef)) {
				image = nt.GetImageForResRef(behaviour.ResRef);		
			}
						
			// Or if this has an icon, use that:
			else if (!String.IsNullOrEmpty(behaviour.IconName)) {
				image = GetIconImage(behaviour.IconName);
			}
			
			// Otherwise, try to get a generic image representing this instance's template:
			else {	
				image = GetImage(objectType,behaviour.ResRef);
			}
			
			// If neither is available, use a placeholder image:
			if (image == null) {
				if (behaviour.Nwn2Type == Nwn2Type.Item) {
					image = GetImage("Placeholder","Item");
				}
				else if (behaviour.Nwn2Type == Nwn2Type.Waypoint) {
					image = GetImage("Placeholder","Waypoint");
				}
				else {
					image = GetImage("Placeholder","Instance");
				}
			}
			
			return image;
		}
		
		
		public Image GetImage(BlueprintBehaviour behaviour)
		{
			Image image;				
			string objectType = behaviour.Nwn2Type.ToString();
			
			// First, try to get a Narrative Threads user-created image:
			if (nt != null && nt.CreatedByNarrativeThreads(behaviour) && nt.HasImage(behaviour.ResRef)) {
				image = nt.GetImageForResRef(behaviour.ResRef);	
			}
			
			// Or if this has an icon, use that:
			else if (!String.IsNullOrEmpty(behaviour.IconName)) {
				image = GetIconImage(behaviour.IconName);
			}
			
			// Otherwise, try to get an image representing this blueprint:
			else {	
				image = GetImage(objectType,behaviour.ResRef);
			}
			
			// Failing that, try to retrieve an image of the blueprint it derives from:
			if (image == null) {
				image = GetImage(objectType,behaviour.BaseResRef);
			}
			
			// If neither is available, use a placeholder image:
			if (image == null) {
				if (behaviour.Nwn2Type == Nwn2Type.Item) {
					image = GetImage("Placeholder","Item");
				}
				else if (behaviour.Nwn2Type == Nwn2Type.Waypoint) {
					image = GetImage("Placeholder","Waypoint");
				}
				else {
					image = GetImage("Placeholder","Blueprint");
				}
			}
			
			return image;
		}
		
		
		public Image GetImage(AreaBehaviour behaviour)
		{
			Image image;
			
			if (behaviour.IsExterior) {
				image = GetImage("Other","Area_Exterior");
			}
			else {
				image = GetImage("Other","Area_Interior");
			}
						
			if (image == null) image = GetImage("Placeholder","Default");
		
			return image;
		}
		
		
		public Image GetPlayerImage()
		{
			Image image;
			
			image = GetImage("Other","Player");
			
			if (image == null) image = GetImage("Placeholder","Default");
			
			return image;
		}
		
		
		public Image GetModuleImage()
		{
			Image image;
			
			image = GetImage("Other","Module");
			
			if (image == null) image = GetImage("Placeholder","Default");
			
			return image;
		}
		
		
		public Image GetWildcardImage()
		{
			Image image;
			
			image = GetImage("Other","Wildcard");
			
			if (image == null) image = GetImage("Placeholder","Default");
			
			return image;
		}
		
		
		public override void AssignImage(ObjectBlock block)
		{
			if (block == null) throw new ArgumentNullException("block");
			
			ObjectBehaviour b = block.Behaviour;
			Image image;
			
			if (b is InstanceBehaviour) image = GetImage((InstanceBehaviour)b);
			else if (b is BlueprintBehaviour) image = GetImage((BlueprintBehaviour)b);
			else if (b is AreaBehaviour) image = GetImage((AreaBehaviour)b);
			else if (b is PlayerBehaviour) image = GetPlayerImage();
			else if (b is ModuleBehaviour) image = GetModuleImage();
			else if (b is WildcardBehaviour) image = GetWildcardImage();
			else image = null;
						
			object[] args = new object[] { image, block };
			block.Dispatcher.Invoke(new AssignImageDelegate(AssignImage),System.Windows.Threading.DispatcherPriority.Normal,args);
		}
		
		
		protected delegate void AssignImageDelegate(Image image, ObjectBlock block);
		
		
		protected void AssignImage(Image image, ObjectBlock block)
		{
			if (block == null) throw new ArgumentNullException("block");
			
			block.DisplayImage = image;
		}
		
		
		/// <summary>
		/// Gets an image representing a particular Neverwinter Nights 2
		/// entity, of a given name and type.
		/// </summary>
		/// <param name="type">The type of entity to get an image of. Examples
		/// include 'Creature', 'Door', 'Placeholder' and 'Other'.</param>
		/// <param name="name">The name of the specific entity of the given
		/// type to get an image of. Usually the tag of an instance or the
		/// ResRef of a blueprint.</param>
		/// <returns>An Image, or null if an exception was raised.</returns>
		public Image GetImage(string type, string name)
		{
			return imageGetter.GetImage(type,name,true);
		}
		
		
		public Image GetIconImage(string icon)
		{
			return imageGetter.GetImage("Icon",icon,true);
		}
	}
}
