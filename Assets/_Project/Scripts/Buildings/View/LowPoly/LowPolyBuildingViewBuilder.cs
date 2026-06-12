using UnityEngine;

namespace FactoryColony
{
    public static class LowPolyBuildingViewBuilder
    {
        public static void Build(BuildingModel building, Transform parent, float width, float depth, float cellSize)
        {
            switch (building.Definition.Type)
            {
                case BuildingType.Miner:
                    BuildMiner(building, parent, width, depth, cellSize);
                    break;
                case BuildingType.Conveyor:
                    BuildConveyor(building, parent, width, depth, cellSize);
                    break;
                case BuildingType.Smelter:
                    BuildSmelter(building, parent, width, depth, cellSize);
                    break;
                case BuildingType.Storage:
                    BuildStorage(parent, width, depth);
                    break;
                case BuildingType.Generator:
                    BuildGenerator(parent, width, depth);
                    break;
                case BuildingType.Assembler:
                    BuildAssembler(building, parent, width, depth, cellSize);
                    break;
                case BuildingType.ResearchLab:
                    BuildResearchLab(building, parent, width, depth, cellSize);
                    break;
                default:
                    CreatePart("Body", PrimitiveType.Cube, parent, Vector3.zero, new Vector3(width * 0.78f, 0.42f, depth * 0.78f), VisualStyleConfig.GetBuildingColor(building.Definition.Type));
                    break;
            }
        }

        private static void BuildMiner(BuildingModel building, Transform parent, float width, float depth, float cellSize)
        {
            Color color = VisualStyleConfig.MinerColor;
            CreatePart("Base", PrimitiveType.Cube, parent, new Vector3(0f, 0.18f, 0f), new Vector3(width * 0.78f, 0.36f, depth * 0.78f), color);
            CreatePart("Drill", PrimitiveType.Cylinder, parent, new Vector3(0f, 0.48f, 0f), new Vector3(cellSize * 0.26f, 0.34f, cellSize * 0.26f), VisualStyleConfig.DarkAccentColor);
            CreatePart("OreCap", PrimitiveType.Sphere, parent, new Vector3(-width * 0.17f, 0.62f, depth * 0.12f), new Vector3(cellSize * 0.22f, cellSize * 0.16f, cellSize * 0.22f), VisualStyleConfig.LightAccentColor);
            CreateDirectionPort("OutputPort", building.Direction, parent, width, depth, 0.42f, cellSize, color);
        }

        private static void BuildConveyor(BuildingModel building, Transform parent, float width, float depth, float cellSize)
        {
            CreatePart("BeltBase", PrimitiveType.Cube, parent, new Vector3(0f, 0.08f, 0f), new Vector3(width * 0.88f, 0.16f, depth * 0.58f), VisualStyleConfig.ConveyorColor);
            CreatePart("BeltTop", PrimitiveType.Cube, parent, new Vector3(0f, 0.19f, 0f), new Vector3(width * 0.8f, 0.08f, depth * 0.32f), VisualStyleConfig.DarkAccentColor);
            CreateDirectionPort("DirectionIndicator", building.Direction, parent, width, depth, 0.28f, cellSize, VisualStyleConfig.LightAccentColor);
        }

        private static void BuildSmelter(BuildingModel building, Transform parent, float width, float depth, float cellSize)
        {
            CreatePart("FurnaceBase", PrimitiveType.Cube, parent, new Vector3(0f, 0.26f, 0f), new Vector3(width * 0.78f, 0.52f, depth * 0.72f), VisualStyleConfig.SmelterColor);
            CreatePart("HotCore", PrimitiveType.Cube, parent, new Vector3(0f, 0.56f, -depth * 0.18f), new Vector3(width * 0.42f, 0.12f, depth * 0.18f), new Color(1f, 0.42f, 0.12f));
            CreatePart("ChimneyA", PrimitiveType.Cylinder, parent, new Vector3(-width * 0.18f, 0.82f, depth * 0.18f), new Vector3(cellSize * 0.16f, 0.48f, cellSize * 0.16f), VisualStyleConfig.DarkAccentColor);
            CreatePart("ChimneyB", PrimitiveType.Cylinder, parent, new Vector3(width * 0.18f, 0.72f, depth * 0.16f), new Vector3(cellSize * 0.13f, 0.36f, cellSize * 0.13f), VisualStyleConfig.DarkAccentColor);
            CreateDirectionPort("OutputPort", building.Direction, parent, width, depth, 0.56f, cellSize, VisualStyleConfig.LightAccentColor);
        }

        private static void BuildStorage(Transform parent, float width, float depth)
        {
            Color color = VisualStyleConfig.StorageColor;
            CreatePart("CrateA", PrimitiveType.Cube, parent, new Vector3(-width * 0.17f, 0.22f, -depth * 0.08f), new Vector3(width * 0.38f, 0.44f, depth * 0.42f), color);
            CreatePart("CrateB", PrimitiveType.Cube, parent, new Vector3(width * 0.18f, 0.18f, depth * 0.08f), new Vector3(width * 0.36f, 0.36f, depth * 0.42f), ScaleColor(color, 0.9f));
            CreatePart("CrateTop", PrimitiveType.Cube, parent, new Vector3(0f, 0.56f, 0f), new Vector3(width * 0.42f, 0.28f, depth * 0.38f), ScaleColor(color, 1.08f));
        }

        private static void BuildGenerator(Transform parent, float width, float depth)
        {
            CreatePart("GeneratorBase", PrimitiveType.Cube, parent, new Vector3(0f, 0.22f, 0f), new Vector3(width * 0.76f, 0.44f, depth * 0.7f), VisualStyleConfig.GeneratorColor);
            CreatePart("Core", PrimitiveType.Cylinder, parent, new Vector3(0f, 0.62f, 0f), new Vector3(width * 0.26f, 0.42f, depth * 0.26f), VisualStyleConfig.LightAccentColor);
            CreatePart("CoilA", PrimitiveType.Cube, parent, new Vector3(-width * 0.24f, 0.55f, 0f), new Vector3(width * 0.12f, 0.32f, depth * 0.52f), VisualStyleConfig.DarkAccentColor);
            CreatePart("CoilB", PrimitiveType.Cube, parent, new Vector3(width * 0.24f, 0.55f, 0f), new Vector3(width * 0.12f, 0.32f, depth * 0.52f), VisualStyleConfig.DarkAccentColor);
        }

        private static void BuildAssembler(BuildingModel building, Transform parent, float width, float depth, float cellSize)
        {
            Color color = VisualStyleConfig.AssemblerColor;
            CreatePart("AssemblerBase", PrimitiveType.Cube, parent, new Vector3(0f, 0.24f, 0f), new Vector3(width * 0.82f, 0.48f, depth * 0.76f), color);
            CreatePart("TopModule", PrimitiveType.Cube, parent, new Vector3(0f, 0.64f, 0f), new Vector3(width * 0.48f, 0.28f, depth * 0.46f), ScaleColor(color, 1.08f));
            CreatePart("ArmA", PrimitiveType.Cube, parent, new Vector3(-width * 0.27f, 0.58f, depth * 0.05f), new Vector3(width * 0.14f, 0.16f, depth * 0.5f), VisualStyleConfig.DarkAccentColor);
            CreatePart("ArmB", PrimitiveType.Cube, parent, new Vector3(width * 0.27f, 0.58f, -depth * 0.05f), new Vector3(width * 0.14f, 0.16f, depth * 0.5f), VisualStyleConfig.DarkAccentColor);
            CreateDirectionPort("OutputPort", building.Direction, parent, width, depth, 0.5f, cellSize, VisualStyleConfig.LightAccentColor);
        }

        private static void BuildResearchLab(BuildingModel building, Transform parent, float width, float depth, float cellSize)
        {
            Color color = VisualStyleConfig.ResearchLabColor;
            CreatePart("ResearchBase", PrimitiveType.Cube, parent, new Vector3(0f, 0.24f, 0f), new Vector3(width * 0.72f, 0.48f, depth * 0.72f), color);
            CreatePart("Dome", PrimitiveType.Sphere, parent, new Vector3(0f, 0.66f, 0f), new Vector3(width * 0.32f, cellSize * 0.28f, depth * 0.32f), ScaleColor(color, 1.18f));
            CreatePart("Antenna", PrimitiveType.Cylinder, parent, new Vector3(width * 0.18f, 0.98f, depth * 0.12f), new Vector3(cellSize * 0.08f, cellSize * 0.42f, cellSize * 0.08f), VisualStyleConfig.LightAccentColor);
            CreatePart("SignalCap", PrimitiveType.Sphere, parent, new Vector3(width * 0.18f, 1.22f, depth * 0.12f), new Vector3(cellSize * 0.14f, cellSize * 0.14f, cellSize * 0.14f), VisualStyleConfig.LightAccentColor);
            CreateDirectionPort("DirectionIndicator", building.Direction, parent, width, depth, 0.58f, cellSize, VisualStyleConfig.LightAccentColor);
        }

        private static void CreateDirectionPort(string name, BuildingDirection direction, Transform parent, float width, float depth, float y, float cellSize, Color color)
        {
            GridPosition offset = direction.ToOffset();
            Vector3 localDirection = new Vector3(offset.X, 0f, offset.Y);
            Vector3 position = new Vector3(localDirection.x * width * 0.36f, y, localDirection.z * depth * 0.36f);
            Vector3 scale = offset.X != 0
                ? new Vector3(cellSize * 0.34f, cellSize * 0.1f, cellSize * 0.16f)
                : new Vector3(cellSize * 0.16f, cellSize * 0.1f, cellSize * 0.34f);

            CreatePart(name, PrimitiveType.Cube, parent, position, scale, color);
        }

        private static GameObject CreatePart(string name, PrimitiveType primitiveType, Transform parent, Vector3 localPosition, Vector3 localScale, Color color)
        {
            GameObject part = GameObject.CreatePrimitive(primitiveType);
            part.name = name;
            part.transform.SetParent(parent, false);
            part.transform.localPosition = localPosition;
            part.transform.localScale = localScale;

            Renderer renderer = part.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = MaterialFactory.CreateOpaque(color);
            }

            return part;
        }

        private static Color ScaleColor(Color color, float scale)
        {
            return new Color(
                Mathf.Clamp01(color.r * scale),
                Mathf.Clamp01(color.g * scale),
                Mathf.Clamp01(color.b * scale),
                color.a);
        }
    }
}
