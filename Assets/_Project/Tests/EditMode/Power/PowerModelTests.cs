using System;
using FactoryColony;
using NUnit.Framework;

namespace FactoryColony.Tests.EditMode.Power
{
    public sealed class PowerModelTests
    {
        [Test]
        public void HasEnoughPower_ReturnsTrueWhenProductionMeetsConsumption()
        {
            PowerModel power = new PowerModel(10, 10);

            Assert.IsTrue(power.HasEnoughPower);
        }

        [Test]
        public void HasEnoughPower_ReturnsFalseWhenConsumptionExceedsProduction()
        {
            PowerModel power = new PowerModel(9, 10);

            Assert.IsFalse(power.HasEnoughPower);
        }

        [Test]
        public void AvailablePower_ReturnsProductionMinusConsumption()
        {
            PowerModel power = new PowerModel(10, 4);

            Assert.AreEqual(6, power.AvailablePower);
        }

        [Test]
        public void Constructor_RejectsNegativeProducedPower()
        {
            Assert.Throws<ArgumentException>(() => new PowerModel(-1, 0));
        }

        [Test]
        public void Constructor_RejectsNegativeConsumedPower()
        {
            Assert.Throws<ArgumentException>(() => new PowerModel(0, -1));
        }
    }
}
