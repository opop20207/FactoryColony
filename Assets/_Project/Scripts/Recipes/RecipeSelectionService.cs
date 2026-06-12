using System;
using System.Collections.Generic;
using System.Linq;

namespace FactoryColony
{
    public sealed class RecipeSelectionService
    {
        private readonly IReadOnlyDictionary<string, RecipeModel> _recipes;
        private readonly ResearchSystem _researchSystem;

        public RecipeSelectionService(IReadOnlyDictionary<string, RecipeModel> recipes, ResearchSystem researchSystem)
        {
            _recipes = recipes ?? new Dictionary<string, RecipeModel>();
            _researchSystem = researchSystem;
        }

        public IReadOnlyList<RecipeModel> GetAvailableRecipesFor(BuildingModel building)
        {
            if (building == null)
            {
                return Array.Empty<RecipeModel>();
            }

            return _recipes.Values
                .Where(recipe => CanUseRecipe(building, recipe))
                .OrderBy(recipe => recipe.Id)
                .ToArray();
        }

        public bool CanUseRecipe(BuildingModel building, RecipeModel recipe)
        {
            if (building == null || recipe == null)
            {
                return false;
            }

            if (recipe.RequiredBuildingType != building.Definition.Type)
            {
                return false;
            }

            return _researchSystem == null || _researchSystem.IsRecipeUnlocked(recipe.Id);
        }

        public bool TrySetRecipe(BuildingModel building, string recipeId, out string message)
        {
            if (building == null)
            {
                message = "No building selected.";
                return false;
            }

            if (string.IsNullOrEmpty(recipeId))
            {
                message = "No recipe selected.";
                return false;
            }

            if (!_recipes.TryGetValue(recipeId, out RecipeModel recipe))
            {
                message = "Unknown recipe.";
                return false;
            }

            if (recipe.RequiredBuildingType != building.Definition.Type)
            {
                message = "Recipe cannot be used by " + building.Definition.Type + ".";
                return false;
            }

            if (_researchSystem != null && !_researchSystem.IsRecipeUnlocked(recipeId))
            {
                message = "Recipe is locked.";
                return false;
            }

            building.SelectedRecipeId = recipeId;
            message = "Selected " + recipe.DisplayName + ".";
            return true;
        }

        public RecipeModel GetSelectedRecipe(BuildingModel building)
        {
            if (building == null || string.IsNullOrEmpty(building.SelectedRecipeId))
            {
                return null;
            }

            return _recipes.TryGetValue(building.SelectedRecipeId, out RecipeModel recipe)
                ? recipe
                : null;
        }
    }
}
