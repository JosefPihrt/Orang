# List of Option Values

* [\<ASK_MODE>](#ask_mode)
* [\<ATTRIBUTES>](#attributes)
* [\<COMPARE_OPTIONS>](#compare_options)
* [\<CONFLICT_RESOLUTION>](#conflict_resolution)
* [\<CONTENT_DISPLAY>](#content_display)
* [\<DISPLAY_OPTIONS>](#display_options)
* [\<EXTENSION_OPTIONS>](#extension_options)
* [\<FILE_PROPERTIES>](#file_properties)
* [\<HIGHLIGHT>](#highlight)
* [\<INPUT_OPTIONS>](#input_options)
* [\<MAX_OPTIONS>](#max_options)
* [\<MODIFY_OPTIONS>](#modify_options)
* [\<NAME_PART>](#name_part)
* [\<OUTPUT_OPTIONS>](#output_options)
* [\<PATH_DISPLAY>](#path_display)
* [\<PATTERN_OPTIONS>](#pattern_options)
* [\<PIPE_MODE>](#pipe_mode)
* [\<REGEX_OPTIONS>](#regex_options)
* [\<REPLACE_MODIFY>](#replace_modify)
* [\<REPLACEMENT_OPTIONS>](#replacement_options)
* [\<SORT_OPTIONS>](#sort_options)
* [\<SORT_PROPERTY>](#sort_property)
* [\<SYNTAX_SECTIONS>](#syntax_sections)
* [\<VERBOSITY>](#verbosity)

## \<ASK\_MODE>

| Value |     | Description                             |
| ----- | --- | --------------------------------------- |
| file  | f   | Ask for confirmation after each file\.  |
| value | v   | Ask for confirmation after each value\. |

## \<ATTRIBUTES>

| Value                 |     | Description |
| --------------------- | --- | ----------- |
| archive               |     |             |
| compressed            |     |             |
| directory             | d   |             |
| empty                 | e   |             |
| encrypted             |     |             |
| file                  | f   |             |
| hidden                | h   |             |
| integrity\-stream     |     |             |
| normal                |     |             |
| no\-scrub\-data       |     |             |
| not\-content\-indexed |     |             |
| offline               |     |             |
| read\-only            | r   |             |
| reparse\-point        | rp  |             |
| sparse\-file          |     |             |
| system                | s   |             |
| temporary             |     |             |

## \<COMPARE\_OPTIONS>

| Value          |     | Description                             |
| -------------- | --- | --------------------------------------- |
| none           | n   | Compare files only by name\.            |
| attributes     | a   | Compare file attributes\.               |
| content        | c   | Compare file content\.                  |
| modified\-time | mt  | Compare time a file was last modified\. |
| size           | s   | Compare file size\.                     |

## \<CONFLICT\_RESOLUTION>

| Value     |     | Description                                       |
| --------- | --- | ------------------------------------------------- |
| ask       | a   | Ask when a file or already exists\.               |
| overwrite | o   | Overwrite a file when it already exists\.         |
| suffix    |     | Add suffix to file name if it already exists\.    |
| skip      | s   | Do not copy or move a file if it already exists\. |

## \<CONTENT\_DISPLAY>

| Value            |     | Description                                                                              |
| ---------------- | --- | ---------------------------------------------------------------------------------------- |
| all\-lines       | a   | Display all lines\.                                                                      |
| line             | l   | Display entire line containing the matched value\.                                       |
| unmatched\-lines | u   | Display all lines that do not contain the matched value\.                                |
| value            | v   | Display just the matched value\.                                                         |
| value\-detail    | d   | Display matched value and include information about each value such as index or length\. |
| omit             | o   | Do not display file's content\.                                                          |

## \<DISPLAY\_OPTIONS>

| Value                                          |     | Description                                                            |
| ---------------------------------------------- | --- | ---------------------------------------------------------------------- |
| content=[\<CONTENT_DISPLAY>](#content_display) | c   |                                                                        |
| context=\<NUM>                                 | co  | A number of lines to display before and after matching line\.          |
| context\-before=\<NUM>                         | b   | A number of lines to display before matching line\.                    |
| context\-after=\<NUM>                          | a   | A number of lines to display after matching line\.                     |
| count                                          | c   | Include number of matches in file\.                                    |
| creation\-time                                 | ct  | Include file creation time\.                                           |
| indent=\<INDENT>                               |     | Indentation for a list of results\. Default indentation are 2 spaces\. |
| line\-number                                   | l   | Include line number\.                                                  |
| modified\-time                                 | mt  | Include file last modified time\.                                      |
| no\-align                                      |     | Do not align columns\.                                                 |
| path=[\<PATH_DISPLAY>](#path_display)          | p   |                                                                        |
| size                                           | s   | Include file size\.                                                    |
| separator=\<SEPARATOR>                         | s   | String that separate each value\.                                      |
| summary                                        | su  | Include summary\.                                                      |
| trim\-line                                     |     | Trim leading and trailing white\-space from a line\.                   |

## \<EXTENSION\_OPTIONS>

| Value                        |     | Description                                                                                                             |
| ---------------------------- | --- | ----------------------------------------------------------------------------------------------------------------------- |
| case\-sensitive              | cs  | Use case\-sensitive matching\.                                                                                          |
| culture\-invariant           | ci  | Ignore cultural differences between languages\.                                                                         |
| from\-file                   | f   | Load pattern from a file\.                                                                                              |
| list\-separator=\<SEPARATOR> | ls  | String that separate each value in a list\. Default value is comma \(,\) or newline if the list is loaded from a file\. |
| literal                      | l   | Pattern should be treated as a literal expression and not as a regular expression\.                                     |
| negative                     | ne  | Search succeeds if the regular expression does not match\.                                                              |
| timeout=\<NUM>               |     | Match time\-out interval in seconds\.                                                                                   |

## \<FILE\_PROPERTIES>

| Value                  |     | Description                                                                       |
| ---------------------- | --- | --------------------------------------------------------------------------------- |
| creation\-time=\<DATE> | ct  | Filter files by creation time \(See 'Expression syntax' for other expressions\)\. |
| modified\-time=\<DATE> | mt  | Filter files by modified time \(See 'Expression syntax' for other expressions\)\. |
| size=\<NUM>            | s   | Filter files by size \(See 'Expression syntax' for other expressions\)\.          |

## \<HIGHLIGHT>

| Value              |     | Description                                         |
| ------------------ | --- | --------------------------------------------------- |
| none               | n   | No highlighting\.                                   |
| match              | m   | Highlight match value\.                             |
| replacement        | r   | Highlight replacement value\.                       |
| split              | s   | Highlight split value\.                             |
| empty\-match       | em  | Highlight match value that is empty string\.        |
| empty\-replacement | er  | Highlight replacement value that is empty string\.  |
| empty\-split       | es  | Highlight split value that is empty string\.        |
| empty              | e   | Highlight value that is empty string\.              |
| boundary           | b   | Highlight start and end of the value\.              |
| tab                | t   | Highlight tab character\.                           |
| carriage\-return   | cr  | Highlight carriage return character\.               |
| linefeed           | lf  | Highlight linefeed character\.                      |
| newline            |     | Highlight carriage return and linefeed characters\. |
| space              |     | Highlight space character\.                         |

## \<INPUT\_OPTIONS>

| Value  |     | Description                                                                    |
| ------ | --- | ------------------------------------------------------------------------------ |
| escape | e   | Interpret literals \\a, \\b, \\f, \\n, \\r, \\t and \\v as character escapes\. |

## \<MAX\_OPTIONS>

| Value          |     | Description                                                     |
| -------------- | --- | --------------------------------------------------------------- |
| \<NUM>         |     | Stop searching after \<NUM> matching files and/or directories\. |
| matches=\<NUM> | m   | Stop searching in each file after \<NUM> matches\.              |

## \<MODIFY\_OPTIONS>

| Value                                       |     | Description                                                    |
| ------------------------------------------- | --- | -------------------------------------------------------------- |
| aggregate                                   | ag  | Display list of all values at the end of search\.              |
| aggregate\-only                             | o   | Display only list of all values at the end of search\.         |
| ascending                                   | a   | Sort values in an ascending order\.                            |
| culture\-invariant                          | ci  | Ignore cultural differences between languages\.                |
| descending                                  | d   | Sort values in a descending order\.                            |
| distinct                                    | di  | Return distinct values\.                                       |
| except                                      |     | Return values from first file except values from second file\. |
| intersect                                   |     | Return values that were found in all files\.                   |
| group                                       |     | Group matching files by matched values\.                       |
| ignore\-case                                | i   | Use case\-insensitive matching\.                               |
| remove\-empty                               | re  | Remove values that are empty strings\.                         |
| remove\-white\-space                        | rw  | Remove values that are empty or consist of white\-space\.      |
| sort\-by=[\<SORT_PROPERTY>](#sort_property) |     |                                                                |
| to\-lower                                   | tl  | Convert value to lowercase\.                                   |
| to\-upper                                   | tu  | Convert value to uppercase\.                                   |
| trim                                        | t   | Trim leading and trailing white\-space\.                       |
| trim\-end                                   | te  | Trim trailing white\-space\.                                   |
| trim\-start                                 | ts  | Trim leading white\-space\.                                    |

## \<NAME\_PART>

| Value                    |     | Description                             |
| ------------------------ | --- | --------------------------------------- |
| extension                | e   | Search in file extension\.              |
| full\-name               | f   | Search in full path\.                   |
| name                     | n   | Search in file name and its extension\. |
| name\-without\-extension | w   | Search in file name without extension\. |

## \<OUTPUT\_OPTIONS>

| Value                                |     | Description                                                         |
| ------------------------------------ | --- | ------------------------------------------------------------------- |
| encoding=\<ENCODING>                 | e   |                                                                     |
| verbosity=[\<VERBOSITY>](#verbosity) | v   |                                                                     |
| append                               | a   | If the file exists output will be appended to the end of the file\. |

## \<PATH\_DISPLAY>

| Value    |     | Description                                     |
| -------- | --- | ----------------------------------------------- |
| full     | f   | Display full path\.                             |
| relative | r   | Display path relatively to the base directory\. |
| match    | m   | Display only match\.                            |
| omit     | o   | Do not display path\.                           |

## \<PATTERN\_OPTIONS>

| Value                           |     | Description                                                                                                                                            |
| ------------------------------- | --- | ------------------------------------------------------------------------------------------------------------------------------------------------------ |
| compiled                        |     | Compile the regular expression to an assembly\.                                                                                                        |
| culture\-invariant              | ci  | Ignore cultural differences between languages\.                                                                                                        |
| ecma\-script                    | es  | Enable ECMAScript\-compliant behavior for the expression\.                                                                                             |
| ends\-with                      | ew  | Pattern should match from the end of the input string\.                                                                                                |
| equals                          | e   | Pattern should match whole input string\.                                                                                                              |
| explicit\-capture               | n   | Do not capture unnamed groups\.                                                                                                                        |
| from\-file                      | f   | Load pattern from a file\.                                                                                                                             |
| group=\<GROUP\_NAME>            | g   |                                                                                                                                                        |
| ignore\-case                    | i   | Use case\-insensitive matching\.                                                                                                                       |
| ignore\-pattern\-whitespace     | x   | Exclude unescaped white\-space from the pattern and enable comments after a number sign \(\#\)\.                                                       |
| list                            | li  | Interpret pattern as a list of patterns any of which is to be matched\. Separator is either comma \(,\) or newline if the list is loaded from a file\. |
| length=\<NUM>                   |     | Include matches whose length matches the expression \(See 'Expression syntax' for other expressions\)\.                                                |
| list\-separator=\<SEPARATOR>    | ls  | String that separate each value in a list\. Default value is comma \(,\) or newline if the list is loaded from a file\.                                |
| literal                         | l   | Pattern should be treated as a literal expression and not as a regular expression\.                                                                    |
| multiline                       | m   | ^ and $ match the beginning and end of each line \(instead of the beginning and end of the input string\)\.                                            |
| negative                        | ne  | Search succeeds if the regular expression does not match\.                                                                                             |
| part=[\<NAME_PART>](#name_part) | p   | The part of a file or a directory name that should be matched\.                                                                                        |
| right\-to\-left                 | r   | Specifies that the search will be from right to left\.                                                                                                 |
| singleline                      | s   | The period \(\.\) matches every character \(instead of every character except \\n\)\.                                                                  |
| starts\-with                    | sw  | Pattern should match from the start of the input string\.                                                                                              |
| timeout=\<NUM>                  |     | Match time\-out interval in seconds\.                                                                                                                  |
| whole\-line                     | wl  | Pattern should match whole line\.                                                                                                                      |
| whole\-word                     | w   | Pattern should match whole word\.                                                                                                                      |

## \<PIPE\_MODE>

| Value |     | Description                                                       |
| ----- | --- | ----------------------------------------------------------------- |
| text  | t   | Use redirected input as a text to be searched\.                   |
| paths | p   | Use redirected input as a list of paths separated with newlines\. |

## \<REGEX\_OPTIONS>

| Value                       |     | Description                                                                                                 |
| --------------------------- | --- | ----------------------------------------------------------------------------------------------------------- |
| compiled                    | c   | Compile the regular expression to an assembly\.                                                             |
| culture\-invariant          | ci  | Ignore cultural differences between languages\.                                                             |
| ecma\-script                | es  | Enable ECMAScript\-compliant behavior for the expression\.                                                  |
| explicit\-capture           | n   | Do not capture unnamed groups\.                                                                             |
| ignore\-case                | i   | Use case\-insensitive matching\.                                                                            |
| ignore\-pattern\-whitespace | x   | Exclude unescaped white\-space from the pattern and enable comments after a number sign \(\#\)\.            |
| multiline                   | m   | ^ and $ match the beginning and end of each line \(instead of the beginning and end of the input string\)\. |
| right\-to\-left             | r   | Specifies that the search will be from right to left\.                                                      |
| singleline                  | s   | The period \(\.\) matches every character \(instead of every character except \\n\)\.                       |

## \<REPLACE\_MODIFY>

| Value              |     | Description                                     |
| ------------------ | --- | ----------------------------------------------- |
| culture\-invariant | ci  | Ignore cultural differences between languages\. |
| to\-lower          | tl  | Convert value to lowercase\.                    |
| to\-upper          | tu  | Convert value to uppercase\.                    |
| trim               | t   | Trim leading and trailing white\-space\.        |
| trim\-end          | te  | Trim trailing white\-space\.                    |
| trim\-start        | ts  | Trim leading white\-space\.                     |

## \<REPLACEMENT\_OPTIONS>

| Value      |     | Description                                                                                                                                                                                                                                                                                                                                                |
| ---------- | --- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| from\-file | f   | Load text from a file whose path is specified in \<REPLACEMENT> value\.                                                                                                                                                                                                                                                                                    |
| literal    | l   | Replacement should be treated as a literal expression and not as a replacement expression\.                                                                                                                                                                                                                                                                |
| escape     | e   | Interpret literals \\a, \\b, \\f, \\n, \\r, \\t and \\v as character escapes\.                                                                                                                                                                                                                                                                             |
| csharp     | cs  | \<REPLACEMENT> is either expression\-body of a method with signature 'string M\(Match match\)' or a path to a code file that contains public method with signature 'string M\(Match match\)'\. Imported namespaces \(when inline expression is specified\): System System\.Collections\.Generic System\.Linq System\.Text System\.Text\.RegularExpressions |
| from\-dll  |     | \<REPLACEMENT> is a path to a method in DLL file\. The format is 'DllPath,FullTypeName\.MethodName'\.                                                                                                                                                                                                                                                      |

## \<SORT\_OPTIONS>

| Value             |     | Description                        |
| ----------------- | --- | ---------------------------------- |
| ascending         | a   | Sort items in ascending order\.    |
| creation\-time    | ct  | Sort items by creation time\.      |
| descending        | d   | Sort items in descending order\.   |
| max\-count=\<NUM> | m   | Show only \<NUM> items\.           |
| modified\-time    | mt  | Sort items by last modified time\. |
| name              | n   | Sort items by full name\.          |
| size              | s   | Sort items by size\.               |

## \<SORT\_PROPERTY>

| Value  |     | Description                     |
| ------ | --- | ------------------------------- |
| length | l   | Sort values by value's length\. |

## \<SYNTAX\_SECTIONS>

| Value                     |     | Description |
| ------------------------- | --- | ----------- |
| alternation\-constructs   | ac  |             |
| anchors                   | a   |             |
| backreference\-constructs | b   |             |
| character\-classes        | c   |             |
| character\-escapes        | ce  |             |
| general\-categories       | gc  |             |
| grouping\-constructs      | g   |             |
| miscellaneous             | m   |             |
| named\-blocks             | n   |             |
| options                   | o   |             |
| quantifiers               | q   |             |
| substitutions             | s   |             |

## \<VERBOSITY>

| Value      |     | Description |
| ---------- | --- | ----------- |
| quiet      | q   |             |
| minimal    | m   |             |
| normal     | n   |             |
| detailed   | d   |             |
| diagnostic | di  |             |

## Expression Syntax

| Expression            | Description                               |
| --------------------- | ----------------------------------------- |
| `x=n`                 |
| `x<n`                 |
| `x>n`                 |
| `x<=n`                |
| `x>=n`                |
| `x=<min;max>`         | Inclusive interval                        |
| `x=(min;max)`         | Exclusive interval                        |
| `x=-d\|[d.]hh:mm[:ss]` | x is greater than actual date \- \<VALUE> |

*\(Generated with [DotMarkdown](http://github.com/JosefPihrt/DotMarkdown)\)*