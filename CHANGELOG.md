# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

N/A

## Version [3.4.5]
- Fixed an issue where bodies would not ragdoll after death when using vanilla support.
- Fixed an issue where hurting sometimes did not work while using vanilla support.

## Version [3.4.4]
- Fixed an issue where dropping/grabbing items would not work properly if vanilla support was enabled.

## Version [3.4.3]
- Fixed an issue where intro tips would play regardless of if you've seen them or not.

## Version [3.4.2]
- Fixed object placement not rotating with the player properly.

## Version [3.4.1]
- Fixed players not taking damage or dying.

## Version [3.4.0]
- Added `Player.Died` and `Player.Hurt` *past-tense* events.
- Added `Player.StartGrabbingItem` and `Player.GrabbingItem` and `Player.GrabbedItem` events.
- Added `Player.DroppingItem` event.
- Added new `Player.ShowTip(string header, string message, float duration, bool isWarning, bool useSave, string prefsKey)` and `Player.QueueTip(string header, string message, float duration, int priority, bool isWarning, bool useSave, string prefsKey)`
  - `Player.ShowTip` bypasses the new tip queue.
  - Base game tips (or mods that use the `HUDManager.DisplayTip` method) will be treated as max priority tips for compatibility.
  - Tips now have a configurable duration in which they will stay on the screen.
  - The host can show tips to anyone, however local clients can only show tips to themselves.
  - The `int priority` parameter of the `QueueTip` method allows you to set a priority, where higher means it will be shown sooner.
    - Tips will always go to the end of their equivalent priority "list" meaning tips added after another that have the same priority will be shown after the ones added previously.
  - When a tip bypasses the queue for any reason, the currently showing tip will continue afterwards with whatever time left it had remaining as long as there were >= 1.5 seconds remaining.
    - This is the only "breaking" change as it will also affect tips shown by other plugins using other methods, which usually won't requeue the current tip. This will be monitored in case it needs to be removed.
  - With this, `GameTips` class has been deprecated, but will still work as expected.
- Fixed `Player.Username` not updating on the radar or escape menu.  
- Added `Player.PlayerObjectId` which returns the player's specified player object index in places like `allPlayerScripts`.
- Fixed an issue where players would be added twice to the player's dictionary causing an error.
- Fixed an issue where giving an item to a player but not switching to it would cause it to not be held properly.

## Version [3.3.2]

- Fixed `Player.Postion` setter for v47.

## Version [3.3.1]
- Fixed `Player.Username` not updating on the radar or escape menu.
  - Because of this, `Player.PlayerObjectId` has been added which returns the player's specified player object index in places like `allPlayerScripts`.
- Fixed `Player.ActiveList` not resetting.
- Fixed `Player.Dictionary` not resetting.

## Version [3.3.0]

- Fixed an issue where the old networking (`ServerAPI.Networking`) would not work properly.
- Fixed issues with the build workflow.
- Fixed the file structure in the README for manual installation.
- Fixed an issue where network messages wouldn't properly be re-registered when rejoining.
- Added a "Vanilla Support" config option that will disable installing the network bundle so you can play on vanilla servers.
  - The `Player` and `Item` classes will be completely disabled and this will probably break mods! Please just consider everyone in your game having LC API.

## Version [3.2.4]

- Fixed an issue with the `Player.Joined` and `Player.Left` events where they wouldn't work on LAN due to not having a steam ID.

## Version [3.2.3]

- Fixed package references being added as transitive dependencies when using the NuGet package.

## Version [3.2.2]

- Remove directory validation when pushing to NuGet (dotnet validates for us)
- Pack readme, changelog and license into nuget package

## Version [3.2.1]

- Fix nupkg-build artifact pathing in Publish workflow
- [Lordfirespeed](https://github.com/Lordfirespeed) sucks. 

## Version [3.2.0]

- Added `Networking` namespace
  - Provides much better networking that the previous `ServerAPI.Networking` class, which still exists for backwards compatibility.
  - See the [wiki](https://github.com/steven4547466/LC-API/wiki/Networking) for usage instructions.
- Added CI/CD github actions.
- Changed a hard-coded file location to be dynamically based off of where the plugin file is to prevent an issue with manual installation.
- Significantly revamped project structure.

## Version [3.1.0]

- Added `Item` class for interacting with grabbable objects easily.
- The `Player` class now has multiple new properties for inventory management.
  - `Player.Inventory` will return a `PlayerInventory` for this.
- The `Player.Joined` event should now work properly on LAN.

## Version [3.0.2]

- Fixed the command handler "eating" messages if they started with the command prefix, but weren't registered as commands.

## Version [3.0.1]

- Fixed `Player.HostPlayer`.

## Version [3.0.0]

- Removed automated bundle loading.
  - Legacy loading will still automatically load bundles if wanted.
- Added event system.
  - More events to be added in future.
- Added `Player` class for interacting with players easily.
- `ModdedServer.GameVersion` will now contain the base game version even if LC API modified the version to set modded only.

## Version [2.2.0]

- Added a command handler.
- The bundle loader will only attempt to load actual bundles.
- Added a temporary fix for lethal expansion bundles. Will be looking into a long term solution in the next update.
- The local player now will NOT receive data that they broadcast. The bool value on the receive delegates is also gone. If you were using Networking, you will need to ajust your code.

## Version [2.1.2]

- Updated to game version 45.

## Version [2.1.1]

- Actually fixed the BundleLodaer loading assets twice.
- Added new CheatDatabase with the purpose of catching users trying to join non-modded servers with cheaty mods. The CheatDatabase also allows for the host to view all mods installed by people joining. (As long as they have LC_API installed).

## Version [2.1.0]

- Fixed the BundleLodaer loading assets twice.

## Version [2.0.0]

- Changes to the BundleLoader to stop conflicts with other plugins loading assets without the BundleLoader.
- Changes to Networking and GameState events, plugins using these will need to be rebuilt.
- Added GameTips to GameInterfaceAPI. GameTips uses a tip que system to ensure no popup tip messages overlap with eachother.
- Changed the CheatDatabase to now (in theory) work for all players, not just the host.
- Changes the CheatDatabase to use GameTips to display information. It will still output information to the logs.

## Version [1.4.2]
- Fix for the new config option causing the API to fail to initialize.


## Version [1.4.1]

- LC_API should now be able to load no matter if the Hide Manager GameObject option is on or off.
- A config option has been added that will disable the BundleLoader.


## Version [1.4.0]

- Changed how the BundleLoader in the BundleAPI loads assets to fix issues with certain languages. This will break some mods, but a config option is included to revert to the old system so you can still use older mods.
- If you are a plugin developer, use GetLoadedAsset to get an asset, instead of using the asset dictionary. This ensures that your plugin will still work even when changes like this are made.


## Version [1.3.0]

- Changed how the BundleLoader in the BundleAPI loads assets to fix issues caused by downloading mods from mod managers. The path BepInEx > Bundles is outdated and should not be used anymore.

## Version [1.2.1]

- Adjusted README formatting.


## Version [1.2.0]

- Added new GameInterfaceAPI. Documentation will be created soon.

## Version [1.1.1]

- General bug fixes for Networking

## Version [1.1.0]

- General bug fixes for Networking

## Version [1.0.0]

- Release
