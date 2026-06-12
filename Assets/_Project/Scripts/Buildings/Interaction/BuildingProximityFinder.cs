using System;

namespace FactoryColony
{
    public static class BuildingProximityFinder
    {
        public static BuildingModel FindNearest(
            GridModel gridModel,
            float playerWorldX,
            float playerWorldZ,
            float cellSize,
            float interactionRange)
        {
            if (gridModel == null || cellSize <= 0f || interactionRange <= 0f)
            {
                return null;
            }

            BuildingModel nearest = null;
            double nearestDistanceSquared = interactionRange * interactionRange;

            foreach (BuildingModel building in gridModel.GetAllBuildings())
            {
                double distanceSquared = GetClosestOccupiedDistanceSquared(
                    building,
                    playerWorldX,
                    playerWorldZ,
                    cellSize);

                if (distanceSquared > nearestDistanceSquared)
                {
                    continue;
                }

                nearest = building;
                nearestDistanceSquared = distanceSquared;
            }

            return nearest;
        }

        private static double GetClosestOccupiedDistanceSquared(
            BuildingModel building,
            float playerWorldX,
            float playerWorldZ,
            float cellSize)
        {
            double closest = double.MaxValue;

            foreach (GridPosition position in building.OccupiedPositions)
            {
                double worldX = position.X * cellSize;
                double worldZ = position.Y * cellSize;
                double deltaX = worldX - playerWorldX;
                double deltaZ = worldZ - playerWorldZ;
                closest = Math.Min(closest, deltaX * deltaX + deltaZ * deltaZ);
            }

            return closest;
        }
    }
}
