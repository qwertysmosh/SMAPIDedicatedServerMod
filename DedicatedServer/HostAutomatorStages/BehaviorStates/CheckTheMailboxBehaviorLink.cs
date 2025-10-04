using StardewValley;
using StardewValley.Menus;

namespace DedicatedServer.HostAutomatorStages.BehaviorStates
{
    internal class CheckTheMailboxBehaviorLink : BehaviorLink
    {
        #region Required in derived class

        public override int WaitTimeAutoLoad { get; set; } = 120;
        public override int WaitTime { get; set; }

        public override void Process()
        {
            if (false == hasCheckedForMails &&
                null == Game1.CurrentEvent &&
                DedicatedServer.IsIdle() &&
                Game1.currentLocation is Farm
            ){
                DedicatedServer.IdleLockEnable();
                DedicatedServer.OpenMailboxIfHasMail();
                hasCheckedForMails = true;
            }
        }

        #endregion

        private static bool hasCheckedForMails = false;

        public CheckTheMailboxBehaviorLink() => Enable();

        ~CheckTheMailboxBehaviorLink() => Dispose();

        public void Dispose() => Disable();

        public static void Enable() => DedicatedServer.helper.Events.GameLoop.DayStarted += OnDayStarted;

        public static void Disable() => DedicatedServer.helper.Events.GameLoop.DayStarted -= OnDayStarted;

        public static void OnDayStarted(object sender, StardewModdingAPI.Events.DayStartedEventArgs e)
        {
            CheckForNewMails();
        }

        public static void CheckForNewMails() => hasCheckedForMails = false;
    }
}
