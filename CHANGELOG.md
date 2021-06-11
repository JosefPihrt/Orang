## 0.2.1 (2021-06-08)

* Add command `spellcheck` ([commit](https://github.com/JosefPihrt/Roslynator/commit/6d815be953b0d01b9f777d4a6323cb26cc944c33))
* Add option `--interactive` (applies to `rename` and `replace` commands) ([commit](https://github.com/JosefPihrt/Roslynator/commit/79f51787c6422087ecd408d97641c39b64770e47))
* Add value `culture-invariant` to option `--sort` ([commit](https://github.com/JosefPihrt/Roslynator/commit/3bf8db68ad480d96b43992f6d143c6668ea58638))

## 0.2.0 (2021-01-30)

## 0.2.0-rc (2021-01-04)

* Update target framework to .netcoreapp3.1 (LTS)
* Update Roslyn reference to 3.8.0
* Disallow empty regex pattern
* Copy/move directory including its content
* Add option `--split` (applies to `find` command)
* Add option `--pipe p[aths]` to use piped input as a list of paths  (applies to `find` and `replace` commands)
* Add option value `--modify group` to group paths by matched value  (applies to `find` command)
* Add option value `--display c[ontent]=o[mit]` to not display content of a file (applies to `find` and `replace` commands)
* Add option `-o|--online` to open help in a default browser

### Breaking Changes

* Redirected/piped input is used by default as a text not as a list of paths (applies to `find` and `replace` commands)
* Filter for `list-patterns` command uses regex search instead of simple text search and search is case-insensitive by default
* Filter for `help` command is case-insensitive by default
* Option `--ask` has no value (applies to `copy` and `move`)
* Change option value `new-line` to `newline` and remove shortcut `nl`
* Change shortcut `ao` to `o` (`aggregate-only`)
* Change shortcut `t` to `co` (`context`)
* Change shortcut `ta` to `a` (`context-after`)
* Change shortcut `tb` to `b` (`context-before`)

## 0.1.2 (2020-12-12)

* Enable 'except' for more than two files

## 0.1.1 (2020-10-26)

* Use C# code to compute replacements ([issue](https://github.com/JosefPihrt/Orang/issues/4))
  * `--replace <CSHARP_EXPRESSION> cs[harp]`
  * `--replace <CSHARP_CODE_FILE_PATH> cs[harp] f[rom-file]`
* Deprecate option `--evaluator` and replace it with `--replace "DllPath,FullTypeName.MethodName" from-dll`
* Add option `--display no-align` to disable alignment of columns
* Add option `--paths`
  * Use this option when you need to specify path(s) at a different than first position

## 0.1.0 (2020-09-01)

* Add shortcut 'b' for option '--attributes-to-skip'
* Add shortcut 'tb' for option 'context-before'
* Add shortcut 'ta' for option 'context-after'

## 0.1.0-rc2 (2020-06-09)

* Change target framework to 'netcoreapp3.0'
* Enable escape sequences in the input (applies to 'replace' command)
* Rename option `--conflict-resolution rename` to `--conflict-resolution suffix` (applies to 'copy', 'move' and 'rename' commands)
* Add option `--conflict-resolution` to 'rename' command
* Verify if new file name contains invalid characters (applies to 'rename' command)
* Add option `-f|--filter` (applies to 'help' command)
* Change option `-m|--max-count`
  * `--max-count <NUM>`: Stop searching after `<NUM>` matching files and/or directories.
  * `--max-count m[atches]=<NUM>`: Stop searching in each file after `<NUM>` matches.
* Add option to recognize escape sequences in the input string (applies to 'match' and 'split' commands)
* Add option to display lines before and after matching line.
  * Add option `--display context=<NUM>` or `-y t=<NUM>`
  * Add option `--display context-before=<NUM>`
  * Add option `--display context-after=<NUM>`
* Rename command 'list-syntax' to 'list-patterns'
* Replace option `-v|--values` with `-v|--verbosity d[etailed]` (applies to 'help' command)
* Add ability to separate matching lines with newline: `--display separator=\n`

## 0.1.0-rc (2020-02-09)

* Remove shortcut 't' from option 'trim-line'
* Add shortcut '-i' for option '--input' (applies to 'match' and 'split' command)
* Add ability to list syntax that matches specified character (applies to 'list-syntax' command)
* Add ability to list syntax that matches specified character (applies to 'list-syntax' command)
* Add 'copy' and 'move' commands
* Add option '--modify' (applies to 'find', 'rename' and 'replace' commands)
* Add pattern options 'starts-with' and 'ends-with'
* Rename pattern option 'whole-input' to 'equals'
* Make extension filter case-insensitive by default
* Display summary when explicitly required even if verbosity is set to 'quiet'
* Count processed files/directories in dry run but display it in gray colors
* Change display of renamed paths
* Require '--name' option only when no path is specified (i.e. current directory is used) (applies to 'delete' command)
* Compare paths based on current OS
* Calculate directory size
* Add option to display only match: `--display path=match`

## 0.1.0-beta (2019-12-28)

* Initial release
