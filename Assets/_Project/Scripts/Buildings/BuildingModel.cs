using System;
using System.Collections.Generic;

namespace FactoryColony
{
    public sealed class BuildingModel
    {
        private readonly List<GridPosition> _occupiedPositions;

        public string InstanceId { get; }
        public BuildingDefinition Definition { get; }
        public GridPosition Origin { get; }
        public BuildingDirection Direction { get; }
        public InventoryModel Inventory { get; }
        public string SelectedRecipeId { get; set; }
        public bool CanStoreResources
        {
            get { return Definition.Type == BuildingType.Storage; }
        }

        public bool CanProduceResources
        {
            get { return Definition.Type == BuildingType.Miner; }
        }

        public IReadOnlyList<GridPosition> OccupiedPositions
        {
            get { return _occupiedPositions; }
        }

        public BuildingModel(
            string instanceId,
            BuildingDefinition definition,
            GridPosition origin,
            BuildingDirection direction)
        {
            if (definition == null)
            {
                throw new ArgumentNullException(nameof(definition));
            }

            InstanceId = instanceId;
            Definition = definition;
            Origin = origin;
            Direction = direction;
            Inventory = new InventoryModel();
            _occupiedPositions = CalculateOccupiedPositions();
        }

        private List<GridPosition> CalculateOccupiedPositions()
        {
            int occupiedWidth = Definition.Width;
            int occupiedHeight = Definition.Height;

            if (Direction == BuildingDirection.East || Direction == BuildingDirection.West)
            {
                occupiedWidth = Definition.Height;
                occupiedHeight = Definition.Width;
            }

            List<GridPosition> positions = new List<GridPosition>(occupiedWidth * occupiedHeight);

            for (int x = 0; x < occupiedWidth; x++)
            {
                for (int y = 0; y < occupiedHeight; y++)
                {
                    positions.Add(new GridPosition(Origin.X + x, Origin.Y + y));
                }
            }

            return positions;
        }
    }
}
