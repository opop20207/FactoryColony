using System.Collections.Generic;
using System.Linq;
using FactoryColony;
using NUnit.Framework;

namespace FactoryColony.Tests.EditMode.Save
{
    public sealed class SaveGameServiceTests
    {
        [Test]
        public void CreateSaveData_StoresGridSize()
        {
            SaveGameService service = new SaveGameService();
            FactorySaveData saveData = service.CreateSaveData(new GridModel(12, 8), new BaseInventoryModel(), null);

            Assert.AreEqual(12, saveData.Grid.Width);
            Assert.AreEqual(8, saveData.Grid.Height);
        }

        [Test]
        public void CreateSaveData_StoresResourceNodes()
        {
            GridModel gridModel = new GridModel(10, 10);
            gridModel.SetResourceNode(new GridPosition(2, 3), ResourceType.CopperOre);
            SaveGameService service = new SaveGameService();

            FactorySaveData saveData = service.CreateSaveData(gridModel, new BaseInventoryModel(), null);

            ResourceNodeSaveData node = saveData.Grid.ResourceNodes.Single();
            Assert.AreEqual(2, node.X);
            Assert.AreEqual(3, node.Y);
            Assert.AreEqual(ResourceType.CopperOre, node.ResourceType);
        }

        [Test]
        public void CreateSaveData_StoresBuildingCoreData()
        {
            GridModel gridModel = new GridModel(10, 10);
            BuildingModel building = PlaceBuilding(gridModel, "assembler-1", BuildingType.Assembler, new GridPosition(4, 5), BuildingDirection.East);
            building.SelectedRecipeId = RecipeCatalog.IronPlateRecipeId;
            SaveGameService service = new SaveGameService();

            FactorySaveData saveData = service.CreateSaveData(gridModel, new BaseInventoryModel(), null);

            BuildingSaveData savedBuilding = saveData.Grid.Buildings.Single();
            Assert.AreEqual("assembler-1", savedBuilding.InstanceId);
            Assert.AreEqual("test-Assembler", savedBuilding.DefinitionId);
            Assert.AreEqual(BuildingType.Assembler, savedBuilding.BuildingType);
            Assert.AreEqual(4, savedBuilding.OriginX);
            Assert.AreEqual(5, savedBuilding.OriginY);
            Assert.AreEqual(BuildingDirection.East, savedBuilding.Direction);
            Assert.AreEqual(RecipeCatalog.IronPlateRecipeId, savedBuilding.SelectedRecipeId);
        }

        [Test]
        public void CreateSaveData_StoresBuildingInventory()
        {
            GridModel gridModel = new GridModel(10, 10);
            BuildingModel building = PlaceBuilding(gridModel, "storage-1", BuildingType.Storage, new GridPosition(1, 1), BuildingDirection.North);
            building.Inventory.Add(ResourceType.IronPlate, 7);
            SaveGameService service = new SaveGameService();

            FactorySaveData saveData = service.CreateSaveData(gridModel, new BaseInventoryModel(), null);

            ResourceAmountSaveData resource = saveData.Grid.Buildings.Single().Inventory.Resources.Single();
            Assert.AreEqual(ResourceType.IronPlate, resource.ResourceType);
            Assert.AreEqual(7, resource.Amount);
        }

        [Test]
        public void CreateSaveData_StoresBaseInventory()
        {
            BaseInventoryModel baseInventory = new BaseInventoryModel();
            baseInventory.Add(ResourceType.CopperWire, 12);
            SaveGameService service = new SaveGameService();

            FactorySaveData saveData = service.CreateSaveData(new GridModel(10, 10), baseInventory, null);

            ResourceAmountSaveData resource = saveData.BaseInventory.Inventory.Resources.Single();
            Assert.AreEqual(ResourceType.CopperWire, resource.ResourceType);
            Assert.AreEqual(12, resource.Amount);
        }

        [Test]
        public void CreateSaveData_StoresGoalProgressSnapshot()
        {
            BaseInventoryModel baseInventory = new BaseInventoryModel();
            baseInventory.Add(ResourceType.IronPlate, 50);
            GoalTracker goalTracker = new GoalTracker(
                baseInventory,
                new[] { new ProductionGoalModel("iron", "IronPlate", ResourceType.IronPlate, 50) });
            SaveGameService service = new SaveGameService();

            FactorySaveData saveData = service.CreateSaveData(new GridModel(10, 10), baseInventory, goalTracker);

            GoalSaveData goal = saveData.Goals.Single();
            Assert.AreEqual("iron", goal.Id);
            Assert.AreEqual(50, goal.CurrentAmount);
            Assert.IsTrue(goal.IsCompleted);
        }

        [Test]
        public void Json_RoundTripsSaveData()
        {
            SaveGameService service = new SaveGameService();
            GridModel gridModel = new GridModel(10, 10);
            gridModel.SetResourceNode(new GridPosition(1, 1), ResourceType.IronOre);
            PlaceBuilding(gridModel, "miner-1", BuildingType.Miner, new GridPosition(1, 1), BuildingDirection.East, requiresResourceNode: true);
            FactorySaveData saveData = service.CreateSaveData(gridModel, new BaseInventoryModel(), null);

            string json = service.ToJson(saveData);
            bool loaded = service.TryFromJson(json, out FactorySaveData restored);

            Assert.IsTrue(loaded);
            Assert.AreEqual(10, restored.Grid.Width);
            Assert.AreEqual(1, restored.Grid.ResourceNodes.Count);
            Assert.AreEqual(1, restored.Grid.Buildings.Count);
        }

        [Test]
        public void RestoreGrid_RestoresResourceNodeBuildingInventoryAndRecipe()
        {
            SaveGameService service = new SaveGameService();
            GridModel gridModel = new GridModel(10, 10);
            gridModel.SetResourceNode(new GridPosition(1, 1), ResourceType.IronOre);
            BuildingModel building = PlaceBuilding(gridModel, "assembler-1", BuildingType.Assembler, new GridPosition(2, 1), BuildingDirection.East);
            building.Inventory.Add(ResourceType.IronPlate, 3);
            building.SelectedRecipeId = RecipeCatalog.GearRecipeId;
            FactorySaveData saveData = service.CreateSaveData(gridModel, new BaseInventoryModel(), null);

            GridModel restoredGrid = new FactorySaveLoader().RestoreGrid(saveData, Definitions());

            Assert.AreEqual(ResourceType.IronOre, restoredGrid.GetCell(new GridPosition(1, 1)).ResourceNodeType);
            Assert.IsTrue(restoredGrid.TryGetBuilding("assembler-1", out BuildingModel restoredBuilding));
            Assert.AreEqual(3, restoredBuilding.Inventory.GetAmount(ResourceType.IronPlate));
            Assert.AreEqual(RecipeCatalog.GearRecipeId, restoredBuilding.SelectedRecipeId);
        }

        [Test]
        public void RestoreBaseInventory_RestoresAmounts()
        {
            SaveGameService service = new SaveGameService();
            BaseInventoryModel baseInventory = new BaseInventoryModel();
            baseInventory.Add(ResourceType.Gear, 5);
            FactorySaveData saveData = service.CreateSaveData(new GridModel(10, 10), baseInventory, null);

            BaseInventoryModel restored = new FactorySaveLoader().RestoreBaseInventory(saveData);

            Assert.AreEqual(5, restored.GetAmount(ResourceType.Gear));
        }

        [Test]
        public void RestoreGrid_SkipsUnknownDefinition()
        {
            FactorySaveData saveData = CreateSaveDataWithBuilding("missing-definition", "building-1", 1, 1);

            GridModel restoredGrid = new FactorySaveLoader().RestoreGrid(saveData, Definitions());

            Assert.IsFalse(restoredGrid.TryGetBuilding("building-1", out BuildingModel ignored));
        }

        [Test]
        public void RestoreGrid_SkipsBuildingThatCannotBePlaced()
        {
            FactorySaveData saveData = new FactorySaveData();
            saveData.Grid.Width = 10;
            saveData.Grid.Height = 10;
            saveData.Grid.Buildings.Add(CreateBuildingSaveData("test-Storage", "storage-1", 1, 1));
            saveData.Grid.Buildings.Add(CreateBuildingSaveData("test-Storage", "storage-2", 1, 1));

            GridModel restoredGrid = new FactorySaveLoader().RestoreGrid(saveData, Definitions());

            Assert.IsTrue(restoredGrid.TryGetBuilding("storage-1", out BuildingModel first));
            Assert.IsFalse(restoredGrid.TryGetBuilding("storage-2", out BuildingModel second));
        }

        [Test]
        public void SaveRestoreIntegration_PreservesCoreValues()
        {
            SaveGameService service = new SaveGameService();
            GridModel gridModel = new GridModel(10, 10);
            BaseInventoryModel baseInventory = new BaseInventoryModel();
            baseInventory.Add(ResourceType.IronPlate, 20);
            BuildingModel storage = PlaceBuilding(gridModel, "storage-1", BuildingType.Storage, new GridPosition(2, 2), BuildingDirection.North);
            storage.Inventory.Add(ResourceType.CopperWire, 9);

            FactorySaveData saveData = service.CreateSaveData(gridModel, baseInventory, null);
            GridModel restoredGrid = new FactorySaveLoader().RestoreGrid(saveData, Definitions());
            BaseInventoryModel restoredBaseInventory = new FactorySaveLoader().RestoreBaseInventory(saveData);

            Assert.IsTrue(restoredGrid.TryGetBuilding("storage-1", out BuildingModel restoredStorage));
            Assert.AreEqual(9, restoredStorage.Inventory.GetAmount(ResourceType.CopperWire));
            Assert.AreEqual(20, restoredBaseInventory.GetAmount(ResourceType.IronPlate));
        }

        private static BuildingModel PlaceBuilding(
            GridModel gridModel,
            string instanceId,
            BuildingType type,
            GridPosition origin,
            BuildingDirection direction,
            bool requiresResourceNode = false)
        {
            BuildingDefinition definition = CreateDefinition(type, requiresResourceNode);
            BuildingModel building = new BuildingModel(instanceId, definition, origin, direction);
            gridModel.PlaceBuilding(building);
            return building;
        }

        private static BuildingDefinition CreateDefinition(BuildingType type, bool requiresResourceNode = false)
        {
            return new BuildingDefinition(
                "test-" + type,
                type,
                "Test " + type,
                1,
                1,
                requiresResourceNode,
                ResourceType.None,
                true);
        }

        private static Dictionary<string, BuildingDefinition> Definitions()
        {
            return new Dictionary<string, BuildingDefinition>
            {
                { "test-Miner", CreateDefinition(BuildingType.Miner, true) },
                { "test-Storage", CreateDefinition(BuildingType.Storage) },
                { "test-Assembler", CreateDefinition(BuildingType.Assembler) }
            };
        }

        private static FactorySaveData CreateSaveDataWithBuilding(string definitionId, string instanceId, int x, int y)
        {
            FactorySaveData saveData = new FactorySaveData();
            saveData.Grid.Width = 10;
            saveData.Grid.Height = 10;
            saveData.Grid.Buildings.Add(CreateBuildingSaveData(definitionId, instanceId, x, y));
            return saveData;
        }

        private static BuildingSaveData CreateBuildingSaveData(string definitionId, string instanceId, int x, int y)
        {
            return new BuildingSaveData
            {
                DefinitionId = definitionId,
                InstanceId = instanceId,
                BuildingType = BuildingType.Storage,
                OriginX = x,
                OriginY = y,
                Direction = BuildingDirection.North
            };
        }
    }
}
