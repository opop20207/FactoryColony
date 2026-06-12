using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FactoryColony
{
    [CreateAssetMenu(menuName = "FactoryColony/Data/Definition Database", fileName = "FactoryDefinitionDatabase")]
    public sealed class DefinitionDatabase : ScriptableObject
    {
        [SerializeField] private List<BuildingDefinitionAsset> buildings = new List<BuildingDefinitionAsset>();
        [SerializeField] private List<RecipeDefinitionAsset> recipes = new List<RecipeDefinitionAsset>();
        [SerializeField] private List<ResourceDefinitionAsset> resources = new List<ResourceDefinitionAsset>();
        [SerializeField] private List<ResearchDefinitionAsset> researches = new List<ResearchDefinitionAsset>();

        public IReadOnlyDictionary<string, BuildingDefinition> CreateBuildingDefinitions()
        {
            Dictionary<string, BuildingDefinition> definitions = new Dictionary<string, BuildingDefinition>();

            foreach (BuildingDefinitionAsset asset in buildings)
            {
                if (asset == null)
                {
                    Debug.LogWarning("DefinitionDatabase skipped a null building asset.");
                    continue;
                }

                BuildingDefinition definition = asset.ToModel();
                AddUnique(definitions, definition.Id, definition, "building");
            }

            return definitions;
        }

        public IReadOnlyDictionary<string, RecipeModel> CreateRecipeDefinitions()
        {
            Dictionary<string, RecipeModel> definitions = new Dictionary<string, RecipeModel>();

            foreach (RecipeDefinitionAsset asset in recipes)
            {
                if (asset == null)
                {
                    Debug.LogWarning("DefinitionDatabase skipped a null recipe asset.");
                    continue;
                }

                RecipeModel recipe = asset.ToModel();
                AddUnique(definitions, recipe.Id, recipe, "recipe");
            }

            return definitions;
        }

        public IReadOnlyList<BuildingDefinition> CreateBuildMenuDefinitions()
        {
            return CreateBuildingDefinitions()
                .Values
                .OrderBy(definition => definition.Type)
                .ToArray();
        }

        public IReadOnlyDictionary<string, ResearchDefinition> CreateResearchDefinitions()
        {
            Dictionary<string, ResearchDefinition> definitions = new Dictionary<string, ResearchDefinition>();

            foreach (ResearchDefinitionAsset asset in researches)
            {
                if (asset == null)
                {
                    Debug.LogWarning("DefinitionDatabase skipped a null research asset.");
                    continue;
                }

                ResearchDefinition research = asset.ToModel();
                AddUnique(definitions, research.Id, research, "research");
            }

            return definitions;
        }

        public BuildingDefinition GetBuildingModel(string id)
        {
            IReadOnlyDictionary<string, BuildingDefinition> definitions = CreateBuildingDefinitions();
            return definitions.TryGetValue(id, out BuildingDefinition definition) ? definition : null;
        }

        public RecipeModel GetRecipeModel(string id)
        {
            IReadOnlyDictionary<string, RecipeModel> definitions = CreateRecipeDefinitions();
            return definitions.TryGetValue(id, out RecipeModel recipe) ? recipe : null;
        }

        public bool TryGetResourceColor(ResourceType type, out Color color)
        {
            foreach (ResourceDefinitionAsset resource in resources)
            {
                if (resource != null && resource.Type == type)
                {
                    color = resource.Color;
                    return true;
                }
            }

            color = Color.white;
            return false;
        }

        private static void AddUnique<T>(Dictionary<string, T> definitions, string id, T definition, string label)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new InvalidOperationException("DefinitionDatabase found an empty " + label + " id.");
            }

            if (definitions.ContainsKey(id))
            {
                throw new InvalidOperationException("DefinitionDatabase found duplicate " + label + " id '" + id + "'.");
            }

            definitions.Add(id, definition);
        }
    }
}
