using UnityEngine;

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
        [SerializeField] private DebugBuildingSelectionController buildingSelectionController;

        private void Start()
        {
            EnsureViews();
            GridModel model = gridView.CreateDefaultModel();
            model.SetResourceNode(new GridPosition(1, 1), ResourceType.IronOre);
            model.SetResourceNode(new GridPosition(3, 2), ResourceType.CopperOre);
            model.SetResourceNode(new GridPosition(5, 5), ResourceType.Coal);

            PlaceDebugBuildings(model);
            FactorySimulation simulation = new FactorySimulation(model);

            gridView.Build(model);
            buildingViewFactory.Build(model, gridView.CellSize);
            gridMouseSelector.Initialize(model, gridView.CellSize);
            placementPreview.Initialize(model, gridMouseSelector, gridView.CellSize);
            buildingSelectionController.Initialize(placementPreview);
            simulationTickRunner.Initialize(simulation);
            debugHud.Initialize(simulation, simulationTickRunner, gridMouseSelector, placementPreview);
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

            if (buildingSelectionController == null)
            {
                buildingSelectionController = FindObjectOfType<DebugBuildingSelectionController>();
            }

            if (buildingSelectionController == null)
            {
                GameObject selectionObject = new GameObject("DebugBuildingSelectionController");
                buildingSelectionController = selectionObject.AddComponent<DebugBuildingSelectionController>();
            }

            EnsureCameraController();
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
