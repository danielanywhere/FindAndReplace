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

using Newtonsoft.Json;

using FindAndReplaceLib;
using static FindAndReplaceLib.FindAndReplaceUtil;

namespace FindAndReplace
{
	//*-------------------------------------------------------------------------*
	//*	Program																																	*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Single-focus Find and Replace in files. Main application instance.
	/// </summary>
	public class Program
	{
		//*************************************************************************
		//*	Private																																*
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//*	GetBackupFilename																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the backup version of the specified filename.
		/// </summary>
		private static string GetBackupFilename(string pathname)
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
							DeserializeObject<FindAndReplacePatternCollection>(content);
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
			bool bOutputFileRequired = false;
			char[] comma = new char[] { ',' };
			List<string> inputFilenames = new List<string>();
			string key = "";        //	Current Parameter Key.
			string lowerArg = "";   //	Current Lowercase Argument.
			StringBuilder message = new StringBuilder();
			string name = "";
			string outputFilename = "";
			string patternFilename = "";
			Program prg = new Program();  //	Initialized instance.
			string text = "";
			string value = "";
			string[] values = null;

			Console.WriteLine("FindAndReplace.exe");
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
				key = "/files:";
				if(lowerArg.StartsWith(key))
				{
					if(!bOutputFileRequired)
					{
						inputFilenames.Add(arg.Substring(key.Length));
					}
					continue;
				}
				key = "/find:";
				if(lowerArg.StartsWith(key))
				{
					prg.FindPattern = arg.Substring(key.Length);
					continue;
				}
				key = "/infile:";
				if(lowerArg.StartsWith(key))
				{
					text = arg.Substring(key.Length);
					inputFilenames.Clear();
					inputFilenames.Add(text);
					bOutputFileRequired = true;
					continue;
				}
				key = "/outfile:";
				if(lowerArg.StartsWith(key))
				{
					outputFilename = arg.Substring(key.Length);
					bOutputFileRequired = true;
					continue;
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
					prg.ReplacePattern = arg.Substring(key.Length).
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
					continue;
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
					continue;
				}
			}
			//	Filenames are not tested until after the parameter list has been
			//	read in order to allow arguments to be specified in any order.
			if(inputFilenames.Count > 0)
			{
				foreach(string filenameItem in inputFilenames)
				{
					if(!ValidateFile(filenameItem, prg.mFiles, WorkingPath, true))
					{
						if(bOutputFileRequired)
						{
							//	infile, outfile pair.
							message.AppendLine(
								$"Invalid input filename: {filenameItem}\r\n");
							bShowHelp = true;
						}
						else
						{
							//	All files are I/O.
							message.AppendLine(
								$"Invalid file pattern: {filenameItem}\r\n");
							bShowHelp = true;
						}
					}
				}
				if(outputFilename.Length > 0)
				{
					text = ValidateIndividualFile(outputFilename, WorkingPath,
						create: true, quiet: true);
					if(text.Length > 0)
					{
						prg.mOutputFile = new FileInfo(text);
					}
					else
					{
						message.AppendLine(
							$"Invalid output filename: {outputFilename}\r\n");
						bShowHelp = true;
					}
				}
			}
			if(patternFilename.Length > 0)
			{
				text = ValidateIndividualFile(patternFilename, WorkingPath,
					create: false, quiet: true);
				if(text.Length > 0)
				{
					text = ReadPatterns(prg, text);
					if(text.Length > 0)
					{
						message.AppendLine(text);
						bShowHelp = true;
					}
				}
				else
				{
					message.AppendLine(
						$"Invalid pattern filename: {patternFilename}");
				}
			}
			if(!bShowHelp && bOutputFileRequired &&
				(prg.mFiles.Count < 1 || prg.mOutputFile == null))
			{
				message.AppendLine(
					"Input and output files are required in this mode.");
				bShowHelp = true;
			}
			if(!bShowHelp && !(prg.Patterns?.Count > 0))
			{
				message.AppendLine("Patterns not defined...");
				bShowHelp = true;
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
		//*	FilePattern																														*
		//*-----------------------------------------------------------------------*
		private string mFilePattern = "";
		/// <summary>
		/// Get/Set the pattern to load in files.
		/// </summary>
		public string FilePattern
		{
			get { return mFilePattern; }
			set { mFilePattern = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Files																																	*
		//*-----------------------------------------------------------------------*
		private List<FileInfo> mFiles = new List<FileInfo>();
		/// <summary>
		/// Get a reference to the array of files to be searched.
		/// </summary>
		public List<FileInfo> Files
		{
			get { return mFiles; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	FindPattern																														*
		//*-----------------------------------------------------------------------*
		private string mFindPattern = "";
		/// <summary>
		/// Get/Set the pattern to find in the file.
		/// </summary>
		public string FindPattern
		{
			get { return mFindPattern; }
			set { mFindPattern = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	OutputFile																														*
		//*-----------------------------------------------------------------------*
		private FileInfo mOutputFile = null;
		/// <summary>
		/// Get/Set a reference to the output file to be stored.
		/// </summary>
		public FileInfo OutputFile
		{
			get { return mOutputFile; }
			set { mOutputFile = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Patterns																															*
		//*-----------------------------------------------------------------------*
		private FindAndReplacePatternCollection mPatterns =
			new FindAndReplacePatternCollection();
		/// <summary>
		/// Get/Set a reference to the collection of predefined patterns in this
		/// instance.
		/// </summary>
		public FindAndReplacePatternCollection Patterns
		{
			get { return mPatterns; }
			set { mPatterns = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ReplacePattern																												*
		//*-----------------------------------------------------------------------*
		private string mReplacePattern = "";
		/// <summary>
		/// Get/Set the replacement pattern to apply.
		/// </summary>
		public string ReplacePattern
		{
			get { return mReplacePattern; }
			set { mReplacePattern = value; }
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
			int count = mFiles.Count;
			FileInfo file = null;
			int index = 0;
			int lineCount = 0;
			int lineIndex = 0;

			if(mVariables.Count > 0)
			{
				//	Resolve external variables.
				foreach(FindAndReplacePatternItem patternItem in mPatterns)
				{
					if(patternItem.Enabled)
					{
						foreach(NameValueItem variableItem in mVariables)
						{
							patternItem.FindPattern = patternItem.FindPattern.Replace(
								$"%{variableItem.Name}%", variableItem.Value);
							patternItem.ReplacePattern = patternItem.ReplacePattern.Replace(
								$"%{variableItem.Name}%", variableItem.Value);
							foreach(FileReplacePatternItem filePatternItem in
								patternItem.FileReplacePatterns)
							{
								lineCount = filePatternItem.Pattern.Count;
								for(lineIndex = 0; lineIndex < lineCount; lineIndex++)
								{
									filePatternItem.Pattern[lineIndex] =
										filePatternItem.Pattern[lineIndex].Replace(
											$"%{variableItem.Name}%", variableItem.Value);
								}
							}
						}
					}
					//else
					//{
					//	Console.WriteLine($"Skipping: {patternItem.Name}");
					//}
				}
			}
			if(count > 0)
			{
				//	Files are present.
				if(mPatterns.Count == 0 && mFindPattern?.Length > 0)
				{
					//	Pattern was expressed on the command line.
					mPatterns.Add(new FindAndReplacePatternItem()
					{
						FindPattern = mFindPattern,
						ReplacePattern = mReplacePattern,
						UseRegEx = mUseRegex
					});
				}
				if(mPatterns.Count > 0)
				{
					for(index = 0; index < count; index++)
					{
						file = mFiles[index];
						Console.WriteLine($"File: {file.Name}");
						if(mUseBackup)
						{
							if(mOutputFile != null)
							{
								File.Copy(mOutputFile.FullName,
									GetBackupFilename(mOutputFile.FullName));
							}
							else
							{
								File.Copy(file.FullName, GetBackupFilename(file.FullName));
							}
						}
						content = File.ReadAllText(file.FullName);
						content = mPatterns.Replace(file.Name, content);
						if(mOutputFile != null)
						{
							File.WriteAllText(mOutputFile.FullName, content);
						}
						else
						{
							File.WriteAllText(file.FullName, content);
						}
					}
				}
				Console.WriteLine("Find And Replace Finished...");
			}
			else
			{
				//	No files.
				Console.WriteLine("No matching files found...");
			}
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
