using UnityEngine;

namespace FactoryColony
{
    public sealed class BuildingSelectionController : MonoBehaviour
    {
        private const float HighlightHeight = 0.08f;
        private const float HighlightY = 0.82f;

        private GridModel _model;
        private GridMouseSelector _selector;
        private BuildingViewFactory _buildingViewFactory;
        private BaseInventoryModel _baseInventory;
        private float _cellSize = 1f;
        private GameObject _highlightObject;
        private Material _highlightMaterial;

        public BuildingModel HoveredBuilding { get; private set; }
        public string HoveredBuildingId
        {
            get { return HoveredBuilding != null ? HoveredBuilding.InstanceId : string.Empty; }
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
            RefreshHoveredBuilding();
        }

        private void Update()
        {
            RefreshHoveredBuilding();
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
            UpdateHighlight(building);
        }

        private void HandleRemovalInput()
        {
            if (!Input.GetKeyDown(KeyCode.Delete) && !Input.GetKeyDown(KeyCode.Backspace))
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
                _highlightMaterial = CreateHighlightMaterial();
                renderer.material = _highlightMaterial;
            }

            _highlightObject.SetActive(false);
        }

        private void UpdateHighlight(BuildingModel building)
        {
            EnsureHighlightObject();

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

            int width = maxX - minX + 1;
            int height = maxY - minY + 1;
            float centerX = (minX + maxX) * 0.5f;
            float centerY = (minY + maxY) * 0.5f;

            _highlightObject.transform.localPosition = new Vector3(centerX * _cellSize, HighlightY, centerY * _cellSize);
            _highlightObject.transform.localScale = new Vector3(
                width * _cellSize,
                HighlightHeight,
                height * _cellSize);
            _highlightObject.SetActive(true);
        }

        private void HideHighlight()
        {
            if (_highlightObject != null)
            {
                _highlightObject.SetActive(false);
            }
        }

        private static Material CreateHighlightMaterial()
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null)
            {
                shader = Shader.Find("Standard");
            }

            Material material = new Material(shader);
            material.color = new Color(1f, 1f, 1f, 0.35f);
            material.SetFloat("_Surface", 1f);
            material.SetFloat("_Blend", 0f);
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            material.SetInt("_ZWrite", 0);
            material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
            return material;
        }
    }
}
