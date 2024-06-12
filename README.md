# SMAPI Dedicated Server Mod for Stardew Valley

<p align="center">
  <img src="https://img.shields.io/badge/Version-1.6-blue" />
</p>

This mod provides a dedicated (headless) server for Stardew Valley, powered by SMAPI. It turns the host farmer into an automated bot to facilitate multiplayer gameplay.

## Configuration File

Upon running SMAPI with the mod installed for the first time, a `config.json` file will be generated in the mod's folder. This file specifies which farm will be loaded on startup, farm creation options, host automation details, and other mod configuration options. Default values will be provided, which can then be modified. Here is an overview of the available settings:

### Startup options

- `FarmName`: The name of the farm. If a farm with this name exists, it will automatically be loaded and hosted for co-op. Otherwise, a new farm will be created using the specified farm creation options and then hosted for co-op.

### Farm Creation Options

- `StartingCabins`: The number of starting cabins for the farm. Must be an integer in {0, 1, 2, 3}.
- `CabinLayout`: Specifies the starting cabin layout. Options are "nearby" or "separate".
- `ProfitMargin`: The farm's profit margin. Options are "normal", "75%", "50%", and "25%".
- `MoneyStyle`: Determines whether money is shared or separate among farmers. Options are "shared" or "separate".
- `FarmType`: The type of farm. Options include "standard", "riverland", "forest", "hilltop", "wilderness", "fourcorners", and "beach".
- `CommunityCenterBundles`: The community center bundle type. Options are "normal" or "remixed".
- `GuaranteeYear1Completable`: Set to `true` or `false` to determine if the community center should be guaranteed completable during the first year.
- `MineRewards`: The mine rewards type. Options are "normal" or "remixed".
- `SpawnMonstersOnFarmAtNight`: Set to `true` or `false` to determine if monsters should spawn on the farm at night.
- `RandomSeed`: An optional integer specifying the farm's random seed.

### Host Automation Options

- `AcceptPet`: Set to `true` or `false` to determine if the farm pet should be accepted.
- `PetSpecies`: The desired pet species. Options are "dog" or "cat". Irrelevant if `AcceptPet` is `false`.
- `PetBreed`: An integer in {0, 1, 2} specifying the pet breed index. 0 selects the leftmost breed; 1 selects the middle breed; 2 selects the rightmost breed. Irrelevant if `AcceptPet` is `false`.
- `PetName`: The desired pet name. Irrelevant if `AcceptPet` is `false`.
- `MushroomsOrBats`: Choose between the mushroom or bat cave. Options are "mushrooms" or "bats" (case insensitive).
- `PurchaseJojaMembership`: Set to `true` or `false` to determine if the automated host should "purchase" (acquire for free) a Joja membership when available, committing to the Joja route. Defaults to `false`.

### Additional Options

- `EnableCropSaver`: Set to `true` or `false` to enable or disable the crop saver feature. When enabled, seasonal crops planted by players and fully grown before the season's end are guaranteed to give at least one more harvest before dying. For example, a spring crop planted by a player and fully grown before Summer 1 will not die immediately on Summer 1. Instead, it'll provide exactly one more harvest, even if it's a crop that ordinarily produces multiple harvests. Defaults to `true`.
- `MoveBuildPermission`: Change farmhands permissions to move buildings from the Carpenter's shop. Is set each time the server is started and can be changed in the game. Set to `off` to entirely disable moving buildings, set to `owned` to allow farmhands to move buildings that they purchased, or set to `on` to allow moving all buildings.
- `Password`: Password used to log in. It must be changed to a secure password. An empty string `""` means no password. Any check fails if the value is set to `null`.
- `PasswordProtected`: The password protection of individual functions can be switched on (`true`) and off (`false`) in the group.

## In Game Command

All commands in the game must be sent privately to the player `ServerBot`. For example, you must write the following `/message ServerBot MoveBuildPermission on`. Depending on the setting of the [`Password`](#additional-options) option, the commands can only be executed after you have logged in.

- `Build`: Builds a new cabin for more players at the place where the player is looking at. Allowed parameters are `Stone_Cabin`, `Plank_Cabin`, and `Log_Cabin`.
- `Demolish`: Destroys any building the player is looking at, not only cabins.
- `Pause`: (Toggle command) \
  Pause the game
- `TakeOver`: The host player returns control to the host, all host functions are switched on. Cancels the [`LetMePlay`](#host-in-game-command) command
- `SafeInviteCode`: A file `invite_code.txt` with the invitation code is created in this mods folder. If there is no invitation code, an empty string is saved.
- `InviteCode`: The invitation code is printed.
- `ForceInviteCode`: Forces the invitation code by closing and reopening the multiplayer server. There is an 8 second warning before the server stops, after another 2 seconds it is restarted.
- `Invisible`: (Toggle command) \
  Toggle the visibility of the farm host.
- `Sleep`: (Toggle command) \
  When it is sent, the host goes to bed. When all players leave the game or go to bed, the next day begins. On a second send, the host will get up and the mod's normal behavior will be restored.
- `ForceSleep`: Kick out all players and starts a new day.
- `ForceResetDay`: Kick out all players and restarts the day.
- `ForceShutDown`: Kick out all players, start a new day and shut down the server.
- `WalletSeparate`: Separate the wallets tonight.
- `WalletMerge`: Connect the wallets tonight.
- `SpawnMonster`: (Toggle command, Saved in config) \
  Changes the settings so that monsters spawn on the farm or not. Spawned monsters are not reset.
- `MoveBuildPermission` or
- `MovePermission` or
- `MBP`: (Saved in config) \
  Changes farmhands permissions to move buildings from the Carpenter's shop. Set to `off` to entirely disable moving buildings, set to `owned` to allow farmhands to move buildings that they purchased, or set to `on` to allow moving all buildings.

Administration:

- `LogIn`: This command is used to log in. The command is followed by the password that was defined in `config.json` under `Password`.
- `LogOut`: This command is used to log out.
- `PasswordProtectShow`: An overview of the commands is displayed. This shows which command is protected by a password and which can be executed by any player without restriction.
- `PasswordProtectAdd`: (Saved in config) \
  This command is followed by the name of a command which is now password-protected, so it can only be executed after logging in.
- `PasswordProtectRemove`: (Saved in config) \
  This removes a command from password protection. Every player can now use the command without restrictions.

## Host in Game Command

All these commands only work if you are the host. This allows you to take control of the server. The host sends the commands by entering them directly, without anything before or after:

- `LetMePlay`: Lets the player take over the host. All host functions are switched off. The `TakeOver` command must be entered to hand over the controller. \
  Please note that the host automation accepts gifts from events and NPCs and deletes items from the inventory if necessary.

## Use an 1.5 Game with 1.6 Server

I don't know if it is possible or what will happen if you use a 1.5 savegame with this 1.6 server.
If you want to do it, you need to do the following steps:

- Back up your game data first
- If you have an old version from before 1.6, you must delete the file
  `AdditionalCropData` in the game directory. GropSaver then only works
  for all plants planted from this point onwards.
- If you use an old `config.json` file, the server will not start.
  Delete the file and the file will be created with the default configuration.
  Stop the server, change it according to your wishes and start the server.

## Running the Server on Linux Without GUI

This mod can be run without the use of a GUI. To start the game, you must enter the following command:

```bash
xvfb-run -a "$HOME/GOG Games/Stardew Valley/game/StardewModdingAPI"
```

You can shut down the server from the started terminal session by pressing `Control-C`.
From another terminal session, you can send `Control-C` with `kill -SIGINT ....`.

```bash
ps -aux | grep StardewModdingAPI
kill -SIGINT ....
```

## Development

If Stardew Valley was not installed in the default path, the installation path must be added to the project file `DedicatedServer.csproj`. Add the path with the tag `GamePath` to the `PropertyGroup`. Depending on the path, it should look something like this:

```text
  <PropertyGroup>
    <TargetFramework>net6.0</TargetFramework>
    <GamePath>D:\SteamLibrary\steamapps\common\Stardew Valley</GamePath>
  </PropertyGroup>
```
