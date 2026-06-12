using System;
using System.Collections.Generic;
using UnityEngine;

namespace FactoryColony
{
    [CreateAssetMenu(menuName = "FactoryColony/Data/Recipe Definition", fileName = "RecipeDefinition")]
    public sealed class RecipeDefinitionAsset : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [SerializeField] private BuildingType requiredBuildingType = BuildingType.Assembler;
        [SerializeField] private List<ResourceAmountData> inputs = new List<ResourceAmountData>();
        [SerializeField] private List<ResourceAmountData> outputs = new List<ResourceAmountData>();

        public string Id
        {
            get { return id; }
        }

        public RecipeModel ToModel()
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new InvalidOperationException("Recipe id must not be empty.");
            }

            if (string.IsNullOrEmpty(displayName))
            {
                throw new InvalidOperationException("Recipe display name must not be empty.");
            }

            IReadOnlyDictionary<ResourceType, int> inputDictionary = CreateResourceDictionary(inputs, "recipe inputs");
            IReadOnlyDictionary<ResourceType, int> outputDictionary = CreateResourceDictionary(outputs, "recipe outputs");

            if (inputDictionary.Count == 0)
            {
                throw new InvalidOperationException("Recipe inputs must not be empty.");
            }

            if (outputDictionary.Count == 0)
            {
                throw new InvalidOperationException("Recipe outputs must not be empty.");
            }

            return new RecipeModel(id, displayName, inputDictionary, outputDictionary, requiredBuildingType);
        }

        private static IReadOnlyDictionary<ResourceType, int> CreateResourceDictionary(
            IEnumerable<ResourceAmountData> resources,
            string label)
        {
            Dictionary<ResourceType, int> result = new Dictionary<ResourceType, int>();

            if (resources == null)
            {
                return result;
            }

            foreach (ResourceAmountData resource in resources)
            {
                if (resource == null)
                {
                    continue;
                }

                if (resource.resourceType == ResourceType.None)
                {
                    throw new InvalidOperationException("ResourceType.None is not valid in " + label + ".");
                }

                if (resource.amount <= 0)
                {
                    throw new InvalidOperationException("Resource amount must be greater than zero in " + label + ".");
                }

                result[resource.resourceType] = resource.amount;
            }

            return result;
        }
    }
}
