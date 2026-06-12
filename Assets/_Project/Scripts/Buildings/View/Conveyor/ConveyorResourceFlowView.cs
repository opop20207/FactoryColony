using UnityEngine;

namespace FactoryColony
{
    public sealed class ConveyorResourceFlowView : MonoBehaviour
    {
        private BuildingModel _building;
        private float _cellSize = 1f;
        private float _elapsed;
        private ResourceTokenView[] _tokens = new ResourceTokenView[0];

        public void Initialize(BuildingModel building, float cellSize)
        {
            _building = building;
            _cellSize = cellSize > 0f ? cellSize : 1f;
            enabled = _building != null && _building.Definition.Type == BuildingType.Conveyor;
            _elapsed = 0f;
        }

        public void SetTokens(ResourceTokenView[] tokens)
        {
            _tokens = tokens ?? new ResourceTokenView[0];
            ApplyTokenPositions();
        }

        private void Update()
        {
            if (!enabled || _tokens.Length == 0)
            {
                return;
            }

            _elapsed += Time.deltaTime;
            ApplyTokenPositions();
        }

        private void ApplyTokenPositions()
        {
            if (_building == null)
            {
                return;
            }

            Vector3 direction = GetLocalDirection(_building.Direction);
            float distance = VisualStyleConfig.ConveyorTokenMoveDistance * _cellSize;
            float duration = Mathf.Max(0.1f, VisualStyleConfig.ConveyorTokenLoopDuration);

            for (int i = 0; i < _tokens.Length; i++)
            {
                ResourceTokenView token = _tokens[i];
                if (token == null)
                {
                    continue;
                }

                float phase = Mathf.Repeat((_elapsed / duration) + (i * 0.5f), 1f);
                float offset = Mathf.Lerp(-distance, distance, phase);
                token.SetLocalPosition(direction * offset + Vector3.up * VisualStyleConfig.ConveyorTokenHeight);
            }
        }

        private static Vector3 GetLocalDirection(BuildingDirection direction)
        {
            GridPosition offset = direction.ToOffset();
            return new Vector3(offset.X, 0f, offset.Y).normalized;
        }
    }
}
