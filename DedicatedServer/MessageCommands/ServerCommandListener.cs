﻿using DedicatedServer.Chat;
using DedicatedServer.Config;
using DedicatedServer.HostAutomatorStages;
using DedicatedServer.Utils;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DedicatedServer.MessageCommands
{
    internal class ServerCommandListener
    {
        private EventDrivenChatBox chatBox;

        private ModConfig config;

        private IModHelper helper;

        private const string hardwoodItemId = "709";
        private const string iridiumSprinkler = "645";
        private const string beanStarter = "473";
        private const string pepperSeed = "482";
        private const string eggplantSeeds = "488";

        public ServerCommandListener(IModHelper helper, ModConfig config, EventDrivenChatBox chatBox)
        {
            this.helper  = helper;
            this.config  = config;
            this.chatBox = chatBox;
        }

        public void Enable()
        {
            chatBox.ChatReceived += chatReceived;
        }

        public void Disable()
        {
            chatBox.ChatReceived -= chatReceived;
        }

        #region DEBUG_SKIP_DAYS
        #if true

        // Each day is run so that all events are executed normally.

        private int dayOfMonth = -1;
        private Season season;
        private void EnableSkipDays(int dayOfMonth, Season season)
        {
            this.dayOfMonth = dayOfMonth;
            this.season = season;
            helper.Events.GameLoop.OneSecondUpdateTicked += SkipDays;
            SkipDays(null, null);
        }

        private void DisableSkipDays()
        {
            this.dayOfMonth = -1;
            helper.Events.GameLoop.OneSecondUpdateTicked -= SkipDays;
            Sleeping.ShouldSleepOverwrite = false;
        }

        private void SkipDays(object sender, OneSecondUpdateTickedEventArgs e)
        {
            if (dayOfMonth > Game1.dayOfMonth || Game1.season != this.season)
            {
                if(false == Sleeping.ShouldSleepOverwrite)
                {
                    Sleeping.ShouldSleepOverwrite = true;
                }
            }
            else
            {
                DisableSkipDays();
            }
        }
        #endif
        #endregion

        private void chatReceived(object sender, ChatEventArgs e)
        {
            var tokens = e.Message.Split(' ');

            if (0 == tokens.Length) { return; }

            string command = tokens[0].ToLower();

            var sourceFarmer = Game1.otherFarmers.Values
                .Where(farmer => farmer.UniqueMultiplayerID == e.SourceFarmerId)
                .FirstOrDefault()
                ?? Game1.player;

            string param = 1 < tokens.Length ? tokens[1].ToLower() : "";

            if (Game1.player.UniqueMultiplayerID == e.SourceFarmerId)
            {
                switch (command)
                {
                    case "letmeplay":
                        LetMePlay(sourceFarmer);
                        break;

                    #region DEBUG_COMMANDS
                    #if true

                    case "skipdays":
                        EnableSkipDays(28, Season.Fall);
                        break;

                    case "t1":
                        var a = Utility.getAllPets();
                        break;

                    case "pp":
                        HostAutomation.PreventPauseUntilNextDay();
                        break;

                    case "item":
                        if ("" != param)
                        {
                            int param2 = int.TryParse(2 < tokens.Length ? tokens[2] : "1", out int param2try) ? param2try : 1;
                            if (0 <= param2)
                            {
                                Game1.player.addItemToInventory(new StardewValley.Object(param, param2));
                            }
                        }
                        break;

                    case "inventory":
                        foreach(var inventoryItems in Game1.player.Items)
                        {
                            var itemId = inventoryItems?.ItemId;
                        }
                        break;

                    case "items":
                        Game1.player.addItemToInventory(new StardewValley.Object(StardewValley.Object.iridiumID, 999));
                        Game1.player.addItemToInventory(new StardewValley.Object(StardewValley.Object.iridiumID, 999));
                        Game1.player.addItemToInventory(new StardewValley.Object(StardewValley.Object.iridiumID, 999));
                        Game1.player.addItemToInventory(new StardewValley.Object(StardewValley.Object.woodID, 999));
                        Game1.player.addItemToInventory(new StardewValley.Object(StardewValley.Object.stoneID, 999));
                        break;

                    case "iridiumsprinkler":
                        Game1.player.addItemToInventory(new StardewValley.Object(iridiumSprinkler, 10));
                        break;

                    case "iridium":
                        Game1.player.addItemToInventory(new StardewValley.Object(StardewValley.Object.iridiumID, 999));
                        break;

                    case "wood":
                        Game1.player.addItemToInventory(new StardewValley.Object(StardewValley.Object.woodID, 999));
                        break;

                    case "stone":
                        Game1.player.addItemToInventory(new StardewValley.Object(StardewValley.Object.stoneID, 999));
                        break;

                    case "hardwood":
                        Game1.player.addItemToInventory(new StardewValley.Object(hardwoodItemId, 999));
                        break;

                    case "emptyinventoryall": // /message serverbot EmptyInventoryAll
                        ServerHost.EmptyHostInventory();
                        break;

                    case "menu":
                        var menu = Game1.activeClickableMenu;
                        chatBox.textBoxEnter($" Menu is {(menu?.ToString() ?? "")}" + TextColor.Green);
                        break;
                                            
                    case "multiplayer":
                        MultiplayerOptions.EnableServer = true;
                        break;
                        
                    case "gold":
                        Game1.player.team.SetIndividualMoney(Game1.player, 1000);
                        break;

                    case "singleplayer":
                        MultiplayerOptions.EnableServer = false;
                        break;

                    case "farm":
                        Game1.player.warpFarmer(WarpPoints.FarmWarp);
                        break;

                    case "house":
                        Game1.player.warpFarmer(WarpPoints.FarmHouseWarp);
                        break;

                    case "mine":
                        Game1.player.warpFarmer(WarpPoints.mineWarp);
                        break;

                    case "town":
                        Game1.player.warpFarmer(WarpPoints.townWarp);
                        break;

                    case "beach":
                        Game1.player.warpFarmer(WarpPoints.beachWarp);
                        break;

                    case "robin":
                        Game1.player.warpFarmer(WarpPoints.robinWarp);
                        break;

                    case "clint":
                        Game1.player.warpFarmer(WarpPoints.clintWarp);
                        break;

                    case "pierre":
                        Game1.player.warpFarmer(WarpPoints.pierreWarp);
                        break;

                    case "location":
                        var location = Game1.player.Tile;
                        chatBox.textBoxEnter("location: " + Game1.player.currentLocation.ToString());
                        chatBox.textBoxEnter("x: " + location.X + ", y:" + location.Y);
                        break;

                    #endif
                    #endregion
                }
            }
            else
            {
                if (ChatBox.privateMessage != e.ChatKind)
                {
                    return;
                }
            }

            switch (command)
            {
                case "takeover": // /message ServerBot TakeOver
                    TakeOver(sourceFarmer);
                    break;

                case "updatehouselevel":  // /message ServerBot UpdateHouseLevel
                    UpdateHouseLevel(sourceFarmer, param);
                    break;

                case "safeinvitecode": // /message ServerBot SafeInviteCode
                    SafeInviteCode(sourceFarmer);
                    break;

                case "invitecode": // /message ServerBot InviteCode
                    InviteCode(sourceFarmer);
                    break;

                case "forceinvitecode": // /message ServerBot ForceInviteCode
                    ForceInviteCode(sourceFarmer);
                    break;

                case "invisible": // /message ServerBot Invisible
                    InvisibleSub(sourceFarmer);
                    break;

                case "sleep": // /message ServerBot Sleep
                    Sleep(sourceFarmer);
                    break;

                case "forcesleep": // /message ServerBot ForceSleep
                    ForceSleep(sourceFarmer);
                    break;

                case "forceresetday": // /message ServerBot ForceResetDay
                    ForceResetDay(sourceFarmer);
                    break;

                case "forceshutdown": // /message ServerBot ForceShutdown
                    ForceShutdown(sourceFarmer);
                    break;

                case "walletseparate": // /message ServerBot WalletSeparate
                    WalletSeparate(sourceFarmer);
                    break;

                case "walletmerge": // /message ServerBot WalletMerge
                    WalletMerge(sourceFarmer);
                    break;

                case "spawnmonster": // /message ServerBot SpawnMonster
                    SpawnMonster(sourceFarmer);
                    break;

                case "mbp": // /message ServerBot mbp on
                case "movebuildpermission":
                case "movepermission":
                    MoveBuildPermissionSub(sourceFarmer, param);
                    break;

                default:
                    break;
            }
        }

        private void LetMePlay(Farmer farmer)
        {
            if (false == PasswordValidation.IsAuthorized(farmer.UniqueMultiplayerID, p => p.LetMePlay))
            {
                WriteToPlayer(farmer, PasswordValidation.notAuthorizedMessage);
                return;
            }

            WriteToPlayer(null, $"The host is now a player, all host functions are deactivated." + TextColor.Green);
            HostAutomation.LetMePlay();
        }

        private void TakeOver(Farmer farmer)
        {
            if (false == PasswordValidation.IsAuthorized(farmer.UniqueMultiplayerID, p => p.TakeOver))
            {
                WriteToPlayer(farmer, PasswordValidation.notAuthorizedMessage);
                return;
            }

            WriteToPlayer(null, $"Control has been transferred to the host, all host functions are switched on." + TextColor.Aqua);
            HostAutomation.Reset();
        }

        private void UpdateHouseLevel(Farmer farmer, string param)
        {
            if (false == PasswordValidation.IsAuthorized(farmer.UniqueMultiplayerID, p => p.UpgradeHouseLevelBasedOnFarmhand))
            {
                WriteToPlayer(farmer, PasswordValidation.notAuthorizedMessage);
                return;
            }

            if ("" == param)
            {
                if (HostHouseUpgrade.NeedsUpgrade())
                {
                    WriteToPlayer(null, "A host farm house upgrade is being executed" + TextColor.Yellow);
                }
                else
                {
                    WriteToPlayer(null, "A host farm house upgrade is not necessary" + TextColor.Green);
                }
            }
            else
            {
                WriteToPlayer(null, $"The host farm house is upgraded to {param}" + TextColor.Orange);
                HostHouseUpgrade.ManualUpdate(param);
            }
        }

        private void SafeInviteCode(Farmer farmer)
        {
            if (false == PasswordValidation.IsAuthorized(farmer.UniqueMultiplayerID, p => p.SafeInviteCode))
            {
                WriteToPlayer(farmer, PasswordValidation.notAuthorizedMessage);
                return;
            }

            MultiplayerOptions.SaveInviteCode();
            if (MultiplayerOptions.IsInviteCodeAvailable)
            {
                WriteToPlayer(farmer, $"Your invite code is saved in the mod folder in the file {MultiplayerOptions.inviteCodeSaveFile}." + TextColor.Green);
            }
            else
            {
                WriteToPlayer(farmer, $"The game has no invite code." + TextColor.Red);
            }
        }
        
        private void InviteCode(Farmer farmer)
        {
            if (false == PasswordValidation.IsAuthorized(farmer.UniqueMultiplayerID, p => p.InviteCode))
            {
                WriteToPlayer(farmer, PasswordValidation.notAuthorizedMessage);
                return;
            }
            
            WriteToPlayer(farmer, 
                String.Format(Game1.content.LoadString("Strings\\UI:Server_InviteCode"), MultiplayerOptions.InviteCode) + 
                ("" == MultiplayerOptions.InviteCode ? TextColor.Red : TextColor.Green));
        }

        private void ForceInviteCode(Farmer farmer)
        {
            if (false == PasswordValidation.IsAuthorized(farmer.UniqueMultiplayerID, p => p.ForceInviteCode))
            {
                WriteToPlayer(farmer, PasswordValidation.notAuthorizedMessage);
                return;
            }

            MultiplayerOptions.TryActivatingInviteCode();
        }

        private void InvisibleSub(Farmer farmer)
        {
            if (false == PasswordValidation.IsAuthorized(farmer.UniqueMultiplayerID, p => p.Invisible))
            {
                WriteToPlayer(farmer, PasswordValidation.notAuthorizedMessage);
                return;
            }

            Invisible.InvisibleOverwrite = !Invisible.InvisibleOverwrite;
            WriteToPlayer(farmer, $"The host is invisible {Invisible.InvisibleOverwrite}" + TextColor.Aqua);
        }

        private void Sleep(Farmer farmer)
        {
            if (false == PasswordValidation.IsAuthorized(farmer.UniqueMultiplayerID, p => p.Sleep))
            {
                WriteToPlayer(farmer, PasswordValidation.notAuthorizedMessage);
                return;
            }

            if (false == HostAutomation.EnableHostAutomation)
            {
                WriteToPlayer(farmer, $"Cannot start sleep because the host is controlled by the player." + TextColor.Red);
                return;
            }

            if (Sleeping.ShouldSleepOverwrite)
            {
                Sleeping.ShouldSleepOverwrite = false;
                WriteToPlayer(null, $"The host is back on his feet." + TextColor.Aqua);
            }
            else
            {
                WriteToPlayer(null, $"The host will go to bed." + TextColor.Green);
                Sleeping.ShouldSleepOverwrite = true;
            }
        }

        private void ForceSleep(Farmer farmer)
        {
            if (false == PasswordValidation.IsAuthorized(farmer.UniqueMultiplayerID, p => p.ForceSleep))
            {
                WriteToPlayer(farmer, PasswordValidation.notAuthorizedMessage);
                return;
            }
            
            RestartDay.ForceSleep((seconds) => chatBox.textBoxEnter($"Attention: Server will start the next day in {seconds} seconds" + TextColor.Orange));
        }

        private void ForceResetDay(Farmer farmer)
        {
            if (false == PasswordValidation.IsAuthorized(farmer.UniqueMultiplayerID, p => p.ForceResetDay))
            {
                WriteToPlayer(farmer, PasswordValidation.notAuthorizedMessage);
                return;
            }

            RestartDay.ResetDay((seconds) => WriteToPlayer(null, $"Attention: Server will reset the day in {seconds} seconds" + TextColor.Orange));
        }

        private void ForceShutdown(Farmer farmer)
        {
            if (false == PasswordValidation.IsAuthorized(farmer.UniqueMultiplayerID, p => p.ForceShutdown))
            {
                WriteToPlayer(farmer, PasswordValidation.notAuthorizedMessage);
                return;
            }

            RestartDay.ShutDown((seconds) => WriteToPlayer(null, $"Attention: Server will shut down in {seconds} seconds" + TextColor.Orange));
        }

        private void WalletSeparate(Farmer farmer)
        {
            if (false == PasswordValidation.IsAuthorized(farmer.UniqueMultiplayerID, p => p.Wallet))
            {
                WriteToPlayer(farmer, PasswordValidation.notAuthorizedMessage);
                return;
            }

            Wallet.Separate(farmer);
        }
        
        private void WalletMerge(Farmer farmer)
        {
            if (false == PasswordValidation.IsAuthorized(farmer.UniqueMultiplayerID, p => p.Wallet))
            {
                WriteToPlayer(farmer, PasswordValidation.notAuthorizedMessage);
                return;
            }

            Wallet.Merge(farmer);
        }

        private void SpawnMonster(Farmer farmer)
        {
            if (false == PasswordValidation.IsAuthorized(farmer.UniqueMultiplayerID, p => p.SpawnMonster))
            {
                WriteToPlayer(farmer, PasswordValidation.notAuthorizedMessage);
                return;
            }

            if (MultiplayerOptions.SpawnMonstersAtNight)
            {
                WriteToPlayer(null, $"No more monsters will appear." + TextColor.Green);
                MultiplayerOptions.SpawnMonstersAtNight = false;
            }
            else
            {
                WriteToPlayer(null, $"Monsters will appear." + TextColor.Red);
                MultiplayerOptions.SpawnMonstersAtNight = true;
            }
        }

        private void MoveBuildPermissionSub(Farmer farmer, string param)
        {
            if (false == PasswordValidation.IsAuthorized(farmer.UniqueMultiplayerID, p => p.MoveBuildPermission))
            {
                WriteToPlayer(farmer, PasswordValidation.notAuthorizedMessage);
                return;
            }

            if (MoveBuildPermission.parameter.Any(param.Equals))
            {
                if (config.MoveBuildPermission == param)
                {
                    WriteToPlayer(farmer, "Parameter for MoveBuildPermission is already " + config.MoveBuildPermission + TextColor.Orange);
                }
                else
                {
                    config.MoveBuildPermission = param;
                    MoveBuildPermission.Change(config.MoveBuildPermission);
                    helper.WriteConfig(config);
                }
            }
            else
            {
                WriteToPlayer(farmer, $"Only the following parameters are valid for MoveBuildPermission: {String.Join(", ", MoveBuildPermission.parameter.ToArray())}" + TextColor.Red);
            }
        }

        private void WriteToPlayer(Farmer farmer, string message)
        {
            if (null == farmer || farmer.UniqueMultiplayerID == Game1.player.UniqueMultiplayerID)
            {
                chatBox.textBoxEnter($" {message}");
            }
            else
            {
                chatBox.textBoxEnter($"/message {farmer.Name} {message}");
            }
        }

    }
}
