using DedicatedServer.Chat;
using StardewValley;
using System.Collections.Generic;

namespace DedicatedServer.HostAutomatorStages
{
    internal class FestivalChatBox
    {
        private const string entryMessage = "When you wish to start the festival, type \"start\" into chat. If you'd like to cancel your vote, type \"cancel\".";

        private IDictionary<long, Farmer> otherPlayers;
        private bool enabled = false;
        private HashSet<long> votes = new HashSet<long>();

        public FestivalChatBox()
        {
            this.otherPlayers = DedicatedServer.OnlineFarmers(); 
        }

        public bool IsEnabled()
        {
            return enabled;
        }

        public void Enable()
        {
            if (!enabled)
            {
                enabled = true;
                votes.Clear();
                DedicatedServer.chatBox.textBoxEnter(entryMessage);
                DedicatedServer.chatBox.ChatReceived += onChatReceived;
            }
        }

        public void Disable()
        {
            if (enabled)
            {
                enabled = false;
                votes.Clear();
                DedicatedServer.chatBox.ChatReceived -= onChatReceived;
            }
        }

        private void onChatReceived(object sender, ChatEventArgs e)
        {
            if (!otherPlayers.ContainsKey(e.SourceFarmerId))
            {
                return;
            }

            if (e.Message.ToLower() == "start")
            {
                votes.Add(e.SourceFarmerId);
            }
            else if (e.Message.ToLower() == "cancel")
            {
                votes.Remove(e.SourceFarmerId);
            }
        }

        public int NumVoted()
        {
            int count = 0;
            foreach (var id in otherPlayers.Keys)
            {
                if (votes.Contains(id))
                {
                    count++;
                }
            }
            return count;
        }

        public void SendChatMessage(string message)
        {
            DedicatedServer.chatBox.textBoxEnter(message);
        }
    }
}
