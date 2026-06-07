using System;
using FactoryColony;
using NUnit.Framework;

namespace FactoryColony.Tests.EditMode.Grid
{
    public sealed class GridModelTests
    {
        [Test]
        public void Constructor_SetsWidthAndHeight()
        {
            GridModel gridModel = new GridModel(10, 10);

            Assert.AreEqual(10, gridModel.Width);
            Assert.AreEqual(10, gridModel.Height);
        }

        [Test]
        public void IsInside_ReturnsTrueForValidPositions()
        {
            GridModel gridModel = new GridModel(10, 10);

            Assert.IsTrue(gridModel.IsInside(new GridPosition(0, 0)));
            Assert.IsTrue(gridModel.IsInside(new GridPosition(9, 9)));
        }

        [Test]
        public void IsInside_ReturnsFalseForInvalidPositions()
        {
            GridModel gridModel = new GridModel(10, 10);

            Assert.IsFalse(gridModel.IsInside(new GridPosition(-1, 0)));
            Assert.IsFalse(gridModel.IsInside(new GridPosition(10, 0)));
            Assert.IsFalse(gridModel.IsInside(new GridPosition(0, 10)));
        }

        [Test]
        public void GetCell_ReturnsCellForValidPosition()
        {
            GridModel gridModel = new GridModel(10, 10);
            GridPosition position = new GridPosition(3, 4);

            GridCell cell = gridModel.GetCell(position);

            Assert.NotNull(cell);
            Assert.AreEqual(position, cell.Position);
        }

        [Test]
        public void GetCell_ThrowsForInvalidPosition()
        {
            GridModel gridModel = new GridModel(10, 10);

            Assert.Throws<ArgumentOutOfRangeException>(() => gridModel.GetCell(new GridPosition(10, 0)));
        }

        [Test]
        public void SetBuildable_UpdatesCellBuildableState()
        {
            GridModel gridModel = new GridModel(10, 10);
            GridPosition position = new GridPosition(2, 3);

            gridModel.SetBuildable(position, false);

            Assert.IsFalse(gridModel.GetCell(position).IsBuildable);
        }

        [Test]
        public void SetResourceNode_UpdatesCellResourceNodeType()
        {
            GridModel gridModel = new GridModel(10, 10);
            GridPosition position = new GridPosition(4, 5);

            gridModel.SetResourceNode(position, ResourceType.IronOre);

            Assert.AreEqual(ResourceType.IronOre, gridModel.GetCell(position).ResourceNodeType);
        }
    }
}
