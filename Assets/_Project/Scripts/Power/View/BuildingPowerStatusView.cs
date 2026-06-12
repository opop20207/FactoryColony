using UnityEngine;

namespace FactoryColony
{
    public sealed class BuildingPowerStatusView : MonoBehaviour
    {
        private BuildingModel _building;
        private PowerStatusService _powerStatusService;
        private float _cellSize = 1f;
        private GameObject _marker;
        private Renderer _markerRenderer;
        private BuildingOperationalStatus _lastStatus = BuildingOperationalStatus.None;

        public void Initialize(
            BuildingModel building,
            PowerStatusService powerStatusService,
            float cellSize)
        {
            _building = building;
            _powerStatusService = powerStatusService;
            _cellSize = cellSize > 0f ? cellSize : 1f;
            EnsureMarker();
            Refresh();
        }

        public void Refresh()
        {
            EnsureMarker();

            BuildingOperationalStatus status = _powerStatusService != null
                ? _powerStatusService.GetStatusFor(_building)
                : BuildingOperationalStatus.None;

            bool shouldShow = status == BuildingOperationalStatus.NoPower
                || status == BuildingOperationalStatus.Operating
                || status == BuildingOperationalStatus.Idle;

            _marker.SetActive(shouldShow);

            if (!shouldShow)
            {
                _lastStatus = status;
                return;
            }

            _marker.transform.localPosition = GetMarkerPosition();
            _marker.transform.localScale = GetMarkerScale(status);

            if (status != _lastStatus)
            {
                _markerRenderer.sharedMaterial = MaterialFactory.CreateOpaque(GetColor(status));
                _lastStatus = status;
            }
        }

        private void EnsureMarker()
        {
            if (_marker != null)
            {
                return;
            }

            _marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            _marker.name = "PowerStatusMarker";
            _marker.transform.SetParent(transform, false);
            _markerRenderer = _marker.GetComponent<Renderer>();

            Collider markerCollider = _marker.GetComponent<Collider>();
            if (markerCollider != null)
            {
                if (Application.isPlaying)
                {
                    Destroy(markerCollider);
                }
                else
                {
                    DestroyImmediate(markerCollider);
                }
            }
        }

        private Vector3 GetMarkerPosition()
        {
            float width = _building != null ? _building.Definition.Width : 1f;
            float height = _building != null ? _building.Definition.Height : 1f;
            float offset = Mathf.Max(width, height) * _cellSize * 0.34f;
            return new Vector3(offset, 0.72f * _cellSize, offset);
        }

        private static Vector3 GetMarkerScale(BuildingOperationalStatus status)
        {
            float size = status == BuildingOperationalStatus.NoPower ? 0.18f : 0.13f;
            return new Vector3(size, size, size);
        }

        private static Color GetColor(BuildingOperationalStatus status)
        {
            switch (status)
            {
                case BuildingOperationalStatus.Operating:
                    return VisualStyleConfig.PowerOkColor;
                case BuildingOperationalStatus.NoPower:
                    return VisualStyleConfig.PowerLowColor;
                case BuildingOperationalStatus.Idle:
                    return VisualStyleConfig.PowerIdleColor;
                default:
                    return Color.clear;
            }
        }
    }
}
