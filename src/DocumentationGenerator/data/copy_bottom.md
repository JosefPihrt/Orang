## Redirected/Piped Input

Redirected/piped input will be used as a list of paths separated with newlines.

## Samples

### Sample

Backup Visual Studio project but exclude directories that are not necessary.

#### Syntax

```
orang copy MyProject ^
 --target "backup/MyProject" ^
 --conflict overwrite ^
 --compare attributes size content ^
 --include-directory "bin,obj,packages,node_modules,.vs" literal list equals negative
```

#### Short Syntax

```
orang copy MyProject ^
 --target "backup/MyProject" ^
 --conflict o ^
 --compare a s c ^
 -i "bin,obj,packages,node_modules,.vs" l li e ne
```
