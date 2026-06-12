using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace FactoryColony
{
    public sealed class PlayerInteractionController : MonoBehaviour
    {
        [SerializeField] private float interactionRange = 1.75f;

        private GridModel _gridModel;
        private BuildingSelectionController _selectionController;
        private StorageCollectionController _storageCollectionController;
        private PlayerInventoryTransferService _transferService;
        private ResourceVisualRefreshController _resourceVisualRefreshController;
        private float _cellSize = 1f;

        public BuildingModel NearbyBuilding { get; private set; }
        public string LastInteractionResult { get; private set; } = "None";
        public string LastPlayerInventoryResult { get; private set; } = "None";

        public void Initialize(
            GridModel gridModel,
            BuildingSelectionController selectionController,
            StorageCollectionController storageCollectionController,
            float cellSize)
        {
            Initialize(gridModel, selectionController, storageCollectionController, null, null, cellSize);
        }

        public void Initialize(
            GridModel gridModel,
            BuildingSelectionController selectionController,
            StorageCollectionController storageCollectionController,
            PlayerInventoryTransferService transferService,
            ResourceVisualRefreshController resourceVisualRefreshController,
            float cellSize)
        {
            _gridModel = gridModel;
            _selectionController = selectionController;
            _storageCollectionController = storageCollectionController;
            _transferService = transferService;
            _resourceVisualRefreshController = resourceVisualRefreshController;
            _cellSize = cellSize > 0f ? cellSize : 1f;
            LastInteractionResult = "Ready";
            LastPlayerInventoryResult = transferService != null ? "Ready" : "Player inventory not initialized.";
        }

        private void Update()
        {
            RefreshNearbyBuilding();

            if (IsPointerOverUi())
            {
                return;
            }

            if (PlayerInputReader.WasKeyPressedThisFrame(Key.E))
            {
                if (IsShiftPressed())
                {
                    CollectNearbyStorage();
                    return;
                }

                SelectNearbyBuilding();
                return;
            }

            if (PlayerInputReader.WasKeyPressedThisFrame(Key.Q))
            {
                TakeFromNearbyStorage();
                return;
            }

            if (PlayerInputReader.WasKeyPressedThisFrame(Key.B))
            {
                DepositPlayerInventoryToBase();
                return;
            }
        }

        private void RefreshNearbyBuilding()
        {
            Vector3 position = transform.position;
            NearbyBuilding = BuildingProximityFinder.FindNearest(
                _gridModel,
                position.x,
                position.z,
                _cellSize,
                interactionRange * _cellSize);
        }

        private void SelectNearbyBuilding()
        {
            if (NearbyBuilding == null)
            {
                LastInteractionResult = "No nearby building.";
                return;
            }

            if (_selectionController != null)
            {
                _selectionController.SelectBuilding(NearbyBuilding);
            }

            LastInteractionResult = "Selected " + NearbyBuilding.InstanceId + ".";
        }

        private void CollectNearbyStorage()
        {
            if (NearbyBuilding == null || NearbyBuilding.Definition.Type != BuildingType.Storage)
            {
                LastInteractionResult = "No nearby Storage.";
                return;
            }

            if (_storageCollectionController == null)
            {
                LastInteractionResult = "Storage collection is not initialized.";
                return;
            }

            _storageCollectionController.CollectStorage(NearbyBuilding);
            LastInteractionResult = "Collected from " + NearbyBuilding.InstanceId + ".";
        }

        private void TakeFromNearbyStorage()
        {
            if (NearbyBuilding == null || NearbyBuilding.Definition.Type != BuildingType.Storage)
            {
                LastPlayerInventoryResult = "No nearby Storage.";
                return;
            }

            if (_transferService == null)
            {
                LastPlayerInventoryResult = "Player inventory is not initialized.";
                return;
            }

            _transferService.TryTakeAnyFromStorage(NearbyBuilding, 10, out string message);
            LastPlayerInventoryResult = message;
            _resourceVisualRefreshController?.RefreshNow();
        }

        private void DepositPlayerInventoryToBase()
        {
            if (_transferService == null)
            {
                LastPlayerInventoryResult = "Player inventory is not initialized.";
                return;
            }

            _transferService.DepositAllToBase(out string message);
            LastPlayerInventoryResult = message;
        }

        private static bool IsShiftPressed()
        {
            return PlayerInputReader.IsKeyPressed(Key.LeftShift)
                || PlayerInputReader.IsKeyPressed(Key.RightShift);
        }

        private static bool IsPointerOverUi()
        {
            return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
        }
    }
}
