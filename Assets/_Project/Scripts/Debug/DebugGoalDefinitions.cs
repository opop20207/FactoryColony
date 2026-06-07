namespace FactoryColony
{
    public static class DebugGoalDefinitions
    {
        public static ProductionGoalModel[] CreateGoals()
        {
            return new[]
            {
                new ProductionGoalModel("iron-plate-50", "IronPlate", ResourceType.IronPlate, 50),
                new ProductionGoalModel("copper-wire-30", "CopperWire", ResourceType.CopperWire, 30),
                new ProductionGoalModel("gear-5", "Gear", ResourceType.Gear, 5)
            };
        }
    }
}
