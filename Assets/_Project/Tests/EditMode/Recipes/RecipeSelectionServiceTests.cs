using System.Collections.Generic;
using System.Linq;
using FactoryColony;
using NUnit.Framework;

namespace FactoryColony.Tests.EditMode.Recipes
{
    public sealed class RecipeSelectionServiceTests
    {
        [Test]
        public void GetAvailableRecipesFor_ReturnsMatchingBuildingType()
        {
            RecipeSelectionService service = new RecipeSelectionService(CreateRecipes(), null);
            BuildingModel assembler = CreateBuilding(BuildingType.Assembler);

            IReadOnlyList<RecipeModel> recipes = service.GetAvailableRecipesFor(assembler);

            Assert.IsTrue(recipes.Any(recipe => recipe.Id == RecipeCatalog.IronPlateRecipeId));
        }

        [Test]
        public void GetAvailableRecipesFor_ExcludesWrongBuildingType()
        {
            RecipeSelectionService service = new RecipeSelectionService(CreateRecipes(), null);
            BuildingModel smelter = CreateBuilding(BuildingType.Smelter);

            IReadOnlyList<RecipeModel> recipes = service.GetAvailableRecipesFor(smelter);

            Assert.IsFalse(recipes.Any(recipe => recipe.Id == RecipeCatalog.IronPlateRecipeId));
        }

        [Test]
        public void TrySetRecipe_SucceedsAndChangesSelectedRecipeId()
        {
            RecipeSelectionService service = new RecipeSelectionService(CreateRecipes(), null);
            BuildingModel assembler = CreateBuilding(BuildingType.Assembler);

            bool success = service.TrySetRecipe(assembler, RecipeCatalog.IronPlateRecipeId, out string message);

            Assert.IsTrue(success, message);
            Assert.AreEqual(RecipeCatalog.IronPlateRecipeId, assembler.SelectedRecipeId);
        }

        [Test]
        public void TrySetRecipe_FailsForMissingRecipe()
        {
            RecipeSelectionService service = new RecipeSelectionService(CreateRecipes(), null);
            BuildingModel assembler = CreateBuilding(BuildingType.Assembler);

            bool success = service.TrySetRecipe(assembler, "missing", out _);

            Assert.IsFalse(success);
            Assert.IsNull(assembler.SelectedRecipeId);
        }

        [Test]
        public void TrySetRecipe_FailsForWrongBuildingType()
        {
            RecipeSelectionService service = new RecipeSelectionService(CreateRecipes(), null);
            BuildingModel smelter = CreateBuilding(BuildingType.Smelter);

            bool success = service.TrySetRecipe(smelter, RecipeCatalog.IronPlateRecipeId, out _);

            Assert.IsFalse(success);
            Assert.IsNull(smelter.SelectedRecipeId);
        }

        [Test]
        public void ResearchSystem_BlocksLockedRecipe()
        {
            ResearchSystem researchSystem = new ResearchSystem(CreateResearchDefinitions(), new ResearchStateModel(), new BaseInventoryModel());
            RecipeSelectionService service = new RecipeSelectionService(CreateRecipes(), researchSystem);
            BuildingModel assembler = CreateBuilding(BuildingType.Assembler);

            bool success = service.TrySetRecipe(assembler, RecipeCatalog.IronPlateRecipeId, out string message);

            Assert.IsFalse(success);
            Assert.AreEqual("Recipe is locked.", message);
        }

        [Test]
        public void NullResearchSystem_AllowsRecipe()
        {
            RecipeSelectionService service = new RecipeSelectionService(CreateRecipes(), null);
            BuildingModel assembler = CreateBuilding(BuildingType.Assembler);

            bool success = service.TrySetRecipe(assembler, RecipeCatalog.IronPlateRecipeId, out string message);

            Assert.IsTrue(success, message);
        }

        [Test]
        public void GetSelectedRecipe_ReturnsSelectedRecipe()
        {
            RecipeSelectionService service = new RecipeSelectionService(CreateRecipes(), null);
            BuildingModel assembler = CreateBuilding(BuildingType.Assembler);
            assembler.SelectedRecipeId = RecipeCatalog.CopperWireRecipeId;

            RecipeModel selectedRecipe = service.GetSelectedRecipe(assembler);

            Assert.IsNotNull(selectedRecipe);
            Assert.AreEqual(RecipeCatalog.CopperWireRecipeId, selectedRecipe.Id);
        }

        private static IReadOnlyDictionary<string, RecipeModel> CreateRecipes()
        {
            return RecipeCatalog.CreateDefault().Recipes.ToDictionary(recipe => recipe.Id);
        }

        private static IReadOnlyDictionary<string, ResearchDefinition> CreateResearchDefinitions()
        {
            return new Dictionary<string, ResearchDefinition>
            {
                {
                    "locked_recipe_research",
                    new ResearchDefinition(
                        "locked_recipe_research",
                        "Locked Recipe Research",
                        "",
                        null,
                        new Dictionary<ResourceType, int> { { ResourceType.IronPlate, 1 } },
                        null,
                        new[] { RecipeCatalog.IronPlateRecipeId })
                }
            };
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
