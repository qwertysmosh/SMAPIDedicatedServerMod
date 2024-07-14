using DedicatedServer.Utils;
using StardewValley;

namespace DedicatedServer.HostAutomatorStages
{
    internal class UnlockCommunityCenterBehaviorLink : BehaviorLink
    {
        private bool isUnlocking;

        public UnlockCommunityCenterBehaviorLink(BehaviorLink next = null) : base(next)
        {
            isUnlocking = false;
        }

        public override void Process(BehaviorState state)
        {
            if (!Game1.player.eventsSeen.Contains("611439") && Game1.stats.DaysPlayed > 4 && Game1.timeOfDay >= 800 && Game1.timeOfDay <= 1300 && !Game1.IsRainingHere(Game1.getLocationFromName("Town")) && !isUnlocking && !Utility.isFestivalDay(Game1.Date.DayOfMonth, Game1.Date.Season))
            {
                Game1.player.warpFarmer(WarpPoints.townWarp);
                isUnlocking = true;
            }
            else if (isUnlocking && Game1.player.eventsSeen.Contains("611439")) {
                Game1.player.warpFarmer(WarpPoints.FarmWarp);
                isUnlocking = false;
            }
            else
            {
                processNext(state);
            }
        }
    }
}
