using FactoryColony;
using System.Linq;
using NUnit.Framework;

namespace FactoryColony.Tests.EditMode.Factory
{
    public sealed class FactorySimulationTests
    {
        [Test]
        public void SimulateTick_ProducesIronOreWhenMinerIsOnIronOreNode()
        {
            GridModel gridModel = new GridModel(10, 10);
            BuildingModel miner = PlaceBuildingOnResource(gridModel, "miner-1", BuildingType.Miner, ResourceType.IronOre);
            FactorySimulation simulation = new FactorySimulation(gridModel);

            simulation.SimulateTick();

            Assert.AreEqual(1, miner.Inventory.GetAmount(ResourceType.IronOre));
        }

        [Test]
        public void SimulateTick_ProducesCopperOreWhenMinerIsOnCopperOreNode()
        {
            GridModel gridModel = new GridModel(10, 10);
            BuildingModel miner = PlaceBuildingOnResource(gridModel, "miner-1", BuildingType.Miner, ResourceType.CopperOre);
            FactorySimulation simulation = new FactorySimulation(gridModel);

            simulation.SimulateTick();

            Assert.AreEqual(1, miner.Inventory.GetAmount(ResourceType.CopperOre));
        }

        [Test]
        public void SimulateTick_DoesNotProduceWhenMinerIsNotOnResourceNode()
        {
            GridModel gridModel = new GridModel(10, 10);
            BuildingModel miner = CreateBuilding("miner-1", BuildingType.Miner, new GridPosition(1, 1));
            gridModel.PlaceBuilding(miner);
            FactorySimulation simulation = new FactorySimulation(gridModel);

            simulation.SimulateTick();

            Assert.AreEqual(0, miner.Inventory.GetAmount(ResourceType.IronOre));
            Assert.AreEqual(0, miner.Inventory.GetAmount(ResourceType.CopperOre));
            Assert.AreEqual(0, miner.Inventory.GetAmount(ResourceType.Coal));
        }

        [Test]
        public void SimulateTick_DoesNotProduceWhenBuildingIsNotMiner()
        {
            GridModel gridModel = new GridModel(10, 10);
            BuildingModel storage = PlaceBuildingOnResource(gridModel, "storage-1", BuildingType.Storage, ResourceType.IronOre);
            FactorySimulation simulation = new FactorySimulation(gridModel);

            simulation.SimulateTick();

            Assert.AreEqual(0, storage.Inventory.GetAmount(ResourceType.IronOre));
        }

        [Test]
        public void SimulateTick_AccumulatesResourcesAcrossMultipleTicks()
        {
            GridModel gridModel = new GridModel(10, 10);
            BuildingModel miner = PlaceBuildingOnResource(gridModel, "miner-1", BuildingType.Miner, ResourceType.IronOre);
            FactorySimulation simulation = new FactorySimulation(gridModel);

            simulation.SimulateTick();
            simulation.SimulateTick();
            simulation.SimulateTick();

            Assert.AreEqual(3, miner.Inventory.GetAmount(ResourceType.IronOre));
        }

        [Test]
        public void GetStorageBuildings_ReturnsOnlyStorageBuildings()
        {
            GridModel gridModel = new GridModel(10, 10);
            BuildingModel storage = CreateBuilding("storage-1", BuildingType.Storage, new GridPosition(1, 1));
            BuildingModel miner = CreateBuilding("miner-1", BuildingType.Miner, new GridPosition(2, 2));
            gridModel.PlaceBuilding(storage);
            gridModel.PlaceBuilding(miner);
            FactorySimulation simulation = new FactorySimulation(gridModel);

            BuildingModel[] storageBuildings = simulation.GetStorageBuildings().ToArray();

            Assert.AreEqual(1, storageBuildings.Length);
            Assert.AreEqual(storage, storageBuildings[0]);
        }

        [Test]
        public void GetProducerBuildings_ReturnsOnlyMinerBuildings()
        {
            GridModel gridModel = new GridModel(10, 10);
            BuildingModel storage = CreateBuilding("storage-1", BuildingType.Storage, new GridPosition(1, 1));
            BuildingModel miner = CreateBuilding("miner-1", BuildingType.Miner, new GridPosition(2, 2));
            gridModel.PlaceBuilding(storage);
            gridModel.PlaceBuilding(miner);
            FactorySimulation simulation = new FactorySimulation(gridModel);

            BuildingModel[] producerBuildings = simulation.GetProducerBuildings().ToArray();

            Assert.AreEqual(1, producerBuildings.Length);
            Assert.AreEqual(miner, producerBuildings[0]);
        }

        [Test]
        public void SimulateTick_MovesOneResourceFromConveyorToStorageInDirection()
        {
            GridModel gridModel = new GridModel(10, 10);
            BuildingModel conveyor = CreateBuilding("conveyor-1", BuildingType.Conveyor, new GridPosition(1, 1), BuildingDirection.East);
            BuildingModel storage = CreateBuilding("storage-1", BuildingType.Storage, new GridPosition(2, 1));
            gridModel.PlaceBuilding(conveyor);
            gridModel.PlaceBuilding(storage);
            conveyor.Inventory.Add(ResourceType.IronOre, 1);
            FactorySimulation simulation = new FactorySimulation(gridModel);

            simulation.SimulateTick();

            Assert.AreEqual(0, conveyor.Inventory.GetAmount(ResourceType.IronOre));
            Assert.AreEqual(1, storage.Inventory.GetAmount(ResourceType.IronOre));
        }

        [Test]
        public void SimulateTick_MovesOneResourceFromConveyorToNextConveyor()
        {
            GridModel gridModel = new GridModel(10, 10);
            BuildingModel source = CreateBuilding("conveyor-1", BuildingType.Conveyor, new GridPosition(1, 1), BuildingDirection.East);
            BuildingModel destination = CreateBuilding("conveyor-2", BuildingType.Conveyor, new GridPosition(2, 1), BuildingDirection.East);
            gridModel.PlaceBuilding(source);
            gridModel.PlaceBuilding(destination);
            source.Inventory.Add(ResourceType.IronOre, 1);
            FactorySimulation simulation = new FactorySimulation(gridModel);

            simulation.SimulateTick();

            Assert.AreEqual(0, source.Inventory.GetAmount(ResourceType.IronOre));
            Assert.AreEqual(1, destination.Inventory.GetAmount(ResourceType.IronOre));
        }

        [Test]
        public void SimulateTick_DoesNotMoveConveyorResourceOutsideGrid()
        {
            GridModel gridModel = new GridModel(10, 10);
            BuildingModel conveyor = CreateBuilding("conveyor-1", BuildingType.Conveyor, new GridPosition(0, 0), BuildingDirection.West);
            gridModel.PlaceBuilding(conveyor);
            conveyor.Inventory.Add(ResourceType.IronOre, 1);
            FactorySimulation simulation = new FactorySimulation(gridModel);

            simulation.SimulateTick();

            Assert.AreEqual(1, conveyor.Inventory.GetAmount(ResourceType.IronOre));
        }

        [Test]
        public void SimulateTick_DoesNotMoveConveyorResourceWhenNextCellIsEmpty()
        {
            GridModel gridModel = new GridModel(10, 10);
            BuildingModel conveyor = CreateBuilding("conveyor-1", BuildingType.Conveyor, new GridPosition(1, 1), BuildingDirection.East);
            gridModel.PlaceBuilding(conveyor);
            conveyor.Inventory.Add(ResourceType.IronOre, 1);
            FactorySimulation simulation = new FactorySimulation(gridModel);

            simulation.SimulateTick();

            Assert.AreEqual(1, conveyor.Inventory.GetAmount(ResourceType.IronOre));
        }

        [Test]
        public void SimulateTick_DoesNotMoveConveyorResourceToMiner()
        {
            GridModel gridModel = new GridModel(10, 10);
            BuildingModel conveyor = CreateBuilding("conveyor-1", BuildingType.Conveyor, new GridPosition(1, 1), BuildingDirection.East);
            BuildingModel miner = CreateBuilding("miner-1", BuildingType.Miner, new GridPosition(2, 1));
            gridModel.PlaceBuilding(conveyor);
            gridModel.PlaceBuilding(miner);
            conveyor.Inventory.Add(ResourceType.IronOre, 1);
            FactorySimulation simulation = new FactorySimulation(gridModel);

            simulation.SimulateTick();

            Assert.AreEqual(1, conveyor.Inventory.GetAmount(ResourceType.IronOre));
            Assert.AreEqual(0, miner.Inventory.GetAmount(ResourceType.IronOre));
        }

        [Test]
        public void SimulateTick_MovesAtMostOneResourcePerConveyor()
        {
            GridModel gridModel = new GridModel(10, 10);
            BuildingModel conveyor = CreateBuilding("conveyor-1", BuildingType.Conveyor, new GridPosition(1, 1), BuildingDirection.East);
            BuildingModel storage = CreateBuilding("storage-1", BuildingType.Storage, new GridPosition(2, 1));
            gridModel.PlaceBuilding(conveyor);
            gridModel.PlaceBuilding(storage);
            conveyor.Inventory.Add(ResourceType.IronOre, 2);
            FactorySimulation simulation = new FactorySimulation(gridModel);

            simulation.SimulateTick();

            Assert.AreEqual(1, conveyor.Inventory.GetAmount(ResourceType.IronOre));
            Assert.AreEqual(1, storage.Inventory.GetAmount(ResourceType.IronOre));
        }

        [Test]
        public void SimulateTick_DoesNotChainConveyorMovementWithinSameTick()
        {
            GridModel gridModel = new GridModel(10, 10);
            BuildingModel firstConveyor = CreateBuilding("conveyor-1", BuildingType.Conveyor, new GridPosition(0, 0), BuildingDirection.East);
            BuildingModel secondConveyor = CreateBuilding("conveyor-2", BuildingType.Conveyor, new GridPosition(1, 0), BuildingDirection.East);
            BuildingModel storage = CreateBuilding("storage-1", BuildingType.Storage, new GridPosition(2, 0));
            gridModel.PlaceBuilding(firstConveyor);
            gridModel.PlaceBuilding(secondConveyor);
            gridModel.PlaceBuilding(storage);
            firstConveyor.Inventory.Add(ResourceType.IronOre, 1);
            FactorySimulation simulation = new FactorySimulation(gridModel);

            simulation.SimulateTick();

            Assert.AreEqual(0, firstConveyor.Inventory.GetAmount(ResourceType.IronOre));
            Assert.AreEqual(1, secondConveyor.Inventory.GetAmount(ResourceType.IronOre));
            Assert.AreEqual(0, storage.Inventory.GetAmount(ResourceType.IronOre));
        }

        [Test]
        public void SimulateTick_ProducerTickRunsBeforeConveyorTick()
        {
            GridModel gridModel = new GridModel(10, 10);
            BuildingModel miner = PlaceBuildingOnResource(gridModel, "miner-1", BuildingType.Miner, ResourceType.IronOre);
            BuildingModel conveyor = CreateBuilding("conveyor-1", BuildingType.Conveyor, new GridPosition(3, 3), BuildingDirection.East);
            BuildingModel storage = CreateBuilding("storage-1", BuildingType.Storage, new GridPosition(4, 3));
            gridModel.PlaceBuilding(conveyor);
            gridModel.PlaceBuilding(storage);
            conveyor.Inventory.Add(ResourceType.CopperOre, 1);
            FactorySimulation simulation = new FactorySimulation(gridModel);

            simulation.SimulateTick();

            Assert.AreEqual(1, miner.Inventory.GetAmount(ResourceType.IronOre));
            Assert.AreEqual(1, storage.Inventory.GetAmount(ResourceType.CopperOre));
        }

        [Test]
        public void SimulateTick_MinerOutputsProducedResourceToConveyorInDirection()
        {
            GridModel gridModel = new GridModel(10, 10);
            BuildingModel miner = PlaceBuildingOnResource(gridModel, "miner-1", BuildingType.Miner, ResourceType.IronOre, new GridPosition(1, 1), BuildingDirection.East);
            BuildingModel conveyor = CreateBuilding("conveyor-1", BuildingType.Conveyor, new GridPosition(2, 1), BuildingDirection.East);
            gridModel.PlaceBuilding(conveyor);
            FactorySimulation simulation = new FactorySimulation(gridModel);

            simulation.SimulateTick();

            Assert.AreEqual(0, miner.Inventory.GetAmount(ResourceType.IronOre));
            Assert.AreEqual(1, conveyor.Inventory.GetAmount(ResourceType.IronOre));
        }

        [Test]
        public void SimulateTick_MinerOutputsProducedResourceDirectlyToStorage()
        {
            GridModel gridModel = new GridModel(10, 10);
            BuildingModel miner = PlaceBuildingOnResource(gridModel, "miner-1", BuildingType.Miner, ResourceType.IronOre, new GridPosition(1, 1), BuildingDirection.East);
            BuildingModel storage = CreateBuilding("storage-1", BuildingType.Storage, new GridPosition(2, 1));
            gridModel.PlaceBuilding(storage);
            FactorySimulation simulation = new FactorySimulation(gridModel);

            simulation.SimulateTick();

            Assert.AreEqual(0, miner.Inventory.GetAmount(ResourceType.IronOre));
            Assert.AreEqual(1, storage.Inventory.GetAmount(ResourceType.IronOre));
        }

        [Test]
        public void SimulateTick_MinerOutputsProducedResourceDirectlyToSmelter()
        {
            GridModel gridModel = new GridModel(10, 10);
            BuildingModel miner = PlaceBuildingOnResource(gridModel, "miner-1", BuildingType.Miner, ResourceType.IronOre, new GridPosition(1, 1), BuildingDirection.East);
            BuildingModel smelter = CreateBuilding("smelter-1", BuildingType.Smelter, new GridPosition(2, 1));
            gridModel.PlaceBuilding(smelter);
            FactorySimulation simulation = new FactorySimulation(gridModel);

            simulation.SimulateTick();

            Assert.AreEqual(0, miner.Inventory.GetAmount(ResourceType.IronOre));
            Assert.AreEqual(0, smelter.Inventory.GetAmount(ResourceType.IronOre));
            Assert.AreEqual(1, smelter.Inventory.GetAmount(ResourceType.IronIngot));
        }

        [Test]
        public void SimulateTick_MinerKeepsProducedResourceWhenOutputCellIsEmpty()
        {
            GridModel gridModel = new GridModel(10, 10);
            BuildingModel miner = PlaceBuildingOnResource(gridModel, "miner-1", BuildingType.Miner, ResourceType.IronOre, new GridPosition(1, 1), BuildingDirection.East);
            FactorySimulation simulation = new FactorySimulation(gridModel);

            simulation.SimulateTick();

            Assert.AreEqual(1, miner.Inventory.GetAmount(ResourceType.IronOre));
        }

        [Test]
        public void SimulateTick_MinerKeepsProducedResourceWhenOutputIsOutsideGrid()
        {
            GridModel gridModel = new GridModel(10, 10);
            BuildingModel miner = PlaceBuildingOnResource(gridModel, "miner-1", BuildingType.Miner, ResourceType.IronOre, new GridPosition(0, 0), BuildingDirection.West);
            FactorySimulation simulation = new FactorySimulation(gridModel);

            simulation.SimulateTick();

            Assert.AreEqual(1, miner.Inventory.GetAmount(ResourceType.IronOre));
        }

        [Test]
        public void SimulateTick_MinerDoesNotOutputToAnotherMiner()
        {
            GridModel gridModel = new GridModel(10, 10);
            BuildingModel sourceMiner = PlaceBuildingOnResource(gridModel, "miner-1", BuildingType.Miner, ResourceType.IronOre, new GridPosition(1, 1), BuildingDirection.East);
            BuildingModel destinationMiner = CreateBuilding("miner-2", BuildingType.Miner, new GridPosition(2, 1));
            gridModel.PlaceBuilding(destinationMiner);
            FactorySimulation simulation = new FactorySimulation(gridModel);

            simulation.SimulateTick();

            Assert.AreEqual(1, sourceMiner.Inventory.GetAmount(ResourceType.IronOre));
            Assert.AreEqual(0, destinationMiner.Inventory.GetAmount(ResourceType.IronOre));
        }

        [Test]
        public void SimulateTick_ConveyorMovesIronOreToSmelter()
        {
            GridModel gridModel = new GridModel(10, 10);
            BuildingModel conveyor = CreateBuilding("conveyor-1", BuildingType.Conveyor, new GridPosition(1, 1), BuildingDirection.East);
            BuildingModel smelter = CreateBuilding("smelter-1", BuildingType.Smelter, new GridPosition(2, 1));
            gridModel.PlaceBuilding(conveyor);
            gridModel.PlaceBuilding(smelter);
            conveyor.Inventory.Add(ResourceType.IronOre, 1);
            FactorySimulation simulation = new FactorySimulation(gridModel);

            simulation.SimulateTick();

            Assert.AreEqual(0, conveyor.Inventory.GetAmount(ResourceType.IronOre));
            Assert.AreEqual(0, smelter.Inventory.GetAmount(ResourceType.IronOre));
            Assert.AreEqual(1, smelter.Inventory.GetAmount(ResourceType.IronIngot));
        }

        [Test]
        public void SimulateTick_ConveyorMovesCopperOreToSmelter()
        {
            GridModel gridModel = new GridModel(10, 10);
            BuildingModel conveyor = CreateBuilding("conveyor-1", BuildingType.Conveyor, new GridPosition(1, 1), BuildingDirection.East);
            BuildingModel smelter = CreateBuilding("smelter-1", BuildingType.Smelter, new GridPosition(2, 1));
            gridModel.PlaceBuilding(conveyor);
            gridModel.PlaceBuilding(smelter);
            conveyor.Inventory.Add(ResourceType.CopperOre, 1);
            FactorySimulation simulation = new FactorySimulation(gridModel);

            simulation.SimulateTick();

            Assert.AreEqual(0, conveyor.Inventory.GetAmount(ResourceType.CopperOre));
            Assert.AreEqual(0, smelter.Inventory.GetAmount(ResourceType.CopperOre));
            Assert.AreEqual(1, smelter.Inventory.GetAmount(ResourceType.CopperIngot));
        }

        [Test]
        public void SimulateTick_ConveyorDoesNotMoveIronIngotToSmelter()
        {
            GridModel gridModel = new GridModel(10, 10);
            BuildingModel conveyor = CreateBuilding("conveyor-1", BuildingType.Conveyor, new GridPosition(1, 1), BuildingDirection.East);
            BuildingModel smelter = CreateBuilding("smelter-1", BuildingType.Smelter, new GridPosition(2, 1));
            gridModel.PlaceBuilding(conveyor);
            gridModel.PlaceBuilding(smelter);
            conveyor.Inventory.Add(ResourceType.IronIngot, 1);
            FactorySimulation simulation = new FactorySimulation(gridModel);

            simulation.SimulateTick();

            Assert.AreEqual(1, conveyor.Inventory.GetAmount(ResourceType.IronIngot));
            Assert.AreEqual(0, smelter.Inventory.GetAmount(ResourceType.IronIngot));
        }

        [Test]
        public void SimulateTick_ConveyorDoesNotMoveCopperIngotToSmelter()
        {
            GridModel gridModel = new GridModel(10, 10);
            BuildingModel conveyor = CreateBuilding("conveyor-1", BuildingType.Conveyor, new GridPosition(1, 1), BuildingDirection.East);
            BuildingModel smelter = CreateBuilding("smelter-1", BuildingType.Smelter, new GridPosition(2, 1));
            gridModel.PlaceBuilding(conveyor);
            gridModel.PlaceBuilding(smelter);
            conveyor.Inventory.Add(ResourceType.CopperIngot, 1);
            FactorySimulation simulation = new FactorySimulation(gridModel);

            simulation.SimulateTick();

            Assert.AreEqual(1, conveyor.Inventory.GetAmount(ResourceType.CopperIngot));
            Assert.AreEqual(0, smelter.Inventory.GetAmount(ResourceType.CopperIngot));
        }

        [Test]
        public void SimulateTick_SmelterConvertsIronOreToIronIngot()
        {
            GridModel gridModel = new GridModel(10, 10);
            BuildingModel smelter = CreateBuilding("smelter-1", BuildingType.Smelter, new GridPosition(1, 1));
            gridModel.PlaceBuilding(smelter);
            smelter.Inventory.Add(ResourceType.IronOre, 1);
            FactorySimulation simulation = new FactorySimulation(gridModel);

            simulation.SimulateTick();

            Assert.AreEqual(0, smelter.Inventory.GetAmount(ResourceType.IronOre));
            Assert.AreEqual(1, smelter.Inventory.GetAmount(ResourceType.IronIngot));
        }

        [Test]
        public void SimulateTick_SmelterConvertsCopperOreToCopperIngot()
        {
            GridModel gridModel = new GridModel(10, 10);
            BuildingModel smelter = CreateBuilding("smelter-1", BuildingType.Smelter, new GridPosition(1, 1));
            gridModel.PlaceBuilding(smelter);
            smelter.Inventory.Add(ResourceType.CopperOre, 1);
            FactorySimulation simulation = new FactorySimulation(gridModel);

            simulation.SimulateTick();

            Assert.AreEqual(0, smelter.Inventory.GetAmount(ResourceType.CopperOre));
            Assert.AreEqual(1, smelter.Inventory.GetAmount(ResourceType.CopperIngot));
        }

        [Test]
        public void SimulateTick_SmelterProcessesAtMostOneOrePerTick()
        {
            GridModel gridModel = new GridModel(10, 10);
            BuildingModel smelter = CreateBuilding("smelter-1", BuildingType.Smelter, new GridPosition(1, 1));
            gridModel.PlaceBuilding(smelter);
            smelter.Inventory.Add(ResourceType.IronOre, 1);
            smelter.Inventory.Add(ResourceType.CopperOre, 1);
            FactorySimulation simulation = new FactorySimulation(gridModel);

            simulation.SimulateTick();

            Assert.AreEqual(1, smelter.Inventory.GetAmount(ResourceType.IronIngot));
            Assert.AreEqual(0, smelter.Inventory.GetAmount(ResourceType.CopperIngot));
            Assert.AreEqual(1, smelter.Inventory.GetAmount(ResourceType.CopperOre));
        }

        [Test]
        public void SimulateTick_SmelterDoesNothingWhenNoSmeltableOreExists()
        {
            GridModel gridModel = new GridModel(10, 10);
            BuildingModel smelter = CreateBuilding("smelter-1", BuildingType.Smelter, new GridPosition(1, 1));
            gridModel.PlaceBuilding(smelter);
            smelter.Inventory.Add(ResourceType.Coal, 1);
            FactorySimulation simulation = new FactorySimulation(gridModel);

            simulation.SimulateTick();

            Assert.AreEqual(1, smelter.Inventory.GetAmount(ResourceType.Coal));
            Assert.AreEqual(0, smelter.Inventory.GetAmount(ResourceType.IronIngot));
            Assert.AreEqual(0, smelter.Inventory.GetAmount(ResourceType.CopperIngot));
        }

        [Test]
        public void SimulateTick_SmelterOutputsFreshIronIngotToAdjacentStorage()
        {
            GridModel gridModel = new GridModel(10, 10);
            BuildingModel smelter = CreateBuilding("smelter-1", BuildingType.Smelter, new GridPosition(1, 1), BuildingDirection.East);
            BuildingModel storage = CreateBuilding("storage-1", BuildingType.Storage, new GridPosition(2, 1));
            gridModel.PlaceBuilding(smelter);
            gridModel.PlaceBuilding(storage);
            smelter.Inventory.Add(ResourceType.IronOre, 1);
            FactorySimulation simulation = new FactorySimulation(gridModel);

            simulation.SimulateTick();

            Assert.AreEqual(0, smelter.Inventory.GetAmount(ResourceType.IronIngot));
            Assert.AreEqual(1, storage.Inventory.GetAmount(ResourceType.IronIngot));
        }

        [Test]
        public void SimulateTick_SmelterOutputsCopperIngotToAdjacentStorage()
        {
            GridModel gridModel = new GridModel(10, 10);
            BuildingModel smelter = CreateBuilding("smelter-1", BuildingType.Smelter, new GridPosition(1, 1), BuildingDirection.East);
            BuildingModel storage = CreateBuilding("storage-1", BuildingType.Storage, new GridPosition(2, 1));
            gridModel.PlaceBuilding(smelter);
            gridModel.PlaceBuilding(storage);
            smelter.Inventory.Add(ResourceType.CopperIngot, 1);
            FactorySimulation simulation = new FactorySimulation(gridModel);

            simulation.SimulateTick();

            Assert.AreEqual(0, smelter.Inventory.GetAmount(ResourceType.CopperIngot));
            Assert.AreEqual(1, storage.Inventory.GetAmount(ResourceType.CopperIngot));
        }

        [Test]
        public void SimulateTick_SmelterOutputsIngotToAdjacentConveyor()
        {
            GridModel gridModel = new GridModel(10, 10);
            BuildingModel smelter = CreateBuilding("smelter-1", BuildingType.Smelter, new GridPosition(1, 1), BuildingDirection.East);
            BuildingModel conveyor = CreateBuilding("conveyor-1", BuildingType.Conveyor, new GridPosition(2, 1), BuildingDirection.East);
            gridModel.PlaceBuilding(smelter);
            gridModel.PlaceBuilding(conveyor);
            smelter.Inventory.Add(ResourceType.IronIngot, 1);
            FactorySimulation simulation = new FactorySimulation(gridModel);

            simulation.SimulateTick();

            Assert.AreEqual(0, smelter.Inventory.GetAmount(ResourceType.IronIngot));
            Assert.AreEqual(1, conveyor.Inventory.GetAmount(ResourceType.IronIngot));
        }

        [Test]
        public void SimulateTick_SmelterKeepsIngotWhenOutputCellIsEmpty()
        {
            GridModel gridModel = new GridModel(10, 10);
            BuildingModel smelter = CreateBuilding("smelter-1", BuildingType.Smelter, new GridPosition(1, 1), BuildingDirection.East);
            gridModel.PlaceBuilding(smelter);
            smelter.Inventory.Add(ResourceType.IronIngot, 1);
            FactorySimulation simulation = new FactorySimulation(gridModel);

            simulation.SimulateTick();

            Assert.AreEqual(1, smelter.Inventory.GetAmount(ResourceType.IronIngot));
        }

        [Test]
        public void SimulateTick_SmelterKeepsIngotWhenOutputIsOutsideGrid()
        {
            GridModel gridModel = new GridModel(10, 10);
            BuildingModel smelter = CreateBuilding("smelter-1", BuildingType.Smelter, new GridPosition(0, 0), BuildingDirection.West);
            gridModel.PlaceBuilding(smelter);
            smelter.Inventory.Add(ResourceType.IronIngot, 1);
            FactorySimulation simulation = new FactorySimulation(gridModel);

            simulation.SimulateTick();

            Assert.AreEqual(1, smelter.Inventory.GetAmount(ResourceType.IronIngot));
        }

        [Test]
        public void SimulateTick_SmelterDoesNotOutputToMiner()
        {
            GridModel gridModel = new GridModel(10, 10);
            BuildingModel smelter = CreateBuilding("smelter-1", BuildingType.Smelter, new GridPosition(1, 1), BuildingDirection.East);
            BuildingModel miner = CreateBuilding("miner-1", BuildingType.Miner, new GridPosition(2, 1));
            gridModel.PlaceBuilding(smelter);
            gridModel.PlaceBuilding(miner);
            smelter.Inventory.Add(ResourceType.IronIngot, 1);
            FactorySimulation simulation = new FactorySimulation(gridModel);

            simulation.SimulateTick();

            Assert.AreEqual(1, smelter.Inventory.GetAmount(ResourceType.IronIngot));
            Assert.AreEqual(0, miner.Inventory.GetAmount(ResourceType.IronIngot));
        }

        [Test]
        public void SimulateTick_SmelterDoesNotOutputToAnotherSmelter()
        {
            GridModel gridModel = new GridModel(10, 10);
            BuildingModel source = CreateBuilding("smelter-1", BuildingType.Smelter, new GridPosition(1, 1), BuildingDirection.East);
            BuildingModel destination = CreateBuilding("smelter-2", BuildingType.Smelter, new GridPosition(2, 1));
            gridModel.PlaceBuilding(source);
            gridModel.PlaceBuilding(destination);
            source.Inventory.Add(ResourceType.IronIngot, 1);
            FactorySimulation simulation = new FactorySimulation(gridModel);

            simulation.SimulateTick();

            Assert.AreEqual(1, source.Inventory.GetAmount(ResourceType.IronIngot));
            Assert.AreEqual(0, destination.Inventory.GetAmount(ResourceType.IronIngot));
        }

        [Test]
        public void SimulateTick_SmelterOutputsIngotBeforeOre()
        {
            GridModel gridModel = new GridModel(10, 10);
            BuildingModel smelter = CreateBuilding("smelter-1", BuildingType.Smelter, new GridPosition(1, 1), BuildingDirection.East);
            BuildingModel storage = CreateBuilding("storage-1", BuildingType.Storage, new GridPosition(2, 1));
            gridModel.PlaceBuilding(smelter);
            gridModel.PlaceBuilding(storage);
            smelter.Inventory.Add(ResourceType.IronOre, 1);
            smelter.Inventory.Add(ResourceType.CopperIngot, 1);
            FactorySimulation simulation = new FactorySimulation(gridModel);

            simulation.SimulateTick();

            Assert.AreEqual(1, storage.Inventory.GetAmount(ResourceType.IronIngot));
            Assert.AreEqual(0, storage.Inventory.GetAmount(ResourceType.IronOre));
            Assert.AreEqual(1, smelter.Inventory.GetAmount(ResourceType.CopperIngot));
        }

        [Test]
        public void SimulateTick_MinerConveyorStorageLineStoresIronOreAfterMultipleTicks()
        {
            GridModel gridModel = new GridModel(10, 10);
            PlaceBuildingOnResource(gridModel, "miner-1", BuildingType.Miner, ResourceType.IronOre, new GridPosition(0, 0), BuildingDirection.East);
            BuildingModel firstConveyor = CreateBuilding("conveyor-1", BuildingType.Conveyor, new GridPosition(1, 0), BuildingDirection.East);
            BuildingModel secondConveyor = CreateBuilding("conveyor-2", BuildingType.Conveyor, new GridPosition(2, 0), BuildingDirection.East);
            BuildingModel storage = CreateBuilding("storage-1", BuildingType.Storage, new GridPosition(3, 0));
            gridModel.PlaceBuilding(firstConveyor);
            gridModel.PlaceBuilding(secondConveyor);
            gridModel.PlaceBuilding(storage);
            FactorySimulation simulation = new FactorySimulation(gridModel);

            simulation.SimulateTick();
            simulation.SimulateTick();

            Assert.AreEqual(1, storage.Inventory.GetAmount(ResourceType.IronOre));
        }

        [Test]
        public void SimulateTick_MinerConveyorSmelterLineCreatesIronIngotAfterMultipleTicks()
        {
            GridModel gridModel = new GridModel(10, 10);
            PlaceBuildingOnResource(gridModel, "miner-1", BuildingType.Miner, ResourceType.IronOre, new GridPosition(0, 0), BuildingDirection.East);
            BuildingModel firstConveyor = CreateBuilding("conveyor-1", BuildingType.Conveyor, new GridPosition(1, 0), BuildingDirection.East);
            BuildingModel secondConveyor = CreateBuilding("conveyor-2", BuildingType.Conveyor, new GridPosition(2, 0), BuildingDirection.East);
            BuildingModel smelter = CreateBuilding("smelter-1", BuildingType.Smelter, new GridPosition(3, 0));
            gridModel.PlaceBuilding(firstConveyor);
            gridModel.PlaceBuilding(secondConveyor);
            gridModel.PlaceBuilding(smelter);
            FactorySimulation simulation = new FactorySimulation(gridModel);

            simulation.SimulateTick();
            simulation.SimulateTick();

            Assert.AreEqual(1, smelter.Inventory.GetAmount(ResourceType.IronIngot));
        }

        [Test]
        public void GetStoredAmount_ReturnsCombinedAmountAcrossStorageBuildings()
        {
            GridModel gridModel = new GridModel(10, 10);
            BuildingModel firstStorage = CreateBuilding("storage-1", BuildingType.Storage, new GridPosition(1, 1));
            BuildingModel secondStorage = CreateBuilding("storage-2", BuildingType.Storage, new GridPosition(2, 1));
            gridModel.PlaceBuilding(firstStorage);
            gridModel.PlaceBuilding(secondStorage);
            firstStorage.Inventory.Add(ResourceType.IronIngot, 2);
            secondStorage.Inventory.Add(ResourceType.IronIngot, 3);
            FactorySimulation simulation = new FactorySimulation(gridModel);

            Assert.AreEqual(5, simulation.GetStoredAmount(ResourceType.IronIngot));
        }

        [Test]
        public void GetStoredResources_ReturnsOnlyStorageInventoryResources()
        {
            GridModel gridModel = new GridModel(10, 10);
            BuildingModel storage = CreateBuilding("storage-1", BuildingType.Storage, new GridPosition(1, 1));
            BuildingModel conveyor = CreateBuilding("conveyor-1", BuildingType.Conveyor, new GridPosition(2, 1));
            BuildingModel smelter = CreateBuilding("smelter-1", BuildingType.Smelter, new GridPosition(3, 1));
            gridModel.PlaceBuilding(storage);
            gridModel.PlaceBuilding(conveyor);
            gridModel.PlaceBuilding(smelter);
            storage.Inventory.Add(ResourceType.IronIngot, 2);
            conveyor.Inventory.Add(ResourceType.CopperIngot, 5);
            smelter.Inventory.Add(ResourceType.IronOre, 7);
            FactorySimulation simulation = new FactorySimulation(gridModel);

            var storedResources = simulation.GetStoredResources();

            Assert.AreEqual(1, storedResources.Count);
            Assert.AreEqual(2, storedResources[ResourceType.IronIngot]);
            Assert.IsFalse(storedResources.ContainsKey(ResourceType.CopperIngot));
            Assert.IsFalse(storedResources.ContainsKey(ResourceType.IronOre));
            Assert.IsFalse(storedResources.ContainsKey(ResourceType.None));
        }

        [Test]
        public void GetStoredResources_ExcludesRemovedZeroAmountResources()
        {
            GridModel gridModel = new GridModel(10, 10);
            BuildingModel storage = CreateBuilding("storage-1", BuildingType.Storage, new GridPosition(1, 1));
            gridModel.PlaceBuilding(storage);
            storage.Inventory.Add(ResourceType.IronIngot, 1);
            storage.Inventory.TryRemove(ResourceType.IronIngot, 1);
            FactorySimulation simulation = new FactorySimulation(gridModel);

            var storedResources = simulation.GetStoredResources();

            Assert.AreEqual(0, storedResources.Count);
            Assert.AreEqual(0, simulation.GetStoredAmount(ResourceType.None));
        }

        [Test]
        public void SimulateTick_MinerConveyorSmelterStorageLineStoresIronIngotAfterMultipleTicks()
        {
            GridModel gridModel = new GridModel(10, 10);
            PlaceBuildingOnResource(gridModel, "miner-1", BuildingType.Miner, ResourceType.IronOre, new GridPosition(0, 0), BuildingDirection.East);
            BuildingModel conveyor = CreateBuilding("conveyor-1", BuildingType.Conveyor, new GridPosition(1, 0), BuildingDirection.East);
            BuildingModel smelter = CreateBuilding("smelter-1", BuildingType.Smelter, new GridPosition(2, 0), BuildingDirection.East);
            BuildingModel storage = CreateBuilding("storage-1", BuildingType.Storage, new GridPosition(3, 0));
            gridModel.PlaceBuilding(conveyor);
            gridModel.PlaceBuilding(smelter);
            gridModel.PlaceBuilding(storage);
            FactorySimulation simulation = new FactorySimulation(gridModel);

            simulation.SimulateTick();
            simulation.SimulateTick();

            Assert.AreEqual(2, simulation.GetStoredAmount(ResourceType.IronIngot));
        }

        [Test]
        public void SimulateTick_MinerConveyorSmelterConveyorStorageLineStoresIronIngotAfterMultipleTicks()
        {
            GridModel gridModel = new GridModel(10, 10);
            PlaceBuildingOnResource(gridModel, "miner-1", BuildingType.Miner, ResourceType.IronOre, new GridPosition(0, 0), BuildingDirection.East);
            BuildingModel inputConveyor = CreateBuilding("conveyor-1", BuildingType.Conveyor, new GridPosition(1, 0), BuildingDirection.East);
            BuildingModel smelter = CreateBuilding("smelter-1", BuildingType.Smelter, new GridPosition(2, 0), BuildingDirection.East);
            BuildingModel outputConveyor = CreateBuilding("conveyor-2", BuildingType.Conveyor, new GridPosition(3, 0), BuildingDirection.East);
            BuildingModel storage = CreateBuilding("storage-1", BuildingType.Storage, new GridPosition(4, 0));
            gridModel.PlaceBuilding(inputConveyor);
            gridModel.PlaceBuilding(smelter);
            gridModel.PlaceBuilding(outputConveyor);
            gridModel.PlaceBuilding(storage);
            FactorySimulation simulation = new FactorySimulation(gridModel);

            simulation.SimulateTick();
            simulation.SimulateTick();
            simulation.SimulateTick();

            Assert.AreEqual(2, simulation.GetStoredAmount(ResourceType.IronIngot));
        }

        [Test]
        public void SimulateTick_CopperOreLineStoresCopperIngotAfterMultipleTicks()
        {
            GridModel gridModel = new GridModel(10, 10);
            PlaceBuildingOnResource(gridModel, "miner-1", BuildingType.Miner, ResourceType.CopperOre, new GridPosition(0, 0), BuildingDirection.East);
            BuildingModel conveyor = CreateBuilding("conveyor-1", BuildingType.Conveyor, new GridPosition(1, 0), BuildingDirection.East);
            BuildingModel smelter = CreateBuilding("smelter-1", BuildingType.Smelter, new GridPosition(2, 0), BuildingDirection.East);
            BuildingModel storage = CreateBuilding("storage-1", BuildingType.Storage, new GridPosition(3, 0));
            gridModel.PlaceBuilding(conveyor);
            gridModel.PlaceBuilding(smelter);
            gridModel.PlaceBuilding(storage);
            FactorySimulation simulation = new FactorySimulation(gridModel);

            simulation.SimulateTick();
            simulation.SimulateTick();

            Assert.AreEqual(2, simulation.GetStoredAmount(ResourceType.CopperIngot));
        }

        [Test]
        public void SimulateTick_ConveyorMovesIronIngotToAssembler()
        {
            GridModel gridModel = new GridModel(10, 10);
            BuildingModel conveyor = CreateBuilding("conveyor-1", BuildingType.Conveyor, new GridPosition(1, 1), BuildingDirection.East);
            BuildingModel assembler = CreateAssembler("assembler-1", new GridPosition(2, 1), RecipeCatalog.IronPlateRecipeId);
            gridModel.PlaceBuilding(conveyor);
            gridModel.PlaceBuilding(assembler);
            conveyor.Inventory.Add(ResourceType.IronIngot, 1);
            FactorySimulation simulation = new FactorySimulation(gridModel);

            simulation.SimulateTick();

            Assert.AreEqual(0, conveyor.Inventory.GetAmount(ResourceType.IronIngot));
            Assert.AreEqual(0, assembler.Inventory.GetAmount(ResourceType.IronIngot));
            Assert.AreEqual(1, assembler.Inventory.GetAmount(ResourceType.IronPlate));
        }

        [Test]
        public void SimulateTick_ConveyorMovesCopperIngotToAssembler()
        {
            GridModel gridModel = new GridModel(10, 10);
            BuildingModel conveyor = CreateBuilding("conveyor-1", BuildingType.Conveyor, new GridPosition(1, 1), BuildingDirection.East);
            BuildingModel assembler = CreateAssembler("assembler-1", new GridPosition(2, 1), RecipeCatalog.CopperWireRecipeId);
            gridModel.PlaceBuilding(conveyor);
            gridModel.PlaceBuilding(assembler);
            conveyor.Inventory.Add(ResourceType.CopperIngot, 1);
            FactorySimulation simulation = new FactorySimulation(gridModel);

            simulation.SimulateTick();

            Assert.AreEqual(2, assembler.Inventory.GetAmount(ResourceType.CopperWire));
        }

        [Test]
        public void SimulateTick_ConveyorMovesIronPlateToAssembler()
        {
            GridModel gridModel = new GridModel(10, 10);
            BuildingModel conveyor = CreateBuilding("conveyor-1", BuildingType.Conveyor, new GridPosition(1, 1), BuildingDirection.East);
            BuildingModel assembler = CreateAssembler("assembler-1", new GridPosition(2, 1), RecipeCatalog.GearRecipeId);
            gridModel.PlaceBuilding(conveyor);
            gridModel.PlaceBuilding(assembler);
            conveyor.Inventory.Add(ResourceType.IronPlate, 1);
            FactorySimulation simulation = new FactorySimulation(gridModel);

            simulation.SimulateTick();

            Assert.AreEqual(1, assembler.Inventory.GetAmount(ResourceType.IronPlate));
        }

        [Test]
        public void SimulateTick_AssemblerProducesIronPlate()
        {
            BuildingModel assembler = SimulateAssemblerProduction(RecipeCatalog.IronPlateRecipeId, ResourceType.IronIngot, 1);

            Assert.AreEqual(1, assembler.Inventory.GetAmount(ResourceType.IronPlate));
        }

        [Test]
        public void SimulateTick_AssemblerProducesCopperWire()
        {
            BuildingModel assembler = SimulateAssemblerProduction(RecipeCatalog.CopperWireRecipeId, ResourceType.CopperIngot, 1);

            Assert.AreEqual(2, assembler.Inventory.GetAmount(ResourceType.CopperWire));
        }

        [Test]
        public void SimulateTick_AssemblerProducesGear()
        {
            BuildingModel assembler = SimulateAssemblerProduction(RecipeCatalog.GearRecipeId, ResourceType.IronPlate, 2);

            Assert.AreEqual(1, assembler.Inventory.GetAmount(ResourceType.Gear));
        }

        [Test]
        public void SimulateTick_AssemblerProducesBasicCircuit()
        {
            GridModel gridModel = new GridModel(10, 10);
            BuildingModel assembler = CreateAssembler("assembler-1", new GridPosition(1, 1), RecipeCatalog.BasicCircuitRecipeId);
            gridModel.PlaceBuilding(assembler);
            assembler.Inventory.Add(ResourceType.IronPlate, 1);
            assembler.Inventory.Add(ResourceType.CopperWire, 2);
            FactorySimulation simulation = new FactorySimulation(gridModel);

            simulation.SimulateTick();

            Assert.AreEqual(1, assembler.Inventory.GetAmount(ResourceType.BasicCircuit));
        }

        [Test]
        public void SimulateTick_AssemblerDoesNotProduceWhenInputIsMissing()
        {
            BuildingModel assembler = SimulateAssemblerProduction(RecipeCatalog.GearRecipeId, ResourceType.IronPlate, 1);

            Assert.AreEqual(0, assembler.Inventory.GetAmount(ResourceType.Gear));
            Assert.AreEqual(1, assembler.Inventory.GetAmount(ResourceType.IronPlate));
        }

        [Test]
        public void SimulateTick_AssemblerDoesNotProduceWithoutSelectedRecipe()
        {
            BuildingModel assembler = SimulateAssemblerProduction(null, ResourceType.IronIngot, 1);

            Assert.AreEqual(0, assembler.Inventory.GetAmount(ResourceType.IronPlate));
            Assert.AreEqual(1, assembler.Inventory.GetAmount(ResourceType.IronIngot));
        }

        [Test]
        public void SimulateTick_AssemblerDoesNotProduceWithUnknownRecipe()
        {
            BuildingModel assembler = SimulateAssemblerProduction("missing", ResourceType.IronIngot, 1);

            Assert.AreEqual(0, assembler.Inventory.GetAmount(ResourceType.IronPlate));
            Assert.AreEqual(1, assembler.Inventory.GetAmount(ResourceType.IronIngot));
        }

        [Test]
        public void SimulateTick_AssemblerProducesAtMostOncePerTick()
        {
            BuildingModel assembler = SimulateAssemblerProduction(RecipeCatalog.IronPlateRecipeId, ResourceType.IronIngot, 2);

            Assert.AreEqual(1, assembler.Inventory.GetAmount(ResourceType.IronPlate));
            Assert.AreEqual(1, assembler.Inventory.GetAmount(ResourceType.IronIngot));
        }

        [Test]
        public void SimulateTick_AssemblerOutputsIronPlateToStorage()
        {
            GridModel gridModel = new GridModel(10, 10);
            BuildingModel assembler = CreateAssembler("assembler-1", new GridPosition(1, 1), RecipeCatalog.IronPlateRecipeId, BuildingDirection.East);
            BuildingModel storage = CreateBuilding("storage-1", BuildingType.Storage, new GridPosition(2, 1));
            gridModel.PlaceBuilding(assembler);
            gridModel.PlaceBuilding(storage);
            assembler.Inventory.Add(ResourceType.IronPlate, 1);
            FactorySimulation simulation = new FactorySimulation(gridModel);

            simulation.SimulateTick();

            Assert.AreEqual(1, storage.Inventory.GetAmount(ResourceType.IronPlate));
        }

        [Test]
        public void SimulateTick_AssemblerOutputsIronPlateToConveyor()
        {
            GridModel gridModel = new GridModel(10, 10);
            BuildingModel assembler = CreateAssembler("assembler-1", new GridPosition(1, 1), RecipeCatalog.IronPlateRecipeId, BuildingDirection.East);
            BuildingModel conveyor = CreateBuilding("conveyor-1", BuildingType.Conveyor, new GridPosition(2, 1));
            gridModel.PlaceBuilding(assembler);
            gridModel.PlaceBuilding(conveyor);
            assembler.Inventory.Add(ResourceType.IronPlate, 1);
            FactorySimulation simulation = new FactorySimulation(gridModel);

            simulation.SimulateTick();

            Assert.AreEqual(1, conveyor.Inventory.GetAmount(ResourceType.IronPlate));
        }

        [Test]
        public void SimulateTick_AssemblerKeepsOutputWhenFrontCellIsEmpty()
        {
            BuildingModel assembler = SimulateAssemblerOutputFailure(new GridPosition(1, 1), BuildingDirection.East, null);

            Assert.AreEqual(1, assembler.Inventory.GetAmount(ResourceType.IronPlate));
        }

        [Test]
        public void SimulateTick_AssemblerKeepsOutputWhenFrontCellIsOutsideGrid()
        {
            BuildingModel assembler = SimulateAssemblerOutputFailure(new GridPosition(0, 0), BuildingDirection.West, null);

            Assert.AreEqual(1, assembler.Inventory.GetAmount(ResourceType.IronPlate));
        }

        [TestCase(BuildingType.Miner)]
        [TestCase(BuildingType.Smelter)]
        [TestCase(BuildingType.Assembler)]
        public void SimulateTick_AssemblerDoesNotOutputToBlockedBuildingTypes(BuildingType blockedType)
        {
            BuildingModel assembler = SimulateAssemblerOutputFailure(
                new GridPosition(1, 1),
                BuildingDirection.East,
                CreateBuilding("blocked-1", blockedType, new GridPosition(2, 1)));

            Assert.AreEqual(1, assembler.Inventory.GetAmount(ResourceType.IronPlate));
        }

        [Test]
        public void SimulateTick_IronPlateLineStoresIronPlateAfterMultipleTicks()
        {
            GridModel gridModel = new GridModel(10, 10);
            PlaceBuildingOnResource(gridModel, "miner-1", BuildingType.Miner, ResourceType.IronOre, new GridPosition(0, 0), BuildingDirection.East);
            gridModel.PlaceBuilding(CreateBuilding("conveyor-1", BuildingType.Conveyor, new GridPosition(1, 0), BuildingDirection.East));
            BuildingModel smelter = CreateBuilding("smelter-1", BuildingType.Smelter, new GridPosition(2, 0), BuildingDirection.East);
            gridModel.PlaceBuilding(smelter);
            BuildingModel assembler = CreateAssembler("assembler-1", new GridPosition(3, 0), RecipeCatalog.IronPlateRecipeId, BuildingDirection.East);
            BuildingModel storage = CreateBuilding("storage-1", BuildingType.Storage, new GridPosition(4, 0));
            gridModel.PlaceBuilding(assembler);
            gridModel.PlaceBuilding(storage);
            FactorySimulation simulation = new FactorySimulation(gridModel);

            simulation.SimulateTick();
            simulation.SimulateTick();

            Assert.AreEqual(2, simulation.GetStoredAmount(ResourceType.IronPlate));
        }

        [Test]
        public void SimulateTick_CopperWireLineStoresCopperWireAfterMultipleTicks()
        {
            GridModel gridModel = new GridModel(10, 10);
            PlaceBuildingOnResource(gridModel, "miner-1", BuildingType.Miner, ResourceType.CopperOre, new GridPosition(0, 0), BuildingDirection.East);
            gridModel.PlaceBuilding(CreateBuilding("conveyor-1", BuildingType.Conveyor, new GridPosition(1, 0), BuildingDirection.East));
            BuildingModel smelter = CreateBuilding("smelter-1", BuildingType.Smelter, new GridPosition(2, 0), BuildingDirection.East);
            gridModel.PlaceBuilding(smelter);
            BuildingModel assembler = CreateAssembler("assembler-1", new GridPosition(3, 0), RecipeCatalog.CopperWireRecipeId, BuildingDirection.East);
            BuildingModel storage = CreateBuilding("storage-1", BuildingType.Storage, new GridPosition(4, 0));
            gridModel.PlaceBuilding(assembler);
            gridModel.PlaceBuilding(storage);
            FactorySimulation simulation = new FactorySimulation(gridModel);

            simulation.SimulateTick();
            simulation.SimulateTick();

            Assert.AreEqual(2, simulation.GetStoredAmount(ResourceType.CopperWire));
        }

        [Test]
        public void SimulateTick_GearRecipeStoresGear()
        {
            GridModel gridModel = new GridModel(10, 10);
            BuildingModel assembler = CreateAssembler("assembler-1", new GridPosition(1, 1), RecipeCatalog.GearRecipeId, BuildingDirection.East);
            BuildingModel storage = CreateBuilding("storage-1", BuildingType.Storage, new GridPosition(2, 1));
            gridModel.PlaceBuilding(assembler);
            gridModel.PlaceBuilding(storage);
            assembler.Inventory.Add(ResourceType.IronPlate, 2);
            FactorySimulation simulation = new FactorySimulation(gridModel);

            simulation.SimulateTick();

            Assert.AreEqual(1, simulation.GetStoredAmount(ResourceType.Gear));
        }

        [Test]
        public void SimulateTick_BasicCircuitRecipeStoresBasicCircuit()
        {
            GridModel gridModel = new GridModel(10, 10);
            BuildingModel assembler = CreateAssembler("assembler-1", new GridPosition(1, 1), RecipeCatalog.BasicCircuitRecipeId, BuildingDirection.East);
            BuildingModel storage = CreateBuilding("storage-1", BuildingType.Storage, new GridPosition(2, 1));
            gridModel.PlaceBuilding(assembler);
            gridModel.PlaceBuilding(storage);
            assembler.Inventory.Add(ResourceType.IronPlate, 1);
            assembler.Inventory.Add(ResourceType.CopperWire, 2);
            FactorySimulation simulation = new FactorySimulation(gridModel);

            simulation.SimulateTick();

            Assert.AreEqual(1, simulation.GetStoredAmount(ResourceType.BasicCircuit));
        }

        [Test]
        public void CalculatePower_IncludesGeneratorProduction()
        {
            GridModel gridModel = new GridModel(10, 10);
            gridModel.PlaceBuilding(CreatePoweredBuilding("generator-1", BuildingType.Generator, new GridPosition(0, 0)));
            FactorySimulation simulation = new FactorySimulation(gridModel);

            Assert.AreEqual(10, simulation.CalculatePower().ProducedPower);
        }

        [Test]
        public void CalculatePower_IncludesMinerConsumption()
        {
            GridModel gridModel = new GridModel(10, 10);
            gridModel.PlaceBuilding(CreatePoweredBuilding("miner-1", BuildingType.Miner, new GridPosition(0, 0)));
            FactorySimulation simulation = new FactorySimulation(gridModel);

            Assert.AreEqual(2, simulation.CalculatePower().ConsumedPower);
        }

        [Test]
        public void HasEnoughPower_ReturnsTrueWhenGeneratorCanPowerMiner()
        {
            GridModel gridModel = new GridModel(10, 10);
            gridModel.PlaceBuilding(CreatePoweredBuilding("generator-1", BuildingType.Generator, new GridPosition(0, 0)));
            gridModel.PlaceBuilding(CreatePoweredBuilding("miner-1", BuildingType.Miner, new GridPosition(1, 0)));
            FactorySimulation simulation = new FactorySimulation(gridModel);

            Assert.IsTrue(simulation.HasEnoughPower());
        }

        [Test]
        public void HasEnoughPower_ReturnsFalseWhenMinerHasNoGenerator()
        {
            GridModel gridModel = new GridModel(10, 10);
            gridModel.PlaceBuilding(CreatePoweredBuilding("miner-1", BuildingType.Miner, new GridPosition(0, 0)));
            FactorySimulation simulation = new FactorySimulation(gridModel);

            Assert.IsFalse(simulation.HasEnoughPower());
        }

        [Test]
        public void HasEnoughPower_ReturnsFalseWhenConsumptionExceedsProduction()
        {
            GridModel gridModel = new GridModel(10, 10);
            gridModel.PlaceBuilding(CreatePoweredBuilding("generator-1", BuildingType.Generator, new GridPosition(0, 0)));
            gridModel.PlaceBuilding(CreatePoweredBuilding("assembler-1", BuildingType.Assembler, new GridPosition(1, 0)));
            gridModel.PlaceBuilding(CreatePoweredBuilding("assembler-2", BuildingType.Assembler, new GridPosition(2, 0)));
            gridModel.PlaceBuilding(CreatePoweredBuilding("miner-1", BuildingType.Miner, new GridPosition(3, 0)));
            FactorySimulation simulation = new FactorySimulation(gridModel);

            Assert.IsFalse(simulation.HasEnoughPower());
        }

        [Test]
        public void CalculatePower_UpdatesAfterBuildingRemoval()
        {
            GridModel gridModel = new GridModel(10, 10);
            gridModel.PlaceBuilding(CreatePoweredBuilding("generator-1", BuildingType.Generator, new GridPosition(0, 0)));
            gridModel.PlaceBuilding(CreatePoweredBuilding("miner-1", BuildingType.Miner, new GridPosition(1, 0)));
            FactorySimulation simulation = new FactorySimulation(gridModel);

            gridModel.RemoveBuilding("miner-1");

            Assert.AreEqual(0, simulation.CalculatePower().ConsumedPower);
            Assert.IsTrue(simulation.HasEnoughPower());
        }

        [Test]
        public void SimulateTick_WithEnoughPower_MinerProducesResource()
        {
            GridModel gridModel = new GridModel(10, 10);
            gridModel.PlaceBuilding(CreatePoweredBuilding("generator-1", BuildingType.Generator, new GridPosition(0, 0)));
            BuildingModel miner = PlacePoweredBuildingOnResource(gridModel, "miner-1", ResourceType.IronOre, new GridPosition(1, 0), BuildingDirection.East);
            FactorySimulation simulation = new FactorySimulation(gridModel);

            simulation.SimulateTick();

            Assert.AreEqual(1, miner.Inventory.GetAmount(ResourceType.IronOre));
        }

        [Test]
        public void SimulateTick_WithoutEnoughPower_MinerDoesNotProduceResource()
        {
            GridModel gridModel = new GridModel(10, 10);
            BuildingModel miner = PlacePoweredBuildingOnResource(gridModel, "miner-1", ResourceType.IronOre, new GridPosition(1, 0), BuildingDirection.East);
            FactorySimulation simulation = new FactorySimulation(gridModel);

            simulation.SimulateTick();

            Assert.AreEqual(0, miner.Inventory.GetAmount(ResourceType.IronOre));
        }

        [Test]
        public void SimulateTick_WithEnoughPower_SmelterProcessesIronOre()
        {
            GridModel gridModel = new GridModel(10, 10);
            gridModel.PlaceBuilding(CreatePoweredBuilding("generator-1", BuildingType.Generator, new GridPosition(0, 0)));
            BuildingModel smelter = CreatePoweredBuilding("smelter-1", BuildingType.Smelter, new GridPosition(1, 0));
            gridModel.PlaceBuilding(smelter);
            smelter.Inventory.Add(ResourceType.IronOre, 1);
            FactorySimulation simulation = new FactorySimulation(gridModel);

            simulation.SimulateTick();

            Assert.AreEqual(1, smelter.Inventory.GetAmount(ResourceType.IronIngot));
        }

        [Test]
        public void SimulateTick_WithoutEnoughPower_SmelterDoesNotProcessIronOre()
        {
            GridModel gridModel = new GridModel(10, 10);
            BuildingModel smelter = CreatePoweredBuilding("smelter-1", BuildingType.Smelter, new GridPosition(1, 0));
            gridModel.PlaceBuilding(smelter);
            smelter.Inventory.Add(ResourceType.IronOre, 1);
            FactorySimulation simulation = new FactorySimulation(gridModel);

            simulation.SimulateTick();

            Assert.AreEqual(1, smelter.Inventory.GetAmount(ResourceType.IronOre));
            Assert.AreEqual(0, smelter.Inventory.GetAmount(ResourceType.IronIngot));
        }

        [Test]
        public void SimulateTick_WithEnoughPower_AssemblerProducesIronPlate()
        {
            GridModel gridModel = new GridModel(10, 10);
            gridModel.PlaceBuilding(CreatePoweredBuilding("generator-1", BuildingType.Generator, new GridPosition(0, 0)));
            BuildingModel assembler = CreatePoweredAssembler("assembler-1", new GridPosition(1, 0), RecipeCatalog.IronPlateRecipeId, BuildingDirection.North);
            gridModel.PlaceBuilding(assembler);
            assembler.Inventory.Add(ResourceType.IronIngot, 1);
            FactorySimulation simulation = new FactorySimulation(gridModel);

            simulation.SimulateTick();

            Assert.AreEqual(1, assembler.Inventory.GetAmount(ResourceType.IronPlate));
        }

        [Test]
        public void SimulateTick_WithoutEnoughPower_AssemblerDoesNotProduceIronPlate()
        {
            GridModel gridModel = new GridModel(10, 10);
            BuildingModel assembler = CreatePoweredAssembler("assembler-1", new GridPosition(1, 0), RecipeCatalog.IronPlateRecipeId, BuildingDirection.North);
            gridModel.PlaceBuilding(assembler);
            assembler.Inventory.Add(ResourceType.IronIngot, 1);
            FactorySimulation simulation = new FactorySimulation(gridModel);

            simulation.SimulateTick();

            Assert.AreEqual(1, assembler.Inventory.GetAmount(ResourceType.IronIngot));
            Assert.AreEqual(0, assembler.Inventory.GetAmount(ResourceType.IronPlate));
        }

        [Test]
        public void SimulateTick_WithoutEnoughPower_ConveyorStillMovesExistingResources()
        {
            GridModel gridModel = new GridModel(10, 10);
            gridModel.PlaceBuilding(CreatePoweredBuilding("miner-1", BuildingType.Miner, new GridPosition(0, 0)));
            BuildingModel conveyor = CreatePoweredBuilding("conveyor-1", BuildingType.Conveyor, new GridPosition(1, 0), BuildingDirection.East);
            BuildingModel storage = CreatePoweredBuilding("storage-1", BuildingType.Storage, new GridPosition(2, 0));
            gridModel.PlaceBuilding(conveyor);
            gridModel.PlaceBuilding(storage);
            conveyor.Inventory.Add(ResourceType.IronOre, 1);
            FactorySimulation simulation = new FactorySimulation(gridModel);

            simulation.SimulateTick();

            Assert.AreEqual(1, storage.Inventory.GetAmount(ResourceType.IronOre));
        }

        [Test]
        public void SimulateTick_WithEnoughPower_FullIronPlateLineRuns()
        {
            GridModel gridModel = CreatePoweredIronPlateLine(includeGenerator: true);
            FactorySimulation simulation = new FactorySimulation(gridModel);

            simulation.SimulateTick();
            simulation.SimulateTick();

            Assert.AreEqual(2, simulation.GetStoredAmount(ResourceType.IronPlate));
        }

        [Test]
        public void SimulateTick_WithoutGenerator_FullIronPlateLineStops()
        {
            GridModel gridModel = CreatePoweredIronPlateLine(includeGenerator: false);
            FactorySimulation simulation = new FactorySimulation(gridModel);

            simulation.SimulateTick();
            simulation.SimulateTick();

            Assert.AreEqual(0, simulation.GetStoredAmount(ResourceType.IronPlate));
        }

        [Test]
        public void SimulateTick_AddingGeneratorRestartsStoppedLine()
        {
            GridModel gridModel = CreatePoweredIronPlateLine(includeGenerator: false);
            FactorySimulation simulation = new FactorySimulation(gridModel);

            simulation.SimulateTick();
            gridModel.PlaceBuilding(CreatePoweredBuilding("generator-1", BuildingType.Generator, new GridPosition(5, 0)));
            gridModel.PlaceBuilding(CreatePoweredBuilding("generator-2", BuildingType.Generator, new GridPosition(6, 0)));
            simulation.SimulateTick();

            Assert.AreEqual(1, simulation.GetStoredAmount(ResourceType.IronPlate));
        }

        private static BuildingModel PlaceBuildingOnResource(
            GridModel gridModel,
            string instanceId,
            BuildingType buildingType,
            ResourceType resourceType)
        {
            return PlaceBuildingOnResource(
                gridModel,
                instanceId,
                buildingType,
                resourceType,
                new GridPosition(1, 1),
                BuildingDirection.North);
        }

        private static BuildingModel PlaceBuildingOnResource(
            GridModel gridModel,
            string instanceId,
            BuildingType buildingType,
            ResourceType resourceType,
            GridPosition position,
            BuildingDirection direction)
        {
            gridModel.SetResourceNode(position, resourceType);
            BuildingModel building = CreateBuilding(instanceId, buildingType, position, direction);
            gridModel.PlaceBuilding(building);
            return building;
        }

        private static BuildingModel CreateBuilding(string instanceId, BuildingType buildingType, GridPosition origin)
        {
            return CreateBuilding(instanceId, buildingType, origin, BuildingDirection.North);
        }

        private static BuildingModel CreateBuilding(
            string instanceId,
            BuildingType buildingType,
            GridPosition origin,
            BuildingDirection direction)
        {
            BuildingDefinition definition = new BuildingDefinition(
                "test-building",
                buildingType,
                "Test Building",
                1,
                1,
                false,
                ResourceType.None,
                true);

            return new BuildingModel(instanceId, definition, origin, direction);
        }

        private static BuildingModel CreateAssembler(string instanceId, GridPosition origin, string recipeId)
        {
            return CreateAssembler(instanceId, origin, recipeId, BuildingDirection.North);
        }

        private static BuildingModel CreateAssembler(
            string instanceId,
            GridPosition origin,
            string recipeId,
            BuildingDirection direction)
        {
            BuildingModel assembler = CreateBuilding(instanceId, BuildingType.Assembler, origin, direction);
            assembler.SelectedRecipeId = recipeId;
            return assembler;
        }

        private static BuildingModel CreatePoweredAssembler(
            string instanceId,
            GridPosition origin,
            string recipeId,
            BuildingDirection direction)
        {
            BuildingModel assembler = CreatePoweredBuilding(instanceId, BuildingType.Assembler, origin, direction);
            assembler.SelectedRecipeId = recipeId;
            return assembler;
        }

        private static BuildingModel CreatePoweredBuilding(string instanceId, BuildingType buildingType, GridPosition origin)
        {
            return CreatePoweredBuilding(instanceId, buildingType, origin, BuildingDirection.North);
        }

        private static BuildingModel CreatePoweredBuilding(
            string instanceId,
            BuildingType buildingType,
            GridPosition origin,
            BuildingDirection direction)
        {
            BuildingDefinition definition = new BuildingDefinition(
                "powered-" + buildingType,
                buildingType,
                "Powered " + buildingType,
                1,
                1,
                false,
                ResourceType.None,
                true,
                GetDefaultPowerProduction(buildingType),
                GetDefaultPowerConsumption(buildingType));

            return new BuildingModel(instanceId, definition, origin, direction);
        }

        private static BuildingModel PlacePoweredBuildingOnResource(
            GridModel gridModel,
            string instanceId,
            ResourceType resourceType,
            GridPosition position,
            BuildingDirection direction)
        {
            gridModel.SetResourceNode(position, resourceType);
            BuildingModel building = CreatePoweredBuilding(instanceId, BuildingType.Miner, position, direction);
            gridModel.PlaceBuilding(building);
            return building;
        }

        private static GridModel CreatePoweredIronPlateLine(bool includeGenerator)
        {
            GridModel gridModel = new GridModel(10, 10);

            if (includeGenerator)
            {
                gridModel.PlaceBuilding(CreatePoweredBuilding("generator-1", BuildingType.Generator, new GridPosition(5, 0)));
                gridModel.PlaceBuilding(CreatePoweredBuilding("generator-2", BuildingType.Generator, new GridPosition(6, 0)));
            }

            PlacePoweredBuildingOnResource(gridModel, "miner-1", ResourceType.IronOre, new GridPosition(0, 0), BuildingDirection.East);
            gridModel.PlaceBuilding(CreatePoweredBuilding("conveyor-1", BuildingType.Conveyor, new GridPosition(1, 0), BuildingDirection.East));
            gridModel.PlaceBuilding(CreatePoweredBuilding("smelter-1", BuildingType.Smelter, new GridPosition(2, 0), BuildingDirection.East));
            gridModel.PlaceBuilding(CreatePoweredAssembler("assembler-1", new GridPosition(3, 0), RecipeCatalog.IronPlateRecipeId, BuildingDirection.East));
            gridModel.PlaceBuilding(CreatePoweredBuilding("storage-1", BuildingType.Storage, new GridPosition(4, 0)));

            return gridModel;
        }

        private static int GetDefaultPowerProduction(BuildingType buildingType)
        {
            return buildingType == BuildingType.Generator ? 10 : 0;
        }

        private static int GetDefaultPowerConsumption(BuildingType buildingType)
        {
            switch (buildingType)
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

        private static BuildingModel SimulateAssemblerProduction(string recipeId, ResourceType inputType, int inputAmount)
        {
            GridModel gridModel = new GridModel(10, 10);
            BuildingModel assembler = CreateAssembler("assembler-1", new GridPosition(1, 1), recipeId);
            gridModel.PlaceBuilding(assembler);
            assembler.Inventory.Add(inputType, inputAmount);
            FactorySimulation simulation = new FactorySimulation(gridModel);

            simulation.SimulateTick();

            return assembler;
        }

        private static BuildingModel SimulateAssemblerOutputFailure(
            GridPosition assemblerPosition,
            BuildingDirection direction,
            BuildingModel destination)
        {
            GridModel gridModel = new GridModel(10, 10);
            BuildingModel assembler = CreateAssembler("assembler-1", assemblerPosition, RecipeCatalog.IronPlateRecipeId, direction);
            gridModel.PlaceBuilding(assembler);

            if (destination != null)
            {
                gridModel.PlaceBuilding(destination);
            }

            assembler.Inventory.Add(ResourceType.IronPlate, 1);
            FactorySimulation simulation = new FactorySimulation(gridModel);

            simulation.SimulateTick();

            return assembler;
        }
    }
}
