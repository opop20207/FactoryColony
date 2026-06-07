using System;

namespace FactoryColony
{
    public static class BuildingDirectionExtensions
    {
        public static GridPosition ToOffset(this BuildingDirection direction)
        {
            switch (direction)
            {
                case BuildingDirection.North:
                    return new GridPosition(0, 1);
                case BuildingDirection.East:
                    return new GridPosition(1, 0);
                case BuildingDirection.South:
                    return new GridPosition(0, -1);
                case BuildingDirection.West:
                    return new GridPosition(-1, 0);
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, "Unknown building direction.");
            }
        }
    }
}
