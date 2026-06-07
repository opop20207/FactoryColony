using System;
using System.Collections.Generic;
using FactoryColony;
using NUnit.Framework;

namespace FactoryColony.Tests.EditMode.Factory
{
    public sealed class RecipeModelTests
    {
        [Test]
        public void Constructor_CreatesValidRecipe()
        {
            RecipeModel recipe = CreateRecipe();

            Assert.AreEqual("test", recipe.Id);
            Assert.AreEqual(BuildingType.Assembler, recipe.RequiredBuildingType);
        }

        [Test]
        public void Constructor_RejectsEmptyId()
        {
            Assert.Throws<ArgumentException>(() => new RecipeModel(
                string.Empty,
                "Test",
                Inputs(),
                Outputs(),
                BuildingType.Assembler));
        }

        [Test]
        public void Constructor_RejectsEmptyInputs()
        {
            Assert.Throws<ArgumentException>(() => new RecipeModel(
                "test",
                "Test",
                new Dictionary<ResourceType, int>(),
                Outputs(),
                BuildingType.Assembler));
        }

        [Test]
        public void Constructor_RejectsEmptyOutputs()
        {
            Assert.Throws<ArgumentException>(() => new RecipeModel(
                "test",
                "Test",
                Inputs(),
                new Dictionary<ResourceType, int>(),
                BuildingType.Assembler));
        }

        [Test]
        public void Constructor_RejectsNoneResource()
        {
            Assert.Throws<ArgumentException>(() => new RecipeModel(
                "test",
                "Test",
                new Dictionary<ResourceType, int> { { ResourceType.None, 1 } },
                Outputs(),
                BuildingType.Assembler));
        }

        [Test]
        public void Constructor_RejectsNonPositiveAmount()
        {
            Assert.Throws<ArgumentException>(() => new RecipeModel(
                "test",
                "Test",
                new Dictionary<ResourceType, int> { { ResourceType.IronIngot, 0 } },
                Outputs(),
                BuildingType.Assembler));
        }

        private static RecipeModel CreateRecipe()
        {
            return new RecipeModel("test", "Test", Inputs(), Outputs(), BuildingType.Assembler);
        }

        private static Dictionary<ResourceType, int> Inputs()
        {
            return new Dictionary<ResourceType, int> { { ResourceType.IronIngot, 1 } };
        }

        private static Dictionary<ResourceType, int> Outputs()
        {
            return new Dictionary<ResourceType, int> { { ResourceType.IronPlate, 1 } };
        }
    }
}
