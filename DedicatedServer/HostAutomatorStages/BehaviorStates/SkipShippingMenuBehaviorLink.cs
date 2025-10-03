using DedicatedServer.HostAutomatorStages.BehaviorStates;
using DedicatedServer.Utils;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Threading.Tasks;

namespace DedicatedServer.HostAutomatorStages
{
    internal class SkipShippingMenuBehaviorLink : BehaviorLink
    {
        #region Required in derived class

        public override int WaitTimeAutoLoad { get; set; } = 60;
        public override int WaitTime { get; set; }

        public override void Process()
        {
            if (Game1.activeClickableMenu is ShippingMenu shippingMenu)
            {
                if (shippingMenu.CanReceiveInput())
                {
                    var point = shippingMenu.okButton.bounds.Center;
                    shippingMenu.receiveLeftClick(point.X, point.Y, true);
                }
            }
        }
        #endregion

        #region For external control

        public static bool SkipShippingMenu()
        {
            if (Game1.activeClickableMenu is ShippingMenu shippingMenu)
            {
                if (shippingMenu.CanReceiveInput())
                {
                    var point = shippingMenu.okButton.bounds.Center;
                    shippingMenu.receiveLeftClick(point.X, point.Y, true);

                    return true;
                }
            }

            return false;
        }

        #endregion

        public SkipShippingMenuBehaviorLink() =>Enable();

        ~SkipShippingMenuBehaviorLink() => Dispose();

        public static void Dispose() => Disable();

        private static void Enable()
        {
            DedicatedServer.helper.Events.GameLoop.DayEnding += OnDayEnding;
            DedicatedServer.helper.Events.GameLoop.DayStarted += OnDayStarted;
        }

        private static void Disable()
        {
            DedicatedServer.helper.Events.GameLoop.DayEnding -= OnDayEnding;
            DedicatedServer.helper.Events.GameLoop.DayStarted -= OnDayStarted;
        }

        #region Additional Timer

        // The problem is that the normal update timer appears to be disabled after a certain amount of time.
        // In this case, this timer and the event take over.

        private class WhileDayEndingEventArgs : EventArgs
        {
            public int Seconds { get; set; }
            public WhileDayEndingEventArgs(int seconds) => Seconds = seconds;
        }

        private static event EventHandler<WhileDayEndingEventArgs> WhileDayEnding = Skip;

        private static void OnWhileDayEnding(int seconds)
        {
            WhileDayEnding?.Invoke(null, new WhileDayEndingEventArgs(seconds));
        }

        private static bool shouldRunning = false;

        private static void Skip(object sender, WhileDayEndingEventArgs e)
        {
            DedicatedServer.chatBox.textBoxEnter($"The shipping menu has not responded for {e.Seconds} seconds." + TextColor.Red);
            SkipShippingMenu();
        }

        private static void OnDayEnding(object sender, DayEndingEventArgs e)
        {
            shouldRunning = true;

            Task.Run(async () =>
            {
                // Wait to send an error message until all players have clicked "OK" and see the ready to save dialog.
                // When sleeping, `ready_for_save` is initially set to true, so the wait loop must be performed step by step.

                while (Game1.activeClickableMenu is not ShippingMenu)
                {   // Waiting for shipping menu
                    await Task.Delay(100);
                    if (false == shouldRunning) { return; }
                }

                var shippingMenu = Game1.activeClickableMenu as ShippingMenu ;

                while (false == shippingMenu.CanReceiveInput())
                {   //  Wait for the "OK" button in the shipping menu
                    await Task.Delay(100);
                    if (false == shouldRunning) { return; }
                }

                var farmers = DedicatedServer.OnlineFarmers();
                while (false == DedicatedServer.IsReadyPlayers("ready_for_save", farmers))
                {   // Wait until all other players have clicked "OK". 
                    await Task.Delay(100);
                    if (false == shouldRunning) { return; }
                }

                int seconds = 10;
                await Task.Delay(10000);
                if (false == shouldRunning) { return; }
                OnWhileDayEnding(seconds);

                seconds += 5;
                await Task.Delay(5000);
                if (false == shouldRunning) { return; }
                OnWhileDayEnding(seconds);

                seconds += 2;
                await Task.Delay(2000);
                if (false == shouldRunning) { return; }
                OnWhileDayEnding(seconds);

                seconds += 1;
                await Task.Delay(1000);
                while (shouldRunning)
                {
                    OnWhileDayEnding(seconds);
                    seconds += 1;
                    await Task.Delay(1000);
                }
            });
        }

        private static void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            shouldRunning = false;
        }

        #endregion

#if false // Outdated solution
        private static readonly FieldInfo introTimerField = typeof(ShippingMenu).GetField("introTimer", BindingFlags.Instance | BindingFlags.NonPublic);

        /// <summary>
        /// Provides access to the internal timer; it is better to use the correct method:
        /// <see cref="ShippingMenu.CanReceiveInput()"/>
        /// </summary>
        /// <param name="shippingMenu"></param>
        /// <returns></returns>
        private static int GetIntroTimer(ShippingMenu shippingMenu)
        {
            return (int)introTimerField.GetValue(shippingMenu);
        }
#endif

#if false // Outdated solution
        private static MethodInfo okClickedMethod = typeof(ShippingMenu).GetMethod("okClicked", BindingFlags.Instance | BindingFlags.NonPublic);

        /// <summary>
        ///         This is the internal method; it causes minor stuttering when called frequently.
        /// <br/>   Call it with okClicked(shippingMenu);
        /// </summary>
        /// <param name="shippingMenu"></param>
        private static void okClicked(ShippingMenu shippingMenu) => okClickedMethod.Invoke(shippingMenu, Array.Empty<object>());
#endif

    }
}
