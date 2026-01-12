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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace FindAndReplaceLib
{
	//*-------------------------------------------------------------------------*
	//*	FindAndReplaceUtil																											*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Utility functionality for the find and replace application.
	/// </summary>
	public class FindAndReplaceUtil
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
		//* AbsoluteFilename																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the absolute version of the caller's filename.
		/// </summary>
		/// <param name="filename">
		/// The relative or absolute filename to prepare.
		/// </param>
		/// <param name="workingPath">
		/// The working path to use if the supplied filename is relative.
		/// </param>
		/// <returns>
		/// The fully qualified path and filename.
		/// </returns>
		public static string AbsoluteFilename(string filename, string workingPath)
		{
			string result = "";

			if(filename?.Length > 0)
			{
				if(IsFullPath(filename) || !(workingPath?.Length > 0))
				{
					//	The caller supplied an absolute filename
					//	or no working path was specified.
					result = filename;
				}
				else if(workingPath?.Length > 0)
				{
					//	The filename is relative and
					//	a working path is available.
					result = Path.Combine(workingPath, filename);
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	GetBackupFilename																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the backup version of the specified filename.
		/// </summary>
		public static string GetBackupFilename(string pathname)
		{
			string ext = "";        //	Extension only.
			FileInfo info = null;
			string name = "";       //	Name only.
			string path = "";       //	Path only.
			string result = "";

			if(pathname != null && pathname.Length > 0)
			{
				info = new FileInfo(pathname);
				if(info.DirectoryName.Length > 0)
				{
					path = info.DirectoryName;
					if(!path.EndsWith(@"\"))
					{
						path += @"\";
					}
				}
				ext = info.Extension;
				if(ext.Length > 0)
				{
					name = info.Name.Substring(0, info.Name.Length - ext.Length);
				}
				else
				{
					name = info.Name;
				}
				result = name + "-" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ext;
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	GetFullFoldername																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the fully qualified path of the relatively or fully specified
		/// folder.
		/// </summary>
		/// <param name="foldername">
		/// Relative or absolute name of the folder to retrieve.
		/// </param>
		/// <param name="create">
		/// Value indicating whether the folder can be created if it does not
		/// exist.
		/// </param>
		/// <param name="message">
		/// Message to display with folder name.
		/// </param>
		/// <returns>
		/// Fully qualified path of the specified folder, if found.
		/// Otherwise, an empty string.
		/// </returns>
		public static string GetFullFoldername(string foldername,
			bool create = false, string message = "")
		{
			DirectoryInfo dir = null;
			bool exists = false;
			string result = "";

			if(foldername?.Length == 0)
			{
				//	If no folder was specified, use the current working directory.
				dir = new DirectoryInfo(System.Environment.CurrentDirectory);
			}
			else
			{
				//	Some type of filename has been specified.
				if(foldername.StartsWith("\\") || foldername.StartsWith("/") ||
					foldername.IndexOf(":") > -1)
				{
					//	Absolute.
					dir = new DirectoryInfo(foldername);
				}
				else
				{
					//	Relative.
					dir = new DirectoryInfo(
						Path.Combine(System.Environment.CurrentDirectory, foldername));
				}
				exists = dir.Exists;
				if(!exists && !create)
				{
					Console.WriteLine($"Path not found: {message} {dir.FullName}");
					dir = null;
				}
				else if(!exists && create)
				{
					//	Folder can be created.
					dir.Create();
				}
			}
			if(dir != null)
			{
				Console.WriteLine($"{message} Directory: {dir.FullName}");
				result = dir.FullName;
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetGroupText																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a safe version of text for the provided match group, where 0 is
		/// the entire match and 1+ are 1-based numbered captured groups.
		/// </summary>
		/// <param name="match">
		/// Reference to the match to inspect.
		/// </param>
		/// <param name="index">
		/// The index of the group to retrieve. 0 represents the entire match and
		/// values 1 and higher represent 1-based group captures.
		/// </param>
		/// <returns>
		/// The text of the specified group, if found. Otherwise, an empty string.
		/// </returns>
		public static string GetGroupText(Match match, int index)
		{
			Group group = null;
			string result = "";

			if(match?.Success == true && index > -1 && index <= match.Groups.Count)
			{
				if(index == 0)
				{
					//	Entire match.
					if(match.Value?.Length > 0)
					{
						result = match.Value;
					}
				}
				else
				{
					//	1-based offset in groups.
					group = match.Groups[index - 1];
					if(group != null && group.Value?.Length > 0)
					{
						result = group.Value;
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetMultiline																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return multi-line string content as a single string .
		/// </summary>
		/// <param name="list">
		/// Reference to a list containing the information to append.
		/// </param>
		/// <param name="terminateWithLineEnd">
		/// Value indicating whether the value will terminate with a line-end.
		/// </param>
		public static string GetMultiline(List<string> list,
			bool terminateWithLineEnd = true)
		{
			StringBuilder builder = new StringBuilder();
			int count = 0;
			int index = 0;
			string item = "";

			if(list?.Count > 0)
			{
				count = list.Count;
				for(index = 0; index < count; index ++)
				{
					item = list[index];
					builder.Append(item);
					if(!item.EndsWith(" ") && (terminateWithLineEnd ||
						index + 1 < count))
					{
						builder.Append("\r\n");
					}
				}
			}
			return builder.ToString();
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetValue																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the value of the specified group member in the provided match.
		/// </summary>
		/// <param name="match">
		/// Reference to the match to be inspected.
		/// </param>
		/// <param name="groupName">
		/// Name of the group for which the value will be found.
		/// </param>
		/// <returns>
		/// The value found in the specified group, if found. Otherwise, empty
		/// string.
		/// </returns>
		public static string GetValue(Match match, string groupName)
		{
			string result = "";

			if(match != null && match.Groups[groupName] != null &&
				match.Groups[groupName].Value != null)
			{
				result = match.Groups[groupName].Value;
			}
			return result;
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Return the value of the specified group member in a match found with
		/// the provided source and pattern.
		/// </summary>
		/// <param name="source">
		/// Source string to search.
		/// </param>
		/// <param name="pattern">
		/// Regular expression pattern to apply.
		/// </param>
		/// <param name="groupName">
		/// Name of the group for which the value will be found.
		/// </param>
		/// <returns>
		/// The value found in the specified group, if found. Otherwise, empty
		/// string.
		/// </returns>
		public static string GetValue(string source, string pattern,
			string groupName)
		{
			Match match = null;
			string result = "";

			if(source?.Length > 0 && pattern?.Length > 0 && groupName?.Length > 0)
			{
				match = Regex.Match(source, pattern);
				if(match.Success)
				{
					result = GetValue(match, groupName);
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* IsFullPath																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the specified path name is a fully
		/// qualified path.
		/// </summary>
		/// <param name="pathName">
		/// Path to parse.
		/// </param>
		/// <returns>
		/// Value indicating whether the specified path name is a fully qualified
		/// path.
		/// </returns>
		public static bool IsFullPath(string pathName)
		{
			bool result = pathName?.Length > 0 &&
				Regex.IsMatch(pathName, ResourceMain.rxPathStart);

			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ResolveReplacementVariables																						*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a version of the caller's replacement string where all of the
		/// replacement variables have been resolved with captured groups in the
		/// provided match.
		/// </summary>
		/// <param name="match">
		/// Reference to the regular expression match to inspect.
		/// </param>
		/// <param name="replacementString">
		/// The replacement string containing possible captured group
		/// parameters.
		/// </param>
		/// <returns>
		/// Version of the caller's string where all of the captured group
		/// parameters in the supplied replacement string have been resolved,
		/// if legitimate. Otherwise, an empty string.
		/// </returns>
		public static string ResolveReplacementVariables(Match match,
			string replacementString)
		{
			bool bFound = false;
			int index = 0;
			MatchCollection matches = null;
			ReplacementInfoCollection replacements = new ReplacementInfoCollection();
			string text = "";
			string result = "";

			if(match?.Success == true && replacementString?.Length > 0)
			{
				matches = Regex.Matches(replacementString,
					ResourceMain.rxCaptureGroupReplacement);
				foreach(Match matchItem in matches)
				{
					bFound = false;
					text = GetValue(matchItem, "index");
					if(text.Length > 0)
					{
						//	Normal raw index: $0
						//	0 refers to the entire capture.
						//	1+ are individual groups.
						index = ToInt(text);
						replacements.Add(new ReplacementInfoItem()
						{
							Index = matchItem.Index,
							Length = matchItem.Value.Length,
							Text = GetGroupText(match, index)
						});
						bFound = true;
					}
					if(!bFound)
					{
						text = GetValue(matchItem, "bracketedIndex");
						if(text.Length > 0)
						{
							//	Bracketed index: ${0}
							//	0 refers to the entire capture.
							//	1+ are individual groups.
							index = ToInt(text);
							replacements.Add(new ReplacementInfoItem()
							{
								Index = matchItem.Index,
								Length = matchItem.Value.Length,
								Text = GetGroupText(match, index)
							});
							bFound = true;
						}
					}
					if(!bFound)
					{
						text = GetValue(matchItem, "groupName");
						if(text.Length > 0)
						{
							//	Named capture: ${group}
							replacements.Add(new ReplacementInfoItem()
							{
								Index = matchItem.Index,
								Length = matchItem.Value.Length,
								Text = GetValue(match, text)
							});
							bFound = true;
						}
					}
				}
				if(replacements.Count > 0)
				{
					result =
						ReplacementInfoCollection.Replace(replacementString, replacements);
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ToInt																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Provide fail-safe conversion of string to numeric value.
		/// </summary>
		/// <param name="value">
		/// Value to convert.
		/// </param>
		/// <returns>
		/// Int32 value. 0 if not convertible.
		/// </returns>
		public static int ToInt(object value)
		{
			int result = 0;
			if(value != null)
			{
				result = ToInt(value.ToString());
			}
			return result;
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Provide fail-safe conversion of string to numeric value.
		/// </summary>
		/// <param name="value">
		/// Value to convert.
		/// </param>
		/// <returns>
		/// Int32 value. 0 if not convertible.
		/// </returns>
		public static int ToInt(string value)
		{
			int result = 0;
			try
			{
				result = int.Parse(value);
			}
			catch { }
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ValidateFile																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Validate the caller's specified file pattern and return a value
		/// indicating whether that pattern is valid.
		/// </summary>
		/// <param name="filePath">
		/// The relative or fully-qualified path and filename to validate.
		/// </param>
		/// <param name="fileList">
		/// Reference to the list of files to which the filename should be added
		/// if it is successfully validate.
		/// </param>
		/// <param name="workingPath">
		/// The working path for a relative filename.
		/// </param>
		/// <returns>
		/// Value indicating whether the file was successfully validated.
		/// </returns>
		/// <remarks>
		/// The file pattern can be valid even if there are no files in the
		/// specified directory. It will not be valid, however, if a non-existent
		/// file or directory is specified.
		/// </remarks>
		public static bool ValidateFile(string filePath, List<FileInfo> fileList,
			string workingPath)
		{
			DirectoryInfo dir = null;
			FileInfo file = null;
			string filepattern = "";
			int lastSlash = 0;
			string pathpattern = "";
			string refFilePattern = "";
			bool result = true;

			refFilePattern = AbsoluteFilename(filePath, workingPath);
			lastSlash = Math.Max(
				refFilePattern.LastIndexOf('\\'),
				refFilePattern.LastIndexOf('/'));
			if(lastSlash > -1 && lastSlash < refFilePattern.Length &&
				fileList != null)
			{
				//	A path is specified.
				if(lastSlash + 1 < refFilePattern.Length)
				{
					filepattern = refFilePattern.Substring(lastSlash + 1);
				}
				pathpattern = refFilePattern.Substring(0, lastSlash);
				if(pathpattern.Length == 0)
				{
					pathpattern = Directory.GetCurrentDirectory();
				}
				if(!pathpattern.EndsWith(@"\") && !pathpattern.EndsWith("/"))
				{
					pathpattern += @"\";
				}
				if(filepattern.IndexOf('*') < 0 && filepattern.IndexOf('?') < 0)
				{
					//	No wildcards specified.
					file = new FileInfo(pathpattern + filepattern);
					if(!file.Exists)
					{
						//	This might be a directory name with no trailing symbols.
						dir = new DirectoryInfo(pathpattern + filepattern);
						if(dir.Exists)
						{
							//	This is a directory.
							filepattern = "*";
							pathpattern = refFilePattern;
						}
					}
				}
				if(pathpattern.IndexOf('*') > -1 || pathpattern.IndexOf('?') > -1)
				{
					//	Directory can't contain a wildcard.
					result = false;
					Console.WriteLine("Directory: " + pathpattern +
						" can't contain wildcards...");
				}
			}
			else
			{
				//	Only the filename is specified.
				filepattern = refFilePattern;
			}
			if(result)
			{
				if(pathpattern.Length > 0)
				{
					dir = new DirectoryInfo(pathpattern);
				}
				else
				{
					dir = new DirectoryInfo(Directory.GetCurrentDirectory());
				}
				if(dir.Exists)
				{
					pathpattern = dir.FullName;
					if(!pathpattern.EndsWith(@"\") && !pathpattern.EndsWith("/"))
					{
						pathpattern += @"\";
					}
					if(filepattern.IndexOf('*') > -1 || filepattern.IndexOf('?') > -1 ||
						(filepattern.Length == 0 && pathpattern.Length > 0))
					{
						//	Filename contains wildcards.
						fileList.AddRange(dir.GetFiles(filepattern).ToList());
					}
					else
					{
						//	Specific filename.
						file = new FileInfo(pathpattern + filepattern);
						if(file.Exists)
						{
							fileList.Add(file);
						}
						else
						{
							result = false;
							Console.WriteLine("File not found: " + filepattern);
						}
					}
				}
				else
				{
					result = false;
					Console.WriteLine("Folder not found: " + pathpattern);
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	WorkingPath																														*
		//*-----------------------------------------------------------------------*
		private static string mWorkingPath = "";
		/// <summary>
		/// Get/Set the working path for this session.
		/// </summary>
		public static string WorkingPath
		{
			get { return mWorkingPath; }
			set { mWorkingPath = value; }
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

}
