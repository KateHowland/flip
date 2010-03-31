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
 * This file added by Keiron Nicholson on 31/03/2010 at 10:50.
 */

using System;
using System.Collections.Generic;
using Sussex.Flip.Utils;

namespace Sussex.Flip.UI
{
	/// <summary>
	/// Description of ActionBehaviour.
	/// </summary>
	public abstract class StatementBehaviour: IDeepCopyable<StatementBehaviour>
	{		
		protected int parameterCount;
		protected List<ComponentInfo> components;
		
		
		public virtual List<ComponentInfo> GetComponents()
		{
			return components;
		}
		
		
		public virtual int GetParameterCount()
		{
			return parameterCount;
		}
		
		
		public abstract string GetCode(params string[] args);
		public abstract string GetNaturalLanguage(params string[] args);
		
		
		public abstract StatementBehaviour DeepCopy();
	}

		
	public class Attacks : StatementBehaviour
	{	
		// Attack oAttackee.
		// - bPassive: If this is TRUE, attack is in passive mode.
		// void ActionAttack(object oAttackee, int bPassive=FALSE);
		
		public Attacks()
		{
			parameterCount = 1;
			components = new List<ComponentInfo>(3) 
			{ 
				new ComponentInfo(new ObjectBlockFitter()),
				new ComponentInfo("attacks"),
				new ComponentInfo(new ObjectBlockFitter())
			};
		}
		
		
		public override string GetCode(params string[] args)
		{
			if (args.Length != parameterCount) {
				throw new ArgumentException("Must pass exactly " + parameterCount + " parameters.","args");
			}
			
			return String.Format("AssignCommand({0}, ActionAttack({1}));",args);
		}
		
		
		public override string GetNaturalLanguage(params string[] args)
		{
			if (args.Length != parameterCount) {
				throw new ArgumentException("Must pass exactly " + parameterCount + " parameters.","args");
			}
			
			return String.Format("{0} attacks {1}",args);
		}
		
		
		public override StatementBehaviour DeepCopy()
		{
			return new Attacks();
		}
	}
	
	
	public class ComponentInfo
	{
		protected bool isParameter;
		protected bool isLabel;
		protected bool isAttribute;				
		protected Fitter parameterFitter;		
		protected string labelText;
		
		
		public Fitter ParameterFitter {
			get { return parameterFitter; }
		}
		
		
		public string LabelText {
			get { return labelText; }
		}
		
		
		public ComponentInfo(Fitter parameterFitter)
		{
			this.parameterFitter = parameterFitter;			
			this.labelText = null;
			this.isAttribute = false;
			this.isParameter = true;
			this.isLabel = false;
		}
		
		
		public ComponentInfo(string labelText)
		{
			this.parameterFitter = null;			
			this.labelText = labelText;
			this.isAttribute = false;
			this.isParameter = false;
			this.isLabel = true;
		}
	}
}
