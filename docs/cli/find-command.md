# `orang find`

Searches the file system for files and directories and optionally searches files' content\.

## Synopsis

```
orang find [<PATH>]
[   --ask]                <ASK_MODE>
[-a|--attributes]         <ATTRIBUTES>
[-b|--attributes-to-skip] <ATTRIBUTES>
[-c|--content]            <REGEX>
[-y|--display]            <DISPLAY_OPTIONS>
[   --encoding]           <ENCODING>
[-e|--extension]          <EXTENSION_FILTER>
[-h|--help]
[-t|--highlight]          <HIGHLIGHT>
[-i|--include-directory]  <REGEX>
[-m|--max-count]          <MAX_OPTIONS>
[   --modify]             <MODIFY_OPTIONS>
[-n|--name]               <REGEX>
[   --no-recurse]
[-o|--output]             <OUTPUT_OPTIONS>
[   --paths]              <PATH>
[   --paths-from]         <FILE_PATH>
[   --progress]
[-p|--properties]         <FILE_PROPERTIES>
[-s|--sort]               <SORT_OPTIONS>
[-v|--verbosity]          <VERBOSITY>
```

## Arguments

**`<PATH>`**

Path to one or more files and/or directories that should be searched\.

## Options

**`[--ask] <ASK_MODE>`**

Ask for permission after each file or value\. Allowed values are f\[ile\] and v\[alue\]\.

**`[-a|--attributes] <ATTRIBUTES>`**

File attributes that are required\. Allowed values are archive, compressed, d\[irectory\], e\[mpty\], encrypted, f\[ile\], h\[idden\], normal, offline, r\[ead\-only\], r\[eparse\]\-p\[oint\], s\[ystem\] and temporary\.

**`[-b|--attributes-to-skip] <ATTRIBUTES>`**

File attributes that should be skipped\. Allowed values are archive, compressed, e\[mpty\], encrypted, h\[idden\], normal, offline, r\[ead\-only\], r\[eparse\]\-p\[oint\], s\[ystem\] and temporary\.

**`[-c|--content] <REGEX>`**

Regular expression for files' content\. Syntax is \<PATTERN> \[\<PATTERN\_OPTIONS>\]\. Allowed values are compiled, c\[ulture\-\]i\[nvariant\], e\[cma\-\]s\[cript\], e\[nds\-\]w\[ith\], e\[quals\], n \[explicit\-capture\], f\[rom\-file\], g\[roup\]=\<GROUP\_NAME>, i\[gnore\-case\], x \[ignore\-pattern\-whitespace\], li\[st\], length=\<NUM>, l\[ist\-\]s\[eparator\], l\[iteral\], m\[ultiline\], ne\[gative\], r\[ight\-to\-left\], s\[ingleline\], s\[tarts\-\]w\[ith\], timeout=\<NUM>, w\[hole\-\]l\[ine\] and w\[hole\-word\]\.

**`[-y|--display] <DISPLAY_OPTIONS>`**

Display of the results\. Allowed values are c\[ontent\]=\<CONTENT\_DISPLAY>, t \[context\]=\<NUM>, tb \[context\-before\]=\<NUM>, ta \[context\-after\]=\<NUM>, c\[ount\], c\[reation\-\]t\[ime\], indent=\<INDENT>, l\[ine\-number\], m\[odified\-\]t\[ime\], no\-align, p\[ath\]=\<PATH\_DISPLAY>, s\[ize\], s\[eparator\]=\<SEPARATOR>, su\[mmary\] and trim\-line\.

**`[--encoding] <ENCODING>`**

Encoding to use when a file does not contain byte order mark\. Default encoding is UTF\-8\.

**`[-e|--extension] <EXTENSION_FILTER>`**

A filter for file extensions \(case\-insensitive by default\)\. Syntax is EXT1\[,EXT2,\.\.\.\] \[\<EXTENSION\_OPTIONS>\]\. Allowed values are c\[ase\-\]s\[ensitive\], c\[ulture\-\]i\[nvariant\], f\[rom\-file\], l\[ist\-\]s\[eparator\], l\[iteral\], ne\[gative\] and timeout=\<NUM>\.

**`[-h|--help]`**

Show command line help\.

**`[-t|--highlight] <HIGHLIGHT>`**

Parts of the output to highlight\. Allowed values are n\[one\], m\[atch\], e\[mpty\-\]m\[atch\], e\[mpty\], b\[oundary\], t\[ab\], c\[arriage\-\]r\[eturn\], l\[ine\]f\[eed\], n\[ew\-\]l\[ine\] and space\.

**`[-i|--include-directory] <REGEX>`**

Regular expression for a directory name\. Syntax is \<PATTERN> \[\<PATTERN\_OPTIONS>\]\. Allowed values are compiled, c\[ulture\-\]i\[nvariant\], e\[cma\-\]s\[cript\], e\[nds\-\]w\[ith\], e\[quals\], n \[explicit\-capture\], f\[rom\-file\], g\[roup\]=\<GROUP\_NAME>, i\[gnore\-case\], x \[ignore\-pattern\-whitespace\], li\[st\], length=\<NUM>, l\[ist\-\]s\[eparator\], l\[iteral\], m\[ultiline\], ne\[gative\], p\[art\]=\<NAME\_PART>, r\[ight\-to\-left\], s\[ingleline\], s\[tarts\-\]w\[ith\], timeout=\<NUM>, w\[hole\-\]l\[ine\] and w\[hole\-word\]\.

**`[-m|--max-count] <MAX_OPTIONS>`**

Stop searching after specified number is reached\. Allowed values are \<NUM> and m\[atches\]=\<NUM>\.

**`[--modify] <MODIFY_OPTIONS>`**

Functions to modify results\. Allowed values are ag\[gregate\], ao \[aggregate\-only\], a\[scending\], ci \[culture\-invariant\], d\[escending\], di\[stinct\], except, intersect, i\[gnore\-case\], re\[move\-empty\], rw \[remove\-white\-space\], sort\-by=\<SORT\_PROPERTY>, tl \[to\-lower\], tu \[to\-upper\], t\[rim\], te \[trim\-end\] and ts \[trim\-start\]\.

**`[-n|--name] <REGEX>`**

Regular expression for file or directory name\. Syntax is \<PATTERN> \[\<PATTERN\_OPTIONS>\]\. Allowed values are compiled, c\[ulture\-\]i\[nvariant\], e\[cma\-\]s\[cript\], e\[nds\-\]w\[ith\], e\[quals\], n \[explicit\-capture\], f\[rom\-file\], g\[roup\]=\<GROUP\_NAME>, i\[gnore\-case\], x \[ignore\-pattern\-whitespace\], li\[st\], length=\<NUM>, l\[ist\-\]s\[eparator\], l\[iteral\], m\[ultiline\], ne\[gative\], p\[art\]=\<NAME\_PART>, r\[ight\-to\-left\], s\[ingleline\], s\[tarts\-\]w\[ith\], timeout=\<NUM>, w\[hole\-\]l\[ine\] and w\[hole\-word\]\.

**`[--no-recurse]`**

Do not search subdirectories\.

**`[-o|--output] <OUTPUT_OPTIONS>`**

Path to a file that should store output\. Syntax is \<PATH> \[\<OUTPUT\_OPTIONS>\]\. Allowed values are e\[ncoding\]=\<ENCODING>, v\[erbosity\]=\<VERBOSITY> and a\[ppend\]\.

**`[--paths] <PATH>`**

Path to one or more files and/or directories that should be searched\.

**`[--paths-from] <FILE_PATH>`**

Read the list of paths to search from a file\. Paths should be separated by newlines\.

**`[--progress]`**

Display dot \(\.\) for every hundredth searched file or directory\.

**`[-p|--properties] <FILE_PROPERTIES>`**

A filter for file properties\. Allowed values are c\[reation\-\]t\[ime\], m\[odified\-\]t\[ime\] and s\[ize\]=\<NUM>\.

**`[-s|--sort] <SORT_OPTIONS>`**

Sort matched files and directories\. Allowed values are a\[scending\], c\[reation\-\]t\[ime\], d\[escending\], m\[ax\-count\]=\<NUM>, m\[odified\-\]t\[ime\], n\[ame\] and s\[ize\]\.

**`[-v|--verbosity] <VERBOSITY>`**

The amount of information to display in the log\. Allowed values are q\[uiet\], m\[inimal\], n\[ormal\], d\[etailed\] and di\[agnostic\]\.

## Use Redirected Input

Piped output from a previous command will be treated as a list of paths separated by newlines.

## Samples

### Sample

Find TODO comments in a C# source files in a current directory.

Current directory contains file **pattern.txt** with a following content:

```
(?im)
(?<=
  ^(\ |\t)*/{2,}\ *todo\ *
)
[^\r\n]+
```

#### Syntax

```
orang find ^
 --extension cs ^
 --content "pattern.txt" from-file
```

#### Short Syntax

```
orang find ^
 -e cs ^
 -c "pattern.txt" f
```

### Sample

Display content of new csproj files and pause after each file.

#### Syntax

```
orang find ^
 --extension csproj ^
 --content "<Project Sdk=\"Microsoft.NET.Sdk\">" literal ^
 --display content=all-lines ^
 --ask file
```

#### Short Syntax

```
orang find ^
 -e csproj ^
 -c "<Project Sdk=\"Microsoft.NET.Sdk\">" l ^
 -y c=a ^
 --ask f
```

### Sample

Add copyright banner to each C# file in a current directory.

#### Syntax

```
orang find ^
 --extension cs ^
 --content "banner.txt" from-file literal starts-with negative | ^
orang replace ^
 --content "" starts-with ^
 --replacement "banner.txt" from-file
```

#### Short Syntax

```
orang find ^
 -e cs ^
 -c "banner.txt" f l sw ne | ^
orang replace ^
 -c "" sw ^
 -r "banner.txt" f
```

*\(Generated with [DotMarkdown](http://github.com/JosefPihrt/DotMarkdown)\)*