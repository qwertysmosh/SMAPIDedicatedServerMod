using System;
using System.ComponentModel;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace DedicatedServer.HostAutomatorStages
{
    internal class ProcessPauseBehaviorLink : BehaviorLink
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

        private static IModHelper helper = null;

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

        public ProcessPauseBehaviorLink(IModHelper helper, BehaviorLink next = null) : base(next)
        {
            ProcessPauseBehaviorLink.helper = helper;
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
        public override void Process(BehaviorState state)
        {
            switch (internalState)
            {
                case internalStates.DisablePause:
                    IsPaused = false;
                    internalState = internalStates.WaitingForPlayersToLeave;
                    return;

                case internalStates.WaitingForPlayersToLeave:
                    if (  0   == state.GetNumOtherPlayers() && // If no other player is online
                        false == Game1.isFestival()         )  // if it is not a festival
                    {
                        internalState = internalStates.EnablePause;
                        return;
                    }

                    if (enableHostAutomation)
                    {
                        processNext(state);
                    }
                    return;

                case internalStates.EnablePause:
                    IsPaused = true;
                    internalState = internalStates.WaitingForUpcomingPlayers;
                    return;

                case internalStates.WaitingForUpcomingPlayers:

                    if (false == IsPaused)
                    {
                        IsPaused = true;
                    }

                    if (  0  <  state.GetNumOtherPlayers() ||
                        true == Game1.isFestival()         )
                    {
                        internalState = internalStates.DisablePause;
                        return;
                    }
                    return;

                // A reset must be carried out to exit the states, <see cref="Reset"/>.

                case internalStates.PreventPause:
                    if (IsPaused)
                    {
                        IsPaused = false;
                    }
                    if (enableHostAutomation)
                    {
                        processNext(state);
                    }
                    return;

                case internalStates.PauseDisabled:
                    if (enableHostAutomation)
                    {
                        processNext(state);
                    }
                    return;
            }
        }

        private static void AddOnDayStarted(EventHandler<DayStartedEventArgs> handler)
        {
            if (false == addedHandler)
            {
                addedHandler = true;
                helper.Events.GameLoop.DayStarted += handler;
            }
        }

        private static void RemoveOnDayStarted(EventHandler<DayStartedEventArgs> handler)
        {
            addedHandler = false;
            helper.Events.GameLoop.DayStarted -= handler;
        }

        private static void OnDayStartedWorker(object sender, DayStartedEventArgs e)
        {
            Reset();
        }
    }
}
