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
 * This file added by Keiron Nicholson on 14/04/2010 at 11:27.
 */

using System;
using Sussex.Flip.Core;

namespace Sussex.Flip.Games.NeverwinterNightsTwo.Utils
{
	/// <summary>
	/// Description of Nwn2AddressFactory.
	/// </summary>
	public sealed class Nwn2AddressFactory
	{
		public Nwn2Address GetModuleAddress(string scriptSlot)
		{
			return new Nwn2Address(String.Format("Module{0}{1}",Nwn2Address.Separator,scriptSlot));
		}
		
		
		public Nwn2Address GetAreaAddress(string scriptSlot, string areaTag)
		{
			return new Nwn2Address(String.Format("Area{0}{1}{0}{2}",Nwn2Address.Separator,scriptSlot,areaTag));
		}
		
		
		public Nwn2Address GetInstanceAddress(string scriptSlot, string areaTag, Nwn2Type targetType, string instanceTag)
		{
			return GetInstanceAddress(scriptSlot,areaTag,targetType,instanceTag,-1);
		}
		
		
		public Nwn2Address GetInstanceAddress(string scriptSlot, string areaTag, Nwn2Type targetType, string instanceTag, int index)
		{
			return new Nwn2Address(String.Format("{3}{0}{1}{0}{2}{0}{4}{0}{5}",Nwn2Address.Separator,scriptSlot,areaTag,targetType,instanceTag,index));
		}
		
		
		public Nwn2ConversationAddress GetConversationAddress(string conversationName, Guid lineID, ScriptType scriptType)
		{
			return new Nwn2ConversationAddress(String.Format("Conversation{0}{1}{0}{2}{0}{3}",Nwn2ConversationAddress.Separator,conversationName,lineID.ToString(),scriptType));
		}
	}
}