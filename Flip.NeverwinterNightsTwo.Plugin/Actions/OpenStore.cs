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
 * This file added by Keiron Nicholson on 17/05/2010 at 12:42.
 */

using System;
using System.Collections.Generic;
using System.Text;
using Sussex.Flip.Games.NeverwinterNightsTwo.Behaviours;
using Sussex.Flip.Utils;
using Sussex.Flip.UI;

namespace Sussex.Flip.Games.NeverwinterNightsTwo
{
	public class OpenStore : Nwn2StatementBehaviour
	{	
		// Open oStore for oPC.
		// - nBonusMarkUp is added to the stores default mark up percentage on items sold (-100 to 100)
		// - nBonusMarkDown is added to the stores default mark down percentage on items bought (-100 to 100)
		// void OpenStore(object oStore, object oPC, int nBonusMarkUp=0, int nBonusMarkDown=0);
		
		public OpenStore()
		{
			statementType = StatementType.Action;
			parameterCount = 1;
			components = new List<StatementComponent>(2) 
			{ 
				new StatementComponent("open store"),
				new StatementComponent(fitters.OnlyStores)
			};
		}
		
		
		public override string GetCode(params string[] args)
		{
			if (args.Length != parameterCount) {
				throw new ArgumentException("Must pass exactly " + parameterCount + " parameters.","args");
			}	
			
			StringBuilder code = new StringBuilder("OpenStore(");
			code.Append(args[0]);
			code.Append(",");
			code.Append(PlayerBehaviour.NWScript_GetPlayer);
			code.Append(",0,0);");
			return code.ToString();
		}
		
		
		public override string GetNaturalLanguage(params string[] args)
		{
			if (args.Length != parameterCount) {
				throw new ArgumentException("Must pass exactly " + parameterCount + " parameters.","args");
			}
			
			return String.Format("{0} screen opens",args);
		}
		
		
		public override StatementBehaviour DeepCopy()
		{
			return new OpenStore();
		}
	}
}