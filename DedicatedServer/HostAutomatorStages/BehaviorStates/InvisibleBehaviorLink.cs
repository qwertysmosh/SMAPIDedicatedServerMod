using DedicatedServer.HostAutomatorStages.BehaviorStates;
using DedicatedServer.Utils;
using StardewValley;
using System;

namespace DedicatedServer.HostAutomatorStages
{
    /// <summary>
    ///         This class can make the host invisible
    /// <br/>   
    /// <br/>   - As long as someone is invisible, you cannot give them anything.
    /// <br/>   - After someone has been warped/teleported, they will be visible again.
    /// </summary>
    internal class InvisibleBehaviorLink : BehaviorLink
    {
        #region Required in derived class

        public override int WaitTimeAutoLoad { get; set; } = 0;
        public override int WaitTime { get; set; }

        public override void Process()
        {
            switch (InvisibleOverwrite)
            {
                case true:
                    if (SetInvisibleDisplayOnChanges())
                    {
                        BehaviorChain.WaitTime = 60;
                    }
                    break;

                case false:
                    if (SetVisibleDisplayOnChanges())
                    {
                        BehaviorChain.WaitTime = 60;
                    }
                    break;
            }
        }

        #endregion

        public InvisibleBehaviorLink()
        {
            HostAutomation.ResetAction += new EventHandler((d, e) => Reset() );
        }

        /// <summary>
        ///         Overwrites the behavior of the class <see cref="InvisibleBehaviorLink"/>
        /// <br/>   
        /// <br/>   true : The behavior of this class is not changed.
        /// <br/>   false: Host is visible
        /// <br/>   
        /// <br/>   Works only if <see cref="OnUpdateTicked"/> ticks
        /// <br/>   Otherwise you have to use:
        /// <br/>   - <see cref="SetVisibleDisplayOnChanges"/> or 
        /// <br/>   - <see cref="Invisible.SetVisibleDisplayOnChanges()"/>
        /// </summary>
        protected static bool InvisibleOverwrite { set; get; } = true;

        /// <summary>
        ///         Resets this class to its initial state
        /// </summary>
        private static void Reset()
        {
            InvisibleOverwrite = true;
        }

        /// <summary>
        ///         Makes the host invisible
        /// </summary>
        /// <returns>
        ///         true : Visibility has been changed.
        /// <br/>   false: Visibility has not been changed.</returns>
        protected static bool SetInvisibleDisplayOnChanges()
        {
            bool changed = false;

            if (Game1.displayFarmer)
            {
                Game1.displayFarmer = false;
                changed = true;
            }

            return changed;
        }

        /// <summary>
        ///         Makes the host visible
        /// </summary>
        /// <param name="forcedRefresh">
        ///         true : Forces the change of visibility.
        /// <br/>   false: Changes the visibility only when required.</param>
        /// <returns>
        ///         true : Visibility has been changed.
        /// <br/>   false: Visibility has not been changed.</returns>
        protected static bool SetVisibleDisplayOnChanges(bool forcedRefresh = false)
        {
            bool changed = false;

            if (false == Game1.displayFarmer || forcedRefresh)
            {
                Game1.displayFarmer = true;
                // Refresh to make bot back to visible
                Game1.player.warpFarmer(new Warp(
                    (int)Game1.player.Tile.X, (int)Game1.player.Tile.Y,
                    Game1.player.currentLocation.Name,
                    (int)Game1.player.Tile.X, (int)Game1.player.Tile.Y,
                    false, false));
                changed = true;
            }

            return changed;
        }
    }
}
