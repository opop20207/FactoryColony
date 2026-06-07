using UnityEngine;

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

            if (IsNumberKeyDown(KeyCode.Alpha1, KeyCode.Keypad1))
            {
                Select(BuildingType.Miner);
            }
            else if (IsNumberKeyDown(KeyCode.Alpha2, KeyCode.Keypad2))
            {
                Select(BuildingType.Conveyor);
            }
            else if (IsNumberKeyDown(KeyCode.Alpha3, KeyCode.Keypad3))
            {
                Select(BuildingType.Smelter);
            }
            else if (IsNumberKeyDown(KeyCode.Alpha4, KeyCode.Keypad4))
            {
                Select(BuildingType.Storage);
            }
            else if (IsNumberKeyDown(KeyCode.Alpha5, KeyCode.Keypad5))
            {
                Select(BuildingType.Generator);
            }
            else if (IsNumberKeyDown(KeyCode.Alpha6, KeyCode.Keypad6))
            {
                Select(BuildingType.Assembler);
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                _preview.ClearSelection();
            }
        }

        private static bool IsNumberKeyDown(KeyCode alphaKey, KeyCode keypadKey)
        {
            return Input.GetKeyDown(alphaKey) || Input.GetKeyDown(keypadKey);
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
