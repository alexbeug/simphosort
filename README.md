# simphosort
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://github.com/alexbeug/simphosort/blob/main/LICENSE)
[![.NET](https://github.com/alexbeug/simphosort/actions/workflows/dotnet.yml/badge.svg)](https://github.com/alexbeug/simphosort/actions/workflows/dotnet.yml)

[comment]: # (https://img.shields.io/github/license/alexbeug/simphosort later when repo is public)

simphosort is a tool for copying & sorting photos, intended for periodic backup of folders with large numbers of unsorted images (such as smartphone/camera DCIM folders or messenger image folders).

Note: simphosort is a command line utility (for now).

## Workflow

simphosort provides multiple commands to work with photos. The main purpose is to **copy** and **group** new photos based on an existing photo collection. This collection is typically organized 
in folders and sub folders with a given structure, e.g. by year and month or year and topic. The **copy** command copies new photos from a _source_ folder to a _target_ folder, while 
checking for existing photos in one or more _check_ folders. Duplicate photos in _source_ and _check_ folders are not copied to the target folder. The new files in _target_ folder can 
then be moved their final location in the existing photo collection folder structure.

Short step by step description of a typical workflow:
1. Choose existing _source_ folder (e.g. DCIM) or copy pre-selected photos to an empty _source_ folder.
2. Clean _target_ folder when not empty.
3. Run simphosort copy command with optional _check_ folders. First check folder is typically the exisiting photo collection root folder.
4. Copy new files from _target_ folder to their final storage location.
5. Repeat next time...

## Commands

### copy
Copy all photo files (currently *.jpg and *.jpeg) from a _source_ folder (including sub folders) to a _target_ folder with optional checks.
```
Usage: simphosort.exe copy [options] <source> <target>

Arguments:

  source  <PATH>
  Source folder containing the photo files to copy

  target  <PATH>
  Target folder (has to be empty)

Options:

  -c | --check (Multiple)  <PATH>
  Check for duplicate photos at these folders. Duplicate files will not be copied to target.
```

## License
simphosort is using the MIT license. See [LICENSE](LICENSE).
