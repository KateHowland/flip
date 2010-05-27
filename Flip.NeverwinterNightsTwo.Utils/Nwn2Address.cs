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
 * This file added by Keiron Nicholson on 13/04/2010 at 15:25.
 */

using System;

namespace Sussex.Flip.Games.NeverwinterNightsTwo.Utils
{
	/// <summary>
	/// Description of Nwn2AddressParser.
	/// </summary>
	public sealed class Nwn2Address
	{
		public const char Separator = '|';
		public static readonly char[] separatorChars = new char[]{Separator};
		
		
		private string val;		
		private Nwn2Type targetType;
		private string targetSlot;
		private string areaTag;
		private string instanceTag;
		private int index;
			
		
		public string Value {
			get { return val; }
		}
		
		public Nwn2Type TargetType {
			get { return targetType; }
		}
		
		public string TargetSlot {
			get { return targetSlot; }
		}
		
		public string AreaTag {
			get { return areaTag; }
		}	
		
		public string InstanceTag {
			get { return instanceTag; }
		}	
		
		public int Index {
			get { return index; }
		}		
		
		public bool UseIndex {
			get { return index >= 0; }
		}		
		
		
		public Nwn2Address(string address)
		{
			if (address == null) throw new ArgumentNullException("address");
			if (address == String.Empty) throw new ArgumentException("address");
			
			this.val = address;
			
			string[] components = address.Split(separatorChars,StringSplitOptions.None);
			
			if (components.Length < 2) {
				throw new ArgumentException(
					String.Format("Address format is invalid: to target a module, pass 'Module|<name of script slot>'. {0}{1}",
					"To target an area, pass 'Area|<name of script slot>|<tag of area>'. ",
					"To target an instance, pass '<instance type>|<name of script slot>|<tag of area>|<tag of instance>|<optional index of instance>'."),
					"address");
			}	
			
			string targetTypeString = components[0];
			
			if (!Enum.IsDefined(typeof(Nwn2Type),targetTypeString)) {
				throw new ArgumentException(String.Format("Address format is invalid: '{0}' is not a recognised target type.",targetTypeString),"address");
			}
			
			targetType = (Nwn2Type)Enum.Parse(typeof(Nwn2Type),targetTypeString,true);
			
			if (!Scripts.IsEventRaiser(targetType)) {
				throw new ArgumentException(String.Format("Address format is invalid: '{0}' objects cannot attach scripts.",targetType),"address");
			}
						
			targetSlot = components[1];
						
			if (targetType == Nwn2Type.Module) {
				if (components.Length != 2) {
					throw new ArgumentException("Address format is invalid: to target a module, pass 'Module|<ScriptSlotName>'.",address);
				}				
			}
			
			else if (targetType == Nwn2Type.Area) {
			    if (components.Length != 3) {
			         throw new ArgumentException("Address format is invalid: to target an area, pass 'Area|<name of script slot>|<tag of area>'.",address);
				}
				
				areaTag = components[2];
			}
			
			else {
				if (components.Length < 4 || components.Length > 5) {
					string error = String.Format("Address format is invalid: to target a {0}, pass '{0}|<name of script slot>|<tag of area>|<tag of {0}>|<optional index of {0}>'.",
					                             targetType);
					throw new ArgumentException(error,address);
				}
				
				areaTag = components[2];
				instanceTag = components[3];
				
				if (components.Length == 5) {
					int i;
					bool parsed = Int32.TryParse(components[4],out i);
					
					if (parsed) index = i;
					else {
						throw new ArgumentException("Address format is invalid: '{0}' is not a valid index. Pass an integer of -1 or above for this field.",address);
					}
				}
				else {
					index = -1;
				}
			}
		}
		
		
		public override string ToString()
		{
			return val;
		}
	}
}
