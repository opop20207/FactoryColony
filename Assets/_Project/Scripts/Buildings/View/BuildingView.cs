using System.Collections.Generic;
using UnityEngine;

namespace FactoryColony
{
    public sealed class BuildingView : MonoBehaviour
    {
        private BuildingModel _building;
        private BuildingInventoryVisualView _inventoryVisualView;
        private BuildingPowerStatusView _powerStatusView;

        public BuildingModel Building
        {
            get { return _building; }
        }

        public void Initialize(BuildingModel building, float cellSize)
        {
            Initialize(building, cellSize, null);
        }

        public void Initialize(BuildingModel building, float cellSize, PowerStatusService powerStatusService)
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

            GameObject inventoryVisualObject = new GameObject("InventoryVisualRoot");
            inventoryVisualObject.transform.SetParent(transform, false);
            _inventoryVisualView = inventoryVisualObject.AddComponent<BuildingInventoryVisualView>();
            _inventoryVisualView.Initialize(building, cellSize);

            if (powerStatusService != null)
            {
                GameObject powerStatusObject = new GameObject("PowerStatusRoot");
                powerStatusObject.transform.SetParent(transform, false);
                _powerStatusView = powerStatusObject.AddComponent<BuildingPowerStatusView>();
                _powerStatusView.Initialize(building, powerStatusService, cellSize);
            }
        }

        public void RefreshInventoryVisual()
        {
            if (_inventoryVisualView != null)
            {
                _inventoryVisualView.Refresh();
            }
        }

        public void RefreshPowerStatusVisual()
        {
            if (_powerStatusView != null)
            {
                _powerStatusView.Refresh();
            }
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
