# LC-API
The definitive Lethal Company modding API. Includes some very useful features to make modding life easier.

# Installation
First, install BepInEx 5 into your game
(https://github.com/BepInEx/BepInEx)

Then, drop the LC_API.dll file in Lethal Company\BepInEx\plugins

# For Developers
If you want to use the API in your plugin, add the LC_API.dll as a project reference!

# Features
AssetBundle loading - Put asset bundles in BepInEx > Bundles and load them using BundleAPI.BundleLoader.GetLoadedAsset

ServerAPI - Utilities relating to the network and server. This includes:

ModdedServer - Automatically alerts other users when you host a server that your server is modded. 
It also lets mod authors make their mods put users in special matchmaking where they can only play with other modded users

Networking - Easily send data across the network to sync data between clients effortlessly
