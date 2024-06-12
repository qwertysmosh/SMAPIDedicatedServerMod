using DedicatedServer.Chat;
using DedicatedServer.HostAutomatorStages;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DedicatedServer.MessageCommands
{
    internal class BuildCommandListener
    {
        private static Dictionary<string, Action<EventDrivenChatBox, Farmer>> buildingActions = new Dictionary<string, Action<EventDrivenChatBox, Farmer>>
        {
            {"stone_cabin", genBuildCabin("Stone Cabin")},
            {"plank_cabin", genBuildCabin("Plank Cabin")},
            {"log_cabin", genBuildCabin("Log Cabin")},
        };
        private static readonly string validBuildingNamesList = genValidBuildingNamesList();

        private EventDrivenChatBox chatBox;

        public BuildCommandListener(EventDrivenChatBox chatBox)
        {
            this.chatBox = chatBox;
        }

        private static Action<EventDrivenChatBox, Farmer> genBuildCabin(string cabinBlueprintName)
        {
            void buildCabin(EventDrivenChatBox chatBox, Farmer farmer)
            {
                var point = farmer.Tile;

                if (false == Game1.buildingData.ContainsKey("Cabin"))
                {
                    WriteToPlayer(chatBox, null, $"Error building Cabin");
                    return;
                }

                var building = new Building("Cabin", point);

                building.skinId.Value = cabinBlueprintName;

                switch (farmer.facingDirection.Value)
                {
                    case 1: // Right
                        point.X++;
                        point.Y -= (building.tilesHigh.Value / 2);
                        break;
                    case 2: // Down
                        point.X -= (building.tilesWide.Value / 2);
                        point.Y++;
                        break;
                    case 3: // Left
                        point.X -= building.tilesWide.Value;
                        point.Y -= (building.tilesHigh.Value / 2);
                        break;
                    default: // 0 = Up
                        point.X -= (building.tilesWide.Value / 2);
                        point.Y -= building.tilesHigh.Value;
                        break;
                }

                Game1.player.team.buildLock.RequestLock(delegate
                {
                    if (Game1.locationRequest == null)
                    {
                        var res = ((Farm)Game1.getLocationFromName("Farm")).buildStructure(building, new Vector2(point.X, point.Y), Game1.player, false);
                        if (res)
                        {
                            WriteToPlayer(chatBox, null, $"{farmer.Name} just built a {cabinBlueprintName}");
                        }
                        else
                        {
                            WriteToPlayer(chatBox, farmer, $"Error {Game1.content.LoadString("Strings\\UI:Carpenter_CantBuild")}");
                        }
                    }
                    Game1.player.team.buildLock.ReleaseLock();
                });
            }
            return buildCabin;
        }

        private static string genValidBuildingNamesList()
        {
            string str = "";
            var buildingActionsEnumerable = buildingActions.Keys.ToArray();
            for (int i = 0; i < buildingActionsEnumerable.Length; i++)
            {
                str += "\"" + buildingActionsEnumerable[i] + "\"";
                if (i + 1 < buildingActionsEnumerable.Length)
                {
                    str += ", ";
                }
                if (i + 1 == buildingActionsEnumerable.Length - 1)
                {
                    str += "and ";
                }
            }
            return str;
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
            // Private message chatKind is 3
            var tokens = e.Message.ToLower().Split(' ');
            if (tokens.Length == 0)
            {
                return;
            }

            if (Game1.player.UniqueMultiplayerID != e.SourceFarmerId)
            {
                if (ChatBox.privateMessage != e.ChatKind) { return; }
            }

            var sourceFarmer = Game1.otherFarmers.Values
                .Where(farmer => farmer.UniqueMultiplayerID == e.SourceFarmerId)
                .FirstOrDefault()
                ?? Game1.player;

            if (tokens[0] == "build") // /message ServerBot Build Stone_Cabin
            {
                if (false == PasswordValidation.IsAuthorized(e.SourceFarmerId, p => p.Build))
                {
                    WriteToPlayer(sourceFarmer, PasswordValidation.notAuthorizedMessage);
                    return;
                }

                if (tokens.Length != 2)
                {
                    WriteToPlayer(sourceFarmer, "Error: Invalid command usage.");
                    WriteToPlayer(sourceFarmer, "Usage: build [building_name]");
                    WriteToPlayer(sourceFarmer, $"Valid building names include {validBuildingNamesList}");
                    return;
                }

                var buildingName = tokens[1];
                if (buildingActions.TryGetValue(buildingName, out var action))
                {
                    // Find the farmer it came from and determine their location
                    var location = sourceFarmer.currentLocation;
                     
                    if (location is Farm f)
                    {
                        action(chatBox, sourceFarmer);
                    }
                    else
                    {
                        WriteToPlayer(sourceFarmer, "Error: You cannot place buildings outside of the farm!");
                    }
                }
                else
                {
                    WriteToPlayer(sourceFarmer, $"Error: Unrecognized building name \"{buildingName}\"");
                    WriteToPlayer(sourceFarmer, $"Valid building names include {validBuildingNamesList}");
                }
            }
        }

        private void WriteToPlayer(Farmer farmer, string message)
        {
            WriteToPlayer(chatBox, farmer, message);
        }

        private static void WriteToPlayer(EventDrivenChatBox chatBox, Farmer farmer, string message)
        {
            if (null == farmer || farmer.UniqueMultiplayerID == Game1.player.UniqueMultiplayerID)
            {
                chatBox.textBoxEnter($" {message}");
            }
            else
            {
                chatBox.textBoxEnter($"/message {farmer.Name} {message}");
            }
        }
    }
}
