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
 * This file added by Keiron Nicholson on 07/06/2010 at 10:39.
 */

using System;
using Sussex.Flip.Core;
using Sussex.Flip.Utils;

namespace Sussex.Flip.UI
{
	/// <summary>
	/// Description of ScriptTriggerTuple.
	/// </summary>
	public class ScriptTriggerTuple : IDeepCopyable<ScriptTriggerTuple>
	{
		protected FlipScript script;
		protected TriggerControl trigger;
		
		
		public FlipScript Script {
			get { return script; }
			set { script = value; }
		}
		
		
		public TriggerControl Trigger {
			get { return trigger; }
			set { trigger = value; }
		}	
		
		
		public ScriptTriggerTuple(FlipScript script, TriggerControl trigger)
		{
			this.script = script;
			this.trigger = trigger;
		}
		
		
		public ScriptTriggerTuple(FlipScript script) : this(script,null)
		{			
		}
		
		
		public ScriptTriggerTuple DeepCopy()
		{
			FlipScript scriptCopy;
			TriggerControl triggerCopy;
			
			if (script != null) {
				scriptCopy = script.DeepCopy();
			}
			else {
				scriptCopy = null;
			}
			
			if (trigger != null) {
				triggerCopy = (TriggerControl)trigger.DeepCopy();
			}
			else {
				triggerCopy = null;
			}
			
			return new ScriptTriggerTuple(scriptCopy,triggerCopy);
		}
	}
}
