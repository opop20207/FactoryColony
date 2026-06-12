using System.Collections.Generic;
using System.Linq;
using FactoryColony;
using NUnit.Framework;

namespace FactoryColony.Tests.EditMode.Factory
{
    public sealed class StorageCollectorTests
    {
        [Test]
        public void CollectResource_MovesSingleStorageResourceToBaseInventory()
        {
            GridModel gridModel = new GridModel(10, 10);
            BaseInventoryModel baseInventory = new BaseInventoryModel();
            BuildingModel storage = PlaceBuilding(gridModel, "storage-1", BuildingType.Storage, new GridPosition(1, 1));
            storage.Inventory.Add(ResourceType.IronPlate, 5);
            StorageCollector collector = new StorageCollector(gridModel, baseInventory);

            int collectedAmount = collector.CollectResource(ResourceType.IronPlate);

            Assert.AreEqual(5, collectedAmount);
            Assert.AreEqual(5, baseInventory.GetAmount(ResourceType.IronPlate));
            Assert.AreEqual(0, storage.Inventory.GetAmount(ResourceType.IronPlate));
        }

        [Test]
        public void CollectResource_SumsSameResourceAcrossMultipleStorageBuildings()
        {
            GridModel gridModel = new GridModel(10, 10);
            BaseInventoryModel baseInventory = new BaseInventoryModel();
            PlaceBuilding(gridModel, "storage-1", BuildingType.Storage, new GridPosition(1, 1)).Inventory.Add(ResourceType.IronPlate, 5);
            PlaceBuilding(gridModel, "storage-2", BuildingType.Storage, new GridPosition(2, 1)).Inventory.Add(ResourceType.IronPlate, 3);
            StorageCollector collector = new StorageCollector(gridModel, baseInventory);

            int collectedAmount = collector.CollectResource(ResourceType.IronPlate);

            Assert.AreEqual(8, collectedAmount);
            Assert.AreEqual(8, baseInventory.GetAmount(ResourceType.IronPlate));
        }

        [Test]
        public void CollectAllByType_CollectsMultipleResourceTypes()
        {
            GridModel gridModel = new GridModel(10, 10);
            BaseInventoryModel baseInventory = new BaseInventoryModel();
            BuildingModel storage = PlaceBuilding(gridModel, "storage-1", BuildingType.Storage, new GridPosition(1, 1));
            storage.Inventory.Add(ResourceType.IronPlate, 5);
            storage.Inventory.Add(ResourceType.CopperWire, 2);
            StorageCollector collector = new StorageCollector(gridModel, baseInventory);

            IReadOnlyDictionary<ResourceType, int> collected = collector.CollectAllByType();

            Assert.AreEqual(5, collected[ResourceType.IronPlate]);
            Assert.AreEqual(2, collected[ResourceType.CopperWire]);
            Assert.AreEqual(5, baseInventory.GetAmount(ResourceType.IronPlate));
            Assert.AreEqual(2, baseInventory.GetAmount(ResourceType.CopperWire));
        }

        [Test]
        public void CollectAllByType_DoesNotCollectNonStorageInventory()
        {
            GridModel gridModel = new GridModel(10, 10);
            BaseInventoryModel baseInventory = new BaseInventoryModel();
            BuildingModel conveyor = PlaceBuilding(gridModel, "conveyor-1", BuildingType.Conveyor, new GridPosition(1, 1));
            conveyor.Inventory.Add(ResourceType.IronPlate, 5);
            StorageCollector collector = new StorageCollector(gridModel, baseInventory);

            IReadOnlyDictionary<ResourceType, int> collected = collector.CollectAllByType();

            Assert.AreEqual(0, collected.Count);
            Assert.AreEqual(0, baseInventory.GetAmount(ResourceType.IronPlate));
            Assert.AreEqual(5, conveyor.Inventory.GetAmount(ResourceType.IronPlate));
        }

        [Test]
        public void CollectAllByType_EmptiesStorageInventory()
        {
            GridModel gridModel = new GridModel(10, 10);
            BaseInventoryModel baseInventory = new BaseInventoryModel();
            BuildingModel storage = PlaceBuilding(gridModel, "storage-1", BuildingType.Storage, new GridPosition(1, 1));
            storage.Inventory.Add(ResourceType.IronPlate, 5);
            StorageCollector collector = new StorageCollector(gridModel, baseInventory);

            collector.CollectAllByType();

            Assert.IsTrue(storage.Inventory.IsEmpty);
        }

        [Test]
        public void CollectFromStorage_CollectsOnlyGivenStorage()
        {
            GridModel gridModel = new GridModel(10, 10);
            BaseInventoryModel baseInventory = new BaseInventoryModel();
            BuildingModel firstStorage = PlaceBuilding(gridModel, "storage-1", BuildingType.Storage, new GridPosition(1, 1));
            BuildingModel secondStorage = PlaceBuilding(gridModel, "storage-2", BuildingType.Storage, new GridPosition(2, 1));
            firstStorage.Inventory.Add(ResourceType.IronPlate, 4);
            secondStorage.Inventory.Add(ResourceType.IronPlate, 7);
            StorageCollector collector = new StorageCollector(gridModel, baseInventory);

            IReadOnlyDictionary<ResourceType, int> collected = collector.CollectFromStorage(firstStorage);

            Assert.AreEqual(4, collected[ResourceType.IronPlate]);
            Assert.AreEqual(4, baseInventory.GetAmount(ResourceType.IronPlate));
            Assert.IsTrue(firstStorage.Inventory.IsEmpty);
            Assert.AreEqual(7, secondStorage.Inventory.GetAmount(ResourceType.IronPlate));
        }

        [Test]
        public void CollectFromStorage_ReturnsEmptyForNonStorage()
        {
            GridModel gridModel = new GridModel(10, 10);
            BaseInventoryModel baseInventory = new BaseInventoryModel();
            BuildingModel conveyor = PlaceBuilding(gridModel, "conveyor-1", BuildingType.Conveyor, new GridPosition(1, 1));
            conveyor.Inventory.Add(ResourceType.IronPlate, 4);
            StorageCollector collector = new StorageCollector(gridModel, baseInventory);

            IReadOnlyDictionary<ResourceType, int> collected = collector.CollectFromStorage(conveyor);

            Assert.AreEqual(0, collected.Count);
            Assert.AreEqual(0, baseInventory.GetAmount(ResourceType.IronPlate));
            Assert.AreEqual(4, conveyor.Inventory.GetAmount(ResourceType.IronPlate));
        }

        [Test]
        public void CollectResource_CollectsOnlyRequestedResource()
        {
            GridModel gridModel = new GridModel(10, 10);
            BaseInventoryModel baseInventory = new BaseInventoryModel();
            BuildingModel storage = PlaceBuilding(gridModel, "storage-1", BuildingType.Storage, new GridPosition(1, 1));
            storage.Inventory.Add(ResourceType.IronPlate, 5);
            storage.Inventory.Add(ResourceType.CopperWire, 2);
            StorageCollector collector = new StorageCollector(gridModel, baseInventory);

            int collectedAmount = collector.CollectResource(ResourceType.IronPlate);

            Assert.AreEqual(5, collectedAmount);
            Assert.AreEqual(5, baseInventory.GetAmount(ResourceType.IronPlate));
            Assert.AreEqual(0, baseInventory.GetAmount(ResourceType.CopperWire));
            Assert.AreEqual(2, storage.Inventory.GetAmount(ResourceType.CopperWire));
        }

        [Test]
        public void CollectResource_ReturnsZeroForNone()
        {
            GridModel gridModel = new GridModel(10, 10);
            BaseInventoryModel baseInventory = new BaseInventoryModel();
            StorageCollector collector = new StorageCollector(gridModel, baseInventory);

            Assert.AreEqual(0, collector.CollectResource(ResourceType.None));
        }

        [Test]
        public void CollectAll_ReturnsZeroWhenThereAreNoStorageResources()
        {
            GridModel gridModel = new GridModel(10, 10);
            BaseInventoryModel baseInventory = new BaseInventoryModel();
            StorageCollector collector = new StorageCollector(gridModel, baseInventory);

            Assert.AreEqual(0, collector.CollectAll());
            Assert.AreEqual(0, collector.CollectAllByType().Count);
        }

        [Test]
        public void FactorySimulation_OutputCanBeCollectedIntoBaseInventory()
        {
            GridModel gridModel = CreateIronPlateLine();
            BaseInventoryModel baseInventory = new BaseInventoryModel();
            FactorySimulation simulation = new FactorySimulation(gridModel);
            StorageCollector collector = new StorageCollector(gridModel, baseInventory);

            for (int i = 0; i < 8; i++)
            {
                simulation.SimulateTick();
            }

            int storedIronPlate = simulation.GetStoredAmount(ResourceType.IronPlate);
            int collectedIronPlate = collector.CollectResource(ResourceType.IronPlate);

            Assert.Greater(storedIronPlate, 0);
            Assert.AreEqual(storedIronPlate, collectedIronPlate);
            Assert.AreEqual(collectedIronPlate, baseInventory.GetAmount(ResourceType.IronPlate));
        }

        [Test]
        public void CollectedResourcesCanAffordBuildCost()
        {
            GridModel gridModel = new GridModel(10, 10);
            BaseInventoryModel baseInventory = new BaseInventoryModel();
            BuildingModel storage = PlaceBuilding(gridModel, "storage-1", BuildingType.Storage, new GridPosition(1, 1));
            storage.Inventory.Add(ResourceType.IronPlate, 5);
            StorageCollector collector = new StorageCollector(gridModel, baseInventory);
            BuildingDefinition minerDefinition = new BuildingDefinition(
                "miner",
                BuildingType.Miner,
                "Miner",
                1,
                1,
                true,
                ResourceType.IronOre,
                true,
                0,
                2,
                new Dictionary<ResourceType, int> { { ResourceType.IronPlate, 5 } });

            Assert.IsFalse(baseInventory.CanAfford(minerDefinition.BuildCost));

            collector.CollectAll();

            Assert.IsTrue(baseInventory.CanAfford(minerDefinition.BuildCost));
        }

        private static GridModel CreateIronPlateLine()
        {
            GridModel gridModel = new GridModel(10, 10);
            gridModel.SetResourceNode(new GridPosition(1, 1), ResourceType.IronOre);
            PlaceBuilding(gridModel, "miner", BuildingType.Miner, new GridPosition(1, 1), BuildingDirection.East, requiresResourceNode: true, requiredResourceType: ResourceType.IronOre, powerConsumption: 2);
            PlaceBuilding(gridModel, "conveyor-ore", BuildingType.Conveyor, new GridPosition(2, 1), BuildingDirection.East);
            PlaceBuilding(gridModel, "smelter", BuildingType.Smelter, new GridPosition(3, 1), BuildingDirection.East, powerConsumption: 4);
            PlaceBuilding(gridModel, "conveyor-ingot", BuildingType.Conveyor, new GridPosition(4, 1), BuildingDirection.East);
            BuildingModel assembler = PlaceBuilding(gridModel, "assembler", BuildingType.Assembler, new GridPosition(5, 1), BuildingDirection.East, powerConsumption: 5);
            assembler.SelectedRecipeId = RecipeCatalog.IronPlateRecipeId;
            PlaceBuilding(gridModel, "conveyor-plate", BuildingType.Conveyor, new GridPosition(6, 1), BuildingDirection.East);
            PlaceBuilding(gridModel, "storage", BuildingType.Storage, new GridPosition(7, 1), BuildingDirection.North);
            PlaceBuilding(gridModel, "generator-a", BuildingType.Generator, new GridPosition(0, 6), BuildingDirection.North, powerProduction: 10);
            PlaceBuilding(gridModel, "generator-b", BuildingType.Generator, new GridPosition(1, 6), BuildingDirection.North, powerProduction: 10);
            return gridModel;
        }

        private static BuildingModel PlaceBuilding(
            GridModel gridModel,
            string instanceId,
            BuildingType type,
            GridPosition origin,
            BuildingDirection direction = BuildingDirection.North,
            bool requiresResourceNode = false,
            ResourceType requiredResourceType = ResourceType.None,
            int powerProduction = 0,
            int powerConsumption = 0)
        {
            BuildingDefinition definition = new BuildingDefinition(
                "test-" + type,
                type,
                "Test " + type,
                1,
                1,
                requiresResourceNode,
                requiredResourceType,
                true,
                powerProduction,
                powerConsumption);
            BuildingModel building = new BuildingModel(instanceId, definition, origin, direction);

            Assert.IsTrue(gridModel.TryPlaceBuilding(building));
            return building;
        }
    }
}
