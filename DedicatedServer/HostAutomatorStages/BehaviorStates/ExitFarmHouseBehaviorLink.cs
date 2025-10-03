using DedicatedServer.HostAutomatorStages.BehaviorStates;
using DedicatedServer.Utils;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;

namespace DedicatedServer.HostAutomatorStages
{
    internal class ExitFarmHouseBehaviorLink : BehaviorLink
    {
        #region Required in derived class

        public override int WaitTimeAutoLoad { get; set; } = 60;
        public override int WaitTime { get; set; }

        public override void Process()
        {
            if (false == hasExitedFarmhouse &&
                Game1.timeOfDay < TimeToEnterFarmhouse &&
                null != Game1.currentLocation && 
                Game1.currentLocation is FarmHouse)
            {
                hasExitedFarmhouse = true;
                DedicatedServer.Warp(WarpPoints.FarmWarp);
            }
            else
            {
                if (true == hasExitedFarmhouse &&
                    null == Game1.CurrentEvent &&
                    null == Game1.activeClickableMenu &&
                    Game1.timeOfDay >= TimeToEnterFarmhouse &&
                    null != Game1.currentLocation && 
                    Game1.currentLocation is Farm &&
                    true == Game1.spawnMonstersAtNight
                ){
                    hasExitedFarmhouse = false;
                    DedicatedServer.Warp(WarpPoints.FarmHouseWarp);
                }
            }
        }

        #endregion

        /// <summary>
        ///         After this time, the host is teleported into the house.
        /// </summary>
        protected static int TimeToEnterFarmhouse { set; get; } = 1800;

        private static bool hasExitedFarmhouse = false;

        private static void OnDayStarted(object sender, DayStartedEventArgs e) => hasExitedFarmhouse = false;

        public ExitFarmHouseBehaviorLink() => Enable();

        ~ExitFarmHouseBehaviorLink() => Dispose();

        public static void Dispose() => Disable();

        private static void Enable() => DedicatedServer.helper.Events.GameLoop.DayStarted += OnDayStarted;

        private static void Disable() => DedicatedServer.helper.Events.GameLoop.DayStarted -= OnDayStarted;
    }
}
