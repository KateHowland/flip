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
 * This file added by Keiron Nicholson on 04/06/2010 at 13:32.
 */

using System;
using System.Collections.Generic;
using System.Windows;
using Sussex.Flip.Core;
using Sussex.Flip.Games.NeverwinterNightsTwo.Utils;
using Sussex.Flip.UI;
using NWN2Toolset;
using NWN2Toolset.NWN2.Data;
using OEIShared.IO;

namespace Sussex.Flip.Games.NeverwinterNightsTwo
{
	/// <summary>
	/// Description of ScriptHelper.
	/// </summary>
	public class ScriptHelper
	{
		protected Nwn2TriggerFactory triggerFactory;
		protected Nwn2AddressFactory addressFactory;
		
		// TODO: probably combine this with ScriptWriter
		
		public ScriptHelper(Nwn2TriggerFactory triggerFactory)
		{
			if (triggerFactory == null) throw new ArgumentNullException("triggerFactory");
			
			this.triggerFactory = triggerFactory;
			this.addressFactory = new Nwn2AddressFactory();
		}
		
		
		public bool BelongsTo(NWN2GameScript script)
		{
			if (script == null) throw new ArgumentNullException("script");
			
			return script.Name.StartsWith("flipscript");
		}
		
		
		public List<ScriptTriggerTuple> GetScriptsForModule()
		{			
			NWN2GameModule mod = NWN2ToolsetMainForm.App.Module;
			
			if (mod == null) throw new InvalidOperationException("No module is open.");
			
			List<ScriptTriggerTuple> tuples = new List<ScriptTriggerTuple>();
			
			foreach (string slot in Nwn2ScriptSlot.GetScriptSlotNames(Nwn2Type.Module)) {
				
				// OnChat seems to display odd behaviour, so ignore it:
				if (slot == "OnChat") continue;
				
				IResourceEntry resource = typeof(NWN2ModuleInformation).GetProperty(slot,typeof(IResourceEntry)).GetValue(mod.ModuleInfo,null) as IResourceEntry;
				
				if (resource != null) {
					try {
						NWN2GameScript script = new NWN2GameScript(resource);						
						
						if (BelongsTo(script)) {
							
							script.Demand();
							
							string name = script.Name;
							string code = ScriptWriter.ExtractFlipCodeFromNWScript(script.Data);
								
							FlipScript flipScript = new FlipScript(code,name);
							
							Nwn2Address address = addressFactory.GetModuleAddress(slot);
							TriggerControl trigger = triggerFactory.GetTriggerFromAddress(address);
							
							tuples.Add(new ScriptTriggerTuple(flipScript,trigger));
						}
					}
					catch (Exception e) {
						string msg = String.Format("Failed to add resource '{0}' to the list of scripts which can be opened.{1}{1}{2}",resource,Environment.NewLine,e);
						MessageBox.Show(msg);
					}
				}
			}
			
			return tuples;
		}
	}
}
