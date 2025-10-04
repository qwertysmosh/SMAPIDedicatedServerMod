using DedicatedServer.HostAutomatorStages.BehaviorStates;
using StardewValley;
using System;
using System.Collections.Generic;

namespace DedicatedServer.HostAutomatorStages
{
    /// <summary>
    ///         This class skips all events.
    /// <br/>   If skipping is not desired, this must be entered in the dictionary <see cref="SkipEventsBehaviorLink.Functions"/>.
    /// <br/>   The events must then be handled elsewhere, e.g. <see cref="ProcessDialogueBehaviorLink.Process()"/>.
    /// </summary>
    internal class SkipEventsBehaviorLink : BehaviorLink
    {
        #region Required in derived class

        public override int WaitTimeAutoLoad { get; set; } = 60;
        public override int WaitTime { get; set; }

        public override void Process()
        {
            if (null != Game1.CurrentEvent && Game1.CurrentEvent.skippable)
            {
                if (lastEvent != Game1.CurrentEvent)
                {
                    lastEvent = Game1.CurrentEvent;

                    if (Functions.TryGetValue(Game1.CurrentEvent.id, out var ShouldTheEventBeSkipped))
                    {
                        if (false == ShouldTheEventBeSkipped())
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
            else
            {
                lastEvent = null;
            }
        }

        #endregion

        private Event lastEvent = null;

        /// <summary>
        ///         The return value of the dictionary values Func:
        /// <br/>   false: The event is skipped.
        /// <br/>   true:  The event is not skipped.
        /// </summary>
        private static Dictionary<string, Func<bool>> Functions = new Dictionary<string, Func<bool>>
            {
                { "897405", TheEventShouldBeTriggered }, // Marnie asks the farmer if they'd like to adopt a dog. 
                { "1590166", TheEventShouldBeTriggered }, // Marnie asks the farmer if they'd like to adopt a cat. 
            };

        /// <summary>
        /// The event will be skipped
        /// </summary>
        /// <returns></returns>
        private static bool TheEventShouldBeSkipped() => true;

        /// <summary>
        /// Nothing should be done except to proceed to the next process
        /// </summary>
        /// <returns></returns>
        private static bool TheEventShouldBeTriggered() => false;
    }
}
