namespace DedicatedServer.HostAutomatorStages.BehaviorStates
{
    internal abstract class BehaviorLink
    {
        public abstract int WaitTimeAutoLoad { get; set; }
        /// <summary>
        ///          0: Called without waiting
        /// <br/>   +n: Waits n times before calling the function once.
        /// <br/>   -n: The function is never called.
        /// </summary>
        public abstract int WaitTime { get; set; }
        public abstract void Process();

        public void WaitTimeReset()
        {
            this.WaitTime = this.WaitTimeAutoLoad;
        }

        public BehaviorLink()
        {
            WaitTimeReset();
        }
    }
}
