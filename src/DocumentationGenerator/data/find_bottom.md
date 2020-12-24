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
