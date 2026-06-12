using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FactoryColony
{
    public sealed class BuildMenuController : MonoBehaviour
    {
        private BuildMenuView _view;
        private BuildingPlacementPreview _preview;
        private IReadOnlyList<BuildingDefinition> _definitions;
        private ResearchSystem _researchSystem;

        public void Initialize(
            BuildMenuView view,
            BuildingPlacementPreview preview,
            IReadOnlyList<BuildingDefinition> definitions)
        {
            Initialize(view, preview, definitions, null);
        }

        public void Initialize(
            BuildMenuView view,
            BuildingPlacementPreview preview,
            IReadOnlyList<BuildingDefinition> definitions,
            ResearchSystem researchSystem)
        {
            if (_view != null)
            {
                _view.OnBuildingSelected -= HandleBuildingSelected;
                _view.OnSelectionCleared -= HandleSelectionCleared;
            }

            _view = view;
            _preview = preview;
            _definitions = definitions;
            _researchSystem = researchSystem;

            if (_view == null)
            {
                return;
            }

            Refresh();
            _view.OnBuildingSelected += HandleBuildingSelected;
            _view.OnSelectionCleared += HandleSelectionCleared;
        }

        public void Refresh()
        {
            if (_view == null)
            {
                return;
            }

            if (_definitions == null || _researchSystem == null)
            {
                _view.Initialize(_definitions);
                return;
            }

            List<BuildingDefinition> unlockedDefinitions = new List<BuildingDefinition>();

            foreach (BuildingDefinition definition in _definitions)
            {
                if (_researchSystem.IsBuildingUnlocked(definition.Id))
                {
                    unlockedDefinitions.Add(definition);
                }
            }

            _view.Initialize(unlockedDefinitions);
        }

        private void OnDestroy()
        {
            if (_view != null)
            {
                _view.OnBuildingSelected -= HandleBuildingSelected;
                _view.OnSelectionCleared -= HandleSelectionCleared;
            }
        }

        private void Update()
        {
            if (_preview == null || !PlayerInputReader.WasKeyPressedThisFrame(Key.Escape))
            {
                return;
            }

            _preview.ClearSelection();

            if (_view != null)
            {
                _view.ClearSelectionState();
            }
        }

        private void HandleBuildingSelected(BuildingDefinition definition)
        {
            if (_preview == null)
            {
                return;
            }

            _preview.SetSelectedBuilding(definition);
            _view.SetSelectedDefinition(definition);
        }

        private void HandleSelectionCleared()
        {
            if (_preview != null)
            {
                _preview.ClearSelection();
            }
        }
    }
}
