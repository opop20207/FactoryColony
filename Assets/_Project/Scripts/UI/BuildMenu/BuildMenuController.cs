using System.Collections.Generic;
using UnityEngine;

namespace FactoryColony
{
    public sealed class BuildMenuController : MonoBehaviour
    {
        private BuildMenuView _view;
        private BuildingPlacementPreview _preview;

        public void Initialize(
            BuildMenuView view,
            BuildingPlacementPreview preview,
            IReadOnlyList<BuildingDefinition> definitions)
        {
            if (_view != null)
            {
                _view.OnBuildingSelected -= HandleBuildingSelected;
                _view.OnSelectionCleared -= HandleSelectionCleared;
            }

            _view = view;
            _preview = preview;

            if (_view == null)
            {
                return;
            }

            _view.Initialize(definitions);
            _view.OnBuildingSelected += HandleBuildingSelected;
            _view.OnSelectionCleared += HandleSelectionCleared;
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
            if (_preview == null || !Input.GetKeyDown(KeyCode.Escape))
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
