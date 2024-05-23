# MorePlayers
![Build](https://github.com/ModioMori/MorePlayers/actions/workflows/build.yml/badge.svg)

A [BepInEx](https://github.com/BepInEx/BepInEx) mod for the Steam game/demo [Gladio Mori](https://store.steampowered.com/app/2689120/Gladio_Mori/), adding a player management menu and increasing the player cap to 16.
Default binding to open the manager menu is F6. You can change this in `BepInEx/config/gay.crf.gladiomoreplayers.cfg` after the first run of the mod.

**NOTE: Only the host of the match requires this mod! This will do nothing for clients.**

# Installation
1. [Download and install BepInEx to the game's directory.](https://docs.bepinex.dev/articles/user_guide/installation/index.html#installing-bepinex-1) You'll have to browse to the game's executable; on most systems, this will be at `C:\Program Files (x86)\Steam\steamapps\common\Gladio Mori Demo`. If you can't find it, use the lower instructions to open the game's install folder and find the path.
2. [Go to the Releases page and download the latest release.](https://github.com/ModioMori/MorePlayers/releases)
3. Open the game's install folder. ![steamwebhelper_cULdZeOTQa](https://github.com/ModioMori/MorePlayers/assets/19525688/b07f69d6-7727-48b2-9810-6335479f66fb)
4. Drag the DLL you downloaded into the `BepInEx/plugins` folder. ![explorer_NyczDKF4uW](https://github.com/ModioMori/MorePlayers/assets/19525688/8a2ce78a-0caf-4a80-8fef-578378595896)
5. Launch the game and play!

# Screenshots
![Gladio_Mori_tmRbuSMFnR](https://github.com/ModioMori/MorePlayers/assets/19525688/c39d861c-6c54-481d-bd0f-bbd61194675c)
![Gladio_Mori_2z5C49lTYL](https://github.com/ModioMori/MorePlayers/assets/19525688/97cda077-d8cd-4a3a-9708-31c75be1d916)

# Compiling
The csproj uses reference paths based on the default Steam library installation folder of Gladio Mori. If you have the game installed on another drive, you will have to change these paths manually.
Otherwise, open the solution with Visual Studio 2022 or above (may work on older versions) and the .NET Desktop Development component installed in Visual Studio Installer.
Build > Build Solution should work perfectly fine, even without BepInEx installed (uses NuGet).

Alternatively, download and install [.NET 8](https://dotnet.microsoft.com/en-us/download) and use `dotnet build` in the project directory.
