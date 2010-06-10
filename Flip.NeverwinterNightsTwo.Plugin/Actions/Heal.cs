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
 * This file added by Keiron Nicholson on 18/05/2010 at 16:52.
 */

using System;
using System.Collections.Generic;
using System.Text;
using Sussex.Flip.Utils;
using Sussex.Flip.UI;

namespace Sussex.Flip.Games.NeverwinterNightsTwo
{
	public class Heal : Nwn2StatementBehaviour
	{	
		// Apply eEffect to oTarget.
		// void ApplyEffectToObject(int nDurationType, effect eEffect, object oTarget, float fDuration=0.0f);
		
		// int    DURATION_TYPE_INSTANT    = 0;
		
		// Create a Heal effect. This should be applied as an instantaneous effect.
		// * Returns an effect of type EFFECT_TYPE_INVALIDEFFECT if nDamageToHeal < 0.
		// RWT-OEI 03/13/07 - This will now 'heal' doors and placeables.
		// effect EffectHeal(int nDamageToHeal);
		
		public Heal()
		{
			statementType = StatementType.Action;
			parameterCount = 1;
			components = new List<StatementComponent>(2) 
			{ 
				new StatementComponent("heal"),
				new StatementComponent(fitters.OnlyCreaturesOrPlayers)
			};
		}
		
		
		public override string GetCode(params string[] args)
		{
			if (args.Length != parameterCount) {
				throw new ArgumentException("Must pass exactly " + parameterCount + " parameters.","args");
			}	
						
			return String.Format("ApplyEffectToObject(DURATION_TYPE_INSTANT,EffectHeal(1000),{0},0.0f);",args);
		}
		
		
		public override string GetNaturalLanguage(params string[] args)
		{
			if (args.Length != parameterCount) {
				throw new ArgumentException("Must pass exactly " + parameterCount + " parameters.","args");
			}
			
			return String.Format("{0} has all wounds healed",args);
		}
		
		
		public override StatementBehaviour DeepCopy()
		{
			return new Heal();
		}
	}
}