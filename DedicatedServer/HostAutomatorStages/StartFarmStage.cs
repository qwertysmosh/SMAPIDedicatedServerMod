using DedicatedServer.Chat;
using DedicatedServer.Config;
using DedicatedServer.Crops;
using DedicatedServer.Chests;
using DedicatedServer.HostAutomatorStages.BehaviorStates;
using DedicatedServer.MessageCommands;
using DedicatedServer.Utils;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DedicatedServer.HostAutomatorStages
{
    internal class StartFarmStage
    {
        private void DebugLog(string message, LogLevel level)
        {
            var timestamp = DateTime.Now.ToString("MM-dd-yyyy HH:mm:ss");
            monitor.Log($"[{timestamp}] {message}", level);
        }

        private readonly bool allowRestartOfModAfterTitleMenu = true;

        private readonly IModHelper helper;
        private readonly IMonitor monitor;
        private readonly ModConfig config;

        private CropSaver cropSaver = null;
        private ChestLocker chestLocker = null;
        private ReadyCheckHelper readyCheckHelper = null;
        private InvincibleWorker invincibleWorker = null;
        private PasswordValidation passwordValidation = null;

        // Get connected player count
        private int GetConnectedPlayerCount()
        {
            // Contains other connected players besides host player, add 1 for host player
            return Game1.otherFarmers.Count + 1;
        }

        // Get farmer name
        private string GetFarmerName(long playerID)
        {
            Farmer farmer = Game1.getAllFarmers().FirstOrDefault(f => f.UniqueMultiplayerID == playerID);

            if (farmer != null)
            {
                // If newcomer
                if (string.IsNullOrWhiteSpace(farmer.Name))
                    return "Newcomer";

                // Otherwise, log farmer name
                return farmer.Name;
            }

            // If no farmer found, fallback
            return playerID.ToString();
        }

        private void HookMultiplayerEvents()
        {
            helper.Events.Multiplayer.PeerConnected += OnPlayerConnected;
            helper.Events.Multiplayer.PeerDisconnected += OnPlayerDisconnected;
        }

        private void UnhookMultiplayerEvents()
        {
            helper.Events.Multiplayer.PeerConnected -= OnPlayerConnected;
            helper.Events.Multiplayer.PeerDisconnected -= OnPlayerDisconnected;
        }

        private void OnPlayerConnected(object sender, PeerConnectedEventArgs e)
        {
            string playerName = GetFarmerName(e.Peer.PlayerID);
            int connectedPlayers = GetConnectedPlayerCount();
            int maxPlayers = Game1.netWorldState.Value.CurrentPlayerLimit;
            DebugLog($"{playerName} has joined. {connectedPlayers}/{maxPlayers}", LogLevel.Info);
        }

        private async void OnPlayerDisconnected(object sender, PeerDisconnectedEventArgs e)
        {
            // Wait 1 second
            await Task.Delay(1000);

            string playerName = GetFarmerName(e.Peer.PlayerID);
            int connectedPlayers = GetConnectedPlayerCount();
            int maxPlayers = Game1.netWorldState.Value.CurrentPlayerLimit;

            DebugLog($"{playerName} has left. {connectedPlayers}/{maxPlayers}", LogLevel.Info);
        }

        // Start Farm Stage
        public StartFarmStage(IModHelper helper, IMonitor monitor, ModConfig config)
        {
            this.helper = helper;
            this.monitor = monitor;
            this.config = config;

            MainController.InitStaticVariables(helper, monitor, config);

            if (config.EnableCropSaver)
            {
                cropSaver = new CropSaver(helper, monitor, config);
                cropSaver.Enable();
            }

            if (config.EnableChestLocker) {
                chestLocker = new ChestLocker(helper, config);
                chestLocker.Enable();
            }

            EnableReturnToTitle();
            EnableExecute();
            EnableSaveLoaded();
        }

        ~StartFarmStage()
        {
            Dispose();
            UnhookMultiplayerEvents();
        }

        public void Dispose()
        {
            DisableReturnToTitle();
            DisableExecute();

            if (false == allowRestartOfModAfterTitleMenu)
            {
                DisableSaveLoaded();
            }

            BehaviorChain.Disable();

            BuildCommandListener.Disable();
            DemolishCommandListener.Disable();
            PauseCommandListener.Disable();
            ServerCommandListener.Disable();
            ShippingMenuCommandListener.Disable();

            SleepWorker.Reset();
            RestartDayWorker.Reset();

            CropSaver.Disable();
            MultiplayerOptions.Reset();
            // MoveBuildPermission // Disable/Reset is not necessary

            // HostHouseUpgrade // Disable/Reset is not necessary
            // Wallet // Disable/Reset is not necessary

            readyCheckHelper?.Disable();
            readyCheckHelper = null;

            invincibleWorker?.Disable();
            invincibleWorker = null;

            passwordValidation?.Disable();
            passwordValidation = null;
        }

        private void EnableReturnToTitle()
        {
            helper.Events.GameLoop.ReturnedToTitle += OnReturnToTitle;
        }

        private void DisableReturnToTitle()
        {
            helper.Events.GameLoop.ReturnedToTitle -= OnReturnToTitle;
        }

        private void OnReturnToTitle(object sender, ReturnedToTitleEventArgs e)
        {
            DisableReturnToTitle();
            Dispose();
        }

        private void EnableExecute()
        {
            helper.Events.GameLoop.UpdateTicked += Execute;
        }

        private void DisableExecute()
        {
            helper.Events.GameLoop.UpdateTicked -= Execute;
        }

        private void Execute(object sender, UpdateTickedEventArgs e)
        {
            if (Game1.netWorldState.Value.IsPaused)
            {
                return;
            }

            if (Game1.activeClickableMenu is not TitleMenu)
            {
                return;
            }

            Farmer hostedFarmer = MainController.GetFarmerOfSaveGameOrDefault(config.FarmName);

            if (null == hostedFarmer)
            {
                CreateNewGame();
            }
            else
            {
                LoadExistingGame(hostedFarmer);
            }

            DisableExecute();
        }

        private void CreateNewGame()
        {
            DebugLog($"Failed to find farm slot. Creating new farm \"{config.FarmName}\" and hosting on co-op", LogLevel.Debug);

            // Mechanism pulled from CoopMenu.HostNewFarmSlot; CharacterCustomization class; and AdvancedGameOptions class
            Game1.resetPlayer();

            // Starting cabins
            if (config.StartingCabins < 0 || config.StartingCabins > 10)
            {
                LogConfigError("Starting cabins must be an integer in [0-10]");
                Exit(-1);
            }
            Game1.startingCabins = config.StartingCabins;

            // Cabin layout
            if (config.CabinLayout != "nearby" && config.CabinLayout != "separate")
            {
                LogConfigError("Cabin layout must be either \"nearby\" or \"separate\"");
                Exit(-1);
            }
            if (config.CabinLayout == "separate")
            {
                Game1.cabinsSeparate = true;
            }
            else
            {
                Game1.cabinsSeparate = false;
            }

            // Profit margin
            if (config.ProfitMargin != "normal" && config.ProfitMargin != "75%" && config.ProfitMargin != "50%" && config.ProfitMargin != "25%")
            {
                LogConfigError("Profit margin must be one of \"normal\", \"75%\", \"50%\", or \"25%\"");
                Exit(-1);
            }
            if (config.ProfitMargin == "normal")
            {
                Game1.player.difficultyModifier = 1f;
            }
            else if (config.ProfitMargin == "75%")
            {
                Game1.player.difficultyModifier = 0.75f;
            }
            else if (config.ProfitMargin == "50%")
            {
                Game1.player.difficultyModifier = 0.5f;
            }
            else
            {
                Game1.player.difficultyModifier = 0.25f;
            }

            // Money style
            if (config.MoneyStyle != "shared" && config.MoneyStyle != "separate")
            {
                LogConfigError("Money style must be either \"shared\" or \"separate\"");
                Exit(-1);
            }
            if (config.MoneyStyle == "separate")
            {
                Game1.player.team.useSeparateWallets.Value = true;
            }
            else
            {
                Game1.player.team.useSeparateWallets.Value = false;
            }

            // Farm name
            Game1.player.farmName.Value = config.FarmName;

            // Pet species
            if (config.ShouldAcceptPet())
            {
                if (ModConfig.FirstDogIndex <= config.PetBreed)
                {
                    Game1.player.whichPetType = StardewValley.Characters.Pet.type_dog;
                }
                else
                {
                    Game1.player.whichPetType = StardewValley.Characters.Pet.type_cat;
                }

                Game1.player.whichPetBreed = config.GetPetBreedIndex().ToString();
            }
            else
            {
                Game1.player.whichPetBreed = "0";
                Game1.player.whichPetType = StardewValley.Characters.Pet.type_dog;
            }

            // Farm type
            if (config.FarmType != "standard" && config.FarmType != "riverland" &&
                config.FarmType != "forest" && config.FarmType != "hilltop" &&
                config.FarmType != "wilderness" && config.FarmType != "fourcorners" &&
                config.FarmType != "beach" && config.FarmType != "meadowlands")
            {
                LogConfigError("Farm type must be one of \"standard\", \"riverland\", \"forest\", \"hilltop\", \"wilderness\", \"fourcorners\", \"beach\", or \"meadowlands\"");
                Exit(-1);
            }
            if (config.FarmType == "standard")
            {
                Game1.whichFarm = 0;
            }
            else if (config.FarmType == "riverland")
            {
                Game1.whichFarm = 1;
            }
            else if (config.FarmType == "forest")
            {
                Game1.whichFarm = 2;
            }
            else if (config.FarmType == "hilltop")
            {
                Game1.whichFarm = 3;
            }
            else if (config.FarmType == "wilderness")
            {
                Game1.whichFarm = 4;
            }
            else if (config.FarmType == "fourcorners")
            {
                Game1.whichFarm = 5;
            }
            else if (config.FarmType == "beach")
            {
                Game1.whichFarm = 6;
            }
            else if (config.FarmType == "meadowlands")
            {
                // Farm type 7 is mod
                // https://github.com/Pathoschild/SMAPI/issues/957
                // ModFarmType modFarmType = Game1.whichModFarm;
                // modFarmType.Id = "MeadowlandsFarm";
                // Game1.whichFarm = 7;

                LogConfigError("The meadowlands farm is currently not supported:\nhttps://github.com/Pathoschild/SMAPI/issues/957");
                Exit(-1);
            }

            // Community center bundles type
            if (config.CommunityCenterBundles != "normal" && config.CommunityCenterBundles != "remixed")
            {
                LogConfigError("Community center bundles must be either \"normal\" or \"remixed\"");
                Exit(-1);
            }
            if (config.CommunityCenterBundles == "normal")
            {
                Game1.bundleType = Game1.BundleType.Default;
            }
            else
            {
                Game1.bundleType = Game1.BundleType.Remixed;
            }

            // Guarantee year 1 completable flag
            Game1.game1.SetNewGameOption("YearOneCompletable", config.GuaranteeYear1Completable);

            // Mine rewards type
            if (config.MineRewards != "normal" && config.MineRewards != "remixed")
            {
                LogConfigError("Mine rewards must be either \"normal\" or \"remixed\"");
                Exit(-1);
            }
            if (config.MineRewards == "normal")
            {
                Game1.game1.SetNewGameOption("MineChests", Game1.MineChestType.Default);
            }
            else
            {
                Game1.game1.SetNewGameOption("MineChests", Game1.MineChestType.Remixed);
            }

            // Monsters spawning at night on farm
            Game1.spawnMonstersAtNight = config.SpawnMonstersOnFarmAtNight;
            Game1.game1.SetNewGameOption("SpawnMonstersAtNight", config.SpawnMonstersOnFarmAtNight);

            // Random seed
            Game1.startingGameSeed = config.RandomSeed;

            // Configuration is done; Set server bot constants
            Game1.player.Name = "ServerBot";
            Game1.player.displayName = Game1.player.Name;
            Game1.player.favoriteThing.Value = "Farms";
            Game1.player.isCustomized.Value = true;
            Game1.multiplayerMode = 2;

            var menu = (TitleMenu)Game1.activeClickableMenu;

            // Start game
            menu.createdNewCharacter(true);
        }

        private void LoadExistingGame(Farmer hostedFarmer)
        {
            DebugLog($"Hosting {hostedFarmer.slotName} on co-op", LogLevel.Debug);

            // Mechanisms pulled from CoopMenu.HostFileSlot
            Game1.multiplayerMode = 2;
            SaveGame.Load(hostedFarmer.slotName);
            Game1.exitActiveMenu();
        }

        private void EnableSaveLoaded()
        {
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
        }

        private void DisableSaveLoaded()
        {
            helper.Events.GameLoop.SaveLoaded -= OnSaveLoaded;
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            if (false == allowRestartOfModAfterTitleMenu)
            {
                DisableSaveLoaded();
            }

            if(Game1.MasterPlayer.farmName.Value != config.FarmName)
            {
                // The name of the mod and the current game must match for the
                // automation functions to load.
                // This check is necessary because otherwise it would be possible
                // to switch to the main menu and start the wrong game, which
                // would cause the automations to be executed.
                return;
            }

            Game1.onScreenMenus.Remove(Game1.chatBox);
            var chatBox = new EventDrivenChatBox();
            Game1.chatBox = chatBox;
            Game1.onScreenMenus.Add(chatBox);
            MainController.InitChatBox(chatBox);

           // Max player limit
            if (config.MaxPlayers < 1 || config.MaxPlayers > 32)
            {
                LogConfigError("Max players must be an integer in [1-32].");
                Exit(-1);
            }
            Game1.netWorldState.Value.CurrentPlayerLimit = config.MaxPlayers;

            // We set bot mining lvl to 10 so he doesn't lvlup passively
            Game1.player.miningLevel.Value = 10;

            BuildCommandListener.Enable();
            DemolishCommandListener.Enable();
            PauseCommandListener.Enable();
            ServerCommandListener.Enable();
            ShippingMenuCommandListener.Enable();

            SleepWorker.Reset();
            RestartDayWorker.Reset();

            CropSaver.Init();
            MultiplayerOptions.Init();
            MoveBuildPermission.Init();

            // HostHouseUpgrade
            // Wallet

            readyCheckHelper = new ReadyCheckHelper();
            readyCheckHelper.Enable();

            invincibleWorker = new InvincibleWorker(helper);
            invincibleWorker.Enable();

            passwordValidation = new PasswordValidation(helper, config, chatBox);
            passwordValidation.Enable();

            BehaviorChain.InitStaticVariables(

                // This function is always called, implements the pause behavior, and
                // enables the execution of the behavior chain through the return value.
                ProcessPauseBehaviorLink.ShouldPause,

                new List<BehaviorLink> {

                    // Opens the mailbox when letters are present 
                    new CheckTheMailboxBehaviorLink(),

                    // Skip skippable events
                    new SkipEventsBehaviorLink(),

                    // Respond to dialogue box question if present, skipping non-question dialogue
                    new ProcessDialogueBehaviorLink(),

                    // Skip shipping menu
                    new SkipShippingMenuBehaviorLink(),

                    // If in farmhouse and haven't checked for parsnip seeds, check for parsnip seeds
                    new CheckForParsnipSeedsBehaviorLink(),

                    // If in farmhouse and haven't left farmhouse for the day, leave farmhouse
                    new ExitFarmHouseBehaviorLink(),

                    // Makes the host virtually immortal, which is good if he becomes visible.
                    new InvisibleBehaviorLink(),

                    // If we don't have the fishing rod yet, and it's available, get it.
                    new GetFishingRodBehaviorLink(),

                    // If we haven't unlocked the community center yet, and we can, then unlock it.
                    new UnlockCommunityCenterBehaviorLink(),

                    // If we haven't watched the end cutscene for the community scenter yet, and we can, then watch it.
                    new PurchaseJojaMembershipBehaviorLink(),

                    // Ends the community center
                    new EndCommunityCenterBehaviorLink(),

                    // Put the host to bed
                    new TransitionSleepBehaviorLink(),

                    // If our state of festival attendance should be switched, then switch it
                    // If our leave festival state should be switched, then switch it
                    // If we're at a festival and we need to watch the festival chatbox, then watch it
                    new TransitionFestivalAttendanceBehaviorLink(),
                }
            );

            BehaviorChain.Enable();
            HookMultiplayerEvents();
        }

        private void LogConfigError(string error)
        {
            DebugLog($"Error in DedicatedServer mod config file. {error}", LogLevel.Error);
        }

        private static void Exit(int statusCode)
        {
            MainController.Exit(statusCode);
        }
    }
}
