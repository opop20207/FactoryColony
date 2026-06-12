using UnityEngine;
using UnityEngine.InputSystem;

namespace FactoryColony
{
    public sealed class PlayerCameraFollow : MonoBehaviour
    {
        [SerializeField] private bool followEnabled;
        [SerializeField] private float followSpeed = 8f;

        private Transform _target;
        private Vector3 _offset;

        public bool FollowEnabled
        {
            get { return followEnabled; }
        }

        public void Initialize(Transform target)
        {
            _target = target;
            if (_target != null)
            {
                _offset = transform.position - _target.position;
            }
        }

        private void Update()
        {
            if (PlayerInputReader.WasKeyPressedThisFrame(Key.F))
            {
                followEnabled = !followEnabled;
                if (_target != null)
                {
                    _offset = transform.position - _target.position;
                }
            }

            if (!followEnabled || _target == null)
            {
                return;
            }

            Vector3 targetPosition = _target.position + _offset;
            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
        }
    }
}
