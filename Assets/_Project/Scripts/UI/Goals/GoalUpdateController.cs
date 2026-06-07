using UnityEngine;

namespace FactoryColony
{
    public sealed class GoalUpdateController : MonoBehaviour
    {
        private GoalTracker _tracker;
        private SimulationTickRunner _runner;
        private StorageCollectionController _collectionController;

        public void Initialize(
            GoalTracker tracker,
            SimulationTickRunner runner,
            StorageCollectionController collectionController)
        {
            if (_runner != null)
            {
                _runner.OnTickExecuted -= HandleTickExecuted;
            }

            if (_collectionController != null)
            {
                _collectionController.OnStorageCollected -= HandleStorageCollected;
            }

            _tracker = tracker;
            _runner = runner;
            _collectionController = collectionController;

            if (_runner != null)
            {
                _runner.OnTickExecuted += HandleTickExecuted;
            }

            if (_collectionController != null)
            {
                _collectionController.OnStorageCollected += HandleStorageCollected;
            }

            UpdateGoals();
        }

        private void Update()
        {
            UpdateGoals();
        }

        private void OnDestroy()
        {
            if (_runner != null)
            {
                _runner.OnTickExecuted -= HandleTickExecuted;
            }

            if (_collectionController != null)
            {
                _collectionController.OnStorageCollected -= HandleStorageCollected;
            }
        }

        private void HandleTickExecuted(int tickCount)
        {
            UpdateGoals();
        }

        private void HandleStorageCollected()
        {
            UpdateGoals();
        }

        private void UpdateGoals()
        {
            if (_tracker != null)
            {
                _tracker.UpdateGoals();
            }
        }
    }
}
