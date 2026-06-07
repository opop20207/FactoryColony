using System;
using System.Collections.Generic;
using System.Linq;

namespace FactoryColony
{
    public sealed class FactorySimulation
    {
        private readonly GridModel _gridModel;
        private readonly RecipeCatalog _recipeCatalog;

        public FactorySimulation(GridModel gridModel)
        {
            _gridModel = gridModel ?? throw new ArgumentNullException(nameof(gridModel));
            _recipeCatalog = RecipeCatalog.CreateDefault();
        }

        public IEnumerable<BuildingModel> GetBuildingsByType(BuildingType type)
        {
            return _gridModel.GetAllBuildings().Where(building => building.Definition.Type == type);
        }

        public IEnumerable<BuildingModel> GetStorageBuildings()
        {
            return _gridModel.GetAllBuildings().Where(building => building.CanStoreResources);
        }

        public IEnumerable<BuildingModel> GetProducerBuildings()
        {
            return _gridModel.GetAllBuildings().Where(building => building.CanProduceResources);
        }

        public bool CanReceiveResource(BuildingModel building)
        {
            return building != null
                && (building.Definition.Type == BuildingType.Storage
                    || building.Definition.Type == BuildingType.Conveyor
                    || building.Definition.Type == BuildingType.Smelter
                    || building.Definition.Type == BuildingType.Assembler);
        }

        public int GetStoredAmount(ResourceType type)
        {
            if (type == ResourceType.None)
            {
                return 0;
            }

            return GetStorageBuildings().Sum(storage => storage.Inventory.GetAmount(type));
        }

        public IReadOnlyDictionary<ResourceType, int> GetStoredResources()
        {
            Dictionary<ResourceType, int> storedResources = new Dictionary<ResourceType, int>();

            foreach (BuildingModel storage in GetStorageBuildings())
            {
                foreach (ResourceStack stack in storage.Inventory.GetStacks())
                {
                    storedResources[stack.Type] = GetStoredAmount(stack.Type);
                }
            }

            return storedResources;
        }

        public PowerModel CalculatePower()
        {
            int producedPower = 0;
            int consumedPower = 0;

            foreach (BuildingModel building in _gridModel.GetAllBuildings())
            {
                producedPower += building.Definition.PowerProduction;
                consumedPower += building.Definition.PowerConsumption;
            }

            return new PowerModel(producedPower, consumedPower);
        }

        public bool HasEnoughPower()
        {
            return CalculatePower().HasEnoughPower;
        }

        public void SimulateTick()
        {
            PowerModel power = CalculatePower();

            if (power.HasEnoughPower)
            {
                SimulateMinerProduction();
                SimulateMinerOutput();
            }

            SimulateConveyorMovement();

            if (power.HasEnoughPower)
            {
                SimulateSmelterProcessing();
                SimulateSmelterOutput();
                SimulateAssemblerProduction();
                SimulateAssemblerOutput();
            }
        }

        private void SimulateMinerProduction()
        {
            foreach (BuildingModel building in GetOrderedBuildings(BuildingType.Miner))
            {
                ResourceType resourceType = _gridModel.GetCell(building.Origin).ResourceNodeType;

                if (resourceType == ResourceType.None)
                {
                    continue;
                }

                building.Inventory.Add(resourceType, 1);
            }
        }

        private void SimulateMinerOutput()
        {
            BuildingMove[] moves = GetOrderedBuildings(BuildingType.Miner)
                .Select(CreateBuildingMove)
                .Where(move => move.CanMove)
                .ToArray();

            foreach (BuildingMove move in moves)
            {
                TryMoveResourceForward(move.Building, move.Stack);
            }
        }

        private void SimulateConveyorMovement()
        {
            BuildingMove[] moves = GetOrderedBuildings(BuildingType.Conveyor)
                .Select(CreateBuildingMove)
                .Where(move => move.CanMove)
                .ToArray();

            foreach (BuildingMove move in moves)
            {
                TryMoveResourceForward(move.Building, move.Stack);
            }
        }

        private void SimulateSmelterProcessing()
        {
            foreach (BuildingModel smelter in GetOrderedBuildings(BuildingType.Smelter))
            {
                if (!smelter.Inventory.TryTakeOne(IsSmeltableOre, out ResourceStack inputStack))
                {
                    continue;
                }

                smelter.Inventory.Add(GetSmeltedResourceType(inputStack.Type), 1);
            }
        }

        private void SimulateSmelterOutput()
        {
            BuildingMove[] moves = GetOrderedBuildings(BuildingType.Smelter)
                .Select(CreateSmelterOutputMove)
                .Where(move => move.CanMove)
                .ToArray();

            foreach (BuildingMove move in moves)
            {
                TryMoveResourceForward(move.Building, move.Stack);
            }
        }

        private void SimulateAssemblerProduction()
        {
            foreach (BuildingModel assembler in GetOrderedBuildings(BuildingType.Assembler))
            {
                if (!_recipeCatalog.TryGetRecipe(assembler.SelectedRecipeId, out RecipeModel recipe))
                {
                    continue;
                }

                if (recipe.RequiredBuildingType != BuildingType.Assembler)
                {
                    continue;
                }

                if (!assembler.Inventory.TryRemoveAll(recipe.Inputs))
                {
                    continue;
                }

                assembler.Inventory.AddAll(recipe.Outputs);
            }
        }

        private void SimulateAssemblerOutput()
        {
            BuildingMove[] moves = GetOrderedBuildings(BuildingType.Assembler)
                .Select(CreateAssemblerOutputMove)
                .Where(move => move.CanMove)
                .ToArray();

            foreach (BuildingMove move in moves)
            {
                TryMoveResourceForward(move.Building, move.Stack);
            }
        }

        private IEnumerable<BuildingModel> GetOrderedBuildings(BuildingType type)
        {
            return GetBuildingsByType(type)
                .OrderBy(building => building.Origin.X)
                .ThenBy(building => building.Origin.Y);
        }

        private BuildingMove CreateBuildingMove(BuildingModel building)
        {
            building.Inventory.TryPeekOne(out ResourceStack stack);
            return new BuildingMove(building, stack);
        }

        private BuildingMove CreateSmelterOutputMove(BuildingModel smelter)
        {
            smelter.Inventory.TryPeekOne(IsSmeltedResource, out ResourceStack stack);
            return new BuildingMove(smelter, stack);
        }

        private BuildingMove CreateAssemblerOutputMove(BuildingModel assembler)
        {
            ResourceStack stack = null;

            if (_recipeCatalog.TryGetRecipe(assembler.SelectedRecipeId, out RecipeModel recipe))
            {
                assembler.Inventory.TryPeekOne(recipe.Outputs.ContainsKey, out stack);
            }

            if (stack == null)
            {
                assembler.Inventory.TryPeekOne(out stack);
            }

            return new BuildingMove(assembler, stack);
        }

        private bool TryMoveResourceForward(BuildingModel source, ResourceStack stack)
        {
            if (stack == null || !TryGetForwardDestination(source, out BuildingModel destination))
            {
                return false;
            }

            if (!CanMoveResource(source, destination, stack.Type))
            {
                return false;
            }

            if (!source.Inventory.TryRemove(stack.Type, 1))
            {
                return false;
            }

            destination.Inventory.Add(stack.Type, 1);
            return true;
        }

        private bool CanMoveResource(BuildingModel source, BuildingModel destination, ResourceType resourceType)
        {
            if (!CanReceiveResource(destination, resourceType))
            {
                return false;
            }

            switch (source.Definition.Type)
            {
                case BuildingType.Miner:
                case BuildingType.Conveyor:
                    return true;
                case BuildingType.Assembler:
                    return destination.Definition.Type == BuildingType.Conveyor
                        || destination.Definition.Type == BuildingType.Storage;
                case BuildingType.Smelter:
                    return destination.Definition.Type == BuildingType.Conveyor
                        || destination.Definition.Type == BuildingType.Storage
                        || destination.Definition.Type == BuildingType.Assembler;
                default:
                    return false;
            }
        }

        private bool CanReceiveResource(BuildingModel building, ResourceType resourceType)
        {
            if (building == null || resourceType == ResourceType.None)
            {
                return false;
            }

            switch (building.Definition.Type)
            {
                case BuildingType.Conveyor:
                case BuildingType.Storage:
                    return true;
                case BuildingType.Smelter:
                    return IsSmeltableOre(resourceType);
                case BuildingType.Assembler:
                    return true;
                default:
                    return false;
            }
        }

        private bool TryGetForwardDestination(BuildingModel source, out BuildingModel destination)
        {
            GridPosition offset = source.Direction.ToOffset();
            GridPosition destinationPosition = new GridPosition(
                source.Origin.X + offset.X,
                source.Origin.Y + offset.Y);

            if (!_gridModel.TryGetCell(destinationPosition, out GridCell destinationCell)
                || !destinationCell.HasBuilding)
            {
                destination = null;
                return false;
            }

            return _gridModel.TryGetBuilding(destinationCell.OccupiedByBuildingId, out destination);
        }

        private static bool IsSmeltableOre(ResourceType type)
        {
            return type == ResourceType.IronOre || type == ResourceType.CopperOre;
        }

        private static bool IsSmeltedResource(ResourceType type)
        {
            return type == ResourceType.IronIngot || type == ResourceType.CopperIngot;
        }

        private static ResourceType GetSmeltedResourceType(ResourceType type)
        {
            switch (type)
            {
                case ResourceType.IronOre:
                    return ResourceType.IronIngot;
                case ResourceType.CopperOre:
                    return ResourceType.CopperIngot;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, "Resource type cannot be smelted.");
            }
        }

        private readonly struct BuildingMove
        {
            public BuildingModel Building { get; }
            public ResourceStack Stack { get; }
            public bool CanMove
            {
                get { return Stack != null && Stack.Amount > 0; }
            }

            public BuildingMove(BuildingModel building, ResourceStack stack)
            {
                Building = building;
                Stack = stack;
            }
        }
    }
}
