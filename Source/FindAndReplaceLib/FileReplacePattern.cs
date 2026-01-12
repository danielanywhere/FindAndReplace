/*
 * Copyright (c). 2000-2026 Daniel Patterson, MCSD (danielanywhere).
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <https://www.gnu.org/licenses/>.
 * 
 */

using System.Collections.Generic;

namespace FindAndReplaceLib
{
	//*-------------------------------------------------------------------------*
	//*	FileReplacePatternCollection																						*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of FileReplacePatternItem Items.
	/// </summary>
	public class FileReplacePatternCollection : List<FileReplacePatternItem>
	{
		//*************************************************************************
		//*	Private																																*
		//*************************************************************************
		//*************************************************************************
		//*	Protected																															*
		//*************************************************************************
		//*************************************************************************
		//*	Public																																*
		//*************************************************************************


	}
	//*-------------------------------------------------------------------------*

	//*-------------------------------------------------------------------------*
	//*	FileReplacePatternItem																									*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Information about a replacement when the match is a certain file.
	/// </summary>
	public class FileReplacePatternItem
	{
		//*************************************************************************
		//*	Private																																*
		//*************************************************************************
		//*************************************************************************
		//*	Protected																															*
		//*************************************************************************
		//*************************************************************************
		//*	Public																																*
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//*	Filename																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Filename">Filename</see>.
		/// </summary>
		private string mFilename = "";
		/// <summary>
		/// Get/Set the name of the current file to match.
		/// </summary>
		public string Filename
		{
			get { return mFilename; }
			set { mFilename = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Pattern																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Pattern">Pattern</see>.
		/// </summary>
		private List<string> mPattern = new List<string>();
		/// <summary>
		/// Get a reference to the collection of lines to apply for the replacement
		/// when the specified filename is matched.
		/// </summary>
		public List<string> Pattern
		{
			get { return mPattern; }
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

}
