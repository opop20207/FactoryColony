using UnityEngine;

namespace FactoryColony
{
    public sealed class InteractionHintHud : MonoBehaviour
    {
        private readonly Rect _panelRect = new Rect(500f, 12f, 320f, 104f);
        private PlayerInteractionController _interactionController;
        private GUIStyle _panelStyle;
        private GUIStyle _labelStyle;

        public void Initialize(PlayerInteractionController interactionController)
        {
            _interactionController = interactionController;
        }

        private void OnGUI()
        {
            EnsureStyles();
            GUILayout.BeginArea(_panelRect, GUIContent.none, _panelStyle);
            GUILayout.Label(GetNearbyText(), _labelStyle);
            GUILayout.Label("E: Select", _labelStyle);

            if (_interactionController != null
                && _interactionController.NearbyBuilding != null
                && _interactionController.NearbyBuilding.Definition.Type == BuildingType.Storage)
            {
                GUILayout.Label("Shift+E: Collect Storage", _labelStyle);
                GUILayout.Label("Q: Take to Player Inventory", _labelStyle);
            }

            if (_interactionController != null
                && _interactionController.NearbyBuilding != null
                && _interactionController.NearbyBuilding.Definition.Type == BuildingType.ResearchLab)
            {
                GUILayout.Label("T: Open Research", _labelStyle);
            }

            GUILayout.Label("B: Deposit Player Inventory", _labelStyle);
            GUILayout.Label("Last: " + GetLastInteractionText(), _labelStyle);
            GUILayout.EndArea();
        }

        private string GetNearbyText()
        {
            if (_interactionController == null || _interactionController.NearbyBuilding == null)
            {
                return "Nearby: None";
            }

            BuildingModel building = _interactionController.NearbyBuilding;
            return "Nearby: " + building.Definition.Type + " " + building.InstanceId;
        }

        private string GetLastInteractionText()
        {
            return _interactionController != null ? _interactionController.LastInteractionResult : "N/A";
        }

        private void EnsureStyles()
        {
            if (_panelStyle == null)
            {
                _panelStyle = new GUIStyle(GUI.skin.box);
                _panelStyle.normal.background = Texture2D.grayTexture;
                _panelStyle.padding = new RectOffset(8, 8, 6, 6);
            }

            if (_labelStyle == null)
            {
                _labelStyle = new GUIStyle(GUI.skin.label);
                _labelStyle.normal.textColor = Color.white;
                _labelStyle.fontSize = 13;
            }
        }
    }
}
