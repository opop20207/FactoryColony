namespace FactoryColony
{
    public sealed class PowerStatusService
    {
        private readonly GridModel _gridModel;
        private readonly FactorySimulation _simulation;

        public PowerStatusService(GridModel gridModel, FactorySimulation simulation)
        {
            _gridModel = gridModel;
            _simulation = simulation;
        }

        public PowerModel GetPowerModel()
        {
            if (_simulation != null)
            {
                return _simulation.CalculatePower();
            }

            int producedPower = 0;
            int consumedPower = 0;

            if (_gridModel != null)
            {
                foreach (BuildingModel building in _gridModel.GetAllBuildings())
                {
                    producedPower += building.Definition.PowerProduction;
                    consumedPower += building.Definition.PowerConsumption;
                }
            }

            return new PowerModel(producedPower, consumedPower);
        }

        public bool HasEnoughPower()
        {
            return GetPowerModel().HasEnoughPower;
        }

        public BuildingOperationalStatus GetStatusFor(BuildingModel building)
        {
            if (building == null)
            {
                return BuildingOperationalStatus.None;
            }

            if (IsPowerProducer(building))
            {
                return BuildingOperationalStatus.Operating;
            }

            if (IsPowerConsumer(building))
            {
                return HasEnoughPower()
                    ? BuildingOperationalStatus.Operating
                    : BuildingOperationalStatus.NoPower;
            }

            return BuildingOperationalStatus.NotApplicable;
        }

        public bool IsPoweredBuilding(BuildingModel building)
        {
            return IsPowerProducer(building) || IsPowerConsumer(building);
        }

        public bool IsPowerProducer(BuildingModel building)
        {
            return building != null && building.Definition.PowerProduction > 0;
        }

        public bool IsPowerConsumer(BuildingModel building)
        {
            return building != null && building.Definition.PowerConsumption > 0;
        }

        public int GetPowerProducerCount()
        {
            return CountBuildings(IsPowerProducer);
        }

        public int GetPowerConsumerCount()
        {
            return CountBuildings(IsPowerConsumer);
        }

        private int CountBuildings(System.Func<BuildingModel, bool> predicate)
        {
            if (_gridModel == null || predicate == null)
            {
                return 0;
            }

            int count = 0;

            foreach (BuildingModel building in _gridModel.GetAllBuildings())
            {
                if (predicate(building))
                {
                    count++;
                }
            }

            return count;
        }
    }
}
