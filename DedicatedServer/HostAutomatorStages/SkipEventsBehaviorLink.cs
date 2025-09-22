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
        private readonly Dictionary<string, Func<bool>> Functions;

        public SkipEventsBehaviorLink(BehaviorLink next = null) : base(next)
        {
            Functions = new Dictionary<string, Func<bool>>
            {
                { "897405", Event897405 },
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

        private bool Event897405()
        {
            // Nothing should be done except to proceed to the next process
            return true;
        }
    }
}
