using DedicatedServer.Chat;
using DedicatedServer.Config;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DedicatedServer
{
    internal static class DedicatedServer
    {
        private static FieldInfo multiplayerFieldInfo = typeof(Game1).GetField("multiplayer", BindingFlags.NonPublic | BindingFlags.Static);

        private static Multiplayer multiplayer = null;

        public static IModHelper helper { get; private set; }
        public static IMonitor monitor { get; private set; }
        public static ModConfig config { get; private set; }
        public static EventDrivenChatBox chatBox { get; private set; }

        public static void InitStaticVariables(IModHelper helper, IMonitor monitor, ModConfig config, EventDrivenChatBox chatBox)
        {
            DedicatedServer.helper = helper;
            DedicatedServer.monitor = monitor;
            DedicatedServer.chatBox = chatBox;
        }

        public static IDictionary<long, Farmer> otherPlayers { get; } = new Dictionary<long, Farmer>();

#warning (Function optimization possible), alternative: `Game1.getOnlineFarmers().Count - 1`
        public static void UpdateOtherPlayers()
        {
            if (multiplayer == null)
            {
                multiplayer = (Multiplayer)multiplayerFieldInfo.GetValue(null);
            }
            otherPlayers.Clear();
            foreach (var farmer in Game1.otherFarmers.Values)
            {
                if (false == multiplayer.isDisconnecting(farmer))
                {
                    otherPlayers.Add(farmer.UniqueMultiplayerID, farmer);
                }
            }
        }

#warning (Remove direct access to `otherPlayers`)
        public static int GetNumOtherPlayers()
        {
            return otherPlayers.Count;
        }

#warning (Remove direct access to `otherPlayers`)
        public static IDictionary<long, Farmer> GetOtherPlayers()
        {
            return otherPlayers;
        }
    }
}
