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
 * This file added by Keiron Nicholson on 26/04/2010 at 11:05.
 */

using System;
using System.Reflection;
using System.Windows;
using System.Xml;
using Sussex.Flip.Games.NeverwinterNightsTwo.Behaviours;
using Sussex.Flip.UI;
	
namespace Sussex.Flip.Games.NeverwinterNightsTwo
{
	/// <summary>
	/// Description of Nwn2BehaviourFactory.
	/// </summary>
	public class Nwn2BehaviourFactory : BehaviourFactory
	{
		public override ObjectBehaviour GetObjectBehaviour(XmlReader reader)
		{			
			if (reader == null) throw new ArgumentNullException("reader");
			
			reader.MoveToContent();	
			
			string behaviourType = reader.GetAttribute("Type");
			if (String.IsNullOrEmpty(behaviourType)) {
				throw new ArgumentException("Could not read Type attribute of Behaviour object from XmlReader.","reader");
			}					
				
			ObjectBehaviour behaviour;
			try {
				behaviour = (ObjectBehaviour)Assembly.GetExecutingAssembly().CreateInstance(behaviourType);
			}
			catch (Exception x) {
				throw new ArgumentException("Could not create an ObjectBehaviour of type " + behaviourType + ".",x);
			}
			
			if (behaviour == null) {
				throw new ArgumentException("Could not create an ObjectBehaviour of type " + behaviourType + " - type was not recognised.");
			}
			
			behaviour.ReadXml(reader);
			
			return behaviour;
		}
	}
}
