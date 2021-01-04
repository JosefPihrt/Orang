# `orang list-patterns`

Lists regular expression patterns\.

[Home](README.md#readme) &#x2022; [Synopsis](#Synopsis) &#x2022; [Arguments](#Arguments) &#x2022; [Options](#Options) &#x2022; [Samples](#Samples)

## Synopsis

```
orang list-patterns [<CHAR>]
[   --char-group]
[-f|--filter]     <PATTERN> [<PATTERN_OPTIONS>]
[-h|--help]
[-o|--options]    <REGEX_OPTIONS>
[-s|--section]    <SYNTAX_SECTIONS>
[-v|--verbosity]  <VERBOSITY>
```

## Arguments

**`<CHAR>`**

Character or a decimal number that represents the character\. For a number literal use escape like \\1\.

## Options

##### `[--char-group]`

Treat character as if it is in the character group\.

##### `[-f|--filter] <PATTERN> [<PATTERN_OPTIONS>]`

Regular expression to filter patterns \(case\-insensitive by default\)\.

##### `[-h|--help]`

Show command line help\.

##### `[-o|--options] <REGEX_OPTIONS>`

Regex options that should be used\. Relevant values are \[e\]cma\-\[s\]cript or \[i\]gnore\-case\.

[\<REGEX_OPTIONS>](OptionValues.md#regex_options): `c[ompiled]`, `c[ulture]-i[nvariant]`, `e[cma]-s[cript]`, `n [explicit-capture]`, `i[gnore-case]`, `x [ignore-pattern-whitespace]`, `m[ultiline]`, `r[ight-to-left]`, `s[ingleline]`\.

##### `[-s|--section] <SYNTAX_SECTIONS>`

Syntax sections to filter\.

[\<SYNTAX_SECTIONS>](OptionValues.md#syntax_sections): `a[lternation-]c[onstructs]`, `a[nchors]`, `b[ackreference-constructs]`, `c[haracter-classes]`, `c[haracter-]e[scapes]`, `g[eneral-]c[ategories]`, `g[rouping-constructs]`, `m[iscellaneous]`, `n[amed-blocks]`, `o[ptions]`, `q[uantifiers]`, `s[ubstitutions]`\.

##### `[-v|--verbosity] <VERBOSITY>`

The amount of information to display in the log\.

[\<VERBOSITY>](OptionValues.md#verbosity): `q[uiet]`, `m[inimal]`, `n[ormal]`, `d[etailed]`, `di[agnostic]`\.

### Sample

Display regular expression anchors.

#### Syntax

```
orang list-patterns --section anchors
```

#### Short Syntax

```
orang list-patterns -s a
```

### Sample

Display general categories that matches filter string "num".

#### Syntax

```
orang list-patterns --section general-categories --filter num

```

#### Short Syntax

```
orang list-patterns -s gc -f num
```

### Sample

Display named blocks that matches filter string "han".

#### Syntax

```
orang list-patterns --section named-blocks --filter han

```

#### Short Syntax

```
orang list-patterns -s nb -f han
```

*\(Generated with [DotMarkdown](http://github.com/JosefPihrt/DotMarkdown)\)*