## Use Redirected Input

Piped output from a previous command will be treated as a list of paths separated by newlines.

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
