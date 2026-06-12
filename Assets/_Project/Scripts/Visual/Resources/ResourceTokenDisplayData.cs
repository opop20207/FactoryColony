namespace FactoryColony
{
    public readonly struct ResourceTokenDisplayData
    {
        public ResourceType Type { get; }
        public int Amount { get; }

        public ResourceTokenDisplayData(ResourceType type, int amount)
        {
            Type = type;
            Amount = amount;
        }
    }
}
