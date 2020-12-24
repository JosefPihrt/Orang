## Redirected/Piped Input

Redirected/piped input will be used as a list of paths separated with newlines.

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
