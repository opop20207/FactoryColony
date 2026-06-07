using System.Collections.Generic;
using UnityEngine;

namespace FactoryColony
{
    public sealed class FactoryDebugHud : MonoBehaviour
    {
        private static readonly ResourceType[] DisplayResourceTypes =
        {
            ResourceType.IronOre,
            ResourceType.CopperOre,
            ResourceType.Coal,
            ResourceType.IronIngot,
            ResourceType.CopperIngot,
            ResourceType.IronPlate,
            ResourceType.CopperWire,
            ResourceType.Gear,
            ResourceType.BasicCircuit
        };

        private readonly Rect _panelRect = new Rect(12f, 12f, 470f, 700f);
        private readonly Dictionary<ResourceType, int> _storedResources = new Dictionary<ResourceType, int>();

        private FactorySimulation _simulation;
        private SimulationTickRunner _runner;
        private GridMouseSelector _selector;
        private BuildingPlacementPreview _placementPreview;
        private BuildingPlacementController _placementController;
        private BuildingSelectionController _buildingSelectionController;
        private BaseInventoryModel _baseInventory;
        private StorageCollectionController _storageCollectionController;
        private FactoryDebugSaveController _saveController;
        private PowerModel _powerSnapshot;
        private GUIStyle _panelStyle;
        private GUIStyle _labelStyle;

        public void Initialize(FactorySimulation simulation, SimulationTickRunner runner)
        {
            Initialize(simulation, runner, null);
        }

        public void Initialize(FactorySimulation simulation, SimulationTickRunner runner, GridMouseSelector selector)
        {
            Initialize(simulation, runner, selector, null);
        }

        public void Initialize(
            FactorySimulation simulation,
            SimulationTickRunner runner,
            GridMouseSelector selector,
            BuildingPlacementPreview placementPreview)
        {
            Initialize(simulation, runner, selector, placementPreview, null);
        }

        public void Initialize(
            FactorySimulation simulation,
            SimulationTickRunner runner,
            GridMouseSelector selector,
            BuildingPlacementPreview placementPreview,
            BuildingPlacementController placementController)
        {
            Initialize(simulation, runner, selector, placementPreview, placementController, null);
        }

        public void Initialize(
            FactorySimulation simulation,
            SimulationTickRunner runner,
            GridMouseSelector selector,
            BuildingPlacementPreview placementPreview,
            BuildingPlacementController placementController,
            BuildingSelectionController buildingSelectionController)
        {
            Initialize(simulation, runner, selector, placementPreview, placementController, buildingSelectionController, null);
        }

        public void Initialize(
            FactorySimulation simulation,
            SimulationTickRunner runner,
            GridMouseSelector selector,
            BuildingPlacementPreview placementPreview,
            BuildingPlacementController placementController,
            BuildingSelectionController buildingSelectionController,
            BaseInventoryModel baseInventory)
        {
            Initialize(
                simulation,
                runner,
                selector,
                placementPreview,
                placementController,
                buildingSelectionController,
                baseInventory,
                null);
        }

        public void Initialize(
            FactorySimulation simulation,
            SimulationTickRunner runner,
            GridMouseSelector selector,
            BuildingPlacementPreview placementPreview,
            BuildingPlacementController placementController,
            BuildingSelectionController buildingSelectionController,
            BaseInventoryModel baseInventory,
            StorageCollectionController storageCollectionController)
        {
            Initialize(
                simulation,
                runner,
                selector,
                placementPreview,
                placementController,
                buildingSelectionController,
                baseInventory,
                storageCollectionController,
                null);
        }

        public void Initialize(
            FactorySimulation simulation,
            SimulationTickRunner runner,
            GridMouseSelector selector,
            BuildingPlacementPreview placementPreview,
            BuildingPlacementController placementController,
            BuildingSelectionController buildingSelectionController,
            BaseInventoryModel baseInventory,
            StorageCollectionController storageCollectionController,
            FactoryDebugSaveController saveController)
        {
            if (_runner != null)
            {
                _runner.OnTickExecuted -= HandleTickExecuted;
            }

            if (_storageCollectionController != null)
            {
                _storageCollectionController.OnStorageCollected -= HandleStorageCollected;
            }

            _simulation = simulation;
            _runner = runner;
            _selector = selector;
            _placementPreview = placementPreview;
            _placementController = placementController;
            _buildingSelectionController = buildingSelectionController;
            _baseInventory = baseInventory;
            _storageCollectionController = storageCollectionController;
            _saveController = saveController;

            if (_runner != null)
            {
                _runner.OnTickExecuted += HandleTickExecuted;
            }

            if (_storageCollectionController != null)
            {
                _storageCollectionController.OnStorageCollected += HandleStorageCollected;
            }

            RefreshSnapshot();
        }

        private void OnDestroy()
        {
            if (_runner != null)
            {
                _runner.OnTickExecuted -= HandleTickExecuted;
            }

            if (_storageCollectionController != null)
            {
                _storageCollectionController.OnStorageCollected -= HandleStorageCollected;
            }
        }

        private void HandleTickExecuted(int tickCount)
        {
            RefreshSnapshot();
        }

        private void HandleStorageCollected()
        {
            RefreshSnapshot();
        }

        private void RefreshSnapshot()
        {
            _storedResources.Clear();

            if (_simulation == null)
            {
                _powerSnapshot = null;
                return;
            }

            _powerSnapshot = _simulation.CalculatePower();
            IReadOnlyDictionary<ResourceType, int> storedResources = _simulation.GetStoredResources();

            foreach (KeyValuePair<ResourceType, int> pair in storedResources)
            {
                _storedResources[pair.Key] = pair.Value;
            }
        }

        private void OnGUI()
        {
            EnsureStyles();

            GUILayout.BeginArea(_panelRect, GUIContent.none, _panelStyle);

            if (_simulation == null || _runner == null)
            {
                GUILayout.Label("Factory Debug HUD", _labelStyle);
                GUILayout.Label("Simulation is not initialized.", _labelStyle);
                GUILayout.EndArea();
                return;
            }

            PowerModel power = _powerSnapshot ?? _simulation.CalculatePower();

            GUILayout.Label("Factory Debug HUD", _labelStyle);
            GUILayout.Space(4f);
            GUILayout.Label("Tick Count: " + _runner.TickCount, _labelStyle);
            GUILayout.Label("Running: " + _runner.IsRunning, _labelStyle);
            GUILayout.Label("Hovered Cell: " + GetHoveredCellText(), _labelStyle);
            GUILayout.Label("Selected Building: " + GetSelectedBuildingText(), _labelStyle);
            GUILayout.Label("Selected Building Cost: " + GetSelectedBuildingCostText(), _labelStyle);
            GUILayout.Label("Preview Direction: " + GetPreviewDirectionText(), _labelStyle);
            GUILayout.Label("Can Place: " + GetCanPlaceText(), _labelStyle);
            GUILayout.Label("Can Afford: " + GetCanAffordText(), _labelStyle);
            GUILayout.Label("Cannot Place Reason: " + GetCannotPlaceReasonText(), _labelStyle);
            GUILayout.Label("Total Buildings: " + GetTotalBuildingsText(), _labelStyle);
            GUILayout.Label("Last Placement Result: " + GetLastPlacementResultText(), _labelStyle);
            GUILayout.Label("Hovered Building Id: " + GetHoveredBuildingIdText(), _labelStyle);
            GUILayout.Label("Hovered Building Type: " + GetHoveredBuildingTypeText(), _labelStyle);
            GUILayout.Label("Selected Building Id: " + GetInspectedBuildingIdText(), _labelStyle);
            GUILayout.Label("Hovered Building Inventory: " + GetHoveredBuildingInventoryText(), _labelStyle);
            GUILayout.Label("Last Removal Result: " + GetLastRemovalResultText(), _labelStyle);
            GUILayout.Label("Press C: Collect Storage Resources", _labelStyle);
            GUILayout.Label("F5: Save / F9: Load", _labelStyle);
            GUILayout.Label("Save Path: " + GetSavePathText(), _labelStyle);
            GUILayout.Label("Last Save Result: " + GetLastSaveResultText(), _labelStyle);
            GUILayout.Label("Last Load Result: " + GetLastLoadResultText(), _labelStyle);
            GUILayout.Label("Last Collection Result: " + GetLastCollectionResultText(), _labelStyle);
            GUILayout.Space(6f);
            GUILayout.Label("Power Produced: " + power.ProducedPower, _labelStyle);
            GUILayout.Label("Power Consumed: " + power.ConsumedPower, _labelStyle);
            GUILayout.Label("Power Available: " + power.AvailablePower, _labelStyle);
            GUILayout.Label("Has Enough Power: " + power.HasEnoughPower, _labelStyle);
            GUILayout.Space(6f);
            GUILayout.Label("Base Inventory", _labelStyle);
            GUILayout.Label("IronPlate: " + GetBaseInventoryAmount(ResourceType.IronPlate), _labelStyle);
            GUILayout.Label("CopperWire: " + GetBaseInventoryAmount(ResourceType.CopperWire), _labelStyle);
            GUILayout.Label("Gear: " + GetBaseInventoryAmount(ResourceType.Gear), _labelStyle);
            GUILayout.Label("BasicCircuit: " + GetBaseInventoryAmount(ResourceType.BasicCircuit), _labelStyle);
            GUILayout.Space(6f);
            GUILayout.Label("Stored Resources", _labelStyle);

            bool hasAnyStoredResource = false;

            foreach (ResourceType resourceType in DisplayResourceTypes)
            {
                if (!_storedResources.TryGetValue(resourceType, out int amount) || amount <= 0)
                {
                    continue;
                }

                hasAnyStoredResource = true;
                GUILayout.Label(resourceType + ": " + amount, _labelStyle);
            }

            if (!hasAnyStoredResource)
            {
                GUILayout.Label("None", _labelStyle);
            }

            GUILayout.EndArea();
        }

        private string GetHoveredCellText()
        {
            if (_selector == null || !_selector.HoveredPosition.HasValue)
            {
                return "None";
            }

            GridPosition position = _selector.HoveredPosition.Value;
            return "(" + position.X + ", " + position.Y + ")";
        }

        private string GetSelectedBuildingText()
        {
            if (_placementPreview == null || _placementPreview.SelectedBuilding == null)
            {
                return "None";
            }

            return _placementPreview.SelectedBuilding.DisplayName;
        }

        private string GetSelectedBuildingCostText()
        {
            if (_placementPreview == null || _placementPreview.SelectedBuilding == null)
            {
                return "None";
            }

            return FormatCost(_placementPreview.SelectedBuilding.BuildCost);
        }

        private string GetPreviewDirectionText()
        {
            if (_placementPreview == null || !_placementPreview.HasSelection)
            {
                return "N/A";
            }

            return _placementPreview.Direction.ToString();
        }

        private string GetCanPlaceText()
        {
            if (_placementPreview == null || !_placementPreview.HasSelection || !_placementPreview.HasHoveredCell)
            {
                return "N/A";
            }

            return _placementPreview.CanPlace.ToString();
        }

        private string GetCanAffordText()
        {
            if (_placementPreview == null || !_placementPreview.HasSelection)
            {
                return "N/A";
            }

            return _placementPreview.CanAffordCurrentBuilding.ToString();
        }

        private string GetCannotPlaceReasonText()
        {
            if (_placementPreview == null)
            {
                return "N/A";
            }

            return _placementPreview.CannotPlaceReason;
        }

        private string GetTotalBuildingsText()
        {
            if (_placementController == null)
            {
                return "N/A";
            }

            return _placementController.TotalBuildings.ToString();
        }

        private string GetLastPlacementResultText()
        {
            if (_placementController == null)
            {
                return "N/A";
            }

            return _placementController.LastPlacementResult;
        }

        private string GetHoveredBuildingIdText()
        {
            if (_buildingSelectionController == null
                || _buildingSelectionController.HoveredBuilding == null)
            {
                return "None";
            }

            return _buildingSelectionController.HoveredBuilding.InstanceId;
        }

        private string GetHoveredBuildingTypeText()
        {
            if (_buildingSelectionController == null
                || _buildingSelectionController.HoveredBuilding == null)
            {
                return "None";
            }

            return _buildingSelectionController.HoveredBuilding.Definition.Type.ToString();
        }

        private string GetHoveredBuildingInventoryText()
        {
            if (_buildingSelectionController == null
                || _buildingSelectionController.HoveredBuilding == null)
            {
                return "None";
            }

            InventoryModel inventory = _buildingSelectionController.HoveredBuilding.Inventory;

            if (inventory.IsEmpty)
            {
                return "Empty";
            }

            string result = string.Empty;

            foreach (ResourceStack stack in inventory.GetStacks())
            {
                if (!string.IsNullOrEmpty(result))
                {
                    result += ", ";
                }

                result += stack.Type + ": " + stack.Amount;
            }

            return string.IsNullOrEmpty(result) ? "Empty" : result;
        }

        private string GetInspectedBuildingIdText()
        {
            if (_buildingSelectionController == null
                || !_buildingSelectionController.HasSelectedBuilding)
            {
                return "None";
            }

            return _buildingSelectionController.SelectedBuildingId;
        }

        private string GetLastRemovalResultText()
        {
            if (_buildingSelectionController == null)
            {
                return "N/A";
            }

            return _buildingSelectionController.LastRemovalResult;
        }

        private string GetLastCollectionResultText()
        {
            if (_storageCollectionController == null)
            {
                return "N/A";
            }

            return _storageCollectionController.LastCollectionResult;
        }

        private string GetSavePathText()
        {
            return _saveController != null ? _saveController.SavePath : "N/A";
        }

        private string GetLastSaveResultText()
        {
            return _saveController != null ? _saveController.LastSaveResult : "N/A";
        }

        private string GetLastLoadResultText()
        {
            return _saveController != null ? _saveController.LastLoadResult : "N/A";
        }

        private int GetBaseInventoryAmount(ResourceType type)
        {
            return _baseInventory != null ? _baseInventory.GetAmount(type) : 0;
        }

        private static string FormatCost(IReadOnlyDictionary<ResourceType, int> cost)
        {
            if (cost == null || cost.Count == 0)
            {
                return "Free";
            }

            string result = string.Empty;

            foreach (KeyValuePair<ResourceType, int> item in cost)
            {
                if (!string.IsNullOrEmpty(result))
                {
                    result += ", ";
                }

                result += item.Key + ": " + item.Value;
            }

            return result;
        }

        private void EnsureStyles()
        {
            if (_panelStyle != null && _labelStyle != null)
            {
                return;
            }

            _panelStyle = new GUIStyle(GUI.skin.box)
            {
                padding = new RectOffset(10, 10, 10, 10)
            };

            _labelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 14,
                normal =
                {
                    textColor = Color.white
                }
            };
        }
    }
}
