
# Twisted

This is an attempt to reverse-engineer the different file formats of the game Twisted Metal.

## Introduction

Though incomplete, significant progress has been made so that levels can be previewed from within Unity.

There are two major formats used by the game, the DMD format and the TMS format:

The DMD format is loosely based on the HMD format described in the PlayStation SDK; it is a database in the form of a tree data structure that can contain tens of tousands of nodes. There are different families of nodes, each with a specific purpose, e.g. transformations, sprites, 3D models and possibly more.

The TMS format is a container with all the textures used by a level, the abbrevation meaning TIM(s), the official texture file format that you can read about in the documentation of the PlayStation SDK.

## Requirements

As the project uses some of the latest UI elements, you will need at least [Unity 2022.1.0b16](https://unity3d.com/beta/2022.1b#downloads).

## Usage

If you don't possess the original game, sample levels from the demo version found on the *PlayStation Magazine 4 CD* are available here for your convenience.

Now to examine DMD files, your one stop shop will be the *DMD Viewer* that you will find on the *Twisted* menu.

The tool itself is very easy to use, here's the description of the available actions in the toolbar:

- opening an existing DMD file
- filter out dupe nodes in search mode as there can be many
- automatically frame the selected object in the scene view
- whether or not to consolidate the object as a single mesh
- toggle the texturing for the selected object
- toggle the vertex colors for the selected object
- toggle the tinting of the different families of polygons
- filter the tree of nodes using a regular expression

The nodes shown can be sorted using multiple columns:
- hold <kbd>Ctrl</kbd> and click column headers to sort by multiple columns
- hold <kbd>Shift</kbd> and click any column header to remove the sorting

Also, you can right-click a node to dump some information for further study.

## General notes

There are still many things that have to be figured out:
- the hierarchical coordinate system to render complete levels
- the proper rendering of sprite nodes
- the purpose of the other node families
- etc...

TODO IDA, Ghidra