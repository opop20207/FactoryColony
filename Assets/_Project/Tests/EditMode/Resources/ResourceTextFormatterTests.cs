using FactoryColony;
using NUnit.Framework;

namespace FactoryColony.Tests.EditMode.Resources
{
    public sealed class ResourceTextFormatterTests
    {
        [Test]
        public void FormatInventory_ReturnsEmptyWhenInventoryHasNoItems()
        {
            InventoryModel inventory = new InventoryModel();

            string text = ResourceTextFormatter.FormatInventory(inventory);

            Assert.AreEqual("Empty", text);
        }

        [Test]
        public void FormatInventory_ReturnsResourceAmounts()
        {
            InventoryModel inventory = new InventoryModel();
            inventory.Add(ResourceType.IronOre, 3);
            inventory.Add(ResourceType.IronIngot, 2);

            string text = ResourceTextFormatter.FormatInventory(inventory);

            Assert.AreEqual("IronOre x3, IronIngot x2", text);
        }

        [Test]
        public void FormatStacks_IgnoresNoneAndZeroAmounts()
        {
            ResourceStack[] stacks =
            {
                new ResourceStack(ResourceType.None, 0),
                new ResourceStack(ResourceType.CopperWire, 4)
            };

            string text = ResourceTextFormatter.FormatStacks(stacks);

            Assert.AreEqual("CopperWire x4", text);
        }
    }
}
