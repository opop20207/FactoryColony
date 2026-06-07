using UnityEngine;
using UnityEngine.InputSystem;

namespace FactoryColony
{
    public sealed class DebugBuildingSelectionController : MonoBehaviour
    {
        private BuildingPlacementPreview _preview;

        public void Initialize(BuildingPlacementPreview preview)
        {
            _preview = preview;
        }

        private void Update()
        {
            if (_preview == null)
            {
                return;
            }

            if (IsNumberKeyDown(Key.Digit1, Key.Numpad1))
            {
                Select(BuildingType.Miner);
            }
            else if (IsNumberKeyDown(Key.Digit2, Key.Numpad2))
            {
                Select(BuildingType.Conveyor);
            }
            else if (IsNumberKeyDown(Key.Digit3, Key.Numpad3))
            {
                Select(BuildingType.Smelter);
            }
            else if (IsNumberKeyDown(Key.Digit4, Key.Numpad4))
            {
                Select(BuildingType.Storage);
            }
            else if (IsNumberKeyDown(Key.Digit5, Key.Numpad5))
            {
                Select(BuildingType.Generator);
            }
            else if (IsNumberKeyDown(Key.Digit6, Key.Numpad6))
            {
                Select(BuildingType.Assembler);
            }
            else if (PlayerInputReader.WasKeyPressedThisFrame(Key.Escape))
            {
                _preview.ClearSelection();
            }
        }

        private static bool IsNumberKeyDown(Key alphaKey, Key keypadKey)
        {
            return PlayerInputReader.WasKeyPressedThisFrame(alphaKey)
                || PlayerInputReader.WasKeyPressedThisFrame(keypadKey);
        }

        private void Select(BuildingType type)
        {
            if (DebugBuildingDefinitions.TryGet(type, out BuildingDefinition definition))
            {
                _preview.SetSelectedBuilding(definition);
            }
        }
    }
}
