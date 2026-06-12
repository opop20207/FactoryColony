using FactoryColony;
using NUnit.Framework;

namespace FactoryColony.Tests.EditMode.Power
{
    public sealed class PowerStatusServiceTests
    {
        [Test]
        public void IsPowerProducer_ReturnsTrueForGenerator()
        {
            GridModel gridModel = new GridModel(5, 5);
            BuildingModel generator = CreateBuilding("generator", BuildingType.Generator, new GridPosition(0, 0));
            Assert.IsTrue(gridModel.TryPlaceBuilding(generator));
            PowerStatusService service = CreateService(gridModel);

            Assert.IsTrue(service.IsPowerProducer(generator));
            Assert.IsTrue(service.IsPoweredBuilding(generator));
        }

        [Test]
        public void IsPowerConsumer_ReturnsTrueForMiner()
        {
            GridModel gridModel = new GridModel(5, 5);
            gridModel.SetResourceNode(new GridPosition(1, 1), ResourceType.IronOre);
            BuildingModel miner = CreateBuilding("miner", BuildingType.Miner, new GridPosition(1, 1));
            Assert.IsTrue(gridModel.TryPlaceBuilding(miner));
            PowerStatusService service = CreateService(gridModel);

            Assert.IsTrue(service.IsPowerConsumer(miner));
            Assert.IsTrue(service.IsPoweredBuilding(miner));
        }

        [Test]
        public void GetStatusFor_ReturnsOperatingWhenPowerIsSufficient()
        {
            GridModel gridModel = new GridModel(5, 5);
            BuildingModel generator = CreateBuilding("generator", BuildingType.Generator, new GridPosition(0, 0));
            gridModel.SetResourceNode(new GridPosition(1, 1), ResourceType.IronOre);
            BuildingModel miner = CreateBuilding("miner", BuildingType.Miner, new GridPosition(1, 1));
            Assert.IsTrue(gridModel.TryPlaceBuilding(generator));
            Assert.IsTrue(gridModel.TryPlaceBuilding(miner));
            PowerStatusService service = CreateService(gridModel);

            Assert.AreEqual(BuildingOperationalStatus.Operating, service.GetStatusFor(miner));
            Assert.IsTrue(service.HasEnoughPower());
        }

        [Test]
        public void GetStatusFor_ReturnsNoPowerWhenPowerIsInsufficient()
        {
            GridModel gridModel = new GridModel(5, 5);
            gridModel.SetResourceNode(new GridPosition(1, 1), ResourceType.IronOre);
            BuildingModel miner = CreateBuilding("miner", BuildingType.Miner, new GridPosition(1, 1));
            Assert.IsTrue(gridModel.TryPlaceBuilding(miner));
            PowerStatusService service = CreateService(gridModel);

            Assert.AreEqual(BuildingOperationalStatus.NoPower, service.GetStatusFor(miner));
            Assert.IsFalse(service.HasEnoughPower());
        }

        [Test]
        public void GetStatusFor_ReturnsNotApplicableForZeroPowerBuilding()
        {
            GridModel gridModel = new GridModel(5, 5);
            BuildingModel conveyor = CreateBuilding("conveyor", BuildingType.Conveyor, new GridPosition(1, 1));
            Assert.IsTrue(gridModel.TryPlaceBuilding(conveyor));
            PowerStatusService service = CreateService(gridModel);

            Assert.AreEqual(BuildingOperationalStatus.NotApplicable, service.GetStatusFor(conveyor));
            Assert.IsFalse(service.IsPoweredBuilding(conveyor));
        }

        [Test]
        public void GetStatusFor_ReturnsNoneForNullBuilding()
        {
            PowerStatusService service = CreateService(new GridModel(5, 5));

            Assert.AreEqual(BuildingOperationalStatus.None, service.GetStatusFor(null));
            Assert.IsFalse(service.IsPoweredBuilding(null));
            Assert.IsFalse(service.IsPowerProducer(null));
            Assert.IsFalse(service.IsPowerConsumer(null));
        }

        [Test]
        public void GetPowerModel_IncludesResearchLabConsumption()
        {
            GridModel gridModel = new GridModel(6, 6);
            BuildingModel researchLab = CreateBuilding("research-lab", BuildingType.ResearchLab, new GridPosition(1, 1));
            Assert.IsTrue(gridModel.TryPlaceBuilding(researchLab));
            PowerStatusService service = CreateService(gridModel);

            PowerModel power = service.GetPowerModel();

            Assert.AreEqual(0, power.ProducedPower);
            Assert.AreEqual(3, power.ConsumedPower);
            Assert.AreEqual(BuildingOperationalStatus.NoPower, service.GetStatusFor(researchLab));
        }

        private static PowerStatusService CreateService(GridModel gridModel)
        {
            return new PowerStatusService(gridModel, new FactorySimulation(gridModel));
        }

        private static BuildingModel CreateBuilding(
            string instanceId,
            BuildingType type,
            GridPosition origin)
        {
            return new BuildingModel(
                instanceId,
                DebugBuildingDefinitions.Get(type),
                origin,
                BuildingDirection.North);
        }
    }
}
