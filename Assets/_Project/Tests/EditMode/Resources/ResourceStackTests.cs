using System;
using FactoryColony;
using NUnit.Framework;

namespace FactoryColony.Tests.EditMode.Resources
{
    public sealed class ResourceStackTests
    {
        [Test]
        public void Constructor_CreatesValidStack()
        {
            ResourceStack stack = new ResourceStack(ResourceType.IronOre, 3);

            Assert.AreEqual(ResourceType.IronOre, stack.Type);
            Assert.AreEqual(3, stack.Amount);
            Assert.IsFalse(stack.IsEmpty);
        }

        [Test]
        public void Constructor_RejectsNegativeAmount()
        {
            Assert.Throws<ArgumentException>(() => new ResourceStack(ResourceType.IronOre, -1));
        }

        [Test]
        public void Constructor_RejectsNoneTypeWithPositiveAmount()
        {
            Assert.Throws<ArgumentException>(() => new ResourceStack(ResourceType.None, 1));
        }

        [Test]
        public void AddAndRemove_UpdateAmount()
        {
            ResourceStack stack = new ResourceStack(ResourceType.IronOre, 2);

            stack.Add(3);
            bool removed = stack.Remove(4);

            Assert.IsTrue(removed);
            Assert.AreEqual(1, stack.Amount);
        }
    }
}
