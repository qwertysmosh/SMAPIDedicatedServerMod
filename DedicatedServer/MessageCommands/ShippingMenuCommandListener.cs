using DedicatedServer.Chat;
using DedicatedServer.Config;
using StardewModdingAPI;
using StardewValley.Menus;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DedicatedServer.HostAutomatorStages;
using DedicatedServer.Utils;

namespace DedicatedServer.MessageCommands
{
    internal class ShippingMenuCommandListener
    {
        private IModHelper helper;
        private IMonitor monitor;
        private ModConfig config;
        private EventDrivenChatBox chatBox;

        public ShippingMenuCommandListener(IModHelper helper, IMonitor monitor, ModConfig config, EventDrivenChatBox chatBox)
        {
            this.helper = helper;
            this.monitor = monitor;
            this.config = config;
            this.chatBox = chatBox;
        }

        public void Enable()
        {
            chatBox.ChatReceived += chatReceived;
        }

        public void Disable()
        {
            chatBox.ChatReceived -= chatReceived;
        }

        private void chatReceived(object sender, ChatEventArgs e)
        {
            var tokens = e.Message.Split(' ');

            if (0 == tokens.Length) { return; }

            string command = tokens[0].ToLower();

            var sourceFarmer = Game1.getOnlineFarmers()
                .Where(farmer => farmer.UniqueMultiplayerID == e.SourceFarmerId)
                .FirstOrDefault();

            switch (command)
            {
                case "okay":
                    bool clicked = SkipShippingMenuBehaviorLink.SkipShippingMenu();
                    chatBox?.textBoxEnter("Ok button of the shipping menu clicked." + TextColor.Purple);
                    break;
                default:
                    break;
            }
        }
    }
}
