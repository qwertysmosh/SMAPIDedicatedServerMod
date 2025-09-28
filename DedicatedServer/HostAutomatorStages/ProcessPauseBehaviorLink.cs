using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.ComponentModel;

namespace DedicatedServer.HostAutomatorStages
{
    internal class ProcessPauseBehaviorLink
    {
        [DefaultValue(DisablePause)]
        private enum internalStates
        {
            /// <summary> Transitional action, disables pause </summary>
            DisablePause = 0,

            /// <summary> The server runs as long as players are online </summary>
            WaitingForPlayersToLeave,

            /// <summary> Transitional action, enables pause </summary>
            EnablePause,

            /// <summary> The server is paused as long as no players are online </summary>
            WaitingForUpcomingPlayers,


            /// <summary> Prevents the pause state </summary>
            PreventPause,

            /// <summary> Disables the pause once </summary>
            PauseDisabled,
        }

        private static bool addedHandler = false;

        private static internalStates internalState = default;

        /// <summary>
        ///         Event so that others can reset their own state.
        /// </summary>
        protected static event EventHandler ResetAction = null;

        /// <summary>
        ///         This allows you to deactivate the execution of host automation. Default is true
        /// <br/>   
        /// <br/>   true : The normal behavior of this mod is running
        /// <br/>   false: Keeps the behavior of the mod as it would be in the paused state
        /// <br/>   
        /// <br/>   Works only if <see cref="AutomatedHost"/> ticks and the <see cref="BehaviorChain"/> is executed.
        /// </summary>
        protected static bool enableHostAutomation = true;

        /// <summary>
        /// Alias for <c>Game1.netWorldState.Value.IsPaused</c>
        /// </summary>
        private static bool IsPaused
        {
            set { Game1.netWorldState.Value.IsPaused = value; }
            get { return Game1.netWorldState.Value.IsPaused; }
        }

        /// <summary>
        /// Prevents the pause state.
        /// </summary>
        protected static void PreventPauseUntilNextDay()
        {
            internalState = internalStates.PreventPause;
            AddOnDayStarted(OnDayStartedWorker);
        }

        /// <summary>
        /// Disables the pause once.
        /// </summary>
        protected static void PauseDisabledUntilNextDay()
        {
            internalState = internalStates.PauseDisabled;
            IsPaused = false;
            AddOnDayStarted(OnDayStartedWorker);
        }

        /// <summary>
        ///         Resets this class to its initial state
        /// </summary>
        protected static void Reset()
        {
            internalState = default;
            enableHostAutomation = true;
            RemoveOnDayStarted(OnDayStartedWorker);

            OnResetAction();
        }

        protected static void OnResetAction()
        {
            ResetAction?.Invoke(null, EventArgs.Empty);
        }

        // <![CDATA[
        //    stateDiagram-v2
        //
        //    [*] --> init
        //
        //    init : *
        //    states --> init : Init()
        //    states --> init : ⚡newDay
        //
        //    init --> states
        //
        //    states : ProcessPauseBehaviorLink
        //    state states {
        //        [*] --> core
        //        PreventPause : <center>PreventPause\n----\nIsPause = false\nprocessNext()\n⚡ += newDay</center>
        //        core --> PreventPause : PreventPauseUntilNextDay()
        //
        //
        //        PauseDisabled : <center>PauseDisabled\n----\nprocessNext()\n⚡ += newDay</center>
        //        core --> PauseDisabled : PauseDisabledUntilNextDay()
        //    }
        //
        //    core : core
        //    state core {
        //        [*] --> DisablePause
        //        DisablePause --> WaitingForPlayersToLeave
        //        WaitingForPlayersToLeave --> EnablePause : 1 > player
        //        EnablePause --> WaitingForUpcomingPlayers
        //        WaitingForUpcomingPlayers --> DisablePause : 0 < player
        //
        //        WaitingForUpcomingPlayers : <center>WaitingForUpcomingPlayers\n----\nprocessNext()</center>
        //    }
        // ]]>
        public static bool ShouldPause()
        {
            switch (internalState)
            {
                case internalStates.DisablePause:
                    IsPaused = false;
                    internalState = internalStates.WaitingForPlayersToLeave;
                    break;

                case internalStates.WaitingForPlayersToLeave:
#warning (Do I need to use GetNumOtherPlayers)
                    if (0 == DedicatedServer.GetNumOtherPlayers() && // If no other player is online
                        false == Game1.isFestival() // if it is not a festival
                    )
                    {
                        internalState = internalStates.EnablePause;
                    }
                    break;

                case internalStates.EnablePause:
                    IsPaused = true;
                    internalState = internalStates.WaitingForUpcomingPlayers;
                    break;

                case internalStates.WaitingForUpcomingPlayers:
                    if (false == IsPaused)
                    {
                        IsPaused = true;
                    }

                    if (0 < DedicatedServer.GetNumOtherPlayers() ||
                        true == Game1.isFestival()
                    )
                    {
                        internalState = internalStates.DisablePause;
                    }
                    else
                    {
                        // do not proceed, shouldPause
                        return true;
                    }
                    break;

                // A reset must be carried out to exit the states, <see cref="Reset"/>.

                case internalStates.PreventPause:
                    if (IsPaused)
                    {
                        IsPaused = false;
                    }
                    break;

                case internalStates.PauseDisabled:
                    break;
            }

            if (enableHostAutomation)
            {
                // shouldPause
                return false;
            }
            else
            {
                // shouldPause
                return true;
            }
        }

        private static void AddOnDayStarted(EventHandler<DayStartedEventArgs> handler)
        {
            if (false == addedHandler)
            {
                addedHandler = true;
                DedicatedServer.helper.Events.GameLoop.DayStarted += handler;
            }
        }

        private static void RemoveOnDayStarted(EventHandler<DayStartedEventArgs> handler)
        {
            addedHandler = false;
            DedicatedServer.helper.Events.GameLoop.DayStarted -= handler;
        }

        private static void OnDayStartedWorker(object sender, DayStartedEventArgs e)
        {
            Reset();
        }
    }
}
