using DedicatedServer.Chat;
using DedicatedServer.Config;
//DEBUG
using DedicatedServer.Utils;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DedicatedServer.HostAutomatorStages
{
    internal class AutomatedHost
    {
        private IModHelper helper;
        private BehaviorChain behaviorChain;
        private BehaviorState behaviorState;

        public AutomatedHost(IModHelper helper, IMonitor monitor, ModConfig config, EventDrivenChatBox chatBox)
        {
            behaviorChain = new BehaviorChain(helper, monitor, config, chatBox);
            behaviorState = new BehaviorState(monitor, chatBox);
            this.helper = helper;

            //DEBUG
            HostAutomation.BehaviorChain = behaviorChain;
            HostAutomation.BehaviorState = behaviorState;
        }

        public void Enable()
        {
            helper.Events.GameLoop.UpdateTicked += OnUpdate;
            helper.Events.GameLoop.DayStarted += OnNewDay;
        }

        public void Disable()
        {
            helper.Events.GameLoop.UpdateTicked -= OnUpdate;
            helper.Events.GameLoop.DayStarted -= OnNewDay;
        }

        private void OnNewDay(object sender, StardewModdingAPI.Events.DayStartedEventArgs e)
        {
            behaviorState.NewDay();
        }

        private void OnUpdate(object sender, StardewModdingAPI.Events.UpdateTickedEventArgs e)
        {
            behaviorChain.Process(behaviorState);
        }
    }
}
