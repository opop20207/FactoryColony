using UnityEngine;

namespace FactoryColony
{
    public static class VisualStyleConfig
    {
        public static readonly Color GroundColor = new Color(0.28f, 0.34f, 0.29f);
        public static readonly Color GroundResourceTint = new Color(0.35f, 0.39f, 0.33f);
        public static readonly Color GridLineColor = new Color(0.18f, 0.22f, 0.19f);

        public static readonly Color IronOreColor = new Color(0.62f, 0.64f, 0.66f);
        public static readonly Color CopperOreColor = new Color(0.83f, 0.45f, 0.22f);
        public static readonly Color CoalColor = new Color(0.05f, 0.05f, 0.055f);

        public static readonly Color MinerColor = new Color(0.88f, 0.58f, 0.2f);
        public static readonly Color ConveyorColor = new Color(0.22f, 0.42f, 0.62f);
        public static readonly Color SmelterColor = new Color(0.67f, 0.25f, 0.18f);
        public static readonly Color StorageColor = new Color(0.28f, 0.56f, 0.34f);
        public static readonly Color GeneratorColor = new Color(0.78f, 0.64f, 0.28f);
        public static readonly Color AssemblerColor = new Color(0.18f, 0.62f, 0.58f);

        public static readonly Color PreviewValidColor = new Color(0.2f, 0.85f, 0.38f, 0.5f);
        public static readonly Color PreviewInvalidColor = new Color(0.95f, 0.22f, 0.18f, 0.5f);
        public static readonly Color SelectionColor = new Color(0.16f, 0.72f, 1f, 0.42f);
        public static readonly Color HoverColor = new Color(1f, 0.88f, 0.22f, 0.52f);

        public static readonly Color LightAccentColor = new Color(1f, 0.83f, 0.38f);
        public static readonly Color DarkAccentColor = new Color(0.12f, 0.14f, 0.15f);

        public static Color GetBuildingColor(BuildingType type)
        {
            switch (type)
            {
                case BuildingType.Miner:
                    return MinerColor;
                case BuildingType.Conveyor:
                    return ConveyorColor;
                case BuildingType.Smelter:
                    return SmelterColor;
                case BuildingType.Storage:
                    return StorageColor;
                case BuildingType.Generator:
                    return GeneratorColor;
                case BuildingType.Assembler:
                    return AssemblerColor;
                default:
                    return new Color(0.76f, 0.76f, 0.72f);
            }
        }

        public static Color GetResourceColor(ResourceType type)
        {
            switch (type)
            {
                case ResourceType.IronOre:
                    return IronOreColor;
                case ResourceType.CopperOre:
                    return CopperOreColor;
                case ResourceType.Coal:
                    return CoalColor;
                default:
                    return GroundColor;
            }
        }
    }
}
