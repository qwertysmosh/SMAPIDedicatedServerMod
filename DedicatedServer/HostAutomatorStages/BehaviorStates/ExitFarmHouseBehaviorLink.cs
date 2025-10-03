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

        public override int WaitTimeAutoLoad { get; set; } = 0;
        public override int WaitTime { get; set; }

        public override void Process()
        {
            switch (exitFarmHouseStates)
            {
                case ExitFarmHouseStates.Uninit:
                    if (false == Game1.MasterPlayer.IsBusyDoingSomething())
                    {  
                        if (Game1.currentLocation is FarmHouse)
                        {
                            exitFarmHouseStates = ExitFarmHouseStates.Farmhouse;
                        }
                        if (Game1.currentLocation is Farm)
                        {
                            exitFarmHouseStates = ExitFarmHouseStates.Farm;
                        }
                    }
                    break;


                case ExitFarmHouseStates.WarpToFarmhouse:
                    if (Game1.currentLocation is FarmHouse)
                    {
                        WaitTime = 120;
                        exitFarmHouseStates = ExitFarmHouseStates.Farmhouse; 
                    }
                    break;


                case ExitFarmHouseStates.Farmhouse:
                    if (false == Game1.MasterPlayer.IsBusyDoingSomething())
                    {
                        if (null == Game1.CurrentEvent &&
                            null == Game1.activeClickableMenu &&
                            null != Game1.currentLocation &&
                            Game1.timeOfDay < TimeToEnterFarmhouse &&
                            Game1.currentLocation is FarmHouse
                        ){
                            exitFarmHouseStates = ExitFarmHouseStates.WarpToFarm;
                            DedicatedServer.Warp(WarpPoints.FarmWarp);
                        }
                    }
                    break;


                case ExitFarmHouseStates.WarpToFarm:
                    if (Game1.currentLocation is Farm)
                    {
                        WaitTime = 120;
                        exitFarmHouseStates = ExitFarmHouseStates.Farm;
                    }
                    break;


                case ExitFarmHouseStates.Farm:
                    if (false == Game1.MasterPlayer.IsBusyDoingSomething())
                    {
                        if (null == Game1.CurrentEvent &&
                            null == Game1.activeClickableMenu &&
                            null != Game1.currentLocation &&
                            Game1.timeOfDay >= TimeToEnterFarmhouse &&
                            Game1.currentLocation is Farm
                        ){
                            exitFarmHouseStates = ExitFarmHouseStates.WarpToFarmhouse;
                            DedicatedServer.Warp(WarpPoints.FarmHouseWarp);
                        }
                    }
                    break;


                default:
                    exitFarmHouseStates = ExitFarmHouseStates.Uninit;
                    break;

            }
        }

#endregion

        private enum ExitFarmHouseStates
        {
            Uninit = 0,
            WarpToFarmhouse,
            Farmhouse,
            WarpToFarm,
            Farm
        }

        private static ExitFarmHouseStates exitFarmHouseStates = ExitFarmHouseStates.Uninit;

        /// <summary>
        ///         After this time, the host is teleported into the house.
        /// </summary>
        protected static int TimeToEnterFarmhouse { set; get; } = 1800;

        private static void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            exitFarmHouseStates = ExitFarmHouseStates.Farmhouse;
        }

        public ExitFarmHouseBehaviorLink()
        {
            exitFarmHouseStates = ExitFarmHouseStates.Uninit;
            Enable();
        }

        ~ExitFarmHouseBehaviorLink() => Dispose();

        public static void Dispose() => Disable();

        private static void Enable() => DedicatedServer.helper.Events.GameLoop.DayStarted += OnDayStarted;

        private static void Disable() => DedicatedServer.helper.Events.GameLoop.DayStarted -= OnDayStarted;
    }
}
