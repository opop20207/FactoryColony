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

            transform.localScale = new Vector3(cellSize * 0.92f, TileHeight, cellSize * 0.92f);

            Renderer renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = MaterialFactory.CreateOpaque(GetCellColor(resourceNodeType));
            }

            if (resourceNodeType != ResourceType.None)
            {
                CreateResourceNodeMarker(cellSize);
            }
        }

        private void CreateResourceNodeMarker(float cellSize)
        {
            CreateResourceRock("ResourceNode_" + Position.X + "_" + Position.Y + "_A", new Vector3(-0.16f, 3.9f, 0.04f), new Vector3(0.22f, 2.7f, 0.2f), cellSize);
            CreateResourceRock("ResourceNode_" + Position.X + "_" + Position.Y + "_B", new Vector3(0.11f, 3.5f, -0.13f), new Vector3(0.18f, 2.2f, 0.18f), cellSize);
            CreateResourceRock("ResourceNode_" + Position.X + "_" + Position.Y + "_C", new Vector3(0.16f, 3.2f, 0.14f), new Vector3(0.14f, 1.8f, 0.14f), cellSize);
        }

        private void CreateResourceRock(string objectName, Vector3 localPosition, Vector3 localScale, float cellSize)
        {
            GameObject rock = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            rock.name = objectName;
            rock.transform.SetParent(transform, false);
            rock.transform.localPosition = localPosition;
            rock.transform.localScale = new Vector3(
                localScale.x * cellSize,
                localScale.y * ResourceNodeHeight,
                localScale.z * cellSize);

            Renderer markerRenderer = rock.GetComponent<Renderer>();
            if (markerRenderer != null)
            {
                markerRenderer.material = MaterialFactory.CreateOpaque(VisualStyleConfig.GetResourceColor(ResourceNodeType));
            }
        }

        private static Color GetCellColor(ResourceType resourceType)
        {
            switch (resourceType)
            {
                case ResourceType.IronOre:
                    return VisualStyleConfig.GroundResourceTint;
                case ResourceType.CopperOre:
                    return VisualStyleConfig.GroundResourceTint;
                case ResourceType.Coal:
                    return VisualStyleConfig.GroundResourceTint;
                default:
                    return VisualStyleConfig.GroundColor;
            }
        }
    }
}
