using DedicatedServer.Chat;
using DedicatedServer.HostAutomatorStages;
using DedicatedServer.Utils;
using StardewValley;
using System.Linq;

namespace DedicatedServer.MessageCommands
{
    internal abstract class ShippingMenuCommandListener
    {
        public static void Enable()
        {
            DedicatedServer.chatBox.ChatReceived += chatReceived;
        }

        public static void Disable()
        {
            DedicatedServer.chatBox.ChatReceived -= chatReceived;
        }

        private static void chatReceived(object sender, ChatEventArgs e)
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
                    if (SkipShippingMenuBehaviorLink.SkipShippingMenu())
                    {
                        DedicatedServer.chatBox.textBoxEnter("Ok button of the shipping menu clicked." + TextColor.Purple);
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
