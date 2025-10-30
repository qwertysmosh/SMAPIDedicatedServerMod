using StardewModdingAPI;
using StardewModdingAPI.Events;
using System;
using System.Collections.Generic;

namespace DedicatedServer.HostAutomatorStages.BehaviorStates
{
    internal static class BehaviorChain
    {
        private static void DebugLog(string message, LogLevel level)
        {
            var timestamp = DateTime.Now.ToString("MM-dd-yyyy HH:mm:ss");
            DedicatedServer.monitor.Log($"[{timestamp}] {message}", level);
        }

        private const int waitTimeStartOfDay = 60;

        public static int WaitTimeAutoLoad { get; set; } = 0;

        /// <summary>
        ///         This is a global wait Time
        /// <br/>   If the time is set, the current behavior chain is canceled</summary>
        public static int WaitTime { get; set; } = WaitTimeAutoLoad;

        private static List<BehaviorLink> chain;

        private static Func<bool> shouldPause;

        public static void InitStaticVariables(Func<bool> shouldPause, List<BehaviorLink> chain
        ){
            BehaviorChain.chain = (null == chain) ? new List<BehaviorLink>() : chain;
            BehaviorChain.shouldPause = (null == shouldPause) ? Empty : shouldPause;
        }

        public static void Enable()
        {
            MainController.helper.Events.GameLoop.UpdateTicked += Execute;
            MainController.helper.Events.GameLoop.DayStarted += OnDayStartedWorker;
        }

        public static void Disable()
        {
            MainController.helper.Events.GameLoop.UpdateTicked -= Execute;
            MainController.helper.Events.GameLoop.DayStarted -= OnDayStartedWorker;
        }

        private static void Execute(object sender, UpdateTickedEventArgs e)
        {
            try
            {
                if (0 < WaitTime)
                {
                    WaitTime--;
                }
                else if (0 >= WaitTime)
                {
                    if (false == shouldPause.Invoke())
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

                                if (0 != WaitTime)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                DebugLog(
                    $"Error in {typeof(BehaviorChain).Name} class:\n\n{exception}",
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
