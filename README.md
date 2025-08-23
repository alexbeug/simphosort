# simphosort
[![License](https://img.shields.io/badge/license-MIT-brightgreen.svg)](https://github.com/alexbeug/simphosort/blob/main/LICENSE)

[comment]: # (https://img.shields.io/github/license/alexbeug/simphosort later when repo is public)

simphosort is a tool for sorting photos, intended for periodic backup of folders with large numbers of unsorted images (such as smartphone/camera DCIM folders or messenger image folders).
It compares all photo files in the _work folder_ against existing photos in a _photo folder_ structure and moves any new files to a _sort folder_.
Users can then manually copy these files to either the _photo folder_ structure or a _junk folder_ (optional), where they will be excluded from future sorts.

## Usage
1. Copy photos to an empty _work folder_.
2. Run simphosort
	```
    simphosort [work folder] [photo folder] [sort folder] [junk folder]
	```
3. Copy new files from _sort folder_ into either _photo folder_ sub folders or _junk folder_.
4. Repeat next time...

## License
simphosort is using the MIT license. See [LICENSE](LICENSE).
