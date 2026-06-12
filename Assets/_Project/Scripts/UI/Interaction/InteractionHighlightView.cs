using UnityEngine;

namespace FactoryColony
{
    public sealed class InteractionHighlightView : MonoBehaviour
    {
        private const float HighlightY = 1.02f;
        private const float HighlightHeight = 0.08f;

        private PlayerInteractionController _interactionController;
        private float _cellSize = 1f;
        private GameObject _highlightObject;

        public void Initialize(PlayerInteractionController interactionController, float cellSize)
        {
            _interactionController = interactionController;
            _cellSize = cellSize > 0f ? cellSize : 1f;
            EnsureHighlightObject();
        }

        private void Update()
        {
            if (_interactionController == null || _interactionController.NearbyBuilding == null)
            {
                Hide();
                return;
            }

            UpdateHighlight(_interactionController.NearbyBuilding);
        }

        private void EnsureHighlightObject()
        {
            if (_highlightObject != null)
            {
                return;
            }

            _highlightObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            _highlightObject.name = "InteractionHighlight";
            _highlightObject.transform.SetParent(transform, false);

            Renderer renderer = _highlightObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = MaterialFactory.CreateTransparent(VisualStyleConfig.InteractionColor);
            }

            _highlightObject.SetActive(false);
        }

        private void UpdateHighlight(BuildingModel building)
        {
            int minX = building.OccupiedPositions[0].X;
            int maxX = minX;
            int minY = building.OccupiedPositions[0].Y;
            int maxY = minY;

            foreach (GridPosition position in building.OccupiedPositions)
            {
                minX = Mathf.Min(minX, position.X);
                maxX = Mathf.Max(maxX, position.X);
                minY = Mathf.Min(minY, position.Y);
                maxY = Mathf.Max(maxY, position.Y);
            }

            int width = maxX - minX + 1;
            int height = maxY - minY + 1;
            _highlightObject.transform.localPosition = new Vector3((minX + maxX) * 0.5f * _cellSize, HighlightY, (minY + maxY) * 0.5f * _cellSize);
            _highlightObject.transform.localScale = new Vector3(width * _cellSize, HighlightHeight, height * _cellSize);
            _highlightObject.SetActive(true);
        }

        private void Hide()
        {
            if (_highlightObject != null)
            {
                _highlightObject.SetActive(false);
            }
        }
    }
}
