using System.Linq;
using System.Collections.Generic;
using FactoryColony;
using NUnit.Framework;

namespace FactoryColony.Tests.EditMode.Buildings
{
    public sealed class BuildingPlacementTests
    {
        [Test]
        public void TryPlaceBuilding_PlacesOneByOneBuilding()
        {
            GridModel gridModel = new GridModel(10, 10);
            BuildingModel building = CreateBuilding("miner-1", 1, 1, new GridPosition(2, 2));

            bool wasPlaced = gridModel.TryPlaceBuilding(building);

            Assert.IsTrue(wasPlaced);
            Assert.IsTrue(gridModel.GetCell(new GridPosition(2, 2)).HasBuilding);
            Assert.AreEqual("miner-1", gridModel.GetCell(new GridPosition(2, 2)).OccupiedByBuildingId);
        }

        [Test]
        public void TryPlaceBuilding_PlacesTwoByTwoBuildingAcrossFourCells()
        {
            GridModel gridModel = new GridModel(10, 10);
            BuildingModel building = CreateBuilding("smelter-1", 2, 2, new GridPosition(3, 3));

            bool wasPlaced = gridModel.TryPlaceBuilding(building);

            Assert.IsTrue(wasPlaced);
            Assert.AreEqual(4, building.OccupiedPositions.Count);

            foreach (GridPosition position in building.OccupiedPositions)
            {
                Assert.AreEqual("smelter-1", gridModel.GetCell(position).OccupiedByBuildingId);
            }
        }

        [Test]
        public void TryPlaceBuilding_ReturnsFalseWhenCellIsAlreadyOccupied()
        {
            GridModel gridModel = new GridModel(10, 10);
            gridModel.PlaceBuilding(CreateBuilding("storage-1", 2, 2, new GridPosition(1, 1)));

            bool wasPlaced = gridModel.TryPlaceBuilding(CreateBuilding("storage-2", 1, 1, new GridPosition(2, 2)));

            Assert.IsFalse(wasPlaced);
        }

        [Test]
        public void TryPlaceBuilding_ReturnsFalseWhenBuildingExtendsOutsideGrid()
        {
            GridModel gridModel = new GridModel(10, 10);

            bool wasPlaced = gridModel.TryPlaceBuilding(CreateBuilding("smelter-1", 2, 2, new GridPosition(9, 9)));

            Assert.IsFalse(wasPlaced);
        }

        [Test]
        public void TryPlaceBuilding_ReturnsFalseWhenAnyOccupiedCellIsNotBuildable()
        {
            GridModel gridModel = new GridModel(10, 10);
            gridModel.SetBuildable(new GridPosition(3, 3), false);

            bool wasPlaced = gridModel.TryPlaceBuilding(CreateBuilding("smelter-1", 2, 2, new GridPosition(2, 2)));

            Assert.IsFalse(wasPlaced);
        }

        [Test]
        public void RemoveBuilding_ClearsAllOccupiedCells()
        {
            GridModel gridModel = new GridModel(10, 10);
            BuildingModel building = CreateBuilding("smelter-1", 2, 2, new GridPosition(2, 2));
            gridModel.PlaceBuilding(building);

            gridModel.RemoveBuilding("smelter-1");

            foreach (GridPosition position in building.OccupiedPositions)
            {
                Assert.IsFalse(gridModel.GetCell(position).HasBuilding);
            }
        }

        [Test]
        public void TryRemoveBuilding_ReturnsFalseForMissingBuilding()
        {
            GridModel gridModel = new GridModel(10, 10);

            bool wasRemoved = gridModel.TryRemoveBuilding("missing-building");

            Assert.IsFalse(wasRemoved);
        }

        [Test]
        public void TryRemoveBuilding_ClearsAllOccupiedCellsForMultiCellBuilding()
        {
            GridModel gridModel = new GridModel(10, 10);
            BuildingModel building = CreateBuilding("smelter-1", 2, 3, new GridPosition(2, 2));
            gridModel.PlaceBuilding(building);

            bool wasRemoved = gridModel.TryRemoveBuilding("smelter-1");

            Assert.IsTrue(wasRemoved);

            foreach (GridPosition position in building.OccupiedPositions)
            {
                Assert.IsFalse(gridModel.GetCell(position).HasBuilding);
                Assert.IsNull(gridModel.GetCell(position).OccupiedByBuildingId);
            }
        }

        [Test]
        public void RemovedBuilding_PositionCanBeReused()
        {
            GridModel gridModel = new GridModel(10, 10);
            BuildingModel firstBuilding = CreateBuilding("storage-1", 2, 2, new GridPosition(2, 2));
            BuildingModel secondBuilding = CreateBuilding("storage-2", 2, 2, new GridPosition(2, 2));
            gridModel.PlaceBuilding(firstBuilding);

            bool wasRemoved = gridModel.TryRemoveBuilding("storage-1");
            bool wasPlacedAgain = gridModel.TryPlaceBuilding(secondBuilding);

            Assert.IsTrue(wasRemoved);
            Assert.IsTrue(wasPlacedAgain);

            foreach (GridPosition position in secondBuilding.OccupiedPositions)
            {
                Assert.AreEqual("storage-2", gridModel.GetCell(position).OccupiedByBuildingId);
            }
        }

        [Test]
        public void TryPlaceBuilding_ReturnsFalseForDuplicateInstanceId()
        {
            GridModel gridModel = new GridModel(10, 10);
            gridModel.PlaceBuilding(CreateBuilding("miner-1", 1, 1, new GridPosition(0, 0)));

            bool wasPlaced = gridModel.TryPlaceBuilding(CreateBuilding("miner-1", 1, 1, new GridPosition(2, 2)));

            Assert.IsFalse(wasPlaced);
        }

        [Test]
        public void ResourceNodeBuilding_CanOnlyBePlacedOnResourceNode()
        {
            GridModel gridModel = new GridModel(10, 10);
            BuildingDefinition definition = CreateDefinition(1, 1, true, ResourceType.None);

            Assert.IsFalse(gridModel.TryPlaceBuilding(new BuildingModel("miner-1", definition, new GridPosition(1, 1), BuildingDirection.North)));

            gridModel.SetResourceNode(new GridPosition(1, 1), ResourceType.IronOre);

            Assert.IsTrue(gridModel.TryPlaceBuilding(new BuildingModel("miner-1", definition, new GridPosition(1, 1), BuildingDirection.North)));
        }

        [Test]
        public void ResourceNodeBuilding_ReturnsFalseWhenRequiredResourceTypeDoesNotMatch()
        {
            GridModel gridModel = new GridModel(10, 10);
            gridModel.SetResourceNode(new GridPosition(1, 1), ResourceType.CopperOre);
            BuildingDefinition definition = CreateDefinition(1, 1, true, ResourceType.IronOre);

            bool wasPlaced = gridModel.TryPlaceBuilding(new BuildingModel("miner-1", definition, new GridPosition(1, 1), BuildingDirection.North));

            Assert.IsFalse(wasPlaced);
        }

        [Test]
        public void BuildingModel_RotatesOccupiedSizeForEastAndWestDirections()
        {
            BuildingDefinition definition = CreateDefinition(2, 3, false, ResourceType.None);
            BuildingModel eastBuilding = new BuildingModel("building-east", definition, new GridPosition(0, 0), BuildingDirection.East);
            BuildingModel westBuilding = new BuildingModel("building-west", definition, new GridPosition(0, 0), BuildingDirection.West);

            Assert.AreEqual(6, eastBuilding.OccupiedPositions.Count);
            Assert.IsTrue(eastBuilding.OccupiedPositions.Contains(new GridPosition(2, 1)));
            Assert.IsFalse(eastBuilding.OccupiedPositions.Contains(new GridPosition(1, 2)));

            Assert.AreEqual(6, westBuilding.OccupiedPositions.Count);
            Assert.IsTrue(westBuilding.OccupiedPositions.Contains(new GridPosition(2, 1)));
            Assert.IsFalse(westBuilding.OccupiedPositions.Contains(new GridPosition(1, 2)));
        }

        [Test]
        public void ToOffset_ReturnsExpectedDirectionOffsets()
        {
            Assert.AreEqual(new GridPosition(0, 1), BuildingDirection.North.ToOffset());
            Assert.AreEqual(new GridPosition(1, 0), BuildingDirection.East.ToOffset());
            Assert.AreEqual(new GridPosition(0, -1), BuildingDirection.South.ToOffset());
            Assert.AreEqual(new GridPosition(-1, 0), BuildingDirection.West.ToOffset());
        }

        [Test]
        public void StorageBuilding_CanStoreResources()
        {
            BuildingModel storage = CreateBuilding("storage-1", BuildingType.Storage, new GridPosition(0, 0));

            Assert.IsTrue(storage.CanStoreResources);
            Assert.IsFalse(storage.CanProduceResources);
        }

        [Test]
        public void MinerBuilding_CanProduceResources()
        {
            BuildingModel miner = CreateBuilding("miner-1", BuildingType.Miner, new GridPosition(0, 0));

            Assert.IsTrue(miner.CanProduceResources);
            Assert.IsFalse(miner.CanStoreResources);
        }

        [Test]
        public void StorageBuilding_InventoryCanStoreResourcesDirectly()
        {
            BuildingModel storage = CreateBuilding("storage-1", BuildingType.Storage, new GridPosition(0, 0));

            storage.Inventory.Add(ResourceType.IronOre, 4);

            Assert.AreEqual(4, storage.Inventory.GetAmount(ResourceType.IronOre));
        }

        [Test]
        public void BuildingDefinition_StoresPowerValues()
        {
            BuildingDefinition definition = CreateDefinition(
                1,
                1,
                false,
                ResourceType.None,
                10,
                2);

            Assert.AreEqual(10, definition.PowerProduction);
            Assert.AreEqual(2, definition.PowerConsumption);
        }

        [Test]
        public void BuildingDefinition_RejectsNegativePowerProduction()
        {
            Assert.Throws<System.ArgumentException>(() => CreateDefinition(
                1,
                1,
                false,
                ResourceType.None,
                -1,
                0));
        }

        [Test]
        public void BuildingDefinition_RejectsNegativePowerConsumption()
        {
            Assert.Throws<System.ArgumentException>(() => CreateDefinition(
                1,
                1,
                false,
                ResourceType.None,
                0,
                -1));
        }

        [Test]
        public void BuildingDefinition_StoresBuildCost()
        {
            BuildingDefinition definition = CreateDefinitionWithCost(Cost(ResourceType.IronPlate, 5));

            Assert.AreEqual(5, definition.BuildCost[ResourceType.IronPlate]);
        }

        [Test]
        public void BuildingDefinition_RejectsNoneResourceBuildCost()
        {
            Assert.Throws<System.ArgumentException>(() => CreateDefinitionWithCost(Cost(ResourceType.None, 1)));
        }

        [Test]
        public void BuildingDefinition_RejectsNonPositiveBuildCostAmount()
        {
            Assert.Throws<System.ArgumentException>(() => CreateDefinitionWithCost(Cost(ResourceType.IronPlate, 0)));
        }

        [Test]
        public void BuildingDefinition_DefensivelyCopiesBuildCost()
        {
            Dictionary<ResourceType, int> cost = Cost(ResourceType.IronPlate, 5);
            BuildingDefinition definition = CreateDefinitionWithCost(cost);

            cost[ResourceType.IronPlate] = 99;

            Assert.AreEqual(5, definition.BuildCost[ResourceType.IronPlate]);
        }

        [Test]
        public void ConstructionCost_IsSpentOnlyAfterSuccessfulPlacement()
        {
            GridModel gridModel = new GridModel(10, 10);
            BaseInventoryModel inventory = new BaseInventoryModel();
            inventory.Add(ResourceType.IronPlate, 10);
            BuildingDefinition definition = CreateDefinitionWithCost(Cost(ResourceType.IronPlate, 5));
            BuildingModel building = new BuildingModel("storage-1", definition, new GridPosition(1, 1), BuildingDirection.North);

            bool wasPlaced = gridModel.TryPlaceBuilding(building);
            bool wasSpent = inventory.TrySpend(definition.BuildCost);

            Assert.IsTrue(wasPlaced);
            Assert.IsTrue(wasSpent);
            Assert.AreEqual(5, inventory.GetAmount(ResourceType.IronPlate));
        }

        [Test]
        public void ConstructionCost_IsNotSpentWhenInventoryCannotAfford()
        {
            GridModel gridModel = new GridModel(10, 10);
            BaseInventoryModel inventory = new BaseInventoryModel();
            inventory.Add(ResourceType.IronPlate, 4);
            BuildingDefinition definition = CreateDefinitionWithCost(Cost(ResourceType.IronPlate, 5));
            BuildingModel building = new BuildingModel("storage-1", definition, new GridPosition(1, 1), BuildingDirection.North);

            bool canAfford = inventory.CanAfford(definition.BuildCost);
            bool wasPlaced = canAfford && gridModel.TryPlaceBuilding(building);

            Assert.IsFalse(canAfford);
            Assert.IsFalse(wasPlaced);
            Assert.AreEqual(4, inventory.GetAmount(ResourceType.IronPlate));
            Assert.IsFalse(gridModel.GetCell(new GridPosition(1, 1)).HasBuilding);
        }

        [Test]
        public void ConstructionCost_IsNotSpentWhenPlacementFails()
        {
            GridModel gridModel = new GridModel(10, 10);
            BaseInventoryModel inventory = new BaseInventoryModel();
            inventory.Add(ResourceType.IronPlate, 10);
            BuildingDefinition definition = CreateDefinitionWithCost(Cost(ResourceType.IronPlate, 5));
            gridModel.PlaceBuilding(new BuildingModel("existing", definition, new GridPosition(1, 1), BuildingDirection.North));

            BuildingModel blockedBuilding = new BuildingModel("blocked", definition, new GridPosition(1, 1), BuildingDirection.North);
            bool wasPlaced = gridModel.TryPlaceBuilding(blockedBuilding);

            if (wasPlaced)
            {
                inventory.TrySpend(definition.BuildCost);
            }

            Assert.IsFalse(wasPlaced);
            Assert.AreEqual(10, inventory.GetAmount(ResourceType.IronPlate));
        }

        [Test]
        public void RemovalRefund_ReturnsHalfOfBuildCostAfterSuccessfulRemoval()
        {
            GridModel gridModel = new GridModel(10, 10);
            BaseInventoryModel inventory = new BaseInventoryModel();
            BuildingDefinition definition = CreateDefinitionWithCost(Cost(ResourceType.IronPlate, 5));
            gridModel.PlaceBuilding(new BuildingModel("storage-1", definition, new GridPosition(1, 1), BuildingDirection.North));

            bool wasRemoved = gridModel.TryRemoveBuilding("storage-1");

            if (wasRemoved)
            {
                RefundHalf(inventory, definition.BuildCost);
            }

            Assert.IsTrue(wasRemoved);
            Assert.AreEqual(2, inventory.GetAmount(ResourceType.IronPlate));
        }

        [Test]
        public void RemovalRefund_DoesNotRefundWhenRemovalFails()
        {
            GridModel gridModel = new GridModel(10, 10);
            BaseInventoryModel inventory = new BaseInventoryModel();
            BuildingDefinition definition = CreateDefinitionWithCost(Cost(ResourceType.IronPlate, 5));

            bool wasRemoved = gridModel.TryRemoveBuilding("missing");

            if (wasRemoved)
            {
                RefundHalf(inventory, definition.BuildCost);
            }

            Assert.IsFalse(wasRemoved);
            Assert.AreEqual(0, inventory.GetAmount(ResourceType.IronPlate));
        }

        [Test]
        public void RemovalRefund_RefundsHalfOfEachBuildCostResource()
        {
            GridModel gridModel = new GridModel(10, 10);
            BaseInventoryModel inventory = new BaseInventoryModel();
            Dictionary<ResourceType, int> cost = new Dictionary<ResourceType, int>
            {
                { ResourceType.IronPlate, 8 },
                { ResourceType.CopperWire, 5 }
            };
            BuildingDefinition definition = CreateDefinitionWithCost(cost);
            gridModel.PlaceBuilding(new BuildingModel("smelter-1", definition, new GridPosition(1, 1), BuildingDirection.North));

            bool wasRemoved = gridModel.TryRemoveBuilding("smelter-1");

            if (wasRemoved)
            {
                RefundHalf(inventory, definition.BuildCost);
            }

            Assert.IsTrue(wasRemoved);
            Assert.AreEqual(4, inventory.GetAmount(ResourceType.IronPlate));
            Assert.AreEqual(2, inventory.GetAmount(ResourceType.CopperWire));
        }

        private static BuildingModel CreateBuilding(string instanceId, int width, int height, GridPosition origin)
        {
            return new BuildingModel(instanceId, CreateDefinition(width, height, false, ResourceType.None), origin, BuildingDirection.North);
        }

        private static BuildingModel CreateBuilding(string instanceId, BuildingType buildingType, GridPosition origin)
        {
            BuildingDefinition definition = new BuildingDefinition(
                "test-building",
                buildingType,
                "Test Building",
                1,
                1,
                false,
                ResourceType.None,
                true);

            return new BuildingModel(instanceId, definition, origin, BuildingDirection.North);
        }

        private static BuildingDefinition CreateDefinition(
            int width,
            int height,
            bool requiresResourceNode,
            ResourceType requiredResourceType)
        {
            return CreateDefinition(width, height, requiresResourceNode, requiredResourceType, 0, 0);
        }

        private static BuildingDefinition CreateDefinition(
            int width,
            int height,
            bool requiresResourceNode,
            ResourceType requiredResourceType,
            int powerProduction,
            int powerConsumption)
        {
            return new BuildingDefinition(
                "test-building",
                BuildingType.Miner,
                "Test Building",
                width,
                height,
                requiresResourceNode,
                requiredResourceType,
                true,
                powerProduction,
                powerConsumption);
        }

        private static BuildingDefinition CreateDefinitionWithCost(IReadOnlyDictionary<ResourceType, int> buildCost)
        {
            return new BuildingDefinition(
                "test-building",
                BuildingType.Storage,
                "Test Building",
                1,
                1,
                false,
                ResourceType.None,
                true,
                0,
                0,
                buildCost);
        }

        private static Dictionary<ResourceType, int> Cost(ResourceType type, int amount)
        {
            return new Dictionary<ResourceType, int>
            {
                { type, amount }
            };
        }

        private static void RefundHalf(BaseInventoryModel inventory, IReadOnlyDictionary<ResourceType, int> buildCost)
        {
            foreach (KeyValuePair<ResourceType, int> cost in buildCost)
            {
                int refundAmount = cost.Value / 2;

                if (refundAmount > 0)
                {
                    inventory.Add(cost.Key, refundAmount);
                }
            }
        }
    }
}
