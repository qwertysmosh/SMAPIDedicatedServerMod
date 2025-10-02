using DedicatedServer.HostAutomatorStages;

namespace DedicatedServer.Utils
{
    internal abstract class Sleeping : SleepWorker
    {
        /// <summary>
        /// <inheritdoc cref = "SleepWorker.ShouldSleepOverwrite"/>
        /// </summary>
        public static new bool ShouldSleepOverwrite
        {
            get { return SleepWorker.ShouldSleepOverwrite; }
            set { SleepWorker.ShouldSleepOverwrite = value; }
        }

    }
}
