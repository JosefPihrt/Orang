# `orang escape`

Escapes special characters by replacing them with their escape codes\.

## Synopsis

```
orang escape
[   --char-group]
[-h|--help]
[-i|--input]       <INPUT>
[-o|--output]      <OUTPUT_OPTIONS>
[-r|--replacement]
[-v|--verbosity]   <VERBOSITY>
```

## Options

**`[--char-group]`**

Text is part of a character group\.

**`[-h|--help]`**

Show command line help\.

**`[-i|--input] <INPUT>`**

Text to be escaped\.

**`[-o|--output] <OUTPUT_OPTIONS>`**

Path to a file that should store output\. Syntax is \<PATH> \[\<OUTPUT\_OPTIONS>\]\. Allowed values are e\[ncoding\]=\<ENCODING>, v\[erbosity\]=\<VERBOSITY> and a\[ppend\]\.

**`[-r|--replacement]`**

Text is a replacement string\.

**`[-v|--verbosity] <VERBOSITY>`**

The amount of information to display in the log\. Allowed values are q\[uiet\], m\[inimal\], n\[ormal\], d\[etailed\] and di\[agnostic\]\.

## Use Redirected Input

Piped output from a previous command will be treated as input.

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