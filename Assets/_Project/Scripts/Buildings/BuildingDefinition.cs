using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FactoryColony
{
    public sealed class BuildingDefinition
    {
        public string Id { get; }
        public BuildingType Type { get; }
        public string DisplayName { get; }
        public int Width { get; }
        public int Height { get; }
        public bool RequiresResourceNode { get; }
        public ResourceType RequiredResourceType { get; }
        public bool IsRotatable { get; }
        public int PowerProduction { get; }
        public int PowerConsumption { get; }
        public IReadOnlyDictionary<ResourceType, int> BuildCost { get; }

        public BuildingDefinition(
            string id,
            BuildingType type,
            string displayName,
            int width,
            int height,
            bool requiresResourceNode,
            ResourceType requiredResourceType,
            bool isRotatable)
            : this(
                id,
                type,
                displayName,
                width,
                height,
                requiresResourceNode,
                requiredResourceType,
                isRotatable,
                0,
                0)
        {
        }

        public BuildingDefinition(
            string id,
            BuildingType type,
            string displayName,
            int width,
            int height,
            bool requiresResourceNode,
            ResourceType requiredResourceType,
            bool isRotatable,
            int powerProduction,
            int powerConsumption)
            : this(
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
                null)
        {
        }

        public BuildingDefinition(
            string id,
            BuildingType type,
            string displayName,
            int width,
            int height,
            bool requiresResourceNode,
            ResourceType requiredResourceType,
            bool isRotatable,
            int powerProduction,
            int powerConsumption,
            IReadOnlyDictionary<ResourceType, int> buildCost)
        {
            if (width < 1)
            {
                throw new ArgumentException("Width must be at least one.", nameof(width));
            }

            if (height < 1)
            {
                throw new ArgumentException("Height must be at least one.", nameof(height));
            }

            if (powerProduction < 0)
            {
                throw new ArgumentException("Power production must not be negative.", nameof(powerProduction));
            }

            if (powerConsumption < 0)
            {
                throw new ArgumentException("Power consumption must not be negative.", nameof(powerConsumption));
            }

            BuildCost = CreateBuildCost(buildCost);
            Id = id;
            Type = type;
            DisplayName = displayName;
            Width = width;
            Height = height;
            RequiresResourceNode = requiresResourceNode;
            RequiredResourceType = requiredResourceType;
            IsRotatable = isRotatable;
            PowerProduction = powerProduction;
            PowerConsumption = powerConsumption;
        }

        private static IReadOnlyDictionary<ResourceType, int> CreateBuildCost(IReadOnlyDictionary<ResourceType, int> buildCost)
        {
            Dictionary<ResourceType, int> copiedCost = new Dictionary<ResourceType, int>();

            if (buildCost == null)
            {
                return new ReadOnlyDictionary<ResourceType, int>(copiedCost);
            }

            foreach (KeyValuePair<ResourceType, int> cost in buildCost)
            {
                if (cost.Key == ResourceType.None)
                {
                    throw new ArgumentException("Build cost cannot contain ResourceType.None.", nameof(buildCost));
                }

                if (cost.Value <= 0)
                {
                    throw new ArgumentException("Build cost amounts must be greater than zero.", nameof(buildCost));
                }

                copiedCost[cost.Key] = cost.Value;
            }

            return new ReadOnlyDictionary<ResourceType, int>(copiedCost);
        }
    }
}
