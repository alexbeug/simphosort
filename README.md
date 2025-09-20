# simphosort
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://github.com/alexbeug/simphosort/blob/main/LICENSE)
[![.NET](https://github.com/alexbeug/simphosort/actions/workflows/dotnet.yml/badge.svg)](https://github.com/alexbeug/simphosort/actions/workflows/dotnet.yml)

[comment]: # (https://img.shields.io/github/license/alexbeug/simphosort later when repo is public)

simphosort is a tool for sorting photos, intended for periodic backup of folders with large numbers of unsorted images (such as smartphone/camera DCIM folders or messenger image folders).
It compares all photo files (currently *.jpg and *.jpeg) in the _work_ folder against existing photos in a _photo_ folder (including sub folders) and copies any new files to a _sort_ folder.
Users can then manually move these files to either the _photo_ folder sub folders or a _junk_ folder (optional), where they will be excluded from future sorts.

## Usage
Note: simphosort is a command line utility.

1. Copy photos to an empty _work_ folder.
2. Clean _sort_ folder when not empty.
3. Run simphosort
```
Usage: simphosort.exe sort [options] <work> <photo> <sort>

Arguments:

  work   <PATH>
  Put the unsorted photos here

  photo  <PATH>
  Where your existing photos are saved

  sort   <PATH>
  New photos are copied here

Options:

  -j | --junk  <PATH>
  Treat photos from here as existing
```

	
4. Copy new files from _sort_ folder into either _photo_ folder sub folders or _junk_ folder.
5. Repeat next time...

## License
simphosort is using the MIT license. See [LICENSE](LICENSE).
