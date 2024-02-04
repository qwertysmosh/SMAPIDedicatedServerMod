using DedicatedServer.Utils;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DedicatedServer.HostAutomatorStages
{
    internal class ExitFarmHouseBehaviorLink : BehaviorLink
    {
        public ExitFarmHouseBehaviorLink(BehaviorLink next = null) : base(next)
        {
        }

        /// <summary>
        ///         After this time, the host is teleported into the house.
        /// </summary>
        protected static int TimeToEnterFarmhouse { set; get; } = 1800;

        public override void Process(BehaviorState state)
        {
            if (false == state.ExitedFarmhouse() &&
                Game1.timeOfDay < TimeToEnterFarmhouse && 
                Game1.currentLocation != null && Game1.currentLocation is FarmHouse)
            {
                Game1.player.warpFarmer(WarpPoints.farmWarp);
                state.ExitFarmhouse(); // Mark as exited
                state.SetWaitTicks(60); // Set up wait ticks to wait for possible event
            }
            else
            {
                if (true == state.ExitedFarmhouse() &&
                    Game1.timeOfDay >= TimeToEnterFarmhouse &&
                    Game1.currentLocation != null && Game1.currentLocation is Farm &&
                    true == Game1.spawnMonstersAtNight)
                {
                    Game1.player.warpFarmer(WarpPoints.farmHouseWarp);
                    state.EnteredFarmhouse();
                    state.SetWaitTicks(60);
                }
                else
                {
                    processNext(state);
                }
            }
        }
    }
}
