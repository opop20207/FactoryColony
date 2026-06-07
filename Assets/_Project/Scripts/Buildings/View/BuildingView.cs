using System.Collections.Generic;
using UnityEngine;

namespace FactoryColony
{
    public sealed class BuildingView : MonoBehaviour
    {
        private BuildingModel _building;

        public BuildingModel Building
        {
            get { return _building; }
        }

        public void Initialize(BuildingModel building, float cellSize)
        {
            _building = building;
            gameObject.name = $"Building_{building.InstanceId}_{building.Definition.Type}";

            BoundsData bounds = CalculateBounds(building.OccupiedPositions);
            transform.localPosition = new Vector3(
                bounds.CenterX * cellSize,
                0f,
                bounds.CenterY * cellSize);

            LowPolyBuildingViewBuilder.Build(
                building,
                transform,
                bounds.Width * cellSize,
                bounds.Height * cellSize,
                cellSize);
        }

        private static BoundsData CalculateBounds(IReadOnlyList<GridPosition> positions)
        {
            int minX = positions[0].X;
            int maxX = positions[0].X;
            int minY = positions[0].Y;
            int maxY = positions[0].Y;

            for (int i = 1; i < positions.Count; i++)
            {
                GridPosition position = positions[i];
                minX = Mathf.Min(minX, position.X);
                maxX = Mathf.Max(maxX, position.X);
                minY = Mathf.Min(minY, position.Y);
                maxY = Mathf.Max(maxY, position.Y);
            }

            return new BoundsData(minX, maxX, minY, maxY);
        }

        private readonly struct BoundsData
        {
            public int Width { get; }
            public int Height { get; }
            public float CenterX { get; }
            public float CenterY { get; }

            public BoundsData(int minX, int maxX, int minY, int maxY)
            {
                Width = maxX - minX + 1;
                Height = maxY - minY + 1;
                CenterX = (minX + maxX) * 0.5f;
                CenterY = (minY + maxY) * 0.5f;
            }
        }
    }
}
