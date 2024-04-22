# GladioMoriMorePlayers
A [MelonLoader](https://melonwiki.xyz/#/README) mod for the Steam game/demo [Gladio Mori](https://store.steampowered.com/app/2689120/Gladio_Mori/), adding a player management menu and increasing the player cap to 16.
Default binding to open the manager menu is F6. You can change this in `UserData/MelonPreferences.cfg` after the first run of the mod.

# Installation
1. [Download and install MelonLoader with the automated installer.](https://melonwiki.xyz/#/README?id=automated-installation) If you can't run it, scroll up on that page; you may need .NET Framework 4.8.
2. [Go to the Releases page and download the latest release.](https://github.com/checkraisefold/GladioMorePlayers/releases)
3. Open the game's install folder and drag the DLL into the `Mods` folder.
4. Launch the game and play!

# Compiling
The csproj uses reference paths based on the default Steam library installation folder of Gladio Mori. If you have the game installed on another drive, you will have to change these paths manually.
Otherwise, open the solution with Visual Studio 2022 or above (may work on older versions) and the .NET Desktop Development component installed in Visual Studio Installer.
Build > Build Solution should work perfectly fine.

Alternatively, download and install [.NET 8](https://dotnet.microsoft.com/en-us/download) and use `dotnet build` in the project directory.