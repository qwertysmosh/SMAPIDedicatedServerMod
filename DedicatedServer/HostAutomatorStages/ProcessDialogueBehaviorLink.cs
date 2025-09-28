using DedicatedServer.HostAutomatorStages.BehaviorStates;
using DedicatedServer.Utils;
using StardewValley;
using StardewValley.Menus;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DedicatedServer.HostAutomatorStages
{
    internal class ProcessDialogueBehaviorLink : BehaviorLink2
    {
        #region Required in derived class

        public override int WaitTimeAutoLoad { get; set; } = 0;
        public override int WaitTime { get; set; }

        public override void Process()
        {
            if (Game1.activeClickableMenu != null)
            {
                if (Game1.activeClickableMenu is DialogueBox dialogueBox)
                {
                    if (false == dialogueBox.isQuestion)
                    {
                        // Skip the non-question dialogue
                        dialogueBox.receiveLeftClick(0, 0);
                        WaitTime = (int)(60 * 0.2);
                    }
                    else
                    {
                        // For question dialogues, determine which question is being asked based on the
                        // question / response text
                        int mushroomsResponseIdx = -1;
                        int batsResponseIdx = -1;
                        int yesResponseIdx = -1;
                        int noResponseIdx = -1;

                        for (int i = 0; i < dialogueBox.responses.Length; i++)
                        {
                            var response = dialogueBox.responses[i];
                            var lowercaseText = response.responseText.ToLower();
                            if (lowercaseText == "mushrooms")
                            {
                                mushroomsResponseIdx = i;
                            }
                            else if (lowercaseText == "bats")
                            {
                                batsResponseIdx = i;
                            }
                            else if (lowercaseText == "yes")
                            {
                                yesResponseIdx = i;
                            }
                            else if (lowercaseText == "no")
                            {
                                noResponseIdx = i;
                            }
                        }

                        dialogueBox.selectedResponse = 0;
                        if (mushroomsResponseIdx >= 0 && batsResponseIdx >= 0)
                        {
                            // This is the cave question. Answer based on mod config.
                            if (DedicatedServer.config.MushroomsOrBats.ToLower() == "mushrooms")
                            {
                                dialogueBox.selectedResponse = mushroomsResponseIdx;
                            }
                            else if (DedicatedServer.config.MushroomsOrBats.ToLower() == "bats")
                            {
                                dialogueBox.selectedResponse = batsResponseIdx;
                            }
                        }
                        else if (yesResponseIdx >= 0 && noResponseIdx >= 0)
                        {
                            // This is the pet question. Answer based on mod config.
                            if (DedicatedServer.config.shouldAcceptPet())
                            {
                                dialogueBox.selectedResponse = yesResponseIdx;
                            }
                            else
                            {
                                dialogueBox.selectedResponse = noResponseIdx;
                            }
                        }

                        dialogueBox.receiveLeftClick(0, 0);
                        WaitTime = (int)(60 * 0.2);
                    }
                }
                else if (Game1.activeClickableMenu is NamingMenu namingMenu)
                {
                    TextBox textBox = namingMenu.textBox;
                    textBox.Text = DedicatedServer.config.PetName;
                    textBox.RecieveCommandInput('\r');
                    WaitTime = (int)(60 * 0.2);
                }
                else if (Game1.activeClickableMenu is LevelUpMenu levelUpMenu)
                {
                    levelUpMenu.okButtonClicked();
                }
                else if (Game1.activeClickableMenu is ItemListMenu itemListMenu)
                {
                    // Lost item dialog when the host faints
                    itemListMenuInfo?.Invoke(itemListMenu, new object[] { });
                    WaitTime = (int)(60 * 0.2);
                }
                else if (Game1.activeClickableMenu is ItemGrabMenu itemGrabMenu)
                {
                    // Good to test when you go into the mine and are presented with a sword

                    var count = itemGrabMenu.ItemsToGrabMenu.actualInventory.Count();

                    if (false == ServerHost.EnsureFreeSlotNumber(count))
                    {
                        // Try again later
                        return;
                    }

                    var del = new List<Item>();
                    foreach (var item in itemGrabMenu.ItemsToGrabMenu.actualInventory)
                    {
                        if (null == item) { continue; }

                        Game1.player.addItemToInventoryBool(item);
                        del.Add(item);
                    }

                    foreach (var item in del)
                    {
                        itemGrabMenu.ItemsToGrabMenu.actualInventory.Remove(item);
                    }

                    if (false == itemGrabMenu.areAllItemsTaken())
                    {
                        // Drop all remaining items from the menu
                        itemGrabMenu.DropRemainingItems();
                    }

                    if (itemGrabMenu.readyToClose())
                    {
                        okClicked();
                    }

                    WaitTime = (int)(60 * 0.2);
                }
            }
        }

        #endregion

        private static MethodInfo itemListMenuInfo = typeof(ItemListMenu).GetMethod("okClicked", BindingFlags.Instance | BindingFlags.NonPublic);

        private void okClicked()
        {
            Game1.activeClickableMenu = null;

            if (null != Game1.CurrentEvent)
            {
                Game1.CurrentEvent.CurrentCommand++;
            }

            Game1.playSound("bigDeSelect");
        }
    }
}