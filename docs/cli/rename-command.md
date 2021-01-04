# `orang rename`

Renames files and directories\.

[Home](README.md#readme) &#x2022; [Synopsis](#Synopsis) &#x2022; [Arguments](#Arguments) &#x2022; [Options](#Options) &#x2022; [Samples](#Samples)

## Synopsis

```
orang rename [<PATH>]
[   --ask]
[-a|--attributes]         <ATTRIBUTES>
[-b|--attributes-to-skip] <ATTRIBUTES>
[   --conflict]           <CONFLICT_RESOLUTION>
[-c|--content]            <PATTERN> [<PATTERN_OPTIONS>]
[-y|--display]            <DISPLAY_OPTIONS>
[-d|--dry-run]
[   --encoding]           <ENCODING>
[-e|--extension]          EXT1[,EXT2] [<EXTENSION_OPTIONS>]
[-h|--help]
[-t|--highlight]          <HIGHLIGHT>
[-i|--include-directory]  <PATTERN> [<PATTERN_OPTIONS>]
[-m|--max-count]          <NUM>
[   --modify]             <REPLACE_MODIFY>
 -n|--name                <PATTERN> [<PATTERN_OPTIONS>]
[   --no-recurse]
[-o|--output]             <PATH> [<OUTPUT_OPTIONS>]
[   --paths]              <PATH>
[   --paths-from]         <FILE_PATH>
[   --progress]
[-p|--properties]         <FILE_PROPERTIES>
[-r|--replacement]        <REPLACEMENT> [<REPLACEMENT_OPTIONS>]
[-s|--sort]               <SORT_OPTIONS>
[-v|--verbosity]          <VERBOSITY>
```

## Arguments

**`<PATH>`**

Path to one or more files and/or directories that should be searched\.

## Options

##### `[--ask]`

Ask for a permission to rename file or directory\.

##### `[-a|--attributes] <ATTRIBUTES>`

File attributes that are required\.

[\<ATTRIBUTES>](OptionValues.md#attributes): `archive`, `compressed`, `d[irectory]`, `e[mpty]`, `encrypted`, `f[ile]`, `h[idden]`, `normal`, `offline`, `r[ead-only]`, `r[eparse]-p[oint]`, `s[ystem]`, `temporary`\.

##### `[-b|--attributes-to-skip] <ATTRIBUTES>`

File attributes that should be skipped\.

[\<ATTRIBUTES>](OptionValues.md#attributes): `archive`, `compressed`, `e[mpty]`, `encrypted`, `h[idden]`, `normal`, `offline`, `r[ead-only]`, `r[eparse]-p[oint]`, `s[ystem]`, `temporary`\.

##### `[--conflict] <CONFLICT_RESOLUTION>`

Defines how to resolve conflict when a file/directory already exists\.

[\<CONFLICT_RESOLUTION>](OptionValues.md#conflict_resolution): `a[sk]`, `o[verwrite]`, `suffix`, `s[kip]`\.

##### `[-c|--content] <PATTERN> [<PATTERN_OPTIONS>]`

Regular expression for files' content\.

[\<PATTERN_OPTIONS>](OptionValues.md#pattern_options): `compiled`, `c[ulture-]i[nvariant]`, `e[cma-]s[cript]`, `e[nds-]w[ith]`, `e[quals]`, `n [explicit-capture]`, `f[rom-file]`, `g[roup]=<GROUP_NAME>`, `i[gnore-case]`, `x [ignore-pattern-whitespace]`, `li[st]`, `length=<NUM>`, `l[ist-]s[eparator]`, `l[iteral]`, `m[ultiline]`, `ne[gative]`, `r[ight-to-left]`, `s[ingleline]`, `s[tarts-]w[ith]`, `timeout=<NUM>`, `w[hole-]l[ine]`, `w[hole-word]`\.

##### `[-y|--display] <DISPLAY_OPTIONS>`

Display of the results\.

[\<DISPLAY_OPTIONS>](OptionValues.md#display_options): `c[reation-]t[ime]`, `indent=<INDENT>`, `m[odified-]t[ime]`, `no-align`, `p[ath]=`[\<PATH_DISPLAY>](OptionValues.md#path_display), `s[ize]`, `s[eparator]=<SEPARATOR>`, `su[mmary]`\.

##### `[-d|--dry-run]`

Display which files/directories should be renamed but do not actually rename any file/directory\.

##### `[--encoding] <ENCODING>`

Encoding to use when a file does not contain BOM\. Default encoding is UTF\-8\.

##### `[-e|--extension] EXT1[,EXT2] [<EXTENSION_OPTIONS>]`

A filter for file extensions \(case\-insensitive by default\)\.

[\<EXTENSION_OPTIONS>](OptionValues.md#extension_options): `c[ase-]s[ensitive]`, `c[ulture-]i[nvariant]`, `f[rom-file]`, `l[ist-]s[eparator]`, `l[iteral]`, `ne[gative]`, `timeout=<NUM>`\.

##### `[-h|--help]`

Show command line help\.

##### `[-t|--highlight] <HIGHLIGHT>`

Parts of the output to highlight\.

[\<HIGHLIGHT>](OptionValues.md#highlight): `n[one]`, `m[atch]`, `r[eplacement]`, `e[mpty]`\.

##### `[-i|--include-directory] <PATTERN> [<PATTERN_OPTIONS>]`

Regular expression for a directory name\.

##### `[-m|--max-count] <NUM>`

Stop renaming after specified number is reached\.

##### `[--modify] <REPLACE_MODIFY>`

Functions to modify result\.

[\<REPLACE_MODIFY>](OptionValues.md#replace_modify): `ci [culture-invariant]`, `tl [to-lower]`, `tu [to-upper]`, `t[rim]`, `te [trim-end]`, `ts [trim-start]`\.

##### `-n|--name <PATTERN> [<PATTERN_OPTIONS>]`

Regular expression for file or directory name\.

[\<PATTERN_OPTIONS>](OptionValues.md#pattern_options): `compiled`, `c[ulture-]i[nvariant]`, `e[cma-]s[cript]`, `e[nds-]w[ith]`, `e[quals]`, `n [explicit-capture]`, `f[rom-file]`, `i[gnore-case]`, `x [ignore-pattern-whitespace]`, `li[st]`, `length=<NUM>`, `l[ist-]s[eparator]`, `l[iteral]`, `m[ultiline]`, `p[art]=`[\<NAME_PART>](OptionValues.md#name_part), `r[ight-to-left]`, `s[ingleline]`, `s[tarts-]w[ith]`, `timeout=<NUM>`, `w[hole-]l[ine]`, `w[hole-word]`\.

##### `[--no-recurse]`

Do not search subdirectories\.

##### `[-o|--output] <PATH> [<OUTPUT_OPTIONS>]`

Path to a file that should store output\.

##### `[--paths] <PATH>`

Path to one or more files and/or directories that should be searched\.

##### `[--paths-from] <FILE_PATH>`

Read the list of paths to search from a file\. Paths should be separated by newlines\.

##### `[--progress]`

Display dot \(\.\) for every hundredth searched file or directory\.

##### `[-p|--properties] <FILE_PROPERTIES>`

A filter for file properties\.

[\<FILE_PROPERTIES>](OptionValues.md#file_properties): `c[reation-]t[ime]`, `m[odified-]t[ime]`, `s[ize]=<NUM>`\.

##### `[-r|--replacement] <REPLACEMENT> [<REPLACEMENT_OPTIONS>]`

Replacement pattern\.

##### `[-s|--sort] <SORT_OPTIONS>`

Sort matched files and directories\.

[\<SORT_OPTIONS>](OptionValues.md#sort_options): `a[scending]`, `c[reation-]t[ime]`, `d[escending]`, `m[ax-count]=<NUM>`, `m[odified-]t[ime]`, `n[ame]`, `s[ize]`\.

##### `[-v|--verbosity] <VERBOSITY>`

The amount of information to display in the log\.

[\<VERBOSITY>](OptionValues.md#verbosity): `q[uiet]`, `m[inimal]`, `n[ormal]`, `d[etailed]`, `di[agnostic]`\.

## Redirected/Piped Input

Redirected/piped input will be used as a list of paths separated with newlines.

## Samples

### Sample

Remove suffix "-foo" from all files in a current directory.

#### Syntax

```
orang rename ^
 --name "\-foo$" part=name-without-extension ^
 --replacement ""
```

#### Short Syntax

```
orang rename ^
 -n "\-foo$" p=w ^
 -r ""
```

Note: Syntax `--replacement ""` or `-r ""` can be omitted.

### Sample

Normalize file extensions to lowercase.

#### Syntax

```
orang rename ^
 --name ".*" equals part=extension ^
 --modify to-lower
```

#### Short Syntax

```
orang rename -n ".*" e p=e ^
 --modify tl
```

*\(Generated with [DotMarkdown](http://github.com/JosefPihrt/DotMarkdown)\)*