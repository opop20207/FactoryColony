using UnityEngine;

namespace FactoryColony
{
    public sealed class FactoryCameraSetup : MonoBehaviour
    {
        private void Start()
        {
            Camera camera = Camera.main;

            if (camera == null)
            {
                GameObject cameraObject = new GameObject("Main Camera");
                cameraObject.tag = "MainCamera";
                camera = cameraObject.AddComponent<Camera>();
            }

            camera.orthographic = true;
            camera.orthographicSize = 8f;
            camera.transform.position = new Vector3(5f, 10f, -8f);
            camera.transform.rotation = Quaternion.Euler(60f, 0f, 0f);
        }
    }
}
