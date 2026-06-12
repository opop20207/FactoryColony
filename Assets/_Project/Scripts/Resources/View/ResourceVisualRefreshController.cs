using UnityEngine;

namespace FactoryColony
{
    public sealed class ResourceVisualRefreshController : MonoBehaviour
    {
        private SimulationTickRunner _runner;
        private BuildingViewFactory _buildingViewFactory;
        private StorageCollectionController _storageCollectionController;

        public void Initialize(
            SimulationTickRunner runner,
            BuildingViewFactory buildingViewFactory,
            StorageCollectionController storageCollectionController = null)
        {
            Unsubscribe();
            _runner = runner;
            _buildingViewFactory = buildingViewFactory;
            _storageCollectionController = storageCollectionController;

            if (_runner != null)
            {
                _runner.OnTickExecuted += HandleTickExecuted;
            }

            if (_storageCollectionController != null)
            {
                _storageCollectionController.OnStorageCollected += HandleStorageCollected;
            }

            RefreshNow();
        }

        public void RefreshNow()
        {
            if (_buildingViewFactory != null)
            {
                _buildingViewFactory.RefreshInventoryVisuals();
                _buildingViewFactory.RefreshPowerStatusVisuals();
            }
        }

        private void OnDestroy()
        {
            Unsubscribe();
        }

        private void HandleTickExecuted(int tickCount)
        {
            RefreshNow();
        }

        private void HandleStorageCollected()
        {
            RefreshNow();
        }

        private void Unsubscribe()
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
    }
}
