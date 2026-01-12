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
using System.Text;
using System.Text.RegularExpressions;

using Newtonsoft.Json;

using FindAndReplaceLib;
using static FindAndReplaceLib.FindAndReplaceUtil;

namespace FindAndReplaceInFiles
{
	//*-------------------------------------------------------------------------*
	//*	Program																																	*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Multi-focus Find and Replace in files. Main application instance.
	/// </summary>
	public class Program
	{
		//*************************************************************************
		//*	Private																																*
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//* ProcessMatches																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Process all of the matches and replacements for the current
		/// configuration.
		/// </summary>
		private void ProcessMatches()
		{
			string content = "";
			Match match = null;
			MatchCollection matches = null;
			NameValueCollection matchVariables = new NameValueCollection();
			string text = "";

			//	Get the matches from all source files.
			foreach(FileInfo sourceFileItem in mSourceFiles)
			{
				content = File.ReadAllText(sourceFileItem.FullName);
				foreach(string sourceFindPatternItem in mSourceFindPatterns)
				{
					match = Regex.Match(content, sourceFindPatternItem);
					if(match.Success)
					{
						matches = Regex.Matches(sourceFindPatternItem,
							ResourceMain.rxMatchName);
						foreach(Match matchItem in matches)
						{
							text = GetValue(matchItem, "matchName");
							matchVariables.SetValue(text, GetValue(match, text));
						}
					}
				}
			}

			//	Process the target files.
			if(matchVariables.Count > 0)
			{
				foreach(NameValueItem matchVariableItem in matchVariables)
				{
					mTargetReplacePattern = mTargetReplacePattern.Replace(
						$"${{{matchVariableItem.Name}}}", matchVariableItem.Value);
				}
				foreach(FileInfo targetFileItem in mTargetFiles)
				{
					content = File.ReadAllText(targetFileItem.FullName);
					if(mUseRegex)
					{
						content = Regex.Replace(content,
							mTargetFindPattern, mTargetReplacePattern);
					}
					else
					{
						content = content.Replace(
							mTargetFindPattern, mTargetReplacePattern);
					}
					if(mUseBackup)
					{
						File.Copy(targetFileItem.FullName,
							GetBackupFilename(targetFileItem.FullName));
					}
					File.WriteAllText(targetFileItem.FullName, content);
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ReadPatterns																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Read the find and replace patterns from a JSON data file.
		/// </summary>
		private static string ReadPatterns(Program program, string filename)
		{
			string content = "";
			FileInfo fi = null;
			StringBuilder result = new StringBuilder();

			if(program != null && filename?.Length > 0)
			{
				fi = new FileInfo(filename);
				if(fi.Exists)
				{
					try
					{
						content = File.ReadAllText(filename);
						program.Patterns =
							JsonConvert.
							DeserializeObject<FindAndReplaceFilePatternCollection>(content);
					}
					catch(Exception ex)
					{
						result.Append("Error in Pattern File: " + ex.Message + "\r\n");
					}
				}
				else
				{
					result.Append("Error in Pattern File: File not found...\r\n");
				}
			}
			return result.ToString();
		}
		//*-----------------------------------------------------------------------*

		//*************************************************************************
		//*	Protected																															*
		//*************************************************************************
		//*************************************************************************
		//*	Public																																*
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//*	_Main																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Configure and run the application.
		/// </summary>
		public static void Main(string[] args)
		{
			bool bShowHelp = false; //	Flag - Explicit Show Help.
			char[] comma = new char[] { ',' };
			string key = "";        //	Current Parameter Key.
			string lowerArg = "";   //	Current Lowercase Argument.
			StringBuilder message = new StringBuilder();
			string name = "";
			string patternFilename = "";
			Program prg = new Program();  //	Initialized instance.
			List<string> sourceFiles = new List<string>();
			List<string> targetFiles = new List<string>();
			string text = "";
			string value = "";
			string[] values = null;

			Console.WriteLine("FindAndReplaceInFiles.exe");
			foreach(string arg in args)
			{
				lowerArg = arg.ToLower();
				key = "/?";
				if(lowerArg == key)
				{
					bShowHelp = true;
					continue;
				}
				key = "/backup:";
				if(lowerArg.StartsWith(key))
				{
					prg.UseBackup = Convert.ToBoolean(arg.Substring(key.Length));
					continue;
				}
				key = "/dir";
				if(lowerArg.StartsWith(key))
				{
					bShowHelp = true;
					message.AppendLine("Default directory: " +
						Directory.GetCurrentDirectory() + "\r\n");
				}
				key = "/patternfile:";
				if(lowerArg.StartsWith(key))
				{
					patternFilename = arg.Substring(key.Length);
					continue;
				}
				key = "/regex:";
				if(lowerArg.StartsWith(key))
				{
					prg.UseRegex = Convert.ToBoolean(arg.Substring(key.Length));
					continue;
				}
				key = "/replace:";
				if(lowerArg.StartsWith(key))
				{
					prg.TargetReplacePattern = arg.Substring(key.Length).
						Replace("__r", "\r").
						Replace("__n", "\n").
						Replace("__q", "\"").
						Replace("q__", "\"");
					continue;
				}
				key = "/showarg:";
				if(lowerArg.StartsWith(key))
				{
					message.AppendLine(
						$"Argument parsed: {arg.Substring(key.Length)}\r\n");
					bShowHelp = true;
					continue;
				}
				key = "/sourcefile:";
				if(lowerArg.StartsWith(key))
				{
					sourceFiles.Add(arg.Substring(key.Length));
					continue;
				}
				key = "/sourcefind:";
				if(lowerArg.StartsWith(key))
				{
					prg.mSourceFindPatterns.Add(arg.Substring(key.Length));
					continue;
				}
				key = "/targetfile:";
				if(lowerArg.StartsWith(key))
				{
					targetFiles.Add(arg.Substring(key.Length));
					continue;
				}
				key = "/targetfind:";
				if(lowerArg.StartsWith(key))
				{
					prg.mTargetFindPattern = arg.Substring(key.Length);
					continue;
				}
				key = "/variable:";
				if(lowerArg.StartsWith(key))
				{
					value = arg.Substring(key.Length);
					if(value.IndexOf(',') > -1)
					{
						values = value.Split(comma);
						name = values[0];
						value = values[1];
					}
					else
					{
						name = value;
						value = "";
					}
					prg.mVariables.Add(new NameValueItem()
					{
						Name = name,
						Value = value
					});
				}
				key = "/wait";
				if(lowerArg.StartsWith(key))
				{
					prg.mWaitAfterEnd = true;
					continue;
				}
				key = "/workingpath:";
				if(lowerArg.StartsWith(key))
				{
					WorkingPath =
						GetFullFoldername(
							arg.Substring(key.Length), false, "Working");
				}
			}
			if(!bShowHelp && patternFilename.Length > 0)
			{
				text = ReadPatterns(prg,
					AbsoluteFilename(patternFilename, WorkingPath));
				if(text.Length > 0)
				{
					message.AppendLine(text);
					bShowHelp = (message.Length > 0);
				}
			}
			if(!bShowHelp)
			{
				if(sourceFiles.Count > 0)
				{
					foreach(string sourceFileItem in sourceFiles)
					{
						if(!ValidateFile(sourceFileItem, prg.mSourceFiles, WorkingPath))
						{
							message.AppendLine(
								$"Invalid file pattern: {sourceFileItem}\r\n");
							bShowHelp = true;
						}
					}
				}
				else if(prg.mPatterns.Count == 0)
				{
					message.AppendLine("No source files were specified...");
					bShowHelp = true;
				}
			}
			if(!bShowHelp)
			{
				if(targetFiles.Count > 0)
				{
					foreach(string targetFileItem in targetFiles)
					{
						if(!ValidateFile(targetFileItem, prg.mTargetFiles, WorkingPath))
						{
							message.AppendLine(
								$"Invalid file pattern: {targetFileItem}\r\n");
							bShowHelp = true;
						}
					}
				}
				else if(prg.mPatterns.Count == 0)
				{
					message.AppendLine("No target files were specified...");
					bShowHelp = true;
				}
			}
			if(bShowHelp)
			{
				//	Display Syntax.
				Console.WriteLine(message.ToString() + "\r\n" + ResourceMain.Syntax);
			}
			else
			{
				//	Run the configured application.
				prg.Run();
			}
			if(prg.mWaitAfterEnd)
			{
				Console.WriteLine("Press [Enter] to exit...");
				Console.ReadLine();
			}
		}
		//*-----------------------------------------------------------------------*

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
		public string Name
		{
			get { return mName; }
			set { mName = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Patterns																															*
		//*-----------------------------------------------------------------------*
		private FindAndReplaceFilePatternCollection mPatterns =
			new FindAndReplaceFilePatternCollection();
		/// <summary>
		/// Get/Set a reference to the collection of predefined patterns in this
		/// instance.
		/// </summary>
		public FindAndReplaceFilePatternCollection Patterns
		{
			get { return mPatterns; }
			set { mPatterns = value; }
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
		public string Remarks
		{
			get { return mRemarks; }
			set { mRemarks = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Run																																		*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Run the configured application.
		/// </summary>
		public void Run()
		{
			string content = "";    //	Full file content.
			int count = mSourceFiles.Count;
			FileInfo file = null;
			int index = 0;
			int lineCount = 0;
			int lineIndex = 0;
			Match match = null;
			MatchCollection matches = null;
			NameValueCollection matchVariables = new NameValueCollection();
			FindAndReplaceFilePatternItem pattern = null;
			string text = "";

			if(mVariables.Count > 0)
			{
				//	Resolve external variables.
				foreach(FindAndReplaceFilePatternItem patternItem in mPatterns)
				{
					if(patternItem.Enabled)
					{
						foreach(NameValueItem variableItem in mVariables)
						{
							lineCount = patternItem.SourceFindPatterns.Count;
							for(lineIndex = 0; lineIndex < lineCount; lineIndex ++)
							{
								text = patternItem.SourceFindPatterns[lineIndex];
								text = text.Replace(
									$"%{variableItem.Name}%", variableItem.Value);
								patternItem.SourceFindPatterns[lineIndex] = text;
							}
							patternItem.TargetFindPattern =
								patternItem.TargetFindPattern.Replace(
									$"%{variableItem.Name}%", variableItem.Value);
							patternItem.TargetReplacePattern =
								patternItem.TargetReplacePattern.Replace(
									$"%{variableItem.Name}%", variableItem.Value);
						}
					}
				}
			}
			if(mPatterns.Count > 0)
			{
				//	In this version, every entry from the patterns collection is
				//	loaded into the local variables and processed from here.
				foreach(FindAndReplaceFilePatternItem patternItem in mPatterns)
				{
					//	Configure the local variables with the pattern content.
					mEnabled = patternItem.Enabled;
					mName = patternItem.Name;
					mRemarks = patternItem.Remarks;
					mSourceFiles.Clear();
					foreach(string sourceFileItem in patternItem.SourceFiles)
					{
						ValidateFile(sourceFileItem, mSourceFiles, WorkingPath);
					}
					mSourceFindPatterns.Clear();
					mSourceFindPatterns.AddRange(patternItem.SourceFindPatterns);
					mTargetFiles.Clear();
					foreach(string targetFileItem in patternItem.TargetFiles)
					{
						ValidateFile(targetFileItem, mTargetFiles, WorkingPath);
					}
					mTargetFindPattern = patternItem.TargetFindPattern;
					mTargetReplacePattern = patternItem.TargetReplacePattern;
					mUseRegex = patternItem.UseRegEx;

					ProcessMatches();
				}
				Console.WriteLine("Find And Replace in Files Finished...");
			}
			else if(mSourceFiles.Count > 0 && mSourceFindPatterns.Count > 0)
			{
				ProcessMatches();
				Console.WriteLine("Find And Replace in Files Finished...");
			}
			else
			{
				//	No files.
				Console.WriteLine("No matching files found...");
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	SourceFiles																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="SourceFiles">SourceFiles</see>.
		/// </summary>
		private List<FileInfo> mSourceFiles = new List<FileInfo>();
		/// <summary>
		/// Get a reference to the list of source files for this instance.
		/// </summary>
		public List<FileInfo> SourceFiles
		{
			get { return mSourceFiles; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	SourceFindPatterns																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for
		/// <see cref="SourceFindPatterns">SourceFindPatterns</see>
		/// </summary>
		private List<string> mSourceFindPatterns = new List<string>();
		/// <summary>
		/// Get a reference to the list of patterns to find in the source file.
		/// </summary>
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
		private List<FileInfo> mTargetFiles = new List<FileInfo>();
		/// <summary>
		/// Get a reference to the list of target files for this instance.
		/// </summary>
		public List<FileInfo> TargetFiles
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
		/// Get/Set the target replacement pattern to apply.
		/// </summary>
		public string TargetReplacePattern
		{
			get { return mTargetReplacePattern; }
			set { mTargetReplacePattern = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	UseBackup																															*
		//*-----------------------------------------------------------------------*
		private bool mUseBackup = false;
		/// <summary>
		/// Get/Set a value indicating whether to create a backup file before
		/// making changes.
		/// </summary>
		public bool UseBackup
		{
			get { return mUseBackup; }
			set { mUseBackup = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	UseRegex																															*
		//*-----------------------------------------------------------------------*
		private bool mUseRegex = true;
		/// <summary>
		/// Get/Set a value indicating whether to use regular expressions.
		/// </summary>
		public bool UseRegex
		{
			get { return mUseRegex; }
			set { mUseRegex = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Variables																															*
		//*-----------------------------------------------------------------------*
		private NameValueCollection mVariables = new NameValueCollection();
		/// <summary>
		/// Get a reference to the collection of defined names and values.
		/// </summary>
		public NameValueCollection Variables
		{
			get { return mVariables; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	WaitAfterEnd																													*
		//*-----------------------------------------------------------------------*
		private bool mWaitAfterEnd = false;
		/// <summary>
		/// Get/Set a value indicating whether to wait for user input after the
		/// application ends.
		/// </summary>
		public bool WaitAfterEnd
		{
			get { return mWaitAfterEnd; }
			set { mWaitAfterEnd = value; }
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*
}
