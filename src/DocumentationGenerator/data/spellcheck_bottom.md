## Redirected/Piped Input

Redirected/piped input will be used either as a text to be searched (default) or as a list of paths separated with newlines (when `--pipe p[aths]` is specified.

## List of Allowed Words

* It is required to specify one or more wordlists (parameter `--words`).
* Wordlist is defined as a text file that contains list of values separated with newlines.
* Each value is either a valid word (for example `misspell`) or a fix in a format `<ERROR>: <FIX>` (for example `mispell: misspell`).
* Word matching is case-insensitive by default (use option `--case-sensitive` to specify case-sensitive matching).
* It is recommended to use [Wordb](https://github.com/JosefPihrt/Wordb/tree/main/data) wordlists that are specifically tailored to be used for spellchecking.
