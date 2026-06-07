using System.Collections.Generic;
using FactoryColony;
using NUnit.Framework;

namespace FactoryColony.Tests.EditMode.Resources
{
    public sealed class BaseInventoryModelTests
    {
        [Test]
        public void CanAfford_ReturnsTrueWhenEnoughResourcesExist()
        {
            BaseInventoryModel inventory = CreateInventory(10, 5);

            Assert.IsTrue(inventory.CanAfford(Cost(4, 2)));
        }

        [Test]
        public void CanAfford_ReturnsFalseWhenAnyResourceIsMissing()
        {
            BaseInventoryModel inventory = CreateInventory(10, 1);

            Assert.IsFalse(inventory.CanAfford(Cost(4, 2)));
        }

        [Test]
        public void TrySpend_RemovesResourcesOnlyWhenEnoughResourcesExist()
        {
            BaseInventoryModel inventory = CreateInventory(10, 5);

            bool wasSpent = inventory.TrySpend(Cost(4, 2));

            Assert.IsTrue(wasSpent);
            Assert.AreEqual(6, inventory.GetAmount(ResourceType.IronPlate));
            Assert.AreEqual(3, inventory.GetAmount(ResourceType.CopperWire));
        }

        [Test]
        public void TrySpend_DoesNotRemoveAnythingWhenAnyResourceIsMissing()
        {
            BaseInventoryModel inventory = CreateInventory(10, 1);

            bool wasSpent = inventory.TrySpend(Cost(4, 2));

            Assert.IsFalse(wasSpent);
            Assert.AreEqual(10, inventory.GetAmount(ResourceType.IronPlate));
            Assert.AreEqual(1, inventory.GetAmount(ResourceType.CopperWire));
        }

        [Test]
        public void EmptyCost_IsAffordableAndSpendable()
        {
            BaseInventoryModel inventory = new BaseInventoryModel();
            Dictionary<ResourceType, int> emptyCost = new Dictionary<ResourceType, int>();

            Assert.IsTrue(inventory.CanAfford(emptyCost));
            Assert.IsTrue(inventory.TrySpend(emptyCost));
        }

        [Test]
        public void AddAndGetAmount_StoresResources()
        {
            BaseInventoryModel inventory = new BaseInventoryModel();

            inventory.Add(ResourceType.Gear, 3);

            Assert.AreEqual(3, inventory.GetAmount(ResourceType.Gear));
        }

        private static BaseInventoryModel CreateInventory(int ironPlate, int copperWire)
        {
            BaseInventoryModel inventory = new BaseInventoryModel();
            inventory.Add(ResourceType.IronPlate, ironPlate);
            inventory.Add(ResourceType.CopperWire, copperWire);
            return inventory;
        }

        private static Dictionary<ResourceType, int> Cost(int ironPlate, int copperWire)
        {
            return new Dictionary<ResourceType, int>
            {
                { ResourceType.IronPlate, ironPlate },
                { ResourceType.CopperWire, copperWire }
            };
        }
    }
}
