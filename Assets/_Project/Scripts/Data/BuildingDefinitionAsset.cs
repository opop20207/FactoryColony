using System;
using System.Collections.Generic;
using UnityEngine;

namespace FactoryColony
{
    [CreateAssetMenu(menuName = "FactoryColony/Data/Building Definition", fileName = "BuildingDefinition")]
    public sealed class BuildingDefinitionAsset : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private BuildingType type;
        [SerializeField] private string displayName;
        [SerializeField] private int width = 1;
        [SerializeField] private int height = 1;
        [SerializeField] private bool requiresResourceNode;
        [SerializeField] private ResourceType requiredResourceType = ResourceType.None;
        [SerializeField] private bool isRotatable = true;
        [SerializeField] private int powerProduction;
        [SerializeField] private int powerConsumption;
        [SerializeField] private List<ResourceAmountData> buildCost = new List<ResourceAmountData>();

        public string Id
        {
            get { return id; }
        }

        public BuildingDefinition ToModel()
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new InvalidOperationException("Building definition id must not be empty.");
            }

            if (string.IsNullOrEmpty(displayName))
            {
                throw new InvalidOperationException("Building definition display name must not be empty.");
            }

            if (width < 1 || height < 1)
            {
                throw new InvalidOperationException("Building definition width and height must be at least one.");
            }

            if (powerProduction < 0 || powerConsumption < 0)
            {
                throw new InvalidOperationException("Building definition power values must not be negative.");
            }

            return new BuildingDefinition(
                id,
                type,
                displayName,
                width,
                height,
                requiresResourceNode,
                requiredResourceType,
                isRotatable,
                powerProduction,
                powerConsumption,
                CreateResourceDictionary(buildCost, "build cost"));
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
