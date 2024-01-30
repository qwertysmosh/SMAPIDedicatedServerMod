using DedicatedServer.Chat;
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
using System.ComponentModel;
using System.Linq;

namespace DedicatedServer.MessageCommands
{
    internal class ServerCommandListener
    {
        private EventDrivenChatBox chatBox;

        private ModConfig config;

        private IModHelper helper;

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

        private void chatReceived(object sender, ChatEventArgs e)
        {
            var tokens = e.Message.Split(' ');

            if (0 == tokens.Length) { return; }

            string command = tokens[0].ToLower();

            if(Game1.player.UniqueMultiplayerID == e.SourceFarmerId)
            {
                switch (command)
                {
                    case "letmeplay":
                        chatBox.textBoxEnter($"The host is now a player, all host functions are deactivated." + TextColor.Green);
                        HostAutomation.LetMePlay();
                        break;

                    #region DEBUG_COMMANDS
                    #if false

                    case "letmecontrol":
                        HostAutomation.LetMeControl();
                        break;

                    case "multiplayer":
                        MultiplayerOptions.EnableServer = true;
                        break;

                    case "singleplayer":
                        MultiplayerOptions.EnableServer = false;
                        break;

                    case "farm":
                        var farm = Game1.getLocationFromName("farm") as Farm;
                        var mainFarmHouseEntry = farm.GetMainFarmHouseEntry();
                        Game1.player.warpFarmer(new Warp(mainFarmHouseEntry.X, mainFarmHouseEntry.Y, farm.NameOrUniqueName, mainFarmHouseEntry.X, mainFarmHouseEntry.Y, false, false));
                        break;

                    case "mine":
                        var mine = Game1.getLocationFromName("Mine") as Mine;
                        Game1.player.warpFarmer(new Warp(18, 13, mine.NameOrUniqueName, 18, 13, false));
                        break;

                    case "location":
                        var location = Game1.player.getTileLocation();
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

            string param = 1 < tokens.Length ? tokens[1].ToLower() : "";

            var sourceFarmer = Game1.otherFarmers.Values
                .Where(farmer => farmer.UniqueMultiplayerID == e.SourceFarmerId)
                .FirstOrDefault()?
                .Name ?? Game1.player.Name;

            switch (command)
            {
                case "takeover": // /message ServerBot TakeOver
                    TakeOver(e.SourceFarmerId);
                    break;

                case "safeinvitecode": // /message ServerBot SafeInviteCode
                    SafeInviteCode(e.SourceFarmerId);
                    break;

                case "invitecode": // /message ServerBot InviteCode
                    InviteCode(e.SourceFarmerId);
                    break;

                case "invisible": // /message ServerBot Invisible
                    InvisibleSub(e.SourceFarmerId);
                    break;

                case "sleep": // /message ServerBot Sleep
                    Sleep(e.SourceFarmerId);
                    break;

                case "resetday": // /message ServerBot ResetDay
                    ResetDay(e.SourceFarmerId);
                    break;

                case "shutdown": // /message ServerBot Shutdown
                    Shutdown(e.SourceFarmerId);
                    break;

                case "spawnmonster": // /message ServerBot SpawnMonster
                    SpawnMonster(e.SourceFarmerId);
                    break;

                case "mbp": // /message ServerBot mbp on
                case "movebuildpermission":
                case "movepermissiong":
                    MoveBuildPermission(e.SourceFarmerId, param);
                    break;

                default:
                    break;
            }
        }

        private void TakeOver(long id)
        {
            if (false == PasswordValidation.IsAuthorized(id, p => p.TakeOver))
            {
                chatBox.textBoxEnter(PasswordValidation.notAuthorizedMessage);
                return;
            }

            chatBox.textBoxEnter($"Control has been transferred to the host, all host functions are switched on." + TextColor.Aqua);
            HostAutomation.TakeOver();
        }
        
        private void SafeInviteCode(long id)
        {
            if (false == PasswordValidation.IsAuthorized(id, p => p.SafeInviteCode))
            {
                chatBox.textBoxEnter(PasswordValidation.notAuthorizedMessage);
                return;
            }

            MultiplayerOptions.SaveInviteCode();
            if (MultiplayerOptions.IsInviteCodeAvailable)
            {
                chatBox.textBoxEnter($"Your invite code is saved in the mod folder in the file {MultiplayerOptions.inviteCodeSaveFile}." + TextColor.Green);
            }
            else
            {
                chatBox.textBoxEnter($"The game has no invite code." + TextColor.Red);
            }
        }
        
        private void InviteCode(long id)
        {
            if (false == PasswordValidation.IsAuthorized(id, p => p.InviteCode))
            {
                chatBox.textBoxEnter(PasswordValidation.notAuthorizedMessage);
                return;
            }

            chatBox.textBoxEnter($"Invite code: {MultiplayerOptions.InviteCode}" + ("" == MultiplayerOptions.InviteCode ? TextColor.Red : TextColor.Green));
        }

        private void InvisibleSub(long id)
        {
            if (false == PasswordValidation.IsAuthorized(id, p => p.Invisible))
            {
                chatBox.textBoxEnter(PasswordValidation.notAuthorizedMessage);
                return;
            }

            Invisible.InvisibleOverwrite = !Invisible.InvisibleOverwrite;
            chatBox.textBoxEnter($"The host is invisible {Invisible.InvisibleOverwrite}" + TextColor.Aqua);
        }

        /// <summary>
        ///         (Toggle command)
        /// <br/>   When it is sent, the host goes to bed.When all players leave the game
        /// <br/>   or go to bed, the next day begins.On a second send, the host will get
        /// <br/>   up and the mod's normal behavior will be restored.
        /// </summary>
        /// <param name="id">ID of the player who requested the command</param>
        private void Sleep(long id)
        {
            if (false == PasswordValidation.IsAuthorized(id, p => p.Sleep))
            {
                chatBox.textBoxEnter(PasswordValidation.notAuthorizedMessage);
                return;
            }

            chatBox.textBoxEnter("Host will go to bed." + TextColor.Jungle);
#warning Debug
            return;

            if (false == HostAutomation.EnableHostAutomation)
            {
                chatBox.textBoxEnter($"Cannot start sleep because the host is controlled by the player." + TextColor.Red);
                return;
            }

            if (Sleeping.ShouldSleepOverwrite)
            {
                Sleeping.ShouldSleepOverwrite = false;
                chatBox.textBoxEnter($"The host is back on his feet." + TextColor.Aqua);
            }
            else
            {
                chatBox.textBoxEnter($"The host will go to sleep." + TextColor.Green);
                Sleeping.ShouldSleepOverwrite = true;
            }
        }

        private void ResetDay(long id)
        {
            if (false == PasswordValidation.IsAuthorized(id, p => p.ResetDay))
            {
                chatBox.textBoxEnter(PasswordValidation.notAuthorizedMessage);
                return;
            }

            RestartDay.ResetDay((seconds) => chatBox.textBoxEnter($"Attention: Server will reset the day in {seconds} seconds" + TextColor.Orange));
        }

        private void Shutdown(long id)
        {
            if (false == PasswordValidation.IsAuthorized(id, p => p.Shutdown))
            {
                chatBox.textBoxEnter(PasswordValidation.notAuthorizedMessage);
                return;
            }

            RestartDay.ShutDown((seconds) => chatBox.textBoxEnter($"Attention: Server will shut down in {seconds} seconds" + TextColor.Orange));
        }
        
        private void SpawnMonster(long id)
        {
            if (false == PasswordValidation.IsAuthorized(id, p => p.SpawnMonster))
            {
                chatBox.textBoxEnter(PasswordValidation.notAuthorizedMessage);
                return;
            }

            if (MultiplayerOptions.SpawnMonstersAtNight)
            {
                chatBox.textBoxEnter($"No more monsters will appear." + TextColor.Green);
                MultiplayerOptions.SpawnMonstersAtNight = false;
            }
            else
            {
                chatBox.textBoxEnter($"Monsters will appear." + TextColor.Red);
                MultiplayerOptions.SpawnMonstersAtNight = true;
            }
        }

        /// <summary>
        ///         (Saved in config)
        /// <br/>   Changes farmhands permissions to move buildings from the Carpenter's shop.
        /// <br/>   Set to `off` to entirely disable moving buildings, set to `owned` to allow
        /// <br/>   farmhands to move buildings that they purchased, or set to `on` to allow
        /// <br/>   moving all buildings.
        /// <br/>   
        /// <br/>   As the host you can run commands in the chat box, using a forward slash(/) before the command.
        /// <br/>   See: <seealso href="https://stardewcommunitywiki.com/Multiplayer"/>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="param"></param>
        private void MoveBuildPermission(long id, string param)
        {
            if (false == PasswordValidation.IsAuthorized(id, p => p.MoveBuildPermission))
            {
                chatBox.textBoxEnter(PasswordValidation.notAuthorizedMessage);
                return;
            }

            var moveBuildPermissionParameter = new List<string>() { "off", "owned", "on" };

            if (moveBuildPermissionParameter.Any(param.Equals))
            {
                if (config.MoveBuildPermission == param)
                {
                    chatBox.textBoxEnter("Parameter for MoveBuildPermission is already " + config.MoveBuildPermission + TextColor.Orange);
                }
                else
                {
                    config.MoveBuildPermission = param;
                    chatBox.textBoxEnter($"Changed MoveBuildPermission to {config.MoveBuildPermission}" + TextColor.Green);
                    chatBox.textBoxEnter("/mbp " + config.MoveBuildPermission);
                    helper.WriteConfig(config);
                }
            }
            else
            {
                chatBox.textBoxEnter($"Only the following parameters are valid for MoveBuildPermission: {String.Join(", ", moveBuildPermissionParameter.ToArray())}" + TextColor.Red);
            }
        }
    }
}
