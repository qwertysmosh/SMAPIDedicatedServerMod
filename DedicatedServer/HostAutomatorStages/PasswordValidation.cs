using DedicatedServer.Chat;
using DedicatedServer.Config;
using DedicatedServer.Utils;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using xTile.ObjectModel;
using static StardewValley.Minigames.MineCart;

namespace DedicatedServer.HostAutomatorStages
{
    internal class PasswordValidation
    {
        public static string notAuthorizedMessage = $"You are not authorized to do this." + TextColor.Red;

        private IModHelper helper;

        private EventDrivenChatBox chatBox;

        private static ModConfig config = null;

        private static List<long> ids = new List<long>();

        private static bool passwordValidationDisabled = false;

        private static List<string> allowedPasswordProtectedFunctions = new List<string>();

        public PasswordValidation(IModHelper helper, ModConfig config, EventDrivenChatBox chatBox)
        {
            this.helper = helper;
            PasswordValidation.config = config;
            this.chatBox = chatBox;

            allowedPasswordProtectedFunctions.AddRange(
                typeof(PasswordProtectedCommands).GetProperties()
                    .Select(p => p.Name.ToLower()).ToList());
        }

        public void Enable()
        {
            if ("" == config.Password)
            {
                passwordValidationDisabled = true;
                return;
            }

            chatBox.ChatReceived += chatReceived;
            helper.Events.Multiplayer.PeerDisconnected += PlayerHasLoggedOut;
            //Host should not have access
            //LogIn(Game1.player.UniqueMultiplayerID);
            passwordValidationDisabled = false;
        }

        public void Disable(bool validationDisable = false)
        {
            chatBox.ChatReceived -= chatReceived;
            helper.Events.Multiplayer.PeerDisconnected -= PlayerHasLoggedOut;
            ids = new List<long>();
            passwordValidationDisabled = validationDisable;
        }


        public static bool IsAuthorized(long id, Func<PasswordProtectedCommands, bool> property = null)
        {
            if (passwordValidationDisabled)
            {
                return true;
            }

            if (null != property && false == property(config.PasswordProtected))
            {
                return true;
            }

            return ids.Contains(id);
        }

        /// <summary>
        /// <see cref="PasswordProtectedCommands"/>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chatReceived(object sender, ChatEventArgs e)
        {
            var tokens = e.Message.Split(' ');

            if (0 == tokens.Length) { return; }

            if (Game1.player.UniqueMultiplayerID != e.SourceFarmerId)
            {
                if (ChatBox.privateMessage != e.ChatKind) { return; }
            }

            string command = tokens[0].ToLower();

            string param = 1 < tokens.Length ? tokens[1] : "";

            long id = e.SourceFarmerId;

            var sourceFarmer = Game1.getAllFarmhands() // Host is null
                .Where(farmer => farmer.UniqueMultiplayerID == id)
                .FirstOrDefault();

            switch (command)
            {
                case "login": // /message ServerBot Login xxxx
                    LogIn(sourceFarmer, password: param, id);
                    break;

                case "logout": // /message ServerBot LogOut
                    LogOut(sourceFarmer, id);
                    break;

                case "passwordprotectadd": // /message ServerBot PasswordProtectAdd xxxx
                    PasswordProtectAdd(sourceFarmer, command: param, id);
                    break;

                case "passwordprotectremove": // /message ServerBot PasswordProtectRemove xxxx
                    PasswordProtectRemove(sourceFarmer, command: param, id);
                    break;
            }
        }

        private void LogIn(Farmer sourceFarmer, string password, long id)
        {
            if (password == config.Password)
            {
                if (AddId(id))
                {
                    WriteToPlayer(sourceFarmer, "Password correct." + TextColor.Green);
                }
                else
                {
                    WriteToPlayer(sourceFarmer, "You were already logged in." + TextColor.Green);
                }
            }
            else
            {
                WriteToPlayer(sourceFarmer, "Password wrong." + TextColor.Red);
            }
        }

        private void LogOut(Farmer sourceFarmer, long id)
        {
            if (RemoveId(id))
            {
                WriteToPlayer(sourceFarmer, "Successfully logged out." + TextColor.Green);
            }
            else
            {
                WriteToPlayer(sourceFarmer, "You were not logged in." + TextColor.Aqua);
            }
        }

        private void PasswordProtectAdd(Farmer sourceFarmer, string command, long id)
        {
            if (false == PasswordValidation.IsAuthorized(id))
            {
                chatBox.textBoxEnter(PasswordValidation.notAuthorizedMessage);
                return;
            }

            command = command.ToLower();
            if (allowedPasswordProtectedFunctions.Contains(command))
            {
                var property = config.PasswordProtected.GetType().GetProperties()
                    .Where(p => p.Name.ToLower() == command)
                    .FirstOrDefault();

                if (null != property)
                {
                    property.SetValue(config.PasswordProtected, true, null);
                    WriteToPlayer(sourceFarmer, $"added \"{command}\"" + TextColor.Green);
                }

                helper.WriteConfig(config);
            }
            else
            {
                WriteToPlayer(sourceFarmer, $"Parameter \"{command}\" is unknown." + TextColor.Red);
            }
        }

        private void PasswordProtectRemove(Farmer sourceFarmer, string command, long id)
        {
            if (false == PasswordValidation.IsAuthorized(id))
            {
                chatBox.textBoxEnter(PasswordValidation.notAuthorizedMessage);
                return;
            }

            command = command.ToLower();
            if (allowedPasswordProtectedFunctions.Contains(command))
            {
                var property = config.PasswordProtected.GetType().GetProperties()
                    .Where(p => p.Name.ToLower() == command)
                    .FirstOrDefault();

                if (null != property)
                {
                    property.SetValue(config.PasswordProtected, false, null);
                    WriteToPlayer(sourceFarmer, $"Removed \"{command}\"" + TextColor.Orange);
                }

                helper.WriteConfig(config);
            }
            else
            {
                WriteToPlayer(sourceFarmer, $"Parameter \"{command}\" is unknown." + TextColor.Red);
            }
        }

        private void PlayerHasLoggedOut(object sender, PeerDisconnectedEventArgs e)
        {
            RemoveId(e.Peer.PlayerID);
        }

        private void WriteToPlayer(Farmer farmer, string message)
        {
            chatBox.textBoxEnter((null == farmer) ? $"{message}" : $"/message {farmer.Name} {message}");
        }

        private bool AddId(long id)
        {
            if (false == ids.Contains(id))
            {
                ids.Add(id);
                return true;
            }

            return false;
        }

        private bool RemoveId(long id)
        {
            return ids.Remove(id);
        }

    }
}
