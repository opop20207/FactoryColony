using FactoryColony;
using NUnit.Framework;

namespace FactoryColony.Tests.EditMode.Goals
{
    public sealed class ProductionGoalModelTests
    {
        [Test]
        public void Constructor_CreatesValidGoal()
        {
            ProductionGoalModel goal = new ProductionGoalModel("iron-plate", "Iron Plate", ResourceType.IronPlate, 50);

            Assert.AreEqual("iron-plate", goal.Id);
            Assert.AreEqual("Iron Plate", goal.DisplayName);
            Assert.AreEqual(ResourceType.IronPlate, goal.ResourceType);
            Assert.AreEqual(50, goal.RequiredAmount);
            Assert.IsFalse(goal.IsCompleted);
        }

        [Test]
        public void Constructor_RejectsEmptyId()
        {
            Assert.Throws<System.ArgumentException>(
                () => new ProductionGoalModel(string.Empty, "Iron Plate", ResourceType.IronPlate, 50));
        }

        [Test]
        public void Constructor_RejectsEmptyDisplayName()
        {
            Assert.Throws<System.ArgumentException>(
                () => new ProductionGoalModel("goal", string.Empty, ResourceType.IronPlate, 50));
        }

        [Test]
        public void Constructor_RejectsNoneResourceType()
        {
            Assert.Throws<System.ArgumentException>(
                () => new ProductionGoalModel("goal", "Invalid", ResourceType.None, 50));
        }

        [Test]
        public void Constructor_RejectsRequiredAmountLessThanOne()
        {
            Assert.Throws<System.ArgumentException>(
                () => new ProductionGoalModel("goal", "Iron Plate", ResourceType.IronPlate, 0));
        }

        [Test]
        public void UpdateProgress_KeepsGoalIncompleteWhenCurrentAmountIsBelowRequiredAmount()
        {
            ProductionGoalModel goal = new ProductionGoalModel("goal", "Iron Plate", ResourceType.IronPlate, 50);

            goal.UpdateProgress(49);

            Assert.IsFalse(goal.IsCompleted);
        }

        [Test]
        public void UpdateProgress_CompletesGoalWhenCurrentAmountEqualsRequiredAmount()
        {
            ProductionGoalModel goal = new ProductionGoalModel("goal", "Iron Plate", ResourceType.IronPlate, 50);

            goal.UpdateProgress(50);

            Assert.IsTrue(goal.IsCompleted);
        }

        [Test]
        public void UpdateProgress_CompletesGoalWhenCurrentAmountExceedsRequiredAmount()
        {
            ProductionGoalModel goal = new ProductionGoalModel("goal", "Iron Plate", ResourceType.IronPlate, 50);

            goal.UpdateProgress(80);

            Assert.IsTrue(goal.IsCompleted);
        }

        [Test]
        public void ProgressRatio_IsClampedBetweenZeroAndOne()
        {
            ProductionGoalModel goal = new ProductionGoalModel("goal", "Iron Plate", ResourceType.IronPlate, 50);

            goal.UpdateProgress(-10);
            Assert.AreEqual(0f, goal.ProgressRatio);

            goal.UpdateProgress(100);
            Assert.AreEqual(1f, goal.ProgressRatio);
        }
    }
}
