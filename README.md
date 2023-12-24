# LC-API
The definitive Lethal Company modding API. Includes some very useful features to make modding life easier.

# Installation
First, install BepInEx 5 into your game
(https://github.com/BepInEx/BepInEx)

Then, get the latest release off of thunderstore 
(https://thunderstore.io/c/lethal-company/p/2018/LC_API/)

# For Developers
If you want to use the API in your plugin, add the LC_API.dll as a project reference!

## Contributing
If you wish to contribute to this project, you will need [unity netcode weaver](https://github.com/EvaisaDev/UnityNetcodeWeaver/releases) to make custom networking properly. Unzip it anywhere on your computer and open the solution properties, go to build events and change the `cd D:\NetcodePatcher` to the location of the netcode patcher on your machine. The folder you have should have `NetcodePatcher.dll` inside of it.

Also ensure your Assembly CSharp is set `Publicize="true"` in the .csproj file to ensure it gets publicized.

Once you have these done, you will be able to properly build the solution.

# Features
AssetBundle loading - Put asset bundles in BepInEx > Bundles and load them using BundleAPI.BundleLoader.GetLoadedAsset

ServerAPI - Utilities relating to the network and server. This includes:

ModdedServer - Automatically alerts other users when you host a server that your server is modded. 
It also lets mod authors make their mods put users in special matchmaking where they can only play with other modded users

Networking - Easily send data across the network to sync data between clients

# TODO
- Picking up and dropping item events
- Using item events
  - For shovels, that's hitting
  - For boomboxes, that's activating music
  - etc. though this would all fall under one event probably just `Item.Using` or `Item.Activating`
- Player begin and end moving events
- Player crouch and jump events
- Past tense versions of hurting and dying
  - These won't be cancellable, but you will be able to use these to do things on death/hurt as hurting and dying can be cancelled by other mods
- Changing item event
- Selling items event
  - Probably one for selling many items on the counter which will also call an event for each individual item being sold
- Item spawning event
- Enemy stuff is a bit further out, but here's some events that will be useful for them (as well as their past tense versions):
  - Spawning
  - Hurting
  - Dying
  - Attacking (when the enemy attacks a player)
  - There may be ones specific to some kind of enemy as well