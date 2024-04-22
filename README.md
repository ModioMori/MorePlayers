# GladioMorePlayers
A [MelonLoader](https://melonwiki.xyz/#/README) mod for the Steam game/demo [Gladio Mori](https://store.steampowered.com/app/2689120/Gladio_Mori/), adding a player management menu and increasing the player cap to 16.
Default binding to open the manager menu is F6. You can change this in `UserData/MelonPreferences.cfg` after the first run of the mod.
**NOTE: Only the host of the match requires this mod! This will do nothing for clients.**

# Installation
1. [Download and install MelonLoader with the automated installer.](https://melonwiki.xyz/#/README?id=automated-installation) If you can't run it, scroll up on that page; you may need .NET Framework 4.8. You'll have to browse to the game's executable; on most systems, this will be at `C:\Program Files (x86)\Steam\steamapps\common\Gladio Mori Demo`. If you can't find it, use the lower instructions to open the game's install folder and find the path.
2. [Go to the Releases page and download the latest release.](https://github.com/checkraisefold/GladioMorePlayers/releases)
3. Open the game's install folder. ![steamwebhelper_MgEX2j797W](https://github.com/checkraisefold/GladioMorePlayers/assets/19525688/757debf4-1969-4d88-a4a9-bc62e1907f2e)
4. Drag the DLL you downloaded into the `Mods` folder.![firefox_zZ8CpQOniM](https://github.com/checkraisefold/GladioMorePlayers/assets/19525688/24957617-2844-44b0-8bae-937d5b0898d7)
5. Launch the game and play!

# Screenshots
![Gladio_Mori_0bEQd287Gm](https://github.com/checkraisefold/GladioMorePlayers/assets/19525688/97fa94f4-4418-4838-9a62-75f5ed63fd87)
![Gladio_Mori_2z5C49lTYL](https://github.com/checkraisefold/GladioMorePlayers/assets/19525688/97cda077-d8cd-4a3a-9708-31c75be1d916)

# Compiling
The csproj uses reference paths based on the default Steam library installation folder of Gladio Mori. If you have the game installed on another drive, you will have to change these paths manually.
Otherwise, open the solution with Visual Studio 2022 or above (may work on older versions) and the .NET Desktop Development component installed in Visual Studio Installer.
Build > Build Solution should work perfectly fine.

Alternatively, download and install [.NET 8](https://dotnet.microsoft.com/en-us/download) and use `dotnet build` in the project directory.
