using FactoryColony;
using NUnit.Framework;

namespace FactoryColony.Tests.EditMode.Factory
{
    public sealed class BuildingProximityFinderTests
    {
        [Test]
        public void FindNearest_ReturnsNearestBuildingInsideRange()
        {
            GridModel gridModel = new GridModel(10, 10);
            BuildingModel near = Place(gridModel, "near", new GridPosition(2, 2));
            Place(gridModel, "far", new GridPosition(8, 8));

            BuildingModel result = BuildingProximityFinder.FindNearest(gridModel, 2.1f, 2.1f, 1f, 2f);

            Assert.AreEqual(near, result);
        }

        [Test]
        public void FindNearest_ReturnsNullOutsideRange()
        {
            GridModel gridModel = new GridModel(10, 10);
            Place(gridModel, "far", new GridPosition(8, 8));

            BuildingModel result = BuildingProximityFinder.FindNearest(gridModel, 0f, 0f, 1f, 1f);

            Assert.IsNull(result);
        }

        [Test]
        public void FindNearest_UsesOccupiedPositions()
        {
            GridModel gridModel = new GridModel(10, 10);
            BuildingDefinition definition = new BuildingDefinition(
                "large",
                BuildingType.Storage,
                "Large Storage",
                2,
                2,
                false,
                ResourceType.None,
                false);
            BuildingModel building = new BuildingModel("large-1", definition, new GridPosition(3, 3), BuildingDirection.North);
            Assert.IsTrue(gridModel.TryPlaceBuilding(building));

            BuildingModel result = BuildingProximityFinder.FindNearest(gridModel, 4f, 4f, 1f, 0.25f);

            Assert.AreEqual(building, result);
        }

        private static BuildingModel Place(GridModel gridModel, string id, GridPosition position)
        {
            BuildingDefinition definition = new BuildingDefinition(
                id,
                BuildingType.Storage,
                "Storage",
                1,
                1,
                false,
                ResourceType.None,
                false);
            BuildingModel building = new BuildingModel(id, definition, position, BuildingDirection.North);
            Assert.IsTrue(gridModel.TryPlaceBuilding(building));
            return building;
        }
    }
}
