# LC-API

[![Build](https://github.com/steven4547466/LC-API/actions/workflows/build.yml/badge.svg)](https://github.com/steven4547466/LC-API/actions/workflows/build.yml)
[![Latest Version](https://img.shields.io/thunderstore/v/2018/LC_API?logo=thunderstore&logoColor=white)](https://thunderstore.io/c/lethal-company/p/2018/LC_API)
[![Total Downloads](https://img.shields.io/thunderstore/dt/2018/LC_API?logo=thunderstore&logoColor=white)](https://thunderstore.io/c/lethal-company/p/2018/LC_API)

The definitive Lethal Company modding API. Includes some very useful features to make modding life easier.

# For Developers
If you want to use the API in your plugin, add the LC_API.dll as a project reference!

## Contributing
If you wish to contribute to this project, you will need [unity netcode weaver](https://github.com/EvaisaDev/UnityNetcodeWeaver/releases) 
to implement custom networking properly. Follow their instructions to get NetcodeWeaver set-up for patching Lethal Company mods 
and keep note of the filepath where you chose to install it.

Once you have forked and cloned the repository, you will need to create a file in the solution folder called `LC-API.csproj.user` 
to set paths to build dependencies. Here's a template for that file's contents:
```xml
<?xml version="1.0" encoding="utf-8"?>
<Project ToolsVersion="Current" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
    <PropertyGroup>
        <LETHAL_COMPANY_DIR>F:/SteamLibrary/steamapps/common/Lethal Company</LETHAL_COMPANY_DIR>
        <TEST_PROFILE_DIR>$(APPDATA)/r2modmanPlus-local/LethalCompany/profiles/Test LC API</TEST_PROFILE_DIR>
        <NETCODE_PATCHER_DIR>$(SolutionDir)NetcodeWeaver</NETCODE_PATCHER_DIR>
    </PropertyGroup>

    <!-- Create your 'Test Profile' using your modman of choice before enabling this. 
    Enable by setting the Condition attribute to "true". *nix users should switch out `copy` for `cp`. -->
    <Target Name="CopyToTestProfile" AfterTargets="PostBuildEvent;NetcodeWeave" Condition="false">
        <MakeDir
                Directories="$(TEST_PROFILE_DIR)/BepInEx/plugins/2018-LC_API"
                Condition="Exists('$(TEST_PROFILE_DIR)') And !Exists('$(TEST_PROFILE_DIR)/BepInEx/plugins/2018-LC_API')"
        />
        <Exec Command="copy &quot;$(TargetPath)&quot; &quot;$(TEST_PROFILE_DIR)/BepInEx/plugins/2018-LC_API/&quot;" />
    </Target>
</Project>
```

It is vital that you change the `NETCODE_PATCHER_DIR` property to the location of your local NetcodeWeaver installation.

Ensure your Assembly CSharp is set `Publicize="true"` in the .csproj file to ensure it gets publicized.

Once you have completed these steps, you will be able to properly build the solution.

## Making a PR
Your [pull request](https://github.com/steven4547466/LC-API/pulls) should target the [dev branch](https://github.com/steven4547466/LC-API/tree/dev). This is because the `main` branch is reserved for tested features that are ready for release. Basically if someone were to clone the repo, they should be able to build `main` and use it without any fear of broken things.

The `dev` branch, however, may contain untested features and is used to build release candidates. Before releases, the `dev` branch will be frozen and tested for issues, when it passes its testing then it will be merged into `main` and a release will be made. Pre-releases may come from the `dev` branch for release candidates and testing. These will be generally stable, but may still contain broken features before testing is done.

Pull requests targeting the `main` branch will not be merged in most circumstances. Most merges to `main` will be directly from the frozen `dev` branch after testing.

# Features
AssetBundle loading - Put asset bundles in BepInEx > Bundles and load them using BundleAPI.BundleLoader.GetLoadedAsset

ServerAPI - Utilities relating to the network and server. This includes:

ModdedServer - Automatically alerts other users when you host a server that your server is modded. 
It also lets mod authors make their mods put users in special matchmaking where they can only play with other modded users

Networking - Easily send data across the network to sync data between clients

# Installation

## R2ModMan or Thunderstore Manager (highly recommended)

### R2ModMan
1. Go to the [thunderstore page](https://thunderstore.io/c/lethal-company/p/2018/LC_API)
2. Click `Install with Mod Manager`

### Thunderstore Manager
(if the above doesn't work for you, open up the Thunderstore App to do the following)
1. Click `Get mods`/`Online` (whatever it happens to be called)
2. Search for LC API
3. Download it

## Manual
1. Go to the [thunderstore page](https://thunderstore.io/c/lethal-company/p/2018/LC_API)
2. Click `Manual Download`
3. Unzip files
4. Navigate to `2018-LC_API-VERSION/BepinEx/plugins` and copy the contents
5. Find your BepinEx installation's plugin folder, by default it would be in steamapps: `steamapps\common\Lethal Company\BepInEx\plugins`
6. Create a folder titled `2018-LC_API`
7. Paste the contents into that folder

**DO NOT PLACE THE BUNDLES FOLDER IN THE PREMADE `BepinEx/Bundles` FOLDER!** It must be in the `2018-LC_API` folder.

If you did all of this correctly, it should load properly.

The resulting file structure should look like this:
```
BepinEx
├───Bundles
├───cache
├───config
├───core
├───patchers
└───plugins
    └───2018-LC_API
        ├───Bundles
        │   └───networking
        └───LC_API.dll
```

# TODO
- Picking up and dropping item events
  - Including `Player.StartGrabbingItem`, `Player.GrabbingItem`, `Player.GrabbedItem`, `Player.DroppingItem`, and `Player.DroppedItem`
    - The present-tense versions are cancellable, `StartGrabbingItem`, `GrabbingItem` and `DroppingItem`.
    - `StartGrabbingItem` exists for items that take time to pickup.
- Using item events
  - For shovels, that's hitting
  - For boomboxes, that's activating music
  - etc. though this would all fall under one event probably just `Item.Using` or `Item.Activating`
- Player begin and end moving events
- Player crouch and jump events
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
- `Player.EnteringFacility` and `Player.LeavingFacility`
  - With a bool for using fire escape