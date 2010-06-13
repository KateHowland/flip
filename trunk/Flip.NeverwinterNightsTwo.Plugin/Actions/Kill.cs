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
 * This file added by Keiron Nicholson on 18/05/2010 at 16:43.
 */

using System;
using System.Collections.Generic;
using System.Text;
using Sussex.Flip.Utils;
using Sussex.Flip.UI;

namespace Sussex.Flip.Games.NeverwinterNightsTwo
{
	public class Kill : Nwn2StatementBehaviour
	{	
		// Apply eEffect to oTarget.
		// void ApplyEffectToObject(int nDurationType, effect eEffect, object oTarget, float fDuration=0.0f);
		
		// int    DURATION_TYPE_INSTANT    = 0;
		
		// Create a Death effect
		// - nSpectacularDeath: if this is TRUE, the creature to which this effect is
		//   applied will die in an extraordinary fashion
		// - nDisplayFeedback
		// - nIgnoreDeathImmunity: if TRUE, this Death effect ignores any Death Immunity.
		// - bPurgeEffects: Normally TRUE. If changed to FALSE, then the creature will not
		//                  have their effects purged when they die. This might result
		//                  in weird situations where a creature won't die (Due to a
		//                  bonus HP effect, or somesuch), so should only be used in
		//                  specific situations, such as wanting to preserve a visual
		//                  effect on the dead body.
		// effect EffectDeath(int nSpectacularDeath=FALSE, int nDisplayFeedback=TRUE, int nIgnoreDeathImmunity=FALSE, int bPurgeEffects=TRUE);
				
		/// <summary>
		/// Checks whether the given statement is an asynchronous action (that is,
		/// it does not wait for the action to finish before executing the next command).
		/// </summary>
		public override bool IsAsynchronous { // DoesNotWaitForActionToFinishBeforeProceeding
			get { return false; }
		}
		
		
		public Kill()
		{
			statementType = StatementType.Action;
			parameterCount = 1;
			components = new List<StatementComponent>(2) 
			{ 
				new StatementComponent("kill"),
				new StatementComponent(fitters.OnlyCreaturesOrPlayers)
			};
		}
		
		
		public override string GetCode(params string[] args)
		{
			if (args.Length != parameterCount) {
				throw new ArgumentException("Must pass exactly " + parameterCount + " parameters.","args");
			}	
						
			return String.Format("ApplyEffectToObject(DURATION_TYPE_INSTANT,EffectDeath(FALSE,FALSE,TRUE,TRUE),{0},0.0f);",args);
		}
		
		
		public override string GetNaturalLanguage(params string[] args)
		{
			if (args.Length != parameterCount) {
				throw new ArgumentException("Must pass exactly " + parameterCount + " parameters.","args");
			}
			
			return String.Format("{0} is instantly killed",args);
		}
		
		
		public override StatementBehaviour DeepCopy()
		{
			return new Kill();
		}
	}
}