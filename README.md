
# Twisted

This is an attempt to reverse-engineer the different file formats of the game Twisted Metal.

## DMD format

The DMD format is loosely based on the HMD format described in the PlayStation SDK documentation; by loosely, it is meant that it consists of a tree data structure where nodes are pointed to by pointers with values within some memory range of the PlayStation.

The format is pretty verbose, there can be tens of thousands of nodes and a node nesting depth greater than twenty in some cases.

The layout of a DMD file:

- header
  - magic number
  - version
  - build time
  - table of contents
- intertwined blocks of vertices and normals
- intertwined blocks of nodes and polygons
