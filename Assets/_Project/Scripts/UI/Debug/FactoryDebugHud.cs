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

        private readonly Rect _panelRect = new Rect(12f, 12f, 300f, 360f);
        private readonly Dictionary<ResourceType, int> _storedResources = new Dictionary<ResourceType, int>();

        private FactorySimulation _simulation;
        private SimulationTickRunner _runner;
        private GridMouseSelector _selector;
        private BuildingPlacementPreview _placementPreview;
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
            if (_runner != null)
            {
                _runner.OnTickExecuted -= HandleTickExecuted;
            }

            _simulation = simulation;
            _runner = runner;
            _selector = selector;
            _placementPreview = placementPreview;

            if (_runner != null)
            {
                _runner.OnTickExecuted += HandleTickExecuted;
            }

            RefreshSnapshot();
        }

        private void OnDestroy()
        {
            if (_runner != null)
            {
                _runner.OnTickExecuted -= HandleTickExecuted;
            }
        }

        private void HandleTickExecuted(int tickCount)
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
            GUILayout.Label("Preview Direction: " + GetPreviewDirectionText(), _labelStyle);
            GUILayout.Label("Can Place: " + GetCanPlaceText(), _labelStyle);
            GUILayout.Space(6f);
            GUILayout.Label("Power Produced: " + power.ProducedPower, _labelStyle);
            GUILayout.Label("Power Consumed: " + power.ConsumedPower, _labelStyle);
            GUILayout.Label("Power Available: " + power.AvailablePower, _labelStyle);
            GUILayout.Label("Has Enough Power: " + power.HasEnoughPower, _labelStyle);
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
