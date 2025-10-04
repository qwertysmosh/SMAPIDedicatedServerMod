using StardewValley;
using System;

namespace DedicatedServer.Utils
{
    internal abstract class Wallet
    {
        public static bool IsSeparate => Game1.player.team.useSeparateWallets.Value;

        public static bool IsChangeTonight => Game1.player.changeWalletTypeTonight.Value;

        /// <summary>
        ///         This field remembers whether the wallet type is changed tonight.
        /// <br/>   
        /// <br/>   The bool field <see cref="Farmer.changeWalletTypeTonight"/>,
        /// <br/>   which is accessed with <see cref="Game1.player.changeWalletTypeTonight.Value"/>,
        /// <br/>   remembers whether the wallet type is changed tonight.
        /// </summary>
        public static bool ChangeTonight {
            get => Game1.player.changeWalletTypeTonight.Value;
            set => Game1.player.changeWalletTypeTonight.Value = value;
        }

        /// <summary>
        /// </summary>
        /// <param name="farmer"></param>
        public static void Separate(Farmer farmer)
        {
            /// <code>
            /// | Should   | Is       | Toggle   | Action
            /// | separate | Separate | separate |
            /// +----------+----------+----------+------------
            /// | t        | t        | t        | toggle = f
            /// | t        | t        | f        | 
            /// | t        | f        | t        | 
            /// | t        | f        | f        | toggle = t
            /// </code>
            if (IsSeparate == ChangeTonight)
            {
                WriteToPlayer(null, String.Format(Game1.content.LoadString("Strings\\UI:Chat_SeparateWallets"), farmer.Name));
                ChangeTonight = !ChangeTonight;
            }
            else
            {
                WriteToPlayer(farmer, "Nothing to do");
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="farmer"></param>
        public static void Merge(Farmer farmer)
        {
            /// <code>
            /// | Should   | Is       | Toggle   | Action
            /// | separate | Separate | separate |
            /// +----------+----------+----------+------------
            /// | f        | t        | t        | 
            /// | f        | t        | f        | toggle = f
            /// | f        | f        | t        | toggle = f
            /// | f        | f        | f        | 
            /// </code>
            if (IsSeparate != ChangeTonight)
            {
                WriteToPlayer(null, String.Format(Game1.content.LoadString("Strings\\UI:Chat_MergeWallets"), farmer.Name));
                ChangeTonight = !ChangeTonight;
            }
            else
            {
                WriteToPlayer(farmer, "Nothing to do");
            }
        }

        private static void WriteToPlayer(Farmer farmer, string message)
        {
            if (null == farmer || farmer.UniqueMultiplayerID == Game1.player.UniqueMultiplayerID)
            {
                DedicatedServer.chatBox.textBoxEnter($" {message}");
            }
            else
            {
                DedicatedServer.chatBox.textBoxEnter($"/message {farmer.Name} {message}");
            }
        }
    }
}
