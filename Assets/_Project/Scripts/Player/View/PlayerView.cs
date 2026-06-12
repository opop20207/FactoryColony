using UnityEngine;

namespace FactoryColony
{
    public sealed class PlayerView : MonoBehaviour
    {
        private GameObject _root;

        public void Initialize(float cellSize)
        {
            if (_root != null)
            {
                return;
            }

            float scale = Mathf.Max(0.5f, cellSize);
            _root = new GameObject("PlayerVisualRoot");
            _root.transform.SetParent(transform, false);
            _root.transform.localPosition = Vector3.zero;

            GameObject body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            body.name = "Body";
            body.transform.SetParent(_root.transform, false);
            body.transform.localPosition = new Vector3(0f, 0.34f * scale, 0f);
            body.transform.localScale = new Vector3(0.28f * scale, 0.34f * scale, 0.28f * scale);
            SetMaterial(body, new Color(0.24f, 0.78f, 0.92f));

            GameObject head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            head.name = "Head";
            head.transform.SetParent(_root.transform, false);
            head.transform.localPosition = new Vector3(0f, 0.82f * scale, 0f);
            head.transform.localScale = Vector3.one * 0.22f * scale;
            SetMaterial(head, new Color(0.95f, 0.92f, 0.74f));

            GameObject forwardMarker = GameObject.CreatePrimitive(PrimitiveType.Cube);
            forwardMarker.name = "ForwardMarker";
            forwardMarker.transform.SetParent(_root.transform, false);
            forwardMarker.transform.localPosition = new Vector3(0f, 0.55f * scale, 0.24f * scale);
            forwardMarker.transform.localScale = new Vector3(0.12f * scale, 0.08f * scale, 0.28f * scale);
            SetMaterial(forwardMarker, new Color(1f, 0.74f, 0.25f));
        }

        private static void SetMaterial(GameObject target, Color color)
        {
            Renderer renderer = target.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = MaterialFactory.CreateOpaque(color);
            }
        }
    }
}
