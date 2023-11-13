# LC-API
An API for use when modding Lethal Company to make your life easier

# Installation
First, install BepInEx 5 into your game
(https://github.com/BepInEx/BepInEx)

Then, drop the LC_API.dll file in Lethal Company\BepInEx\plugins

# For Developers
If you want to use the API in your plugin, add the LC_API.dll as a project reference!

# Features
AssetBundle loading - Put asset bundles in BepInEx > Bundles and load them using BundleAPI.BundleLoader.GetLoadedAsset

ServerAPI - Automatically alerts other users when you host that your server is modded, and lets mod authors make their mods put users in special matchmaking where they can only play with other modded users

NetworkingAPI - Easily send data across the network to sync data between clients effortlessly
