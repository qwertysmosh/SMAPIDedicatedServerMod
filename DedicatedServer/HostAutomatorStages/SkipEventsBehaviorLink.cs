using DedicatedServer.HostAutomatorStages.BehaviorStates;
using StardewValley;
using System;
using System.Collections.Generic;

namespace DedicatedServer.HostAutomatorStages
{
    internal class SkipEventsBehaviorLink : BehaviorLink2
    {
        #region Required in derived class

        public override int WaitTimeAutoLoad { get; set; } = 0;
        public override int WaitTime { get; set; }

        public override void Process()
        {
            if (Game1.CurrentEvent != null && Game1.CurrentEvent.skippable)
            {
                if (Functions.TryGetValue(Game1.CurrentEvent.id, out var ChecksIfEventShouldBeSkipped))
                {
                    if (false == ChecksIfEventShouldBeSkipped())
                    {
                        return;
                    }
                }

                Game1.CurrentEvent.skipEvent();

                // Set up wait ticks to wait before trying to skip event again,
                // and wait to anticipate another following event
                WaitTime = (int)(600 * 0.2);
                
            }
        }

        #endregion

        /// <summary>
        ///         The return value of the dictionary values Func:
        /// <br/>   false The event is skipped.
        /// <br/>   true The event is not skipped.
        /// </summary>
        private static Dictionary<string, Func<bool>> Functions = new Dictionary<string, Func<bool>>
            {
                { "897405", Event897405 },
                { "1590166", Event1590166 },
            };

        /// <summary>
        /// Marnie asks the farmer if they'd like to adopt a dog. 
        /// </summary>
        /// <returns></returns>
        private static bool Event897405()
        {
            // Nothing should be done except to proceed to the next process
            return false;
        }

        /// <summary>
        /// Marnie asks the farmer if they'd like to adopt a cat. 
        /// </summary>
        /// <returns></returns>
        private static bool Event1590166()
        {
            // Nothing should be done except to proceed to the next process
            return false;
        }
    }
}
