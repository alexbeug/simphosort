# simphosort
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://github.com/alexbeug/simphosort/blob/main/LICENSE)
[![.NET](https://github.com/alexbeug/simphosort/actions/workflows/dotnet.yml/badge.svg)](https://github.com/alexbeug/simphosort/actions/workflows/dotnet.yml)

[comment]: # (https://img.shields.io/github/license/alexbeug/simphosort later when repo is public)

simphosort is a tool for copying & sorting photos, intended for periodic backup of folders with large numbers of unsorted images (such as smartphone/camera DCIM folders or messenger image folders).

Note: simphosort is a command line utility (for now).

## Workflow

simphosort supports sorting new photos by filtering for existing duplicates and pre-grouping them for you manually doing the final sort. The main purpose is to **copy** and **group**
new photos based on an existing photo collection. This collection is typically organized in folders and sub folders with a given structure, e.g. by year and month or year and topic. 
The **copy** command copies new photos from a _source_ folder to a _target_ folder, while checking for existing photos in one or more _check_ folders. Duplicate photos in _source_ 
and _check_ folders are not copied to the _target_ folder. The new files in _target_ folder can then be pre-grouped to sub folders and then moved their final location in the existing 
photo collection folder structure.

### Short step by step description of a typical workflow:
1. Choose existing _source_ folder (e.g. DCIM) or copy pre-selected photos to an empty _source_ folder.
2. Clean _target_ folder when not empty. Attention: The _target_ folder is a temporary work folder for new photos, not your photo collection.
3. Run simphosort copy command with optional _check_ folders. First check folder is typically the exisiting photo collection root folder.
4. Run simphosort group command in _target_ folder to pre-group photos into sub folders (e.g. by year and month).
5. Copy new files from _target_ folder to their final storage location.
6. Repeat next time...

## Commands

### copy
Copy all photo files (default *.jpg and *.jpeg) from a _source_ folder (including sub folders) to a _target_ folder with optional checks.
```
Usage: simphosort copy [options] <source> <target>

Arguments:

  source  <PATH>
  Source folder (containing the photo files to copy)

  target  <PATH>
  Target folder (work folder, has to be empty)

Options:

  -c | --check (Multiple)  <PATH>
  Check for duplicate photos at these folders. Duplicate files will not be copied to target.

  -s | --search (Multiple)  <TEXT>
  Specify custom search pattern using wildcards (e.g. *.jpg or IMG_*.nef)
```

#### Paths
All folder paths can be absolute or relative. Relative paths are resolved against the current working directory. Paths containing spaces should be enclosed in quotes. All specified
pathes must exist. The target folder must be empty. Paths have to be unique (no duplicates). Unique paths are determined by the full absolute path and are always checked case insensitive.

#### Checks
The copy command supports multiple check folders. Each photo file in the source folder is checked for duplicates in all specified check folders (including their sub folders). 
If a duplicate is found in any of the check folders, the file is not copied to the target. The comparison is done by file name and file size. File name comparison casing is platform 
default (usually case insensitive on Windows and case sensitive on Linux).

### group
Move photo files (default *.jpg and *.jpeg) in a folder to sub folders. Grouping mode is controlled by sub commands.

#### fixed
Group photos using a fixed date and a format string for sub folder names.

```
Usage: simphosort group fixed [options] <folder>

Arguments:

  folder  <PATH>
  Folder (containing the photo files to group)

Options:

  -f | --format  <TEXT>
  Format string (e.g. yyyy-MM-dd for daily sub folders)

  -s | --search (Multiple)  <TEXT>
  Specify custom search pattern using wildcards (e.g. *.jpg or IMG_*.nef)
```

### ungroup
Move files (default *.jpg and *.jpeg) from sub folders back to the parent folder. Checks for duplicates before moving. Optionally delete empty sub folders after moving files back to parent folder.
This command is intended to undo a previous grouping step.

```
Usage: simphosort ungroup [options] <parent>

Arguments:

  parent  <PATH>
  Parent folder (containing the sub folders to ungroup)

Options:

  -d | --delete-empty
  Delete empty sub folders containing no files after ungroup

  -s | --search (Multiple)  <TEXT>
  Specify custom search pattern using wildcards (e.g. *.jpg or IMG_*.nef)
```

#### Checks
The ungroup command checks for duplicate files in the parent folder before moving files from sub folders back to the parent folder. A file is considered a duplicate if a file with the same name 
already exists in the parent folder. File name comparison casing is platform default (usually case insensitive on Windows and case sensitive on Linux).

#### Delete empty
If the -d or --delete-empty option is specified, empty sub folders (containing no files) are deleted after moving files back to the parent folder. If a sub folder contains other sub folders, the 
check is done recursively. An empty sub folder is only deleted if it and all its sub folders contain no files.

## Search patterns
Without any search pattern specified, simphosort will search for files with the common JPEG file extensions .jpg and .jpeg. You can specify custom search patterns using wildcards with the -s or --search option. 
Multiple search patterns can be specified by repeating the option. Case sensitivity is platform dependent (usually case insensitive on Windows, case sensitive on Linux).

## License
simphosort is using the MIT license. See [LICENSE](LICENSE).
