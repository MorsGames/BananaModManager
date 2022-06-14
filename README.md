# BananaModManager v0.9.3

A mod manager for Super Monkey Ball games that are made in Unity.

GameBanana Page: [SMB:BBHD](https://gamebanana.com/tools/7464) | [SMB:BM](https://gamebanana.com/tools/7542)

**This project is currently is still in beta and is not finished! As of right now, it only supports mods that make use of code injection.**

If you need some mods to use this mod manager with, check my [BananaBlitzHDMods](https://github.com/MorsGames/BananaBlitzHDMods) and [BananaManiaMods](https://github.com/MorsGames/BananaManiaMods) repositories.

### Currently Supported Games
- [Super Monkey Ball: Banana Blitz HD](https://store.steampowered.com/app/1061730/Super_Monkey_Ball_Banana_Blitz_HD)
- [Super Monkey Ball: Banana Mania](https://store.steampowered.com/app/1316910/Super_Monkey_Ball_Banana_Mania/)

(Technically, this mod manager can support many other Unity games, but this will likely change as more Super Monkey Ball specific features get added.)

### Missing Things
- Support for file redirection.
- Automatic updates for the mod manager itself.
- Automatic updates for the mods.
- GameBanana 1-click install support.

I'm currently busy with other projects to maintain this project on a regular basis, so I cannot guarantee when these features will arrive (if ever).

If you have a question, or want to help please reach out to me on the [Banana Mania Modding Discord](https://discord.gg/vuZWDMzzye). Make sure to ping `@Mors#3278` so I see your message!

This project is licensed under GPLv3.

## Installation
Download the release from the [releases](https://github.com/MorsGames/BananaModManager/releases) section and extract the contents of the "Release" folder inside the zip to the same folder as the game's executable.

Don't put the mod loader to a different folder than the game's executable, otherwise it won't work.

To install the mods, create a folder called "mods" and extract the zip files there. Each mod should be in their own folder.

## Making Mods
Check the [BananaBlitzHDMods](https://github.com/MorsGames/BananaBlitzHDMods) and [BananaManiaMods](https://github.com/MorsGames/BananaManiaMods) repositories to see some example mods.

A full guide is coming soon. Maybe. 

Or maybe not.

## Credits
- [Mors](http://mors-games.com): Programming

### Open Source Projects Used:
- [UnityDoorstop](https://github.com/NeighTools/UnityDoorstop)
- [Il2CppDumper](https://github.com/Perfare/Il2CppDumper)
- [Il2CppAssemblyUnhollower](https://github.com/knah/Il2CppAssemblyUnhollower)
- [Detours](https://github.com/microsoft/Detours)
- [Mono](https://github.com/mono/mono)
- [Harmony](https://github.com/pardeike/Harmony) (Included, but is optional for the mods)