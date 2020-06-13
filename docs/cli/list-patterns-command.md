# `orang list-patterns`

Lists regular expression patterns\.

## Synopsis

```
orang list-patterns [<CHAR>]
[   --char-group]
[-f|--filter]     <FILTER>
[-h|--help]
[-o|--options]    <REGEX_OPTIONS>
[-o|--output]     <OUTPUT_OPTIONS>
[-s|--section]    <SYNTAX_SECTIONS>
[-v|--verbosity]  <VERBOSITY>
```

## Arguments

**`<CHAR>`**

Character or a decimal number that represents the character\. For a number literal use escape like \\1\.

## Options

**`[--char-group]`**

Treat character as if it is in the character group\.

**`[-f|--filter] <FILTER>`**

Filter string that should be used to filter results\.

**`[-h|--help]`**

Show command line help\.

**`[-o|--options] <REGEX_OPTIONS>`**

Regex options that should be used\. Relevant values are \[e\]cma\-\[s\]cript or \[i\]gnore\-case\. Allowed values are c\[ompiled\], c\[ulture\]\-i\[nvariant\], e\[cma\]\-s\[cript\], n \[explicit\-capture\], i\[gnore\-case\], x \[ignore\-pattern\-whitespace\], m\[ultiline\], r\[ight\-to\-left\] and s\[ingleline\]\.

**`[-o|--output] <OUTPUT_OPTIONS>`**

Path to a file that should store output\. Syntax is \<PATH> \[\<OUTPUT\_OPTIONS>\]\. Allowed values are e\[ncoding\]=\<ENCODING>, v\[erbosity\]=\<VERBOSITY> and a\[ppend\]\.

**`[-s|--section] <SYNTAX_SECTIONS>`**

Syntax sections to filter\. Allowed values are a\[lternation\-\]c\[onstructs\], a\[nchors\], b\[ackreference\-constructs\], c\[haracter\-classes\], c\[haracter\-\]e\[scapes\], g\[eneral\-\]c\[ategories\], g\[rouping\-constructs\], m\[iscellaneous\], n\[amed\-blocks\], o\[ptions\], q\[uantifiers\] and s\[ubstitutions\]\.

**`[-v|--verbosity] <VERBOSITY>`**

The amount of information to display in the log\. Allowed values are q\[uiet\], m\[inimal\], n\[ormal\], d\[etailed\] and di\[agnostic\]\.

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