using DedicatedServer.HostAutomatorStages.BehaviorStates;

namespace DedicatedServer.HostAutomatorStages
{
    internal class UpdateStateBehaviorLink : BehaviorLink2
    {
        #region Required in derived class

        public override int WaitTimeAutoLoad { get; set; } = 0;
        public override int WaitTime { get; set; }

        public override void Process()
        {
            DedicatedServer.UpdateOtherPlayers();
        }

        #endregion

        
    }
}
