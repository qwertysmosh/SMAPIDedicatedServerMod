using DedicatedServer.Config;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Network;
using StardewValley.Objects;
using System.Linq;

namespace DedicatedServer.Chests
{
    public class ChestLocker
    {
        private IModHelper helper;
        private ModConfig config;

        public ChestLocker(IModHelper helper, ModConfig config)
        {
            this.helper = helper;
            this.config = config;
        }

        public void Enable()
        {
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
        }

        public void Disable()
        {
            helper.Events.GameLoop.UpdateTicked -= OnUpdateTicked;
        }

        private void OnUpdateTicked(object sender, StardewModdingAPI.Events.UpdateTickedEventArgs e)
        {
            foreach (var farmer in Game1.otherFarmers.Values)
            {
                var location = farmer.currentLocation;
                if (location == null)
                    continue;

                var farm = Game1.getFarm();
                if (farm == null)
                    return;

                // Farm House Location
                if (location is FarmHouse house)
                {
                    // Owner check
                    if (farmer.UniqueMultiplayerID == house.owner.UniqueMultiplayerID)
                    {
                        // If owner, skip mutex
                        continue;
                    }

                    // Lock offline player inventory
                    if (house is Cabin cabin)
                    {
                        var inventoryMutex = helper.Reflection.GetField<NetMutex>(cabin, "inventoryMutex").GetValue();
                        if (!inventoryMutex.IsLockHeld())
                            inventoryMutex.RequestLock();
                    }

                    // Lock fridge
                    var fridgeMutex = house.fridge.Value.mutex;
                    if (!fridgeMutex.IsLockHeld())
                        fridgeMutex.RequestLock();

                    // Lock chests
                    foreach (var chest in house.objects.Values.OfType<Chest>())
                    {
                        var chestMutex = chest.mutex;
                        if (!chestMutex.IsLockHeld())
                            chestMutex.RequestLock();
                    }
                }

                // Animal House Location
                else if (location is AnimalHouse animalHouse)
                {
                    var animalBuilding = farm.buildings.FirstOrDefault(b => b.indoors.Value == animalHouse);

                    if (animalBuilding != null && animalBuilding.owner.Value != 0
                        && farmer.UniqueMultiplayerID != animalBuilding.owner.Value)
                    {
                        // Lock chests
                        foreach (var chest in animalHouse.objects.Values.OfType<Chest>())
                        {
                            var chestMutex = chest.mutex;
                            if (!chestMutex.IsLockHeld())
                                chestMutex.RequestLock();
                        }
                    }
                }

                // Shed Location
                else if (location.NameOrUniqueName.Contains("Shed"))
                {
                    // Find matching building for interior
                    var shedBuilding = farm.buildings.FirstOrDefault(b => b.indoors.Value == location);

                    if (shedBuilding != null && shedBuilding.owner.Value != 0
                        && farmer.UniqueMultiplayerID != shedBuilding.owner.Value)
                    {
                        // Lock chests
                        foreach (var chest in location.objects.Values.OfType<Chest>())
                        {
                            var chestMutex = chest.mutex;
                            if (!chestMutex.IsLockHeld())
                                chestMutex.RequestLock();
                        }
                    }
                }

                // Slime Hutch Location
                else if (location.NameOrUniqueName == "SlimeHutch")
                {
                    var slimeBuilding = farm.buildings.FirstOrDefault(b => b.buildingType.Value == "Slime Hutch");

                    if (slimeBuilding != null && slimeBuilding.owner.Value != 0
                        && farmer.UniqueMultiplayerID != slimeBuilding.owner.Value)
                    {
                        // Lock chests
                        foreach (var chest in location.objects.Values.OfType<Chest>())
                        {
                            var chestMutex = chest.mutex;
                            if (!chestMutex.IsLockHeld())
                                chestMutex.RequestLock();
                        }
                    }
                }
            }
        }
    }
}