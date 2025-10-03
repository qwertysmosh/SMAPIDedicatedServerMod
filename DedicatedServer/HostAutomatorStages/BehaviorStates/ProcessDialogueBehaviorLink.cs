using DedicatedServer.HostAutomatorStages.BehaviorStates;
using DedicatedServer.Utils;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;
using System.Collections.Generic;
using System.Reflection;

namespace DedicatedServer.HostAutomatorStages
{
    internal class ProcessDialogueBehaviorLink : BehaviorLink
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
                    // The variable safetyTimer is counted down by the game.
                    // The if statement prevents multiple and too early clicks.
                    if (0 < dialogueBox.safetyTimer)
                    {
                        return;
                    }

                    if (false == dialogueBox.isQuestion)
                    {
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
                            if (Game1.MasterPlayer.isInBed.Value)
                            {
                                // It is not possible to sleep in the morning.
                                // The time of day is only increased in increments of 10.
                                if (610 <= Game1.timeOfDay)
                                {
                                    dialogueBox.selectedResponse = yesResponseIdx;
                                }
                                else
                                {
                                    dialogueBox.selectedResponse = noResponseIdx;
                                }
                            }

                            if(false == hasPetSelectEvent)
                            {
                                if (null != Game1.CurrentEvent)
                                {
                                    if ("897405" == Game1.CurrentEvent.id ||
                                        "1590166" == Game1.CurrentEvent.id
                                    ){
                                        hasPetSelectEvent = true;

                                        // This is the pet question. Answer based on mod config.
                                        if (DedicatedServer.config.ShouldAcceptPet())
                                        {
                                            dialogueBox.selectedResponse = yesResponseIdx;
                                        }
                                        else
                                        {
                                            dialogueBox.selectedResponse = noResponseIdx;
                                        }
                                    }
                                }
                            }
                        }

                        // The index is used to select the button:
                        //  `dialogueBox.selectedResponse`
                        // An alternative is to jump on the button:
                        //  var button = dialogueBox.allClickableComponents[0].bounds.Center;
                        //  dialogueBox.receiveLeftClick(button.X, button.Y, true);

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
                else if (Game1.activeClickableMenu is LetterViewerMenu letterViewerMenu)
                {
                    // This block deals with opened letters
                    // This can be triggered with `DedicatedServer.OpenMailboxIfHasMail()`
                    // and it is triggered with the `CheckTheMailboxBehaviorLink` class

                    if (letterViewerMenu.readyToClose())
                    {
                        var point = letterViewerMenu.upperRightCloseButton.bounds.Center;
                        letterViewerMenu.receiveLeftClick(point.X, point.Y, true);

                        CheckTheMailboxBehaviorLink.CheckForNewMails();
                    }
                    WaitTime = (int)(60.0);
                }
                //else if (Game1.activeClickableMenu is QuestLog questLog)
                //{
                //    ; // As far as I know, you don't have to accept the quests to trigger anything.
                //}
                else if (Game1.activeClickableMenu is ItemListMenu itemListMenu)
                {
                    // Lost item dialog when the host faints
                    itemListMenuInfo?.Invoke(itemListMenu, System.Array.Empty<object>());
                    WaitTime = (int)(60 * 0.2);
                }
                else if (Game1.activeClickableMenu is ItemGrabMenu itemGrabMenu)
                {
                    // Good to test when you go into the mine and are presented with a sword, open a chest with items

                    if (false == (itemGrabMenu.context is ShippingBin))
                    {

                        var count = itemGrabMenu.ItemsToGrabMenu.actualInventory.Count;

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
        }

        #endregion

        public ProcessDialogueBehaviorLink()
        {
            if (Game1.player.eventsSeen.Contains("897405") || 
                Game1.player.eventsSeen.Contains("1590166")
            ){
                hasPetSelectEvent = true;
            }
        }

        private static bool hasPetSelectEvent = false;

        private static readonly MethodInfo itemListMenuInfo = typeof(ItemListMenu).GetMethod("okClicked", BindingFlags.Instance | BindingFlags.NonPublic);
                
        private static void okClicked()
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