using DedicatedServer.Chat;
using StardewModdingAPI;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DedicatedServer.HostAutomatorStages.BehaviorStates
{
#warning Rename later
    internal abstract class BehaviorLink2
    {
        public abstract int WaitTimeAutoLoad { get; set; }
        /// <summary>
        ///          0: Called without waiting
        /// <br/>   +n: Waits n times before calling the function once.
        /// <br/>   -n: The function is never called.
        /// </summary>
        public abstract int WaitTime { get; set; }
        public abstract void Process();

        public void WaitTimeReset()
        {
            this.WaitTime = this.WaitTimeAutoLoad;
        }

        public BehaviorLink2()
        {
            WaitTimeReset();
        }
    }

#warning (only for testing)
    internal class TestBehaviorLink : BehaviorLink2
    {
        #region Required in derived class

        public override int WaitTimeAutoLoad { get; set; } = 5 * 60;
        public override int WaitTime { get; set; }
        public override void Process()
        {
            DedicatedServer.chatBox.textBoxEnter($" Called: {calles++}");
        }

        #endregion

        private int calles = 0;
    }
}
