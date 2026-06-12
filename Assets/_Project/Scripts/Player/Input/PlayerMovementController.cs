using UnityEngine;

namespace FactoryColony
{
    public sealed class PlayerMovementController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 4f;

        private bool _hasBounds;
        private Vector2 _minBounds;
        private Vector2 _maxBounds;

        public void InitializeBounds(float minX, float minZ, float maxX, float maxZ)
        {
            _hasBounds = true;
            _minBounds = new Vector2(minX, minZ);
            _maxBounds = new Vector2(maxX, maxZ);
        }

        private void Update()
        {
            Vector2 input = PlayerInputReader.GetMoveInput();

            if (input.sqrMagnitude <= 0.0001f)
            {
                return;
            }

            Vector3 direction = new Vector3(input.x, 0f, input.y).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;
            ClampToBounds();
            transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
        }

        private void ClampToBounds()
        {
            if (!_hasBounds)
            {
                return;
            }

            Vector3 position = transform.position;
            position.x = Mathf.Clamp(position.x, _minBounds.x, _maxBounds.x);
            position.z = Mathf.Clamp(position.z, _minBounds.y, _maxBounds.y);
            transform.position = position;
        }
    }
}
