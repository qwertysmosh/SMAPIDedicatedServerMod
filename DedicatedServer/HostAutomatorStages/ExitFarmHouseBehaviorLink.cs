using Microsoft.Xna.Framework;
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

        private static Farm farmLocation = Game1.getLocationFromName("Farm") as Farm;

        private static FarmHouse farmHouseLocation = Game1.getLocationFromName("FarmHouse") as FarmHouse;

        private static Point farmHouseEnryLocation = farmHouseLocation.getEntryLocation();

        /// <summary>
        ///         Warppoint on the farm
        /// <br/>
        /// <br/>   Warping to 64, 10 warps just behind the farmhouse.
        /// <br/>   It "hides" the bot, but still allows him to perform
        /// <br/>   actions like talking to npcs.
        /// <br/>   64, 15 coords are "magic numbers" pulled from Game1.cs, line 11282, warpFarmer()
        /// </summary>
        private static Warp farmWarp = new Warp(64, 15, farmLocation.NameOrUniqueName, 64, 10, false, false);

        /// <summary>
        ///         Warppoint into the farmhouse
        /// </summary>
        private static Warp farmHouseWarp = new Warp(
            farmHouseEnryLocation.X, farmHouseEnryLocation.Y,
            farmHouseLocation.NameOrUniqueName,
            farmHouseEnryLocation.X, farmHouseEnryLocation.Y,
            false, false);

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
                Game1.player.warpFarmer(farmWarp);
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
                    Game1.player.warpFarmer(farmHouseWarp);
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
