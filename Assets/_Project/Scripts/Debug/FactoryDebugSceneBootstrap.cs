using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace FactoryColony
{
    public sealed class FactoryDebugSceneBootstrap : MonoBehaviour
    {
        [SerializeField] private GridView gridView;
        [SerializeField] private BuildingViewFactory buildingViewFactory;
        [SerializeField] private SimulationTickRunner simulationTickRunner;
        [SerializeField] private FactoryDebugHud debugHud;
        [SerializeField] private GridMouseSelector gridMouseSelector;
        [SerializeField] private BuildingPlacementPreview placementPreview;
        [SerializeField] private BuildingPlacementController placementController;
        [SerializeField] private BuildingSelectionController buildingSelectionController;
        [SerializeField] private DebugBuildingSelectionController debugBuildingSelectionController;
        [SerializeField] private StorageCollectionController storageCollectionController;
        [SerializeField] private Canvas debugCanvas;
        [SerializeField] private BuildMenuView buildMenuView;
        [SerializeField] private BuildMenuController buildMenuController;
        [SerializeField] private BuildingDetailPanelView buildingDetailPanelView;
        [SerializeField] private BuildingDetailPanelController buildingDetailPanelController;
        [SerializeField] private GoalHudView goalHudView;
        [SerializeField] private GoalUpdateController goalUpdateController;
        [SerializeField] private FactoryDebugSaveController saveController;
        [SerializeField] private ResourceVisualRefreshController resourceVisualRefreshController;
        [SerializeField] private DefinitionDatabase definitionDatabase;
        [SerializeField] private PlayerView playerView;
        [SerializeField] private PlayerInteractionController playerInteractionController;
        [SerializeField] private InteractionHighlightView interactionHighlightView;
        [SerializeField] private InteractionHintHud interactionHintHud;

        private bool _initialized;
        private IReadOnlyDictionary<string, BuildingDefinition> _buildingDefinitionsById;
        private IReadOnlyDictionary<BuildingType, BuildingDefinition> _buildingDefinitionsByType;
        private IReadOnlyList<BuildingDefinition> _buildMenuDefinitions;
        private RecipeCatalog _recipeCatalog;

        private void Start()
        {
            InitializeDebugScene();
        }

        private void InitializeDebugScene()
        {
            if (_initialized)
            {
                return;
            }

            string initializationStep = "Begin";

            try
            {
                initializationStep = "Create and connect view components";
                EnsureViews();

                initializationStep = "Create GridModel";
                GridModel model = CreateGridModel();

                initializationStep = "Add resource nodes";
                AddResourceNodes(model);

                initializationStep = "Create BaseInventoryModel";
                BaseInventoryModel baseInventory = CreateBaseInventory();

                initializationStep = "Create GoalTracker";
                GoalTracker goalTracker = CreateGoalTracker(baseInventory);

                initializationStep = "Load definitions";
                LoadDefinitions();

                initializationStep = "Place initial buildings";
                PlaceDebugBuildings(model);

                initializationStep = "Create FactorySimulation";
                FactorySimulation simulation = new FactorySimulation(model, _recipeCatalog);

                initializationStep = "Create StorageCollector";
                StorageCollector storageCollector = new StorageCollector(model, baseInventory);

                initializationStep = "Setup camera";
                SetupCamera();
                VisualSceneSetup.ApplyDebugLighting();

                initializationStep = "Setup player";
                SetupPlayer(model, gridView.CellSize);

                initializationStep = "Build grid view";
                gridView.Build(model);

                initializationStep = "Build building views";
                buildingViewFactory.Build(model, gridView.CellSize);

                initializationStep = "Setup mouse selector";
                gridMouseSelector.Initialize(model, gridView.CellSize);

                initializationStep = "Setup placement preview";
                placementPreview.Initialize(model, gridMouseSelector, gridView.CellSize, baseInventory);

                initializationStep = "Setup placement controller";
                placementController.Initialize(model, gridMouseSelector, placementPreview, buildingViewFactory, gridView.CellSize, baseInventory);

                initializationStep = "Setup building selection controller";
                buildingSelectionController.Initialize(model, gridMouseSelector, buildingViewFactory, gridView.CellSize, baseInventory);

                initializationStep = "Setup building detail panel";
                buildingDetailPanelController.Initialize(buildingSelectionController, buildingDetailPanelView);

                initializationStep = "Setup number-key selection controller";
                debugBuildingSelectionController.Initialize(placementPreview, _buildMenuDefinitions);

                initializationStep = "Setup storage collection controller";
                storageCollectionController.Initialize(storageCollector);

                initializationStep = "Setup player interaction";
                SetupPlayerInteraction(model, gridView.CellSize);

                initializationStep = "Setup goals";
                goalHudView.Initialize(goalTracker);
                goalUpdateController.Initialize(goalTracker, simulationTickRunner, storageCollectionController);

                initializationStep = "Setup save controller";
                saveController.Initialize(
                    model,
                    baseInventory,
                    goalTracker,
                    simulation,
                    gridView,
                    buildingViewFactory,
                    gridMouseSelector,
                    placementPreview,
                    placementController,
                    buildingSelectionController,
                    buildingDetailPanelView,
                    buildingDetailPanelController,
                    simulationTickRunner,
                    storageCollectionController,
                    debugHud,
                    goalHudView,
                    goalUpdateController,
                    resourceVisualRefreshController,
                    playerInteractionController,
                    _buildingDefinitionsById,
                    _recipeCatalog,
                    gridView.CellSize);

                initializationStep = "Setup build menu";
                buildMenuController.Initialize(buildMenuView, placementPreview, _buildMenuDefinitions);

                initializationStep = "Setup tick runner";
                simulationTickRunner.Initialize(simulation);

                initializationStep = "Setup resource visuals";
                resourceVisualRefreshController.Initialize(simulationTickRunner, buildingViewFactory, storageCollectionController);

                initializationStep = "Setup debug HUD";
                debugHud.Initialize(
                    simulation,
                    simulationTickRunner,
                    gridMouseSelector,
                    placementPreview,
                    placementController,
                    buildingSelectionController,
                    baseInventory,
                    storageCollectionController,
                    saveController);

                _initialized = true;
            }
            catch (System.Exception exception)
            {
                Debug.LogError("FactoryDebugSceneBootstrap failed during step '" + initializationStep + "': " + exception);
            }
        }

        private void EnsureViews()
        {
            if (gridView == null)
            {
                gridView = FindObjectOfType<GridView>();
            }

            if (gridView == null)
            {
                GameObject gridViewObject = new GameObject("GridView");
                gridView = gridViewObject.AddComponent<GridView>();
            }

            if (buildingViewFactory == null)
            {
                buildingViewFactory = FindObjectOfType<BuildingViewFactory>();
            }

            if (buildingViewFactory == null)
            {
                GameObject buildingViewFactoryObject = new GameObject("BuildingViewFactory");
                buildingViewFactory = buildingViewFactoryObject.AddComponent<BuildingViewFactory>();
            }

            if (simulationTickRunner == null)
            {
                simulationTickRunner = FindObjectOfType<SimulationTickRunner>();
            }

            if (simulationTickRunner == null)
            {
                GameObject tickRunnerObject = new GameObject("SimulationTickRunner");
                simulationTickRunner = tickRunnerObject.AddComponent<SimulationTickRunner>();
            }

            if (debugHud == null)
            {
                debugHud = FindObjectOfType<FactoryDebugHud>();
            }

            if (debugHud == null)
            {
                GameObject debugHudObject = new GameObject("FactoryDebugHud");
                debugHud = debugHudObject.AddComponent<FactoryDebugHud>();
            }

            if (gridMouseSelector == null)
            {
                gridMouseSelector = FindObjectOfType<GridMouseSelector>();
            }

            if (gridMouseSelector == null)
            {
                GameObject selectorObject = new GameObject("GridMouseSelector");
                gridMouseSelector = selectorObject.AddComponent<GridMouseSelector>();
            }

            if (placementPreview == null)
            {
                placementPreview = FindObjectOfType<BuildingPlacementPreview>();
            }

            if (placementPreview == null)
            {
                GameObject previewObject = new GameObject("BuildingPlacementPreviewController");
                placementPreview = previewObject.AddComponent<BuildingPlacementPreview>();
            }

            if (placementController == null)
            {
                placementController = FindObjectOfType<BuildingPlacementController>();
            }

            if (placementController == null)
            {
                GameObject placementObject = new GameObject("BuildingPlacementController");
                placementController = placementObject.AddComponent<BuildingPlacementController>();
            }

            if (buildingSelectionController == null)
            {
                buildingSelectionController = FindObjectOfType<BuildingSelectionController>();
            }

            if (buildingSelectionController == null)
            {
                GameObject buildingSelectionObject = new GameObject("BuildingSelectionController");
                buildingSelectionController = buildingSelectionObject.AddComponent<BuildingSelectionController>();
            }

            if (debugBuildingSelectionController == null)
            {
                debugBuildingSelectionController = FindObjectOfType<DebugBuildingSelectionController>();
            }

            if (debugBuildingSelectionController == null)
            {
                GameObject selectionObject = new GameObject("DebugBuildingSelectionController");
                debugBuildingSelectionController = selectionObject.AddComponent<DebugBuildingSelectionController>();
            }

            if (storageCollectionController == null)
            {
                storageCollectionController = FindObjectOfType<StorageCollectionController>();
            }

            if (storageCollectionController == null)
            {
                GameObject collectionObject = new GameObject("StorageCollectionController");
                storageCollectionController = collectionObject.AddComponent<StorageCollectionController>();
            }

            if (saveController == null)
            {
                saveController = FindObjectOfType<FactoryDebugSaveController>();
            }

            if (saveController == null)
            {
                GameObject saveObject = new GameObject("FactoryDebugSaveController");
                saveController = saveObject.AddComponent<FactoryDebugSaveController>();
            }

            if (resourceVisualRefreshController == null)
            {
                resourceVisualRefreshController = FindObjectOfType<ResourceVisualRefreshController>();
            }

            if (resourceVisualRefreshController == null)
            {
                GameObject refreshObject = new GameObject("ResourceVisualRefreshController");
                resourceVisualRefreshController = refreshObject.AddComponent<ResourceVisualRefreshController>();
            }

            EnsureBuildMenu();
            EnsureBuildingDetailPanel();
            EnsureGoalHud();
            EnsureCameraController();
        }

        private GridModel CreateGridModel()
        {
            if (gridView == null)
            {
                throw new System.InvalidOperationException("GridView is missing.");
            }

            return gridView.CreateDefaultModel();
        }

        private static void AddResourceNodes(GridModel model)
        {
            model.SetResourceNode(new GridPosition(1, 1), ResourceType.IronOre);
            model.SetResourceNode(new GridPosition(1, 3), ResourceType.CopperOre);
            model.SetResourceNode(new GridPosition(1, 5), ResourceType.Coal);
        }

        private void EnsureBuildMenu()
        {
            EnsureEventSystem();

            if (debugCanvas == null)
            {
                debugCanvas = FindObjectOfType<Canvas>();
            }

            if (debugCanvas == null)
            {
                GameObject canvasObject = new GameObject("FactoryDebugCanvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
                debugCanvas = canvasObject.GetComponent<Canvas>();
                debugCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

                CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1280f, 720f);
            }

            if (debugCanvas.GetComponent<GraphicRaycaster>() == null)
            {
                debugCanvas.gameObject.AddComponent<GraphicRaycaster>();
            }

            if (buildMenuView == null)
            {
                buildMenuView = FindObjectOfType<BuildMenuView>();
            }

            if (buildMenuView == null)
            {
                GameObject buildMenuObject = new GameObject("BuildMenuView", typeof(RectTransform), typeof(BuildMenuView));
                buildMenuObject.transform.SetParent(debugCanvas.transform, false);
                buildMenuView = buildMenuObject.GetComponent<BuildMenuView>();
            }

            if (buildMenuController == null)
            {
                buildMenuController = FindObjectOfType<BuildMenuController>();
            }

            if (buildMenuController == null)
            {
                GameObject buildMenuControllerObject = new GameObject("BuildMenuController");
                buildMenuController = buildMenuControllerObject.AddComponent<BuildMenuController>();
            }
        }

        private void EnsureBuildingDetailPanel()
        {
            EnsureEventSystem();

            if (debugCanvas == null)
            {
                debugCanvas = FindObjectOfType<Canvas>();
            }

            if (debugCanvas == null)
            {
                GameObject canvasObject = new GameObject("FactoryDebugCanvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
                debugCanvas = canvasObject.GetComponent<Canvas>();
                debugCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

                CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1280f, 720f);
            }

            if (debugCanvas.GetComponent<GraphicRaycaster>() == null)
            {
                debugCanvas.gameObject.AddComponent<GraphicRaycaster>();
            }

            if (buildingDetailPanelView == null)
            {
                buildingDetailPanelView = FindObjectOfType<BuildingDetailPanelView>();
            }

            if (buildingDetailPanelView == null)
            {
                GameObject panelObject = new GameObject("BuildingDetailPanelView", typeof(RectTransform), typeof(BuildingDetailPanelView));
                panelObject.transform.SetParent(debugCanvas.transform, false);
                buildingDetailPanelView = panelObject.GetComponent<BuildingDetailPanelView>();
            }

            if (buildingDetailPanelController == null)
            {
                buildingDetailPanelController = FindObjectOfType<BuildingDetailPanelController>();
            }

            if (buildingDetailPanelController == null)
            {
                GameObject controllerObject = new GameObject("BuildingDetailPanelController");
                buildingDetailPanelController = controllerObject.AddComponent<BuildingDetailPanelController>();
            }
        }

        private void EnsureGoalHud()
        {
            if (goalHudView == null)
            {
                goalHudView = FindObjectOfType<GoalHudView>();
            }

            if (goalHudView == null)
            {
                GameObject goalHudObject = new GameObject("GoalHudView");
                goalHudView = goalHudObject.AddComponent<GoalHudView>();
            }

            if (goalUpdateController == null)
            {
                goalUpdateController = FindObjectOfType<GoalUpdateController>();
            }

            if (goalUpdateController == null)
            {
                GameObject goalUpdateObject = new GameObject("GoalUpdateController");
                goalUpdateController = goalUpdateObject.AddComponent<GoalUpdateController>();
            }
        }

        private static void EnsureEventSystem()
        {
            EventSystem eventSystem = FindObjectOfType<EventSystem>();

            if (eventSystem == null)
            {
                GameObject eventSystemObject = new GameObject("EventSystem", typeof(EventSystem));
                eventSystem = eventSystemObject.GetComponent<EventSystem>();
            }

            StandaloneInputModule oldInputModule = eventSystem.GetComponent<StandaloneInputModule>();
            if (oldInputModule != null)
            {
                Destroy(oldInputModule);
            }

            if (eventSystem.GetComponent<InputSystemUIInputModule>() == null)
            {
                eventSystem.gameObject.AddComponent<InputSystemUIInputModule>();
            }
        }

        private static void EnsureCameraController()
        {
            Camera camera = Camera.main;

            if (camera == null)
            {
                GameObject cameraObject = new GameObject("Main Camera");
                cameraObject.tag = "MainCamera";
                camera = cameraObject.AddComponent<Camera>();
            }

            if (camera.GetComponent<CameraController>() == null)
            {
                camera.gameObject.AddComponent<CameraController>();
            }
        }

        private void SetupPlayer(GridModel model, float cellSize)
        {
            if (playerView == null)
            {
                playerView = FindObjectOfType<PlayerView>();
            }

            if (playerView == null)
            {
                GameObject playerObject = new GameObject("Player");
                playerObject.transform.position = new Vector3(5f, 0f, 5f);
                playerView = playerObject.AddComponent<PlayerView>();
            }

            playerView.Initialize(cellSize);

            PlayerMovementController movement = playerView.GetComponent<PlayerMovementController>();
            if (movement == null)
            {
                movement = playerView.gameObject.AddComponent<PlayerMovementController>();
            }

            if (model != null)
            {
                movement.InitializeBounds(-0.5f, -0.5f, model.Width - 0.5f, model.Height - 0.5f);
            }

            Camera camera = Camera.main;
            if (camera == null)
            {
                return;
            }

            PlayerCameraFollow follow = camera.GetComponent<PlayerCameraFollow>();
            if (follow == null)
            {
                follow = camera.gameObject.AddComponent<PlayerCameraFollow>();
            }

            follow.Initialize(playerView.transform);
        }

        private void SetupPlayerInteraction(GridModel model, float cellSize)
        {
            if (playerView == null)
            {
                return;
            }

            if (playerInteractionController == null)
            {
                playerInteractionController = playerView.GetComponent<PlayerInteractionController>();
            }

            if (playerInteractionController == null)
            {
                playerInteractionController = playerView.gameObject.AddComponent<PlayerInteractionController>();
            }

            playerInteractionController.Initialize(
                model,
                buildingSelectionController,
                storageCollectionController,
                cellSize);

            if (interactionHighlightView == null)
            {
                interactionHighlightView = FindObjectOfType<InteractionHighlightView>();
            }

            if (interactionHighlightView == null)
            {
                GameObject highlightObject = new GameObject("InteractionHighlightView");
                interactionHighlightView = highlightObject.AddComponent<InteractionHighlightView>();
            }

            interactionHighlightView.Initialize(playerInteractionController, cellSize);

            if (interactionHintHud == null)
            {
                interactionHintHud = FindObjectOfType<InteractionHintHud>();
            }

            if (interactionHintHud == null)
            {
                GameObject hintObject = new GameObject("InteractionHintHud");
                interactionHintHud = hintObject.AddComponent<InteractionHintHud>();
            }

            interactionHintHud.Initialize(playerInteractionController);
        }

        private static void SetupCamera()
        {
            Camera camera = Camera.main;

            if (camera == null)
            {
                GameObject cameraObject = new GameObject("Main Camera");
                cameraObject.tag = "MainCamera";
                camera = cameraObject.AddComponent<Camera>();
            }

            camera.orthographic = true;
            camera.orthographicSize = 7.4f;
            camera.transform.position = new Vector3(4.5f, 9.5f, -7.4f);
            camera.transform.rotation = Quaternion.Euler(58f, 0f, 0f);

            if (camera.GetComponent<CameraController>() == null)
            {
                camera.gameObject.AddComponent<CameraController>();
            }
        }

        private void PlaceDebugBuildings(GridModel model)
        {
            TryPlace(model, CreateBuilding("miner-iron-1", GetBuildingDefinition(BuildingType.Miner), new GridPosition(1, 1), BuildingDirection.East));
            TryPlace(model, CreateBuilding("conveyor-ore-1", GetBuildingDefinition(BuildingType.Conveyor), new GridPosition(2, 1), BuildingDirection.East));
            TryPlace(model, CreateBuilding("smelter-iron-1", GetBuildingDefinition(BuildingType.Smelter), new GridPosition(3, 1), BuildingDirection.East));
            TryPlace(model, CreateBuilding("conveyor-ingot-1", GetBuildingDefinition(BuildingType.Conveyor), new GridPosition(4, 1), BuildingDirection.East));
            TryPlace(model, CreateIronPlateAssembler(new GridPosition(5, 1), BuildingDirection.East));
            TryPlace(model, CreateBuilding("conveyor-plate-1", GetBuildingDefinition(BuildingType.Conveyor), new GridPosition(6, 1), BuildingDirection.East));
            TryPlace(model, CreateBuilding("storage-output-1", GetBuildingDefinition(BuildingType.Storage), new GridPosition(7, 1), BuildingDirection.North));

            TryPlace(model, CreateBuilding("miner-copper-1", GetBuildingDefinition(BuildingType.Miner), new GridPosition(1, 3), BuildingDirection.East));
            TryPlace(model, CreateBuilding("conveyor-copper-ore-1", GetBuildingDefinition(BuildingType.Conveyor), new GridPosition(2, 3), BuildingDirection.East));
            TryPlace(model, CreateBuilding("smelter-copper-1", GetBuildingDefinition(BuildingType.Smelter), new GridPosition(3, 3), BuildingDirection.East));
            TryPlace(model, CreateBuilding("conveyor-copper-ingot-1", GetBuildingDefinition(BuildingType.Conveyor), new GridPosition(4, 3), BuildingDirection.East));
            TryPlace(model, CreateCopperWireAssembler(new GridPosition(5, 3), BuildingDirection.East));
            TryPlace(model, CreateBuilding("conveyor-wire-1", GetBuildingDefinition(BuildingType.Conveyor), new GridPosition(6, 3), BuildingDirection.East));
            TryPlace(model, CreateBuilding("storage-wire-1", GetBuildingDefinition(BuildingType.Storage), new GridPosition(7, 3), BuildingDirection.North));

            TryPlace(model, CreateSeededGearAssembler(new GridPosition(5, 5), BuildingDirection.East));
            TryPlace(model, CreateBuilding("conveyor-gear-1", GetBuildingDefinition(BuildingType.Conveyor), new GridPosition(6, 5), BuildingDirection.East));
            TryPlace(model, CreateBuilding("storage-gear-1", GetBuildingDefinition(BuildingType.Storage), new GridPosition(7, 5), BuildingDirection.North));

            TryPlace(model, CreateBuilding("generator-1", GetBuildingDefinition(BuildingType.Generator), new GridPosition(0, 6), BuildingDirection.East));
            TryPlace(model, CreateBuilding("generator-2", GetBuildingDefinition(BuildingType.Generator), new GridPosition(1, 6), BuildingDirection.East));
            TryPlace(model, CreateBuilding("generator-3", GetBuildingDefinition(BuildingType.Generator), new GridPosition(2, 6), BuildingDirection.East));
        }

        private static BuildingModel CreateBuilding(
            string instanceId,
            BuildingDefinition definition,
            GridPosition origin,
            BuildingDirection direction)
        {
            return new BuildingModel(instanceId, definition, origin, direction);
        }

        private BuildingModel CreateIronPlateAssembler(GridPosition origin, BuildingDirection direction)
        {
            BuildingModel assembler = CreateBuilding(
                "assembler-iron-plate-1",
                GetBuildingDefinition(BuildingType.Assembler),
                origin,
                direction);

            assembler.SelectedRecipeId = RecipeCatalog.IronPlateRecipeId;
            return assembler;
        }

        private BuildingModel CreateCopperWireAssembler(GridPosition origin, BuildingDirection direction)
        {
            BuildingModel assembler = CreateBuilding(
                "assembler-copper-wire-1",
                GetBuildingDefinition(BuildingType.Assembler),
                origin,
                direction);

            assembler.SelectedRecipeId = RecipeCatalog.CopperWireRecipeId;
            return assembler;
        }

        private BuildingModel CreateSeededGearAssembler(GridPosition origin, BuildingDirection direction)
        {
            BuildingModel assembler = CreateBuilding(
                "assembler-gear-1",
                GetBuildingDefinition(BuildingType.Assembler),
                origin,
                direction);

            assembler.SelectedRecipeId = RecipeCatalog.GearRecipeId;
            assembler.Inventory.Add(ResourceType.IronPlate, 10);
            return assembler;
        }

        private static BaseInventoryModel CreateBaseInventory()
        {
            BaseInventoryModel inventory = new BaseInventoryModel();
            inventory.Add(ResourceType.IronPlate, 20);
            inventory.Add(ResourceType.CopperWire, 20);
            return inventory;
        }

        private static GoalTracker CreateGoalTracker(BaseInventoryModel baseInventory)
        {
            return new GoalTracker(
                baseInventory,
                DebugGoalDefinitions.CreateGoals());
        }

        private void LoadDefinitions()
        {
            try
            {
                if (definitionDatabase != null)
                {
                    IReadOnlyDictionary<string, BuildingDefinition> buildingDefinitions = definitionDatabase.CreateBuildingDefinitions();
                    IReadOnlyDictionary<string, RecipeModel> recipeDefinitions = definitionDatabase.CreateRecipeDefinitions();

                    if (buildingDefinitions.Count > 0)
                    {
                        _buildingDefinitionsById = buildingDefinitions;
                        _buildingDefinitionsByType = buildingDefinitions.Values
                            .GroupBy(definition => definition.Type)
                            .ToDictionary(group => group.Key, group => group.First());
                        _buildMenuDefinitions = definitionDatabase.CreateBuildMenuDefinitions();
                    }

                    if (recipeDefinitions.Count > 0)
                    {
                        _recipeCatalog = new RecipeCatalog(recipeDefinitions.Values);
                    }
                }
            }
            catch (System.Exception exception)
            {
                Debug.LogWarning("DefinitionDatabase failed. Falling back to debug definitions. " + exception.Message);
            }

            if (_buildingDefinitionsById == null || _buildingDefinitionsByType == null || _buildMenuDefinitions == null)
            {
                _buildMenuDefinitions = DebugBuildingDefinitions.GetBuildMenuDefinitions();
                _buildingDefinitionsById = _buildMenuDefinitions.ToDictionary(definition => definition.Id);
                _buildingDefinitionsByType = _buildMenuDefinitions
                    .GroupBy(definition => definition.Type)
                    .ToDictionary(group => group.Key, group => group.First());
            }

            if (_recipeCatalog == null)
            {
                _recipeCatalog = RecipeCatalog.CreateDefault();
            }
        }

        private BuildingDefinition GetBuildingDefinition(BuildingType type)
        {
            if (_buildingDefinitionsByType != null && _buildingDefinitionsByType.TryGetValue(type, out BuildingDefinition definition))
            {
                return definition;
            }

            return DebugBuildingDefinitions.Get(type);
        }

        private static void TryPlace(GridModel model, BuildingModel building)
        {
            if (model.TryPlaceBuilding(building))
            {
                return;
            }

            Debug.LogWarning(
                "Failed to place debug building '" + building.InstanceId
                + "' type '" + building.Definition.Type
                + "' at " + building.Origin
                + " direction '" + building.Direction + "'.");
        }
    }
}
