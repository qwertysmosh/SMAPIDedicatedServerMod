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

        private static Point farmEnryLocation = farmLocation.GetMainFarmHouseEntry();

        private static FarmHouse farmHouseLocation = Game1.getLocationFromName("FarmHouse") as FarmHouse;

        private static Point farmHouseEnryLocation = farmHouseLocation.getEntryLocation();

        /// <summary>
        ///         Warppoint on the farm
        /// <br/>
        /// <br/>   As the host is invisible and cannot be interacted with, the position
        /// <br/>   does not matter. A visible position simply allows you to interact with
        /// <br/>   the host when it has been made visible again with the `Invisible` command.
        /// <br/>   +2 places the host on the veranda on the right
        /// <br/>
        /// <br/>   Warping to 64, 10 warps just behind the farmhouse. It "hides" the bot,
        /// <br/>   but still allows him to perform actions like talking to npcs.
        /// <br/>   64, 15 coords are "magic numbers" pulled from Game1.cs, line 11282, warpFarmer()
        /// </summary>
        private static Warp farmWarp = new Warp(
            farmEnryLocation.X + 2, farmEnryLocation.Y,
            farmLocation.NameOrUniqueName,
            farmEnryLocation.X + 2, farmEnryLocation.Y,
            false, false);

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
