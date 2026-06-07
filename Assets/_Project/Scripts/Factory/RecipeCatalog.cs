using System.Collections.Generic;
using System.Linq;

namespace FactoryColony
{
    public sealed class RecipeCatalog
    {
        public const string IronPlateRecipeId = "iron_plate";
        public const string CopperWireRecipeId = "copper_wire";
        public const string GearRecipeId = "gear";
        public const string BasicCircuitRecipeId = "basic_circuit";

        private readonly Dictionary<string, RecipeModel> _recipes;

        public IReadOnlyCollection<RecipeModel> Recipes
        {
            get { return _recipes.Values.ToArray(); }
        }

        public RecipeCatalog(IEnumerable<RecipeModel> recipes)
        {
            _recipes = recipes.ToDictionary(recipe => recipe.Id);
        }

        public bool TryGetRecipe(string recipeId, out RecipeModel recipe)
        {
            if (string.IsNullOrEmpty(recipeId))
            {
                recipe = null;
                return false;
            }

            return _recipes.TryGetValue(recipeId, out recipe);
        }

        public static RecipeCatalog CreateDefault()
        {
            return new RecipeCatalog(new[]
            {
                new RecipeModel(
                    IronPlateRecipeId,
                    "Iron Plate",
                    new Dictionary<ResourceType, int> { { ResourceType.IronIngot, 1 } },
                    new Dictionary<ResourceType, int> { { ResourceType.IronPlate, 1 } },
                    BuildingType.Assembler),
                new RecipeModel(
                    CopperWireRecipeId,
                    "Copper Wire",
                    new Dictionary<ResourceType, int> { { ResourceType.CopperIngot, 1 } },
                    new Dictionary<ResourceType, int> { { ResourceType.CopperWire, 2 } },
                    BuildingType.Assembler),
                new RecipeModel(
                    GearRecipeId,
                    "Gear",
                    new Dictionary<ResourceType, int> { { ResourceType.IronPlate, 2 } },
                    new Dictionary<ResourceType, int> { { ResourceType.Gear, 1 } },
                    BuildingType.Assembler),
                new RecipeModel(
                    BasicCircuitRecipeId,
                    "Basic Circuit",
                    new Dictionary<ResourceType, int>
                    {
                        { ResourceType.IronPlate, 1 },
                        { ResourceType.CopperWire, 2 }
                    },
                    new Dictionary<ResourceType, int> { { ResourceType.BasicCircuit, 1 } },
                    BuildingType.Assembler)
            });
        }
    }
}
