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
	public class StatementComponent
	{
		protected ComponentType componentType;		
		protected Fitter parameterFitter;		
		protected string labelText;
		
		
		public ComponentType ComponentType {
			get { return componentType; }
		}
		
		
		public Fitter ParameterFitter {
			get { return parameterFitter; }
		}
		
		
		public string LabelText {
			get { return labelText; }
		}
		
		
		public StatementComponent(Fitter parameterFitter)
		{
			this.parameterFitter = parameterFitter;			
			this.labelText = null;
			this.componentType = ComponentType.Parameter;
		}
		
		
		public StatementComponent(string labelText)
		{
			this.parameterFitter = null;			
			this.labelText = labelText;
			this.componentType = ComponentType.Label;
		}
	}
}
