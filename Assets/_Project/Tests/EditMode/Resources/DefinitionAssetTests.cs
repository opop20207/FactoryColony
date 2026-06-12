using System;
using System.Collections.Generic;
using System.Reflection;
using FactoryColony;
using NUnit.Framework;
using UnityEngine;

namespace FactoryColony.Tests.EditMode.Resources
{
    public sealed class DefinitionAssetTests
    {
        [Test]
        public void BuildingDefinitionAsset_ToModel_CreatesBuildingDefinition()
        {
            BuildingDefinitionAsset asset = CreateBuildingAsset();

            BuildingDefinition definition = asset.ToModel();

            Assert.AreEqual("debug-Miner", definition.Id);
            Assert.AreEqual(BuildingType.Miner, definition.Type);
            Assert.AreEqual(5, definition.BuildCost[ResourceType.IronPlate]);
        }

        [Test]
        public void BuildingDefinitionAsset_ToModel_RejectsInvalidSize()
        {
            BuildingDefinitionAsset asset = CreateBuildingAsset();
            SetField(asset, "width", 0);

            Assert.Throws<InvalidOperationException>(() => asset.ToModel());
        }

        [Test]
        public void BuildingDefinitionAsset_ToModel_RejectsNegativePower()
        {
            BuildingDefinitionAsset asset = CreateBuildingAsset();
            SetField(asset, "powerConsumption", -1);

            Assert.Throws<InvalidOperationException>(() => asset.ToModel());
        }

        [Test]
        public void BuildingDefinitionAsset_ToModel_RejectsInvalidCost()
        {
            BuildingDefinitionAsset asset = CreateBuildingAsset();
            SetField(asset, "buildCost", new List<ResourceAmountData>
            {
                new ResourceAmountData { resourceType = ResourceType.None, amount = 1 }
            });

            Assert.Throws<InvalidOperationException>(() => asset.ToModel());
        }

        [Test]
        public void RecipeDefinitionAsset_ToModel_CreatesRecipeModel()
        {
            RecipeDefinitionAsset asset = CreateRecipeAsset();

            RecipeModel recipe = asset.ToModel();

            Assert.AreEqual(RecipeCatalog.IronPlateRecipeId, recipe.Id);
            Assert.AreEqual(1, recipe.Inputs[ResourceType.IronIngot]);
            Assert.AreEqual(1, recipe.Outputs[ResourceType.IronPlate]);
        }

        [Test]
        public void RecipeDefinitionAsset_ToModel_RejectsEmptyId()
        {
            RecipeDefinitionAsset asset = CreateRecipeAsset();
            SetField(asset, "id", string.Empty);

            Assert.Throws<InvalidOperationException>(() => asset.ToModel());
        }

        [Test]
        public void RecipeDefinitionAsset_ToModel_RejectsEmptyInputs()
        {
            RecipeDefinitionAsset asset = CreateRecipeAsset();
            SetField(asset, "inputs", new List<ResourceAmountData>());

            Assert.Throws<InvalidOperationException>(() => asset.ToModel());
        }

        [Test]
        public void RecipeDefinitionAsset_ToModel_RejectsInvalidOutputAmount()
        {
            RecipeDefinitionAsset asset = CreateRecipeAsset();
            SetField(asset, "outputs", new List<ResourceAmountData>
            {
                new ResourceAmountData { resourceType = ResourceType.IronPlate, amount = 0 }
            });

            Assert.Throws<InvalidOperationException>(() => asset.ToModel());
        }

        [Test]
        public void DefinitionDatabase_CreateBuildingDefinitions_SkipsNullAssets()
        {
            DefinitionDatabase database = ScriptableObject.CreateInstance<DefinitionDatabase>();
            SetField(database, "buildings", new List<BuildingDefinitionAsset>
            {
                null,
                CreateBuildingAsset()
            });

            IReadOnlyDictionary<string, BuildingDefinition> definitions = database.CreateBuildingDefinitions();

            Assert.AreEqual(1, definitions.Count);
            Assert.IsTrue(definitions.ContainsKey("debug-Miner"));
        }

        [Test]
        public void DefinitionDatabase_CreateBuildingDefinitions_RejectsDuplicateIds()
        {
            DefinitionDatabase database = ScriptableObject.CreateInstance<DefinitionDatabase>();
            SetField(database, "buildings", new List<BuildingDefinitionAsset>
            {
                CreateBuildingAsset(),
                CreateBuildingAsset()
            });

            Assert.Throws<InvalidOperationException>(() => database.CreateBuildingDefinitions());
        }

        [Test]
        public void DefinitionDatabase_CreateRecipeDefinitions_CreatesRecipes()
        {
            DefinitionDatabase database = ScriptableObject.CreateInstance<DefinitionDatabase>();
            SetField(database, "recipes", new List<RecipeDefinitionAsset> { CreateRecipeAsset() });

            IReadOnlyDictionary<string, RecipeModel> recipes = database.CreateRecipeDefinitions();

            Assert.AreEqual(1, recipes.Count);
            Assert.IsTrue(recipes.ContainsKey(RecipeCatalog.IronPlateRecipeId));
        }

        [Test]
        public void DefinitionDatabase_CreateBuildMenuDefinitions_ReturnsBuildingModels()
        {
            DefinitionDatabase database = ScriptableObject.CreateInstance<DefinitionDatabase>();
            SetField(database, "buildings", new List<BuildingDefinitionAsset> { CreateBuildingAsset() });

            IReadOnlyList<BuildingDefinition> definitions = database.CreateBuildMenuDefinitions();

            Assert.AreEqual(1, definitions.Count);
            Assert.AreEqual(BuildingType.Miner, definitions[0].Type);
        }

        private static BuildingDefinitionAsset CreateBuildingAsset()
        {
            BuildingDefinitionAsset asset = ScriptableObject.CreateInstance<BuildingDefinitionAsset>();
            SetField(asset, "id", "debug-Miner");
            SetField(asset, "type", BuildingType.Miner);
            SetField(asset, "displayName", "Debug Miner");
            SetField(asset, "width", 1);
            SetField(asset, "height", 1);
            SetField(asset, "requiresResourceNode", true);
            SetField(asset, "requiredResourceType", ResourceType.None);
            SetField(asset, "isRotatable", true);
            SetField(asset, "powerProduction", 0);
            SetField(asset, "powerConsumption", 2);
            SetField(asset, "buildCost", new List<ResourceAmountData>
            {
                new ResourceAmountData { resourceType = ResourceType.IronPlate, amount = 5 }
            });
            return asset;
        }

        private static RecipeDefinitionAsset CreateRecipeAsset()
        {
            RecipeDefinitionAsset asset = ScriptableObject.CreateInstance<RecipeDefinitionAsset>();
            SetField(asset, "id", RecipeCatalog.IronPlateRecipeId);
            SetField(asset, "displayName", "Iron Plate");
            SetField(asset, "requiredBuildingType", BuildingType.Assembler);
            SetField(asset, "inputs", new List<ResourceAmountData>
            {
                new ResourceAmountData { resourceType = ResourceType.IronIngot, amount = 1 }
            });
            SetField(asset, "outputs", new List<ResourceAmountData>
            {
                new ResourceAmountData { resourceType = ResourceType.IronPlate, amount = 1 }
            });
            return asset;
        }

        private static void SetField<T>(object target, string fieldName, T value)
        {
            FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            field.SetValue(target, value);
        }
    }
}
