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
 * This file added by Keiron Nicholson on 27/05/2010 at 10:46.
 */

using System;
using Sussex.Flip.Core;

namespace Sussex.Flip.Games.NeverwinterNightsTwo.Utils
{
	/// <summary>
	/// Description of Nwn2ConversationAddress.
	/// </summary>
	public sealed class Nwn2ConversationAddress
	{
		public const char Separator = '|';
		public static readonly char[] separatorChars = new char[]{Separator};
		private const string formatError = "Address format is invalid: to target a conversation, pass 'Conversation|<name of conversation>|<guid of conversation line>'.";
		
		
		private string val;
		private string conversation;		
		private Guid lineID;
		private ScriptType attachedAs;
		
		
		public string Value {
			get { return val; }
		}
		
		public string Conversation {
			get { return conversation; }
		}
		
		public Guid LineID {
			get { return lineID; }
		}
		
		public ScriptType AttachedAs {
			get { return attachedAs; }
		}
			
		
		public Nwn2ConversationAddress(string address)
		{
			if (address == null) throw new ArgumentNullException("address");
			if (address == String.Empty) throw new ArgumentException("address");			
			
			this.val = address;
			
			string[] components = address.Split(separatorChars,StringSplitOptions.None);
			
			if (components.Length != 4) throw new ArgumentException(formatError,"address");
			
			string targetTypeString = components[0];
			
			if (targetTypeString != "Conversation") throw new ArgumentException(formatError,"address");
			
			try {
				conversation = components[1];
				lineID = new Guid(components[2]);
				attachedAs = (ScriptType)Enum.Parse(typeof(ScriptType),components[3]);
			}
			catch (Exception e) {
				throw new ArgumentException(formatError,"address",e);
			}
		}
		
		
		public static Nwn2ConversationAddress TryCreate(string address)
		{
			try {
				return new Nwn2ConversationAddress(address);
			}
			catch (Exception) {
				return null;
			}
		}
		
		
		public override string ToString()
		{
			return val;
		}
	}
}
