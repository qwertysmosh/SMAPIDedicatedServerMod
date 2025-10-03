using DedicatedServer.HostAutomatorStages.BehaviorStates;
using DedicatedServer.Utils;
using StardewValley;

namespace DedicatedServer.HostAutomatorStages
{
    internal class UnlockCommunityCenterBehaviorLink : BehaviorLink
    {
        #region Required in derived class

        public override int WaitTimeAutoLoad { get; set; } = 0;
        public override int WaitTime { get; set; }

        public override void Process()
        {
            if (false == Game1.player.eventsSeen.Contains("611439") && 
                Game1.stats.DaysPlayed > 4 && 
                Game1.timeOfDay >= 800 && 
                Game1.timeOfDay <= 1300 && 
                false == Game1.IsRainingHere(Game1.getLocationFromName("Town")) && 
                false == isUnlocking && 
                false == Utility.isFestivalDay(Game1.Date.DayOfMonth, Game1.Date.Season))
            {
                isUnlocking = true;
                DedicatedServer.Warp(WarpPoints.townWarp);
            }
            else if (isUnlocking &&
                null == Game1.CurrentEvent &&
                null == Game1.activeClickableMenu &&
                Game1.player.eventsSeen.Contains("611439")
            )
            {
                isUnlocking = false;
                DedicatedServer.Warp(WarpPoints.FarmWarp);
            }
        }

        #endregion

        private bool isUnlocking = false;
    }
}
