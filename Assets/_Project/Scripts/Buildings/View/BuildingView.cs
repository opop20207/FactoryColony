using System.Collections.Generic;
using UnityEngine;

namespace FactoryColony
{
    public sealed class BuildingView : MonoBehaviour
    {
        private const float BodyInset = 0.84f;
        private const float IndicatorLength = 0.42f;
        private const float IndicatorThickness = 0.12f;

        private BuildingModel _building;

        public BuildingModel Building
        {
            get { return _building; }
        }

        public void Initialize(BuildingModel building, float cellSize)
        {
            _building = building;
            gameObject.name = $"Building_{building.InstanceId}_{building.Definition.Type}";

            BoundsData bounds = CalculateBounds(building.OccupiedPositions);
            float height = GetBuildingHeight(building.Definition.Type);
            transform.localPosition = new Vector3(
                bounds.CenterX * cellSize,
                height * 0.5f,
                bounds.CenterY * cellSize);

            CreateBody(building, bounds, cellSize, height);
            CreateDirectionIndicator(building, bounds, cellSize, height);
        }

        private void CreateBody(BuildingModel building, BoundsData bounds, float cellSize, float height)
        {
            GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cube);
            body.name = "Body";
            body.transform.SetParent(transform, false);
            body.transform.localPosition = Vector3.zero;
            body.transform.localScale = new Vector3(
                bounds.Width * cellSize * BodyInset,
                height,
                bounds.Height * cellSize * BodyInset);

            Renderer renderer = body.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = CreateMaterial(GetBuildingColor(building.Definition.Type));
            }
        }

        private void CreateDirectionIndicator(BuildingModel building, BoundsData bounds, float cellSize, float height)
        {
            GridPosition offset = building.Direction.ToOffset();
            Vector3 localDirection = new Vector3(offset.X, 0f, offset.Y);

            GameObject indicator = GameObject.CreatePrimitive(PrimitiveType.Cube);
            indicator.name = "DirectionIndicator";
            indicator.transform.SetParent(transform, false);
            indicator.transform.localPosition = new Vector3(
                localDirection.x * bounds.Width * cellSize * 0.28f,
                height * 0.58f,
                localDirection.z * bounds.Height * cellSize * 0.28f);

            if (offset.X != 0)
            {
                indicator.transform.localScale = new Vector3(cellSize * IndicatorLength, cellSize * IndicatorThickness, cellSize * IndicatorThickness);
            }
            else
            {
                indicator.transform.localScale = new Vector3(cellSize * IndicatorThickness, cellSize * IndicatorThickness, cellSize * IndicatorLength);
            }

            Renderer renderer = indicator.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = CreateMaterial(Color.white);
            }
        }

        private static BoundsData CalculateBounds(IReadOnlyList<GridPosition> positions)
        {
            int minX = positions[0].X;
            int maxX = positions[0].X;
            int minY = positions[0].Y;
            int maxY = positions[0].Y;

            for (int i = 1; i < positions.Count; i++)
            {
                GridPosition position = positions[i];
                minX = Mathf.Min(minX, position.X);
                maxX = Mathf.Max(maxX, position.X);
                minY = Mathf.Min(minY, position.Y);
                maxY = Mathf.Max(maxY, position.Y);
            }

            return new BoundsData(minX, maxX, minY, maxY);
        }

        private static float GetBuildingHeight(BuildingType type)
        {
            switch (type)
            {
                case BuildingType.Conveyor:
                    return 0.22f;
                case BuildingType.Storage:
                    return 0.55f;
                case BuildingType.Smelter:
                    return 0.72f;
                case BuildingType.Assembler:
                    return 0.78f;
                case BuildingType.Generator:
                    return 0.68f;
                case BuildingType.Miner:
                    return 0.5f;
                default:
                    return 0.4f;
            }
        }

        private static Color GetBuildingColor(BuildingType type)
        {
            switch (type)
            {
                case BuildingType.Miner:
                    return new Color(0.95f, 0.62f, 0.12f);
                case BuildingType.Conveyor:
                    return new Color(0.12f, 0.38f, 0.95f);
                case BuildingType.Smelter:
                    return new Color(0.82f, 0.18f, 0.12f);
                case BuildingType.Storage:
                    return new Color(0.18f, 0.62f, 0.25f);
                case BuildingType.Generator:
                    return new Color(0.68f, 0.32f, 0.92f);
                case BuildingType.Assembler:
                    return new Color(0.08f, 0.72f, 0.68f);
                default:
                    return new Color(0.82f, 0.82f, 0.82f);
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

        private readonly struct BoundsData
        {
            public int Width { get; }
            public int Height { get; }
            public float CenterX { get; }
            public float CenterY { get; }

            public BoundsData(int minX, int maxX, int minY, int maxY)
            {
                Width = maxX - minX + 1;
                Height = maxY - minY + 1;
                CenterX = (minX + maxX) * 0.5f;
                CenterY = (minY + maxY) * 0.5f;
            }
        }
    }
}
