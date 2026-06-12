using UnityEngine;

namespace FactoryColony
{
    public sealed class CameraController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 10f;
        [SerializeField] private float zoomSpeed = 5f;
        [SerializeField] private float minOrthographicSize = 4f;
        [SerializeField] private float maxOrthographicSize = 20f;

        private Camera _camera;

        private void Awake()
        {
            _camera = GetComponent<Camera>();
        }

        private void Update()
        {
            if (_camera == null || !_camera.orthographic)
            {
                return;
            }

            MoveCamera();
            ZoomCamera();
        }

        private void MoveCamera()
        {
            if (!PlayerInputReader.IsKeyPressed(UnityEngine.InputSystem.Key.LeftShift)
                && !PlayerInputReader.IsKeyPressed(UnityEngine.InputSystem.Key.RightShift))
            {
                return;
            }

            Vector2 moveInput = PlayerInputReader.GetMoveInput();
            float horizontal = moveInput.x;
            float vertical = moveInput.y;

            if (Mathf.Approximately(horizontal, 0f) && Mathf.Approximately(vertical, 0f))
            {
                return;
            }

            Vector3 movement = new Vector3(horizontal, 0f, vertical).normalized;
            transform.position += movement * moveSpeed * Time.deltaTime;
        }

        private void ZoomCamera()
        {
            float scrollDelta = PlayerInputReader.GetMouseScrollY();

            if (Mathf.Approximately(scrollDelta, 0f))
            {
                return;
            }

            float orthographicSize = _camera.orthographicSize - scrollDelta * zoomSpeed;
            _camera.orthographicSize = Mathf.Clamp(orthographicSize, minOrthographicSize, maxOrthographicSize);
        }
    }
}
