using DedicatedServer.Utils;
using StardewValley;

namespace DedicatedServer.HostAutomatorStages
{
    internal class ReadyCheckHelper
    {
        public void Enable()
        {
            DedicatedServer.helper.Events.GameLoop.DayStarted += OnDayStarted;
        }

        public void Disable()
        {
            DedicatedServer.helper.Events.GameLoop.DayStarted -= OnDayStarted;
        }

        public void OnDayStarted(object sender, StardewModdingAPI.Events.DayStartedEventArgs e)
        {
            //Unlocks the sewer
            if (false == Game1.player.eventsSeen.Contains("295672") &&
                60 <= Game1.netWorldState.Value.MuseumPieces.Count()
            ){
                Game1.player.eventsSeen.Add("295672");
            }

            if (DedicatedServer.config.UpgradeHouseLevelBasedOnFarmhand)
            {
                HostHouseUpgrade.NeedsUpgrade();
            }
        }
    }
}
