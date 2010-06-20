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
 * This file added by Keiron Nicholson on 08/04/2010 at 15:36.
 */

using System;
using System.Collections.Generic;
using Sussex.Flip.Utils;
using Sussex.Flip.UI;

namespace Sussex.Flip.Games.NeverwinterNightsTwo
{
	public class DisplayMessage : Nwn2StatementBehaviour
	{	
		//RWT-OEI 01/05/06
		//This script function displays a message box popup on the client of the
		//player passed in as the first parameter.
		//////
		// oPC           - The player object of the player to show this message box to
		// nMessageStrRef- The STRREF for the Message Box message.
		// sMessage      - The text to display in the message box. Overrides anything
		//               - indicated by the nMessageStrRef
		// sOkCB         - The callback script to call if the user clicks OK, defaults
		//               - to none. The script name MUST start with 'gui'
		// sCancelCB     - The callback script to call if the user clicks Cancel, defaults
		//               - to none. The script name MUST start with 'gui'
		// bShowCancel   - If TRUE, Cancel Button will appear on the message box.
		// sScreenName   - The GUI SCREEN NAME to use in place of the default message box.
		//               - The default is SCREEN_MESSAGEBOX_DEFAULT
		// nOkStrRef     - The STRREF to display in the OK button, defaults to OK
		// sOkString     - The string to show in the OK button. Overrides anything that
		//               - nOkStrRef indicates if it is not an empty string
		// nCancelStrRef - The STRREF to dispaly in the Cancel button, defaults to Cancel.
		// sCancelString - The string to display in the Cancel button. Overrides anything
		//               - that nCancelStrRef indicates if it is anything besides empty string
		//void DisplayMessageBox( object oPC, int nMessageStrRef..................
				
		/// <summary>
		/// Checks whether the given statement is an asynchronous action (that is,
		/// it does not wait for the action to finish before executing the next command).
		/// </summary>
		public override bool IsAsynchronous { // DoesNotWaitForActionToFinishBeforeProceeding
			get { return true; }
		}
		
				
		public DisplayMessage()
		{
			statementType = StatementType.Action;
			parameterCount = 1;
			components = new List<StatementComponent>(3) 
			{ 
				new StatementComponent("display"),
				new StatementComponent(fitters.OnlyStrings),
				new StatementComponent("as pop-up message"),
			};
		}
		
		
		public override string GetCode(params string[] args)
		{
			if (args.Length != parameterCount) {
				throw new ArgumentException("Must pass exactly " + parameterCount + " parameters.","args");
			}			
			
			return String.Format("DisplayMessageBox(GetFirstPC(),0,{0});",args);
		}
		
		
		public override string GetNaturalLanguage(params string[] args)
		{
			if (args.Length != parameterCount) {
				throw new ArgumentException("Must pass exactly " + parameterCount + " parameters.","args");
			}
			
			if (args[0] == fitters.OnlyStrings.GetMoveableDescription()) return "a message pops up";
			else return String.Format("a message pops up ({0})",args);
		}
		
		
		public override StatementBehaviour DeepCopy()
		{
			return new DisplayMessage();
		}
	}
}