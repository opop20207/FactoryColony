using UnityEngine;

namespace FactoryColony
{
    public sealed class ResourceTokenView : MonoBehaviour
    {
        private const float BaseSize = 0.16f;
        private const float MaxSize = 0.34f;

        private GameObject _body;

        public ResourceType Type { get; private set; }
        public int Amount { get; private set; }

        public void SetLocalPosition(Vector3 localPosition)
        {
            transform.localPosition = localPosition;
        }

        public void Initialize(ResourceType type, int amount)
        {
            Type = type;
            Amount = amount;
            gameObject.name = "ResourceToken_" + type + "_" + amount;
            EnsureBody();

            float size = Mathf.Clamp(BaseSize + Mathf.Log(Mathf.Max(1, amount), 2f) * 0.035f, BaseSize, MaxSize);
            _body.transform.localScale = new Vector3(size, size, size);

            Renderer renderer = _body.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = MaterialFactory.CreateOpaque(VisualStyleConfig.GetResourceColor(type));
            }
        }

        private void EnsureBody()
        {
            if (_body != null)
            {
                return;
            }

            _body = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            _body.name = "TokenBody";
            _body.transform.SetParent(transform, false);
            _body.transform.localPosition = Vector3.zero;
        }
    }
}
