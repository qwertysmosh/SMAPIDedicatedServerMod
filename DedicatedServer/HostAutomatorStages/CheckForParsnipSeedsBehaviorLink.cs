using DedicatedServer.HostAutomatorStages.BehaviorStates;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;

namespace DedicatedServer.HostAutomatorStages
{
    internal class CheckForParsnipSeedsBehaviorLink : BehaviorLink2
    {
        #region Required in derived class

        public override int WaitTimeAutoLoad { get; set; } = 0;
        public override int WaitTime { get; set; }

        public override void Process()
        {
            if (false == hasCheckedForParsnipSeeds && Game1.currentLocation is FarmHouse fh)
            {
                hasCheckedForParsnipSeeds = true;
                foreach (var kvp in fh.Objects.Pairs)
                {
                    var obj = kvp.Value;
                    if (obj is Chest chest)
                    {
                        if (chest.giftbox.Value)
                        {
                            chest.checkForAction(Game1.player);

#warning The following was a globla wait tick
                            // state.SetWaitTicks(60 * 2);
                            break;
                        }
                    }
                }
            }
        }

        #endregion

        private bool hasCheckedForParsnipSeeds = false;
    }
}
