using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FactoryColony
{
    public sealed class ResearchDefinition
    {
        public string Id { get; }
        public string DisplayName { get; }
        public string Description { get; }
        public IReadOnlyList<string> PrerequisiteResearchIds { get; }
        public IReadOnlyDictionary<ResourceType, int> Cost { get; }
        public IReadOnlyList<string> UnlockBuildingDefinitionIds { get; }
        public IReadOnlyList<string> UnlockRecipeIds { get; }

        public ResearchDefinition(
            string id,
            string displayName,
            string description,
            IEnumerable<string> prerequisiteResearchIds,
            IReadOnlyDictionary<ResourceType, int> cost,
            IEnumerable<string> unlockBuildingDefinitionIds,
            IEnumerable<string> unlockRecipeIds)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("Research id must not be empty.", nameof(id));
            }

            if (string.IsNullOrEmpty(displayName))
            {
                throw new ArgumentException("Research display name must not be empty.", nameof(displayName));
            }

            Id = id;
            DisplayName = displayName;
            Description = description ?? string.Empty;
            PrerequisiteResearchIds = CreateUniqueList(prerequisiteResearchIds);
            Cost = CreateCost(cost);
            UnlockBuildingDefinitionIds = CreateUniqueList(unlockBuildingDefinitionIds);
            UnlockRecipeIds = CreateUniqueList(unlockRecipeIds);
        }

        private static IReadOnlyList<string> CreateUniqueList(IEnumerable<string> ids)
        {
            List<string> result = new List<string>();
            HashSet<string> seen = new HashSet<string>();

            if (ids == null)
            {
                return result.AsReadOnly();
            }

            foreach (string id in ids)
            {
                if (string.IsNullOrEmpty(id) || !seen.Add(id))
                {
                    continue;
                }

                result.Add(id);
            }

            return result.AsReadOnly();
        }

        private static IReadOnlyDictionary<ResourceType, int> CreateCost(IReadOnlyDictionary<ResourceType, int> cost)
        {
            Dictionary<ResourceType, int> result = new Dictionary<ResourceType, int>();

            if (cost == null)
            {
                return new ReadOnlyDictionary<ResourceType, int>(result);
            }

            foreach (KeyValuePair<ResourceType, int> item in cost)
            {
                if (item.Key == ResourceType.None || item.Value <= 0)
                {
                    throw new ArgumentException("Research cost must use valid resources and positive amounts.", nameof(cost));
                }

                result[item.Key] = item.Value;
            }

            return new ReadOnlyDictionary<ResourceType, int>(result);
        }
    }
}
