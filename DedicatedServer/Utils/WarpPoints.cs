using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using StardewValley.Locations;

namespace DedicatedServer.Utils
{
    internal abstract class WarpPoints
    {
        private static Farm farmLocation = Game1.getLocationFromName("Farm") as Farm;
        private static FarmHouse farmHouseLocation = Game1.getLocationFromName("FarmHouse") as FarmHouse;
        private static Town townLocation = Game1.getLocationFromName("Town") as Town;
        private static Mine mineLocation = Game1.getLocationFromName("Mine") as Mine;
        private static Beach beachLocation = Game1.getLocationFromName("Beach") as Beach;

        private static Point farmEntryLocation = farmLocation.GetMainFarmHouseEntry();
        private static Point farmHouseEntryLocation = farmHouseLocation.getEntryLocation();
        private static Point townNorthWestEntryLocation = new Point(0, 54);
        private static Point mineEntryLocation = new Point(18, 13);
        private static Point beachEntryLocation = new Point(38, 0);

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
        public static readonly Warp farmWarp = new Warp(
            farmEntryLocation.X + 2, farmEntryLocation.Y,
            farmLocation.NameOrUniqueName,
            farmEntryLocation.X + 2, farmEntryLocation.Y,
            false, false);

        /// <summary>
        ///         Warppoint into the farmhouse
        /// </summary>
        public static readonly Warp farmHouseWarp = new Warp(
            farmHouseEntryLocation.X, farmHouseEntryLocation.Y,
            farmHouseLocation.NameOrUniqueName,
            farmHouseEntryLocation.X, farmHouseEntryLocation.Y,
            false, false);

        /// <summary>
        ///         Warppoint to town, northwest entrance
        /// </summary>
        public static readonly Warp townWarp = new Warp(
            townNorthWestEntryLocation.X, townNorthWestEntryLocation.Y,
            townLocation.NameOrUniqueName,
            townNorthWestEntryLocation.X, townNorthWestEntryLocation.Y,
            false, false);

        /// <summary>
        ///         Warppoint to mine
        /// </summary>
        public static readonly Warp mineWarp = new Warp(
            mineEntryLocation.X, mineEntryLocation.Y,
            mineLocation.NameOrUniqueName,
            mineEntryLocation.X, mineEntryLocation.Y,
            false, false);


        /// <summary>
        ///         Warppoint to mine
        /// </summary>
        public static readonly Warp beachWarp = new Warp(
            beachEntryLocation.X, beachEntryLocation.Y,
            beachLocation.NameOrUniqueName,
            beachEntryLocation.X, beachEntryLocation.Y,
            false, false);

        public static Warp Refresh(Farmer farmer)
        {

            return new Warp(
                (int)farmer.Tile.X, (int)farmer.Tile.Y,
                farmer.currentLocation.Name,
                (int)farmer.Tile.X, (int)farmer.Tile.Y,
                false, false);
        }
    }
}
