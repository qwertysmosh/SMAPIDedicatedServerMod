using DedicatedServer.HostAutomatorStages.BehaviorStates;
using DedicatedServer.Utils;
using StardewValley;
using StardewValley.Locations;

namespace DedicatedServer.HostAutomatorStages
{
    internal class ExitFarmHouseBehaviorLink : BehaviorLink2
    {
        #region Required in derived class

        public override int WaitTimeAutoLoad { get; set; } = 0;
        public override int WaitTime { get; set; }

        public override void Process()
        {
            if (false == hasExitedFarmhouse &&
                Game1.timeOfDay < TimeToEnterFarmhouse &&
                Game1.currentLocation != null && Game1.currentLocation is FarmHouse)
            {
                Game1.player.warpFarmer(WarpPoints.FarmWarp);
                hasExitedFarmhouse = true;

#warning This is a global wait Time
                // Set up wait ticks to wait for possible event
                //state.SetWaitTicks(60); 
            }
            else
            {
                if (true == hasExitedFarmhouse &&
                    Game1.timeOfDay >= TimeToEnterFarmhouse &&
                    Game1.currentLocation != null && Game1.currentLocation is Farm &&
                    true == Game1.spawnMonstersAtNight)
                {
                    Game1.player.warpFarmer(WarpPoints.FarmHouseWarp);
                    hasExitedFarmhouse = false;

#warning This is a global wait Time
                    // Set up wait ticks to wait for possible event
                    //state.SetWaitTicks(60);
                }
            }
        }

        #endregion

        /// <summary>
        ///         After this time, the host is teleported into the house.
        /// </summary>
        protected static int TimeToEnterFarmhouse { set; get; } = 1800;

        private bool hasExitedFarmhouse = false;
    }
}
