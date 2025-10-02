using DedicatedServer.Chat;
using DedicatedServer.Config;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace DedicatedServer
{
    internal static class DedicatedServer
    {
        private static FieldInfo multiplayerFieldInfo = typeof(Game1).GetField("multiplayer", BindingFlags.NonPublic | BindingFlags.Static);

        private static Multiplayer multiplayer = null;

        public static IModHelper helper { get; private set; }
        public static IMonitor monitor { get; private set; }
        public static ModConfig config { get; private set; }
        public static EventDrivenChatBox chatBox { get; private set; }

        public static void InitStaticVariables(IModHelper helper, IMonitor monitor, ModConfig config)
        {
            DedicatedServer.helper = helper;
            DedicatedServer.monitor = monitor;
            DedicatedServer.config = config;
        }

        public static void InitChatBox(EventDrivenChatBox chatBox)
        {
            DedicatedServer.chatBox = chatBox;
        }

        public static void Dispose()
        {
            DedicatedServer.helper = null;
            DedicatedServer.monitor = null;
            DedicatedServer.config = null;
            DedicatedServer.chatBox = null;
        }

        #region Game

        static MethodInfo findSaveGames = typeof(LoadGameMenu).GetMethod("FindSaveGames", BindingFlags.Static | BindingFlags.NonPublic);

        /// <summary>
        ///         List of farmers of saved games
        /// </summary>
        /// <returns>
        ///         List of <see cref="Farmer"/>
        /// </returns>
        public static List<Farmer> GetSaveGameFarmers()
        {
            object result = findSaveGames.Invoke(obj: null, parameters: new object[] { "" });
            List<Farmer> farmers = result as List<Farmer>;
            return farmers;
        }

        public static Farmer GetFarmerOfSaveGameOrDefault(string farmName, List<Farmer> saveGameFarmers)
        {
            foreach (Farmer farmer in saveGameFarmers)
            {
                if (!farmer.slotCanHost)
                {
                    continue;
                }
                if (farmer.farmName.Value == farmName)
                {
                    return farmer;
                }
            }

            return null;
        }

        public static Farmer GetFarmerOfSaveGameOrDefault(string farmName)
        {
            return GetFarmerOfSaveGameOrDefault(farmName, GetSaveGameFarmers());
        }

        #endregion

        #region Players

        /// <summary>
        /// Get number of all players who are currently connected
        /// </summary>
        public static int NumberOfPlayers { get => Game1.getOnlineFarmers().Count - 1; }

        public static Dictionary<long, Farmer> OnlineFarmers()
        {
            if (multiplayer == null)
            {
                multiplayer = (Multiplayer)multiplayerFieldInfo.GetValue(null);
            }

            var otherPlayers = new Dictionary<long, Farmer>();
            
            foreach (var farmer in Game1.otherFarmers.Values)
            {
                if (false == multiplayer.isDisconnecting(farmer))
                {
                    otherPlayers.Add(farmer.UniqueMultiplayerID, farmer);
                }
            }

            return otherPlayers;
        }

        #endregion

        #region Mailbox 

        /// <summary>
        ///         Open mailbox
        /// <br/>   
        /// <br/>   It's the same as if you were standing in front of the mailbox and right-clicking.
        /// <br/>   Then you need to process the start menu `LetterViewerMenu`.
        /// </summary>
        public static void OpenMailbox()
        {
            Game1.getFarm().mailbox();
        }

        /// <summary>
        ///         Opens the mailbox if there are letters and returns whether there were letters.
        /// </summary>
        /// <returns>
        ///         true: There were letters in the mailbox.
        /// <br/>   false: The mailbox was empty</returns>
        public static bool OpenMailboxIfHasMail()
            {
            if(HasMail())
            {
                OpenMailbox();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks whether the mailbox contains messages
        /// </summary>
        /// <returns>
        ///         true: There are letters in the mailbox
        /// <br/>   false: The mailbox is empty</returns>
        public static bool HasMail()
        {
            return 0 < MailCount();
            }

        /// <summary>
        /// Returns the number of emails in the mailbox
        /// </summary>
        /// <returns>Number of emails in the mailbox</returns>
        public static int MailCount()
        {
            // The command `Game1.MasterPlayer.mailbox.Count` can also be used
            return Game1.mailbox.Count;
        }

        /// <summary>
        /// Returns the number of open journal items
        /// </summary>
        //public static int JournalCount()
        //{
        //    return Game1.MasterPlayer.mailReceived.Count;
        //}

        #endregion

        #region Alias with documentation

        public static void Exit(int statusCode)
        {
            monitor.Log("Exiting...", LogLevel.Error);
            Environment.Exit(statusCode);
        }

        /// <summary>
        ///         Get whether all required players are ready to proceed.
        /// <br/>
        /// <br/>   <c>return ReadyCheckHelper.IsReady("festivalStart");</c>
        /// <br/>   <c>return ReadyCheckHelper.IsReady("festivalEnd");</c>
        /// </summary>
        /// <param name="checkName">The ready check ID.</param>
        /// <returns>
        ///         true : If all required players are ready
        /// <br/>   false: If one required player is not ready</returns>
        public static bool IsReady(string checkName)
        {
            return Game1.netReady.IsReady(checkName);
        }

        /// <summary>
        ///         Get the number of players that are ready to proceed.
        /// </summary>
        /// <param name="checkName">The ready check ID.</param>
        /// <returns></returns>
        public static int GetNumberReady(string checkName)
        {
            return Game1.netReady.GetNumberReady(checkName);
        }

        #endregion
    }
}
