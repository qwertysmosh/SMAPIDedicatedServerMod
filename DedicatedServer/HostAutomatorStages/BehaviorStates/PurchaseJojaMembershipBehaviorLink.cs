using DedicatedServer.HostAutomatorStages.BehaviorStates;
using StardewValley;

namespace DedicatedServer.HostAutomatorStages
{
    internal class PurchaseJojaMembershipBehaviorLink : BehaviorLink
    {
        #region Required in derived class

        public override int WaitTimeAutoLoad { get; set; } = 0;
        public override int WaitTime { get; set; }

        public override void Process()
        {
            // If the community center has been unlocked, the config specifies that the host
            // should purchase the joja membership, and the host has not yet purchased it...
            var ccAvailable = Game1.player.eventsSeen.Contains("611439");
            var purchased = 
                Game1.player.mailForTomorrow.Contains("JojaMember%&NL&%") || 
                Game1.player.mailReceived.Contains("JojaMember");

            if (ccAvailable && DedicatedServer.config.PurchaseJojaMembership && false == purchased)
            {
                // Then purchase it
                Game1.addMailForTomorrow("JojaMember", noLetter: true, sendToEveryone: true);
                Game1.player.removeQuest("26");
            }
        }

        #endregion
    }
}
