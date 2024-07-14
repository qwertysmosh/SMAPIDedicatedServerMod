using DedicatedServer.Utils;
using StardewValley;

namespace DedicatedServer.HostAutomatorStages
{
    internal class GetFishingRodBehaviorLink : BehaviorLink
    {
        private bool isGettingFishingRod;

        public GetFishingRodBehaviorLink(BehaviorLink next = null) : base(next)
        {
            isGettingFishingRod = false;
        }

        public override void Process(BehaviorState state)
        {
            //If we don't get the fishing rod, Willy isn't available
            if (!Game1.player.eventsSeen.Contains("739330") && Game1.player.hasQuest("13") && Game1.timeOfDay <= 1710 && !isGettingFishingRod && !Utility.isFestivalDay(Game1.Date.DayOfMonth, Game1.Date.Season))
            {
                Game1.player.warpFarmer(WarpPoints.beachWarp);
                isGettingFishingRod = true;
            }
            else if (isGettingFishingRod && Game1.player.eventsSeen.Contains("739330")) {
                Game1.player.warpFarmer(WarpPoints.FarmWarp);
                isGettingFishingRod = false;
            }
            else
            {
                processNext(state);
            }
        }
    }
}
