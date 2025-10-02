using DedicatedServer.HostAutomatorStages.BehaviorStates;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;

namespace DedicatedServer.HostAutomatorStages
{
    internal class CheckForParsnipSeedsBehaviorLink : BehaviorLink
    {
        #region Required in derived class

        public override int WaitTimeAutoLoad { get; set; } = 0;
        public override int WaitTime { get; set; }

        public override void Process()
        {
            if (false == hasCheckedForParsnipSeeds &&
                Game1.currentLocation is FarmHouse farmHouse)
            {
                hasCheckedForParsnipSeeds = true;
                foreach (var kvp in farmHouse.Objects.Pairs)
                {
                    var obj = kvp.Value;
                    if (obj is Chest chest)
                    {
                        if (chest.giftbox.Value)
                        {
                            chest.checkForAction(Game1.player);

                            BehaviorChain.WaitTime = (60 * 2);

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
