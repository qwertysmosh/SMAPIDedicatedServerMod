using DedicatedServer.Config;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;

namespace DedicatedServer.Crops
{
    // Plants that ripen on the first day are not saved.
    public class CropSaver
    {
        private static readonly List<Season> AllSeasons = new List<Season>() { Season.Spring, Season.Summer, Season.Fall, Season.Winter };

#warning TODO: Crop saver is this necessary
        private IModHelper helper;
        private IMonitor monitor;
        private ModConfig config;

        private List<CropDataInfo> cropDataInfoList = null;

#warning TODO: Crop saver to static
        public CropSaver(IModHelper helper, IMonitor monitor, ModConfig config)
        {
            this.helper = helper;
            this.monitor = monitor;
            this.config = config;
        }

#warning TODO: Crop saver is this necessary
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

        public void Enable()
        {
            helper.Events.GameLoop.DayStarted += onDayStart;
            helper.Events.GameLoop.DayEnding += onDayEnd;
        }

        public void Disable()
        {
            helper.Events.GameLoop.DayStarted -= onDayStart;
            helper.Events.GameLoop.DayEnding -= onDayEnd;
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

        private void SetAllSeasons()
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

        private void ResetSeason()
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

        private void onDayEnd(object sender, EventArgs e)
        {
            if (28 == Game1.Date.DayOfMonth)
            {
                SetAllSeasons();
            }
        }

        private void onDayStart(object sender, EventArgs e)
        {
            if (1 == Game1.Date.DayOfMonth)
            {
                ResetSeason();
            }
        }
    }
}
