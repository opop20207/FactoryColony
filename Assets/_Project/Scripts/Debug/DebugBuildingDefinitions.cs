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
                    Create(BuildingType.Miner, 1, 1, true, ResourceType.IronOre, true, 0, 2)
                },
                {
                    BuildingType.Conveyor,
                    Create(BuildingType.Conveyor, 1, 1, false, ResourceType.None, true, 0, 0)
                },
                {
                    BuildingType.Smelter,
                    Create(BuildingType.Smelter, 1, 1, false, ResourceType.None, true, 0, 4)
                },
                {
                    BuildingType.Storage,
                    Create(BuildingType.Storage, 1, 1, false, ResourceType.None, false, 0, 0)
                },
                {
                    BuildingType.Generator,
                    Create(BuildingType.Generator, 1, 1, false, ResourceType.None, false, 10, 0)
                },
                {
                    BuildingType.Assembler,
                    Create(BuildingType.Assembler, 1, 1, false, ResourceType.None, true, 0, 5)
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

        private static BuildingDefinition Create(
            BuildingType type,
            int width,
            int height,
            bool requiresResourceNode,
            ResourceType requiredResourceType,
            bool isRotatable,
            int powerProduction,
            int powerConsumption)
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
                powerConsumption);
        }
    }
}
