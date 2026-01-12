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

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using static FindAndReplaceLib.FindAndReplaceUtil;

namespace FindAndReplaceLib
{
	//*-------------------------------------------------------------------------*
	//*	FindAndReplacePatternCollection																					*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of FindAndReplacePatternItem Items.
	/// </summary>
	public class FindAndReplacePatternCollection :
		List<FindAndReplacePatternItem>
	{
		//*************************************************************************
		//*	Private																																*
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//* Replace																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Perform all of the replacements on an individual pattern and a selected
		/// body of text.
		/// </summary>
		/// <param name="pattern">
		/// Reference to the selected pattern for this set of replacements.
		/// </param>
		/// <param name="filename">
		/// The name of the currently active filename.
		/// </param>
		/// <param name="source">
		/// Source string within which matching patterns will be located.
		/// </param>
		/// <returns>
		/// The new text where all matching patterns have had appropriate
		/// replacements.
		/// </returns>
		private string Replace(FindAndReplacePatternItem pattern, string filename,
			string source)
		{
			bool bContinue = true;
			string content = "";
			int count = 0;
			FileReplacePatternItem filePattern = null;
			string functionText = "";
			MatchCollection matches = null;
			string paramText = "";
			string replacementPattern = "";
			string replacementResult = "";
			ReplacementInfoCollection replacements = new ReplacementInfoCollection();
			string result = "";

			if(pattern != null && source?.Length > 0)
			{
				result = source;
				if(pattern.UseRegEx)
				{
					bContinue = false;
					//	Per-filename replacements defined?
					if(pattern.FileReplacePatterns.Count > 0)
					{
						filePattern = pattern.FileReplacePatterns.FirstOrDefault(x =>
							x.Filename.ToLower() == filename.ToLower());
						if(filePattern != null)
						{
							//	This file was found in the specifications list.
							replacementPattern = GetMultiline(filePattern.Pattern, false);
							bContinue = true;
						}
					}
					else
					{
						//	The replacement will be made in this file unconditionally.
						replacementPattern = pattern.ReplacePattern;
						bContinue = true;
					}
					if(bContinue)
					{
						//	Check for supported replacement string functions.
						if(Regex.IsMatch(replacementPattern,
							ResourceMain.rxReplacementStringFunctionLoadFileContent))
						{
							//	LoadFileContent.
							//	Prepare the function parameters.
							//	Get matches in source.
							matches = Regex.Matches(result, pattern.FindPattern);
							foreach(Match matchItem in matches)
							{
								replacementResult = ResolveReplacementVariables(matchItem,
									replacementPattern);
								functionText = GetValue(replacementResult,
									ResourceMain.rxReplacementStringFunctionLoadFileContent,
									"functiontext");
								paramText = GetValue(functionText,
									ResourceMain.rxReplacementStringFunctionLoadFileContent,
									"parameters");
								if(Regex.IsMatch(paramText, ResourceMain.rxPathName))
								{
									//	Legal filename.
									try
									{
										content = File.ReadAllText(
											AbsoluteFilename(paramText, WorkingPath));
										replacementResult =
											replacementResult.Replace(functionText, content);
										replacements.Add(new ReplacementInfoItem()
										{
											Index = matchItem.Index,
											Length = matchItem.Value.Length,
											Text = replacementResult
										});
									}
									catch(Exception ex)
									{
										Console.WriteLine(
											$"Error loading file content for {paramText}:");
										Console.WriteLine($" {ex.Message}");
									}
								}
							}
							if(replacements.Count > 0)
							{
								result =
									ReplacementInfoCollection.Replace(result, replacements);
							}
							//	The individual matches have been processed.
							bContinue = false;
						}
						else if(Regex.IsMatch(replacementPattern,
							ResourceMain.rxReplacementStringFunctionLowerCase))
						{
							//	LowerCase.
							//	Prepare the function parameters.
							//	Get matches in source.
							matches = Regex.Matches(result, pattern.FindPattern);
							foreach(Match matchItem in matches)
							{
								replacementResult = ResolveReplacementVariables(matchItem,
									replacementPattern);
								functionText = GetValue(replacementResult,
									ResourceMain.rxReplacementStringFunctionLowerCase,
									"functiontext");
								paramText = GetValue(functionText,
									ResourceMain.rxReplacementStringFunctionLowerCase,
									"parameters");
								content = paramText.ToLower();
								replacementResult =
									replacementResult.Replace(functionText, content);
								replacements.Add(new ReplacementInfoItem()
								{
									Index = matchItem.Index,
									Length = matchItem.Value.Length,
									Text = replacementResult
								});
							}
							if(replacements.Count > 0)
							{
								result =
									ReplacementInfoCollection.Replace(result, replacements);
							}
							//	The individual matches have been processed.
							bContinue = false;
						}
					}
					if(bContinue)
					{
						//	No individual replacements have been done.
						//	Perform a global replacement.
						count = Regex.Matches(result, pattern.FindPattern).Count;
						if(count > 0)
						{
							Console.WriteLine(
								$"Replacing {count} matches for {pattern.FindPattern}");
							result =
								Regex.Replace(result,
								pattern.FindPattern, replacementPattern);
						}
					}
				}
				else
				{
					//	Count the plain text matches.
					count = Regex.Matches(result,
						Regex.Escape(pattern.FindPattern)).Count;
					if(count > 0)
					{
						Console.WriteLine(
							$"Replacing {count} matches for {pattern.FindPattern}");
						result = result.Replace(pattern.FindPattern,
							pattern.ReplacePattern);
					}
				}

			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*************************************************************************
		//*	Protected																															*
		//*************************************************************************
		//*************************************************************************
		//*	Public																																*
		//*************************************************************************

		//*-----------------------------------------------------------------------*
		//*	Replace																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Perform all of the replacements represented by the patterns of this
		/// collection and return the result to the caller.
		/// </summary>
		/// <param name="filename">
		/// Name of the file currently open for inspection.
		/// </param>
		/// <param name="source">
		/// Source content of the file.
		/// </param>
		/// <returns>
		/// The new text where all matching patterns have had appropriate
		/// replacements.
		/// </returns>
		public string Replace(string filename, string source)
		{
			int index = 0;
			MatchCollection matches = null;
			ReplacementInfoCollection replacements = new ReplacementInfoCollection();
			string result = "";

			if(source?.Length > 0)
			{
				result = source;
			}
			foreach(FindAndReplacePatternItem pattern in this)
			{
				//if(index == 9)
				//{
				//	Console.WriteLine("FindAndReplace.Replace: Break here...");
				//}
				if(pattern.Enabled)
				{
					if(pattern.GroupFindPattern.Length > 0)
					{
						//	The group find pattern currently uses regex unconditionally.
						replacements.Clear();
						matches = Regex.Matches(result, pattern.GroupFindPattern);
						foreach(Match matchItem in matches)
						{
							replacements.Add(new ReplacementInfoItem()
							{
								Index = matchItem.Index,
								Length = matchItem.Length,
								Text = Replace(pattern, filename, matchItem.Value)
							});
						}
						if(replacements.Count > 0)
						{
							result =
								ReplacementInfoCollection.Replace(result, replacements);
						}
					}
					else
					{
						//	The entire file will undergo a single find and replace.
						result = Replace(pattern, filename, result);
					}
				}
				else
				{
					Console.WriteLine($" Skipping: {pattern.Name}...");
				}
				index++;
			}
			return result;
		}
		//*-----------------------------------------------------------------------*


	}
	//*-------------------------------------------------------------------------*

	//*-------------------------------------------------------------------------*
	//*	FindAndReplacePatternItem																								*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Portable find and replace pattern.
	/// </summary>
	public class FindAndReplacePatternItem
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
		[JsonProperty(Order = 6)]
		public bool Enabled
		{
			get { return mEnabled; }
			set { mEnabled = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	FileReplacePatterns																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for
		/// <see cref="FileReplacePatterns">FileReplacePatterns</see>.
		/// </summary>
		private FileReplacePatternCollection mFileReplacePatterns =
			new FileReplacePatternCollection();
		/// <summary>
		/// Get a reference to the collection of file-based replacement patterns.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This collection is a set of filenames, each of which might match the
		/// name of the currently loaded file. If the name of the loaded file
		/// matches one of the filenames in the collection, that item's pattern
		/// will become the replacement pattern for the current find pattern.
		/// </para>
		/// <para>
		/// For example, consider the Next button on a given page. Depending upon
		/// which page we are currently viewing, the 'Next' page is going to be
		/// different than any other. To address this, we create a collection
		/// of file replace patterns where conceptually,
		/// Filename[0] = "Page1.htm", Pattern[0] = "Page2.htm";
		/// Filename[1] = "Page2.htm", Pattern[1] = "Page3.htm";
		/// Filename[2] = "Page3.htm", Pattern[2] = "Page4.htm"; etc.
		/// </para>
		/// <para>
		/// To load the content of another file as a replacement for a provided
		/// filename, see the replacement string function
		/// LoadFileContent({filename}).
		/// </para>
		/// </remarks>
		[JsonProperty(Order = 4)]
		public FileReplacePatternCollection FileReplacePatterns
		{
			get { return mFileReplacePatterns; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	FindPattern																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="FindPattern">FindPattern</see>.
		/// </summary>
		private string mFindPattern = "";
		/// <summary>
		/// Get/Set the find pattern for this item.
		/// </summary>
		[JsonProperty(Order = 2)]
		public string FindPattern
		{
			get { return mFindPattern; }
			set { mFindPattern = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	GroupFindPattern																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="GroupFindPattern">GroupFindPattern</see>.
		/// </summary>
		private string mGroupFindPattern = "";
		/// <summary>
		/// Get/Set a pattern used to separate the file into distinct groups
		/// before performing find and replace on those sections.
		/// </summary>
		[JsonProperty(Order = 1)]
		public string GroupFindPattern
		{
			get { return mGroupFindPattern; }
			set { mGroupFindPattern = value; }
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
		/// Private member <see cref="Remarks">Remarks</see>.
		/// </summary>
		private string mRemarks = "";
		/// <summary>
		/// Get/Set any remarks related to this action.
		/// </summary>
		[JsonProperty(Order = 7)]
		public string Remarks
		{
			get { return mRemarks; }
			set { mRemarks = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ReplacePattern																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="ReplacePattern">ReplacePattern</see>.
		/// </summary>
		private string mReplacePattern = "";
		/// <summary>
		/// Get/Set the replacement pattern for this instance.
		/// </summary>
		[JsonProperty(Order = 3)]
		public string ReplacePattern
		{
			get { return mReplacePattern; }
			set { mReplacePattern = value; }
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
		[JsonProperty(Order = 5)]
		public bool UseRegEx
		{
			get { return mUseRegEx; }
			set { mUseRegEx = value; }
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*


}
