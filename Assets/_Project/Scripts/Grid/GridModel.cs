using System;
using System.Collections.Generic;

namespace FactoryColony
{
    public sealed class GridModel
    {
        private readonly GridCell[,] _cells;
        private readonly Dictionary<string, BuildingModel> _buildings;

        public int Width { get; }
        public int Height { get; }

        public GridModel(int width, int height)
        {
            if (width <= 0)
            {
                throw new ArgumentException("Width must be greater than zero.", nameof(width));
            }

            if (height <= 0)
            {
                throw new ArgumentException("Height must be greater than zero.", nameof(height));
            }

            Width = width;
            Height = height;
            _cells = new GridCell[width, height];
            _buildings = new Dictionary<string, BuildingModel>();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    GridPosition position = new GridPosition(x, y);
                    _cells[x, y] = new GridCell(position);
                }
            }
        }

        public bool IsInside(GridPosition position)
        {
            return position.X >= 0
                && position.Y >= 0
                && position.X < Width
                && position.Y < Height;
        }

        public GridCell GetCell(GridPosition position)
        {
            if (!IsInside(position))
            {
                throw new ArgumentOutOfRangeException(nameof(position), position, "Grid position is outside the grid.");
            }

            return _cells[position.X, position.Y];
        }

        public bool TryGetCell(GridPosition position, out GridCell cell)
        {
            if (!IsInside(position))
            {
                cell = null;
                return false;
            }

            cell = _cells[position.X, position.Y];
            return true;
        }

        public IEnumerable<GridCell> GetAllCells()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    yield return _cells[x, y];
                }
            }
        }

        public IEnumerable<BuildingModel> GetAllBuildings()
        {
            return _buildings.Values;
        }

        public void SetBuildable(GridPosition position, bool isBuildable)
        {
            GetCell(position).IsBuildable = isBuildable;
        }

        public void SetResourceNode(GridPosition position, ResourceType resourceType)
        {
            GetCell(position).ResourceNodeType = resourceType;
        }

        public void ClearResourceNode(GridPosition position)
        {
            SetResourceNode(position, ResourceType.None);
        }

        public bool CanPlaceBuilding(BuildingModel building)
        {
            if (building == null || string.IsNullOrEmpty(building.InstanceId))
            {
                return false;
            }

            if (_buildings.ContainsKey(building.InstanceId))
            {
                return false;
            }

            foreach (GridPosition position in building.OccupiedPositions)
            {
                if (!TryGetCell(position, out GridCell cell))
                {
                    return false;
                }

                if (!cell.IsBuildable || cell.HasBuilding)
                {
                    return false;
                }
            }

            if (!building.Definition.RequiresResourceNode)
            {
                return true;
            }

            GridCell originCell = GetCell(building.Origin);

            if (originCell.ResourceNodeType == ResourceType.None)
            {
                return false;
            }

            return building.Definition.RequiredResourceType == ResourceType.None
                || originCell.ResourceNodeType == building.Definition.RequiredResourceType;
        }

        public void PlaceBuilding(BuildingModel building)
        {
            if (building == null)
            {
                throw new ArgumentNullException(nameof(building));
            }

            if (string.IsNullOrEmpty(building.InstanceId))
            {
                throw new ArgumentException("Building instance id must not be empty.", nameof(building));
            }

            if (_buildings.ContainsKey(building.InstanceId))
            {
                throw new InvalidOperationException($"Building instance '{building.InstanceId}' is already placed.");
            }

            ValidateBuildingPlacement(building);

            foreach (GridPosition position in building.OccupiedPositions)
            {
                GetCell(position).OccupiedByBuildingId = building.InstanceId;
            }

            _buildings.Add(building.InstanceId, building);
        }

        public bool TryPlaceBuilding(BuildingModel building)
        {
            if (!CanPlaceBuilding(building))
            {
                return false;
            }

            PlaceBuilding(building);
            return true;
        }

        public void RemoveBuilding(string buildingInstanceId)
        {
            if (!_buildings.TryGetValue(buildingInstanceId, out BuildingModel building))
            {
                throw new KeyNotFoundException($"Building instance '{buildingInstanceId}' is not placed.");
            }

            foreach (GridPosition position in building.OccupiedPositions)
            {
                GridCell cell = GetCell(position);

                if (cell.OccupiedByBuildingId == buildingInstanceId)
                {
                    cell.OccupiedByBuildingId = null;
                }
            }

            _buildings.Remove(buildingInstanceId);
        }

        public bool TryRemoveBuilding(string buildingInstanceId)
        {
            if (!_buildings.ContainsKey(buildingInstanceId))
            {
                return false;
            }

            RemoveBuilding(buildingInstanceId);
            return true;
        }

        public BuildingModel GetBuilding(string buildingInstanceId)
        {
            if (!_buildings.TryGetValue(buildingInstanceId, out BuildingModel building))
            {
                throw new KeyNotFoundException($"Building instance '{buildingInstanceId}' is not placed.");
            }

            return building;
        }

        public bool TryGetBuilding(string buildingInstanceId, out BuildingModel building)
        {
            return _buildings.TryGetValue(buildingInstanceId, out building);
        }

        private void ValidateBuildingPlacement(BuildingModel building)
        {
            foreach (GridPosition position in building.OccupiedPositions)
            {
                GridCell cell = GetCell(position);

                if (!cell.IsBuildable)
                {
                    throw new InvalidOperationException($"Grid cell {position} is not buildable.");
                }

                if (cell.HasBuilding)
                {
                    throw new InvalidOperationException($"Grid cell {position} is already occupied by building '{cell.OccupiedByBuildingId}'.");
                }
            }

            if (!building.Definition.RequiresResourceNode)
            {
                return;
            }

            GridCell originCell = GetCell(building.Origin);

            if (originCell.ResourceNodeType == ResourceType.None)
            {
                throw new InvalidOperationException($"Building '{building.InstanceId}' requires a resource node at origin {building.Origin}.");
            }

            if (building.Definition.RequiredResourceType != ResourceType.None
                && originCell.ResourceNodeType != building.Definition.RequiredResourceType)
            {
                throw new InvalidOperationException(
                    $"Building '{building.InstanceId}' requires resource node '{building.Definition.RequiredResourceType}' at origin {building.Origin}.");
            }
        }
    }
}
