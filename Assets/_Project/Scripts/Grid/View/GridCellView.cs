using UnityEngine;

namespace FactoryColony
{
    public sealed class GridCellView : MonoBehaviour
    {
        private const float TileHeight = 0.05f;
        private const float ResourceNodeHeight = 0.18f;

        public GridPosition Position { get; private set; }
        public ResourceType ResourceNodeType { get; private set; }

        public void Initialize(GridPosition position, ResourceType resourceNodeType, float cellSize)
        {
            Position = position;
            ResourceNodeType = resourceNodeType;

            transform.localScale = new Vector3(cellSize * 0.95f, TileHeight, cellSize * 0.95f);

            Renderer renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = CreateMaterial(GetCellColor(resourceNodeType));
            }

            if (resourceNodeType != ResourceType.None)
            {
                CreateResourceNodeMarker(cellSize);
            }
        }

        private void CreateResourceNodeMarker(float cellSize)
        {
            GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            marker.name = $"ResourceNode_{Position.X}_{Position.Y}";
            marker.transform.SetParent(transform, false);
            marker.transform.localPosition = new Vector3(0f, 5f, 0f);
            marker.transform.localScale = new Vector3(
                ResourceNodeHeight / 0.95f,
                ResourceNodeHeight / TileHeight,
                ResourceNodeHeight / 0.95f);

            Renderer markerRenderer = marker.GetComponent<Renderer>();
            if (markerRenderer != null)
            {
                markerRenderer.material = CreateMaterial(GetResourceNodeColor(ResourceNodeType));
            }
        }

        private static Material CreateMaterial(Color color)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null)
            {
                shader = Shader.Find("Standard");
            }

            Material material = new Material(shader);
            material.color = color;
            return material;
        }

        private static Color GetCellColor(ResourceType resourceType)
        {
            switch (resourceType)
            {
                case ResourceType.IronOre:
                    return new Color(0.35f, 0.36f, 0.38f);
                case ResourceType.CopperOre:
                    return new Color(0.58f, 0.32f, 0.18f);
                case ResourceType.Coal:
                    return new Color(0.08f, 0.08f, 0.09f);
                default:
                    return new Color(0.42f, 0.45f, 0.42f);
            }
        }

        private static Color GetResourceNodeColor(ResourceType resourceType)
        {
            switch (resourceType)
            {
                case ResourceType.IronOre:
                    return new Color(0.72f, 0.74f, 0.78f);
                case ResourceType.CopperOre:
                    return new Color(0.95f, 0.48f, 0.22f);
                case ResourceType.Coal:
                    return new Color(0.01f, 0.01f, 0.01f);
                default:
                    return Color.clear;
            }
        }
    }
}
