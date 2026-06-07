using UnityEngine;

namespace FactoryColony
{
    public sealed class FactoryDebugSceneBootstrap : MonoBehaviour
    {
        [SerializeField] private GridView gridView;
        [SerializeField] private BuildingViewFactory buildingViewFactory;

        private void Start()
        {
            EnsureViews();
            GridModel model = gridView.CreateDefaultModel();
            model.SetResourceNode(new GridPosition(1, 1), ResourceType.IronOre);
            model.SetResourceNode(new GridPosition(3, 2), ResourceType.CopperOre);
            model.SetResourceNode(new GridPosition(5, 5), ResourceType.Coal);

            PlaceDebugBuildings(model);
            gridView.Build(model);
            buildingViewFactory.Build(model, gridView.CellSize);
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
        }

        private void PlaceDebugBuildings(GridModel model)
        {
            TryPlace(model, CreateBuilding("miner-iron-1", DebugDefinition(BuildingType.Miner, 1, 1, true, ResourceType.IronOre), new GridPosition(1, 1), BuildingDirection.East));
            TryPlace(model, CreateBuilding("conveyor-input-1", DebugDefinition(BuildingType.Conveyor, 1, 1), new GridPosition(2, 1), BuildingDirection.East));
            TryPlace(model, CreateBuilding("conveyor-input-2", DebugDefinition(BuildingType.Conveyor, 1, 1), new GridPosition(3, 1), BuildingDirection.East));
            TryPlace(model, CreateBuilding("smelter-1", DebugDefinition(BuildingType.Smelter, 2, 2), new GridPosition(4, 1), BuildingDirection.East));
            TryPlace(model, CreateBuilding("conveyor-output-1", DebugDefinition(BuildingType.Conveyor, 1, 1), new GridPosition(6, 1), BuildingDirection.East));
            TryPlace(model, CreateBuilding("conveyor-output-2", DebugDefinition(BuildingType.Conveyor, 1, 1), new GridPosition(7, 1), BuildingDirection.East));
            TryPlace(model, CreateBuilding("storage-1", DebugDefinition(BuildingType.Storage, 1, 1), new GridPosition(8, 1), BuildingDirection.North));
            TryPlace(model, CreateBuilding("generator-1", DebugDefinition(BuildingType.Generator, 2, 2), new GridPosition(0, 6), BuildingDirection.East));
            TryPlace(model, CreateBuilding("assembler-1", DebugDefinition(BuildingType.Assembler, 2, 2), new GridPosition(6, 4), BuildingDirection.East));
        }

        private static BuildingModel CreateBuilding(
            string instanceId,
            BuildingDefinition definition,
            GridPosition origin,
            BuildingDirection direction)
        {
            return new BuildingModel(instanceId, definition, origin, direction);
        }

        private static BuildingDefinition DebugDefinition(BuildingType type, int width, int height)
        {
            return DebugDefinition(type, width, height, false, ResourceType.None);
        }

        private static BuildingDefinition DebugDefinition(
            BuildingType type,
            int width,
            int height,
            bool requiresResourceNode,
            ResourceType requiredResourceType)
        {
            return new BuildingDefinition(
                "debug-" + type,
                type,
                "Debug " + type,
                width,
                height,
                requiresResourceNode,
                requiredResourceType,
                true,
                GetPowerProduction(type),
                GetPowerConsumption(type));
        }

        private static int GetPowerProduction(BuildingType type)
        {
            return type == BuildingType.Generator ? 10 : 0;
        }

        private static int GetPowerConsumption(BuildingType type)
        {
            switch (type)
            {
                case BuildingType.Miner:
                    return 2;
                case BuildingType.Smelter:
                    return 4;
                case BuildingType.Assembler:
                    return 5;
                default:
                    return 0;
            }
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
