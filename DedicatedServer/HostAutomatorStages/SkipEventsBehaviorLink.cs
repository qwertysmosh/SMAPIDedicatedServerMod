using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DedicatedServer.HostAutomatorStages
{
    internal class SkipEventsBehaviorLink : BehaviorLink
    {
        /// <summary>
        ///         The return value false waits in the current process.
        /// <br/>   The return value true switches to the next process.
        /// </summary>
        private readonly Dictionary<string, Func<bool>> Functions;

        public SkipEventsBehaviorLink(BehaviorLink next = null) : base(next)
        {
            Functions = new Dictionary<string, Func<bool>>
            {
                { "897405", Event897405 },
                { "1590166", Event1590166 },      
            };
        }

        public override void Process(BehaviorState state)
        {
            if (Game1.CurrentEvent != null && Game1.CurrentEvent.skippable)
            {
                if (state.HasBetweenEventsWaitTicks())
                {
                    state.DecrementBetweenEventsWaitTicks();
                }
                else
                {
                    if (Functions.TryGetValue(Game1.CurrentEvent.id, out var action))
                    {
                        if(action())
                        {
                            state.ClearBetweenEventsWaitTicks();
                            processNext(state);
                        }
                    }
                    else
                    {
                        Game1.CurrentEvent.skipEvent();
                        state.SkipEvent(); // Set up wait ticks to wait before trying to skip event again, and wait to anticipate another following event
                    }
                }
            }
            else
            {
                state.ClearBetweenEventsWaitTicks();
                processNext(state);
            }
        }

        /// <summary>
        /// Marnie asks the farmer if they'd like to adopt a dog. 
        /// </summary>
        /// <returns></returns>
        private bool Event897405()
        {
            // Nothing should be done except to proceed to the next process
            return true;
        }

        /// <summary>
        /// Marnie asks the farmer if they'd like to adopt a cat. 
        /// </summary>
        /// <returns></returns>
        private bool Event1590166()
        {
            // Nothing should be done except to proceed to the next process
            return true;
        }
    }
}
