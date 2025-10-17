using StardewValley;
using System;
using System.Collections.Generic;

namespace DedicatedServer.Crops
{
    /// <summary>
    ///         Plants that ripen on the first day are not saved.
    /// </summary>
    public abstract class CropSaver
    {
        private static readonly List<Season> AllSeasons = new List<Season>() { Season.Spring, Season.Summer, Season.Fall, Season.Winter };

        private static List<CropDataInfo> cropDataInfoList = null;

        private static bool enabled = false;

#if false

        public static void CropTest()
        {
            var list = CropInfo.GetAllCropInfo();

            foreach (var c in list)
            {
                MainController.chatBox.textBoxEnter(
                    $"n:{c.SeedName} is {(c.IsFruitAvailable ? "fruit" : "empty")}, " +
                    $"d:{c.GetDays()}/{c.FullDaysToMaturity}, " +
                    $"m:{c.GetMaturityDays()}, " +
                    $"Dead:{c.IsDead}, " +
                    $"s:{c.Crop.IsInSeason(c.Crop.currentLocation)}");
            }
        }

#endif

        public static bool Init()
        {
            if (MainController.config.EnableCropSaver)
            {
                Enable();
                return true;
            }

            return false;
        }

        public static void Enable()
        {
            if (false == enabled)
            {
                enabled = true;
                MainController.helper.Events.GameLoop.DayStarted += onDayStart;
                MainController.helper.Events.GameLoop.DayEnding += onDayEnd;
            }
        }

        public static void Disable()
        {
            enabled = false;
            MainController.helper.Events.GameLoop.DayStarted -= onDayStart;
            MainController.helper.Events.GameLoop.DayEnding -= onDayEnd;
        }

        private static List<CropDataInfo> GetCropDataTag(List<CropInfo> cropInfoList)
        {
            var cropDataTagList = new List<CropDataInfo>();
            foreach (var cropInfo in cropInfoList)
            {
                if (cropInfo.IsFruitAvailable &&
                    false == cropDataTagList.Exists(c => c.SeedId == cropInfo.SeedId)
                )
                {
                    cropDataTagList.Add(new CropDataInfo(cropInfo));
                }
            }
            return cropDataTagList;
        }

        private static void SetAllSeasons()
        {
            var cropInfoList = CropInfo.GetAllCropInfo();

            foreach (var crop in cropInfoList)
            {
                if (false == crop.IsFruitAvailable)
                {
                    crop.Crop.Kill();
                }
            }

            cropDataInfoList = GetCropDataTag(cropInfoList);

            foreach (var cropData in cropDataInfoList)
            {
                cropData.CropData.Seasons = AllSeasons;
                cropData.CropData.RegrowDays = -1;
#if false
                var id = "com.GitHub.Chris82111.DedicatedServer.PlantRuleOutOfSeason";

                var plantableRule = new PlantableRule();
                plantableRule.Id = id;
                plantableRule.Result = PlantableResult.Deny;
                plantableRule.DeniedMessage = "Out of season.";

                var rules = cropData.CropData.PlantableLocationRules;

                if (null == rules)
                {
                    rules = new List<PlantableRule>
                    {
                        plantableRule
                    };
                }
                else if (0 == rules.Count)
                {
                    rules.Add(plantableRule);
                }
                else
                {
                    var item = rules.FirstOrDefault(rule => rule.Id == id);

                    if (null != item)
                    {
                        rules.Remove(item);
                    }
                }
#endif
            }
        }

        private static void ResetSeason()
        {
            if (null == cropDataInfoList)
            {
                return;
            }

            foreach (var cropData in cropDataInfoList)
            {
                cropData.CropData.Seasons = cropData.Seasons;
                cropData.CropData.RegrowDays = cropData.RegrowDays;
            }

            cropDataInfoList = null;
        }

        private static void onDayEnd(object sender, EventArgs e)
        {
            if (28 == Game1.Date.DayOfMonth)
            {
                SetAllSeasons();
            }
        }

        private static void onDayStart(object sender, EventArgs e)
        {
            if (1 == Game1.Date.DayOfMonth)
            {
                ResetSeason();
            }
        }
    }
}
