using UnityEngine;
using UnityEngine.EventSystems;
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

        private void Start()
        {
            EnsureViews();
            GridModel model = gridView.CreateDefaultModel();
            model.SetResourceNode(new GridPosition(1, 1), ResourceType.IronOre);
            model.SetResourceNode(new GridPosition(3, 2), ResourceType.CopperOre);
            model.SetResourceNode(new GridPosition(5, 5), ResourceType.Coal);

            PlaceDebugBuildings(model);
            FactorySimulation simulation = new FactorySimulation(model);
            BaseInventoryModel baseInventory = CreateBaseInventory();
            StorageCollector storageCollector = new StorageCollector(model, baseInventory);

            gridView.Build(model);
            buildingViewFactory.Build(model, gridView.CellSize);
            gridMouseSelector.Initialize(model, gridView.CellSize);
            placementPreview.Initialize(model, gridMouseSelector, gridView.CellSize, baseInventory);
            placementController.Initialize(model, gridMouseSelector, placementPreview, buildingViewFactory, gridView.CellSize, baseInventory);
            buildingSelectionController.Initialize(model, gridMouseSelector, buildingViewFactory, gridView.CellSize, baseInventory);
            debugBuildingSelectionController.Initialize(placementPreview);
            storageCollectionController.Initialize(storageCollector);
            buildMenuController.Initialize(buildMenuView, placementPreview, DebugBuildingDefinitions.GetBuildMenuDefinitions());
            simulationTickRunner.Initialize(simulation);
            debugHud.Initialize(
                simulation,
                simulationTickRunner,
                gridMouseSelector,
                placementPreview,
                placementController,
                buildingSelectionController,
                baseInventory,
                storageCollectionController);
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

            EnsureBuildMenu();
            EnsureCameraController();
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

        private static void EnsureEventSystem()
        {
            if (FindObjectOfType<EventSystem>() != null)
            {
                return;
            }

            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
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

        private void PlaceDebugBuildings(GridModel model)
        {
            TryPlace(model, CreateBuilding("miner-iron-1", DebugBuildingDefinitions.Get(BuildingType.Miner), new GridPosition(1, 1), BuildingDirection.East));
            TryPlace(model, CreateBuilding("conveyor-ore-1", DebugBuildingDefinitions.Get(BuildingType.Conveyor), new GridPosition(2, 1), BuildingDirection.East));
            TryPlace(model, CreateBuilding("smelter-iron-1", DebugBuildingDefinitions.Get(BuildingType.Smelter), new GridPosition(3, 1), BuildingDirection.East));
            TryPlace(model, CreateBuilding("conveyor-ingot-1", DebugBuildingDefinitions.Get(BuildingType.Conveyor), new GridPosition(4, 1), BuildingDirection.East));
            TryPlace(model, CreateIronPlateAssembler(new GridPosition(5, 1), BuildingDirection.East));
            TryPlace(model, CreateBuilding("conveyor-plate-1", DebugBuildingDefinitions.Get(BuildingType.Conveyor), new GridPosition(6, 1), BuildingDirection.East));
            TryPlace(model, CreateBuilding("storage-output-1", DebugBuildingDefinitions.Get(BuildingType.Storage), new GridPosition(7, 1), BuildingDirection.North));
            TryPlace(model, CreateBuilding("generator-1", DebugBuildingDefinitions.Get(BuildingType.Generator), new GridPosition(0, 6), BuildingDirection.East));
            TryPlace(model, CreateBuilding("generator-2", DebugBuildingDefinitions.Get(BuildingType.Generator), new GridPosition(1, 6), BuildingDirection.East));
        }

        private static BuildingModel CreateBuilding(
            string instanceId,
            BuildingDefinition definition,
            GridPosition origin,
            BuildingDirection direction)
        {
            return new BuildingModel(instanceId, definition, origin, direction);
        }

        private static BuildingModel CreateIronPlateAssembler(GridPosition origin, BuildingDirection direction)
        {
            BuildingModel assembler = CreateBuilding(
                "assembler-iron-plate-1",
                DebugBuildingDefinitions.Get(BuildingType.Assembler),
                origin,
                direction);

            assembler.SelectedRecipeId = RecipeCatalog.IronPlateRecipeId;
            return assembler;
        }

        private static BaseInventoryModel CreateBaseInventory()
        {
            BaseInventoryModel inventory = new BaseInventoryModel();
            inventory.Add(ResourceType.IronPlate, 100);
            inventory.Add(ResourceType.CopperWire, 100);
            inventory.Add(ResourceType.Gear, 20);
            inventory.Add(ResourceType.BasicCircuit, 10);
            return inventory;
        }

        private static void TryPlace(GridModel model, BuildingModel building)
        {
            if (model.TryPlaceBuilding(building))
            {
                return;
            }

            Debug.LogWarning($"Failed to place debug building '{building.InstanceId}' at {building.Origin}.");
        }
    }
}
