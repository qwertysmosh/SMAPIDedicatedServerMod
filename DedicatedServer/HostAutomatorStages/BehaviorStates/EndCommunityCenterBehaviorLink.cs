using DedicatedServer.HostAutomatorStages.BehaviorStates;
using DedicatedServer.Utils;
using StardewValley;

namespace DedicatedServer.HostAutomatorStages
{
    internal class EndCommunityCenterBehaviorLink : BehaviorLink
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
                false == Utility.isFestivalDay(Game1.Date.DayOfMonth, Game1.Date.Season)
            ){
                isEnding = true;
                DedicatedServer.Warp(WarpPoints.townWarp);
            }
            else if (isEnding &&
                null == Game1.CurrentEvent &&
                null == Game1.activeClickableMenu && 
                Game1.player.eventsSeen.Contains("191393")
            ){
                isEnding = false;
                DedicatedServer.Warp(WarpPoints.FarmWarp);
            }
        }

        #endregion

        private bool isEnding = false;
    }
}
