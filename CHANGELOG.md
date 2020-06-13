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
