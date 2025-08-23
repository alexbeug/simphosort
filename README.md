# simphosort
[![License](https://img.shields.io/badge/license-MIT-brightgreen.svg)](https://github.com/alexbeug/simphosort/blob/main/LICENSE.txt)

[comment]: # (https://img.shields.io/github/license/alexbeug/simphosort later when repo is public)

simphosort is a tool for sorting photos, intended for periodic backup of folders with large numbers of unsorted images (such as smartphone/camera DCIM folders or messenger image folders).
It compares all photo files in the \[work folder\] against existing photos in the \[photo folder\] structure and moves any new files to the \[sort folder\].
Users can then manually copy these files to either the \[photo folder\] structure or a [junk folder], where they will be excluded from future sorts (optional).

## Usage
1. Copy photos to an empty working folder.
2. Run simphosort
	```
    simphosort [work folder] [photo folder] [sort folder] [junk folder]
	```
3. Copy new files from \[sort folder\] into either \[photo folder\] sub folders or \[junk [folder\].
4. Repeat next time...

## License
simphosort is using the MIT license. See [LICENSE](LICENSE).
