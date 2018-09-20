// Copyright 2013, Catlike Coding, http://catlikecoding.com

This is Catlike Coding's NumberFlow library.

Read the "Getting started with NumberFlow" PDF to learn how to use NumberFlow with your projects.

Documentation and online demos can be found at http://catlikecoding.com/numberflow/

You can create a new diagram via Asset / Create / NumberFlow Diagram.
To edit the diagram, select it and open the edit window via the inspector or via Window / NumberFlow.
You can add nodes to the diagram via the right-click context menu.

There are five examples included in the library.
"Demo", "Game of Life", and "Multiscene" run on desktops, web, and mobiles.
"Gallery" and "Terrain" are not meant for mobiles, because of complex shaders and lack of touch input support.

The NumberFlow product also includes Catlike Coding's Noise and Utilities libraries.

VERSION HISTORY
1.2.2
Fix: Y and Z domain offsets for tiling Manhattan and Chebyshev noise didn't work.
New: Added 2D version for all noise types, both API and nodes.

1.2.1
New: FPSCounter replaces the old FrameRateCounter script in the Utilities library.
Change: The Demo, Game of Life, and Terrain example scenes use the new Unity UI instead of OnGUI.

1.2
IMPORTANT: If you are upgrading from a previous version, you have to adjust diagrams that use Perlin or Turbulence noise.

Fix: Eliminated errors when adding input nodes in Unity 5.
Fix: Tiled noise now works with negative offsets that aren't whole numbers.
Removed: The Math library has been removed. It has been replaced with the Noise library.
Change: Noise methods have been moved to dedicated classes per noise type, all in the CatlikeCoding.Noise namespace.
Change: All noise methods now have a frequency parameter.
Change: Multi-frequency noise sampling has been normalized, so the output range doesn't change when adding frequencies.
Change: Switched to lookup tables for noise gradients for Perlin noise, instead of computing them. The patterns are now slightly different.
Change: Single-frequency Perlin noise has been normalized so it can produce values from -1 to 1 instead of roughly from -0.7 to 0.7.
Change: Multi-frequency Perlin noise now produces values from 0 to 1, to match all other noise types.
Change: Adjusted all diagrams so they work with the new noise values.
Change: Restructured the diagram editor context menu for adding noise nodes.
Change: Updated the noise example scene to use the new Unity UI.

1.1.2
Upgrade: Now works with Unity 5.0.1. Support for Unity 4 is dropped.
Fix: Deleting an output node sometimes got a diagram stuck in an invalid state.

1.1.1
Fix: Editor layout of node properties is now correct in Unity 4.5.

1.1
Constant numeric values are now shown in diagrams.
Added support for multiple texture outputs per diagram.
Added support for normal map creation as a post process step.
Added versions of Diagram.Fill and Diagram.FillRows method that fill multiple pixel buffers at once
or allow you to choose an output.
Obsoleted Diagram.Fill(Texture2D), Diagram.FillRow, and Diagram.FillInSteps methods.
Added Diagram.PostProcess.
You can now use a diagram material manager to link diagrams to materials in your scenes.
The textures will be generated both in play mode and in edit mode.
Added DiagramMaterialManager and ScenePersistentObject components.
Added Gallery example scene.
Added Value noise nodes and methods.
Added Colors / Lerp node.
Added Floats / * + Multiply Add node.
Added Floats / With 1 nodes.
Added Floats / Relative nodes.
Added nodes to Vector3s / Axes that combine X and Y.
Optimized some code.
Fixed a bug in GetXYClamped.
Updated the example scenes.
Rewrote the Getting started with NumberFlow document.

1.0.2
Replaced Perlin.cs with Noise.cs and prevented clashes with other plugins.
Added Voronoi noise functions and nodes.
Restructured the add-node context menu.
Replaced Ripples diagram with Voronoi Stones diagram.
DemoController.cs now uses a thread.

1.0.1
Reduced minimum required Unity version from 4.3 to 4.2.2.

1.0.0
Initial version.
