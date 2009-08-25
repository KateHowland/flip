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
 * This file added by Keiron Nicholson on 25/08/2009 at 14:28.
 */

using System;

namespace Sussex.Flip.Utils.Communication
{
	/// <summary>
	/// Processes and responds to messages received from an 
	/// instance of <see cref="ICommunicator"/>.
	/// </summary>
	public abstract class MessageProcessor
	{
		#region Fields
		
		protected ICommunicator communicator;
		
		private object padlock = new object();
		
		#endregion
		
		#region Properties
		
		/// <summary>
		/// Sends and receives messages to and from other communicators.
		/// Messages received by the <see cref="ICommunicator"/> are 
		/// handled by this <see cref="MessageProcessor"/>.
		/// </summary>
		public ICommunicator Communicator {
			get { return communicator; }
			set { 
				lock (padlock) {
					communicator.Received -= ProcessMessage;
					communicator = value;
					communicator.Received += ProcessMessage;
				}
			}
		}
		
		#endregion
		
		#region Constructors
		
		/// <summary>
		/// Constructs a new instance of <see cref="MessageProcessor"/>.
		/// </summary>
		/// <param name="communicator">A communicator object whose
		/// received messages will be processed by this instance.</param>
		public MessageProcessor(ICommunicator communicator)
		{
			this.Communicator = communicator;
		}
		
		#endregion
		
		#region Methods

		/// <summary>
		/// Processes and responds to messages received.
		/// </summary>
		/// <param name="message">The message to process.</param>
		/// <param name="sender">The sender of the message.</param>
		public abstract void ProcessMessage(object message, object sender);
		
		
		protected void MessageReceived(object sender, MessageEventArgs e)
		{
			ProcessMessage(e.Message,e.Sender);
		}
		
		#endregion
	}
}
