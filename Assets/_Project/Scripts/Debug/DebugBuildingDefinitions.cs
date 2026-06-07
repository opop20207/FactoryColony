using System.Collections.Generic;

namespace FactoryColony
{
    public static class DebugBuildingDefinitions
    {
        private static readonly Dictionary<BuildingType, BuildingDefinition> Definitions =
            new Dictionary<BuildingType, BuildingDefinition>
            {
                {
                    BuildingType.Miner,
                    Create(
                        BuildingType.Miner,
                        1,
                        1,
                        true,
                        ResourceType.IronOre,
                        true,
                        0,
                        2,
                        Cost(ResourceType.IronPlate, 5))
                },
                {
                    BuildingType.Conveyor,
                    Create(
                        BuildingType.Conveyor,
                        1,
                        1,
                        false,
                        ResourceType.None,
                        true,
                        0,
                        0,
                        Cost(ResourceType.IronPlate, 1))
                },
                {
                    BuildingType.Smelter,
                    Create(
                        BuildingType.Smelter,
                        1,
                        1,
                        false,
                        ResourceType.None,
                        true,
                        0,
                        4,
                        Cost(ResourceType.IronPlate, 8, ResourceType.CopperWire, 4))
                },
                {
                    BuildingType.Storage,
                    Create(
                        BuildingType.Storage,
                        1,
                        1,
                        false,
                        ResourceType.None,
                        false,
                        0,
                        0,
                        Cost(ResourceType.IronPlate, 6))
                },
                {
                    BuildingType.Generator,
                    Create(
                        BuildingType.Generator,
                        1,
                        1,
                        false,
                        ResourceType.None,
                        false,
                        10,
                        0,
                        Cost(ResourceType.IronPlate, 10, ResourceType.CopperWire, 6))
                },
                {
                    BuildingType.Assembler,
                    Create(
                        BuildingType.Assembler,
                        1,
                        1,
                        false,
                        ResourceType.None,
                        true,
                        0,
                        5,
                        Cost(ResourceType.IronPlate, 12, ResourceType.CopperWire, 8, ResourceType.Gear, 2))
                }
            };

        public static BuildingDefinition Get(BuildingType type)
        {
            return Definitions[type];
        }

        public static bool TryGet(BuildingType type, out BuildingDefinition definition)
        {
            return Definitions.TryGetValue(type, out definition);
        }

        public static IReadOnlyList<BuildingDefinition> GetBuildMenuDefinitions()
        {
            return new[]
            {
                Get(BuildingType.Miner),
                Get(BuildingType.Conveyor),
                Get(BuildingType.Smelter),
                Get(BuildingType.Storage),
                Get(BuildingType.Generator),
                Get(BuildingType.Assembler)
            };
        }

        private static BuildingDefinition Create(
            BuildingType type,
            int width,
            int height,
            bool requiresResourceNode,
            ResourceType requiredResourceType,
            bool isRotatable,
            int powerProduction,
            int powerConsumption,
            IReadOnlyDictionary<ResourceType, int> buildCost)
        {
            return new BuildingDefinition(
                "debug-" + type,
                type,
                "Debug " + type,
                width,
                height,
                requiresResourceNode,
                requiredResourceType,
                isRotatable,
                powerProduction,
                powerConsumption,
                buildCost);
        }

        private static IReadOnlyDictionary<ResourceType, int> Cost(ResourceType type, int amount)
        {
            return new Dictionary<ResourceType, int>
            {
                { type, amount }
            };
        }

        private static IReadOnlyDictionary<ResourceType, int> Cost(
            ResourceType firstType,
            int firstAmount,
            ResourceType secondType,
            int secondAmount)
        {
            return new Dictionary<ResourceType, int>
            {
                { firstType, firstAmount },
                { secondType, secondAmount }
            };
        }

        private static IReadOnlyDictionary<ResourceType, int> Cost(
            ResourceType firstType,
            int firstAmount,
            ResourceType secondType,
            int secondAmount,
            ResourceType thirdType,
            int thirdAmount)
        {
            return new Dictionary<ResourceType, int>
            {
                { firstType, firstAmount },
                { secondType, secondAmount },
                { thirdType, thirdAmount }
            };
        }
    }
}
