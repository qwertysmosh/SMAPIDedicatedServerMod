using DedicatedServer.Chat;
using DedicatedServer.Config;
using StardewModdingAPI;
using StardewValley;
using System.Collections.Generic;
using static StardewValley.FarmerTeam;

namespace DedicatedServer.Utils
{
    internal class MoveBuildPermission
    {
        private static IMonitor monitor = null;
        private static ModConfig config = null;
        private static EventDrivenChatBox chatBox = null;

        private static readonly string moveBuildPermission =
            Game1.content.LoadString("Strings\\UI:GameMenu_MoveBuildingPermissions");

        private static readonly string[] moveBuildPermissionStrings = new string[]{
            Game1.content.LoadString("Strings\\UI:GameMenu_MoveBuildingPermissions_Off"),
            Game1.content.LoadString("Strings\\UI:GameMenu_MoveBuildingPermissions_Owned"),
            Game1.content.LoadString("Strings\\UI:GameMenu_MoveBuildingPermissions_On") };

        public static readonly List<string> parameter = new List<string>() { "off", "owned", "on" };

        /// <summary>
        ///         Alias to change farmhands permissions to move buildings.
        /// </summary>
        private static RemoteBuildingPermissions farmhandsCanMoveBuildings
        {
            get => Game1.player.team.farmhandsCanMoveBuildings.Value;
            set => Game1.player.team.farmhandsCanMoveBuildings.Value = value;
        }

        public MoveBuildPermission(IMonitor monitor, ModConfig config, EventDrivenChatBox chatBox)
        {
            MoveBuildPermission.monitor = monitor;
            MoveBuildPermission.config = config;
            MoveBuildPermission.chatBox = chatBox;

            MoveBuildPermission.Change(config.MoveBuildPermission);
        }

        /// <summary>
        ///         Changes farmhands permissions to move buildings from the Carpenter's shop.
        /// </summary>
        /// <param name="moveBuildPermission">
        ///         Accepts the type <see cref="RemoteBuildingPermissions"/> as a string as well
        /// <br/>   as the normal multiplayer parameters "on", "owned" and "off".
        /// <br/>   Is not case sensitive.
        /// </param>
        public static void Change(string moveBuildPermission)
        {
            RemoteBuildingPermissions buildPermission;
            switch (moveBuildPermission.ToLower())
            {
                case "on":
                    buildPermission = RemoteBuildingPermissions.On;
                    monitor?.Log($"Changed move permission to {RemoteBuildingPermissions.On}", LogLevel.Debug);
                    break;
                case "owned":
                case "ownedbuildings":
                    buildPermission = RemoteBuildingPermissions.OwnedBuildings;
                    monitor?.Log($"Changed move permission to {RemoteBuildingPermissions.OwnedBuildings}", LogLevel.Debug);
                    break;
                default:
                    buildPermission = RemoteBuildingPermissions.Off;
                    monitor?.Log($"Changed move permission to {RemoteBuildingPermissions.Off}", LogLevel.Debug);
                    break;
            }

            farmhandsCanMoveBuildings = buildPermission;
            WriteMoveBuildPermission();
        }

        public static void Change(RemoteBuildingPermissions buildPermission)
        {
            farmhandsCanMoveBuildings = buildPermission;
            WriteMoveBuildPermission();
        }

        public static void WriteMoveBuildPermission()
        {
            chatBox?.textBoxEnter(
                " " + moveBuildPermission + ": " +
                moveBuildPermissionStrings[(int)farmhandsCanMoveBuildings] + 
                TextColor.Green);
        }
    }
}
