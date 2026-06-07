using UnityEngine;
using UnityEngine.EventSystems;

namespace FactoryColony
{
    public sealed class BuildingPlacementController : MonoBehaviour
    {
        private GridModel _model;
        private GridMouseSelector _selector;
        private BuildingPlacementPreview _preview;
        private BuildingViewFactory _buildingViewFactory;
        private BaseInventoryModel _baseInventory;
        private float _cellSize = 1f;
        private int _nextInstanceNumber = 1;

        public string LastPlacementResult { get; private set; } = "None";
        public int TotalBuildings
        {
            get
            {
                if (_model == null)
                {
                    return 0;
                }

                int count = 0;

                foreach (BuildingModel building in _model.GetAllBuildings())
                {
                    count++;
                }

                return count;
            }
        }

        public void Initialize(
            GridModel model,
            GridMouseSelector selector,
            BuildingPlacementPreview preview,
            BuildingViewFactory buildingViewFactory,
            float cellSize)
        {
            Initialize(model, selector, preview, buildingViewFactory, cellSize, null);
        }

        public void Initialize(
            GridModel model,
            GridMouseSelector selector,
            BuildingPlacementPreview preview,
            BuildingViewFactory buildingViewFactory,
            float cellSize,
            BaseInventoryModel baseInventory)
        {
            _model = model;
            _selector = selector;
            _preview = preview;
            _buildingViewFactory = buildingViewFactory;
            _baseInventory = baseInventory;
            _cellSize = cellSize > 0f ? cellSize : 1f;
            LastPlacementResult = "Ready";
        }

        private void Update()
        {
            if (!Input.GetMouseButtonDown(0))
            {
                return;
            }

            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            TryPlaceSelectedBuilding();
        }

        private void TryPlaceSelectedBuilding()
        {
            if (_model == null || _selector == null || _preview == null || _buildingViewFactory == null)
            {
                SetPlacementFailure("Placement system is not initialized.");
                return;
            }

            if (!_preview.HasSelection || _preview.SelectedDefinition == null)
            {
                return;
            }

            if (!_preview.CurrentOrigin.HasValue)
            {
                SetPlacementFailure("No hovered grid cell.");
                return;
            }

            if (!_preview.CanPlaceCurrentPreview)
            {
                SetPlacementFailure("Cannot place " + _preview.SelectedDefinition.DisplayName + " at " + _preview.CurrentOrigin.Value + ": " + _preview.CannotPlaceReason + ".");
                return;
            }

            if (_baseInventory != null && !_baseInventory.CanAfford(_preview.SelectedDefinition.BuildCost))
            {
                SetPlacementFailure("Insufficient resources for " + _preview.SelectedDefinition.DisplayName + ".");
                return;
            }

            BuildingModel building = new BuildingModel(
                CreateInstanceId(_preview.SelectedDefinition.Type),
                _preview.SelectedDefinition,
                _preview.CurrentOrigin.Value,
                _preview.CurrentDirection);

            if (!_model.TryPlaceBuilding(building))
            {
                SetPlacementFailure("Grid rejected placement for " + building.InstanceId + ".");
                return;
            }

            if (_baseInventory != null && !_baseInventory.TrySpend(_preview.SelectedDefinition.BuildCost))
            {
                _model.TryRemoveBuilding(building.InstanceId);
                SetPlacementFailure("Placed " + building.InstanceId + " but failed to spend build cost.");
                return;
            }

            _buildingViewFactory.Build(_model, _cellSize);
            LastPlacementResult = "Placed " + building.InstanceId + " at " + building.Origin + ".";
        }

        private string CreateInstanceId(BuildingType type)
        {
            string id;

            do
            {
                id = type + "_" + _nextInstanceNumber.ToString("000");
                _nextInstanceNumber++;
            }
            while (_model != null && _model.TryGetBuilding(id, out BuildingModel ignored));

            return id;
        }

        private void SetPlacementFailure(string message)
        {
            LastPlacementResult = message;
            Debug.LogWarning(message);
        }
    }
}
