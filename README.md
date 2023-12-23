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

Once you have forked and cloned the repository, you will need to create a file in the solution folder called `LC-API.csproj.user` 
to set paths to build dependencies. Here's a template for that file's contents:
```xml
<?xml version="1.0" encoding="utf-8"?>
<Project ToolsVersion="Current" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
    <PropertyGroup>
        <LETHAL_COMPANY_DIR>F:/SteamLibrary/steamapps/common/Lethal Company</LETHAL_COMPANY_DIR>
        <TEST_PROFILE_DIR>$(APPDATA)/r2modmanPlus-local/LethalCompany/profiles/Test LC API</TEST_PROFILE_DIR>
        <NETCODE_PATCHER_DIR>./NetcodeWeaver</NETCODE_PATCHER_DIR>
    </PropertyGroup>

    <!-- Create your 'Test Profile' using your modman of choice before enabling this. 
    Enable by setting the Condition attribute to "true". *nix users should switch out `copy` for `cp`. -->
    <Target Name="CopyToTestProfile" AfterTargets="PostBuildEvent;NetcodeWeave" Condition="true">
        <MakeDir
                Directories="$(TEST_PROFILE_DIR)/BepInEx/plugins/2018-LC_API"
                Condition="Exists('$(TEST_PROFILE_DIR)') And !Exists('$(TEST_PROFILE_DIR)/BepInEx/plugins/2018-LC_API')"
        />
        <Exec Command="copy &quot;$(TargetPath)&quot; &quot;$(TEST_PROFILE_DIR)/BepInEx/plugins/2018-LC_API/&quot;" />
    </Target>
</Project>
```

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