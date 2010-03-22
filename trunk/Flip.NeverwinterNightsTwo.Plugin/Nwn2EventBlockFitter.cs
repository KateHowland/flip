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
 * This file added by Keiron Nicholson on 22/03/2010 at 09:37.
 */

using System;
using System.Collections.Generic;
using Sussex.Flip.Games.NeverwinterNightsTwo.Utils;
using Sussex.Flip.UI;

namespace Sussex.Flip.Games.NeverwinterNightsTwo
{
	/// <summary>
	/// Description of EventBlockFitter.
	/// </summary>
	public class Nwn2EventBlockFitter : Fitter
	{		
		protected ObjectBlock raiserBlock;		
		public ObjectBlock RaiserBlock {
			get { return raiserBlock; }
			internal set { raiserBlock = value; }
		}
		
		
		public Nwn2EventBlockFitter() : base()
		{
		}
		
		
		protected ObjectBlockSlot eventRaiserSlot = null;
		protected ObjectBlock GetEventRaiserBlock()
		{
			if (eventRaiserSlot == null) return null;
			else return eventRaiserSlot.Contents as ObjectBlock;
		}
		
		
		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="moveable"></param>
		/// <returns></returns>
		public override bool Fits(Moveable moveable)
		{
			EventBlock block = moveable as EventBlock;		
			if (block == null) return false;
			
			ObjectBlock raiserBlock = GetEventRaiserBlock();
			if (raiserBlock == null) return true; // fit any event if no event raiser is specified
				
			IList<string> validEvents = Nwn2EventRaiserBlockFitter.GetEvents(raiserBlock);
				
			return validEvents.Contains(block.EventName);
		}
	}
}