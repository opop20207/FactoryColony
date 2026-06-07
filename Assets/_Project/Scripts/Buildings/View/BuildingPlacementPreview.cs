using UnityEngine;

namespace FactoryColony
{
    public sealed class BuildingPlacementPreview : MonoBehaviour
    {
        private const string PreviewInstanceId = "preview-only";
        private const float PreviewHeight = 0.28f;
        private const float PreviewY = 0.32f;

        private GridModel _model;
        private GridMouseSelector _selector;
        private float _cellSize = 1f;
        private GameObject _previewObject;
        private Renderer _previewRenderer;
        private Material _previewMaterial;
        private bool _canPlace;

        public BuildingDefinition SelectedBuilding { get; private set; }
        public BuildingDirection Direction { get; private set; } = BuildingDirection.North;
        public bool HasSelection
        {
            get { return SelectedBuilding != null; }
        }

        public bool HasHoveredCell
        {
            get { return _selector != null && _selector.HoveredPosition.HasValue; }
        }

        public bool CanPlace
        {
            get { return HasSelection && HasHoveredCell && _canPlace; }
        }

        public void Initialize(GridModel model, GridMouseSelector selector, float cellSize)
        {
            _model = model;
            _selector = selector;
            _cellSize = cellSize > 0f ? cellSize : 1f;
            EnsurePreviewObject();
            HidePreview();
        }

        public void SetSelectedBuilding(BuildingDefinition definition)
        {
            SelectedBuilding = definition;
            Direction = BuildingDirection.North;
            UpdatePreview();
        }

        public void ClearSelection()
        {
            SelectedBuilding = null;
            _canPlace = false;
            HidePreview();
        }

        private void Update()
        {
            HandleRotationInput();
            UpdatePreview();
        }

        private void HandleRotationInput()
        {
            if (SelectedBuilding == null || !SelectedBuilding.IsRotatable || !Input.GetKeyDown(KeyCode.R))
            {
                return;
            }

            Direction = GetNextDirection(Direction);
        }

        private void UpdatePreview()
        {
            if (_model == null
                || _selector == null
                || SelectedBuilding == null
                || !_selector.HoveredPosition.HasValue)
            {
                _canPlace = false;
                HidePreview();
                return;
            }

            EnsurePreviewObject();

            GridPosition origin = _selector.HoveredPosition.Value;
            BuildingModel previewBuilding = new BuildingModel(PreviewInstanceId, SelectedBuilding, origin, Direction);
            _canPlace = _model.CanPlaceBuilding(previewBuilding);

            ApplyPreviewTransform(previewBuilding);
            ApplyPreviewColor(_canPlace);
            _previewObject.SetActive(true);
        }

        private void ApplyPreviewTransform(BuildingModel previewBuilding)
        {
            int minX = previewBuilding.OccupiedPositions[0].X;
            int maxX = previewBuilding.OccupiedPositions[0].X;
            int minY = previewBuilding.OccupiedPositions[0].Y;
            int maxY = previewBuilding.OccupiedPositions[0].Y;

            for (int i = 1; i < previewBuilding.OccupiedPositions.Count; i++)
            {
                GridPosition position = previewBuilding.OccupiedPositions[i];
                minX = Mathf.Min(minX, position.X);
                maxX = Mathf.Max(maxX, position.X);
                minY = Mathf.Min(minY, position.Y);
                maxY = Mathf.Max(maxY, position.Y);
            }

            int width = maxX - minX + 1;
            int height = maxY - minY + 1;
            float centerX = (minX + maxX) * 0.5f;
            float centerY = (minY + maxY) * 0.5f;

            _previewObject.transform.localPosition = new Vector3(centerX * _cellSize, PreviewY, centerY * _cellSize);
            _previewObject.transform.localScale = new Vector3(
                width * _cellSize * 0.92f,
                PreviewHeight,
                height * _cellSize * 0.92f);
        }

        private void ApplyPreviewColor(bool canPlace)
        {
            if (_previewMaterial == null)
            {
                return;
            }

            _previewMaterial.color = canPlace
                ? new Color(0.15f, 0.95f, 0.32f, 0.55f)
                : new Color(1f, 0.15f, 0.12f, 0.55f);
        }

        private void EnsurePreviewObject()
        {
            if (_previewObject != null)
            {
                return;
            }

            _previewObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            _previewObject.name = "BuildingPlacementPreview";
            _previewObject.transform.SetParent(transform, false);
            _previewRenderer = _previewObject.GetComponent<Renderer>();

            if (_previewRenderer != null)
            {
                _previewMaterial = CreatePreviewMaterial();
                _previewRenderer.material = _previewMaterial;
            }
        }

        private void HidePreview()
        {
            if (_previewObject != null)
            {
                _previewObject.SetActive(false);
            }
        }

        private static BuildingDirection GetNextDirection(BuildingDirection direction)
        {
            switch (direction)
            {
                case BuildingDirection.North:
                    return BuildingDirection.East;
                case BuildingDirection.East:
                    return BuildingDirection.South;
                case BuildingDirection.South:
                    return BuildingDirection.West;
                default:
                    return BuildingDirection.North;
            }
        }

        private static Material CreatePreviewMaterial()
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null)
            {
                shader = Shader.Find("Standard");
            }

            Material material = new Material(shader);
            material.color = new Color(0.15f, 0.95f, 0.32f, 0.55f);
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
