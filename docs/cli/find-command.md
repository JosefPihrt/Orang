# `orang find`

Searches the file system for files and directories and optionally searches files' content\.

[Home](README.md#readme) &#x2022; [Synopsis](#Synopsis) &#x2022; [Arguments](#Arguments) &#x2022; [Options](#Options) &#x2022; [Samples](#Samples)

## Synopsis

```
orang find [<PATH>]
[   --ask]                <ASK_MODE>
[-a|--attributes]         <ATTRIBUTES>
[-b|--attributes-to-skip] <ATTRIBUTES>
[-c|--content]            <PATTERN> [<PATTERN_OPTIONS>]
[-y|--display]            <DISPLAY_OPTIONS>
[   --encoding]           <ENCODING>
[-e|--extension]          EXT1[,EXT2] [<EXTENSION_OPTIONS>]
[-h|--help]
[-t|--highlight]          <HIGHLIGHT>
[-i|--include-directory]  <PATTERN> [<PATTERN_OPTIONS>]
[-m|--max-count]          <MAX_OPTIONS>
[   --modify]             <MODIFY_OPTIONS>
[-n|--name]               <PATTERN> [<PATTERN_OPTIONS>]
[   --no-recurse]
[-o|--output]             <PATH> [<OUTPUT_OPTIONS>]
[   --paths]              <PATH>
[   --paths-from]         <FILE_PATH>
[   --pipe]               <PIPE_MODE>
[   --progress]
[-p|--properties]         <FILE_PROPERTIES>
[-s|--sort]               <SORT_OPTIONS>
[   --split]
[-v|--verbosity]          <VERBOSITY>
```

## Arguments

**`<PATH>`**

Path to one or more files and/or directories that should be searched\.

## Options

##### `[--ask] <ASK_MODE>`

Ask for permission after each file or value\.

[\<ASK_MODE>](OptionValues.md#ask_mode): `f[ile]`, `v[alue]`\.

##### `[-a|--attributes] <ATTRIBUTES>`

File attributes that are required\.

[\<ATTRIBUTES>](OptionValues.md#attributes): `archive`, `compressed`, `d[irectory]`, `e[mpty]`, `encrypted`, `f[ile]`, `h[idden]`, `normal`, `offline`, `r[ead-only]`, `r[eparse-]p[oint]`, `s[ystem]`, `temporary`\.

##### `[-b|--attributes-to-skip] <ATTRIBUTES>`

File attributes that should be skipped\.

[\<ATTRIBUTES>](OptionValues.md#attributes): `archive`, `compressed`, `e[mpty]`, `encrypted`, `h[idden]`, `normal`, `offline`, `r[ead-only]`, `r[eparse-]p[oint]`, `s[ystem]`, `temporary`\.

##### `[-c|--content] <PATTERN> [<PATTERN_OPTIONS>]`

Regular expression for files' content\.

[\<PATTERN_OPTIONS>](OptionValues.md#pattern_options): `compiled`, `c[ulture-]i[nvariant]`, `e[cma-]s[cript]`, `e[nds-]w[ith]`, `e[quals]`, `n [explicit-capture]`, `f[rom-file]`, `g[roup]=<GROUP_NAME>`, `i[gnore-case]`, `x [ignore-pattern-whitespace]`, `li[st]`, `length=<NUM>`, `l[ist-]s[eparator]`, `l[iteral]`, `m[ultiline]`, `ne[gative]`, `r[ight-to-left]`, `s[ingleline]`, `s[tarts-]w[ith]`, `timeout=<NUM>`, `w[hole-]l[ine]`, `w[hole-word]`\.

##### `[-y|--display] <DISPLAY_OPTIONS>`

Display of the results\.

[\<DISPLAY_OPTIONS>](OptionValues.md#display_options): `c[ontent]=`[\<CONTENT_DISPLAY>](OptionValues.md#content_display), `co[ntext]=<NUM>`, `b [context-before]=<NUM>`, `a [context-after]=<NUM>`, `c[ount]`, `c[reation-]t[ime]`, `indent=<INDENT>`, `l[ine-number]`, `m[odified-]t[ime]`, `no-align`, `p[ath]=`[\<PATH_DISPLAY>](OptionValues.md#path_display), `s[ize]`, `s[eparator]=<SEPARATOR>`, `su[mmary]`, `trim-line`\.

##### `[--encoding] <ENCODING>`

Encoding to use when a file does not contain BOM\. Default encoding is UTF\-8\.

##### `[-e|--extension] EXT1[,EXT2] [<EXTENSION_OPTIONS>]`

A filter for file extensions \(case\-insensitive by default\)\.

[\<EXTENSION_OPTIONS>](OptionValues.md#extension_options): `c[ase-]s[ensitive]`, `c[ulture-]i[nvariant]`, `f[rom-file]`, `l[ist-]s[eparator]`, `l[iteral]`, `ne[gative]`, `timeout=<NUM>`\.

##### `[-h|--help]`

Show command line help\.

##### `[-t|--highlight] <HIGHLIGHT>`

Parts of the output to highlight\.

[\<HIGHLIGHT>](OptionValues.md#highlight): `n[one]`, `m[atch]`, `e[mpty-]m[atch]`, `e[mpty]`, `b[oundary]`, `t[ab]`, `c[arriage-]r[eturn]`, `l[ine]f[eed]`, `newline`, `space`\.

##### `[-i|--include-directory] <PATTERN> [<PATTERN_OPTIONS>]`

Regular expression for a directory name\.

##### `[-m|--max-count] <MAX_OPTIONS>`

Stop searching after specified number is reached\.

[\<MAX_OPTIONS>](OptionValues.md#max_options): `<NUM>`, `m[atches]=<NUM>`\.

##### `[--modify] <MODIFY_OPTIONS>`

Functions to modify results\.

[\<MODIFY_OPTIONS>](OptionValues.md#modify_options): `ag[gregate]`, `o [aggregate-only]`, `a[scending]`, `ci [culture-invariant]`, `d[escending]`, `di[stinct]`, `except`, `intersect`, `group`, `i[gnore-case]`, `re[move-empty]`, `rw [remove-white-space]`, `sort-by=`[\<SORT_PROPERTY>](OptionValues.md#sort_property), `tl [to-lower]`, `tu [to-upper]`, `t[rim]`, `te [trim-end]`, `ts [trim-start]`\.

##### `[-n|--name] <PATTERN> [<PATTERN_OPTIONS>]`

Regular expression for file or directory name\.

##### `[--no-recurse]`

Do not search subdirectories\.

##### `[-o|--output] <PATH> [<OUTPUT_OPTIONS>]`

Path to a file that should store output\.

##### `[--paths] <PATH>`

Path to one or more files and/or directories that should be searched\.

##### `[--paths-from] <FILE_PATH>`

Read the list of paths to search from a file\. Paths should be separated by newlines\.

##### `[--pipe] <PIPE_MODE>`

Defines how to use redirected/piped input\.

[\<PIPE_MODE>](OptionValues.md#pipe_mode): `t[ext]`, `p[aths]`\.

##### `[--progress]`

Display dot \(\.\) for every hundredth searched file or directory\.

##### `[-p|--properties] <FILE_PROPERTIES>`

A filter for file properties\.

[\<FILE_PROPERTIES>](OptionValues.md#file_properties): `c[reation-]t[ime]`, `m[odified-]t[ime]`, `s[ize]=<NUM>`\.

##### `[-s|--sort] <SORT_OPTIONS>`

Sort matched files and directories\.

[\<SORT_OPTIONS>](OptionValues.md#sort_options): `a[scending]`, `c[reation-]t[ime]`, `ci [culture-invariant]`, `d[escending]`, `m[ax-count]=<NUM>`, `m[odified-]t[ime]`, `n[ame]`, `s[ize]`\.

##### `[--split]`

Execute regex in a split mode\.

##### `[-v|--verbosity] <VERBOSITY>`

The amount of information to display in the log\.

[\<VERBOSITY>](OptionValues.md#verbosity): `q[uiet]`, `m[inimal]`, `n[ormal]`, `d[etailed]`, `di[agnostic]`\.

## Redirected/Piped Input

Redirected/piped input will be used either as a text to be searched (default) or as a list of paths separated with newlines (when `--pipe p[aths]` is specified.

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