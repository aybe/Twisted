
# Twisted

This is an attempt to reverse-engineer the different file formats of the game Twisted Metal.

## DMD format

The DMD format seems to be approximately based on the HMD format described in the PlayStation SDK; it consists of a tree data structure where there can be +10K nodes and deeply nested nodes with a depth of +20.

The tree nodes are pointed to by pointers ranging from `0x80010000` and above, just like the HMD format is, it is plausible that the data is *pre-built* to be uploaded to the memory of the PSX and to be consumed directly from there.

Once you realize that fact and that the first pointer in the file is the base pointer to be subtracted to any pointer being encountered, it immediately becomes much easier to read the tree and its nodes constituting a DMD file.
