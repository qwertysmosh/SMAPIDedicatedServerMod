using DedicatedServer.Chat;
using DedicatedServer.Utils;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DedicatedServer.HostAutomatorStages
{
    internal class SkipShippingMenuBehaviorLink : BehaviorLink
    {
        private static MethodInfo info = typeof(ShippingMenu).GetMethod("okClicked", BindingFlags.Instance | BindingFlags.NonPublic);
        
        private static IMonitor monitor;
        private static EventDrivenChatBox chatBox;

        public SkipShippingMenuBehaviorLink(IMonitor monitor, EventDrivenChatBox chatBox, BehaviorLink next = null) : base(next)
        {
            SkipShippingMenuBehaviorLink.monitor = monitor;
            SkipShippingMenuBehaviorLink.chatBox = chatBox;
        }

        public override void Process(BehaviorState state)
        {
            string buf = "";

            if (Game1.activeClickableMenu is ShippingMenu sm)
            {
                buf = "SkipShippingMenuBehaviorLink";
                monitor.Log(buf, LogLevel.Debug);
                chatBox?.textBoxEnter(buf + TextColor.Purple);
                if (state.HasBetweenShippingMenusWaitTicks())
                {
                    buf = "SkipShippingMenuBehaviorLink-Wait";
                    monitor.Log(buf, LogLevel.Debug);
                    chatBox?.textBoxEnter(buf + TextColor.Purple);
                    state.DecrementBetweenShippingMenusWaitTicks();
                } else
                {
                    buf = "SkipShippingMenuBehaviorLink-OkClicked";
                    monitor.Log(buf, LogLevel.Debug);
                    chatBox?.textBoxEnter(buf + TextColor.Purple);
                    info.Invoke(sm, new object[]{});
                    state.SkipShippingMenu();
                }
            } else
            {
                state.ClearBetweenShippingMenusWaitTicks();
                processNext(state);
            }
        }
    }
}
