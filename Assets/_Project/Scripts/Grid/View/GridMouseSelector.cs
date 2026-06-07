using System;
using UnityEngine;

namespace FactoryColony
{
    public sealed class GridMouseSelector : MonoBehaviour
    {
        private const float HighlightHeight = 0.035f;
        private const float HighlightY = 0.055f;

        [SerializeField] private Camera targetCamera;

        private GridModel _model;
        private float _cellSize = 1f;
        private GameObject _highlightObject;

        public event Action<GridPosition?> OnHoveredPositionChanged;

        public GridPosition? HoveredPosition { get; private set; }

        public void Initialize(GridModel model, float cellSize)
        {
            _model = model;
            _cellSize = cellSize > 0f ? cellSize : 1f;
            EnsureCamera();
            EnsureHighlight();
            SetHoveredPosition(null);
        }

        private void Update()
        {
            if (_model == null)
            {
                SetHoveredPosition(null);
                return;
            }

            EnsureCamera();

            if (targetCamera == null || !TryGetMouseWorldPosition(out Vector3 worldPosition))
            {
                SetHoveredPosition(null);
                return;
            }

            GridPosition position = WorldToGridPosition(worldPosition);

            if (!_model.IsInside(position))
            {
                SetHoveredPosition(null);
                return;
            }

            SetHoveredPosition(position);
        }

        private void EnsureCamera()
        {
            if (targetCamera == null)
            {
                targetCamera = Camera.main;
            }
        }

        private bool TryGetMouseWorldPosition(out Vector3 worldPosition)
        {
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = targetCamera.ScreenPointToRay(Input.mousePosition);

            if (!groundPlane.Raycast(ray, out float distance))
            {
                worldPosition = default;
                return false;
            }

            worldPosition = ray.GetPoint(distance);
            return true;
        }

        private GridPosition WorldToGridPosition(Vector3 worldPosition)
        {
            int x = Mathf.RoundToInt(worldPosition.x / _cellSize);
            int y = Mathf.RoundToInt(worldPosition.z / _cellSize);
            return new GridPosition(x, y);
        }

        private void SetHoveredPosition(GridPosition? position)
        {
            if (HoveredPosition == position)
            {
                return;
            }

            HoveredPosition = position;
            UpdateHighlight();
            OnHoveredPositionChanged?.Invoke(HoveredPosition);
        }

        private void EnsureHighlight()
        {
            if (_highlightObject != null)
            {
                return;
            }

            _highlightObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            _highlightObject.name = "GridHoverHighlight";
            _highlightObject.transform.SetParent(transform, false);
            _highlightObject.transform.localScale = new Vector3(_cellSize * 0.98f, HighlightHeight, _cellSize * 0.98f);

            Renderer renderer = _highlightObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = CreateHighlightMaterial();
            }

            _highlightObject.SetActive(false);
        }

        private void UpdateHighlight()
        {
            EnsureHighlight();

            if (!HoveredPosition.HasValue)
            {
                _highlightObject.SetActive(false);
                return;
            }

            GridPosition position = HoveredPosition.Value;
            _highlightObject.transform.localPosition = new Vector3(
                position.X * _cellSize,
                HighlightY,
                position.Y * _cellSize);
            _highlightObject.transform.localScale = new Vector3(_cellSize * 0.98f, HighlightHeight, _cellSize * 0.98f);
            _highlightObject.SetActive(true);
        }

        private static Material CreateHighlightMaterial()
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null)
            {
                shader = Shader.Find("Standard");
            }

            Material material = new Material(shader);
            material.color = new Color(1f, 0.95f, 0.15f, 0.55f);
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
