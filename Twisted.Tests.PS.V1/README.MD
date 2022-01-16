An attempt to read Twisted Metal files

1.
- reads 100% of DMD files for all TM1 PSX versions, as usual, JAP versions are slightly different :)
- this stuff should be ported to a hierarchical structure like `IDMDNode`, not done yet

2.
- the huge `switch` block on top is what comes after `00FF` block (the model data) (and possibly others)
- there are many because it's graphical data, but not as much as one would think, it took only a few minutes to write them to get 100% passed tests
- to figure out which these belong to, use the call stack then according DMD nodes should read them, after that this huge, messy block will not be needed anymore

3.
- once you have that low hanging fruit, there will be only 6 different type of structures in DMD files
- then comes the hard work to understand/identify position and stuff, etc

4.
- at some point a Unity project should be added to try do renderings

5.
- DPC should be somewhat readable but as I said, I favor PSX instead
- TMS files are easy

Unfortunately I've lost my previous Unity project where there was a tree view you could click on and get an instant preview of that model :(((


Put games in a `.twisted` folder at root that you can create by typing `.twisted.` in Windows Explorer.

```
.twisted\TM1PSJAP
.twisted\TM1PSUSA
.twisted\TM1PSEUR
```

Then you can run the tests.

The up to date code is in OpenTwistedTests.PSX namespace, forget about everything else as it's old, this is when I was trying before the serious work of really understanding it.

You can use Hexedit 5 that has coloring to look at DMDs, it's on Github.

(tip: use its calculator + macro against base DMD pointer to easily navigate in DMD)

Hopefully at some point, a modern engine ??? :DDD