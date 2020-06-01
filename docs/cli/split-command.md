﻿# `orang split`

Splits the input string into an list of substrings at the positions defined by a regular expression\.

## Synopsis

```
orang split [<PATH>]
 -c|--content    <REGEX>
[-y|--display]   <DISPLAY_OPTIONS>
[-h|--help]
[-t|--highlight] <HIGHLIGHT>
[-i|--input]     <INPUT>
[-m|--max-count] <NUM>
[   --modify]    <MODIFY_OPTIONS>
[   --no-groups]
[-o|--output]    <OUTPUT_OPTIONS>
[-v|--verbosity] <VERBOSITY>
```

## Arguments

**`<PATH>`**

Path to a file that should be analyzed\.

## Options

**`-c|--content <REGEX>`**

Regular expression for the input string\. Syntax is \<PATTERN> \[\<PATTERN\_OPTIONS>\]\. Allowed values are compiled, c\[ulture\-\]i\[nvariant\], e\[cma\-\]s\[cript\], e\[nds\-\]w\[ith\], e\[quals\], n \[explicit\-capture\], f\[rom\-file\], i\[gnore\-case\], x \[ignore\-pattern\-whitespace\], li\[st\], length=\<NUM>, l\[ist\-\]s\[eparator\], l\[iteral\], m\[ultiline\], r\[ight\-to\-left\], s\[ingleline\], s\[tarts\-\]w\[ith\], timeout=\<NUM>, w\[hole\-\]l\[ine\] and w\[hole\-word\]\.

**`[-y|--display] <DISPLAY_OPTIONS>`**

Display of the results\. Allowed values are c\[ontent\]=\<CONTENT\_DISPLAY>, indent=\<INDENT>, s\[eparator\]=\<SEPARATOR> and su\[mmary\]\.

**`[-h|--help]`**

Show command line help\.

**`[-t|--highlight] <HIGHLIGHT>`**

Parts of the output to highlight\. Allowed values are n\[one\], s\[plit\], e\[mpty\-\]s\[plit\], e\[mpty\], b\[oundary\], t\[ab\], c\[arriage\-\]r\[eturn\], l\[ine\]f\[eed\], n\[ew\-\]l\[ine\] and space\.

**`[-i|--input] <INPUT>`**

The input string to be searched\. Syntax is \<INPUT> \[\<INPUT\_OPTIONS>\]\. Allowed value is c\[haracter\-\]e\[scapes\]\.

**`[-m|--max-count] <NUM>`**

Maximum number of times the split can occur\.

**`[--modify] <MODIFY_OPTIONS>`**

Functions to modify results\. Allowed values are ag\[gregate\], ao \[aggregate\-only\], a\[scending\], ci \[culture\-invariant\], d\[escending\], di\[stinct\], except, intersect, i\[gnore\-case\], re\[move\-empty\], rw \[remove\-white\-space\], sort\-by=\<SORT\_PROPERTY>, tl \[to\-lower\], tu \[to\-upper\], t\[rim\], te \[trim\-end\] and ts \[trim\-start\]\.

**`[--no-groups]`**

Do not include groups in the results\.

**`[-o|--output] <OUTPUT_OPTIONS>`**

Path to a file that should store output\. Syntax is \<PATH> \[\<OUTPUT\_OPTIONS>\]\. Allowed values are e\[ncoding\]=\<ENCODING>, v\[erbosity\]=\<VERBOSITY> and a\[ppend\]\.

**`[-v|--verbosity] <VERBOSITY>`**

The amount of information to display in the log\. Allowed values are q\[uiet\], m\[inimal\], n\[ormal\], d\[etailed\] and di\[agnostic\]\.

## Use Redirected Input

Piped output from a previous command will be treated as input.

*\(Generated with [DotMarkdown](http://github.com/JosefPihrt/DotMarkdown)\)*