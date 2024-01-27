using DedicatedServer.HostAutomatorStages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DedicatedServer.Utils
{
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

    }
}
