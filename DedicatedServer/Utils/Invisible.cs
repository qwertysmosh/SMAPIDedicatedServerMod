using DedicatedServer.HostAutomatorStages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DedicatedServer.Utils
{
    /// <summary>
    /// <inheritdoc cref = "InvisibleBehaviorLink"/>
    /// </summary>
    internal abstract class Invisible : InvisibleBehaviorLink
    {
        private Invisible() : base(null)
        {
        }

        /// <summary>
        /// <inheritdoc cref = "InvisibleBehaviorLink.InvisibleOverwrite"/>
        /// </summary>
        public static new bool InvisibleOverwrite
        {
            get { return InvisibleBehaviorLink.InvisibleOverwrite; }
            set { InvisibleBehaviorLink.InvisibleOverwrite = value; }
        }

        /// <summary><inheritdoc cref = "InvisibleBehaviorLink.SetInvisibleDisplayOnChanges"/></summary>
        /// <returns><inheritdoc cref = "InvisibleBehaviorLink.SetInvisibleDisplayOnChanges"/></returns>
        public static new bool SetInvisibleDisplayOnChanges()
        {
            return InvisibleBehaviorLink.SetInvisibleDisplayOnChanges();
        }

        /// <summary><inheritdoc cref = "InvisibleBehaviorLink.SetVisibleDisplayOnChanges"/></summary>
        /// <returns><inheritdoc cref = "InvisibleBehaviorLink.SetVisibleDisplayOnChanges"/></returns>
        public static bool SetVisibleDisplayOnChanges()
        {
            return InvisibleBehaviorLink.SetVisibleDisplayOnChanges(forcedRefresh: true);
        }

    }
}
