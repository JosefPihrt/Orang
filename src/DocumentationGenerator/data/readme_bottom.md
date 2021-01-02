## Exit Code

Code | Comment
--- | ---
0 | Match found
1 | No match found
2 | Error occured or execution canceled

## Redirected/Piped Input

Redirected/piped input will be used either as a raw text or as a list of paths separated with newlines.

Command | Pipe Mode
[copy](copy-command.md) | paths
[delete](delete-command.md) | paths
[escape](escape-command.md) | text
[find](find-command.md) | text (default) or paths when `--pipe p[aths]` is specified
[match](match-command.md) | text
[move](move-command.md) | paths
[rename](rename-command.md) | paths
[replace](replace-command.md) | text (default) or paths when `--pipe p[aths]` is specified
[split](split-command.md) | text

## Links

* [List of Option Values](OptionValues.md)
* [How To](HowTo.md)

## External Links

* [.NET Core Global Tools Overview](https://docs.microsoft.com/dotnet/core/tools/global-tools)
* [Create a .NET Core Global Tool Using the .NET Core CLI](https://docs.microsoft.com/dotnet/core/tools/global-tools-how-to-create)
* [.NET Core 2.1 Global Tools](https://natemcmaster.com/blog/2018/05/12/dotnet-global-tools/)
* [Windows CMD Shell](https://ss64.com/nt/syntax.html)
* [Parsing C++ Command-Line Arguments](https://docs.microsoft.com/cpp/cpp/parsing-cpp-command-line-arguments?view=vs-2019)
