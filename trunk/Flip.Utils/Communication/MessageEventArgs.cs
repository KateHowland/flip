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
 * This file added by Keiron Nicholson on 25/08/2009 at 14:05.
 */

using System;

namespace Sussex.Flip.Utils.Communication
{
	/// <summary>
	/// Arguments to accompany the event of a message being communicated.
	/// </summary>
	public class MessageEventArgs : EventArgs
	{
		#region Fields
		
		protected object message;
		
		protected object sender;
		
		#endregion
		
		#region Properties
		
		/// <summary>
		/// The message which has been communicated.
		/// </summary>
		public object Message {
			get { return message; }
		}
		
		
		/// <summary>
		/// The sender of the message.
		/// </summary>
		public object Sender {
			get { return sender; }
		}
		
		#endregion
		
		#region Constructors
		
		/// <summary>
		/// Creates a new <see cref="MessageEventArgs"/> instance.
		/// </summary>
		/// <param name="message">The message which has been
		/// communicated.</param>
		public MessageEventArgs(object message)
		{
			this.message = message;
		}
		
		#endregion
	}
}
