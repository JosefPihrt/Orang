# `orang delete`

Deletes files and directories\.

## Synopsis

```
orang delete [<PATH>]
[   --ask]
[-a|--attributes]         <ATTRIBUTES>
[-b|--attributes-to-skip] <ATTRIBUTES>
[-c|--content]            <REGEX>
[   --content-only]
[-y|--display]            <DISPLAY_OPTIONS>
[-d|--dry-run]
[   --encoding]           <ENCODING>
[-e|--extension]          <EXTENSION_FILTER>
[-h|--help]
[-t|--highlight]          <HIGHLIGHT>
[-i|--include-directory]  <REGEX>
[   --including-bom]
[-m|--max-count]          <NUM>
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

**`[--ask]`**

Ask for a permission to delete file or directory\.

**`[-a|--attributes] <ATTRIBUTES>`**

File attributes that are required\. Allowed values are archive, compressed, d\[irectory\], e\[mpty\], encrypted, f\[ile\], h\[idden\], normal, offline, r\[ead\-only\], r\[eparse\]\-p\[oint\], s\[ystem\] and temporary\.

**`[-b|--attributes-to-skip] <ATTRIBUTES>`**

File attributes that should be skipped\. Allowed values are archive, compressed, e\[mpty\], encrypted, h\[idden\], normal, offline, r\[ead\-only\], r\[eparse\]\-p\[oint\], s\[ystem\] and temporary\.

**`[-c|--content] <REGEX>`**

Regular expression for files' content\. Syntax is \<PATTERN> \[\<PATTERN\_OPTIONS>\]\. Allowed values are compiled, c\[ulture\-\]i\[nvariant\], e\[cma\-\]s\[cript\], e\[nds\-\]w\[ith\], e\[quals\], n \[explicit\-capture\], f\[rom\-file\], g\[roup\]=\<GROUP\_NAME>, i\[gnore\-case\], x \[ignore\-pattern\-whitespace\], li\[st\], length=\<NUM>, l\[ist\-\]s\[eparator\], l\[iteral\], m\[ultiline\], ne\[gative\], r\[ight\-to\-left\], s\[ingleline\], s\[tarts\-\]w\[ith\], timeout=\<NUM>, w\[hole\-\]l\[ine\] and w\[hole\-word\]\.

**`[--content-only]`**

Delete content of a file or directory but not the file or directory itself\.

**`[-y|--display] <DISPLAY_OPTIONS>`**

Display of the results\. Allowed values are c\[reation\-\]t\[ime\], indent=\<INDENT>, m\[odified\-\]t\[ime\], no\-align, p\[ath\]=\<PATH\_DISPLAY>, s\[ize\], s\[eparator\]=\<SEPARATOR> and su\[mmary\]\.

**`[-d|--dry-run]`**

Display which files or directories should be deleted but do not actually delete any file or directory\.

**`[--encoding] <ENCODING>`**

Encoding to use when a file does not contain byte order mark\. Default encoding is UTF\-8\.

**`[-e|--extension] <EXTENSION_FILTER>`**

A filter for file extensions \(case\-insensitive by default\)\. Syntax is EXT1\[,EXT2,\.\.\.\] \[\<EXTENSION\_OPTIONS>\]\. Allowed values are c\[ase\-\]s\[ensitive\], c\[ulture\-\]i\[nvariant\], f\[rom\-file\], l\[ist\-\]s\[eparator\], l\[iteral\], ne\[gative\] and timeout=\<NUM>\.

**`[-h|--help]`**

Show command line help\.

**`[-t|--highlight] <HIGHLIGHT>`**

Parts of the output to highlight\. Allowed values are n\[one\], m\[atch\] and e\[mpty\]\.

**`[-i|--include-directory] <REGEX>`**

Regular expression for a directory name\. Syntax is \<PATTERN> \[\<PATTERN\_OPTIONS>\]\. Allowed values are compiled, c\[ulture\-\]i\[nvariant\], e\[cma\-\]s\[cript\], e\[nds\-\]w\[ith\], e\[quals\], n \[explicit\-capture\], f\[rom\-file\], g\[roup\]=\<GROUP\_NAME>, i\[gnore\-case\], x \[ignore\-pattern\-whitespace\], li\[st\], length=\<NUM>, l\[ist\-\]s\[eparator\], l\[iteral\], m\[ultiline\], ne\[gative\], p\[art\]=\<NAME\_PART>, r\[ight\-to\-left\], s\[ingleline\], s\[tarts\-\]w\[ith\], timeout=\<NUM>, w\[hole\-\]l\[ine\] and w\[hole\-word\]\.

**`[--including-bom]`**

Delete byte order mark \(BOM\) when deleting file's content\.

**`[-m|--max-count] <NUM>`**

Stop deleting after specified number is reached\.

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

Delete empty directories in a current directory.

#### Syntax

```
orang delete ^
 --name "^(?=)" ^
 --attributes directory empty
```

#### Short Syntax

```
orang delete ^
 -n "^(?=)" ^
 -a d e
```

### Sample

Delete content of "bin" or "obj" directories in a current directory.

#### Syntax

```
orang delete ^
 --name "bin|obj" equals ^
 --attributes directory ^
 --content-only
```

#### Short Syntax

```
orang delete ^
 -n "bin|obj" e ^
 -a d ^
 --content-only
```

*\(Generated with [DotMarkdown](http://github.com/JosefPihrt/DotMarkdown)\)*