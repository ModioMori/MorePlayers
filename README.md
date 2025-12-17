# MorePlayers

![Build](https://github.com/ModioMori/MorePlayers/actions/workflows/build.yml/badge.svg)

A mod for the Steam game [Gladio Mori](https://store.steampowered.com/app/2689120/Gladio_Mori/), adding a player management menu and increasing the player cap to 16. Powered by [HarmonyXPack](https://github.com/ModioMori/HarmonyXPack).
Default binding to open the manager menu is F6. This can not be changed, but support for changing the keybind, along with saving other settings, is planned at a later date.

**NOTE: Only the host of the match requires this mod! This will do nothing for clients.**

## Installation

1. [Go to the mod's page on mod.io,](https://mod.io/g/gladio-mori/m/moreplayers) or view the mod in the in-game mod.io menu.
2. Hit the Subscribe button.
3. Have fun!

## Screenshots

![Gladio_Mori_tmRbuSMFnR](https://github.com/ModioMori/MorePlayers/assets/19525688/c39d861c-6c54-481d-bd0f-bbd61194675c)
![Gladio_Mori_2z5C49lTYL](https://github.com/ModioMori/MorePlayers/assets/19525688/97cda077-d8cd-4a3a-9708-31c75be1d916)

## Compiling

The csproj uses reference paths based on the default Steam library installation folder of Gladio Mori. If you have the game installed on another drive, you will have to change these paths manually.
Otherwise, open the solution with Visual Studio 2022 or above (may work on older versions) and the .NET Desktop Development component installed in Visual Studio Installer.
Build > Build Solution should work perfectly fine, even without BepInEx installed (uses NuGet).

To locally test the mod, you will need to manually compile [HarmonyXPack](https://github.com/ModioMori/HarmonyXPack) and load it in your local mod.

Alternatively, download and install [.NET 8](https://dotnet.microsoft.com/en-us/download) and use `dotnet build` in the project directory.
