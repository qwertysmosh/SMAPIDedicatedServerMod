using DedicatedServer.HostAutomatorStages.BehaviorStates;
using DedicatedServer.Utils;
using StardewValley;

namespace DedicatedServer.HostAutomatorStages
{
    internal class GetFishingRodBehaviorLink : BehaviorLink
    {
        #region Required in derived class

        public override int WaitTimeAutoLoad { get; set; } = 0;
        public override int WaitTime { get; set; }

        public override void Process()
        {
            //If we don't get the fishing rod, Willy isn't available
            if (false == Game1.player.eventsSeen.Contains("739330") && 
                Game1.player.hasQuest("13") && 
                Game1.timeOfDay <= 1710 && 
                false == isGettingFishingRod && 
                false == Utility.isFestivalDay(Game1.Date.DayOfMonth, Game1.Date.Season))
            {
                Game1.player.warpFarmer(WarpPoints.beachWarp);
                isGettingFishingRod = true;
            }
            else if (isGettingFishingRod && 
                Game1.player.eventsSeen.Contains("739330"))
            {
                Game1.player.warpFarmer(WarpPoints.FarmWarp);
                isGettingFishingRod = false;
            }
        }

        #endregion

        private bool isGettingFishingRod = false;
    }
}
