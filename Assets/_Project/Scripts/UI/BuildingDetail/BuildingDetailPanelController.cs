using UnityEngine;

namespace FactoryColony
{
    public sealed class BuildingDetailPanelController : MonoBehaviour
    {
        private BuildingSelectionController _selectionController;
        private BuildingDetailPanelView _panelView;

        public void Initialize(
            BuildingSelectionController selectionController,
            BuildingDetailPanelView panelView)
        {
            _selectionController = selectionController;
            _panelView = panelView;
            RefreshPanel();
        }

        private void Update()
        {
            RefreshPanel();
        }

        public void RefreshNow()
        {
            RefreshPanel();
        }

        private void RefreshPanel()
        {
            if (_panelView == null)
            {
                return;
            }

            if (_selectionController == null || !_selectionController.HasSelectedBuilding)
            {
                _panelView.ShowEmpty();
                return;
            }

            _panelView.ShowBuilding(_selectionController.SelectedBuilding);
        }
    }
}
