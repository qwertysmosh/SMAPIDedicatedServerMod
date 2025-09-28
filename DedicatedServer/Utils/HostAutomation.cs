using System;
using StardewValley;
using DedicatedServer.HostAutomatorStages;

namespace DedicatedServer.Utils
{
    internal abstract class HostAutomation : ProcessPauseBehaviorLink
    {
        public static new event EventHandler ResetAction
        {
            add { ProcessPauseBehaviorLink.ResetAction += value; }
            remove { ProcessPauseBehaviorLink.ResetAction -= value; }
        }

        /// <summary>
        /// Alias for <c>Game1.netWorldState.Value.IsPaused</c>
        /// </summary>
        public static bool IsPaused
        {
            get { return Game1.netWorldState.Value.IsPaused; }
            set { Game1.netWorldState.Value.IsPaused = value; }
        }

        /// <summary>
        /// <inheritdoc cref = "ProcessPauseBehaviorLink.PreventPauseUntilNextDay"/>
        /// </summary>
        public static new void PreventPauseUntilNextDay()
        {
            ProcessPauseBehaviorLink.PreventPauseUntilNextDay();
        }

        /// <summary>
        /// <inheritdoc cref = "ProcessPauseBehaviorLink.PauseDisabledUntilNextDay"/>
        /// </summary>
        public static new void PauseDisabledUntilNextDay()
        {
            ProcessPauseBehaviorLink.PauseDisabledUntilNextDay();
        }

        /// <summary>
        /// <inheritdoc cref = "ProcessPauseBehaviorLink.Reset"/>
        /// </summary>
        public static new void Reset()
        {
            ProcessPauseBehaviorLink.Reset();
        }

        /// <summary>
        /// <inheritdoc cref = "ProcessPauseBehaviorLink.enableHostAutomation"/>
        /// </summary>
        public static bool EnableHostAutomation
        {
            get { return ProcessPauseBehaviorLink.enableHostAutomation; }
            set { ProcessPauseBehaviorLink.enableHostAutomation = value; }
        }

        /// <summary>
        ///         Lets the player take over the host.
        /// <br/>   
        /// <br/>   - All host functions are switched off.
        /// <br/>   - Deactivates the Sleep command.
        /// </summary>
        public static void LetMePlay()
        {
            EnableHostAutomation = false;
            PauseDisabledUntilNextDay();

            Invincible.InvincibilityOverwrite = false;
            Sleeping.ShouldSleepOverwrite = false;
            Invisible.InvisibleOverwrite = false;
            Invisible.SetVisibleDisplayOnChanges();
        }
    }
}
