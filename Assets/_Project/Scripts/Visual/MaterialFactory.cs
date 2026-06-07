using UnityEngine;

namespace FactoryColony
{
    public static class MaterialFactory
    {
        public static Material CreateOpaque(Color color)
        {
            Material material = new Material(FindLitShader());
            material.color = color;
            return material;
        }

        public static Material CreateTransparent(Color color)
        {
            Material material = new Material(FindLitShader());
            material.color = color;
            material.SetFloat("_Surface", 1f);
            material.SetFloat("_Blend", 0f);
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            material.SetInt("_ZWrite", 0);
            material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
            return material;
        }

        private static Shader FindLitShader()
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader != null)
            {
                return shader;
            }

            shader = Shader.Find("Universal Render Pipeline/Unlit");
            if (shader != null)
            {
                return shader;
            }

            return Shader.Find("Standard");
        }
    }
}
