using System.Linq;
using FactoryColony;
using NUnit.Framework;

namespace FactoryColony.Tests.EditMode.Goals
{
    public sealed class GoalTrackerTests
    {
        [Test]
        public void UpdateGoals_UsesBaseInventoryAmounts()
        {
            BaseInventoryModel baseInventory = new BaseInventoryModel();
            baseInventory.Add(ResourceType.IronPlate, 12);
            GoalTracker tracker = CreateTracker(baseInventory);

            tracker.UpdateGoals();

            Assert.AreEqual(12, tracker.GetGoal("iron").CurrentAmount);
        }

        [Test]
        public void Goals_ReturnsMultipleGoals()
        {
            GoalTracker tracker = CreateTracker(new BaseInventoryModel());

            Assert.AreEqual(2, tracker.Goals.Count);
            Assert.IsTrue(tracker.Goals.Any(goal => goal.Id == "iron"));
            Assert.IsTrue(tracker.Goals.Any(goal => goal.Id == "wire"));
        }

        [Test]
        public void AllCompleted_ReturnsFalseWhenOnlySomeGoalsAreComplete()
        {
            BaseInventoryModel baseInventory = new BaseInventoryModel();
            baseInventory.Add(ResourceType.IronPlate, 50);
            GoalTracker tracker = CreateTracker(baseInventory);

            tracker.UpdateGoals();

            Assert.IsFalse(tracker.AllCompleted);
        }

        [Test]
        public void AllCompleted_ReturnsTrueWhenEveryGoalIsComplete()
        {
            BaseInventoryModel baseInventory = new BaseInventoryModel();
            baseInventory.Add(ResourceType.IronPlate, 50);
            baseInventory.Add(ResourceType.CopperWire, 30);
            GoalTracker tracker = CreateTracker(baseInventory);

            tracker.UpdateGoals();

            Assert.IsTrue(tracker.AllCompleted);
        }

        [Test]
        public void TryGetGoal_ReturnsFalseForUnknownId()
        {
            GoalTracker tracker = CreateTracker(new BaseInventoryModel());

            bool found = tracker.TryGetGoal("missing", out ProductionGoalModel goal);

            Assert.IsFalse(found);
            Assert.IsNull(goal);
        }

        [Test]
        public void StorageResources_DoNotCountBeforeCollection()
        {
            GridModel gridModel = new GridModel(10, 10);
            BaseInventoryModel baseInventory = new BaseInventoryModel();
            BuildingModel storage = PlaceStorage(gridModel);
            storage.Inventory.Add(ResourceType.IronPlate, 50);
            GoalTracker tracker = new GoalTracker(
                baseInventory,
                new[] { new ProductionGoalModel("iron", "IronPlate", ResourceType.IronPlate, 50) });

            tracker.UpdateGoals();

            Assert.IsFalse(tracker.GetGoal("iron").IsCompleted);
            Assert.AreEqual(0, tracker.GetGoal("iron").CurrentAmount);
        }

        [Test]
        public void BaseInventoryAdd_IncreasesGoalProgressAfterUpdate()
        {
            BaseInventoryModel baseInventory = new BaseInventoryModel();
            GoalTracker tracker = new GoalTracker(
                baseInventory,
                new[] { new ProductionGoalModel("iron", "IronPlate", ResourceType.IronPlate, 50) });

            baseInventory.Add(ResourceType.IronPlate, 25);
            tracker.UpdateGoals();

            Assert.AreEqual(25, tracker.GetGoal("iron").CurrentAmount);
            Assert.IsFalse(tracker.GetGoal("iron").IsCompleted);
        }

        [Test]
        public void StorageCollection_CanCompleteGoal()
        {
            GridModel gridModel = new GridModel(10, 10);
            BaseInventoryModel baseInventory = new BaseInventoryModel();
            BuildingModel storage = PlaceStorage(gridModel);
            storage.Inventory.Add(ResourceType.IronPlate, 50);
            StorageCollector collector = new StorageCollector(gridModel, baseInventory);
            GoalTracker tracker = new GoalTracker(
                baseInventory,
                new[] { new ProductionGoalModel("iron", "IronPlate", ResourceType.IronPlate, 50) });

            collector.CollectAll();
            tracker.UpdateGoals();

            Assert.IsTrue(tracker.GetGoal("iron").IsCompleted);
            Assert.AreEqual(50, tracker.GetGoal("iron").CurrentAmount);
        }

        private static GoalTracker CreateTracker(BaseInventoryModel baseInventory)
        {
            return new GoalTracker(
                baseInventory,
                new[]
                {
                    new ProductionGoalModel("iron", "IronPlate", ResourceType.IronPlate, 50),
                    new ProductionGoalModel("wire", "CopperWire", ResourceType.CopperWire, 30)
                });
        }

        private static BuildingModel PlaceStorage(GridModel gridModel)
        {
            BuildingDefinition definition = new BuildingDefinition(
                "storage",
                BuildingType.Storage,
                "Storage",
                1,
                1,
                false,
                ResourceType.None,
                false);
            BuildingModel storage = new BuildingModel("storage-1", definition, new GridPosition(1, 1), BuildingDirection.North);
            gridModel.PlaceBuilding(storage);
            return storage;
        }
    }
}
