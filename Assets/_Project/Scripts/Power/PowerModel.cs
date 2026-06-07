using System;

namespace FactoryColony
{
    public sealed class PowerModel
    {
        public int ProducedPower { get; }
        public int ConsumedPower { get; }
        public bool HasEnoughPower
        {
            get { return ProducedPower >= ConsumedPower; }
        }

        public int AvailablePower
        {
            get { return ProducedPower - ConsumedPower; }
        }

        public PowerModel(int producedPower, int consumedPower)
        {
            if (producedPower < 0)
            {
                throw new ArgumentException("Produced power must not be negative.", nameof(producedPower));
            }

            if (consumedPower < 0)
            {
                throw new ArgumentException("Consumed power must not be negative.", nameof(consumedPower));
            }

            ProducedPower = producedPower;
            ConsumedPower = consumedPower;
        }
    }
}
