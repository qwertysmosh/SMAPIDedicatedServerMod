using DedicatedServer.HostAutomatorStages.BehaviorStates;
using DedicatedServer.Utils;
using StardewValley;

namespace DedicatedServer.HostAutomatorStages
{
    internal class EndCommunityCenterBehaviorLink : BehaviorLink2
    {
        #region Required in derived class

        public override int WaitTimeAutoLoad { get; set; } = 0;
        public override int WaitTime { get; set; }

        public override void Process()
        {
            if (false == Game1.player.eventsSeen.Contains("191393") && 
                Game1.player.hasCompletedCommunityCenter() && 
                false == Game1.IsRainingHere(Game1.getLocationFromName("Town")) && 
                false == isEnding && 
                false == Utility.isFestivalDay(Game1.Date.DayOfMonth, Game1.Date.Season))
            {
                Game1.player.warpFarmer(WarpPoints.townWarp);
                isEnding = true;
            }
            else if (isEnding && Game1.player.eventsSeen.Contains("191393"))
            {
                Game1.player.warpFarmer(WarpPoints.FarmWarp);
                isEnding = false;
            }
        }

        #endregion

        private bool isEnding = false;
    }
}
