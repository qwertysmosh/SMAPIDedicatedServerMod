using Netcode;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DedicatedServer.HostAutomatorStages
{
    internal class ReadyCheckHelper
    {
        public static void OnDayStarted(object sender, StardewModdingAPI.Events.DayStartedEventArgs e)
        {
            //Checking mailbox sometimes gives some gold, but it's compulsory to unlock some events
            for (int i = 0; i < 10; ++i) {
                Game1.getFarm().mailbox();
            }

            //Unlocks the sewer
            if (!Game1.player.eventsSeen.Contains("295672") && Game1.netWorldState.Value.MuseumPieces.Count() >= 60) {
                Game1.player.eventsSeen.Add("295672");
            }

            //Upgrade farmhouse to match highest level cabin
            var targetLevel = Game1.getFarm().buildings.Where(o => o.isCabin).Select(o => ((Cabin)o.indoors.Value).upgradeLevel).DefaultIfEmpty(0).Max();
            if (targetLevel > Game1.player.HouseUpgradeLevel) {
                Game1.player.HouseUpgradeLevel = targetLevel;
                Game1.player.performRenovation("FarmHouse");
            }
        }

        /// <summary>
        ///         Get whether all required players are ready to proceed.
        /// </summary>
        /// <param name="checkName">The ready check ID.</param>
        /// <returns>
        ///         true : If all required players are ready
        /// <br/>   false: If one required player is not ready</returns>
        public static bool IsReady(string checkName)
        {
            return Game1.netReady.IsReady(checkName);
        }
    }
}
