using System;
using System.Collections.Generic;
using System.Linq;

namespace FactoryColony
{
    public sealed class RecipeModel
    {
        public string Id { get; }
        public string DisplayName { get; }
        public IReadOnlyDictionary<ResourceType, int> Inputs { get; }
        public IReadOnlyDictionary<ResourceType, int> Outputs { get; }
        public BuildingType RequiredBuildingType { get; }

        public RecipeModel(
            string id,
            string displayName,
            IReadOnlyDictionary<ResourceType, int> inputs,
            IReadOnlyDictionary<ResourceType, int> outputs,
            BuildingType requiredBuildingType)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("Recipe id must not be empty.", nameof(id));
            }

            if (string.IsNullOrEmpty(displayName))
            {
                throw new ArgumentException("Recipe display name must not be empty.", nameof(displayName));
            }

            ValidateResources(inputs, nameof(inputs));
            ValidateResources(outputs, nameof(outputs));

            Id = id;
            DisplayName = displayName;
            Inputs = new Dictionary<ResourceType, int>(inputs);
            Outputs = new Dictionary<ResourceType, int>(outputs);
            RequiredBuildingType = requiredBuildingType;
        }

        private static void ValidateResources(IReadOnlyDictionary<ResourceType, int> resources, string parameterName)
        {
            if (resources == null || resources.Count == 0)
            {
                throw new ArgumentException("Recipe resources must not be empty.", parameterName);
            }

            if (resources.Any(resource => resource.Key == ResourceType.None || resource.Value <= 0))
            {
                throw new ArgumentException("Recipe resources must use non-empty resource types and positive amounts.", parameterName);
            }
        }
    }
}
