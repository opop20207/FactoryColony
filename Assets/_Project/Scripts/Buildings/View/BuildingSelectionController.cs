using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace FactoryColony
{
    public sealed class BuildingSelectionController : MonoBehaviour
    {
        private const float HighlightHeight = 0.08f;
        private const float HighlightY = 0.82f;
        private const float SelectionHighlightHeight = 0.1f;
        private const float SelectionHighlightY = 0.92f;

        private GridModel _model;
        private GridMouseSelector _selector;
        private BuildingViewFactory _buildingViewFactory;
        private BaseInventoryModel _baseInventory;
        private float _cellSize = 1f;
        private GameObject _highlightObject;
        private Material _highlightMaterial;
        private GameObject _selectionHighlightObject;
        private Material _selectionHighlightMaterial;

        public BuildingModel HoveredBuilding { get; private set; }
        public string HoveredBuildingId
        {
            get { return HoveredBuilding != null ? HoveredBuilding.InstanceId : string.Empty; }
        }

        public BuildingModel SelectedBuilding { get; private set; }
        public string SelectedBuildingId
        {
            get { return SelectedBuilding != null ? SelectedBuilding.InstanceId : string.Empty; }
        }

        public bool HasSelectedBuilding
        {
            get { return SelectedBuilding != null; }
        }

        public string LastRemovalResult { get; private set; } = "None";
        public bool LastRemovalSucceeded { get; private set; }

        public void Initialize(
            GridModel model,
            GridMouseSelector selector,
            BuildingViewFactory buildingViewFactory,
            float cellSize)
        {
            Initialize(model, selector, buildingViewFactory, cellSize, null);
        }

        public void Initialize(
            GridModel model,
            GridMouseSelector selector,
            BuildingViewFactory buildingViewFactory,
            float cellSize,
            BaseInventoryModel baseInventory)
        {
            _model = model;
            _selector = selector;
            _buildingViewFactory = buildingViewFactory;
            _baseInventory = baseInventory;
            _cellSize = cellSize > 0f ? cellSize : 1f;
            EnsureHighlightObject();
            EnsureSelectionHighlightObject();
            RefreshHoveredBuilding();
            RefreshSelectedBuilding();
        }

        private void Update()
        {
            RefreshHoveredBuilding();
            RefreshSelectedBuilding();
            HandleSelectionInput();
            HandleRemovalInput();
        }

        private void RefreshHoveredBuilding()
        {
            HoveredBuilding = null;

            if (_model == null
                || _selector == null
                || !_selector.HoveredPosition.HasValue
                || !_model.TryGetCell(_selector.HoveredPosition.Value, out GridCell cell)
                || !cell.HasBuilding
                || !_model.TryGetBuilding(cell.OccupiedByBuildingId, out BuildingModel building))
            {
                HideHighlight();
                return;
            }

            HoveredBuilding = building;
            UpdateHighlight(_highlightObject, building, HighlightY, HighlightHeight);
        }

        private void RefreshSelectedBuilding()
        {
            if (SelectedBuilding == null)
            {
                HideSelectionHighlight();
                return;
            }

            if (_model == null
                || !_model.TryGetBuilding(SelectedBuilding.InstanceId, out BuildingModel currentBuilding))
            {
                ClearSelection();
                return;
            }

            SelectedBuilding = currentBuilding;
            UpdateHighlight(_selectionHighlightObject, SelectedBuilding, SelectionHighlightY, SelectionHighlightHeight);
        }

        public void ClearSelection()
        {
            SelectedBuilding = null;
            HideSelectionHighlight();
        }

        public void SelectBuilding(BuildingModel building)
        {
            if (building == null)
            {
                ClearSelection();
                return;
            }

            SelectedBuilding = building;
            UpdateHighlight(_selectionHighlightObject, SelectedBuilding, SelectionHighlightY, SelectionHighlightHeight);
        }

        private void HandleSelectionInput()
        {
            if (PlayerInputReader.WasKeyPressedThisFrame(Key.Escape))
            {
                ClearSelection();
                return;
            }

            if (!PlayerInputReader.WasRightMousePressedThisFrame() || IsPointerOverUi())
            {
                return;
            }

            if (HoveredBuilding == null)
            {
                return;
            }

            SelectedBuilding = HoveredBuilding;
            UpdateHighlight(_selectionHighlightObject, SelectedBuilding, SelectionHighlightY, SelectionHighlightHeight);
        }

        private void HandleRemovalInput()
        {
            if (!PlayerInputReader.WasKeyPressedThisFrame(Key.Delete)
                && !PlayerInputReader.WasKeyPressedThisFrame(Key.Backspace))
            {
                return;
            }

            if (HoveredBuilding == null)
            {
                return;
            }

            string instanceId = HoveredBuilding.InstanceId;

            if (_model == null || !_model.TryRemoveBuilding(instanceId))
            {
                LastRemovalSucceeded = false;
                LastRemovalResult = "Failed to remove " + instanceId + ".";
                Debug.LogWarning(LastRemovalResult);
                return;
            }

            LastRemovalSucceeded = true;
            LastRemovalResult = "Removed " + instanceId + RefundBuildCost(HoveredBuilding) + ".";

            if (SelectedBuilding != null && SelectedBuilding.InstanceId == instanceId)
            {
                ClearSelection();
            }

            HoveredBuilding = null;
            HideHighlight();

            if (_buildingViewFactory != null)
            {
                _buildingViewFactory.Build(_model, _cellSize);
            }
        }

        private string RefundBuildCost(BuildingModel building)
        {
            if (_baseInventory == null || building == null || building.Definition.BuildCost.Count == 0)
            {
                return string.Empty;
            }

            string refundSummary = string.Empty;

            foreach (System.Collections.Generic.KeyValuePair<ResourceType, int> cost in building.Definition.BuildCost)
            {
                int refundAmount = cost.Value / 2;

                if (refundAmount <= 0)
                {
                    continue;
                }

                _baseInventory.Add(cost.Key, refundAmount);

                if (!string.IsNullOrEmpty(refundSummary))
                {
                    refundSummary += ", ";
                }

                refundSummary += cost.Key + " +" + refundAmount;
            }

            return string.IsNullOrEmpty(refundSummary) ? string.Empty : " Refund: " + refundSummary;
        }

        private void EnsureHighlightObject()
        {
            if (_highlightObject != null)
            {
                return;
            }

            _highlightObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            _highlightObject.name = "BuildingHoverHighlight";
            _highlightObject.transform.SetParent(transform, false);

            Renderer renderer = _highlightObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                _highlightMaterial = MaterialFactory.CreateTransparent(new Color(1f, 1f, 1f, 0.32f));
                renderer.material = _highlightMaterial;
            }

            _highlightObject.SetActive(false);
        }

        private void EnsureSelectionHighlightObject()
        {
            if (_selectionHighlightObject != null)
            {
                return;
            }

            _selectionHighlightObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            _selectionHighlightObject.name = "BuildingSelectionHighlight";
            _selectionHighlightObject.transform.SetParent(transform, false);

            Renderer renderer = _selectionHighlightObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                _selectionHighlightMaterial = MaterialFactory.CreateTransparent(VisualStyleConfig.SelectionColor);
                renderer.material = _selectionHighlightMaterial;
            }

            _selectionHighlightObject.SetActive(false);
        }

        private void UpdateHighlight(GameObject highlightObject, BuildingModel building, float yPosition, float highlightHeight)
        {
            if (highlightObject == null)
            {
                return;
            }

            int minX = building.OccupiedPositions[0].X;
            int maxX = building.OccupiedPositions[0].X;
            int minY = building.OccupiedPositions[0].Y;
            int maxY = building.OccupiedPositions[0].Y;

            for (int i = 1; i < building.OccupiedPositions.Count; i++)
            {
                GridPosition position = building.OccupiedPositions[i];
                minX = Mathf.Min(minX, position.X);
                maxX = Mathf.Max(maxX, position.X);
                minY = Mathf.Min(minY, position.Y);
                maxY = Mathf.Max(maxY, position.Y);
            }

            int occupiedWidth = maxX - minX + 1;
            int occupiedHeight = maxY - minY + 1;
            float centerX = (minX + maxX) * 0.5f;
            float centerY = (minY + maxY) * 0.5f;

            highlightObject.transform.localPosition = new Vector3(centerX * _cellSize, yPosition, centerY * _cellSize);
            highlightObject.transform.localScale = new Vector3(
                occupiedWidth * _cellSize,
                highlightHeight,
                occupiedHeight * _cellSize);
            highlightObject.SetActive(true);
        }

        private void HideHighlight()
        {
            if (_highlightObject != null)
            {
                _highlightObject.SetActive(false);
            }
        }

        private void HideSelectionHighlight()
        {
            if (_selectionHighlightObject != null)
            {
                _selectionHighlightObject.SetActive(false);
            }
        }

        private static bool IsPointerOverUi()
        {
            return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
        }
    }
}
