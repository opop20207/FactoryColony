using UnityEngine;

namespace FactoryColony
{
    public static class VisualSceneSetup
    {
        public static void ApplyDebugLighting()
        {
            Light directionalLight = FindDirectionalLight();

            if (directionalLight == null)
            {
                GameObject lightObject = new GameObject("FactoryDebugDirectionalLight");
                directionalLight = lightObject.AddComponent<Light>();
                directionalLight.type = LightType.Directional;
            }

            directionalLight.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            directionalLight.intensity = 1.15f;
            directionalLight.shadows = LightShadows.Soft;
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.46f, 0.49f, 0.52f);
        }

        private static Light FindDirectionalLight()
        {
            Light[] lights = Object.FindObjectsOfType<Light>();

            foreach (Light light in lights)
            {
                if (light.type == LightType.Directional)
                {
                    return light;
                }
            }

            return null;
        }
    }
}
