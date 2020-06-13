# List of Allowed Values

* [\<ASK_MODE>](#ask-mode)
* [\<ATTRIBUTES>](#attributes)
* [\<COMPARE_OPTIONS>](#compare-options)
* [\<CONFLICT_RESOLUTION>](#conflict-resolution)
* [\<CONTENT_DISPLAY>](#content-display)
* [\<DISPLAY_OPTIONS>](#display-options)
* [\<EXTENSION_OPTIONS>](#extension-options)
* [\<FILE_PROPERTIES>](#file-properties)
* [\<HIGHLIGHT>](#highlight)
* [\<INPUT_OPTIONS>](#input-options)
* [\<MAX_OPTIONS>](#max-options)
* [\<MODIFY_OPTIONS>](#modify-options)
* [\<NAME_PART>](#name-part)
* [\<OUTPUT_OPTIONS>](#output-options)
* [\<PATH_DISPLAY>](#path-display)
* [\<PATTERN_OPTIONS>](#pattern-options)
* [\<REGEX_OPTIONS>](#regex-options)
* [\<REPLACE_MODIFY>](#replace-modify)
* [\<REPLACEMENT_OPTIONS>](#replacement-options)
* [\<SORT_OPTIONS>](#sort-options)
* [\<SORT_PROPERTY>](#sort-property)
* [\<SYNTAX_SECTIONS>](#syntax-sections)
* [\<VERBOSITY>](#verbosity)

## \<ASK\_MODE>

| Value     | Description                             |
| --------- | --------------------------------------- |
| f\[ile\]  | Ask for confirmation after each file\.  |
| v\[alue\] | Ask for confirmation after each value\. |

## \<ATTRIBUTES>

| Value                  | Description |
| ---------------------- | ----------- |
| archive                |
| compressed             |
| d\[irectory\]          |
| e\[mpty\]              |
| encrypted              |
| f\[ile\]               |
| h\[idden\]             |
| integrity\-stream      |
| normal                 |
| no\-scrub\-data        |
| not\-content\-indexed  |
| offline                |
| r\[ead\-only\]         |
| r\[eparse\]\-p\[oint\] |
| sparse\-file           |
| s\[ystem\]             |
| temporary              |

## \<COMPARE\_OPTIONS>

| Value                  | Description                             |
| ---------------------- | --------------------------------------- |
| n\[one\]               | Compare files only by name\.            |
| a\[ttributes\]         | Compare file attributes\.               |
| c\[ontent\]            | Compare file content\.                  |
| m\[odified\-\]t\[ime\] | Compare time a file was last modified\. |
| s\[ize\]               | Compare file size\.                     |

## \<CONFLICT\_RESOLUTION>

| Value         | Description                                       |
| ------------- | ------------------------------------------------- |
| a\[sk\]       | Ask when a file or already exists\.               |
| o\[verwrite\] | Overwrite a file when it already exists\.         |
| suffix        | Add suffix to file name if it already exists\.    |
| s\[kip\]      | Do not copy or move a file if it already exists\. |

## \<CONTENT\_DISPLAY>

| Value                | Description                                                                              |
| -------------------- | ---------------------------------------------------------------------------------------- |
| a\[ll\-lines\]       | Display all lines\.                                                                      |
| l\[ine\]             | Display entire line containing the matched value\.                                       |
| u\[nmatched\-lines\] | Display all lines that do not contain the matched value\.                                |
| v\[alue\]            | Display just the matched value\.                                                         |
| d \[value\-detail\]  | Display matched value and include information about each value such as index or length\. |

## \<DISPLAY\_OPTIONS>

| Value                           | Description                                                            |
| ------------------------------- | ---------------------------------------------------------------------- |
| c\[ontent\]=\<CONTENT\_DISPLAY> |
| t \[context\]=\<NUM>            | A number of lines to display before and after matching line\.          |
| context\-before=\<NUM>          | A number of lines to display before matching line\.                    |
| context\-after=\<NUM>           | A number of lines to display after matching line\.                     |
| c\[ount\]                       | Include number of matches in file\.                                    |
| c\[reation\-\]t\[ime\]          | Include file creation time\.                                           |
| indent=\<INDENT>                | Indentation for a list of results\. Default indentation are 2 spaces\. |
| l\[ine\-number\]                | Include line number\.                                                  |
| m\[odified\-\]t\[ime\]          | Include file last modified time\.                                      |
| p\[ath\]=\<PATH\_DISPLAY>       |
| s\[ize\]                        | Include file size\.                                                    |
| s\[eparator\]=\<SEPARATOR>      | String that separate each value\.                                      |
| su\[mmary\]                     | Include summary\.                                                      |
| trim\-line                      | Trim leading and trailing white\-space from a line\.                   |

## \<EXTENSION\_OPTIONS>

| Value                      | Description                                                                                                             |
| -------------------------- | ----------------------------------------------------------------------------------------------------------------------- |
| c\[ase\-\]s\[ensitive\]    | Use case\-sensitive matching\.                                                                                          |
| c\[ulture\-\]i\[nvariant\] | Ignore cultural differences between languages\.                                                                         |
| f\[rom\-file\]             | Load pattern from a file\.                                                                                              |
| l\[ist\-\]s\[eparator\]    | String that separate each value in a list\. Default value is comma \(,\) or newline if the list is loaded from a file\. |
| l\[iteral\]                | Pattern should be treated as a literal expression and not as a regular expression\.                                     |
| ne\[gative\]               | Search succeeds if the regular expression does not match\.                                                              |
| timeout=\<NUM>             | Match time\-out interval in seconds\.                                                                                   |

## \<FILE\_PROPERTIES>

| Value                  | Description                                                                       |
| ---------------------- | --------------------------------------------------------------------------------- |
| c\[reation\-\]t\[ime\] | Filter files by creation time \(See 'Expression syntax' for other expressions\)\. |
| m\[odified\-\]t\[ime\] | Filter files by modified time \(See 'Expression syntax' for other expressions\)\. |
| s\[ize\]=\<NUM>        | Filter files by size \(See 'Expression syntax' for other expressions\)\.          |

## \<HIGHLIGHT>

| Value                      | Description                                         |
| -------------------------- | --------------------------------------------------- |
| n\[one\]                   | No highlighting\.                                   |
| m\[atch\]                  | Highlight match value\.                             |
| r\[eplacement\]            | Highlight replacement value\.                       |
| s\[plit\]                  | Highlight split value\.                             |
| e\[mpty\-\]m\[atch\]       | Highlight match value that is empty string\.        |
| e\[mpty\-\]r\[eplacement\] | Highlight replacement value that is empty string\.  |
| e\[mpty\-\]s\[plit\]       | Highlight split value that is empty string\.        |
| e\[mpty\]                  | Highlight value that is empty string\.              |
| b\[oundary\]               | Highlight start and end of the value\.              |
| t\[ab\]                    | Highlight tab character\.                           |
| c\[arriage\-\]r\[eturn\]   | Highlight carriage return character\.               |
| l\[ine\]f\[eed\]           | Highlight linefeed character\.                      |
| n\[ew\-\]l\[ine\]          | Highlight carriage return and linefeed characters\. |
| space                      | Highlight space character\.                         |

## \<INPUT\_OPTIONS>

| Value      | Description                                                                    |
| ---------- | ------------------------------------------------------------------------------ |
| e\[scape\] | Interpret literals \\a, \\b, \\f, \\n, \\r, \\t and \\v as character escapes\. |

## \<MAX\_OPTIONS>

| Value              | Description                                                     |
| ------------------ | --------------------------------------------------------------- |
| \<NUM>             | Stop searching after \<NUM> matching files and/or directories\. |
| m\[atches\]=\<NUM> | Stop searching in each file after \<NUM> matches\.              |

## \<MODIFY\_OPTIONS>

| Value                       | Description                                                    |
| --------------------------- | -------------------------------------------------------------- |
| ag\[gregate\]               | Display list of all values at the end of search\.              |
| ao \[aggregate\-only\]      | Display only list of all values at the end of search\.         |
| a\[scending\]               | Sort values in an ascending order\.                            |
| ci \[culture\-invariant\]   | Ignore cultural differences between languages\.                |
| d\[escending\]              | Sort values in a descending order\.                            |
| di\[stinct\]                | Return distinct values\.                                       |
| except                      | Return values from first file except values from second file\. |
| intersect                   | Return values that were found in all files\.                   |
| i\[gnore\-case\]            | Use case\-insensitive matching\.                               |
| re\[move\-empty\]           | Remove values that are empty strings\.                         |
| rw \[remove\-white\-space\] | Remove values that are empty or consist of white\-space\.      |
| sort\-by=\<SORT\_PROPERTY>  |
| tl \[to\-lower\]            | Convert value to lowercase\.                                   |
| tu \[to\-upper\]            | Convert value to uppercase\.                                   |
| t\[rim\]                    | Trim leading and trailing white\-space\.                       |
| te \[trim\-end\]            | Trim trailing white\-space\.                                   |
| ts \[trim\-start\]          | Trim leading white\-space\.                                    |

## \<NAME\_PART>

| Value                          | Description                             |
| ------------------------------ | --------------------------------------- |
| e\[xtension\]                  | Search in file extension\.              |
| f\[ull\-name\]                 | Search in full path\.                   |
| n\[ame\]                       | Search in file name and its extension\. |
| w \[name\-without\-extension\] | Search in file name without extension\. |

## \<OUTPUT\_OPTIONS>

| Value                      | Description                                                         |
| -------------------------- | ------------------------------------------------------------------- |
| e\[ncoding\]=\<ENCODING>   |
| v\[erbosity\]=\<VERBOSITY> |
| a\[ppend\]                 | If the file exists output will be appended to the end of the file\. |

## \<PATH\_DISPLAY>

| Value        | Description                                     |
| ------------ | ----------------------------------------------- |
| f\[ull\]     | Display full path\.                             |
| r\[elative\] | Display path relatively to the base directory\. |
| m\[atch\]    | Display only match\.                            |
| o\[mit\]     | Do not display path\.                           |

## \<PATTERN\_OPTIONS>

| Value                             | Description                                                                                                                                            |
| --------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------ |
| compiled                          | Compile the regular expression to an assembly\.                                                                                                        |
| c\[ulture\-\]i\[nvariant\]        | Ignore cultural differences between languages\.                                                                                                        |
| e\[cma\-\]s\[cript\]              | Enable ECMAScript\-compliant behavior for the expression\.                                                                                             |
| e\[nds\-\]w\[ith\]                | Pattern should match from the end of the input string\.                                                                                                |
| e\[quals\]                        | Pattern should match whole input string\.                                                                                                              |
| n \[explicit\-capture\]           | Do not capture unnamed groups\.                                                                                                                        |
| f\[rom\-file\]                    | Load pattern from a file\.                                                                                                                             |
| g\[roup\]=\<GROUP\_NAME>          |
| i\[gnore\-case\]                  | Use case\-insensitive matching\.                                                                                                                       |
| x \[ignore\-pattern\-whitespace\] | Exclude unescaped white\-space from the pattern and enable comments after a number sign \(\#\)\.                                                       |
| li\[st\]                          | Interpret pattern as a list of patterns any of which is to be matched\. Separator is either comma \(,\) or newline if the list is loaded from a file\. |
| length=\<NUM>                     | Include matches whose length matches the expression \(See 'Expression syntax' for other expressions\)\.                                                |
| l\[ist\-\]s\[eparator\]           | String that separate each value in a list\. Default value is comma \(,\) or newline if the list is loaded from a file\.                                |
| l\[iteral\]                       | Pattern should be treated as a literal expression and not as a regular expression\.                                                                    |
| m\[ultiline\]                     | ^ and $ match the beginning and end of each line \(instead of the beginning and end of the input string\)\.                                            |
| ne\[gative\]                      | Search succeeds if the regular expression does not match\.                                                                                             |
| p\[art\]=\<NAME\_PART>            | The part of a file or a directory name that should be matched\.                                                                                        |
| r\[ight\-to\-left\]               | Specifies that the search will be from right to left\.                                                                                                 |
| s\[ingleline\]                    | The period \(\.\) matches every character \(instead of every character except \\n\)\.                                                                  |
| s\[tarts\-\]w\[ith\]              | Pattern should match from the start of the input string\.                                                                                              |
| timeout=\<NUM>                    | Match time\-out interval in seconds\.                                                                                                                  |
| w\[hole\-\]l\[ine\]               | Pattern should match whole line\.                                                                                                                      |
| w\[hole\-word\]                   | Pattern should match whole word\.                                                                                                                      |

## \<REGEX\_OPTIONS>

| Value                             | Description                                                                                                 |
| --------------------------------- | ----------------------------------------------------------------------------------------------------------- |
| c\[ompiled\]                      | Compile the regular expression to an assembly\.                                                             |
| c\[ulture\]\-i\[nvariant\]        | Ignore cultural differences between languages\.                                                             |
| e\[cma\]\-s\[cript\]              | Enable ECMAScript\-compliant behavior for the expression\.                                                  |
| n \[explicit\-capture\]           | Do not capture unnamed groups\.                                                                             |
| i\[gnore\-case\]                  | Use case\-insensitive matching\.                                                                            |
| x \[ignore\-pattern\-whitespace\] | Exclude unescaped white\-space from the pattern and enable comments after a number sign \(\#\)\.            |
| m\[ultiline\]                     | ^ and $ match the beginning and end of each line \(instead of the beginning and end of the input string\)\. |
| r\[ight\-to\-left\]               | Specifies that the search will be from right to left\.                                                      |
| s\[ingleline\]                    | The period \(\.\) matches every character \(instead of every character except \\n\)\.                       |

## \<REPLACE\_MODIFY>

| Value                     | Description                                     |
| ------------------------- | ----------------------------------------------- |
| ci \[culture\-invariant\] | Ignore cultural differences between languages\. |
| tl \[to\-lower\]          | Convert value to lowercase\.                    |
| tu \[to\-upper\]          | Convert value to uppercase\.                    |
| t\[rim\]                  | Trim leading and trailing white\-space\.        |
| te \[trim\-end\]          | Trim trailing white\-space\.                    |
| ts \[trim\-start\]        | Trim leading white\-space\.                     |

## \<REPLACEMENT\_OPTIONS>

| Value          | Description                                                                                 |
| -------------- | ------------------------------------------------------------------------------------------- |
| f\[rom\-file\] | Load replacement string from a file whose path is specified in \<REPLACEMENT> value\.       |
| l\[iteral\]    | Replacement should be treated as a literal expression and not as a replacement expression\. |
| e\[scape\]     | Interpret literals \\a, \\b, \\f, \\n, \\r, \\t and \\v as character escapes\.              |

## \<SORT\_OPTIONS>

| Value                  | Description                        |
| ---------------------- | ---------------------------------- |
| a\[scending\]          | Sort items in ascending order\.    |
| c\[reation\-\]t\[ime\] | Sort items by creation time\.      |
| d\[escending\]         | Sort items in descending order\.   |
| m\[ax\-count\]=\<NUM>  | Show only \<NUM> items\.           |
| m\[odified\-\]t\[ime\] | Sort items by last modified time\. |
| n\[ame\]               | Sort items by full name\.          |
| s\[ize\]               | Sort items by size\.               |

## \<SORT\_PROPERTY>

| Value      | Description                     |
| ---------- | ------------------------------- |
| l\[ength\] | Sort values by value's length\. |

## \<SYNTAX\_SECTIONS>

| Value                           | Description |
| ------------------------------- | ----------- |
| a\[lternation\-\]c\[onstructs\] |
| a\[nchors\]                     |
| b\[ackreference\-constructs\]   |
| c\[haracter\-classes\]          |
| c\[haracter\-\]e\[scapes\]      |
| g\[eneral\-\]c\[ategories\]     |
| g\[rouping\-constructs\]        |
| m\[iscellaneous\]               |
| n\[amed\-blocks\]               |
| o\[ptions\]                     |
| q\[uantifiers\]                 |
| s\[ubstitutions\]               |

## \<VERBOSITY>

| Value          | Description |
| -------------- | ----------- |
| q\[uiet\]      |
| m\[inimal\]    |
| n\[ormal\]     |
| d\[etailed\]   |
| di\[agnostic\] |

*\(Generated with [DotMarkdown](http://github.com/JosefPihrt/DotMarkdown)\)*