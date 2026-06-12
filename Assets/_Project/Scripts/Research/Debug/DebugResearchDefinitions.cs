using System.Collections.Generic;
using System.Linq;

namespace FactoryColony
{
    public static class DebugResearchDefinitions
    {
        public const string BasicAutomationId = "basic_automation";
        public const string OreProcessingId = "ore_processing";
        public const string AssemblyId = "assembly";
        public const string PowerExpansionId = "power_expansion";

        public static IReadOnlyDictionary<string, ResearchDefinition> CreateDefinitions()
        {
            ResearchDefinition[] definitions =
            {
                new ResearchDefinition(
                    BasicAutomationId,
                    "Basic Automation",
                    "Unlock basic logistics.",
                    null,
                    Cost(ResourceType.IronPlate, 10),
                    UnlockBuildings(BuildingType.Conveyor, BuildingType.Storage),
                    null),
                new ResearchDefinition(
                    OreProcessingId,
                    "Ore Processing",
                    "Unlock smelting and basic materials.",
                    new[] { BasicAutomationId },
                    Cost(ResourceType.IronPlate, 20, ResourceType.CopperWire, 10),
                    UnlockBuildings(BuildingType.Smelter),
                    new[] { RecipeCatalog.IronPlateRecipeId, RecipeCatalog.CopperWireRecipeId }),
                new ResearchDefinition(
                    AssemblyId,
                    "Assembly",
                    "Unlock assemblers and advanced parts.",
                    new[] { OreProcessingId },
                    Cost(ResourceType.IronPlate, 30, ResourceType.CopperWire, 20, ResourceType.Gear, 5),
                    UnlockBuildings(BuildingType.Assembler),
                    new[] { RecipeCatalog.GearRecipeId, RecipeCatalog.BasicCircuitRecipeId }),
                new ResearchDefinition(
                    PowerExpansionId,
                    "Power Expansion",
                    "Unlock generators.",
                    new[] { BasicAutomationId },
                    Cost(ResourceType.IronPlate, 25, ResourceType.CopperWire, 15),
                    UnlockBuildings(BuildingType.Generator),
                    null)
            };

            return definitions.ToDictionary(definition => definition.Id);
        }

        private static IReadOnlyList<string> UnlockBuildings(params BuildingType[] types)
        {
            return types
                .Select(type => DebugBuildingDefinitions.Get(type).Id)
                .ToArray();
        }

        private static IReadOnlyDictionary<ResourceType, int> Cost(ResourceType type, int amount)
        {
            return new Dictionary<ResourceType, int> { { type, amount } };
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
