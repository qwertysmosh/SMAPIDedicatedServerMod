using DedicatedServer.HostAutomatorStages;

namespace DedicatedServer.Utils
{
    internal abstract class Invincible : InvincibleWorker
    {
        private Invincible() : base(null)
        {
        }

        /// <summary>
        /// <inheritdoc cref = "InvincibleWorker.InvincibilityOverwrite"/>
        /// </summary>
        public static new bool? InvincibilityOverwrite
        {
            get{ return InvincibleWorker.InvincibilityOverwrite; }
            set{ InvincibleWorker.InvincibilityOverwrite = value; }
        }
    }
}
