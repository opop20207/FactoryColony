using System;
using System.Collections.Generic;
using System.Reflection;
using FactoryColony;
using NUnit.Framework;
using UnityEngine;

namespace FactoryColony.Tests.EditMode.Research
{
    public sealed class ResearchSystemTests
    {
        [Test]
        public void ResearchDefinition_RejectsInvalidValues()
        {
            Assert.Throws<ArgumentException>(() => new ResearchDefinition(string.Empty, "Name", "", null, null, null, null));
            Assert.Throws<ArgumentException>(() => new ResearchDefinition("id", string.Empty, "", null, null, null, null));
            Assert.Throws<ArgumentException>(() => new ResearchDefinition(
                "id",
                "Name",
                "",
                null,
                new Dictionary<ResourceType, int> { { ResourceType.None, 1 } },
                null,
                null));
        }

        [Test]
        public void ResearchDefinition_RemovesDuplicateIds()
        {
            ResearchDefinition definition = new ResearchDefinition(
                "id",
                "Name",
                "",
                new[] { "a", "a" },
                null,
                new[] { "building", "building" },
                new[] { "recipe", "recipe" });

            Assert.AreEqual(1, definition.PrerequisiteResearchIds.Count);
            Assert.AreEqual(1, definition.UnlockBuildingDefinitionIds.Count);
            Assert.AreEqual(1, definition.UnlockRecipeIds.Count);
        }

        [Test]
        public void ResearchStateModel_TracksRestoreAndClear()
        {
            ResearchStateModel state = new ResearchStateModel();

            Assert.IsTrue(state.TryComplete("a"));
            Assert.IsFalse(state.TryComplete("a"));
            Assert.IsTrue(state.IsCompleted("a"));

            state.RestoreCompleted(new[] { "b" });
            Assert.IsFalse(state.IsCompleted("a"));
            Assert.IsTrue(state.IsCompleted("b"));

            state.Clear();
            Assert.IsFalse(state.IsCompleted("b"));
        }

        [Test]
        public void ResearchSystem_ResearchConsumesCostAndUnlocks()
        {
            BaseInventoryModel inventory = new BaseInventoryModel();
            inventory.Add(ResourceType.IronPlate, 10);
            ResearchSystem system = new ResearchSystem(CreateDefinitions(), new ResearchStateModel(), inventory);

            Assert.IsTrue(system.CanResearch("basic"));
            Assert.IsTrue(system.TryResearch("basic", out string message), message);
            Assert.AreEqual(5, inventory.GetAmount(ResourceType.IronPlate));
            Assert.IsTrue(system.IsBuildingUnlocked("debug-Conveyor"));
            Assert.IsFalse(system.TryResearch("basic", out string ignored));
        }

        [Test]
        public void ResearchSystem_PrerequisiteLocksResearch()
        {
            BaseInventoryModel inventory = new BaseInventoryModel();
            inventory.Add(ResourceType.IronPlate, 20);
            ResearchSystem system = new ResearchSystem(CreateDefinitions(), new ResearchStateModel(), inventory);

            Assert.IsFalse(system.CanResearch("advanced"));
            Assert.AreEqual(1, system.GetLockedResearch().Count);

            system.TryResearch("basic", out string ignored);

            Assert.IsTrue(system.CanResearch("advanced"));
        }

        [Test]
        public void ResearchSystem_FailsWhenCostIsMissing()
        {
            ResearchSystem system = new ResearchSystem(CreateDefinitions(), new ResearchStateModel(), new BaseInventoryModel());

            Assert.IsFalse(system.TryResearch("basic", out string message));
            Assert.AreEqual("Insufficient resources.", message);
        }

        [Test]
        public void ResearchDefinitionAsset_ToModel_ConvertsData()
        {
            ResearchDefinitionAsset asset = ScriptableObject.CreateInstance<ResearchDefinitionAsset>();
            SetField(asset, "id", "asset_research");
            SetField(asset, "displayName", "Asset Research");
            SetField(asset, "cost", new List<ResourceAmountData>
            {
                new ResourceAmountData { resourceType = ResourceType.IronPlate, amount = 1 }
            });

            ResearchDefinition definition = asset.ToModel();

            Assert.AreEqual("asset_research", definition.Id);
            Assert.AreEqual(1, definition.Cost[ResourceType.IronPlate]);
        }

        [Test]
        public void ResearchDefinitionAsset_ToModel_RejectsInvalidCost()
        {
            ResearchDefinitionAsset asset = ScriptableObject.CreateInstance<ResearchDefinitionAsset>();
            SetField(asset, "id", "asset_research");
            SetField(asset, "displayName", "Asset Research");
            SetField(asset, "cost", new List<ResourceAmountData>
            {
                new ResourceAmountData { resourceType = ResourceType.None, amount = 1 }
            });

            Assert.Throws<InvalidOperationException>(() => asset.ToModel());
        }

        private static IReadOnlyDictionary<string, ResearchDefinition> CreateDefinitions()
        {
            return new Dictionary<string, ResearchDefinition>
            {
                {
                    "basic",
                    new ResearchDefinition(
                        "basic",
                        "Basic",
                        "",
                        null,
                        new Dictionary<ResourceType, int> { { ResourceType.IronPlate, 5 } },
                        new[] { "debug-Conveyor" },
                        new[] { "recipe-a" })
                },
                {
                    "advanced",
                    new ResearchDefinition(
                        "advanced",
                        "Advanced",
                        "",
                        new[] { "basic" },
                        new Dictionary<ResourceType, int> { { ResourceType.IronPlate, 5 } },
                        new[] { "debug-Assembler" },
                        null)
                }
            };
        }

        private static void SetField<T>(object target, string fieldName, T value)
        {
            FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            field.SetValue(target, value);
        }
    }
}
