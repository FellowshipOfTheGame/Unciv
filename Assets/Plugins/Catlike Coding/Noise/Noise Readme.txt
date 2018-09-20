// Copyright 2013, Catlike Coding, http://catlikecoding.com

This is Catlike Coding's Noise library.

It contains a collection static classes that allow you to sample from a few pseudorandom noise types.

The example scene visualizes the different noise types with a colored height field.

To use the library, include "using CatlikeCoding.Noise;" at the top of your C# script.
You can then invoke the methods from the various Noise classes.
For example:
	
	float noiseSample = PerlinNoise.Sample3D(somePoint, 5f, 3, 2f, 0.5f);
