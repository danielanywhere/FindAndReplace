# FindAndReplace Command-Line Application

This application performs advanced batch-style bulk find and replace operations on the contents of files using any combination of either direct text or regular expression replacement.

There are two variations of this application: FindAndReplace.exe and FindAndReplaceInFiles.exe. While both applications open text files and perform find and replace operations in those files, FindAndReplaceInFiles has a more specialized behavior of opening all of the target documents to retrieve the needed variables before making the replacements in the target files. More on that will be documented soon.

## FindAndReplace Command-Line Syntax

Following is the syntax for the FindAndReplace application.

```plaintext
Find and replace text in one or more files.

Syntax:
 FindAndReplace.exe /files:{SourceFilePath}
                    [/?]
                    [/find:{FindPattern}]
                    [/replace:{ReplacePattern}]
                    [/patternfile:{PatternFilename}]
                    [/backup:{BackupOption}][0]
                    [/dir]
                    [/regex:{RegexOption}][1]
                    [/showarg:{TestArgument}]
                    [/variable:{VariableNameAndValue}]
                    [/workingpath:{WorkingPath}]
                    [/wait]
Where:
  BackupOption    - Value indicating whether or not to backup the
                    original file.
                    Choices are 0-No; 1-Yes.
                    Default is 0.
  dir             - If specified, the current working directory name is printed.
  FindPattern     - Pattern to find. If the % character appears anywhere, and
                    this file is being run from a batch file, make sure to
                    escape it with a second %. If not running in a batch file,
                    the second escape is not needed. When including double
                    quote in a quoted string pattern, double up on the quote
                    characters.
  PatternFilename - Name of the JSON find and replace data file to use.
                    The object format of this file is as follows:
                    [
                      {
                        "Name": "GeneralName",
                        "GroupFindPattern": "PatternString",
                        "FindPattern": "PatternString",
                        "ReplacePattern": "PatternString",
                        "FileReplacePatterns": "FileReplacePatternCollection",
                        "UseRegEx": 1|0,
                        "Remarks": "GeneralRemarks"
                      },
                      {
                        ...
                      }
                    ]
                    ... where
                    GroupFindPattern - A pattern used to separate the file into
                    distinct groups before performing find and replace on those
                    sections.
                    FindPattern - The pattern of string to find.
                    ReplacePattern - The replacement pattern to apply,
                    including the 'LoadFileContent(filename)' function, where
                    applicable.
                    FileReplacePatterns - Collection of Filename and Pattern
                    entries to use when replacements are distinct according to
                    the filename of the currently open file. Consider using
                    this parameter for features like Previous and Next button
                    linking, where each page contains different specific
                    details for similar functions.
                    UseRegEx - Value indicating whether to use regular
                    expressions (1) or explicit text find and replace (0).
  RegexOption     - Value indicating whether or not to use Regex.
                    Choices are 0-No; 1-Yes.
                    Default is 1.
  ReplacePattern  - The pattern used to make the replacement. The replacement
                    pattern can contain one or more of the following control
                    patterns.
                    __n - Newline.
                    __q - Double-quote character.
                    __r - Carriage return.
                    q__ - Double-quote character.
  SourceFilePath  - File path and / or filename to locate.
  TestArgument    - If specified, the argument expressed on the command line is
                    repeated back as parsed. Use this parameter to determine
                    how escaped patterns are interpreted.
  VariableNameAndValue  -   Name and value in the pattern of {Name},{Value}
  wait            - If specified, the application will wait for user input
                    after finishing but before exiting.
  WorkingPath     - Fully qualified directory name without a filename in which
                    the files will be opened when a relative name is
                    specified.

```

## FindAndReplaceInFiles Command-Line Syntax

Following is the syntax for the FindAndReplaceInFiles application.

```plaintext
Find and replace text in one or more files.

Syntax:
 FindAndReplaceInFiles.exe /sourceFile:{SourceFilePath}
                    /targetFile:{TargetFilePath}
                    [/sourceFind:{FindPattern}]
                    [/targetFind:{FindPattern}]
                    [/replace:{ReplacePattern}]
                    [/patternfile:{PatternFilename}]
                    [/workingpath:{WorkingPath}]
                    [/variable:{VariableNameAndValue}]
                    [/regex:{RegexOption}][1]
                    [/showarg:{TestArgument}]
                    [/backup:{BackupOption}][0]
                    [/dir]
                    [/wait]
                    [/?]

Where:
  BackupOption    - Value indicating whether or not to backup the
                    original file.
                    Choices are 0-No; 1-Yes.
                    Default is 0.
  dir             - If specified, the current working directory name is printed.
  FindPattern     - Pattern to find. If the % character appears anywhere, and
                    this file is being run from a batch file, make sure to
                    escape it with a second %. If not running in a batch file,
                    the second escape is not needed. When including double
                    quote in a quoted string pattern, double up on the quote
                    characters. For example, a search for "Data Point" should
                    be expressed as ""Data Point"".
  PatternFilename - Name of the JSON find and replace data file to use.
                    The object format of this file is as follows:
                    [
                      {
                        "Name": string,
                        "Remarks": string,
                        "SourceFiles": string[],
                        "TargetFiles": string[],
                        "SourceFindPatterns": string[],
                        "TargetFindPattern": string,
                        "TargetReplacePattern": string
                        "UseRegEx": {0|1}
                      },
                      {
                        ...
                      }
                    ]
                    ... where
                    Name - A general name to provide for the action.
                    Remarks - Brief remarks related to this action.
                    SourceFiles - A list of one or more filenames to be
                        searched for variable values, that can either be
                        relative, if a working path has been provided,
                        or fully qualified.
                    SourceFindPatterns - The patterns to find within the
                        source files.
                    TargetFiles - A list of one or more filenames to be
                        searched for targets, each of which can either be
                        relative, if a working path has been provided,
                        or fully qualified.
                    TargetFindPattern - The pattern to find in the target
                        files.
                    TargetReplacePattern - The pattern to use as a replacement
                        on all matches found in the target file.
                    UseRegEx - Value indicating whether to use regular
                    expressions (1) or explicit text find and replace (0).
  RegexOption     - Value indicating whether or not to use Regex.
                    Choices are 0-No; 1-Yes.
                    Default is 1.
  ReplacePattern  - The pattern used to make the replacement. The replacement
                    pattern can contain one or more of the following control
                    patterns.
                    __n - Newline.
                    __q - Double-quote character.
                    __r - Carriage return.
                    q__ - Double-quote character.
  TestArgument    - If specified, the argument expressed on the command line is
                    repeated back as parsed. Use this parameter to determine
                    how escaped patterns are interpreted.
  VariableNameAndValue  -   Name and value in the pattern of {Name},{Value}
  wait            - If specified, the application will wait for user input
                    after finishing but before exiting.
  WorkingPath     - Fully qualified directory name without a filename in which
                    the files will be opened when a relative name is
                    specified.

```
