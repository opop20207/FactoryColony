using System.Linq;
using FactoryColony;
using NUnit.Framework;

namespace FactoryColony.Tests.EditMode.Resources
{
    public sealed class ResourceTokenDisplaySelectorTests
    {
        [Test]
        public void SelectTokens_ExcludesNoneAndZeroAmount()
        {
            ResourceStack[] stacks =
            {
                new ResourceStack(ResourceType.None, 0),
                new ResourceStack(ResourceType.IronOre, 2)
            };

            ResourceTokenDisplayData[] tokens = ResourceTokenDisplaySelector.SelectTokens(stacks).ToArray();

            Assert.AreEqual(1, tokens.Length);
            Assert.AreEqual(ResourceType.IronOre, tokens[0].Type);
        }

        [Test]
        public void SelectTokens_LimitsTokenCount()
        {
            ResourceStack[] stacks =
            {
                new ResourceStack(ResourceType.IronOre, 1),
                new ResourceStack(ResourceType.CopperOre, 1),
                new ResourceStack(ResourceType.Coal, 1)
            };

            ResourceTokenDisplayData[] tokens = ResourceTokenDisplaySelector.SelectTokens(stacks, 2).ToArray();

            Assert.AreEqual(2, tokens.Length);
        }

        [Test]
        public void SelectTokens_ReturnsEmptyWhenMaxTokenCountIsZero()
        {
            ResourceStack[] stacks =
            {
                new ResourceStack(ResourceType.IronOre, 1)
            };

            Assert.AreEqual(0, ResourceTokenDisplaySelector.SelectTokens(stacks, 0).Count);
        }
    }
}
