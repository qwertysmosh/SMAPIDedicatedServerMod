using DedicatedServer.Utils;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.ComponentModel;

namespace DedicatedServer.HostAutomatorStages
{
    /// <summary>
    /// Makes the host invincible
    /// </summary>
    internal class InvincibleWorker
    {
        /// <summary>
        ///         Overwrites the behavior of the class <see cref="InvincibleWorker"/>
        /// <br/>   
        /// <br/>   null : The behavior of this class is not changed.
        /// <br/>   true : Host is invincible
        /// <br/>   false: Host is not invincible
        /// <br/>   
        /// <br/>   Works only if <see cref="OnUpdateTicked"/> ticks
        /// </summary>
        protected static bool? InvincibilityOverwrite { get; set; } = null;

        [DefaultValue(WaitingForWorldIsReady)]
        private enum InvincibleWorkerStates
        {
            WaitingForWorldIsReady = 0,
            Mortal,
            ToMortal,
            Immortal,
        }

        private static InvincibleWorkerStates state = default;

        private IModHelper helper = null;

        public InvincibleWorker(IModHelper helper)
        {
            this.helper = helper;

            HostAutomation.ResetAction += new EventHandler((d, e) => Reset());
        }

        public void Enable()
        {
            helper.Events.GameLoop.OneSecondUpdateTicked += OneSecondUpdateTicked;
        }

        public void Disable()
        {
            Game1.player.temporaryInvincibilityTimer = 0;
            helper.Events.GameLoop.OneSecondUpdateTicked -= OneSecondUpdateTicked;
        }

        private void OneSecondUpdateTicked(object sender, OneSecondUpdateTickedEventArgs e)
        {
            switch (state)
            {
                case InvincibleWorkerStates.WaitingForWorldIsReady:
                    if (Context.IsWorldReady)
                    {
                        state = InvincibleWorkerStates.Immortal;
                    }
                    break;

                case InvincibleWorkerStates.Mortal:
                    if (false != InvincibilityOverwrite)
                    {
                        state = InvincibleWorkerStates.Immortal;
                    }
                    break;

                case InvincibleWorkerStates.ToMortal:
                    Game1.player.temporaryInvincibilityTimer = 0;
                    state = InvincibleWorkerStates.Mortal;
                    break;

                case InvincibleWorkerStates.Immortal:
                    if (false == InvincibilityOverwrite)
                    {
                        state = InvincibleWorkerStates.ToMortal;
                    }
                    Game1.player.temporarilyInvincible = true;
                    Game1.player.temporaryInvincibilityTimer = -1000000000;
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        ///         Resets this class to its initial state
        /// </summary>
        private static void Reset()
        {
            state = default;
            Invincible.InvincibilityOverwrite = null;
        }
    }
}
