# simphosort
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://github.com/alexbeug/simphosort/blob/main/LICENSE)
[![.NET](https://github.com/alexbeug/simphosort/actions/workflows/dotnet.yml/badge.svg)](https://github.com/alexbeug/simphosort/actions/workflows/dotnet.yml)

[comment]: # (https://img.shields.io/github/license/alexbeug/simphosort later when repo is public)

simphosort is a tool for sorting photos, intended for periodic backup of folders with large numbers of unsorted images (such as smartphone/camera DCIM folders or messenger image folders).
It compares all photo files (currently *.jpg and *.jpeg) in the _work folder_ against existing photos in a _photo folder_ structure and copies any new files to a _sort folder_.
Users can then manually move these files to either the _photo folder_ structure or a _junk folder_ (optional), where they will be excluded from future sorts.

## Usage
Note: simphosort is a command line utility.

1. Copy photos to an empty _work folder_.
2. Clean _sort folder_ when not empty.
3. Run simphosort
	```
    simphosort [work folder] [photo folder] [sort folder] [[junk folder]]
	```
4. Move new files from _sort folder_ into either _photo folder_ sub folders or _junk folder_.
5. Repeat next time...

## License
simphosort is using the MIT license. See [LICENSE](LICENSE).
