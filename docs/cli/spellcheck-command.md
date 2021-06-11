# `orang spellcheck`

Searches the files' content for potential misspellings and typos\.

[Home](README.md#readme) &#x2022; [Synopsis](#Synopsis) &#x2022; [Arguments](#Arguments) &#x2022; [Options](#Options) &#x2022; [Samples](#Samples)

## Synopsis

```
orang spellcheck [<PATH>]
[   --ask]                <ASK_MODE>
[-a|--attributes]         <ATTRIBUTES>
[-b|--attributes-to-skip] <ATTRIBUTES>
[   --case-sensitive]
[-c|--content]            <PATTERN> [<PATTERN_OPTIONS>]
[-y|--display]            <DISPLAY_OPTIONS>
[-d|--dry-run]
[   --encoding]           <ENCODING>
[-e|--extension]          EXT1[,EXT2] [<EXTENSION_OPTIONS>]
[-h|--help]
[-t|--highlight]          <HIGHLIGHT>
[-i|--include-directory]  <PATTERN> [<PATTERN_OPTIONS>]
[   --input]              <INPUT> [<INPUT_OPTIONS>]
[   --interactive]
[-m|--max-count]          <MAX_OPTIONS>
[   --min-word-length]
[-n|--name]               <PATTERN> [<PATTERN_OPTIONS>]
[   --no-recurse]
[-o|--output]             <PATH> [<OUTPUT_OPTIONS>]
[   --paths]              <PATH>
[   --paths-from]         <FILE_PATH>
[   --pipe]               <PIPE_MODE>
[   --progress]
[-p|--properties]         <FILE_PROPERTIES>
[-s|--sort]               <SORT_OPTIONS>
[-v|--verbosity]          <VERBOSITY>
    --words               <PATH>
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

##### `[--case-sensitive]`

Specifies case\-sensitive matching\.

##### `[-c|--content] <PATTERN> [<PATTERN_OPTIONS>]`

Regular expression for files' content\.

[\<PATTERN_OPTIONS>](OptionValues.md#pattern_options): `compiled`, `c[ulture-]i[nvariant]`, `e[cma-]s[cript]`, `e[nds-]w[ith]`, `e[quals]`, `n [explicit-capture]`, `f[rom-file]`, `i[gnore-case]`, `x [ignore-pattern-whitespace]`, `li[st]`, `length=<NUM>`, `l[ist-]s[eparator]`, `l[iteral]`, `m[ultiline]`, `r[ight-to-left]`, `s[ingleline]`, `s[tarts-]w[ith]`, `timeout=<NUM>`, `w[hole-]l[ine]`, `w[hole-word]`\.

##### `[-y|--display] <DISPLAY_OPTIONS>`

Display of the results\.

[\<DISPLAY_OPTIONS>](OptionValues.md#display_options): `c[ontent]=`[\<CONTENT_DISPLAY>](OptionValues.md#content_display), `co[ntext]=<NUM>`, `b [context-before]=<NUM>`, `a [context-after]=<NUM>`, `c[ount]`, `c[reation-]t[ime]`, `indent=<INDENT>`, `l[ine-number]`, `m[odified-]t[ime]`, `no-align`, `p[ath]=`[\<PATH_DISPLAY>](OptionValues.md#path_display), `s[ize]`, `s[eparator]=<SEPARATOR>`, `su[mmary]`, `trim-line`\.

##### `[-d|--dry-run]`

Display which files should be updated but do not actually update any file\.

##### `[--encoding] <ENCODING>`

Encoding to use when a file does not contain BOM\. Default encoding is UTF\-8\.

##### `[-e|--extension] EXT1[,EXT2] [<EXTENSION_OPTIONS>]`

A filter for file extensions \(case\-insensitive by default\)\.

[\<EXTENSION_OPTIONS>](OptionValues.md#extension_options): `c[ase-]s[ensitive]`, `c[ulture-]i[nvariant]`, `f[rom-file]`, `l[ist-]s[eparator]`, `l[iteral]`, `ne[gative]`, `timeout=<NUM>`\.

##### `[-h|--help]`

Show command line help\.

##### `[-t|--highlight] <HIGHLIGHT>`

Parts of the output to highlight\.

[\<HIGHLIGHT>](OptionValues.md#highlight): `n[one]`, `m[atch]`, `r[eplacement]`, `e[mpty-]m[atch]`, `e[mpty-]r[eplacement]`, `e[mpty]`, `b[oundary]`, `t[ab]`, `c[arriage-]r[eturn]`, `l[ine]f[eed]`, `newline`, `space`\.

##### `[-i|--include-directory] <PATTERN> [<PATTERN_OPTIONS>]`

Regular expression for a directory name\.

##### `[--input] <INPUT> [<INPUT_OPTIONS>]`

The input string to be searched\.

##### `[--interactive]`

Enable editing of a replacement\.

##### `[-m|--max-count] <MAX_OPTIONS>`

Stop searching after specified number is reached\.

[\<MAX_OPTIONS>](OptionValues.md#max_options): `<NUM>`, `m[atches]=<NUM>`\.

##### `[--min-word-length]`

Specifies minimal word length to be checked\. Default value is 3\.

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

##### `[-v|--verbosity] <VERBOSITY>`

The amount of information to display in the log\.

[\<VERBOSITY>](OptionValues.md#verbosity): `q[uiet]`, `m[inimal]`, `n[ormal]`, `d[etailed]`, `di[agnostic]`\.

##### `--words <PATH>`

Specified path to file and/or directory that contains list of allowed words\.

## Redirected/Piped Input

Redirected/piped input will be used either as a text to be searched (default) or as a list of paths separated with newlines (when `--pipe p[aths]` is specified.

## List of Allowed Words

* It is required to specify one or more wordlists (parameter `--words`).
* Wordlist is defined as a text file that contains list of values separated with newlines.
* Each value is either a valid word (for example `misspell`) or a fix in a format `<ERROR>: <FIX>` (for example `mispell: misspell`).
* Word matching is case-insensitive by default (use option `--case-sensitive` to specify case-sensitive matching).
* It is recommended to use [Wordb](https://github.com/JosefPihrt/Wordb/tree/main/data) wordlists that are specifically tailored to be used for spellchecking.

*\(Generated with [DotMarkdown](http://github.com/JosefPihrt/DotMarkdown)\)*