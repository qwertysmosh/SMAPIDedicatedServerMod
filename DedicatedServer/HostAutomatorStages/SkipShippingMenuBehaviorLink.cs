using DedicatedServer.HostAutomatorStages.BehaviorStates;
using Force.DeepCloner;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Reflection;

namespace DedicatedServer.HostAutomatorStages
{
    internal class SkipShippingMenuBehaviorLink : BehaviorLink2
    {
        #region Required in derived class

        public override int WaitTimeAutoLoad { get; set; } = 60;
        public override int WaitTime { get; set; }

        public override void Process()
        {
            if (Game1.activeClickableMenu is ShippingMenu shippingMenu)
            {
               
                var clickables = shippingMenu.allClickableComponents;
                if (null == copy) { copy = DeepClonerExtensions.DeepClone(clickables[6]); }
#warning I need to find a way so that the click is performed after the button is visible
                //if (clickables[6].visible)
                //{
                var a = false;
                if (a)
                {
                    if (runOncePerMenu)
                    {
                        runOncePerMenu = false;
                        okClicked(shippingMenu);
                    }
                }
                //}
            }
            else
            {
                runOncePerMenu = true;
                //WaitTime = 60;
            }
        }

        #endregion

        private static MethodInfo info = typeof(ShippingMenu).GetMethod("okClicked", BindingFlags.Instance | BindingFlags.NonPublic);

        private static void okClicked(ShippingMenu shippingMenu) => info.Invoke(shippingMenu, Array.Empty<object>());

        bool runOncePerMenu = true;

        public SkipShippingMenuBehaviorLink()
        {
            DedicatedServer.helper.Events.GameLoop.DayEnding += OnDayEndingWorker;
        }

#warning TEST
        ClickableComponent copy = null;
        
        public void OnDayEndingWorker(object sender, DayEndingEventArgs e)
        {
            DedicatedServer.chatBox.textBoxEnter("Shipping menu Workaroud, if the host does not click Ok, enter 'okay'");
        }

        public static bool SkipShippingMenu()
        {
            if (Game1.activeClickableMenu is ShippingMenu shippingMenu)
            {
                okClicked(shippingMenu);
                DedicatedServer.monitor.Log("SkipShippingMenu-OkClicked", LogLevel.Debug);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
