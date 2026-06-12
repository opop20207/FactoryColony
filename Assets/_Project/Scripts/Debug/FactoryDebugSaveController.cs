using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FactoryColony
{
    public sealed class FactoryDebugSaveController : MonoBehaviour
    {
        private readonly SaveGameService _saveGameService = new SaveGameService();
        private FactorySaveLoader _saveLoader;

        private GridModel _gridModel;
        private BaseInventoryModel _baseInventory;
        private GoalTracker _goalTracker;
        private FactorySimulation _simulation;
        private GridView _gridView;
        private BuildingViewFactory _buildingViewFactory;
        private GridMouseSelector _gridMouseSelector;
        private BuildingPlacementPreview _placementPreview;
        private BuildingPlacementController _placementController;
        private BuildingSelectionController _selectionController;
        private BuildingDetailPanelView _detailPanelView;
        private BuildingDetailPanelController _detailPanelController;
        private SimulationTickRunner _tickRunner;
        private StorageCollectionController _storageCollectionController;
        private PlayerInteractionController _playerInteractionController;
        private FactoryDebugHud _debugHud;
        private GoalHudView _goalHudView;
        private GoalUpdateController _goalUpdateController;
        private ResourceVisualRefreshController _resourceVisualRefreshController;
        private IReadOnlyDictionary<string, BuildingDefinition> _definitionsById;
        private RecipeCatalog _recipeCatalog;
        private float _cellSize = 1f;

        public string SavePath { get; private set; }
        public string LastSaveResult { get; private set; } = "None";
        public string LastLoadResult { get; private set; } = "None";

        private void Awake()
        {
            _saveLoader = new FactorySaveLoader(message => Debug.LogWarning(message));
        }

        public void Initialize(
            GridModel gridModel,
            BaseInventoryModel baseInventory,
            GoalTracker goalTracker,
            FactorySimulation simulation,
            GridView gridView,
            BuildingViewFactory buildingViewFactory,
            GridMouseSelector gridMouseSelector,
            BuildingPlacementPreview placementPreview,
            BuildingPlacementController placementController,
            BuildingSelectionController selectionController,
            BuildingDetailPanelView detailPanelView,
            BuildingDetailPanelController detailPanelController,
            SimulationTickRunner tickRunner,
            StorageCollectionController storageCollectionController,
            FactoryDebugHud debugHud,
            GoalHudView goalHudView,
            GoalUpdateController goalUpdateController,
            ResourceVisualRefreshController resourceVisualRefreshController,
            PlayerInteractionController playerInteractionController,
            IReadOnlyDictionary<string, BuildingDefinition> definitionsById,
            RecipeCatalog recipeCatalog,
            float cellSize)
        {
            _gridModel = gridModel;
            _baseInventory = baseInventory;
            _goalTracker = goalTracker;
            _simulation = simulation;
            _gridView = gridView;
            _buildingViewFactory = buildingViewFactory;
            _gridMouseSelector = gridMouseSelector;
            _placementPreview = placementPreview;
            _placementController = placementController;
            _selectionController = selectionController;
            _detailPanelView = detailPanelView;
            _detailPanelController = detailPanelController;
            _tickRunner = tickRunner;
            _storageCollectionController = storageCollectionController;
            _debugHud = debugHud;
            _goalHudView = goalHudView;
            _goalUpdateController = goalUpdateController;
            _resourceVisualRefreshController = resourceVisualRefreshController;
            _playerInteractionController = playerInteractionController;
            _definitionsById = definitionsById;
            _recipeCatalog = recipeCatalog;
            _cellSize = cellSize > 0f ? cellSize : 1f;
            SavePath = Path.Combine(Application.persistentDataPath, "factory_debug_save.json");
        }

        private void Update()
        {
            if (PlayerInputReader.WasKeyPressedThisFrame(Key.F5))
            {
                Save();
            }

            if (PlayerInputReader.WasKeyPressedThisFrame(Key.F9))
            {
                Load();
            }
        }

        public void Save()
        {
            if (_gridModel == null || _baseInventory == null)
            {
                LastSaveResult = "Save Failed: debug state is not initialized.";
                Debug.LogWarning(LastSaveResult);
                return;
            }

            try
            {
                FactorySaveData saveData = _saveGameService.CreateSaveData(_gridModel, _baseInventory, _goalTracker);
                _saveGameService.SaveToFile(saveData, SavePath);
                LastSaveResult = "Saved to: " + SavePath;
            }
            catch (System.Exception exception)
            {
                LastSaveResult = "Save Failed: " + exception.Message;
                Debug.LogWarning(LastSaveResult);
            }
        }

        public void Load()
        {
            if (!_saveGameService.TryLoadFromFile(SavePath, out FactorySaveData saveData))
            {
                LastLoadResult = "Load Failed: File not found or invalid JSON.";
                Debug.LogWarning(LastLoadResult);
                return;
            }

            try
            {
                IReadOnlyDictionary<string, BuildingDefinition> definitionsById = CreateDefinitionLookup();
                FactorySaveLoader saveLoader = _saveLoader ?? new FactorySaveLoader(message => Debug.LogWarning(message));
                GridModel restoredGrid = saveLoader.RestoreGrid(saveData, definitionsById);
                BaseInventoryModel restoredBaseInventory = saveLoader.RestoreBaseInventory(saveData);
                GoalTracker restoredGoalTracker = new GoalTracker(restoredBaseInventory, DebugGoalDefinitions.CreateGoals());
                saveLoader.RestoreGoals(saveData, restoredGoalTracker);

                FactorySimulation restoredSimulation = new FactorySimulation(restoredGrid, _recipeCatalog);
                StorageCollector restoredStorageCollector = new StorageCollector(restoredGrid, restoredBaseInventory);

                RebindDebugScene(
                    restoredGrid,
                    restoredBaseInventory,
                    restoredGoalTracker,
                    restoredSimulation,
                    restoredStorageCollector);

                LastLoadResult = "Load Success";
            }
            catch (System.Exception exception)
            {
                LastLoadResult = "Load Failed: " + exception.Message;
                Debug.LogWarning(LastLoadResult);
            }
        }

        private void RebindDebugScene(
            GridModel restoredGrid,
            BaseInventoryModel restoredBaseInventory,
            GoalTracker restoredGoalTracker,
            FactorySimulation restoredSimulation,
            StorageCollector restoredStorageCollector)
        {
            _gridModel = restoredGrid;
            _baseInventory = restoredBaseInventory;
            _goalTracker = restoredGoalTracker;
            _simulation = restoredSimulation;

            if (_placementPreview != null)
            {
                _placementPreview.ClearSelection();
            }

            if (_selectionController != null)
            {
                _selectionController.ClearSelection();
            }

            if (_gridView != null)
            {
                _gridView.Build(restoredGrid);
            }

            if (_buildingViewFactory != null)
            {
                _buildingViewFactory.Build(restoredGrid, _cellSize);
            }

            if (_gridMouseSelector != null)
            {
                _gridMouseSelector.Initialize(restoredGrid, _cellSize);
            }

            if (_placementPreview != null)
            {
                _placementPreview.Initialize(restoredGrid, _gridMouseSelector, _cellSize, restoredBaseInventory);
            }

            if (_placementController != null)
            {
                _placementController.Initialize(restoredGrid, _gridMouseSelector, _placementPreview, _buildingViewFactory, _cellSize, restoredBaseInventory);
            }

            if (_selectionController != null)
            {
                _selectionController.Initialize(restoredGrid, _gridMouseSelector, _buildingViewFactory, _cellSize, restoredBaseInventory);
            }

            if (_detailPanelController != null)
            {
                _detailPanelController.Initialize(_selectionController, _detailPanelView);
            }

            if (_storageCollectionController != null)
            {
                _storageCollectionController.Initialize(restoredStorageCollector);
            }

            if (_playerInteractionController != null)
            {
                _playerInteractionController.Initialize(restoredGrid, _selectionController, _storageCollectionController, _cellSize);
            }

            if (_tickRunner != null)
            {
                _tickRunner.Initialize(restoredSimulation);
            }

            if (_goalHudView != null)
            {
                _goalHudView.Initialize(restoredGoalTracker);
            }

            if (_goalUpdateController != null)
            {
                _goalUpdateController.Initialize(restoredGoalTracker, _tickRunner, _storageCollectionController);
            }

            if (_resourceVisualRefreshController != null)
            {
                _resourceVisualRefreshController.Initialize(_tickRunner, _buildingViewFactory, _storageCollectionController);
            }

            if (_debugHud != null)
            {
                _debugHud.Initialize(
                    restoredSimulation,
                    _tickRunner,
                    _gridMouseSelector,
                    _placementPreview,
                    _placementController,
                    _selectionController,
                    restoredBaseInventory,
                    _storageCollectionController,
                    this);
            }
        }

        private IReadOnlyDictionary<string, BuildingDefinition> CreateDefinitionLookup()
        {
            if (_definitionsById != null && _definitionsById.Count > 0)
            {
                return _definitionsById;
            }

            Dictionary<string, BuildingDefinition> definitionsById = new Dictionary<string, BuildingDefinition>();

            foreach (BuildingDefinition definition in DebugBuildingDefinitions.GetAll())
            {
                definitionsById[definition.Id] = definition;
            }

            return definitionsById;
        }
    }
}
