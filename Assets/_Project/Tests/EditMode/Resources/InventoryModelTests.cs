using System;
using System.Linq;
using FactoryColony;
using NUnit.Framework;

namespace FactoryColony.Tests.EditMode.Resources
{
    public sealed class InventoryModelTests
    {
        [Test]
        public void AddAndGetAmount_StoresResourceAmount()
        {
            InventoryModel inventory = new InventoryModel();

            inventory.Add(ResourceType.IronOre, 5);

            Assert.AreEqual(5, inventory.GetAmount(ResourceType.IronOre));
        }

        [Test]
        public void TryRemove_RemovesWhenEnoughItemsExist()
        {
            InventoryModel inventory = new InventoryModel();
            inventory.Add(ResourceType.CopperOre, 5);

            bool removed = inventory.TryRemove(ResourceType.CopperOre, 3);

            Assert.IsTrue(removed);
            Assert.AreEqual(2, inventory.GetAmount(ResourceType.CopperOre));
        }

        [Test]
        public void TryRemove_ReturnsFalseWhenNotEnoughItemsExist()
        {
            InventoryModel inventory = new InventoryModel();
            inventory.Add(ResourceType.Coal, 1);

            bool removed = inventory.TryRemove(ResourceType.Coal, 2);

            Assert.IsFalse(removed);
            Assert.AreEqual(1, inventory.GetAmount(ResourceType.Coal));
        }

        [Test]
        public void Add_RejectsNoneType()
        {
            InventoryModel inventory = new InventoryModel();

            Assert.Throws<ArgumentException>(() => inventory.Add(ResourceType.None, 1));
        }

        [Test]
        public void IsEmpty_ReturnsTrueOnlyWhenInventoryHasNoItems()
        {
            InventoryModel inventory = new InventoryModel();

            Assert.IsTrue(inventory.IsEmpty);

            inventory.Add(ResourceType.IronOre, 1);

            Assert.IsFalse(inventory.IsEmpty);
        }

        [Test]
        public void TotalAmount_ReturnsSumOfAllResourceAmounts()
        {
            InventoryModel inventory = new InventoryModel();

            inventory.Add(ResourceType.IronOre, 2);
            inventory.Add(ResourceType.CopperOre, 3);

            Assert.AreEqual(5, inventory.TotalAmount);
        }

        [Test]
        public void GetStacks_ReturnsOnlyNonEmptyResourceStacks()
        {
            InventoryModel inventory = new InventoryModel();
            inventory.Add(ResourceType.IronOre, 2);
            inventory.Add(ResourceType.CopperOre, 1);
            inventory.TryRemove(ResourceType.CopperOre, 1);

            ResourceStack[] stacks = inventory.GetStacks().ToArray();

            Assert.AreEqual(1, stacks.Length);
            Assert.AreEqual(ResourceType.IronOre, stacks[0].Type);
            Assert.AreEqual(2, stacks[0].Amount);
            Assert.IsFalse(stacks.Any(stack => stack.Type == ResourceType.None));
        }

        [Test]
        public void TryTakeOne_RemovesAndReturnsOneResource()
        {
            InventoryModel inventory = new InventoryModel();
            inventory.Add(ResourceType.CopperOre, 1);
            inventory.Add(ResourceType.IronOre, 2);

            bool wasTaken = inventory.TryTakeOne(out ResourceStack stack);

            Assert.IsTrue(wasTaken);
            Assert.AreEqual(ResourceType.IronOre, stack.Type);
            Assert.AreEqual(1, stack.Amount);
            Assert.AreEqual(1, inventory.GetAmount(ResourceType.IronOre));
            Assert.AreEqual(1, inventory.GetAmount(ResourceType.CopperOre));
        }

        [Test]
        public void TryPeekOne_ReturnsOneResourceWithoutRemovingIt()
        {
            InventoryModel inventory = new InventoryModel();
            inventory.Add(ResourceType.CopperOre, 1);
            inventory.Add(ResourceType.IronOre, 2);

            bool wasPeeked = inventory.TryPeekOne(out ResourceStack stack);

            Assert.IsTrue(wasPeeked);
            Assert.AreEqual(ResourceType.IronOre, stack.Type);
            Assert.AreEqual(1, stack.Amount);
            Assert.AreEqual(2, inventory.GetAmount(ResourceType.IronOre));
        }

        [Test]
        public void TryTakeOne_WithPredicate_RemovesFirstMatchingResource()
        {
            InventoryModel inventory = new InventoryModel();
            inventory.Add(ResourceType.IronOre, 1);
            inventory.Add(ResourceType.CopperOre, 2);

            bool wasTaken = inventory.TryTakeOne(type => type == ResourceType.CopperOre, out ResourceStack stack);

            Assert.IsTrue(wasTaken);
            Assert.AreEqual(ResourceType.CopperOre, stack.Type);
            Assert.AreEqual(1, stack.Amount);
            Assert.AreEqual(1, inventory.GetAmount(ResourceType.IronOre));
            Assert.AreEqual(1, inventory.GetAmount(ResourceType.CopperOre));
        }

        [Test]
        public void TryTakeOne_ReturnsFalseWhenInventoryIsEmpty()
        {
            InventoryModel inventory = new InventoryModel();

            bool wasTaken = inventory.TryTakeOne(out ResourceStack stack);

            Assert.IsFalse(wasTaken);
            Assert.IsNull(stack);
        }

        [Test]
        public void HasAll_ReturnsTrueWhenEnoughResourcesExist()
        {
            InventoryModel inventory = new InventoryModel();
            inventory.Add(ResourceType.IronPlate, 1);
            inventory.Add(ResourceType.CopperWire, 2);

            Assert.IsTrue(inventory.HasAll(Requirements()));
        }

        [Test]
        public void HasAll_ReturnsFalseWhenAnyResourceIsMissing()
        {
            InventoryModel inventory = new InventoryModel();
            inventory.Add(ResourceType.IronPlate, 1);

            Assert.IsFalse(inventory.HasAll(Requirements()));
        }

        [Test]
        public void TryRemoveAll_RemovesOnlyWhenAllResourcesExist()
        {
            InventoryModel inventory = new InventoryModel();
            inventory.Add(ResourceType.IronPlate, 1);
            inventory.Add(ResourceType.CopperWire, 2);

            bool removed = inventory.TryRemoveAll(Requirements());

            Assert.IsTrue(removed);
            Assert.AreEqual(0, inventory.GetAmount(ResourceType.IronPlate));
            Assert.AreEqual(0, inventory.GetAmount(ResourceType.CopperWire));
        }

        [Test]
        public void TryRemoveAll_DoesNotRemoveAnythingWhenAnyResourceIsMissing()
        {
            InventoryModel inventory = new InventoryModel();
            inventory.Add(ResourceType.IronPlate, 1);

            bool removed = inventory.TryRemoveAll(Requirements());

            Assert.IsFalse(removed);
            Assert.AreEqual(1, inventory.GetAmount(ResourceType.IronPlate));
        }

        [Test]
        public void AddAll_AddsMultipleResources()
        {
            InventoryModel inventory = new InventoryModel();

            inventory.AddAll(Requirements());

            Assert.AreEqual(1, inventory.GetAmount(ResourceType.IronPlate));
            Assert.AreEqual(2, inventory.GetAmount(ResourceType.CopperWire));
        }

        [Test]
        public void RemoveAll_RemovesAllOfResourceTypeAndReturnsAmount()
        {
            InventoryModel inventory = new InventoryModel();
            inventory.Add(ResourceType.IronPlate, 4);
            inventory.Add(ResourceType.CopperWire, 2);

            int removedAmount = inventory.RemoveAll(ResourceType.IronPlate);

            Assert.AreEqual(4, removedAmount);
            Assert.AreEqual(0, inventory.GetAmount(ResourceType.IronPlate));
            Assert.AreEqual(2, inventory.GetAmount(ResourceType.CopperWire));
        }

        [Test]
        public void RemoveAll_ReturnsZeroWhenResourceDoesNotExist()
        {
            InventoryModel inventory = new InventoryModel();

            int removedAmount = inventory.RemoveAll(ResourceType.IronPlate);

            Assert.AreEqual(0, removedAmount);
        }

        [Test]
        public void ToDictionary_ReturnsCopy()
        {
            InventoryModel inventory = new InventoryModel();
            inventory.Add(ResourceType.IronPlate, 4);

            System.Collections.Generic.IReadOnlyDictionary<ResourceType, int> snapshot = inventory.ToDictionary();
            ((System.Collections.Generic.Dictionary<ResourceType, int>)snapshot)[ResourceType.IronPlate] = 99;

            Assert.AreEqual(4, inventory.GetAmount(ResourceType.IronPlate));
        }

        [Test]
        public void Clear_RemovesAllResources()
        {
            InventoryModel inventory = new InventoryModel();
            inventory.Add(ResourceType.IronPlate, 4);
            inventory.Add(ResourceType.CopperWire, 2);

            inventory.Clear();

            Assert.IsTrue(inventory.IsEmpty);
            Assert.AreEqual(0, inventory.GetAmount(ResourceType.IronPlate));
            Assert.AreEqual(0, inventory.GetAmount(ResourceType.CopperWire));
        }

        private static System.Collections.Generic.Dictionary<ResourceType, int> Requirements()
        {
            return new System.Collections.Generic.Dictionary<ResourceType, int>
            {
                { ResourceType.IronPlate, 1 },
                { ResourceType.CopperWire, 2 }
            };
        }
    }
}
