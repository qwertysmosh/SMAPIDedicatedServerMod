using DedicatedServer.Chat;
using DedicatedServer.HostAutomatorStages;
using StardewValley;
using StardewValley.Menus;

namespace DedicatedServer.MessageCommands
{
    internal class PauseCommandListener
    {
        private EventDrivenChatBox chatBox;

        public PauseCommandListener(EventDrivenChatBox chatBox)
        {
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
            var tokens = e.Message.ToLower().Split(' ');
            if (tokens.Length == 0)
            {
                return;
            }

            if (Game1.player.UniqueMultiplayerID != e.SourceFarmerId)
            {
                if (ChatBox.privateMessage != e.ChatKind) { return; }
            }

            if (tokens[0] == "pause")
            {
                if (false == PasswordValidation.IsAuthorized(e.SourceFarmerId, p => p.Pause))
                {
                    chatBox.textBoxEnter(PasswordValidation.notAuthorizedMessage);
                    return;
                }

                Game1.netWorldState.Value.IsPaused = !Game1.netWorldState.Value.IsPaused;
                if (Game1.netWorldState.Value.IsPaused)
                {
                    chatBox.globalInfoMessage("Paused");
                    return;
                }
                chatBox.globalInfoMessage("Resumed");
            }
        }
    }
}
