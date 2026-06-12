using System;
using System.Collections.Generic;
using UnityEngine;

namespace FactoryColony
{
    [CreateAssetMenu(menuName = "FactoryColony/Data/Research Definition", fileName = "ResearchDefinition")]
    public sealed class ResearchDefinitionAsset : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [SerializeField] private string description;
        [SerializeField] private List<string> prerequisiteResearchIds = new List<string>();
        [SerializeField] private List<ResourceAmountData> cost = new List<ResourceAmountData>();
        [SerializeField] private List<string> unlockBuildingDefinitionIds = new List<string>();
        [SerializeField] private List<string> unlockRecipeIds = new List<string>();

        public ResearchDefinition ToModel()
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new InvalidOperationException("Research id must not be empty.");
            }

            if (string.IsNullOrEmpty(displayName))
            {
                throw new InvalidOperationException("Research display name must not be empty.");
            }

            return new ResearchDefinition(
                id,
                displayName,
                description,
                prerequisiteResearchIds,
                CreateCost(),
                unlockBuildingDefinitionIds,
                unlockRecipeIds);
        }

        private IReadOnlyDictionary<ResourceType, int> CreateCost()
        {
            Dictionary<ResourceType, int> result = new Dictionary<ResourceType, int>();

            foreach (ResourceAmountData item in cost)
            {
                if (item == null)
                {
                    continue;
                }

                if (item.resourceType == ResourceType.None || item.amount <= 0)
                {
                    throw new InvalidOperationException("Research cost must use valid resources and positive amounts.");
                }

                result[item.resourceType] = item.amount;
            }

            return result;
        }
    }
}
