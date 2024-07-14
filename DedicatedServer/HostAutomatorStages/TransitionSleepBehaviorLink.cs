using DedicatedServer.Utils;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DedicatedServer.HostAutomatorStages
{
    internal class TransitionSleepBehaviorLink : BehaviorLink
    {
        private IMonitor monitor;

        private static MethodInfo info = typeof(GameLocation).GetMethod("doSleep", BindingFlags.Instance | BindingFlags.NonPublic);

        public TransitionSleepBehaviorLink(IMonitor monitor, BehaviorLink next = null) : base(next)
        {
            this.monitor = monitor;
        }

        public override void Process(BehaviorState state)
        {
            if (Utils.Sleeping.ShouldSleep() && !Utils.Sleeping.IsSleeping())
            {
                // After the required number of players have triggered sleep once,
                // the pause is deactivated until the next day.
                HostAutomation.PreventPauseUntilNextDay();

                if (state.HasBetweenTransitionSleepWaitTicks())
                {
                    state.DecrementBetweenTransitionSleepWaitTicks();
                }
                else if (Game1.currentLocation is FarmHouse)
                {
                    monitor.Log($"The host lies down in bed", LogLevel.Debug);
                    Game1.player.isInBed.Value = true;
                    Game1.player.sleptInTemporaryBed.Value = true;
                    Game1.player.timeWentToBed.Value = Game1.timeOfDay;
                    Game1.netReady.SetLocalReady("sleep", ready: true);
                    Game1.dialogueUp = false;
                    Game1.activeClickableMenu = new ReadyCheckDialog("sleep", allowCancel: true, delegate
                    {
                        Game1.player.isInBed.Value = true;
                        Game1.player.sleptInTemporaryBed.Value = true;
                        info.Invoke(Game1.currentLocation, new object[]{});
                    }, delegate (Farmer who)
                    {
                        if (Game1.activeClickableMenu != null && Game1.activeClickableMenu is ReadyCheckDialog rcd)
                        {
                            rcd.closeDialog(who);
                        }

                        who.timeWentToBed.Value = 0;
                    });

                    if (!Game1.player.team.announcedSleepingFarmers.Contains(Game1.player))
                        Game1.player.team.announcedSleepingFarmers.Add(Game1.player);

                    state.Sleep();
                }
                else
                {
                    monitor.Log($"Warp to sleep", LogLevel.Debug);
                    Game1.player.warpFarmer(WarpPoints.FarmHouseWarp);
                    state.WarpToSleep();
                }
            }
            else if (!Utils.Sleeping.ShouldSleep() && Utils.Sleeping.IsSleeping())
            {
                if (state.HasBetweenTransitionSleepWaitTicks())
                {
                    state.DecrementBetweenTransitionSleepWaitTicks();
                }
                else
                {
                    monitor.Log($"Cancel sleep", LogLevel.Debug);
                    if (Game1.activeClickableMenu != null && Game1.activeClickableMenu is ReadyCheckDialog rcd)
                    {
                        rcd.closeDialog(Game1.player);
                    }
                    Game1.netReady.SetLocalReady("sleep", false);
                    state.CancelSleep();
                }
            }
            else
            {
                state.ClearBetweenTransitionSleepWaitTicks();
                processNext(state);
            }
        }
    }
}
