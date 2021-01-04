# `orang split`

Splits the input string into an list of substrings at the positions defined by a regular expression\.

[Home](README.md#readme) &#x2022; [Synopsis](#Synopsis) &#x2022; [Arguments](#Arguments) &#x2022; [Options](#Options) &#x2022; [Samples](#Samples)

## Synopsis

```
orang split [<PATH>]
 -c|--content    <PATTERN> [<PATTERN_OPTIONS>]
[-y|--display]   <DISPLAY_OPTIONS>
[-h|--help]
[-t|--highlight] <HIGHLIGHT>
[-i|--input]     <INPUT> [<INPUT_OPTIONS>]
[-m|--max-count] <NUM>
[   --modify]    <MODIFY_OPTIONS>
[   --no-groups]
[-o|--output]    <PATH> [<OUTPUT_OPTIONS>]
[-v|--verbosity] <VERBOSITY>
```

## Arguments

**`<PATH>`**

Path to a file that should be analyzed\.

## Options

##### `-c|--content <PATTERN> [<PATTERN_OPTIONS>]`

Regular expression for the input string\.

[\<PATTERN_OPTIONS>](OptionValues.md#pattern_options): `compiled`, `c[ulture-]i[nvariant]`, `e[cma-]s[cript]`, `e[nds-]w[ith]`, `e[quals]`, `n [explicit-capture]`, `f[rom-file]`, `i[gnore-case]`, `x [ignore-pattern-whitespace]`, `li[st]`, `length=<NUM>`, `l[ist-]s[eparator]`, `l[iteral]`, `m[ultiline]`, `r[ight-to-left]`, `s[ingleline]`, `s[tarts-]w[ith]`, `timeout=<NUM>`, `w[hole-]l[ine]`, `w[hole-word]`\.

##### `[-y|--display] <DISPLAY_OPTIONS>`

Display of the results\.

[\<DISPLAY_OPTIONS>](OptionValues.md#display_options): `c[ontent]=`[\<CONTENT_DISPLAY>](OptionValues.md#content_display), `indent=<INDENT>`, `s[eparator]=<SEPARATOR>`, `su[mmary]`\.

##### `[-h|--help]`

Show command line help\.

##### `[-t|--highlight] <HIGHLIGHT>`

Parts of the output to highlight\.

[\<HIGHLIGHT>](OptionValues.md#highlight): `n[one]`, `s[plit]`, `e[mpty-]s[plit]`, `e[mpty]`, `b[oundary]`, `t[ab]`, `c[arriage-]r[eturn]`, `l[ine]f[eed]`, `newline`, `space`\.

##### `[-i|--input] <INPUT> [<INPUT_OPTIONS>]`

The input string to be searched\.

##### `[-m|--max-count] <NUM>`

Maximum number of times the split can occur\.

##### `[--modify] <MODIFY_OPTIONS>`

Functions to modify results\.

[\<MODIFY_OPTIONS>](OptionValues.md#modify_options): `ag[gregate]`, `o [aggregate-only]`, `a[scending]`, `ci [culture-invariant]`, `d[escending]`, `di[stinct]`, `except`, `intersect`, `group`, `i[gnore-case]`, `re[move-empty]`, `rw [remove-white-space]`, `sort-by=`[\<SORT_PROPERTY>](OptionValues.md#sort_property), `tl [to-lower]`, `tu [to-upper]`, `t[rim]`, `te [trim-end]`, `ts [trim-start]`\.

##### `[--no-groups]`

Do not include groups in the results\.

##### `[-o|--output] <PATH> [<OUTPUT_OPTIONS>]`

Path to a file that should store output\.

##### `[-v|--verbosity] <VERBOSITY>`

The amount of information to display in the log\.

[\<VERBOSITY>](OptionValues.md#verbosity): `q[uiet]`, `m[inimal]`, `n[ormal]`, `d[etailed]`, `di[agnostic]`\.

## Redirected/Piped Input

Redirected/piped input will be used as a text to be searched.

*\(Generated with [DotMarkdown](http://github.com/JosefPihrt/DotMarkdown)\)*