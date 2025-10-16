using StardewValley;
using StardewValley.GameData.Crops;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;
using System.Collections.Generic;
using System.Linq;

namespace DedicatedServer.Crops
{
    /// <summary>
    /// The plant goes through the phases of the array `Crop.GetDays().DaysInPhase` until it is mature.
    /// In each phase `Crop.currentPhase`, the plant remains for as long as `crop.dayOfCurrentPhase` is the value of the array.
    /// </summary>
    internal class CropInfo
    {
        /// <summary>
        ///         Reference to the <see cref="StardewValley.Crop"/> object
        /// </summary>
        public Crop Crop { get; }

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
        ///         The id of the harvest item.
        /// </summary>
        public string HarvestId { get; }

        /// <summary>
        ///         The translated display name for the seed, including metadata like the "(Recipe)" suffix.
        /// </summary>
        public string SeedName { get; }

        /// <summary>
        ///         The translated display name for the item, including metadata like the "(Recipe)" suffix.
        /// </summary>
        public string HarvestName { get; }

        /// <summary>
        ///         The number of days it takes after planting for the plant to reach maturity.
        /// </summary>
        public int FullDaysToMaturity { get; }

        /// <summary>
        ///         The number of days before the crop regrows after harvesting, or -1 if it can't.
        /// </summary>
        public int RegrowDays { get; }

        /// <summary>
        ///         Whether this is a raised crop on a trellis that can't be walked through.
        /// </summary>
        public bool RaisedSeeds { get; }

        /// <summary>
        ///         Check whether the plant has died
        /// </summary>
        public bool IsDead { get => Crop.dead.Value; }

        /// <summary>
        ///         Check whether the plant is fully grown.
        /// </summary>
        public bool IsPlantFullyGrown
        {
            get
            {
                // `Crop.currentPhase`: The index of the array `Crop.phaseDays`, which has one more element than the array `Crop.GetDays().DaysInPhase`, namely the value `Crop.finalPhaseLength`.
                // `Crop.GetDays().DaysInPhase`: Array whose number of elements represents the number of phases. Each element stores the number of days in the respective phase.
                // Since the index `Crop.currentPhase` can count one more element than the array `Crop.GetDays().DaysInPhase` has, the index can be compared with the number of elements.
                return Crop.currentPhase.Value >= CropData.DaysInPhase.Count;
            }
        }

        /// <summary>
        ///         Checks whether a fruit is currently hanging and can be picked.
        /// </summary>
        public bool IsFruitAvailable
        {
            get
            {
                if (-1 == RegrowDays)
                {
                    return IsPlantFullyGrown;
                }
                else
                {
                    if (false == this.Crop.fullyGrown.Value)
                    {
                        return IsPlantFullyGrown;
                    }
                    else
                    {
                        // `Crop.dayOfCurrentPhase` counts down when `Crop.safeFullyGrown` is true.
                        // If it has a value of 0, then it has fruit.
                        // It is reset to `Crop.GetDays().RegrowDays` when harvested.
#warning TODO: The description is wrong I net to change the condition to >= !!!
                        return 0 >= this.Crop.dayOfCurrentPhase.Value;
                    }
                }
            }
        }

        public CropInfo(Crop crop, StardewValley.GameData.Crops.CropData cropData = null)
        {
            Crop = crop;
            CropData = cropData ?? crop.GetData();

            if (null == CropData)
            {
                throw new System.Exception("The 'GetData()' methode of crop returned null.");
            }

            FullDaysToMaturity = CropData.DaysInPhase.Sum();
            RegrowDays = CropData.RegrowDays;
            SeedId = Crop.netSeedIndex.Value;
            HarvestId = CropData.HarvestItemId;
            SeedName = new StardewValley.Object(SeedId, 0).DisplayName;
            HarvestName = new StardewValley.Object(HarvestId, 0).DisplayName;
            RaisedSeeds = Crop.raisedSeeds.Value;

        }

        public int GetMaturityDays()
        {
            if (false == IsFruitAvailable)
            {
                return -1;
            }

            return this.Crop.dayOfCurrentPhase.Value;
        }

        /// <summary>
        ///         Indicates the current growth day of the plant until a fruit hangs.
        /// <br/>   If the number matches <see cref="FullDaysToMaturity"/>, then a fruit hangs.
        /// <br/>   This method takes into account when a plant has been picked and can be harvested again.
        /// </summary>
        /// <returns>Current growth day</returns>
        public int GetDays()
        {
            if (Crop.fullyGrown.Value)
            {
                return FullDaysToMaturity - this.Crop.dayOfCurrentPhase.Value;
            }
            else
            {
                var daysStage = 0;
                if (false == IsPlantFullyGrown)
                {
                    for (int i = 0; i < Crop.currentPhase.Value; i++)
                    {
                        daysStage += CropData.DaysInPhase[i];
                    }

                    // The description is only correct as long as `Crop.safeFullyGrown` is false.
                    // This is the current day in the individual phase. This number is smaller than
                    // the number in the array `Crop.GetDays().DaysInPhase`, because reaching the
                    // number means reaching the next phase.
                    daysStage += Crop.dayOfCurrentPhase.Value;

                    return daysStage;
                }

                return FullDaysToMaturity;
            }
        }

        public static List<CropInfo> GetAllCropInfo()
        {
            var list = new List<CropInfo>();
            foreach (var location in Game1.locations)
            {
                if (location.IsOutdoors && !location.SeedsIgnoreSeasonsHere() && location is not IslandLocation)
                {
                    // Found an outdoor location where seeds don't ignore seasons. Find all the
                    // crops here to cache necessary data for protecting them.
                    foreach (var pair in location.terrainFeatures.Pairs)
                    {
                        var tileLocation = pair.Key;
                        var terrainFeature = pair.Value;
                        if (terrainFeature is HoeDirt hoeDirt)
                        {
                            var crop = hoeDirt.crop;
                            if (null != crop)
                            {
                                var cropData = crop.GetData();
                                if (null != cropData)
                                {
                                    var cropInfo = new CropInfo(crop, cropData);
                                    list.Add(cropInfo);
                                }
                            }
                        }
                    }
                }
            }
            return list;
        }
    }
}
