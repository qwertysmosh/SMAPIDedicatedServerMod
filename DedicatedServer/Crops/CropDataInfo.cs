using StardewValley;
using System.Collections.Generic;

namespace DedicatedServer.Crops
{
    internal class CropDataInfo
    {
        /// <summary>
        ///         Reference to the <see cref="StardewValley.GameData.Crops.CropData"/> object 
        /// <br/>   The metadata for a crop that can be planted.
        /// </summary>
        public StardewValley.GameData.Crops.CropData CropData { get; }

        /// <summary>
        ///         The id of the seed.
        /// </summary>
        public string SeedId { get; }

        /// <summary>
        ///         No reference list with seasonal data.         
        /// </summary>
        public List<Season> Seasons { get; }

        /// <summary>
        ///         The number of days before the crop regrows after harvesting, or -1 if it can't.
        /// </summary>
        public int RegrowDays { get; }

        public CropDataInfo(CropInfo cropInfo)
        {
            CropData = cropInfo.CropData;
            SeedId = cropInfo.SeedId;
            Seasons = new List<Season>(cropInfo.CropData.Seasons);
            RegrowDays = cropInfo.CropData.RegrowDays;
        }
    }
}
