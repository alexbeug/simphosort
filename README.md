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
Copy all photo files (currently *.jpg and *.jpeg) from a _source_ folder (including sub folders) to a _target_ folder with optional checks.
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
```

### group
Move photo files (currently *.jpg and *.jpeg) in a folder to sub folders. Grouping mode is controlled by sub commands.

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
```

### ungroup
Move files (currently *.jpg and *.jpeg) from sub folders back to the parent folder. Checks for duplicates before moving. Optionally delete empty sub folders after moving files back to parent folder.
This command is intended to undo a previous grouping step.

```
Usage: simphosort ungroup [options] <parent>

Arguments:

  parent  <PATH>
  Parent folder (containing the sub folders to ungroup)

Options:

  -d | --delete-empty
  Delete empty sub folders containing no files after ungroup
```

## License
simphosort is using the MIT license. See [LICENSE](LICENSE).
