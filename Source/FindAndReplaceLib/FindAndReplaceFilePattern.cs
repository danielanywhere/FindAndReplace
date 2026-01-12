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

using Newtonsoft.Json;

namespace FindAndReplaceLib
{
	//*-------------------------------------------------------------------------*
	//*	FindAndReplaceFilePatternCollection																			*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of FindAndReplaceFilePatternItem Items.
	/// </summary>
	public class FindAndReplaceFilePatternCollection :
		List<FindAndReplaceFilePatternItem>
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
	//*	FindAndReplaceFilePatternItem																						*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Individual find and replace in files pattern entry.
	/// </summary>
	public class FindAndReplaceFilePatternItem
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
		//*	Enabled																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Enabled">Enabled</see>.
		/// </summary>
		private bool mEnabled = true;
		/// <summary>
		/// Get/Set a value indicating whether this entry is enabled for action.
		/// </summary>
		[JsonProperty(Order = 2)]
		public bool Enabled
		{
			get { return mEnabled; }
			set { mEnabled = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Name																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Name">Name</see>.
		/// </summary>
		private string mName = "";
		/// <summary>
		/// Get/Set the name of this action.
		/// </summary>
		[JsonProperty(Order = 0)]
		public string Name
		{
			get { return mName; }
			set { mName = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Remarks																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Remarks">Remarks</see>.
		/// </summary>
		private string mRemarks = "";
		/// <summary>
		/// Get/Set a brief remark for this entry.
		/// </summary>
		[JsonProperty(Order = 1)]
		public string Remarks
		{
			get { return mRemarks; }
			set { mRemarks = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	SourceFiles																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="SourceFiles">SourceFiles</see>.
		/// </summary>
		private List<string> mSourceFiles = new List<string>();
		/// <summary>
		/// Get a reference to the list of source files to search.
		/// </summary>
		[JsonProperty(Order = 3)]
		public List<string> SourceFiles
		{
			get { return mSourceFiles; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	SourceFindPatterns																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for
		/// <see cref="SourceFindPatterns">SourceFindPatterns</see>.
		/// </summary>
		private List<string> mSourceFindPatterns = new List<string>();
		/// <summary>
		/// Get a reference to the patterns to find in the source files.
		/// </summary>
		[JsonProperty(Order = 5)]
		public List<string> SourceFindPatterns
		{
			get { return mSourceFindPatterns; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	TargetFiles																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="TargetFiles">TargetFiles</see>.
		/// </summary>
		private List<string> mTargetFiles = new List<string>();
		/// <summary>
		/// Get a reference to a list of target files to update.
		/// </summary>
		[JsonProperty(Order = 4)]
		public List<string> TargetFiles
		{
			get { return mTargetFiles; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	TargetFindPattern																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for
		/// <see cref="TargetFindPattern">TargetFindPattern</see>.
		/// </summary>
		private string mTargetFindPattern = "";
		/// <summary>
		/// Get/Set the pattern to find on the target file.
		/// </summary>
		[JsonProperty(Order = 6)]
		public string TargetFindPattern
		{
			get { return mTargetFindPattern; }
			set { mTargetFindPattern = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	TargetReplacePattern																									*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for
		/// <see cref="TargetReplacePattern">TargetReplacePattern</see>.
		/// </summary>
		private string mTargetReplacePattern = "";
		/// <summary>
		/// Get/Set the pattern to use as a replacement on all matches found in
		/// the target file.
		/// </summary>
		[JsonProperty(Order = 7)]
		public string TargetReplacePattern
		{
			get { return mTargetReplacePattern; }
			set { mTargetReplacePattern = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	UseRegEx																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="UseRegEx">UseRegEx</see>.
		/// </summary>
		private bool mUseRegEx = false;
		/// <summary>
		/// Get/Set a value indicating whether to use regular expressions on this
		/// pattern.
		/// </summary>
		[JsonProperty(Order = 8)]
		public bool UseRegEx
		{
			get { return mUseRegEx; }
			set { mUseRegEx = value; }
		}
		//*-----------------------------------------------------------------------*


	}
	//*-------------------------------------------------------------------------*

}
