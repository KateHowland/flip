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
 * This file added by Keiron Nicholson on 15/03/2010 at 13:58.
 */

using System;

namespace Sussex.Flip.UI
{
	/// <summary>
	/// Description of MoveableProvider.
	/// </summary>
	public abstract class MoveableProvider
	{
		public const string EventsBagName = "Events";
		public const string ActionsBagName = "Actions";
		public const string ConditionsBagName = "Conditions";
		public const string ControlBagName = "Control";
		public const string BooleanExpressionsBagName = "Booleans";
		
		
		protected IMoveableManager manager;
		
		
		public void Populate(IMoveableManager manager)
		{
			if (manager == null) throw new ArgumentNullException("manager");			
			this.manager = manager;
			
			CreateStandardBagsAndBlocks();
			CreateBags();
			PopulateBags();
		}
		
		
		public void Refresh(IMoveableManager manager)
		{
			if (manager == null) throw new ArgumentNullException("manager");			
			this.manager = manager;
			
			EmptyBags();
			PopulateBags();
		}
		
		
		protected void EmptyBags()
		{
			if (manager == null) throw new InvalidOperationException("Need an IMoveableManager to work with.");
			
			foreach (string bag in manager.GetBags()) {
				if (bag != ControlBagName && bag != BooleanExpressionsBagName) {
					manager.EmptyBag(bag);
				}
			}
		}
		
		
		protected virtual void CreateStandardBagsAndBlocks()
		{
			if (manager == null) throw new InvalidOperationException("Need an IMoveableManager to work with.");
						
			manager.AddBag(EventsBagName,false);
			manager.AddBag(ActionsBagName,false);
			manager.AddBag(ConditionsBagName,false);
			manager.AddBag(ControlBagName,false);
			manager.AddBag(BooleanExpressionsBagName,false);
			
			manager.AddMoveable(ControlBagName,new IfControl());
			manager.AddMoveable(ControlBagName,new IfElseControl());
//			manager.AddMoveable(ControlBagName,new WhileControl());
//			manager.AddMoveable(ControlBagName,new DoWhileControl());		
			
			manager.AddMoveable(BooleanExpressionsBagName,new OrBlock());
			manager.AddMoveable(BooleanExpressionsBagName,new AndBlock());
			manager.AddMoveable(BooleanExpressionsBagName,new NotBlock());
		}
		
		
		protected abstract void CreateBags();
		protected abstract void PopulateBags();
	}
}