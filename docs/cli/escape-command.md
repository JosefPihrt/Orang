# `orang escape`

Escapes special characters by replacing them with their escape codes\.

[Home](README.md#readme) &#x2022; [Synopsis](#Synopsis) &#x2022; [Arguments](#Arguments) &#x2022; [Options](#Options) &#x2022; [Samples](#Samples)

## Synopsis

```
orang escape
[   --char-group]
[-h|--help]
[-i|--input]       <INPUT> [<INPUT_OPTIONS>]
[-o|--output]      <PATH> [<OUTPUT_OPTIONS>]
[-r|--replacement]
[-v|--verbosity]   <VERBOSITY>
```

## Options

##### `[--char-group]`

Text is part of a character group\.

##### `[-h|--help]`

Show command line help\.

##### `[-i|--input] <INPUT> [<INPUT_OPTIONS>]`

Text to be escaped\.

##### `[-o|--output] <PATH> [<OUTPUT_OPTIONS>]`

Path to a file that should store output\.

##### `[-r|--replacement]`

Text is a replacement string\.

##### `[-v|--verbosity] <VERBOSITY>`

The amount of information to display in the log\.

[\<VERBOSITY>](OptionValues.md#verbosity): `q[uiet]`, `m[inimal]`, `n[ormal]`, `d[etailed]`, `di[agnostic]`\.

## Redirected/Piped Input

Redirected/piped input will be used as a text to be escaped.

## Samples

### Sample

Escape a given path so it will be treated as a literal string and not a regular expression.

#### Syntax

```
orang escape --input "foo\bar.txt"
```

#### Short Syntax

```
orang escape -i "foo\bar.txt"
```

*\(Generated with [DotMarkdown](http://github.com/JosefPihrt/DotMarkdown)\)*