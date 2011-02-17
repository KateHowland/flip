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
 * This file added by Keiron Nicholson on 01/06/2010 at 17:34.
 */

using System;

namespace Sussex.Flip.Utils
{
    /// <summary>
    /// Various string constants representing file type filters.
    /// </summary>
	public static class FileExtensionFilters
	{
        public const string PROGRAMS = "Applications (*.exe)|*.exe";
        public const string TXT = "Text files (*.txt)|*.txt";
        public const string XML = "XML files (*.xml)|*.xml";
        public const string PICTURES = "Pictures (*.jpg;*.jpeg;*.bmp;*.png;*.gif)|*.jpg;*.jpeg;*.bmp;*.png;*.gif";
        public const string COMPILEDSCRIPTS = "Compiled NWN2 scripts (*.ncs)|*.ncs";
        public const string UNCOMPILEDSCRIPTS = "NWN2 Scripts (*.nss)|*.nss";
        public const string ALL = "All files (*.*)|*.*";
        
        public const string TXT_ALL = TXT + "|" + ALL;
        public const string XML_ALL = XML + "|" + ALL;
        public const string PICTURES_ALL = PICTURES + "|" + ALL;
        public const string UNCOMPILEDSCRIPTS_ALL = UNCOMPILEDSCRIPTS + "|" + ALL;
	}
}