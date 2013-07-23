Matrix Visualization Tools
==========================

A very simple toolkit for watching square matrix changes prerecorded in another software.

How it works
------------

Some external software (a writer class for C++ is included, but the format is _very_ straightforward) writes a file in a particular format:

```
[matrix_size]
---FRAME::[frame_number]---
[x],[y]|[value]
```

Where:

1. *[matrix_size]* is your N, when you're recording matrix _NxN_,
2. *[frame_number]* denotes the frame (an arbitrary number of changes that are recorded and shown together),
3. *[x]* and *[y]* are the position in the matrix (zero-indexed),
4. *[value]* is the new value to be assigned to that field.

Depending on how your software operates on matrices, the file can only be used to record changing values, as such keeping it's size to a minimum. While operating on a matrix of 121x121 elements and recording >1500 frames, I've been consistently seeing files under a megabyte large.

What does it show
-----------------

MVT was created as an internal tool for my bachelor thesis. I needed to visualize how a matrix changes while operated on by a numerical solver, so my version only colors the nonzero fields while painting the zeroed ones black. I believe it could be easily changed to show other points of interest.

Forking and modification
------------------------

The software is free as in both beer and speech and you are free to do with it as you please.
