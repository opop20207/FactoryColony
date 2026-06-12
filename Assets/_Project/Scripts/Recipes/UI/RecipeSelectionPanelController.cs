using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace FactoryColony
{
    public sealed class RecipeSelectionPanelController : MonoBehaviour
    {
        private BuildingSelectionController _selectionController;
        private RecipeSelectionPanelView _view;
        private RecipeSelectionService _service;
        private BuildingDetailPanelController _detailPanelController;
        private ResourceVisualRefreshController _resourceVisualRefreshController;

        public string LastRecipeSelectionResult { get; private set; } = "None";

        public void Initialize(
            BuildingSelectionController selectionController,
            RecipeSelectionPanelView view,
            RecipeSelectionService service,
            BuildingDetailPanelController detailPanelController,
            ResourceVisualRefreshController resourceVisualRefreshController)
        {
            if (_view != null)
            {
                _view.OnRecipeSelected -= HandleRecipeSelected;
            }

            _selectionController = selectionController;
            _view = view;
            _service = service;
            _detailPanelController = detailPanelController;
            _resourceVisualRefreshController = resourceVisualRefreshController;

            if (_view == null)
            {
                return;
            }

            _view.Initialize(_service);
            _view.OnRecipeSelected += HandleRecipeSelected;
            _view.Hide();
        }

        private void Update()
        {
            if (!PlayerInputReader.WasKeyPressedThisFrame(Key.Y) || IsPointerOverUi())
            {
                return;
            }

            TogglePanel();
        }

        private void OnDestroy()
        {
            if (_view != null)
            {
                _view.OnRecipeSelected -= HandleRecipeSelected;
            }
        }

        private void TogglePanel()
        {
            if (_view == null)
            {
                return;
            }

            if (_view.gameObject.activeSelf)
            {
                _view.Hide();
                return;
            }

            BuildingModel building = _selectionController != null ? _selectionController.SelectedBuilding : null;

            if (building == null)
            {
                LastRecipeSelectionResult = "No building selected.";
                _view.Show(null);
                _view.SetMessage(LastRecipeSelectionResult);
                return;
            }

            if (building.Definition.Type != BuildingType.Assembler)
            {
                LastRecipeSelectionResult = "Selected building has no recipe panel.";
                _view.Show(building);
                _view.SetMessage(LastRecipeSelectionResult);
                return;
            }

            _view.Show(building);
        }

        private void HandleRecipeSelected(string recipeId)
        {
            BuildingModel building = _selectionController != null ? _selectionController.SelectedBuilding : null;
            string message;

            if (_service == null)
            {
                message = "Recipe service unavailable.";
                LastRecipeSelectionResult = message;
            }
            else if (!_service.TrySetRecipe(building, recipeId, out message))
            {
                LastRecipeSelectionResult = message;
            }
            else
            {
                LastRecipeSelectionResult = message;
            }

            if (_view != null)
            {
                _view.SetMessage(LastRecipeSelectionResult);
                _view.Refresh();
            }

            if (_detailPanelController != null)
            {
                _detailPanelController.RefreshNow();
            }

            if (_resourceVisualRefreshController != null)
            {
                _resourceVisualRefreshController.RefreshNow();
            }
        }

        private static bool IsPointerOverUi()
        {
            return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
        }
    }
}
