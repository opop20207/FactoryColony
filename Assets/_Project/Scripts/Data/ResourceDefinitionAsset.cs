using UnityEngine;

namespace FactoryColony
{
    [CreateAssetMenu(menuName = "FactoryColony/Data/Resource Definition", fileName = "ResourceDefinition")]
    public sealed class ResourceDefinitionAsset : ScriptableObject
    {
        [SerializeField] private ResourceType type;
        [SerializeField] private string displayName;
        [SerializeField] private Color color = Color.white;

        public ResourceType Type
        {
            get { return type; }
        }

        public string DisplayName
        {
            get { return displayName; }
        }

        public Color Color
        {
            get { return color; }
        }
    }
}
