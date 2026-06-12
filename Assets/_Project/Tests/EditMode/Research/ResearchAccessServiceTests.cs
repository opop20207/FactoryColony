using System.Linq;
using FactoryColony;
using NUnit.Framework;

namespace FactoryColony.Tests.EditMode.Research
{
    public sealed class ResearchAccessServiceTests
    {
        [Test]
        public void HasResearchLab_ReturnsFalseWhenMissing()
        {
            ResearchAccessService service = new ResearchAccessService(new GridModel(5, 5));

            Assert.IsFalse(service.HasResearchLab());
            Assert.AreEqual(0, service.GetResearchLabs().Count);
        }

        [Test]
        public void HasResearchLab_ReturnsTrueWhenPlaced()
        {
            GridModel gridModel = new GridModel(5, 5);
            BuildingModel researchLab = CreateResearchLab(new GridPosition(1, 1));
            Assert.IsTrue(gridModel.TryPlaceBuilding(researchLab));
            ResearchAccessService service = new ResearchAccessService(gridModel);

            Assert.IsTrue(service.HasResearchLab());
            Assert.AreEqual(researchLab, service.GetResearchLabs().Single());
        }

        [Test]
        public void CanOpenResearch_ReturnsFalseWhenResearchLabIsMissing()
        {
            ResearchAccessService service = new ResearchAccessService(new GridModel(5, 5));

            bool canOpen = service.CanOpenResearch(out string message);

            Assert.IsFalse(canOpen);
            Assert.AreEqual("Build Research Lab first.", message);
        }

        [Test]
        public void CanOpenResearch_ReturnsTrueWhenResearchLabExists()
        {
            GridModel gridModel = new GridModel(5, 5);
            Assert.IsTrue(gridModel.TryPlaceBuilding(CreateResearchLab(new GridPosition(1, 1))));
            ResearchAccessService service = new ResearchAccessService(gridModel);

            bool canOpen = service.CanOpenResearch(out string message);

            Assert.IsTrue(canOpen);
            Assert.AreEqual("Research available.", message);
        }

        [Test]
        public void DebugResearchLabDefinition_IsValid()
        {
            BuildingDefinition definition = DebugBuildingDefinitions.Get(BuildingType.ResearchLab);

            Assert.AreEqual("ResearchLab", definition.Id);
            Assert.AreEqual(BuildingType.ResearchLab, definition.Type);
            Assert.AreEqual(2, definition.Width);
            Assert.AreEqual(2, definition.Height);
            Assert.AreEqual(3, definition.PowerConsumption);
            Assert.AreEqual(20, definition.BuildCost[ResourceType.IronPlate]);
            Assert.AreEqual(15, definition.BuildCost[ResourceType.CopperWire]);
            Assert.AreEqual(5, definition.BuildCost[ResourceType.Gear]);
            Assert.AreEqual(2, definition.BuildCost[ResourceType.BasicCircuit]);
        }

        [Test]
        public void ResearchLabPowerConsumption_IsIncludedInPowerModel()
        {
            GridModel gridModel = new GridModel(6, 6);
            Assert.IsTrue(gridModel.TryPlaceBuilding(CreateResearchLab(new GridPosition(1, 1))));
            FactorySimulation simulation = new FactorySimulation(gridModel);

            PowerModel power = simulation.CalculatePower();

            Assert.AreEqual(0, power.ProducedPower);
            Assert.AreEqual(3, power.ConsumedPower);
            Assert.IsFalse(power.HasEnoughPower);
        }

        private static BuildingModel CreateResearchLab(GridPosition origin)
        {
            return new BuildingModel(
                "research-lab",
                DebugBuildingDefinitions.Get(BuildingType.ResearchLab),
                origin,
                BuildingDirection.North);
        }
    }
}
