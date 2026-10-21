# StardewValley.Stubs

Compile-time stubs for the Stardew Valley Android assemblies.

The loader must compile against the game's types, but the real game DLLs are not
part of this repository. This project provides the small API surface the loader
touches. It is referenced with `Private=false` and is never packaged; at runtime
the loader loads the real `StardewValley.dll` from the installed game.

`MonoGame.Framework.dll` is copied into the APK and used at runtime. The Android
Java.Interop tooling generates Java peer types for this assembly, so the game's
different MonoGame build cannot be substituted after loading.

The stub layout is adapted from
[IvanKr08/SMAPILoader-Googleless](https://github.com/IvanKr08/SMAPILoader-Googleless)
(GPLv3). `MonoGame.Framework.dll` is the MonoGame build matching Stardew Valley
1.6.15.3 (.NET 9), part of MonoGame (MS-PL), and is packaged with the loader.
