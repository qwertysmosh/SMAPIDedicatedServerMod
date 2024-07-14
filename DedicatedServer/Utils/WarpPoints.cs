using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using StardewValley.Locations;
using xTile.Dimensions;

namespace DedicatedServer.Utils
{
    internal abstract class WarpPoints
    {
        private static readonly Farm farmLocation = Game1.getLocationFromName("Farm") as Farm;
        private static readonly FarmHouse farmHouseLocation = Game1.getLocationFromName("FarmHouse") as FarmHouse;
        private static readonly Town townLocation = Game1.getLocationFromName("Town") as Town;
        private static readonly Mine mineLocation = Game1.getLocationFromName("Mine") as Mine;
        private static readonly Beach beachLocation = Game1.getLocationFromName("Beach") as Beach;
        private static readonly Mountain mountainLocation = Game1.getLocationFromName("Mountain") as Mountain;

        private static Point FarmEntryLocation => farmLocation.GetMainFarmHouseEntry();
        private static Point FarmHouseEntryLocation => farmHouseLocation.getEntryLocation();

        private static readonly Point townNorthWestEntryLocation = new Point(0, 54);
        private static readonly Point mineEntryLocation = new Point(18, 13);
        private static readonly Point beachEntryLocation = new Point(38, 0);
        private static readonly Point robinLocation = new Point(12, 26);
        private static readonly Point clintLocation = new Point(94, 82);

        /// <summary>
        ///         Warppoint on the farm
        /// <br/>
        /// <br/>   As the host is invisible and cannot be interacted with, the position
        /// <br/>   does not matter. A visible position simply allows you to interact with
        /// <br/>   the host when it has been made visible again with the `Invisible` command.
        /// <br/>   +2 places the host on the veranda on the right.
        /// </summary>
        public static Warp FarmWarp
        {
            get
            {
                var location = FarmEntryLocation;
                return new Warp(
                    location.X + 2, location.Y,
                    farmLocation.NameOrUniqueName,
                    location.X + 2, location.Y,
                    false, false);
            }
        }

        /// <summary>
        ///         Warppoint into the farmhouse
        /// </summary>
        public static Warp FarmHouseWarp
        {
            get
            {
                var location = FarmHouseEntryLocation;
                return new Warp(
                    location.X, location.Y,
                    farmHouseLocation.NameOrUniqueName,
                    location.X, location.Y,
                    false, false);
            }
        }

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

        /// <summary>
        ///         Warp to Robin
        /// </summary>
        public static readonly Warp robinWarp = new Warp(
            robinLocation.X, robinLocation.Y,
            mountainLocation.NameOrUniqueName,
            robinLocation.X, robinLocation.Y,
            false, false);

        /// <summary>
        ///         Warp to Clint
        /// </summary>
        public static readonly Warp clintWarp = new Warp(
            clintLocation.X, clintLocation.Y,
            townLocation.NameOrUniqueName,
            clintLocation.X, clintLocation.Y,
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
