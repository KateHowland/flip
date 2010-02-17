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
 * This file added by Keiron Nicholson on 17/02/2010 at 13:30.
 */

using System;
using System.Collections.Generic;

namespace Sussex.Flip.UI
{
	public class SimpleFitter : Fitter
	{
		protected List<string> types;
		protected List<string> subtypes;
		
		
		public SimpleFitter()
		{			
			this.types = null;
			this.subtypes = null;
		}
		
		
		public SimpleFitter(string type) : this(new List<string>{type},null)
		{			
		}
		
		
		public SimpleFitter(List<string> types) : this(types,null)
		{			
		}
		
		
		public SimpleFitter(string type, string subtype) : this(new List<string>{type},new List<string>{subtype})
		{			
		}
		
		
		public SimpleFitter(string type, List<string> subtypes) : this(new List<string>{type},subtypes)
		{			
		}
		
		
		public SimpleFitter(List<string> types, List<string> subtypes)
		{
			this.types = types;
			this.subtypes = subtypes;
		}
		
		
		public bool Fits(string type, string subtype)
		{
			return (types == null || types.Contains(type)) && (subtypes == null || subtypes.Contains(subtype));
		}
		
		
		public override bool Fits(ObjectBlock block)
		{
			return Fits(block.Type,block.Subtype);
		}
	}
}
