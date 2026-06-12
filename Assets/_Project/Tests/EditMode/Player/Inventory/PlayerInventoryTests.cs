using FactoryColony;
using NUnit.Framework;

namespace FactoryColony.Tests.EditMode.Player.Inventory
{
    public sealed class PlayerInventoryTests
    {
        [Test]
        public void PlayerInventory_AddAndRemove_UpdatesAmount()
        {
            PlayerInventoryModel inventory = new PlayerInventoryModel(10);

            inventory.Add(ResourceType.IronPlate, 3);
            bool removed = inventory.TryRemove(ResourceType.IronPlate, 2);

            Assert.IsTrue(removed);
            Assert.AreEqual(1, inventory.GetAmount(ResourceType.IronPlate));
            Assert.AreEqual(1, inventory.TotalAmount);
        }

        [Test]
        public void PlayerInventory_RejectsInvalidResources()
        {
            PlayerInventoryModel inventory = new PlayerInventoryModel();

            Assert.Throws<System.ArgumentException>(() => inventory.Add(ResourceType.None, 1));
            Assert.Throws<System.ArgumentException>(() => inventory.Add(ResourceType.IronPlate, 0));
            Assert.Throws<System.ArgumentException>(() => new PlayerInventoryModel(0));
        }

        [Test]
        public void PlayerInventory_TryAddFailsWhenCapacityExceeded()
        {
            PlayerInventoryModel inventory = new PlayerInventoryModel(2);

            bool added = inventory.TryAdd(ResourceType.IronPlate, 3);

            Assert.IsFalse(added);
            Assert.IsTrue(inventory.IsEmpty);
        }

        [Test]
        public void PlayerInventory_Clear_RemovesAllResources()
        {
            PlayerInventoryModel inventory = new PlayerInventoryModel();
            inventory.Add(ResourceType.IronPlate, 2);

            inventory.Clear();

            Assert.IsTrue(inventory.IsEmpty);
        }

        [Test]
        public void TransferService_TakesResourcesFromStorage()
        {
            PlayerInventoryModel playerInventory = new PlayerInventoryModel(10);
            BaseInventoryModel baseInventory = new BaseInventoryModel();
            PlayerInventoryTransferService service = new PlayerInventoryTransferService(playerInventory, baseInventory);
            BuildingModel storage = CreateBuilding(BuildingType.Storage);
            storage.Inventory.Add(ResourceType.IronPlate, 5);

            bool success = service.TryTakeAnyFromStorage(storage, 3, out string message);

            Assert.IsTrue(success, message);
            Assert.AreEqual(3, playerInventory.GetAmount(ResourceType.IronPlate));
            Assert.AreEqual(2, storage.Inventory.GetAmount(ResourceType.IronPlate));
        }

        [Test]
        public void TransferService_FailsForNonStorage()
        {
            PlayerInventoryModel playerInventory = new PlayerInventoryModel(10);
            BaseInventoryModel baseInventory = new BaseInventoryModel();
            PlayerInventoryTransferService service = new PlayerInventoryTransferService(playerInventory, baseInventory);
            BuildingModel conveyor = CreateBuilding(BuildingType.Conveyor);
            conveyor.Inventory.Add(ResourceType.IronPlate, 5);

            bool success = service.TryTakeAnyFromStorage(conveyor, 3, out _);

            Assert.IsFalse(success);
            Assert.AreEqual(0, playerInventory.GetAmount(ResourceType.IronPlate));
            Assert.AreEqual(5, conveyor.Inventory.GetAmount(ResourceType.IronPlate));
        }

        [Test]
        public void TransferService_FailsForEmptyStorage()
        {
            PlayerInventoryTransferService service = new PlayerInventoryTransferService(
                new PlayerInventoryModel(10),
                new BaseInventoryModel());

            bool success = service.TryTakeAnyFromStorage(CreateBuilding(BuildingType.Storage), 3, out _);

            Assert.IsFalse(success);
        }

        [Test]
        public void TransferService_FailsWhenPlayerInventoryIsFull()
        {
            PlayerInventoryModel playerInventory = new PlayerInventoryModel(2);
            playerInventory.Add(ResourceType.CopperWire, 2);
            PlayerInventoryTransferService service = new PlayerInventoryTransferService(playerInventory, new BaseInventoryModel());
            BuildingModel storage = CreateBuilding(BuildingType.Storage);
            storage.Inventory.Add(ResourceType.IronPlate, 5);

            bool success = service.TryTakeAnyFromStorage(storage, 3, out _);

            Assert.IsFalse(success);
            Assert.AreEqual(5, storage.Inventory.GetAmount(ResourceType.IronPlate));
        }

        [Test]
        public void TransferService_DepositAllToBase_MovesPlayerResources()
        {
            PlayerInventoryModel playerInventory = new PlayerInventoryModel(10);
            BaseInventoryModel baseInventory = new BaseInventoryModel();
            playerInventory.Add(ResourceType.IronPlate, 3);
            playerInventory.Add(ResourceType.CopperWire, 2);
            PlayerInventoryTransferService service = new PlayerInventoryTransferService(playerInventory, baseInventory);

            bool success = service.DepositAllToBase(out string message);

            Assert.IsTrue(success, message);
            Assert.IsTrue(playerInventory.IsEmpty);
            Assert.AreEqual(3, baseInventory.GetAmount(ResourceType.IronPlate));
            Assert.AreEqual(2, baseInventory.GetAmount(ResourceType.CopperWire));
        }

        private static BuildingModel CreateBuilding(BuildingType type)
        {
            BuildingDefinition definition = new BuildingDefinition(
                "test-" + type,
                type,
                "Test " + type,
                1,
                1,
                false,
                ResourceType.None,
                true);

            return new BuildingModel("building-" + type, definition, new GridPosition(0, 0), BuildingDirection.North);
        }
    }
}
