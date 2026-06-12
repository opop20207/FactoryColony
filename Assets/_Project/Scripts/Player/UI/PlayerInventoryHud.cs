using UnityEngine;

namespace FactoryColony
{
    public sealed class PlayerInventoryHud : MonoBehaviour
    {
        private readonly Rect _panelRect = new Rect(835f, 12f, 260f, 160f);

        private PlayerInventoryModel _playerInventory;
        private PlayerInteractionController _interactionController;
        private GUIStyle _panelStyle;
        private GUIStyle _labelStyle;

        public void Initialize(PlayerInventoryModel playerInventory, PlayerInteractionController interactionController)
        {
            _playerInventory = playerInventory;
            _interactionController = interactionController;
        }

        private void OnGUI()
        {
            EnsureStyles();

            GUILayout.BeginArea(_panelRect, GUIContent.none, _panelStyle);
            GUILayout.Label("Player Inventory", _labelStyle);
            GUILayout.Label(GetTotalText(), _labelStyle);
            GUILayout.Label(GetStacksText(), _labelStyle);
            GUILayout.Space(4f);
            GUILayout.Label("Q: Take from nearby Storage", _labelStyle);
            GUILayout.Label("B: Deposit all to Base", _labelStyle);
            GUILayout.Label("Last: " + GetLastTransferText(), _labelStyle);
            GUILayout.EndArea();
        }

        private string GetTotalText()
        {
            if (_playerInventory == null)
            {
                return "Total: N/A";
            }

            return "Total: " + _playerInventory.TotalAmount + " / " + _playerInventory.MaxTotalAmount;
        }

        private string GetStacksText()
        {
            if (_playerInventory == null || _playerInventory.IsEmpty)
            {
                return "Items: Empty";
            }

            string text = "Items: ";
            int count = 0;

            foreach (ResourceStack stack in _playerInventory.GetStacks())
            {
                if (count > 0)
                {
                    text += ", ";
                }

                text += stack.Type + " x" + stack.Amount;
                count++;

                if (count >= 5)
                {
                    break;
                }
            }

            return text;
        }

        private string GetLastTransferText()
        {
            return _interactionController != null ? _interactionController.LastPlayerInventoryResult : "N/A";
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
                _labelStyle.wordWrap = true;
            }
        }
    }
}
