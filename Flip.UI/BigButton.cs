﻿/*
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
 * This file added by Keiron Nicholson on 14/12/2010 at 22:54
 */

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Sussex.Flip.UI
{
	/// <summary>
	/// Description of BigButton.
	/// </summary>
	public class BigButton : Button
	{
		public BigButton()
		{
			Foreground = Brushes.White;
			FontWeight = FontWeights.ExtraBold;
			FontSize = 16;
			Margin = new Thickness(35,5,5,5);
			Height = 40;
			Width = 70;
			AllowDrop = false;
			Background = Brushes.Firebrick.Clone();
			Background.Freeze();
		}
		
		
		public BigButton(string text) : this()
		{	
			Content = text;
		}
	}
}
