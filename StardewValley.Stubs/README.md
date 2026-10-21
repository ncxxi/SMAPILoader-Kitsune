# StardewValley.Stubs

Compile-time stubs for the Stardew Valley Android assemblies.

The loader must compile against the game's types, but the real game DLLs are not
part of this repository. This project provides the small API surface the loader
touches. It is referenced with `Private=false` and is never packaged; at runtime
the loader loads the real `StardewValley.dll` and `MonoGame.Framework.dll` from
the installed game.

The stub layout is adapted from
[IvanKr08/SMAPILoader-Googleless](https://github.com/IvanKr08/SMAPILoader-Googleless)
(GPLv3). `MonoGame.Framework.dll` is part of MonoGame (MS-PL) and is used only
as a compile-time reference.
