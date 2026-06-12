using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private PlayerInventoryModel _playerInventory;
        private GoalTracker _goalTracker;
        private ResearchSystem _researchSystem;
        private ResearchStateModel _researchState;
        private ResearchAccessService _researchAccessService;
        private FactorySimulation _simulation;
        private PowerStatusService _powerStatusService;
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
        private PlayerInventoryHud _playerInventoryHud;
        private FactoryDebugHud _debugHud;
        private GoalHudView _goalHudView;
        private GoalUpdateController _goalUpdateController;
        private ResearchPanelController _researchPanelController;
        private ResearchPanelView _researchPanelView;
        private RecipeSelectionPanelController _recipeSelectionPanelController;
        private RecipeSelectionPanelView _recipeSelectionPanelView;
        private BuildMenuController _buildMenuController;
        private ResourceVisualRefreshController _resourceVisualRefreshController;
        private IReadOnlyDictionary<string, ResearchDefinition> _researchDefinitions;
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
            PlayerInventoryModel playerInventory,
            GoalTracker goalTracker,
            ResearchSystem researchSystem,
            ResearchStateModel researchState,
            ResearchAccessService researchAccessService,
            FactorySimulation simulation,
            PowerStatusService powerStatusService,
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
            ResearchPanelView researchPanelView,
            ResearchPanelController researchPanelController,
            BuildMenuController buildMenuController,
            ResourceVisualRefreshController resourceVisualRefreshController,
            PlayerInteractionController playerInteractionController,
            PlayerInventoryHud playerInventoryHud,
            RecipeSelectionPanelView recipeSelectionPanelView,
            RecipeSelectionPanelController recipeSelectionPanelController,
            IReadOnlyDictionary<string, BuildingDefinition> definitionsById,
            IReadOnlyDictionary<string, ResearchDefinition> researchDefinitions,
            RecipeCatalog recipeCatalog,
            float cellSize)
        {
            _gridModel = gridModel;
            _baseInventory = baseInventory;
            _playerInventory = playerInventory;
            _goalTracker = goalTracker;
            _researchSystem = researchSystem;
            _researchState = researchState;
            _researchAccessService = researchAccessService;
            _simulation = simulation;
            _powerStatusService = powerStatusService;
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
            _researchPanelView = researchPanelView;
            _researchPanelController = researchPanelController;
            _buildMenuController = buildMenuController;
            _resourceVisualRefreshController = resourceVisualRefreshController;
            _playerInteractionController = playerInteractionController;
            _playerInventoryHud = playerInventoryHud;
            _recipeSelectionPanelView = recipeSelectionPanelView;
            _recipeSelectionPanelController = recipeSelectionPanelController;
            _definitionsById = definitionsById;
            _researchDefinitions = researchDefinitions;
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
                FactorySaveData saveData = _saveGameService.CreateSaveData(_gridModel, _baseInventory, _goalTracker, _researchState, _playerInventory);
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
                PlayerInventoryModel restoredPlayerInventory = saveLoader.RestorePlayerInventory(saveData);
                GoalTracker restoredGoalTracker = new GoalTracker(restoredBaseInventory, DebugGoalDefinitions.CreateGoals());
                saveLoader.RestoreGoals(saveData, restoredGoalTracker);
                ResearchStateModel restoredResearchState = new ResearchStateModel();
                saveLoader.RestoreResearch(saveData, restoredResearchState);
                ResearchSystem restoredResearchSystem = new ResearchSystem(_researchDefinitions, restoredResearchState, restoredBaseInventory);
                ResearchAccessService restoredResearchAccessService = new ResearchAccessService(restoredGrid);

                FactorySimulation restoredSimulation = new FactorySimulation(restoredGrid, _recipeCatalog);
                PowerStatusService restoredPowerStatusService = new PowerStatusService(restoredGrid, restoredSimulation);
                StorageCollector restoredStorageCollector = new StorageCollector(restoredGrid, restoredBaseInventory);
                RecipeSelectionService restoredRecipeSelectionService = new RecipeSelectionService(
                    _recipeCatalog.Recipes.ToDictionary(recipe => recipe.Id),
                    restoredResearchSystem);

                RebindDebugScene(
                    restoredGrid,
                    restoredBaseInventory,
                    restoredPlayerInventory,
                    restoredGoalTracker,
                    restoredResearchSystem,
                    restoredResearchState,
                    restoredResearchAccessService,
                    restoredSimulation,
                    restoredPowerStatusService,
                    restoredStorageCollector,
                    restoredRecipeSelectionService);

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
            PlayerInventoryModel restoredPlayerInventory,
            GoalTracker restoredGoalTracker,
            ResearchSystem restoredResearchSystem,
            ResearchStateModel restoredResearchState,
            ResearchAccessService restoredResearchAccessService,
            FactorySimulation restoredSimulation,
            PowerStatusService restoredPowerStatusService,
            StorageCollector restoredStorageCollector,
            RecipeSelectionService restoredRecipeSelectionService)
        {
            _gridModel = restoredGrid;
            _baseInventory = restoredBaseInventory;
            _playerInventory = restoredPlayerInventory;
            _goalTracker = restoredGoalTracker;
            _researchSystem = restoredResearchSystem;
            _researchState = restoredResearchState;
            _researchAccessService = restoredResearchAccessService;
            _simulation = restoredSimulation;
            _powerStatusService = restoredPowerStatusService;

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
                _buildingViewFactory.SetPowerStatusService(restoredPowerStatusService);
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
                if (_detailPanelView != null)
                {
                    _detailPanelView.Initialize(restoredRecipeSelectionService, restoredPowerStatusService);
                }

                _detailPanelController.Initialize(_selectionController, _detailPanelView);
            }

            if (_storageCollectionController != null)
            {
                _storageCollectionController.Initialize(restoredStorageCollector);
            }

            if (_playerInteractionController != null)
            {
                PlayerInventoryTransferService transferService = new PlayerInventoryTransferService(restoredPlayerInventory, restoredBaseInventory);
                _playerInteractionController.Initialize(
                    restoredGrid,
                    _selectionController,
                    _storageCollectionController,
                    transferService,
                    _resourceVisualRefreshController,
                    _cellSize);
            }

            if (_playerInventoryHud != null)
            {
                _playerInventoryHud.Initialize(restoredPlayerInventory, _playerInteractionController);
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

            if (_researchPanelController != null)
            {
                _researchPanelController.Initialize(_researchPanelView, restoredResearchSystem, _buildMenuController, restoredResearchAccessService);
            }

            if (_recipeSelectionPanelController != null)
            {
                _recipeSelectionPanelController.Initialize(
                    _selectionController,
                    _recipeSelectionPanelView,
                    restoredRecipeSelectionService,
                    _detailPanelController,
                    _resourceVisualRefreshController);
            }

            if (_buildMenuController != null)
            {
                _buildMenuController.Refresh();
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
                    this,
                    restoredResearchSystem,
                    _researchPanelController,
                    restoredPlayerInventory,
                    restoredPowerStatusService);
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
