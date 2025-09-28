using StardewModdingAPI;
using StardewModdingAPI.Events;
using System;
using System.Collections.Generic;

namespace DedicatedServer.HostAutomatorStages.BehaviorStates
{
    internal static class BehaviorChain2
    {
        private const int waitTimeStartOfDay = 60;

        public static int WaitTimeAutoLoad { get; set; } = 0;
        public static int WaitTime { get; set; } = WaitTimeAutoLoad;

        private static List<BehaviorLink2> chain;

        private static Func<bool> shouldPause;

        public static void InitStaticVariables(Func<bool> shouldPause, List<BehaviorLink2> chain
        ){
            BehaviorChain2.chain = (null == chain) ? new List<BehaviorLink2>() : chain;
            BehaviorChain2.shouldPause = (null == shouldPause) ? Empty : shouldPause;
        }

        public static void Enable()
        {
            DedicatedServer.helper.Events.GameLoop.UpdateTicked += Execute;
            DedicatedServer.helper.Events.GameLoop.DayStarted += OnDayStartedWorker;
        }

        public static void Disable()
        {
            DedicatedServer.helper.Events.GameLoop.UpdateTicked -= Execute;
            DedicatedServer.helper.Events.GameLoop.DayStarted -= OnDayStartedWorker;
        }
        private static void Execute(object sender, UpdateTickedEventArgs e)
        {
            try
            {
                if (false == shouldPause.Invoke())
                {
                    if (0 < WaitTime)
                    {
                        WaitTime--;
                    }
                    else if (0 == WaitTime)
                    {
                        WaitTimeReset();

                        foreach (var chainLink in chain)
                        {
                            if (0 < chainLink.WaitTime)
                            {
                                chainLink.WaitTime--;
                            }
                            else if (0 == chainLink.WaitTime)
                            {
                                chainLink.WaitTimeReset();
                                chainLink.Process();
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                DedicatedServer.monitor.Log(
                    $"Error in {typeof(BehaviorChain2).Name} class:\n\n{exception}",
                    LogLevel.Error);
            }
        }

        private static bool Empty() { return false; }

        private static void WaitTimeReset()
        {
            WaitTime = WaitTimeAutoLoad;
        }

        private static void OnDayStartedWorker(object sender, DayStartedEventArgs e)
        {
            WaitTime = waitTimeStartOfDay;
        }
    }
}
